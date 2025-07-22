using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models
{
    public class Figure
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IEnumerable<string> ImageUrls { get; set; } = new List<string>();
        public int RarityId { get; set; }
        public Rarity Rarity { get; set; } = null!;
        public IdentityUser Owner { get; set; } = null!;
        public string OwnerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IEnumerable<UserFigures> UserFigures { get; set; } = new List<UserFigures>();
        public IEnumerable<Hashtag> Hashtags { get; set; } = new List<Hashtag>();
    }
}
