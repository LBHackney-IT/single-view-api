using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetByIdUseCase
{
    ResponseObject Execute(int id);
}
