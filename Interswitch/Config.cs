using System; 
using System.Text; 
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;

namespace Payment
{
    public class Config
    {
        //public string Authorizations;
        private string clientID;
        private string secretKey;
        private string HTTPVerb;
        private string url;
        private string accessToken;
        public string PostData { get; private set; }
        public string Nonce { get; private set; }
        public string PasportAuthorization { get; private set; }
        public string TimeStamp { get; set; }
        public string Authorization { get; private set; }
        public string Signature { get; set; }
        public static SecureRandom Random
        {
            get { return _random; }
            set { _random = value; }
        }


        private static SecureRandom _random = new SecureRandom();

        public long GetTimeStamp()
        {
            return (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public string GetAuthorization()
        {
            Authorization = Constants.Bearer + accessToken;
            return Authorization;
        }
        public string GetNonce()
        {
            var guid = Guid.NewGuid();
            var nonce = guid.ToString();
            nonce = nonce.Replace("-", "");
            return nonce;
        }

        public string GetSignature()
        {
            var signature = new StringBuilder(HTTPVerb);
            signature.Append("&")
                .Append(Uri.EscapeDataString(url))
                .Append("&")
                .Append(TimeStamp)
                .Append("&")
                .Append(Nonce)
                .Append("&")
                .Append(clientID)
                .Append("&")
                .Append(secretKey);
            return ComputeHash(signature.ToString());
        }

        public static string ComputeHash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            Sha1Digest hash = new Sha1Digest();
            hash.BlockUpdate(data, 0, data.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            return Convert.ToBase64String(result);
        }
        public Config(string httpVerb, string url, string clientId, string secretKey, string accessToken, string postData, string authorization)
        {
            HTTPVerb = httpVerb;
            this.url = url;
            clientID = clientId;
            this.secretKey = secretKey;
            this.accessToken = accessToken;
            PostData = postData;
            TimeStamp = GetTimeStamp().ToString();
            Nonce = GetNonce();
            Authorization = GetAuthorization();
            PasportAuthorization = authorization;
            Signature = GetSignature();
        }
    }
}
