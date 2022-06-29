namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetCouncilTaxAccountsByCustomerNameUseCase
{
    dynamic Execute(string firstName, string lastName, string userToken);
}
