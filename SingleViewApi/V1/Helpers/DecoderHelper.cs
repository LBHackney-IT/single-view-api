using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SingleViewApi.V1.Helpers.Interfaces;


namespace SingleViewApi.V1.Helpers;

public class DecoderHelper : IDecoderHelper
{
    private readonly byte[] _iv;
    private readonly byte[] _key;

    public DecoderHelper(string aeskey, string aesiv)
    {
        _key = Encoding.UTF8.GetBytes(aeskey);
        _iv = Encoding.UTF8.GetBytes(aesiv);
    }
    public JigsawCredentials DecodeJigsawCredentials(string cipher)
    {

        var cipherText = Convert.FromBase64String(cipher);

        var decodedJson = Decrypt(cipherText, _key, _iv);

        var credentials = JsonConvert.DeserializeObject<JigsawCredentials>(decodedJson);

        return credentials;
    }
    static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
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
            aesAlg.Padding = PaddingMode.None;

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



