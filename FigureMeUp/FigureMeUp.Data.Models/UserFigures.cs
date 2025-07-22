using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Models
{
    public class UserFigures
    {
        public IdentityUser User { get; set; } = null!;
        public string UserId { get; set; }

        public Figure Figure { get; set; } = null!;
        public int FigureId { get; set; }

    }
}
