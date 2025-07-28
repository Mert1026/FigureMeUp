using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IEnumerable<string> ImageUrls { get; set; } = new List<string>();
        public IEnumerable<Hashtag> Hashtags { get; set; } = new List<Hashtag>();
        public string PublisherId { get; set; } = string.Empty;
        public IdentityUser Publisher { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
