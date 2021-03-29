using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Respone
    {
        public int RequestId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public object Data {get;set;}
    }
}
