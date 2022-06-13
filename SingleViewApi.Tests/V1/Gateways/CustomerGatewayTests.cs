using System;
using System.Diagnostics;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ServiceStack.Redis;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class CustomerTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private CustomerGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new CustomerGateway(SingleViewContext);
        }

        [Test]
        public void AddsValue()
        {
            var customer = _fixture.Create<SavedCustomer>();

            _ = _classUnderTest.Add(customer);

            var actual = SingleViewContext.Customers.ToList().LastOrDefault();

            Assert.AreEqual(customer.FirstName, actual.FirstName);
            Assert.AreEqual(customer.LastName, actual.LastName);

        }

    }
}
