using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment;

namespace SampleProject
{
    public class PayCode
    {
        static string clientId = "IKIAF6C068791F465D2A2AA1A3FE88343B9951BAC9C3";
        static string clientSecret = "FTbMeBD7MtkGBQJw1XoM74NaikuPL13Sxko1zb0DMjI=";
        static void Main(string[] args)
        {

            Interswitch.Init(clientId, clientSecret, "http://qa.interswitchng.com");
            var token = Interswitch.GetToken();
            Console.WriteLine(token);
            object data = new
            {                
                frontEndPartner="455",
                paymentMethodIdentifier="E192F3F3B3BA4596BC9704C44EA801BC",
                amount=500000,
                channel="ATM",
                macData="78e0ec06bdccacab629fbe725fc753364b8af4a15e3ebd1e5fd5c0d8b5e4d332",
                oneTimePin="1234",
                pinBlock="865d73eec7385017",
                secure="814caf1b0d2951c3454cade6a27d2bddd5546126fae7d39889d26a95dc573adbfdd8d60a286cea90f71916e1577b7dd1ad3d98436ecbec50206fc100adf93793715fd0be7926f5029bf068da7b7f22f13127ba304b0431a59360b45f989e313db0f35d1f15dc9867606fec20c7fe55d464b12c5d4c35030cdc82c8db1ceff718f78577b1a74a7f4b50cd90eb58f4eed96a903412ce1e8a7c89d0e5b559635941dbfd2204777a26a09d8f9a099867c779f0426cf1357ebb6966f872dedfd92d4520f711719d1c57bb74d982b20af0dc9c6d0b95ff4b677f587aab9c67409d3d453a04cf33fa3c55b58eb27898a827bffd8d88fd185fe5075561832f6e70316628",
                subscriberId="2348124888436",
                tokenLifeTimeInMinutes=1440,
                ttid=809,
                providerToken="LEBUCODE",
                transactionType="payment"

            };
            var paycodeTokens = Interswitch.Send("/cardless-service/api/v1/cardless-services/tokens", "POST", data).Result;

            Console.WriteLine(paycodeTokens);

            Console.WriteLine("########################################################");
            var paycodeConfig = Interswitch.Send("/cardless-service/api/v1/admin/config", "GET", null).Result;

            Console.WriteLine(paycodeConfig);
            Console.ReadKey();
        }        
    }
}
