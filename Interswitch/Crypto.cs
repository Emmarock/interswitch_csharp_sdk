using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Payment
{
    public class Crypto
    {
        public static string Mod = "9C7B3BA621A26C4B02F48CFC07EF6EE0AED8E12B4BD11C5CC0ABF80D5206BE69E1891E60FC88E2D565E2FABE4D0CF630E318A6C721C3DED718D0C530CDF050387AD0A30A336899BBDA877D0EC7C7C3FFE693988BFAE0FFBAB71B25468C7814924F022CB5FDA36E0D2C30A7161FA1C6FB5FBD7D05ADBEF7E68D48F8B6C5F511827C4B1C5ED15B6F20555AFFC4D0857EF7AB2B5C18BA22BEA5D3A79BD1834BADB5878D8C7A4B19DA20C1F62340B1F7FBF01D2F2E97C9714A9DF376AC0EA58072B2B77AEB7872B54A89667519DE44D0FC73540BEEAEC4CB778A45EEBFBEFE2D817A8A8319B2BC6D9FA714F5289EC7C0DBC43496D71CF2A642CB679B0FC4072FD2CF";
        public static string PubExponent = "010001";

        public static string GetAuthData(string pan, string pin, string expiryDate, string cvv2, string modulus, string pubExpo)
        {
            pan = pan != null ? pan.Trim() : "";

            cvv2 = cvv2 != null ? cvv2.Trim() : "";

            expiryDate = expiryDate != null ? expiryDate.Trim() : "";

            Mod = modulus;
            PubExponent = pubExpo;
            var authData = string.Format("1Z{0}Z{1}Z{2}Z{3}", pan, pin, expiryDate, cvv2);
            var result = RsaEncryptWithPrivate(authData);
            return result;
        }

       
        public static string GetAuthData(string pan, string pin, string expiryDate, string cvv2, string certificatePath = null)
        {
            pan = pan != null ? pan.Trim() : "";
            string result;
            cvv2 = cvv2 != null ? cvv2.Trim() : "";
            expiryDate = expiryDate != null ? expiryDate.Trim() : "";
            var authData = string.Format("1Z{0}Z{1}Z{2}Z{3}", pan, pin, expiryDate, cvv2);
            if (certificatePath == null)
            {
                result = RsaEncryptWithPrivate(authData);
                return result;
            }
            result = RsaEncryptWithPublicKey(authData, certificatePath);
            return result;
        }
        #region -- Experiment on Secure --
        //public static KeyValuePair<string, string> GetSecureData(string pan, string pin, string expiryDate, string cvv2, string certificatePath = null)
        //{
        //    KeyValuePair<String, String> secureData = new KeyValuePair<String, String>();

        //    byte[] secureBytes = new byte[64];
        //    byte[] headerBytes = new byte[1];
        //    byte[] formatVersionBytes = new byte[1];
        //    byte[] macVersionBytes = new byte[1];
        //    byte[] pinDesKey = new byte[16];
        //    byte[] macDesKey = new byte[16];
        //    byte[] macBytes = new byte[4];
        //    byte[] customerIdBytes = new byte[10];
        //    byte[] footerBytes = new byte[1];
        //    byte[] otherBytes = new byte[14];
        //    byte[] keyBytes = GenerateKey();

        //    Array.Copy(customerIdBytes, 0, secureBytes, 35, 10);
        //    Array.Copy(macBytes, 0, secureBytes, 45, 4);
        //    Array.Copy(otherBytes, 0, secureBytes, 49, 14);
        //    Array.Copy(footerBytes, 0, secureBytes, 63, 1);

        //    headerBytes = Encoding.ASCII.GetBytes("4D");
        //    formatVersionBytes = Encoding.ASCII.GetBytes("10");
        //    macVersionBytes = Encoding.ASCII.GetBytes("10");
        //    pinDesKey = keyBytes;


        //    if (!string.IsNullOrEmpty(pan))
        //    {
        //        var panDiff = 20 - pan.Length;
        //        var panString = panDiff + pan;
        //        var panlen = 20 - panString.Length;
        //        for (var i = 0; i < panlen; i++)
        //        {
        //            panString += "F";
        //        }

        //        customerIdBytes = Encoding.ASCII.GetBytes(padRight(panString, 20));
        //    }
        //    String macData = "";
        //    macBytes = Hex.Decode(GetMAC(macData, macDesKey, 11));
        //    footerBytes = Encoding.ASCII.GetBytes("5A");

        //    Array.Copy(headerBytes, 0, secureBytes, 0, 1);
        //    Array.Copy(formatVersionBytes, 0, secureBytes, 1, 1);
        //    Array.Copy(macVersionBytes, 0, secureBytes, 2, 1);
        //    Array.Copy(pinDesKey, 0, secureBytes, 3, 16);
        //    Array.Copy(macDesKey, 0, secureBytes, 19, 16);
        //    Array.Copy(customerIdBytes, 0, secureBytes, 35, 10);
        //    Array.Copy(macBytes, 0, secureBytes, 45, 4);
        //    Array.Copy(otherBytes, 0, secureBytes, 49, 14);
        //    Array.Copy(footerBytes, 0, secureBytes, 63, 1);
        //    return  new KeyValuePair<string, string>();

        //}

        //private static string GetMAC(String macData, byte[] macKey, int macVersion)
        //{
        //    byte[] macBytes = new byte[4];
        //    byte[] macDataBytes = Encoding.ASCII.GetBytes(macData);
        //    byte[] encodedMacBytes;
        //    String macCipher1;
        //    //SecretKeySpec keyParameters1;
        //    //Mac engine1;
        //    //if (macVersion == 8)
        //    //{
        //    //    macCipher1 = "";

        //    //    try
        //    //    {
        //    //        keyParameters1 = new SecretKeySpec(macKey, "HmacSHA1");
        //    //        engine1 = Mac.getInstance(keyParameters1.getAlgorithm());
        //    //        engine1.init(keyParameters1);
        //    //        encodedMacBytes = macData.getBytes();
        //    //        macBytes = engine1.doFinal(encodedMacBytes);
        //    //        macCipher1 = new String(Hex.encode(macBytes), "UTF-8");
        //    //    }
        //    //    catch (InvalidKeyException var11)
        //    //    {
        //    //        ;
        //    //    }
        //    //    catch (NoSuchAlgorithmException var12)
        //    //    {
        //    //        ;
        //    //    }
        //    //    catch (UnsupportedEncodingException var13)
        //    //    {
        //    //        ;
        //    //    }

        //    //    return macCipher1;
        //    //}
        //    //else if (macVersion == 12)
        //    //{
        //    //    macCipher1 = "";

        //    //    try
        //    //    {
        //    //        keyParameters1 = new SecretKeySpec(macKey, "HmacSHA256");
        //    //        engine1 = Mac.getInstance(keyParameters1.getAlgorithm());
        //    //        engine1.Init(keyParameters1);
        //    //        encodedMacBytes = macData.getBytes();
        //    //        macBytes = engine1.doFinal(encodedMacBytes);
        //    //        macCipher1 = new String(Hex.Encode(macBytes), "UTF-8");
        //    //    }
        //    //    catch (InvalidKeyException var14)
        //    //    {
        //    //        ;
        //    //    }
        //    //    catch (NoSuchAlgorithmException var15)
        //    //    {
        //    //        ;
        //    //    }
               
        //    //    return macCipher1;
        //    //}
        //    //else
        //    //{
        //    //    CbcBlockCipher macCipher = new CbcBlockCipher(
        //    //            new DesEdeEngine());
        //    //    DesEdeParameters keyParameters = new DesEdeParameters(macKey);
        //    //    DesEdeEngine engine = new DesEdeEngine();
        //    //    engine.Init(true, keyParameters);
        //    //    macCipher.Init(keyParameters);

        //    //    macCipher.update(macDataBytes, 0, macData.Length);
        //    //    macCipher.Final(macBytes, 0);
        //    //    encodedMacBytes = Hex.Encode(macBytes);
        //    //    String mac = new string(encodedMacBytes);
        //    //    return mac;
        //    //}
        //    return null;
        //}

        //private static String padRight(String data, int maxLen)
        //{

        //    if (data == null || data.Length >= maxLen)
        //    {
        //        return data;
        //    }

        //    int len = data.Length;
        //    int deficitLen = maxLen - len;
        //    for (int i = 0; i < deficitLen; i++)
        //    {
        //        data += "0";
        //    }

        //    return data;
        //}
        //public static byte[] GenerateKey()
        //{
        //    var secureRandom = new SecureRandom();
        //    var kgp = new KeyGenerationParameters(secureRandom, DesEdeParameters.DesKeyLength * 16);
        //    var kg = new DesEdeKeyGenerator();
        //    kg.Init(kgp);

        //    byte[] desKeyBytes = kg.GenerateKey();
        //    DesEdeParameters.SetOddParity(desKeyBytes);

        //    return desKeyBytes;
        //}
#endregion
        private static string RsaEncryptWithPublicKey(string input, string certificatePath)
        {
            var output = string.Empty;
            X509Certificate2 cert;
            try
            {
                cert = getCertificate(certificatePath);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            using (RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key)
            {
                byte[] bytesData = Encoding.UTF8.GetBytes(input);
                byte[] bytesEncrypted = csp.Encrypt(bytesData, false);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            return output;
        }
        private static X509Certificate2 getCertificate(string certificateName)
        {
            return new X509Certificate2(X509Certificate2.CreateFromCertFile(certificateName));
        }
        public static string RsaEncryptWithPrivate(string clearText)
        {
            var mod = new BigInteger(Mod, 16);
            var pubExp = new BigInteger(PubExponent, 16);

            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);
            var pubParameters = new RsaKeyParameters(false, mod, pubExp);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(true, pubParameters);
            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }
    }

}
