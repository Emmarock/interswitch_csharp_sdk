using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Payment;
using Newtonsoft.Json;
namespace SampleProject
{
    public class Example
    {
        //static string clientId = "IKIAF8F70479A6902D4BFF4E443EBF15D1D6CB19E232";
        //static string clientSecret = "ugsmiXPXOOvks9MR7+IFHSQSdk8ZzvwQMGvd0GJva30=";

        static string clientId = "IKIA9614B82064D632E9B6418DF358A6A4AEA84D7218";
        static string clientSecret = "XCTiBtLy1G9chAnyg0z3BcaFK4cVpwDg/GTw2EmjTZ8=";

        const string mod = "9C7B3BA621A26C4B02F48CFC07EF6EE0AED8E12B4BD11C5CC0ABF80D5206BE69E1891E60FC88E2D565E2FABE4D0CF630E318A6C721C3DED718D0C530CDF050387AD0A30A336899BBDA877D0EC7C7C3FFE693988BFAE0FFBAB71B25468C7814924F022CB5FDA36E0D2C30A7161FA1C6FB5FBD7D05ADBEF7E68D48F8B6C5F511827C4B1C5ED15B6F20555AFFC4D0857EF7AB2B5C18BA22BEA5D3A79BD1834BADB5878D8C7A4B19DA20C1F62340B1F7FBF01D2F2E97C9714A9DF376AC0EA58072B2B77AEB7872B54A89667519DE44D0FC73540BEEAEC4CB778A45EEBFBEFE2D817A8A8319B2BC6D9FA714F5289EC7C0DBC43496D71CF2A642CB679B0FC4072FD2CF";
        const string pubExponent = "010001";
        static List<KeyValuePair<string, string>> hashMap,hashMapSecure;
        //static void Main(string[] args)
        //{

        //    Interswitch.Init(clientId, clientSecret);

        //    #region -- Get Token --
        //    var token = Interswitch.GetToken();
        //    #endregion

        //    #region --  Get Auth Data With Certificate --
        //    var certificatePath = Environment.CurrentDirectory + "\\paymentgateway.cert";
        //    var authdataWithCertificate = Interswitch.GetAuthData("5060990580000217499", "1111", "2004", "111", certificatePath);
        //    Console.WriteLine("*******************************");
        //    Console.WriteLine("auth data With Certificate ");
        //    Console.WriteLine(authdataWithCertificate);
        //    Console.WriteLine("*******************************");
        //    #endregion
        //    #region -- Get Auth Data using Modulus and Exponent --
        //    var authdataWithModulusExponent = Interswitch.GetAuthData("5060990580000217499", "1111", "1802", "350", mod, pubExponent);
        //    Console.WriteLine("*******************************");
        //    Console.WriteLine("Auth Data with Modulus and Exponent");
        //    Console.WriteLine(authdataWithModulusExponent);
        //    Console.WriteLine("*******************************");
        //    #endregion

        //    #region --  Get Auth Data
        //    var authdata = Interswitch.GetAuthData("5061020000000002330", "1111", "1801", "350");
        //    Console.WriteLine("*******************************");
        //    Console.WriteLine("Auth Data ");
        //    Console.WriteLine(authdata);
        //    Console.WriteLine("*******************************");
        //    #endregion

        //    #region -- Initializing Payment Parameters --
        //    var rand = new Random();

        //    object paymentRequest = new
        //    {
        //        customerId = "1234567890",
        //        amount = "100",
        //        transactionRef = rand.Next(99999999),
        //        currency = "NGN",
        //        authData = authdata
        //    };
        //    #endregion

        //    #region  -- Sending Payment Without token --
        //    Console.WriteLine("****************************");
        //    Console.WriteLine("First overload method started ....");
        //    var response = Interswitch.Send("/api/v2/purchases", "POST", paymentRequest).Result;
        //    Console.WriteLine(response);
        //    Console.WriteLine("First overload method done ....");
        //    Console.WriteLine("****************************");
        //    Console.WriteLine("****************************");
        //    #endregion

        //    #region -- Test QuickTeller Get Biller --
        //    //Console.WriteLine(" QuickTeller Get Biller ....");
        //    //hashMap = new List<KeyValuePair<string, string>>();
        //    //hashMap.AddRange(new List<KeyValuePair<string, string>> {
        //    //    new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
        //    //    new KeyValuePair<string, string>("SignatureMethod", "SHA1"),
        //    //    new KeyValuePair<string, string>("terminalId", "3IWP0001")}
        //    //    );

        //    //var getQuickTellerBiller = Interswitch.Send("/api/v1/quickteller/billers", "GET", "", token, hashMap, "").Result;
        //    //Console.WriteLine(getQuickTellerBiller);
        //    //Console.WriteLine(" QuickTeller Get Billerr done ....");
        //    //Console.WriteLine("****************************");
        //    #endregion

        //    #region -- Sending Payment With Token --
        //    Console.WriteLine("Second overload method started ....");
        //    object paymentRequest2 = new
        //    {
        //        customerId = "1234567890",
        //        amount = "100",
        //        transactionRef = rand.Next(99999999),
        //        currency = "NGN",
        //        authData = authdata
        //    };

        //    var response2 = Interswitch.Send("/api/v2/purchases", "POST", paymentRequest2, token).Result;
        //    Console.WriteLine(response2);
        //    Console.WriteLine("Second overload method done ....");

        //    Console.WriteLine("****************************");
        //    #endregion

        //    #region -- Sending Payment With Token & Custom Headers --
        //    hashMap = new List<KeyValuePair<string, string>>();
        //    hashMap.AddRange(new List<KeyValuePair<string, string>> {
        //        new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
        //        new KeyValuePair<string, string>("SignatureMethod", "SHA1")}
        //        );
        //    paymentRequest = new
        //    {
        //        customerId = "1234567890",
        //        amount = "120",
        //        transactionRef = rand.Next(99999999),
        //        currency = "NGN",
        //        authData = authdata
        //    }; ;
        //    var paymentWithTokenHeadersResponse = Interswitch.Send("/api/v2/purchases", "POST", paymentRequest, token, hashMap).Result;
        //    Console.WriteLine(paymentWithTokenHeadersResponse);
        //    Console.WriteLine("Sending Payment With Token & Custom Headers done ....");
        //    Console.WriteLine("****************************");

        //    Console.ReadKey();
        //    #endregion

        //    #region -- Sending Payment With Token, Custom Headers & Signed Parameter --
        //    hashMap = new List<KeyValuePair<string, string>>();
        //    hashMap.AddRange(new List<KeyValuePair<string, string>> {
        //        new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
        //        new KeyValuePair<string, string>("SignatureMethod", "SHA1")}
        //        );

        //    var paymentWithTokenHeadersSignedParamentResponse = Interswitch.Send("/api/v2/purchases", "POST", paymentRequest, token, hashMap, "").Result;
        //    Console.WriteLine(paymentWithTokenHeadersSignedParamentResponse);
        //    Console.WriteLine(" Sending Payment With Token, Custom Headers & Signed Parameter done ....");
        //    Console.WriteLine("****************************");
        //    #endregion




        //    Console.ReadKey();



        //}

    }
}
