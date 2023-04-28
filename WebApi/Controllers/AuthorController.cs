using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        //private readonly ILogger<AuthorController> logger;

        public AuthorController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IConfiguration configuration
            )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.configuration = configuration;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            var authors = await dbContext.Authors.ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthorById")]
        public async Task<ActionResult<AuthorDTOBooks>> Get(int id){
            var author = await dbContext.Authors
                .Include( authorDB => authorDB.AuthorsBooks )
                .ThenInclude( authorBooksDB => authorBooksDB.Book )
                .FirstOrDefaultAsync(author => author.Id == id);

            if( author == null )
            {
                return NotFound(); 
            }

            return Ok(mapper.Map<AuthorDTOBooks>(author));
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> Get( [FromRoute] string name)
        {
            var authors = await dbContext.Authors.Where(author => author.Name.Contains(name)).ToListAsync();
            return Ok(mapper.Map<List<AuthorDTO>>(authors));
        }


        [HttpPost]
        public async Task<ActionResult> Post( [FromBody] AuthorCreateDTO authorCreateDTO )
        {
            var authorExists = await dbContext.Authors
                .AnyAsync( authorDb => authorDb.Name == authorCreateDTO.Name );

            if (authorExists)
            {
                return BadRequest("There is already an Author with the same name");
            }

            var author = mapper.Map<Author>(authorCreateDTO);

            dbContext.Add(author);
            await dbContext.SaveChangesAsync();

            var authorDTO = mapper.Map<AuthorDTO>( author );
            return CreatedAtRoute("getAuthorById", new { id = author.Id }, authorDTO );
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(AuthorCreateDTO authorCreateDTO, int id)
        {
            var existAuthor = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!existAuthor)
            {
                return NotFound();
            }

            var author = mapper.Map<Author>(authorCreateDTO);
            author.Id = id; 
             
            dbContext.Update(author);
            await dbContext.SaveChangesAsync();

            return NoContent();
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
