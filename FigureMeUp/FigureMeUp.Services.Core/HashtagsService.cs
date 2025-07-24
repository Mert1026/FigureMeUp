using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core
{
    public class HashtagsService : IHashtagService
    {
        private readonly IHashtagsRepository _hashtagsRepository;

        public HashtagsService(IHashtagsRepository hashtagsRepository)
        {
            this._hashtagsRepository = hashtagsRepository;
        }

        public async Task<bool> CreateHashtagAsync(Hashtag hashtag)
        {
            await _hashtagsRepository.AddAsync(hashtag);
            return true;
        }

        public async Task<bool> DeleteHashtagWithIdAsync(int id)
        {
            bool OpResult = false;
            Hashtag? hashtagToDelete = await GetHashtagByIdAsync(id);
            if (hashtagToDelete != null)
            {
                OpResult = await _hashtagsRepository.DeleteAsync(hashtagToDelete);
            }
            return OpResult;
        }
        public async Task<bool> DeleteHashtagWithNameAsync(string name)
        {
            bool OpResult = false;
            Hashtag? hashtagToDelete = await GetHashtagByNameAsync(name);
            if (hashtagToDelete != null)
            {
                OpResult = await _hashtagsRepository.DeleteAsync(hashtagToDelete);
            }
            return OpResult;
        }


        //##############################################################
        public async Task<Hashtag?> GetHashtagByIdAsync(int id)
        {
            Hashtag? hashtag = await _hashtagsRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(h => h.Id == id);

            return hashtag;
        }


        public async Task<Hashtag?> GetHashtagByNameAsync(string name)
        {
            Hashtag? hashtag = await _hashtagsRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(h => h.Name == name);

            return hashtag;
        }

    }
}
