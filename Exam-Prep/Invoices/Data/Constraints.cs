using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Data
{
    public static class Constraints
    {
        public const byte ProductNameMinLenght = 9;
        public const byte ProductNameMaxLenght = 30;
        //⦁	Price – decimal in range [5.00…1000.00] (required)
        public const string ProductPriceMinValue = "5.00";
        public const string ProductPriceMaxValue = "1000.00";

    }
}
