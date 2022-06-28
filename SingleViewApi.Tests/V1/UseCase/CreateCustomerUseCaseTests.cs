using System;
using System.Collections.Generic;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class CreateCustomerUseCaseTests : LogCallAspectFixture
    {
        private CreateCustomerUseCase _classUnderTest;
        private Mock<ICustomerGateway> _mockCustomerGateway;
        private Mock<ICustomerDataSourceGateway> _mockCustomerDataSourceGateway;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;
        private Fixture _fixture;


        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mockCustomerGateway = new Mock<ICustomerGateway>();
            _mockCustomerDataSourceGateway = new Mock<ICustomerDataSourceGateway>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();
            _classUnderTest = new CreateCustomerUseCase(_mockCustomerGateway.Object, _mockCustomerDataSourceGateway.Object, _mockDataSourceGateway.Object);
        }

        [Test]
        public void CreatesCustomer()
        {
            var mockCustomerRequest = new CreateCustomerRequest()
            {
                FirstName = "Luna",
                LastName = "Kitty",
                DateOfBirth = DateTime.Parse("07/01/2021").ToUniversalTime(),
                NiNumber = "SG00000000B",
                DataSources = new List<DataSourceRequest>
                {
                    new DataSourceRequest()
                    {
                        DataSource = "Jigsaw", SourceId = "12345"
                    }, new DataSourceRequest()
                    {
                        DataSource = "PersonAPI", SourceId = "abcd-1234"
                    }
                }
            };

            var fakeSavedCustomer = new SavedCustomer()
            {
                Id = new Guid(),
                FirstName = mockCustomerRequest.FirstName,
                LastName = mockCustomerRequest.LastName,
                DateOfBirth = mockCustomerRequest.DateOfBirth,
                NiNumber = mockCustomerRequest.NiNumber
            };

            _mockCustomerGateway.Setup(x => x.Add(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()
                )).Returns(fakeSavedCustomer);

            _mockDataSourceGateway.Setup(x => x.GetAll()).Returns(new List<DataSource>()
            {
                new DataSource() { Id = 1, Name = "PersonAPI" }, new DataSource() { Id = 2, Name = "Jigsaw" }
            });

            var invocations = new List<CustomerDataSourceParams>();

            _mockCustomerDataSourceGateway
                .Setup(x => x.Add(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback<Guid, int, string>((customerId, dataSourceId, sourceId) => invocations.Add(new CustomerDataSourceParams { CustomerId = customerId, DataSourceId = dataSourceId, SourceId = sourceId }))
                .Returns(_fixture.Create<CustomerDataSource>());


            var result = _classUnderTest.Execute(mockCustomerRequest);

            _mockCustomerGateway.Verify(x => x.Add(mockCustomerRequest.FirstName, mockCustomerRequest.LastName, mockCustomerRequest.DateOfBirth, mockCustomerRequest.NiNumber), Times.Once);

            Assert.AreEqual(2, invocations.Count);

            Assert.AreEqual(fakeSavedCustomer.Id, invocations[0].CustomerId);
            Assert.AreEqual(2, invocations[0].DataSourceId);
            Assert.AreEqual(mockCustomerRequest.DataSources[0].SourceId, invocations[0].SourceId);
            Assert.AreEqual(fakeSavedCustomer.Id, invocations[1].CustomerId);
            Assert.AreEqual(1, invocations[1].DataSourceId);
            Assert.AreEqual(mockCustomerRequest.DataSources[1].SourceId, invocations[1].SourceId);

            Assert.AreEqual(2, result.DataSources.Count);
        }

        public struct CustomerDataSourceParams
        {
            public Guid CustomerId { get; set; }
            public int DataSourceId { get; set; }
            public string SourceId { get; set; }
        }


    }
}

