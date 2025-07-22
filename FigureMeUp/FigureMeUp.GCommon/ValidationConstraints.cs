using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.GCommon
{
    public class ValidationConstraints
    {

        public class PostConstraints
        {
            public const int TitleMaxLength = 80;
            public const int ContentMaxLength = 700;
            public const int ImageUrlsMaxCount = 10;
            public const int HashtagsMaxCount = 10;
        }

        public class HashtagConstraints
        {
            public const int NameMaxLength = 15;
        }   

        public class FigureConstraints
        {
            public const int NameMaxLength = 50;
            public const int DescriptionMaxLength = 500;
            public const int ImageUrlsMaxCount = 5;
            public const int HashtagsMaxCount = 5;
        }

        public class RarityConstraints
        {
            public const int NameMaxLength = 30;
            public const int DescriptionMaxLength = 200;
        }   

    }
}
