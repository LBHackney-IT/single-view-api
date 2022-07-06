using System.Threading.Tasks;
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

public class GetCouncilTaxAccountByIdUseCaseTests : LogCallAspectFixture
{
    private Mock<IAcademyGateway> _mockAcademyGateway;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;
    private GetCouncilTaxAccountByIdUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockAcademyGateway = new Mock<IAcademyGateway>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetCouncilTaxAccountByIdUseCase(_mockAcademyGateway.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public async Task GetsCouncilTaxAccount()
    {
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedCouncilTaxAccount = _fixture.Create<CouncilTaxRecordResponseObject>();

        _mockAcademyGateway.Setup(x => x.GetCouncilTaxAccountById(id, userToken))
            .ReturnsAsync(stubbedCouncilTaxAccount);
        _mockDataSourceGateway.Setup(x => x.GetEntityById(3)).Returns(stubbedDataSource);

        var result = await _classUnderTest.Execute(id, userToken);

        result.Customer.Id.Should().BeEquivalentTo(stubbedCouncilTaxAccount.AccountReference.ToString());
        result.Customer.Surname.Should().BeEquivalentTo(stubbedCouncilTaxAccount.LastName);
        result.Customer.FirstName.Should().BeEquivalentTo(stubbedCouncilTaxAccount.FirstName);
        result.Customer.CouncilTaxAccount.AccountBalance.Should().Be(stubbedCouncilTaxAccount.AccountBalance);
        result.Customer.CouncilTaxAccount.PropertyAddress.Should().BeEquivalentTo(stubbedCouncilTaxAccount.PropertyAddress);
        result.Customer.CouncilTaxAccount.ForwardingAddress.Should().BeEquivalentTo(stubbedCouncilTaxAccount.ForwardingAddress);


    }
}
