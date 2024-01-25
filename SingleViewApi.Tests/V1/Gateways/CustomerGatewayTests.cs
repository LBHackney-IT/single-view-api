using System;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;
using ServiceStack;
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

        [Test]
        public void SearchSavedCustomer()
        {
            // Set up
            _ = SingleViewContext.Customers.Add(new SavedCustomer()
            {
                FirstName = "Testy",
                LastName = "McTestFace",
                DateOfBirth = DateTime.ParseExact("18/08/1996", "dd/MM/yyyy", null).ToUniversalTime(),
                NiNumber = "SG01010101B"
            }.ToDatabase()).Entity;

            var expectedCustomer = SingleViewContext.Customers.Add(new SavedCustomer()
            {
                FirstName = "Luna",
                LastName = "Kitty",
                DateOfBirth = DateTime.ParseExact("07/01/2021", "dd/MM/yyyy", null).ToUniversalTime(),
                NiNumber = "SG00000000B"
            }.ToDatabase()).Entity;

            SingleViewContext.SaveChanges();

            // Act
            var actual = _classUnderTest.Search("Luna", "Kitty");

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expectedCustomer.FirstName, actual[0].FirstName);
            Assert.AreEqual(expectedCustomer.LastName, actual[0].LastName);
            Assert.AreEqual(expectedCustomer.DateOfBirth, actual[0].DateOfBirth);
            Assert.AreEqual(expectedCustomer.NiNumber, actual[0].NiNumber);
        }

        [Test]
        public void SearchSavedCustomerIgnoreCase()
        {
            // Set up
            var expectedCustomer = SingleViewContext.Customers.Add(new SavedCustomer()
            {
                FirstName = "Testy",
                LastName = "McTestFace",
                DateOfBirth = DateTime.ParseExact("18/08/1996", "dd/MM/yyyy", null).ToUniversalTime(),
                NiNumber = "SG01010101B"
            }.ToDatabase()).Entity;

            _ = SingleViewContext.Customers.Add(new SavedCustomer()
            {
                FirstName = "Luna",
                LastName = "Kitty",
                DateOfBirth = DateTime.ParseExact("07/01/2021", "dd/MM/yyyy", null).ToUniversalTime(),
                NiNumber = "SG00000000B"
            }.ToDatabase()).Entity;

            SingleViewContext.SaveChanges();

            // Act
            var actual = _classUnderTest.Search("testy", "testface");

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expectedCustomer.FirstName, actual[0].FirstName);
            Assert.AreEqual(expectedCustomer.LastName, actual[0].LastName);
            Assert.AreEqual(expectedCustomer.DateOfBirth, actual[0].DateOfBirth);
            Assert.AreEqual(expectedCustomer.NiNumber, actual[0].NiNumber);
        }
    }
}
