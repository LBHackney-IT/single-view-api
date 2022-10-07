using System;
using System.Collections.Generic;
using AutoFixture;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

public class CreateCustomerUseCaseTests : LogCallAspectFixture
{
    private CreateCustomerUseCase _classUnderTest;
    private Fixture _fixture;
    private Mock<ICustomerDataSourceGateway> _mockCustomerDataSourceGateway;
    private Mock<ICustomerGateway> _mockCustomerGateway;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;


    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _mockCustomerGateway = new Mock<ICustomerGateway>();
        _mockCustomerDataSourceGateway = new Mock<ICustomerDataSourceGateway>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new CreateCustomerUseCase(_mockCustomerGateway.Object, _mockCustomerDataSourceGateway.Object,
            _mockDataSourceGateway.Object);
    }

    [Test]
    public void CreatesCustomer()
    {
        var mockCustomerRequest = new CreateCustomerRequest
        {
            FirstName = "Luna",
            LastName = "Kitty",
            DateOfBirth = DateTime.Parse("07/01/2021").ToUniversalTime(),
            NiNumber = "SG00000000B",
            DataSources = new List<DataSourceRequest>
            {
                new() { DataSource = "Jigsaw", SourceId = "12345" },
                new() { DataSource = "PersonAPI", SourceId = "abcd-1234" }
            }
        };

        var fakeSavedCustomer = new SavedCustomer
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

        _mockDataSourceGateway.Setup(x => x.GetAll()).Returns(new List<DataSource>
        {
            new() { Id = 1, Name = "PersonAPI" }, new() { Id = 2, Name = "Jigsaw" }
        });

        var invocations = new List<CustomerDataSourceParams>();

        _mockCustomerDataSourceGateway
            .Setup(x => x.Add(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .Callback<Guid, int, string>((customerId, dataSourceId, sourceId) =>
                invocations.Add(new CustomerDataSourceParams
                {
                    CustomerId = customerId, DataSourceId = dataSourceId, SourceId = sourceId
                }))
            .Returns(_fixture.Create<CustomerDataSource>());


        var result = _classUnderTest.Execute(mockCustomerRequest);

        _mockCustomerGateway.Verify(
            x => x.Add(mockCustomerRequest.FirstName, mockCustomerRequest.LastName, mockCustomerRequest.DateOfBirth,
                mockCustomerRequest.NiNumber), Times.Once);

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
