using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterswitchSecure
{
    public class Secure
    {
        public static String[] createPaymentMethod(string subscriberId, string ttid, string pan, string expiryDate,  string pinKey)
        {
            byte[] pinKeyBytes = HexConverter.StringToByteArray(pinKey);
            byte[] macKeyByte = pinKeyBytes;
            string macCipherText = SecurityUtils.getMacCipherText(subscriberId, ttid, null, null, null, null);

            string mac = MACUtils.getMacValue(macCipherText, macKeyByte);
            string pinData = SecurityUtils.getEncryptedExpiryDateBlock(expiryDate, pinKeyBytes);
            string secure = SecurityUtils.getCreatePaymentMethodSecure(pan, mac, pinKeyBytes, macKeyByte);

            String[] result = { pinData, secure};
            return result;
        }

        public static String[] balanceMethod(string subscriberId, string ttid, string pan, string expiryDate, string paymentMethodTypeCode, string pinKey)
        {
            byte[] pinKeyBytes = HexConverter.StringToByteArray(pinKey);
            byte[] macKeyByte = pinKeyBytes;
            string macCipherText = SecurityUtils.getMacCipherText(subscriberId, ttid, null, null, null, null);

            string mac = MACUtils.getMacValue(macCipherText, macKeyByte);
            string pinData = SecurityUtils.getEncryptedExpiryDateBlock(expiryDate, pinKeyBytes);
            string secure = SecurityUtils.getSecure(pan, mac, pinKeyBytes, macKeyByte);

            String[] result = { pinData, secure };
            return result;
        }

        public static String[] cardDetails(string subscriberId, string ttid, string pan, string pin, string cvv2, string expiryDate, string paymentMethodTypeCode, string pinKey)
        {
            byte[] pinKeyBytes = HexConverter.StringToByteArray(pinKey);
            byte[] macKeyByte = pinKeyBytes;
            string macCipherText = SecurityUtils.getMacCipherText(subscriberId, ttid, null, null, null, null);

            string mac = MACUtils.getMacValue(macCipherText, macKeyByte);
            string pinData = SecurityUtils.getEncryptedPinCvv2ExpiryDateBlock(pin, cvv2, expiryDate, pinKeyBytes);
            string secure = SecurityUtils.getSecure(pan, mac, pinKeyBytes, macKeyByte);

            String[] result = { pinData, secure };
            return result;
        }
    }
}
