using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public BookController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            return await dbContext.Books
                .Include( book => book.Author )
                .FirstOrDefaultAsync( book => book.Id == id );
        }

        [HttpPost]
        public async Task<ActionResult> Post(Book book)
        {
            var existsAuthor = await dbContext.Authors.AnyAsync(author => author.Id == book.AuthorId);
            if (!existsAuthor) 
            {
                return BadRequest($"No existe un Autor con el id: {book.AuthorId}");
            }

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync(); 
            return Ok(book);    

        }
    }
}
