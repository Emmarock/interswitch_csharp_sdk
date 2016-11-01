using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment;
using Newtonsoft.Json.Linq;

namespace SampleProject
{
    public class QuickTeller
    {
        static List<KeyValuePair<string, string>>  hashMapSecure, hashMap;
        static string clientId = "IKIAF6C068791F465D2A2AA1A3FE88343B9951BAC9C3";
        static string clientSecret = "FTbMeBD7MtkGBQJw1XoM74NaikuPL13Sxko1zb0DMjI=";
        static void Main(string[] args)
        {
            Interswitch.Init(clientId, clientSecret);
            var token = Interswitch.GetToken();
            Console.WriteLine(token);
            #region
            Console.WriteLine("************ Secure Data Starts here ****************");
            string[] secureData = Interswitch.GetSecureData("2347012345678", "1436", "6280511000000095", "1111", "111", "5004", "GTB00199212");
            Console.WriteLine(secureData[0]);
            Console.WriteLine(secureData[1]);

            hashMapSecure = new List<KeyValuePair<string, string>>();
            hashMapSecure.AddRange(new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
                new KeyValuePair<string, string>("SignatureMethod", "SHA1"),
                new KeyValuePair<string, string>("terminalId", "3IWP0001")}
                );
            object doEnquiry = new
            {
                PaymentCode = "10803",
                CustomerId = "08124888436",
                CustomerEmail = "iswtester2@yahoo.com",
                CustomerMobile = "2348056731575",
                PageFlowValues = "",
                TerminalId = "3IWP0076"
            };

            hashMap = new List<KeyValuePair<string, string>>();
            hashMap.AddRange(new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
                new KeyValuePair<string, string>("SignatureMethod", "SHA1"),
                new KeyValuePair<string, string>("terminalId", "3IWP0001")}
                );

            Console.WriteLine("Do Enquiry ....");
            var enquiryResponse = Interswitch.Send("/api/v1/quickteller/billers", "GET", doEnquiry, token, hashMapSecure).Result;
            JObject json = JObject.Parse(enquiryResponse);
            Console.WriteLine(json.GetValue("TransactionRef"));
            Console.WriteLine(enquiryResponse);
            Console.WriteLine("Do Enquiry done ....");


            #region
            object securePayment = new
            {
                bankCbnCode = "058",
                amount = "100",
                cardBin = "53998316306",
                Msisdn = "2348052678744",
                TransactionRef = "IWP|T|Web|3IWP0001|QTFT|261016185145|00000041",
                terminalId = "3PBL0001",
                PinData = secureData[0],
                SecureData = secureData[1]
            };
            
            Console.WriteLine("****************************");
            Console.WriteLine("Send Payment with secure ....");
            var secureResponse = Interswitch.Send("/api/v1/quickteller/categorys", "GET", securePayment, token, hashMap).Result;
            Console.WriteLine(secureResponse);
            Console.WriteLine("Send Payment with secure done ....");
            #endregion

            Console.WriteLine("************ Secure Data Ends here ****************");
            #endregion
            Console.ReadKey();
        }
    }
}
