using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Portable;
using RestSharp.Portable.Deserializers;
using System.Net;
using System.Net.Http;
using Payment.Models;

namespace Payment
{

    public class Interswitch
    {
        public static string ClientId;
        public static string ClientSecret;
        public static string MyAccessToken;
        public static string Environment;
        public static string AuthData;

        public static String GetAuthData(string pan, string pin, string expiryDate, string cvv)
        {
            AuthData = Crypto.GetAuthData(pan, pin, expiryDate, cvv);
            return AuthData;
        }
        public static string GetAuthData(string mod, string pubExpo, string pan, string pin, string expiryDate, string cvv)
        {
            AuthData = Crypto.GetAuthData(mod, pubExpo, pan, pin, expiryDate, cvv);
            return AuthData;
        }
        public static string GetAuthData(string certificatePath, string pan, string pin, string expiryDate, string cvv2)
        {
            AuthData = Crypto.GetAuthData(certificatePath, pan, pin, expiryDate, cvv2);
            return AuthData;
        }

        public static string[] GetSecureData(string subscriberId, string ttid, string pan, string pin, string cvv2, string expiryDate, string paymentMethodTypeCode)
        {
            Byte[] pinKeyBytes = InterswitchSecure.DESUtils.generateKey();
            String pinKeyHex = InterswitchSecure.HexConverter.ByteArrayToString(pinKeyBytes);
            String[] result = InterswitchSecure.Secure.createPaymentMethod(subscriberId,ttid,pan,expiryDate, pinKeyHex);
            return result;           
        }





        //public static String GetSecureData(string pan, string pin, string expiryDate, string cvv)
        //{
        //    AuthData = Crypto.GetAuthData(pan, pin, expiryDate, cvv);
        //    return AuthData;
        //}
        
 
        //public static string GetSecureData(string pan, string pin, string expiryDate, string cvv2, string certificatePath)
        //{
        //    AuthData = Crypto.GetAuthData(certificatePath, pan, pin, expiryDate, cvv2);
        //    return AuthData;
        //}
        //public static string GetSecureData(string pan, string pin, string expiryDate, string cvv, string mod, string pubExpo, string additionalData = null)
        //{
        //    AuthData = Crypto.GetAuthData(mod, pubExpo, pan, pin, expiryDate, cvv);
        //    return AuthData;
        //}



        public static string GetToken()
        {
            var accessToken = GetClientAccessToken(ClientId, ClientSecret).Result;
            return accessToken.access_token;
        }
       

        public static async Task<Token> GetClientAccessToken(string clientId, string clientSecret)
        {
            var url = string.Concat(Environment, "/passport/oauth/token");
            var client = new RestClient(url);
            client.IgnoreResponseStatusCode = true;

            var request = new RestRequest(url, HttpMethod.Post);
            request.AddHeader(Constants.ContentType, "application/x-www-form-urlencoded");
            request.AddHeader(Constants.Authorization, SetAuthorization(clientId, clientSecret));
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            request.AddParameter("Scope", "profile", ParameterType.GetOrPost);

            JsonDeserializer deserial = new JsonDeserializer();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response;
            try
            {
                response = await client.Execute(request);
            }
            catch (Exception exception)
            {
                return new Token { ErrorMessage = exception.Message, ErrorCode = HttpStatusCode.BadRequest.ToString() };
            }
            var httpStatusCode = response.StatusCode;
            var numericStatusCode = (int)httpStatusCode;
            var passportResponse = new Token();
            if (numericStatusCode == (int)HttpStatusCode.OK)
            {
                passportResponse = deserial.Deserialize<Token>(response);

                passportResponse.setAccessToken(passportResponse.access_token);
            }
            else if (response.ContentType == "text/html" || (numericStatusCode == 401 || numericStatusCode == 404 || numericStatusCode == 502 || numericStatusCode == 504))
            {
                passportResponse.ErrorCode = numericStatusCode.ToString();
                passportResponse.ErrorMessage = response.StatusDescription;
            }
            else
            {
                var errorResponse = deserial.Deserialize<ErrorResponse>(response);
                passportResponse.ErrorCode = errorResponse.error.code;
                passportResponse.ErrorMessage = errorResponse.error.message;
            }
            return passportResponse;
        }
        /// <summary>
        /// Send Payment
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <param name="hashMap"></param>
        /// <param name="signedParameters"></param>
        /// <returns></returns>
        public static async Task<string> Send(string uri, string httpMethod, object data, string token = null, List<KeyValuePair<string, string>> hashMap = null, string signedParameters = null)
        {
            try
            {
                var url = string.Concat(Environment, uri);
                var client = new RestClient(url);
                client.IgnoreResponseStatusCode = true;
                var authConfig = string.IsNullOrEmpty(token)
                    ? new Config(httpMethod, url, ClientId, ClientSecret, MyAccessToken, "", "")
                    : new Config(httpMethod, url, ClientId, ClientSecret, token, "", "");
                RestRequest paymentRequests =null;
                if (httpMethod.Equals("POST"))
                {
                     paymentRequests = new RestRequest(url, HttpMethod.Post);
                }
                if (httpMethod.Equals("GET"))
                {
                    paymentRequests = new RestRequest(url, HttpMethod.Get);
                }                    
                #region -- Add Headers --
                paymentRequests.AddHeader("Signature", authConfig.Signature);
                paymentRequests.AddHeader("Timestamp", authConfig.TimeStamp);
                paymentRequests.AddHeader("Nonce", authConfig.Nonce);
                paymentRequests.AddHeader("Authorization", authConfig.Authorization);

                if (hashMap != null)
                {
                    foreach (var keyValue in hashMap)
                    {
                        paymentRequests.AddHeader(keyValue.Key, keyValue.Value);
                    }
                }
                else
                {
                    paymentRequests.AddHeader(Constants.ContentType, "application/json");
                    paymentRequests.AddHeader("SignatureMethod", "SHA1");
                }
                #endregion
                if(data != null)
                    paymentRequests.AddJsonBody(data);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var response = await client.Execute(paymentRequests);
                var httpStatusCode = response.StatusCode;
                var numericStatusCode = (int)httpStatusCode;
                if (numericStatusCode == 200 || numericStatusCode == 202)
                {
                    return Encoding.UTF8.GetString(response.RawBytes);
                }

                return Encoding.UTF8.GetString(response.RawBytes);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }
        public static string SetAuthorization(string clientId, string clientSecret)
        {
            try
            {
                String Auth;
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", clientId, clientSecret));
                Auth = Convert.ToBase64String(bytes);
                return String.Concat("Basic ", Auth);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to encode parameters, Please contact connect@interswitchng.com. for assistance.", e);
            }
        }

        public static void Init(string clientId, string clientSecret, string environment = "https://sandbox.interswitchng.com")
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Environment = environment;
            MyAccessToken = GetToken();
        }
    }

    public class Error1
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Error2
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ErrorResponse
    {
        public List<Error1> errors { get; set; }
        public Error2 error { get; set; }
        public string transactionRef { get; set; }
    }
}
