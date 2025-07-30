using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models.View_models
{
    public class PostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<string>? Hashtags { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string PostedOn { get; set; } = DateTime.Now.ToString();
    }
}
