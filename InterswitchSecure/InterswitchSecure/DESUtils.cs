using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace InterswitchSecure
{
    
        public class DESUtils
        {
           
            public static byte[] encrypt(string clearPINBlock, byte[] pinKey)
            {
                DesEdeEngine engine = new DesEdeEngine();
                DesEdeParameters keyParameters = new DesEdeParameters(pinKey);
                engine.Init(true, keyParameters);
                byte[] clearPINBlockBytes = Hex.Decode(clearPINBlock);
                byte[] encryptedPINBlockBytes = new byte[8];
                int res = engine.ProcessBlock(clearPINBlockBytes, 0, encryptedPINBlockBytes, 0);
                byte[] encodedEncryptedPINBlock = Hex.Encode(encryptedPINBlockBytes);
                AppUtils.zeroise(clearPINBlockBytes);
                AppUtils.zeroise(encryptedPINBlockBytes);
                return encodedEncryptedPINBlock;
               
            }

            private static readonly UTF8Encoding encoding = new UTF8Encoding();


            public static byte[] decrypt(String encryptedPINBlock, byte[] pinKey)
            {
                DesEdeEngine engine = new DesEdeEngine();
                DesEdeParameters keyParameters = new DesEdeParameters(pinKey);
                engine.Init(false, keyParameters);
                byte[] encryptedPINBlockBytes = Hex.Encode(Encoding.UTF8.GetBytes(encryptedPINBlock));
                byte[] clearPINBlockBytes = new byte[8];
                int res = engine.ProcessBlock(encryptedPINBlockBytes, 0, clearPINBlockBytes, 0);
                byte[] decodedClearPINBlockBytes = Hex.Decode(clearPINBlockBytes);
                AppUtils.zeroise(encryptedPINBlockBytes);
                AppUtils.zeroise(clearPINBlockBytes);
                return decodedClearPINBlockBytes;
            }

            internal static byte[] GenerateRandomBytes(int length)
            {
                var bytes = new byte[length];
                RngProvider.GetBytes(bytes);
                return bytes;
            }

            private static RNGCryptoServiceProvider _rngProvider;
            /// <summary>
            ///   Gets the RNG provider.
            /// </summary>
            /// <value> The RNG provider. </value>
            private static RNGCryptoServiceProvider RngProvider
            {
                get { return _rngProvider ?? (_rngProvider = new RNGCryptoServiceProvider()); }
            }

            public static byte[] generateKey()
            {
                RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
                SecureRandom sr = new SecureRandom();
                KeyGenerationParameters kgp = new KeyGenerationParameters(sr, DesEdeParameters.DesEdeKeyLength * 8);
                DesEdeKeyGenerator kg = new DesEdeKeyGenerator();
                kg.Init(kgp);
                var key = kg.GenerateKey();
                int len = key.Length;
                DesEdeParameters.SetOddParity(key);
                return key;
            }

           

        }


        
    
}
