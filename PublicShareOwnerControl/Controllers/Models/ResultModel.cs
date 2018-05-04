using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicShareOwnerControl.Controllers.Models
{
    public class ResultModel<T>
    {
        public Result Result { get; set; }
        public string Error { get; set; }

        public T ReturnResult { get; set; }
    }

    public enum Result { Ok, Error }
    
}
