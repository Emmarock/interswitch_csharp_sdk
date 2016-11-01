using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Math;

namespace InterswitchSecure
{
    public class SecurityUtils
    {

        protected static String publicKeyExponent = "010001";
        protected static String publicKeyModulus = "009C7B3BA621A26C4B02F48CFC07EF6EE0AED8E12B4BD11C5CC0ABF80D5206BE69E1891E60FC88E2D565E2FABE4D0CF630E318A6C721C3DED718D0C530CDF050387AD0A30A336899BBDA877D0EC7C7C3FFE693988BFAE0FFBAB71B25468C7814924F022CB5FDA36E0D2C30A7161FA1C6FB5FBD7D05ADBEF7E68D48F8B6C5F511827C4B1C5ED15B6F20555AFFC4D0857EF7AB2B5C18BA22BEA5D3A79BD1834BADB5878D8C7A4B19DA20C1F62340B1F7FBF01D2F2E97C9714A9DF376AC0EA58072B2B77AEB7872B54A89667519DE44D0FC73540BEEAEC4CB778A45EEBFBEFE2D817A8A8319B2BC6D9FA714F5289EC7C0DBC43496D71CF2A642CB679B0FC4072FD2CF";


        private static String encryptPinBlock(String clearPinBlock, byte[] pinKey)
        {
            Debug.Assert(clearPinBlock != null, "Pin block cannot be null");
            Debug.Assert(clearPinBlock.Length <= 16, "Pin block cannot be more than 16 xters");
            byte[] randomBytes = new byte[1];
            RandomNumberGenerator sr = RNGCryptoServiceProvider.Create();
            sr.GetBytes(randomBytes);
            int randomDigit = (int)((randomBytes[0] * 10) / 128);
            randomDigit = Math.Abs(randomDigit);
            int pinpadlen = 16 - clearPinBlock.Length;
            for (int i = 0; i < pinpadlen; i++)
                clearPinBlock = clearPinBlock + randomDigit;

            byte[] encodedEncryptedPINBlockBytes = DESUtils.encrypt(clearPinBlock, pinKey); 
            String encryptedPinBlock = Encoding.Default.GetString(encodedEncryptedPINBlockBytes);
            clearPinBlock = "0000000000000000";
            AppUtils.zeroise(encodedEncryptedPINBlockBytes);
            return encryptedPinBlock;
        }

        /***
         * Use when you have pin, cvv2, and expiry date
         * @param pin: card pin
         * @param cvv2: card cvv2
         * @param expiryDate: card expiry date
         * @param pinKey: pin Key
         * @return
         */
        public static String getEncryptedPinCvv2ExpiryDateBlock(string pin, string cvv2, string expiryDate, byte[] pinKey)
        {
            if (pin == null || string.Compare(pin,"",true) == 0)
                pin = "0000";
            if (cvv2 == null || string.Compare(cvv2,"",true) == 0)
                cvv2 = "000";
            if (expiryDate == null || string.Compare(expiryDate,"", true) == 0)
                expiryDate = "0000";

            String pinBlockString = pin + cvv2 + expiryDate;
            int pinBlockStringLen = pinBlockString.Length;
            String pinBlockLenLenString = Convert.ToString(pinBlockStringLen);
            int pinBlockLenLen = pinBlockLenLenString.Length;
            String clearPinBlock = Convert.ToString(pinBlockLenLen) + pinBlockStringLen + pinBlockString;
            //clearPinBlock = "00000000";
            return encryptPinBlock(clearPinBlock, pinKey);
        }

        /***
         * Use when you have pin, cvv2, but no expiry date
         * @param pin: card pin
         * @param cvv2: card cvv2
         * @param pinKey: pin Key
         * @return
         */
        public static String getEncryptedPinCvv2Block(String pin, String cvv2, byte[] pinKey)
        {
            return getEncryptedPinCvv2ExpiryDateBlock(pin, cvv2, "", pinKey);
        }


        /***
         * Use when you have pin and expiry date, but no cvv2
         * @param pin: card pin
         * @param expiryDate: card expiry date
         * @param pinKey: pin Key
         * @return
         */
        public static String getEncryptedPinExpiryDateBlock(String pin, String expiryDate, byte[] pinKey)
        {
            return getEncryptedPinCvv2ExpiryDateBlock(pin, "000", expiryDate, pinKey);
        }

        /***
         * Use when you have only pin
         * @param pin: card pin
         * @param pinKey: pin Key
         * @return
         */
        public static String getEncryptedPinBlock(String pin, byte[] pinKey)
        {
            return getEncryptedPinCvv2ExpiryDateBlock(pin, "", "", pinKey);
        }

        /***
         * Use when you have only expiryDate
         * @param expiryDate: card expiry date
         * @param pinKey: pin Key
         * @return
         */
        public static String getEncryptedExpiryDateBlock(String expiryDate, byte[] pinKey)
        {
            return getEncryptedPinCvv2ExpiryDateBlock("0000", "000", expiryDate, pinKey);
        }



