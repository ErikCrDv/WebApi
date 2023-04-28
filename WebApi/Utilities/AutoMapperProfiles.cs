using AutoMapper;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //
            CreateMap<AuthorCreateDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorDTOBooks>()
                .ForMember( autor => autor.Books, options => options.MapFrom(MapAuthorBooks)); 
            //
            CreateMap<BookCreateDTO, Book>()
                .ForMember( book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookDTO>();
            CreateMap<Book, BookDTOAuthors>()
                .ForMember( book => book.Authors, options => options.MapFrom(MapBookAuthors) );
            CreateMap<BookPatchDTO, Book>().ReverseMap();
            //
            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
            //

        }
        
        private List<BookDTO> MapAuthorBooks( Author author, AuthorDTO authorDTO)
        {
            var result = new List<BookDTO>();

            if( author.AuthorsBooks == null) { return result; }
            foreach (var authorBooks in author.AuthorsBooks )
            {
                result.Add( new BookDTO()
                {
                    Id = authorBooks.BookId,
                     Title = authorBooks.Book.Title
                });
            }

            return result;
        }

        private List<AuthorBook> MapAuthorsBooks(BookCreateDTO bookCreateDTO, Book book)
        {
            var result = new List<AuthorBook>();

            if (bookCreateDTO.AuthorsId == null) { return result; }

            foreach (var authorId in bookCreateDTO.AuthorsId)
            {
                result.Add(new AuthorBook() { AuthorId = authorId });
            }

            return result;
        }

        private List<AuthorDTO> MapBookAuthors( Book book, BookDTO bookDTO)
        {
            var result = new List<AuthorDTO>();

            if(book.AuthorsBooks == null) { return result; }

            foreach (var autorBook in book.AuthorsBooks)
            {
                result.Add( new AuthorDTO()
                {
                    Id = autorBook.AuthorId,
                    Name = autorBook.Author.Name
                });
            }

            return result;
        }
    }


}
