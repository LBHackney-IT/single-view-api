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
        private readonly IGetHousingBenefitsAccountByAccountRefUseCase _getHousingBenefitsAccountByAccountRefUseCase;

        public GetCustomerByIdUseCase(ICustomerGateway gateway, IGetPersonApiByIdUseCase getPersonApiByIdUseCase, IGetJigsawCustomerByIdUseCase jigsawCustomerByIdUseCase,
        IGetCouncilTaxAccountByAccountRefUseCase getCouncilTaxAccountByAccountRefUseCase, IGetHousingBenefitsAccountByAccountRefUseCase getHousingBenefitsAccountByAccountRefUseCase)
        {
            _gateway = gateway;
            _getPersonApiByIdUseCase = getPersonApiByIdUseCase;
            _jigsawCustomerByIdUseCase = jigsawCustomerByIdUseCase;
            _getCouncilTaxAccountByAccountRefUseCase = getCouncilTaxAccountByAccountRefUseCase;
            _getHousingBenefitsAccountByAccountRefUseCase = getHousingBenefitsAccountByAccountRefUseCase;
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
                    case 4:
                        res = _getHousingBenefitsAccountByAccountRefUseCase.Execute(customerDataSource.SourceId, userToken).Result;
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
            var allContactDetails = new List<CustomerContactDetails>();
            var allPersonType = new List<string>();
            var allCautionaryAlerts = new List<CautionaryAlert>();
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
                    if (r.Customer.AllContactDetails != null)
                    {
                        allContactDetails.AddRange(r.Customer.AllContactDetails);
                    }
                    if (r.Customer.PersonTypes != null)
                    {
                        allPersonType.AddRange(r.Customer.PersonTypes);
                    }

                    if (r.Customer.CautionaryAlerts != null)
                    {
                        allCautionaryAlerts.AddRange(r.Customer.CautionaryAlerts);
                    }

                    mergedCustomer.Title ??= r.Customer.Title;
                    mergedCustomer.PreferredTitle ??= r.Customer.PreferredTitle;
                    mergedCustomer.PreferredFirstName ??= r.Customer.PreferredFirstName;
                    mergedCustomer.PreferredMiddleName ??= r.Customer.PreferredMiddleName;
                    mergedCustomer.PreferredSurname ??= r.Customer.PreferredSurname;
                    mergedCustomer.PlaceOfBirth ??= r.Customer.PlaceOfBirth;
                    mergedCustomer.NhsNumber ??= r.Customer.NhsNumber;
                    mergedCustomer.NiNo ??= r.Customer.NiNo;
                    mergedCustomer.PregnancyDueDate ??= r.Customer.PregnancyDueDate;
                    mergedCustomer.AccommodationTypeId ??= r.Customer.AccommodationTypeId;
                    mergedCustomer.HousingCircumstanceId ??= r.Customer.HousingCircumstanceId;
                    mergedCustomer.IsSettled ??= r.Customer.IsSettled;
                    mergedCustomer.SupportWorker ??= r.Customer.SupportWorker;
                    mergedCustomer.Gender ??= r.Customer.Gender;
                    mergedCustomer.IsAMinor ??= r.Customer.IsAMinor;
                    mergedCustomer.DateOfDeath ??= r.Customer.DateOfDeath;
                    mergedCustomer.CouncilTaxAccount ??= r.Customer.CouncilTaxAccount;
                    mergedCustomer.HousingBenefitsAccount ??= r.Customer.HousingBenefitsAccount;
                }
            }

            mergedCustomer.KnownAddresses = allKnownAddresses;
            mergedCustomer.AllContactDetails = allContactDetails;
            mergedCustomer.PersonTypes = allPersonType;
            mergedCustomer.CautionaryAlerts = allCautionaryAlerts;

            return new MergedCustomerResponseObject()
            {
                SystemIds = allSystemIds,
                Customer = mergedCustomer
            };

        }
    }
}
