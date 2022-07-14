using System;
using System.Collections.Generic;
using System.Text.Json;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetCustomerByIdUseCase : IGetCustomerByIdUseCase
    {
        private readonly ICustomerGateway _gateway;
        private readonly IGetPersonApiByIdUseCase _getPersonApiByIdUseCase;
        private readonly IGetJigsawCustomerByIdUseCase _jigsawCustomerByIdUseCase;
        private readonly IGetCouncilTaxAccountByAccountRefUseCase _getCouncilTaxAccountByAccountRefUseCase;

        public GetCustomerByIdUseCase(ICustomerGateway gateway, IGetPersonApiByIdUseCase getPersonApiByIdUseCase, IGetJigsawCustomerByIdUseCase jigsawCustomerByIdUseCase,
        IGetCouncilTaxAccountByAccountRefUseCase getCouncilTaxAccountByAccountRefUseCase)
        {
            _gateway = gateway;
            _getPersonApiByIdUseCase = getPersonApiByIdUseCase;
            _jigsawCustomerByIdUseCase = jigsawCustomerByIdUseCase;
            _getCouncilTaxAccountByAccountRefUseCase = getCouncilTaxAccountByAccountRefUseCase;
        }

        [LogCall]
        public MergedCustomerResponseObject Execute(Guid customerId, string userToken, string redisId)
        {
            var customer = _gateway.Find(customerId);

            List<CustomerResponseObject> foundRecords = new List<CustomerResponseObject>();

            foreach (var customerDataSource in customer.DataSources)
            {
                CustomerResponseObject res;
                switch (customerDataSource.DataSourceId)
                {
                    case 1:
                        res = _getPersonApiByIdUseCase.Execute(customerDataSource.SourceId, userToken).Result;
                        foundRecords.Add(res);
                        break;
                    case 2:
                        if (redisId != null)
                        {
                            res = _jigsawCustomerByIdUseCase
                                .Execute(customerDataSource.SourceId, redisId, userToken).Result;
                        }
                        else
                        {
                            res = new CustomerResponseObject()
                            {
                                SystemIds = new List<SystemId>()
                                {
                                    new SystemId()
                                    {
                                        Error = SystemId.UnauthorisedMessage,
                                        Id = customerDataSource.SourceId,
                                        SystemName = "Jigsaw"
                                    }
                                }
                            };
                        }
                        foundRecords.Add(res);
                        break;
                    case 3:
                        res = _getCouncilTaxAccountByAccountRefUseCase.Execute(customerDataSource.SourceId, userToken).Result;
                        foundRecords.Add(res);
                        break;
                }
            }

            return MergeRecords(customer, foundRecords);
        }

        private static MergedCustomerResponseObject MergeRecords(SavedCustomer customer, List<CustomerResponseObject> records)
        {
            var allSystemIds = new List<SystemId>();
            var allKnownAddresses = new List<KnownAddress>();
            var allContactDetails = new List<CutomerContactDetails>();
            var allPersonType = new List<string>();
            var mergedCustomer = new MergedCustomer()
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName,
                Surname = customer.LastName,
                DateOfBirth = customer.DateOfBirth,
                NiNo = customer.NiNumber
            };



            foreach (var r in records)
            {
                allSystemIds.AddRange(r.SystemIds);
                if (r.Customer != null)
                {
                    if (r.Customer.KnownAddresses != null)
                    {
                        allKnownAddresses.AddRange(r.Customer.KnownAddresses);
                    }
                    if (r.Customer.ContactDetails != null)
                    {
                        allContactDetails.Add(new CutomerContactDetails()
                        {
                            ContactDetails = r.Customer.ContactDetails,
                            DataSourceName = r.Customer.DataSource.Name
                        });
                    }
                    if (r.Customer.PersonTypes != null)
                    {
                        allPersonType.AddRange(r.Customer.PersonTypes);
                    }

                    mergedCustomer.Title ??= r.Customer.Title;
                    mergedCustomer.PreferredTitle ??= r.Customer.PreferredTitle;
                    mergedCustomer.PreferredFirstName ??= r.Customer.PreferredFirstName;
                    mergedCustomer.PreferredMiddleName ??= r.Customer.PreferredMiddleName;
                    mergedCustomer.PreferredSurname ??= r.Customer.PreferredSurname;
                    mergedCustomer.PlaceOfBirth ??= r.Customer.PlaceOfBirth;
                    mergedCustomer.NhsNumber ??= r.Customer.NhsNumber;
                    mergedCustomer.IsAMinor ??= r.Customer.IsAMinor;
                    mergedCustomer.DateOfDeath ??= r.Customer.DateOfDeath;
                    mergedCustomer.CouncilTaxAccount ??= r.Customer.CouncilTaxAccount;

                }
            }

            mergedCustomer.KnownAddresses = allKnownAddresses;
            mergedCustomer.ContactDetails = allContactDetails;
            mergedCustomer.PersonTypes = allPersonType;

            return new MergedCustomerResponseObject()
            {
                SystemIds = allSystemIds,
                Customer = mergedCustomer
            };

        }
    }
}
