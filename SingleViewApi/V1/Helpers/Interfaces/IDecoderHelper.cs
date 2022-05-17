namespace SingleViewApi.V1.Helpers.Interfaces;

public interface IDecoderHelper
{
    JigsawCredentials DecodeJigsawCredentials(string cipher);
}
