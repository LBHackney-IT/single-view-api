using System;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;

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
            var dummyFirstName = "Luna";
            var dummyLastName = "Kitty";
            var dummyDateOfBirth = DateTime.Parse("07/01/2021").ToUniversalTime();
            var dummyNiNumber = "SG00000000B";

            _ = _classUnderTest.Add(dummyFirstName, dummyLastName, dummyDateOfBirth, dummyNiNumber);

            var actual = SingleViewContext.Customers.ToList().LastOrDefault();

            Assert.AreEqual(dummyFirstName, actual.FirstName);
            Assert.AreEqual(dummyLastName, actual.LastName);
            Assert.AreEqual(dummyDateOfBirth, actual.DateOfBirth);
            Assert.AreEqual(dummyNiNumber, actual.NiNumber);
        }

        [Test]
        public void AddsValueWithoutNiNumber()
        {
            var dummyFirstName = "Luna";
            var dummyLastName = "Kitty";
            var dummyDateOfBirth = DateTime.Parse("07/01/2021").ToUniversalTime();

            _ = _classUnderTest.Add(dummyFirstName, dummyLastName, dummyDateOfBirth);

            var actual = SingleViewContext.Customers.ToList().LastOrDefault();

            Assert.AreEqual(dummyFirstName, actual.FirstName);
            Assert.AreEqual(dummyLastName, actual.LastName);
            Assert.AreEqual(dummyDateOfBirth, actual.DateOfBirth);
            Assert.AreEqual(null, actual.NiNumber);
        }


        [Test]
        public void FindSavedCustomer()
        {
            // Set up
            var expectedCustomer = SingleViewContext.Customers.Add(new SavedCustomer()
            {
                FirstName = "Luna",
                LastName = "Kitty",
                DateOfBirth = DateTime.Parse("07/01/2021").ToUniversalTime(),
                NiNumber = "SG00000000B"
            }.ToDatabase()).Entity;

            var personApi = new CustomerDataSource()
            {
                CustomerId = expectedCustomer.Id,
                DataSourceId = 1,
                SourceId = "abdc-1234-xyz0"
            };

            var jigsaw = new CustomerDataSource()
            {
                CustomerId = expectedCustomer.Id,
                DataSourceId = 2,
                SourceId = "1234"
            };

            SingleViewContext.CustomerDataSources.Add(personApi.ToDatabase());
            SingleViewContext.CustomerDataSources.Add(jigsaw.ToDatabase());

            SingleViewContext.SaveChanges();

            // Act
            var actual = _classUnderTest.Find(expectedCustomer.Id);

            // Assert
            Assert.AreEqual(expectedCustomer.FirstName, actual.FirstName);
            Assert.AreEqual(expectedCustomer.LastName, actual.LastName);
            Assert.AreEqual(expectedCustomer.DateOfBirth, actual.DateOfBirth);
            Assert.AreEqual(expectedCustomer.NiNumber, actual.NiNumber);
            Assert.AreEqual(2, actual.DataSources.Count);
            Assert.AreEqual(personApi.CustomerId, actual.DataSources[0].CustomerId);
            Assert.AreEqual(personApi.DataSourceId, actual.DataSources[0].DataSourceId);
            Assert.AreEqual(personApi.SourceId, actual.DataSources[0].SourceId);
            Assert.AreEqual(jigsaw.CustomerId, actual.DataSources[1].CustomerId);
            Assert.AreEqual(jigsaw.DataSourceId, actual.DataSources[1].DataSourceId);
            Assert.AreEqual(jigsaw.SourceId, actual.DataSources[1].SourceId);
        }
    }
}
