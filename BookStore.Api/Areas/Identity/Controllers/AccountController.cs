using BookStore.Api.DTOs.Request;
using BookStore.Api.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace BookStore.Api.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPrepositry;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepository<ApplicationUserOTP> applicationUserOTPrepositry)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _applicationUserOTPrepositry = applicationUserOTPrepositry;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistorRequest registorRequest)
        {
            // Create New User
            var user = new ApplicationUser() 
            {
            FirstName = registorRequest.FirstName,
            LastName = registorRequest.LastName,
            Email = registorRequest.Email,
            UserName = registorRequest.UserName,
            };

            var result =await _userManager.CreateAsync(user , registorRequest.Password);

            if (!result.Succeeded) 
            {
            return BadRequest(result.Errors);
            }

            // Send Comfirm Email
            var token =await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(ConfirmEmail),"Account" , new {area = "Identity",token , userId = user.Id },Request.Scheme);

           await _emailSender.SendEmailAsync(registorRequest.Email, "BookStore - Comfirm your email", $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

          await  _userManager.AddToRoleAsync(user, SD.Customer_Role);

            return Ok(new
            {
                msg = "Create Account Successfully"
            });
        }
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId , string token) 
        {
            var user =await _userManager.FindByIdAsync(userId);

            if(user is null)
            {
                return NotFound(new
                {
                    msg = "Invalid User"
                });
            }

            var result =await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                   msg = "Invalid or expired token"
                });
            }
            else
            {
                return Ok(new
                {
                    msg = "Email Confirmed Successfully"
                });
            }

                
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user =await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail) ??await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail);

            if(user is null)
            {
                return NotFound(new ErrorModelResponse
                {
                   Code = "Invalid Cred",
                   Description = "Invalid User Name / Email OR Password"
                });
            }

            var result =await _signInManager.PasswordSignInAsync(user ,loginRequest.Password,loginRequest.RememberMe,lockoutOnFailure:false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return BadRequest(new ErrorModelResponse
                    {
                        Code = "Too many attemps",
                        Description = "Too many attemps, try again after 5 min"
                    });
                else if (!user.EmailConfirmed)
                    return BadRequest(new ErrorModelResponse
                    {
                        Code = "Confirm Your Email",
                        Description = "Please Confirm Your Email First!!"
                    });
                else
                    return NotFound(new ErrorModelResponse
                    {
                        Code = "Invalid Cred.",
                        Description = "Invalid User Name / Email OR Password"
                    });
            }
            return Ok();
        }
        [HttpPost("ResendComfirmEmail")]
        public async Task<IActionResult> ResendComfirmEmail(ResendComfirmEmailRequset resendComfirmEmailRequset)
        {
            var user = await _userManager.FindByNameAsync(resendComfirmEmailRequset.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(resendComfirmEmailRequset.UserNameOrEmail);

            if (user is null)
            {
                return NotFound(new ErrorModelResponse
                {
                    Code = "Invalid Cred",
                    Description = "Invalid User Name / Email OR Password"
                });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new ErrorModelResponse
                {
                    Code = "Already Confirmed!!",
                    Description = "Already Confirmed!!"
                });
            }
            // send comfirm email 
            var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);


            await _emailSender.SendEmailAsync(user.Email!, "BookStore - Resend Comfirm your email", $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            return Ok(new
            {
                msg = "Send msg successfully"
            });
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            var user = await _userManager.FindByNameAsync(forgetPasswordRequest.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(forgetPasswordRequest.UserNameOrEmail);

            if (user is null)
            {
                return NotFound(new ErrorModelResponse
                {
                    Code = "Invalid Cred",
                    Description = "Invalid User Name / Email OR Password"
                });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new ErrorModelResponse
                {
                    Code = "Already Confirmed!!",
                    Description = "Already Confirmed!!"
                });
            }

            var userOTPs =await _applicationUserOTPrepositry.GetAsync(e => e.ApplicationUserId == user.Id);

            var totalOTP = userOTPs.Count(e => (DateTime.UtcNow - e!.CreateAt).TotalHours < 24);

            if (totalOTP > 24)
            {
                BadRequest(new ErrorModelResponse
                {
                    Code = "Too Many Attemp",
                    Description = "Too many attemps, try again later"
                });
            }

            var otp = new Random().Next(1000, 9999).ToString();

            await _applicationUserOTPrepositry.AddAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                IsValid = true,
                OTP = otp,
                ValidTO = DateTime.UtcNow.AddDays(1),
            });

           await _applicationUserOTPrepositry.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email!, "BookStore - Reset your password", $"<h1>Use This OTP: {otp} To Reset Your Account. Don't share it.</h1>");

            return CreatedAtAction("ValidateOTP" , new
            {
                userId = user.Id,
            });
        }

        public async Task<IActionResult> ValidateOTP(ValidateOTPRequset validateOTPRequset)
        {
            var result = await _applicationUserOTPrepositry.GetOneAsync(e => e.ApplicationUserId == validateOTPRequset.ApplicationUserId && e.OTP == validateOTPRequset.OTP && e.IsValid);

            if (result is null)
            {
                return CreatedAtAction("ValidateOTP", new { userId = validateOTPRequset.ApplicationUserId });

            }

            return CreatedAtAction("ValidateOTP", new { userId = validateOTPRequset.ApplicationUserId });
        }

        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordRequset newPasswordRequset)
        {
            var user = await _userManager.FindByIdAsync(newPasswordRequset.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new ErrorModelResponse
                {
                    Code = "Invalid Cred",
                    Description = "Invalid User Name / Email OR Password"
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordRequset.Password);

            if (!result.Succeeded)
            { 
                return BadRequest(result.Errors);
            }
            return Ok();
        }

    }
}
