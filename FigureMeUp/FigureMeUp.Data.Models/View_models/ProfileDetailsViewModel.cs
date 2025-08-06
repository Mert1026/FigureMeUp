using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models.View_models
{
    public class ProfileDetailsViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int FiguresCount { get; set; }
        public int PostsCount { get; set; }
        public int LikedContentCount { get; set; } = 0;

    }
}
