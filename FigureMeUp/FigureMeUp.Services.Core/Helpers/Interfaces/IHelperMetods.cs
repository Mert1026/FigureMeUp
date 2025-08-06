using FigureMeUp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Helpers.Interfaces
{
    public interface IHelperMetods
    {
        List<Hashtag> HashtagsConversion(List<string> hashtags);
        Rarity RarityValidation(string rarity);
    }
}
