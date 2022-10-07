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

public class GetHousingBenefitsAccountByAccountRefUseCaseTests : LogCallAspectFixture
{
    private Mock<IAcademyGateway> _mockAcademyGateway;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;
    private GetHousingBenefitsAccountByAccountRefUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockAcademyGateway = new Mock<IAcademyGateway>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetHousingBenefitsAccountByAccountRefUseCase(_mockAcademyGateway.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public async Task GetsBenefitsAccount()
    {
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedHousingBenefitsRecordResponse = _fixture.Create<HousingBenefitsRecordResponseObject>();

        _mockAcademyGateway.Setup(x => x.GetHousingBenefitsAccountByAccountRef(id, userToken))
            .ReturnsAsync(stubbedHousingBenefitsRecordResponse);
        _mockDataSourceGateway.Setup(x => x.GetEntityById(4)).Returns(stubbedDataSource);

        var result = await _classUnderTest.Execute(id, userToken);

        result.Customer.Id.Should().BeEquivalentTo(id);
        result.Customer.Surname.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.LastName);
        result.Customer.FirstName.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.FirstName);
        result.Customer.HousingBenefitsAccount.ClaimId.Should().Be(stubbedHousingBenefitsRecordResponse.ClaimId);
        result.Customer.HousingBenefitsAccount.CheckDigit.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.CheckDigit);
        result.Customer.HousingBenefitsAccount.PersonReference.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.PersonReference);
        result.Customer.HousingBenefitsAccount.HouseholdMembers.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.HouseholdMembers);
        result.Customer.HousingBenefitsAccount.Benefits.Should().BeEquivalentTo(stubbedHousingBenefitsRecordResponse.Benefits);

    }
}
