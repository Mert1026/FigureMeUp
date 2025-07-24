using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models.View_models
{
    public class FiguresViewModel
    {
        //Used as input model too.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> Hashtags { get; set; }
        public string PostedOn { get; set; }
        public string Rarity { get; set; }
        public string OwnerName { get; set; }
    }
}
