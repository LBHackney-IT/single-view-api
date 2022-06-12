using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Gateways
{
    public class CustomerDataSourceGateway : ICustomerDataSourceGateway
    {
        private readonly SingleViewContext _singleViewContext;

        public CustomerDataSourceGateway(SingleViewContext singleViewContext)
        {
            _singleViewContext = singleViewContext;
        }

        public CustomerDataSource Add(CustomerDataSource customerDataSource)
        {
            var entity = customerDataSource.ToDatabase();
            _singleViewContext.CustomerDataSources.Add(entity);
            _singleViewContext.SaveChanges();

            var result = _singleViewContext.CustomerDataSources.Find(entity.Id);

            return result.ToDomain();
        }
    }
}
