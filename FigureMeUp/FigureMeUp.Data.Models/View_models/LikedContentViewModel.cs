using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models.View_models
{
    public class LikedContentViewModel
    {
        public List<Figure> LikedFigures { get; set; } = new List<Figure>();
        public List<Post> LikedPosts { get; set; } = new List<Post>();
    }
}
