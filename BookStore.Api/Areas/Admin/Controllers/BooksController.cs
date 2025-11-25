using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.Api.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Category> _categryRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(ApplicationDBContext context, IRepository<Book> bookRepository, IRepository<Category> categryRepository,ILogger<BooksController> logger)
        {
            _context = context;
            _bookRepository = bookRepository;
            _categryRepository = categryRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll( FilterBookReaquest filterBookReaquest, CancellationToken cancellationToken, [FromQuery] int page = 1)
        {
            const decimal discount = 50;
            var books = await _bookRepository.GetAsync(includes: [e => e.Category!], tracked: false, cancellationToken: cancellationToken);



            #region Filtter Book
            // Add Filtter
            FilterBookResponse filterBookResponse = new();

            if (filterBookReaquest.name is not null)
            {
                books = books.Where(e => e!.Title.Contains(filterBookReaquest.name));
                filterBookResponse.Name = filterBookResponse.Name;
            }

            if (filterBookReaquest.MainPrice is not null)
            {
                books = books.Where(e => e!.Price - e.Price * e.Discont / 100 > filterBookReaquest.MainPrice);
                filterBookResponse.MainPrice = filterBookReaquest.MainPrice;
            }

            if (filterBookReaquest.MaxPrice is not null)
            {
                books = books.Where(e => e!.Price - e.Price * e.Discont / 100 < filterBookReaquest.MaxPrice);
                filterBookResponse.MaxPrice = filterBookReaquest.MaxPrice;
            }

            if (filterBookReaquest.categoryId is not null)
            {
                books = books.Where(e => e!.CategoryId == filterBookReaquest.categoryId);
                filterBookResponse.CategoryId = filterBookReaquest.categoryId;
            }

            if (filterBookReaquest.LessQuantity)
            {
                books = books.OrderBy(e => e!.Quantity);
                filterBookResponse.LessQuantity = filterBookReaquest.LessQuantity;
            }
            #endregion

            #region pagination

            PaginationResponse paginationResponse = new();

            paginationResponse.TotalPages = Math.Ceiling(books.Count() / 8.0);
            paginationResponse.CurrentPage = page;
            books = books.Skip((page - 1) * 8).Take(8);
            #endregion

            return Ok(new
            {
                Books = books.AsEnumerable(),
                PaginationResponse = paginationResponse,
                FilterBookResponse = filterBookResponse
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id , CancellationToken cancellationToken)
        {
            var book =await _bookRepository.GetOneAsync(e => e.Id == id , tracked:false , cancellationToken:cancellationToken);

            if(book is null)    
                return NotFound( new
                {
                    msg = "Invalid Book"
                });
            return Ok(book);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Super_Admin_Role} ,{SD.Admin_Role}")]
        public async Task<IActionResult> Create(CreateBookRequest createBookRequest , CancellationToken cancellationToken)
        {
            var transation = _context.Database.BeginTransaction();

            Book book = createBookRequest.Adapt<Book>();

            try
            {
                // save img 
              if(createBookRequest.img is not null && createBookRequest.img.Length > 0)
                {
                    var fillName = Guid.NewGuid().ToString() + Path.GetExtension(createBookRequest.img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Book_img", fillName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await createBookRequest.img.CopyToAsync(stream);
                    }
                    book.Image = fillName;
                }
              //save in db
              await _bookRepository.AddAsync(book, cancellationToken :cancellationToken);

                await _bookRepository.CommitAsync(cancellationToken);

                transation.Commit();
                return CreatedAtAction(nameof(GetOne), new { id = book.Id }, new
                {
                    success_notifaction = "Add Book Successfully"
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                transation.Rollback();

                return BadRequest(new ErrorModelResponse
                {
                    Code = "Error While Saving the Book",
                    Description = ex.Message,
                });
            }
           
        }
        [HttpPut("{id}")]
        [Authorize(Roles = $"{SD.Super_Admin_Role} ,{SD.Admin_Role}")]
        public async Task<IActionResult> Edit(int id,UpdateBookRequest updateBookRequest , CancellationToken cancellationToken)
        {
            var bookInDB =await _bookRepository.GetOneAsync(e => e.Id == id , cancellationToken:cancellationToken );
            if(bookInDB is null)
                return NotFound(new
                {
                    msg ="invalid Book"
                });
            // update img 

            if(updateBookRequest.img is not null && updateBookRequest.img.Length > 0)
            {
                var fillName = Guid.NewGuid().ToString() + Path.GetExtension(updateBookRequest.img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Book_img", fillName);

                using (var stream = System.IO.File.Create(filePath)) 
                { 
               await updateBookRequest.img.CopyToAsync(stream);
                }

                //delete old data

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Book_img" ,bookInDB.Image!);
                if(System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                bookInDB.Image = fillName;
            }
            bookInDB.Title = updateBookRequest.Title;
            bookInDB.IsAvailable = updateBookRequest.IsAvailable;
            bookInDB.Description = updateBookRequest.Description;
            bookInDB.Quantity = updateBookRequest.Quantity;
            bookInDB.Discont = updateBookRequest.Discont;
            bookInDB.Author = updateBookRequest.Author;
            bookInDB.Price = updateBookRequest.Price;
            bookInDB.CreatedAt = updateBookRequest.CreatedAt;
            bookInDB.CategoryId = updateBookRequest.CategoryId;

            _bookRepository.Update(bookInDB);
            await _bookRepository.CommitAsync(cancellationToken);

            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.Super_Admin_Role} ,{SD.Admin_Role}")]
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {
            var book =await _bookRepository.GetOneAsync(e=>e.Id == id , cancellationToken:cancellationToken );

            if (book is null)
                return NotFound(new
                {
                    msg = "Invalid book"
                });
            // delete oldpath
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Book_img", book.Image!);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _bookRepository.Delete(book);
          await  _bookRepository.CommitAsync(cancellationToken);
            return NoContent();
        }

    }
}
