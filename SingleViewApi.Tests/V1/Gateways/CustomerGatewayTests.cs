using System;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;

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
    }
}
