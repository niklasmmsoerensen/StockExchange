using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PublicShareOwnerControl.DbAccess.Entities
{
    public class Stock
    {
        [Key]
        public int StockID { get; set; }
        public int UserID { get; set; }
        public string StockName { get; set; }
    }
}
