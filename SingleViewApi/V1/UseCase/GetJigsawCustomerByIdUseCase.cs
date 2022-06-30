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
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawCustomerByIdUseCase : IGetJigsawCustomerByIdUseCase

{
    private IJigsawGateway _jigsawGateway;
    private IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetJigsawCustomerByIdUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase, IDataSourceGateway dataSourceGateway)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
        _dataSourceGateway = dataSourceGateway;
    }

    public async Task<CustomerResponseObject> Execute(string customerId, string redisId, string hackneyToken)
    {

        var jigsawAuthResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

        if (!String.IsNullOrEmpty(jigsawAuthResponse.ExceptionMessage))
        {
            Console.WriteLine($"Error getting Jigsaw token for CustomerById: {jigsawAuthResponse.ExceptionMessage}");
            return null;
        }

        var customer = await _jigsawGateway.GetCustomerById(customerId, jigsawAuthResponse.Token);

        var dataSource = _dataSourceGateway.GetEntityById(2);

        var jigsawId = new SystemId() { SystemName = dataSource.Name, Id = customer.Id };

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
                Id = customer.Id,
                FirstName = customer.PersonInfo.FirstName,
                Surname = customer.PersonInfo.LastName,
                DateOfBirth = customer.PersonInfo.DateOfBirth,
                DataSource = dataSource,
                NiNo = customer.PersonInfo.NationalInsuranceNumber,
                NhsNumber = customer.PersonInfo.NhsNumber,
                KnownAddresses = new List<KnownAddress>()
                {
                    new KnownAddress()
                    {

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
