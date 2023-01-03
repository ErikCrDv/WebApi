using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Filters;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    //[Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IService service;
        private readonly ServiceTransiet serviceTransiet;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<AuthorController> logger;

        public AuthorController(
            ApplicationDbContext dbContext, 
            IService service,
            ServiceTransiet serviceTransiet,
            ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton,
            ILogger<AuthorController> logger
            )
        {
            this.dbContext = dbContext;
            this.service = service;
            this.serviceTransiet = serviceTransiet;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MyActionFilter))]
        public ActionResult getGuid()
        {
            return Ok( new {
                AuthorControllerTransient = serviceTransiet.Guid,
                ServiceATransient = service.GetTransient(),
                AuthorControllerScoped = serviceScoped.Guid,
                ServiceAScoped = service.GetScoped(),
                AuthorControllerSingleton = serviceSingleton.Guid,
                ServiceASingleton = service.GetSingleton()
            });
        }


        [HttpGet] // Route Controller
        [HttpGet("list")] // Route Concat
        [HttpGet("/list")] // Route Action
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<Author>>> Get()
        {
            throw new NotImplementedException();
            logger.LogInformation("We're getting the Athors");
            //service.DoTask();
            return await dbContext.Authors
                .Include( author => author.Books )
                .ToListAsync();
        }

        [HttpGet("first")]
        public async Task<ActionResult<Author>> GetFisrt([FromHeader] int myValue, [FromQuery] string name)
        {
            return await dbContext.Authors.FirstOrDefaultAsync();
        }
        [HttpGet("firsttwo")]
        public ActionResult<Author> GetFisrtTwo()
        {
            return new Author() { Name = "Autor Sinc"};
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> Get(int id){
            var author = await dbContext.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if( author == null )
            {
                return NotFound(); 
            }

            return Ok(author);
        }

        [HttpGet("{name}/{lastname?}")]
        public async Task<ActionResult<Author>> Get( [FromRoute] string name)
        {
            var author = await dbContext.Authors.FirstOrDefaultAsync(author => author.Name.Contains(name));
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }


        [HttpPost]
        public async Task<ActionResult> Post( [FromBody] Author author)
        {
            var authorExists = await dbContext.Authors.AnyAsync( authorDb => authorDb.Name == author.Name );
            if (authorExists)
            {
                return BadRequest("There is already an Author with the same name");
            }
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
