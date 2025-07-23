using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.GCommon
{
    public class ApplicationConstants
    {
        public const string AppDateFormat = "yyyy-MM-dd";
        public const string NoImageUrl = "no-image.jpg";
        public const string IsDeletedPropertyName = "IsDeleted";
        public const string PriceSqlType = "decimal(18, 6)";
        public const string AccessDeniedPath = "/Home/AccessDenied";
        public const string ManagerAuthCookie = "ManagerAuth";
        public const string AllowAllDomainsPolicy = "AllowAllDomainsDebug";

        public const string UserRoleName = "User";
        public const string AdminRoleName = "Admin";
        public const string AdminAreaName = "Admin";
    }
}
