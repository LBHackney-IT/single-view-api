using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Helpers;

public class JwtHelper
{
    public JigsawCredentials DecodeJigsawCredentials(string cipher)
    {
        byte[] key = Encoding.UTF8.GetBytes("GVitnm.QjsXUYjTTJ@_@.hAr-Lh2GVAX"); // 32
        byte[] iv = Encoding.UTF8.GetBytes("J@_@.hAr-Lh2GVAX"); //16
        // var strKey = "GVitnm.QjsXUYjTTJ@_@.hAr-Lh2GVAX";
        //
        var cipherText = Convert.FromBase64String(cipher);

        var decodedJson = DecryptStringFromBytes_Aes(cipherText, key, iv);

        return new JigsawCredentials();
    }

    public static string Decrypt( string encryptedText, string key)
    {

        var combined = Convert.FromBase64String(encryptedText);
        var buffer = new byte[combined.Length];
        var hash = SHA512.Create();
        var aesKey = new byte[24];
        Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

        using (var aes = Aes.Create())
        {
            if (aes == null)
                throw new ArgumentException("Parameter must not be null.", nameof(aes));

            aes.Key = aesKey;

            var iv = new byte[aes.IV.Length];
            var ciphertext = new byte[buffer.Length - iv.Length];

            Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
            Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                using (var plainStream = new MemoryStream(ciphertext))
                {
                    plainStream.CopyTo(aesStream);
                }

                var thing = resultStream.ToArray();
                return Encoding.UTF8.GetString(thing);
            }
        }
    }
    static string EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");
        string encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }

                    char[] characters = msEncrypt.ToArray().Select(b => (char)b).ToArray();
                    encrypted =  new string(characters);
                    // encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}

public class JigsawCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}



