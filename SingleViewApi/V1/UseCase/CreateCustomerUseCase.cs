using System;
using System.Collections.Generic;
using System.Linq;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class CreateCustomerUseCase : ICreateCustomerUseCase
    {
        private readonly ICustomerGateway _customerGateway;
        private readonly ICustomerDataSourceGateway _customerDataSourceGateway;
        private readonly IDataSourceGateway _dataSourceGateway;

        public CreateCustomerUseCase(ICustomerGateway customerGateway, ICustomerDataSourceGateway customerDataSourceGateway, IDataSourceGateway dataSourceGateway)
        {
            _customerGateway = customerGateway;
            _customerDataSourceGateway = customerDataSourceGateway;
            _dataSourceGateway = dataSourceGateway;
        }

        [LogCall]
        public SavedCustomer Execute(CreateCustomerRequest customerRequest)
        {
            var customer = _customerGateway.Add(
               customerRequest.FirstName,
              customerRequest.LastName, customerRequest.DateOfBirth,
              customerRequest.NiNumber
            );

            customer.DataSources = new List<CustomerDataSource>();

            var dataSources = _dataSourceGateway.GetAll();

            foreach (var customerRequestDataSource in customerRequest.DataSources)
            {
                var dataSource = dataSources.FirstOrDefault(d => String.Equals(d.Name, customerRequestDataSource.DataSource, StringComparison.CurrentCultureIgnoreCase));

                if (dataSource != null)
                {

                    var savedCustomerDataSource = _customerDataSourceGateway.Add(
                        customer.Id,
                        dataSource.Id,
                        customerRequestDataSource.SourceId
                    );

                    customer.DataSources.Add(savedCustomerDataSource);
                }
            }

            return customer;
        }
    }
}
