using System;
using System.Linq;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;

namespace SingleViewApi.V1.UseCase
{
    public class GetCustomerByIdUseCase : IGetCustomerByIdUseCase
    {
        private readonly ICustomerGateway _gateway;
        private readonly IGetPersonApiByIdUseCase _getPersonApiByIdUseCase;
        private readonly IGetJigsawCustomerByIdUseCase _jigsawCustomerByIdUseCase;

        public GetCustomerByIdUseCase(ICustomerGateway gateway, IGetPersonApiByIdUseCase getPersonApiByIdUseCase, IGetJigsawCustomerByIdUseCase jigsawCustomerByIdUseCase)
        {
            _gateway = gateway;
            _getPersonApiByIdUseCase = getPersonApiByIdUseCase;
            _jigsawCustomerByIdUseCase = jigsawCustomerByIdUseCase;
        }

        [LogCall]
        public CustomerResponseObject Execute(Guid customerId, string userToken, string redisId = null)
        {
            var customer = _gateway.Find(customerId);

            CustomerResponseObject personApiCustomer = null;
            CustomerResponseObject jigsawCustomer = null;
            foreach (var customerDataSource in customer.DataSources)
            {
                switch (customerDataSource.DataSourceId)
                {
                    case 1:
                        personApiCustomer = _getPersonApiByIdUseCase.Execute(customerDataSource.SourceId, userToken).Result;
                        break;
                    case 2:
                        jigsawCustomer = _jigsawCustomerByIdUseCase
                            .Execute(customerDataSource.SourceId, redisId, userToken).Result;
                        break;
                }
            }

            return new CustomerResponseObject()
            {
                Customer = new Customer()
                {
                    Title = personApiCustomer?.Customer.Title ?? jigsawCustomer?.Customer.Title,
                    PreferredTitle = personApiCustomer?.Customer.PreferredTitle ?? jigsawCustomer?.Customer.PreferredTitle,
                    PreferredFirstName = $"{personApiCustomer?.Customer.PreferredFirstName} / {jigsawCustomer?.Customer.PreferredFirstName}",
                    PreferredSurname = $"{personApiCustomer?.Customer.PreferredSurname} / {jigsawCustomer?.Customer.PreferredSurname}",
                    FirstName = $"{personApiCustomer?.Customer.FirstName} / {jigsawCustomer?.Customer.FirstName}",
                    Surname = $"{personApiCustomer?.Customer.Surname} / {jigsawCustomer?.Customer.Surname}",
                    KnownAddresses = personApiCustomer.Customer.KnownAddresses.Concat(jigsawCustomer.Customer.KnownAddresses).ToList(),
                    ContactDetails = personApiCustomer.Customer.ContactDetails,
                    PersonTypes = personApiCustomer.Customer.PersonTypes,
                    EqualityInformation = personApiCustomer.Customer.EqualityInformation,
                    DateOfBirth = customer.DateOfBirth,
                    NiNo = customer.NiNumber,
                    NhsNumber = jigsawCustomer.Customer.NhsNumber,
                    DateOfDeath = personApiCustomer.Customer.DateOfDeath,
                    IsAMinor = personApiCustomer.Customer.IsAMinor
                },
            };
        }
    }
}
