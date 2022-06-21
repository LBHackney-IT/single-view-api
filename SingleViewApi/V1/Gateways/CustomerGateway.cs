using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Gateways
{
    public class CustomerGateway : ICustomerGateway
    {
        private readonly SingleViewContext _singleViewContext;

        public CustomerGateway(SingleViewContext singleViewContext)
        {
            _singleViewContext = singleViewContext;
        }

        public SavedCustomer Add(string firstName, string lastName, DateTime dateOfBirth, string niNumber = null)
        {
            var entity = new SavedCustomer()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                NiNumber = niNumber
            }.ToDatabase();
            _singleViewContext.Customers.Add(entity);
            _singleViewContext.SaveChanges();

            var result = _singleViewContext.Customers.Find(entity.Id);

            return result.ToDomain();
        }

        public SavedCustomer Find(Guid id)
        {
            var result = _singleViewContext.Customers
                .Include(c => c.DataSources)
                .FirstOrDefault(c => c.Id == id);

            return result.ToDomain();
        }

        public List<SavedCustomer> Search(string firstName, string lastName)
        {
            var customers = from c in _singleViewContext.Customers
                            where c.LastName.ToLower().Contains(lastName.ToLower()) && c.FirstName.ToLower().Contains(firstName.ToLower())
                            select c;

            return customers.ToList().Map(c => c.ToDomain());
        }

    }
}
