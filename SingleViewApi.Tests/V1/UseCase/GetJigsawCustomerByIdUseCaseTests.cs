using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase;

[TestFixture]
public class GetJigsawCustomerByIdUseCaseTest
{
    private Mock<IJigsawGateway> _mockJigsawGateway;
    private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
    private GetJigsawCustomerByIdUseCase _classUnderTest;
    private Fixture _fixture;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetJigsawCustomerByIdUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void UseCaseReturnsNullIfAuthFails()
    {
        string redisId = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        string stubbedCustomerId = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "No token present" }); ;

        var result = _classUnderTest.Execute(stubbedCustomerId, redisId, hackneyToken).Result;

        Assert.That(result, Is.EqualTo(null));

    }

    [Test]
    public void ReturnsCustomerResponseObject()
    {
        var redisId = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        var stubbedEntity = _fixture.Create<JigsawCustomerResponseObject>();
        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedCustomerId = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedDataSource);

        _mockJigsawGateway.Setup(x => x.GetCustomerById(stubbedCustomerId, jigsawToken)).ReturnsAsync(stubbedEntity);

        var results = _classUnderTest.Execute(stubbedCustomerId, redisId, hackneyToken).Result;

        results.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.SystemIds[^1].Id.Should().BeEquivalentTo(stubbedEntity.Id);
        results.Customer.FirstName.Should().BeEquivalentTo(stubbedEntity.PersonInfo.FirstName);
        results.Customer.Surname.Should().BeEquivalentTo(stubbedEntity.PersonInfo.LastName);
        results.Customer.DateOfBirth.Should().Be(stubbedEntity.PersonInfo.DateOfBirth);
        results.Customer.DataSource.Should().Be(stubbedDataSource);
        results.Customer.NiNo.Should().BeEquivalentTo(stubbedEntity.PersonInfo.NationalInsuranceNumber);
        results.Customer.NhsNumber.Should().BeEquivalentTo(stubbedEntity.PersonInfo.NhsNumber);
        results.Customer.KnownAddresses[0].FullAddress.Should().BeEquivalentTo(stubbedEntity.PersonInfo.AddressString);
        results.Customer.KnownAddresses[0].CurrentAddress.Should().Be(true);
        results.Customer.KnownAddresses[0].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
    }
}
