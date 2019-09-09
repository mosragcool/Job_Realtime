using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Realtime
{

    public class Incremental
    {
        public String updated { get; set; }
        public String seq { get; set; }
        public String bu { get; set; }
        public String sinterface { get; set; }

        public List<items> items { get; set; }

    }

    public class items
    {
        public String barcode { get; set; }
        public String sku { get; set; }
        public String status { get; set; }

        public String itemize { get; set; }
        public List<stocks> stocks { get; set; }
    }

    public class stocks
    {
        public String loc { get; set; }
        public String availStock { get; set; }
        public String reserveStock { get; set; }
    }
}
