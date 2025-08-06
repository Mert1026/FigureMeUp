using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models.View_models
{
    public class HomeViewModel
    {
        public IEnumerable<Figure> RecentFigures { get; set; } = new List<Figure>();
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
    }
}
