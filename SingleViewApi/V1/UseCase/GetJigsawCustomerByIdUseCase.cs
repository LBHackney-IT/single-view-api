using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawCustomerByIdUseCase : IGetJigsawCustomerByIdUseCase

{
    private IJigsawGateway _jigsawGateway;
    private IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;

    public GetJigsawCustomerByIdUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
    }

    public async Task<CustomerResponseObject> Execute(string customerId, string redisId)
    {

        var jigsawAuthResponse = _getJigsawAuthTokenUseCase.Execute(redisId).Result;

        if (!String.IsNullOrEmpty(jigsawAuthResponse.ExceptionMessage))
        {
            Console.WriteLine($"Error getting Jigsaw token for CustomerById: {jigsawAuthResponse.ExceptionMessage}");
            return null;
        }

        var customer = await _jigsawGateway.GetCustomerById(customerId, jigsawAuthResponse.Token);

        var jigsawId = new SystemId() { SystemName = DataSource.Jigsaw, Id = customer.Id };

        var systemIdList = new List<SystemId>() { jigsawId };

        var response = new CustomerResponseObject() { SystemIds = systemIdList };

        if (customer == null)
        {
            jigsawId.Error = "Not found";
        }
        else
        {
            response.Customer = new Customer()
            {
                FirstName = customer.PersonInfo.FirstName,
                Surname = customer.PersonInfo.LastName,
                DateOfBirth = customer.PersonInfo.DateOfBirth,
                DataSource = DataSource.Jigsaw,
                NiNo = customer.PersonInfo.NationalInsuranceNumber,
                KnownAddresses = new List<KnownAddress>()
                {
                    new KnownAddress()
                    {
                        Id = new Guid(),
                        FullAddress = customer.PersonInfo.AddressString,
                        CurrentAddress = true
                    }
                }
            };
        }

        return response;

    }

    public static Guid ToGuid(int value)
    {
        byte[] bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

}
