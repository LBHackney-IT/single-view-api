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
                FirstName = jigsawResponse.PersonInfo.FirstName.Upcase(),
                Surname = jigsawResponse.PersonInfo.LastName.Upcase(),
                DateOfBirth = jigsawResponse.PersonInfo.DateOfBirth,
                DataSource = dataSource,
                NiNo = jigsawResponse.PersonInfo.NationalInsuranceNumber,
                NhsNumber = jigsawResponse.PersonInfo.NhsNumber,
                PregnancyDueDate = jigsawResponse.PersonInfo.PregnancyDueDate,
                AccommodationTypeId = jigsawResponse.PersonInfo.AccommodationTypeId,
                HousingCircumstanceId = jigsawResponse.PersonInfo.HousingCircumstanceId,
                IsSettled = jigsawResponse.PersonInfo.IsSettled,
                SupportWorker = jigsawResponse.PersonInfo.SupportWorker,
                Gender = jigsawResponse.PersonInfo.Gender,
                KnownAddresses = new List<KnownAddress>()
                {
                    new KnownAddress()
                    {

                        FullAddress = jigsawResponse.PersonInfo.AddressString,
                        CurrentAddress = true,
                        DataSourceName = dataSource.Name
                    }
                },
                AllContactDetails = GetContacts(jigsawResponse, dataSource.Name)
            };
        }
        return response;
    }

    private List<CustomerContactDetails> GetContacts(JigsawCustomerResponseObject jigsawResponse, string dataSourceName)
    {
        var res = new List<CustomerContactDetails>();
        if (jigsawResponse.PersonInfo.OkToContactOnEmail && !jigsawResponse.PersonInfo.EmailAddress.IsNullOrEmpty())
        {
            res.Add(new CustomerContactDetails()
            {
                ContactType = "Email",
                DataSourceName = dataSourceName,
                Value = jigsawResponse.PersonInfo.EmailAddress
            });
        }
        if (jigsawResponse.PersonInfo.OkToContactOnHomePhoneNumber && !jigsawResponse.PersonInfo.HomePhoneNumber.IsNullOrEmpty())
        {
            res.Add(new CustomerContactDetails()
            {
                ContactType = "Phone",
                SubType = "Home",
                DataSourceName = dataSourceName,
                Value = jigsawResponse.PersonInfo.HomePhoneNumber
            });
        }
        if (jigsawResponse.PersonInfo.OkToContactOnMobilePhoneNumber && !jigsawResponse.PersonInfo.MobilePhoneNumber.IsNullOrEmpty())
        {
            res.Add(new CustomerContactDetails()
            {
                ContactType = "Phone",
                SubType = "Mobile",
                DataSourceName = dataSourceName,
                Value = jigsawResponse.PersonInfo.MobilePhoneNumber
            });
        }
        if (jigsawResponse.PersonInfo.OkToContactOnWorkPhoneNumber && !jigsawResponse.PersonInfo.WorkPhoneNumber.IsNullOrEmpty())
        {
            res.Add(new CustomerContactDetails()
            {
                ContactType = "Phone",
                SubType = "Work Phone",
                DataSourceName = dataSourceName,
                Value = jigsawResponse.PersonInfo.WorkPhoneNumber
            });
        }
        if (!jigsawResponse.PersonInfo.CorrespondenceAddressString.IsNullOrEmpty())
        {
            res.Add(new CustomerContactDetails()
            {
                ContactType = "Address",
                SubType = "Correspondence Address",
                DataSourceName = dataSourceName,
                Value = jigsawResponse.PersonInfo.CorrespondenceAddressString
            });
        }

        return res.Count == 0 ? null : res;
    }

    public static Guid ToGuid(int value)
    {
        byte[] bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

}
