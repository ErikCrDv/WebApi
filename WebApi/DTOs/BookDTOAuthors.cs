namespace WebApi.DTOs
{
    public class BookDTOAuthors : BookDTO
    {
        public List<AuthorDTO> Authors { get; set; }
    }
}
