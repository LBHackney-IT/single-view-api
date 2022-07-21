using System;
using System.Collections.Generic;
using System.Text.Json;
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

        var jigsawResponse = await _jigsawGateway.GetCustomerById(customerId, jigsawAuthResponse.Token);


        var dataSource = _dataSourceGateway.GetEntityById(2);

        var jigsawId = new SystemId() { SystemName = dataSource?.Name, Id = jigsawResponse?.Id };

        var systemIdList = new List<SystemId>() { jigsawId };

        var response = new CustomerResponseObject() { SystemIds = systemIdList };

        if (jigsawResponse == null)
        {
            jigsawId.Error = "Not found";
        }
        else
        {
            response.Customer = new Customer()
            {
                Id = jigsawResponse.Id,
                FirstName = jigsawResponse.PersonInfo.FirstName,
                Surname = jigsawResponse.PersonInfo.LastName,
                DateOfBirth = jigsawResponse.PersonInfo.DateOfBirth,
                DataSource = dataSource,
                NiNo = jigsawResponse.PersonInfo.NationalInsuranceNumber,
                NhsNumber = jigsawResponse.PersonInfo.NhsNumber,
                KnownAddresses = new List<KnownAddress>()
                {
                    new KnownAddress()
                    {

                        FullAddress = jigsawResponse.PersonInfo.AddressString,
                        CurrentAddress = true,
                        DataSourceName = dataSource.Name
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
