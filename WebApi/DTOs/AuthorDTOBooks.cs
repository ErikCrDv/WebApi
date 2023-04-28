namespace WebApi.DTOs
{
    public class AuthorDTOBooks : AuthorDTO
    {
        public List<BookDTO> Books { get; set; }
    }
}
