using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Realtime
{
   public class Sales
    {
        public String updated { get; set; }
        public String seq { get; set; }
        public String bu { get; set; }
        public String sinterface { get; set; }

        public String channel { get; set; }

        public List<salesTicket> salesTicket { get; set; }
    }

    public class salesTicket
    {
        public String loc { get; set; }

        public String receiptDate { get; set; }

        public String ticketNo { get; set; }

        public String receivedNo { get; set; }

        public String customerNo { get; set; }

        public String customerName { get; set; }

        public List<products> products { get; set; }

        public List<payments> payments { get; set; }

        public String totalNetSaleAmt { get; set; }

        public String transType { get; set; }
    }

    public class products
    {
        public String barcode { get; set; }

        public String sku { get; set; }

        public String qty { get; set; }

      
    }

    public class payments
    {
        public String paymentType { get; set; }

        public string tenderCode { get; set; }
        public String netSaleAmt { get; set; }


    }
}
