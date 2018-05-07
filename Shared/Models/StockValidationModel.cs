using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class StockValidationModel
    {
        public int StockID { get; set; }
        public int UserIdToCheck { get; set; }
    }
}
