using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public AuthorController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await dbContext.Authors
                .Include( author => author.Books )
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            dbContext.Add(author);
            await dbContext.SaveChangesAsync();
            return Ok(author);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Author author, int id)
        {
            if( author.Id != id)
            {
                return BadRequest("El ID del Author no coincide");
            }

            var existAuthor = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!existAuthor)
            {
                return NotFound();
            }
             
            dbContext.Update(author);
            await dbContext.SaveChangesAsync();

            return Ok(author);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existAuthor = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!existAuthor)
            {
                return NotFound();
            }

            dbContext.Remove(new Author() { Id = id });
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
