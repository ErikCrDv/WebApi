using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/book/{bookId:int}/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentController(ApplicationDbContext applicationDbContext, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExists = await applicationDbContext.Books.AnyAsync(book => book.Id == bookId);
            if (!bookExists)
            {
                return NotFound();
            }

            var comments = await applicationDbContext.Comments
                .Where(comment => comment.BookId == bookId).ToListAsync();

            return Ok(mapper.Map<List<CommentDTO>>(comments));
        }

        [HttpGet("{id:int}", Name = "getCommentById")]
        public async Task<ActionResult<CommentDTO>> GetById(int id)
        {
            var comment = await applicationDbContext.Comments.FirstOrDefaultAsync(comment => comment.Id == id);
            if (comment == null) { return NotFound(); }

           return mapper.Map<CommentDTO>(comment);   
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where( claim => claim.Type == "email" ).FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var bookExists = await applicationDbContext.Books.AnyAsync( book => book.Id == bookId);
            if (!bookExists)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;
            comment.UserId = userId;

            applicationDbContext.Add(comment);
            await applicationDbContext.SaveChangesAsync();

            var commentDTO = mapper.Map<CommentDTO>(comment);   
            return CreatedAtRoute("getCommentById", new { id = comment.Id, bookId = bookId }, commentDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int bookId, int id, CommentCreateDTO commentCreateDTO)
        {
            var bookExists = await applicationDbContext.Books.AnyAsync(book => book.Id == bookId);
            if (!bookExists)
            {
                return NotFound();
            }

            var commentExists = await applicationDbContext.Comments.AnyAsync(commentDB => commentDB.Id == id);
            if (!commentExists)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comment>(commentCreateDTO);
            comment.Id = id;    
            comment.BookId = bookId;    

            applicationDbContext.Update(comment);
            await applicationDbContext.SaveChangesAsync(); 

            return NoContent(); 
        }

    }
}
