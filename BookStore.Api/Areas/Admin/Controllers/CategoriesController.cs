using BookStore.Api.DTOs.Request;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.Api.Areas.Admin.Controllers
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryrepository;

        public CategoriesController( IRepository<Category> categoryrepository)
        {
           
            _categoryrepository = categoryrepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories =await _categoryrepository.GetAsync(tracked:false , cancellationToken:cancellationToken);

            return Ok(categories.AsEnumerable());
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetOne(int id , CancellationToken cancellationToken)
        {
            var category = _categoryrepository.GetOneAsync(e => e.Id == id , tracked:false , cancellationToken:cancellationToken); 
            
            if(category is null)
                return NotFound(new
                {
                    msg = "Invalid Category"
                });
            return Ok(category);
        }

        [HttpPost("")]
        [Authorize(Roles = $"{SD.Super_Admin_Role} ,{SD.Admin_Role}")]
        public async Task<IActionResult> Create(CreateCategoryRequest createCategoryRequest , CancellationToken cancellationToken)
        {
            Category category = createCategoryRequest.Adapt<Category>();

           await _categoryrepository.AddAsync(category, cancellationToken);
           await _categoryrepository.CommitAsync(cancellationToken);

            return CreatedAtAction(nameof(GetOne), new { id = category.Id }, new
            {
                success_notifaction = "Create Category Successfully"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{SD.Super_Admin_Role} ,{SD.Admin_Role}")]
        public async Task<IActionResult> Edit(int id , UpdateCategoryRequest updateCategoryRequest ,CancellationToken cancellationToken)
        {
            var categoryInDb =await _categoryrepository.GetOneAsync(e => e.Id == id , tracked:false, cancellationToken:cancellationToken);

            if(categoryInDb is null)
                return NotFound();
             
            categoryInDb.Name = updateCategoryRequest.Name;
            categoryInDb.Description = updateCategoryRequest.Description;
            categoryInDb.Status = updateCategoryRequest.Status;

            await _categoryrepository.CommitAsync(cancellationToken);


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id ,CancellationToken cancellationToken)
        {
            var category =await _categoryrepository.GetOneAsync(e=>e.Id == id , cancellationToken: cancellationToken);

            if(category is null) 
                return NotFound();

            _categoryrepository.Delete(category);
           await _categoryrepository.CommitAsync(cancellationToken);
        
            return NoContent(); 
        }
    }
}
