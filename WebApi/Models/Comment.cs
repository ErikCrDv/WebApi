﻿using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
