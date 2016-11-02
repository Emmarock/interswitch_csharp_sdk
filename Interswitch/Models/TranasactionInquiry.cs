using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Models
{
   public class TranasactionInquiry
    {


        public string TransactionRef { get; set; }
        public string ShortTransactionRef { get; set; }
        public string Biller { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentItem { get; set; }
        public string Narration { get; set; }
        public string Amount { get; set; }
        public string IsAmountFixed { get; set; }
        public string CollectionsAccountNumber { get; set; }
        public string CollectionsAccountType { get; set; }
        public string Surcharge { get; set; }
        public string AlternateCustomerId { get; set; }
        public string PaymentItemId { get; set; }
        public string ResponseCode { get; set; }

    }
}
