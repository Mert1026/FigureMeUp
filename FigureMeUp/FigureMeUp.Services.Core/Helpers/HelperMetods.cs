using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Helpers
{
   
    public class HelperMetods
    {
        private ApplicationDbContext _context;

        public HelperMetods(ApplicationDbContext context)
        {
            this._context = context;
        }

        public List<Hashtag> HashtagsConversion(List<string> hashtags)
        {
            List<Hashtag> result = new List<Hashtag>();
            foreach(string h in hashtags)
            {
                result.Add(new Hashtag
                {
                    Name = h,
                    IsDeleted = false
                });
            }
            return result;
        }

        public Rarity? RarityValidation(string rarityName)
        {
            Rarity? result = this._context.Rarities.FirstOrDefault(r => r.Name == rarityName);
            return result;
        }
    }
}
