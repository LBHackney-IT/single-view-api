namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IStoreJigsawCredentialsUseCase
{
    string Execute(string encryptedJigsawCredentials, string hackneyToken);
}
