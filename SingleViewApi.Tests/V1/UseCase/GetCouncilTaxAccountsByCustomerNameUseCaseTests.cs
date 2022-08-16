using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

public class GetCouncilTaxAccountsByCustomerNameUseCaseTests : LogCallAspectFixture
{
    private Mock<IAcademyGateway> _mockAcademyGateway;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;
    private GetCouncilTaxAccountsByCustomerNameUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]

    public void SetUp()
    {
        _mockAcademyGateway = new Mock<IAcademyGateway>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetCouncilTaxAccountsByCustomerNameUseCase(_mockAcademyGateway.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void ReturnsAnErrorWhenErrorReturnedFromGateway()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedDataSource = _fixture.Create<DataSource>();

        _mockAcademyGateway.Setup(x => x.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken))
            .ReturnsAsync(new CouncilTaxSearchResponseObject() { Error = "Test Error" });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(3)).Returns(stubbedDataSource);

        var result = _classUnderTest.Execute(firstName, lastName, userToken).Result;

        Assert.That(result.SystemIds[0].Error, Is.EqualTo("Test Error")); ;
    }

    [Test]
    public void ReturnsAnErrorWhenThereAreNoResults()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedDataSource = _fixture.Create<DataSource>();

        _mockAcademyGateway.Setup(x => x.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken))
            .ReturnsAsync(new CouncilTaxSearchResponseObject() { Error = null, Customers = new List<CouncilTaxSearchResponse>() });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(3)).Returns(stubbedDataSource);

        var result = _classUnderTest.Execute(firstName, lastName, userToken).Result;

        Assert.That(result.SystemIds[0].Error, Is.EqualTo("Not found")); ;
    }

    [Test]
    public void ReturnsSearchResultsFromAcademyCouncilTaxSearch()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedEntity = _fixture.Build<CouncilTaxSearchResponseObject>().Without(x => x.Error).Create();

        _mockAcademyGateway.Setup(x => x.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken)).ReturnsAsync(stubbedEntity);
        _mockDataSourceGateway.Setup(x => x.GetEntityById(3)).Returns(stubbedDataSource);


        var results = _classUnderTest.Execute(firstName, lastName, userToken).Result;

        results.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.SearchResponse.UngroupedResults[0].FirstName.Should().BeEquivalentTo(stubbedEntity.Customers[0].FirstName);
        results.SearchResponse.UngroupedResults[0].SurName.Should().BeEquivalentTo(stubbedEntity.Customers[0].LastName);
        results.SearchResponse.UngroupedResults[0].DateOfBirth.Should().Be(stubbedEntity.Customers[0].DateOfBirth);
        results.SearchResponse.UngroupedResults[0].DataSources[0].Should().BeEquivalentTo(stubbedDataSource.Name);

    }

}
