using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

using SingleViewApi.V1.Helpers.Interfaces;


namespace SingleViewApi.V1.Helpers;

public class DecoderHelper : IDecoderHelper
{
    private readonly string _privateKey;

    public DecoderHelper(string rsaPrivateKey)
    {
        _privateKey = rsaPrivateKey;

    }
    public JigsawCredentials DecodeJigsawCredentials(string cipher)
    {
        if (cipher == "Placeholder-Jigsaw-Token")
        {
            return new JigsawCredentials()
            {
                Password = "Placeholder-Jigsaw-Password", Username = "Placeholder-Jigsaw-Username"
            };
        }

        var cipherText = Convert.FromBase64String(cipher);

        var decodedJson = Decrypt(cipherText, _privateKey);

        var credentials = JsonConvert.DeserializeObject<JigsawCredentials>(decodedJson);

        return credentials;
    }

    private static string Decrypt(byte[] dataByte, string privateKey)
    {

        var rsa = RSA.Create();
        byte[] bytesPrivakeKey = Convert.FromBase64String(privateKey);
        int bytesRead;
        rsa.ImportRSAPrivateKey(new ReadOnlySpan<byte>(bytesPrivakeKey), out bytesRead);
        var decrypted = rsa.Decrypt(dataByte, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(decrypted);
    }
}

public class JigsawCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}



