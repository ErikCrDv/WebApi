using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public BookController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "getBookById")]
        public async Task<ActionResult<BookDTOAuthors>> Get(int id)
        {
            var book = await dbContext.Books
                .Include(bookDB => bookDB.AuthorsBooks)
                .ThenInclude( authorBookDb => authorBookDb.Author )
                .FirstOrDefaultAsync( book => book.Id == id);

            if(book == null) { return NotFound(); }

            book.AuthorsBooks = book.AuthorsBooks.OrderBy( book => book.Order ).ToList();

            return Ok(mapper.Map<BookDTOAuthors>(book));
        }

        [HttpPost]
        public async Task<ActionResult> Post(BookCreateDTO bookCreateDTO)
        {
            if( bookCreateDTO.AuthorsId == null)
            {
                return BadRequest("No se puede crear un libro sin autor");
            }

            var authorsId = await dbContext.Authors
                .Where(author => bookCreateDTO.AuthorsId.Contains(author.Id))
                .Select(author => author.Id).ToListAsync();

            if (bookCreateDTO.AuthorsId.Count != authorsId.Count)
            {
                return BadRequest($"No existe un Autor con alguno de los ids proporcionados");
            }

            var book = mapper.Map<Book>(bookCreateDTO);
            SetOrderAuthor(book);

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            var bookDTO = mapper.Map<BookDTO>(book);
            return CreatedAtRoute("getBookById", new { id = book.Id }, bookDTO);

        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, BookCreateDTO bookCreateDTO )
        {
            var bookDB = await dbContext.Books
                .Include(book => book.AuthorsBooks)
                .FirstOrDefaultAsync(book => book.Id == id );

            if (bookDB == null) { return NotFound(); }


            bookDB = mapper.Map(bookCreateDTO, bookDB);
            SetOrderAuthor(bookDB);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> bookPatchDocument)
        {
            if(bookPatchDocument== null) { return BadRequest(); }

            var bookDB = await dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);  
            if (bookDB == null) { return NotFound(bookDB); }

            var bookDTO = mapper.Map<BookPatchDTO>( bookDB );
            bookPatchDocument.ApplyTo( bookDTO, ModelState );

            var isValid = TryValidateModel( bookDTO );  
            if (!isValid) 
            {
                return BadRequest( ModelState );
            }

            mapper.Map(bookDTO, bookDB);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existBook = await dbContext.Books.AnyAsync(author => author.Id == id);

            if (!existBook)
            {
                return NotFound();
            }

            dbContext.Remove(new Book() { Id = id });
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private void SetOrderAuthor(Book book)
        {
            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }



    }
}
