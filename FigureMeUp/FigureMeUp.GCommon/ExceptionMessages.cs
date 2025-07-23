using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.GCommon
{
    public static class ExceptionMessages
    {
        public const string SoftDeleteOnNonSoftDeletableEntity =
     "Soft Delete can't be performed on an Entity which does not support it!";

        public const string InterfaceNotFoundMessage =
            "Interface {0} not found for class {1}. Make sure the interface is named correctly and follows the convention 'I{ClassName}'.";
    }
}
