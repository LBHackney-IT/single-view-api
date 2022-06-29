using AutoFixture;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

public class GetCouncilTaxAccountsByCustomerNameUseCaseTests
{
    private Mock<IAcademyGateway> _mockAcademyGateway;
    private GetCouncilTaxAccountsByCustomerNameUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]

    public void SetUp()
    {
        _mockAcademyGateway = new Mock<IAcademyGateway>();
        _classUnderTest = new GetCouncilTaxAccountsByCustomerNameUseCase(_mockAcademyGateway.Object);
        _fixture = new Fixture();
    }

}
