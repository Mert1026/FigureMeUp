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
            return _context.Rarities
                .FirstOrDefault(r => r.Name == rarityName && !r.IsDeleted);
        }

        public static async Task<bool> IsValidImageUrlAsync(string url)
        {
        try
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                // Use HEAD request to avoid downloading the entire file
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

                if (!response.IsSuccessStatusCode)
                    return false;

                // Check if content type indicates an image
                var contentType = response.Content.Headers.ContentType?.MediaType;
                return contentType != null && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch
        {
            return false;
        }
        }
    }
}
