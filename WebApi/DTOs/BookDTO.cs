using System.ComponentModel.DataAnnotations;
using WebApi.Validations;

namespace WebApi.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleseDate { get; set; }


        //public List<CommentDTO> Comments { get; set; }
    }
}