        private static string getSecure(byte[] secureBody, byte[] pinKey, byte[] macKey)
        {
            byte[] secureBytes = new byte[64];
            byte[] headerBytes = new byte[1];
            byte[] formatVersionBytes = new byte[1];
            byte[] macVersionBytes = new byte[1];
            byte[] pinDesKey = new byte[16];
            byte[] macDesKey = new byte[16];
            byte[] secureBodyBytes = new byte[28];
            byte[] footerBytes = new byte[1];

            headerBytes = AppUtils.hexConverter(ConstantUtils.SECURE_HEADER);
            formatVersionBytes = AppUtils.hexConverter(ConstantUtils.SECURE_FORMAT_VERSION);
            macVersionBytes = AppUtils.hexConverter(ConstantUtils.SECURE_MAC_VERSION);
            pinDesKey = pinKey;
            macDesKey = macKey;
            secureBodyBytes = secureBody;
            footerBytes = AppUtils.hexConverter(ConstantUtils.SECURE_FOOTER);

            Array.Copy(headerBytes, headerBytes.GetLowerBound(0), secureBytes, 0, 1);
            Array.Copy(formatVersionBytes, 0, secureBytes, 1, 1);
            Array.Copy(macVersionBytes, 0, secureBytes, 2, 1);
            Array.Copy(pinDesKey, 0, secureBytes, 3, 16);
            Array.Copy(macDesKey, 0, secureBytes, 19, 16);
            Array.Copy(secureBodyBytes, 0, secureBytes, 35, 28);
            Array.Copy(footerBytes, 0, secureBytes, 63, 1);
            String encrytedSecure = Encoding.Default.GetString(RSAUtils.rsaEncrypt(publicKeyModulus, publicKeyExponent, secureBytes));
            AppUtils.zeroise(secureBytes);

            return encrytedSecure;
        }

        /***
         * Use this function to calculate secure for CreatePaymentMethod transaction type.
         * @param pan: Payment Method's PAN
         * @param mac: Calculated MAC. Use MACUtils.getMAC().
         * @param pinKey: Generated Pin Key
         * @param macKey: Generated macKey
         * @return
         */
        public static String getCreatePaymentMethodSecure(string pan, string mac, byte[] pinKey, byte[] macKey)
        {

            byte[] panBytes = new byte[20];
            byte[] macBytes = Hex.Decode(mac);
            byte[] padBytes = AppUtils.hexConverter("FFFFFFFF");

                             
            string panLen = Convert.ToString(pan.Length);
            int panLenLen = panLen.Length;
            string panBlock = Convert.ToString(panLenLen) + panLen + pan;
            string rightPadded = AppUtils.padRight(panBlock, 40, "F");
            panBytes = AppUtils.hexConverter(rightPadded);

            byte[] secureBodyBytes = new byte[28];
            Array.Copy(panBytes, 0, secureBodyBytes, 0, 20);
            Array.Copy(macBytes, 0, secureBodyBytes, 20, 4);
            Array.Copy(padBytes, 0, secureBodyBytes, 24, 4);

            string secure = getSecure(secureBodyBytes, pinKey, macKey);
            return secure;
        }

        /***
         * Use this method to generate secure for every other transaction type. (Generate Token, Balance Enquiry, Mini Statement)
         * @param subscriberId Subscriber mobile number
         * @param mac: Calculated MAC. Use MACUtils.getMAC().
         * @param pinKey: Generated Pin Key
         * @param macKey: Generated macKey
         * @return
         */
        public static String getSecure(string subscriberId, string mac, byte[] pinKey, byte[] macKey)
        {
            byte[] subscriberIdBytes = new byte[8];
            byte[] macBytes = Hex.Decode(mac);
            byte[] padBytes = AppUtils.hexConverter("FFFFFFFF");

            string paddedSubscriberId = AppUtils.padRight(subscriberId, 40, "0");
            subscriberIdBytes = AppUtils.hexConverter(paddedSubscriberId);

            byte[] secureBodyBytes = new byte[28];
            Array.Copy(subscriberIdBytes, 0, secureBodyBytes, 0, 20);
            Array.Copy(macBytes, 0, secureBodyBytes, 20, 4);
            Array.Copy(padBytes, 0, secureBodyBytes, 24, 4);

            string secure = getSecure(secureBodyBytes, pinKey, macKey);
            return secure;
        }

        public static String getPanSecure(string pan, string mac, byte[] pinKey, byte[] macKey)
        {
            byte[] subscriberIdBytes = new byte[8];
            byte[] macBytes = Hex.Decode(mac);
            byte[] padBytes = AppUtils.hexConverter("FFFFFFFF");

            string paddedPan = AppUtils.padRight(pan, 20, "0");
            subscriberIdBytes = AppUtils.hexConverter(paddedPan);

            byte[] secureBodyBytes = new byte[28];
            Array.Copy(subscriberIdBytes, 0, secureBodyBytes, 0, 10);
            Array.Copy(macBytes, 0, secureBodyBytes, 10, 4);
            Array.Copy(padBytes, 0, secureBodyBytes, 14, 14);

            string secure = getSecure(secureBodyBytes, pinKey, macKey);
            return secure;
        }


        public static string getMacCipherText(string subscriberId, string ttid, string amount, string phoneNumber, string customerId, string paymentItemCode)
        {
            string macData = "";

            if (!AppUtils.isNullOrEmpty(subscriberId))
                macData += subscriberId;

            macData += "default";

            if (!AppUtils.isNullOrEmpty(ttid))
                macData += ttid;

            if (!AppUtils.isNullOrEmpty(amount))
                macData += amount;

            if (!AppUtils.isNullOrEmpty(phoneNumber))
                macData += phoneNumber;

            if (!AppUtils.isNullOrEmpty(customerId))
                macData += customerId;

            if (!AppUtils.isNullOrEmpty(paymentItemCode))
                macData += paymentItemCode;

            return macData;
        }
    
    }
}
