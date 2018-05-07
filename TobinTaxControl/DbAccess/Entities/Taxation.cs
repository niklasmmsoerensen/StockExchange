using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobinTaxControl.DbAccess.Entities
{
    public class Taxation
    {
        [Key]
        public int ID { get; set; }
        public int StockID  { get; set; }
        public int UserToTaxID { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxSum { get; set; }

    }
}
    