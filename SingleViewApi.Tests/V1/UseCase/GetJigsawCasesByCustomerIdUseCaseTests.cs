using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase;

public class GetJigsawCasesByCustomerIdUseCaseTests : LogCallAspectFixture
{
    private Mock<IJigsawGateway> _mockJigsawGateway;
    private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
    private GetJigsawCasesByCustomerIdUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _classUnderTest =
            new GetJigsawCasesByCustomerIdUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void ReturnsNullIfThereIsNoJigsawToken()
    {
        var redisId = _fixture.Create<string>();
        var customerId = _fixture.Create<string>();
        var hackneyToken = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken))
            .ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "No token present" });

        var result = _classUnderTest.Execute(customerId, redisId, hackneyToken).Result;

        Assert.IsNull(result);
    }

    [Test]
    public void ReturnsACaseResponseObject()
    {
        var redisId = _fixture.Create<string>();
        var customerId = _fixture.Create<string>();
        var hackneyToken = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        var mockCustomerCases = _fixture.Create<JigsawCasesResponseObject>();
        var mockCurrentCustomerCaseId = mockCustomerCases.Cases[0].Id;
        var mockCustomerCaseOverviews = _fixture.Build<JigsawCaseOverviewResponseObject>()
            .With(x => x.CustomerId, mockCurrentCustomerCaseId).Create();
        var mockCustomerPlacements = _fixture.Create<JigsawCasePlacementInformationResponseObject>();

        var mockAdditionalFactors = new JigsawCaseAdditionalFactorsResponseObject()
        {
            QuestionGroups = new List<QuestionGroup>()
            {
                new QuestionGroup()
                {
                    Legend = "Overview of additional factors",
                    Questions = new List<Question>()
                    {
                        new Question()
                        {
                            SelectedValue = "AN_NO",
                            Label = "* Drug/alcohol use?",
                            Options = new List<Option>()
                            {
                                new Option()
                                {
                                    Label = "Yes",
                                    Value = "AN_YES"
                                }, new Option()
                                {
                                    Label = "No",
                                    Value = "AN_NO"
                                }
                            }
                        }, new Question()
                        {
                            SelectedValue = "Paracetamol",
                            Label = "* Detail current medication and dosage for all household members"
                        }
                    }
                }
            }
        };

        var mockHealthAndWellBeing = new JigsawCaseAdditionalFactorsResponseObject()
        {
            QuestionGroups = new List<QuestionGroup>()
            {
                new QuestionGroup()
                {
                    Legend = "Health and well being",
                    Questions = new List<Question>()
                    {
                        new Question()
                        {
                            SelectedValue = "AN_YES",
                            Label = "Do you or any of your household members have any self-reported vulnerabilities?",
                            Options = new List<Option>()
                            {
                                new Option()
                                {
                                    Label = "Yes",
                                    Value = "AN_YES"
                                }, new Option()
                                {
                                    Label = "No",
                                    Value = "AN_NO"
                                }
                            }
                        }, new Question()
                        {
                            SelectedValue = "wheelchair/depression",
                            Label = "If yes, please provide details:"
                        }
                    }
                }
            }
        };
        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockJigsawGateway.Setup(x => x.GetCasesByCustomerId(customerId, jigsawToken)).ReturnsAsync(mockCustomerCases);
        _mockJigsawGateway.Setup(x => x.GetCaseOverviewByCaseId(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockCustomerCaseOverviews);
        _mockJigsawGateway.Setup(x => x.GetCaseAccommodationPlacementsByCaseId(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockCustomerPlacements);

        _mockJigsawGateway.Setup(x => x.GetCaseAdditionalFactors(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockAdditionalFactors);
        _mockJigsawGateway.Setup(x => x.GetCaseHealthAndWellBeing(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockHealthAndWellBeing);


        var result = _classUnderTest.Execute(customerId, redisId, hackneyToken).Result;

        result.CurrentCase.Should().BeEquivalentTo(mockCustomerCases.Cases[0]);
        result.CaseOverview.Id.Should().BeEquivalentTo(mockCustomerCaseOverviews.Id.ToString());
        result.CaseOverview.HouseHoldComposition.Should().BeEquivalentTo(mockCustomerCaseOverviews.HouseholdComposition);
        result.PlacementInformation[0].DclgClassificationType.Should()
            .BeEquivalentTo(mockCustomerPlacements.Placements[0].DclgClassificationType);
        result.AdditionalFactors[0].Legend.Should().BeEquivalentTo("Overview of additional factors");
        result.AdditionalFactors[0].Info[0].Question.Should().BeEquivalentTo("Drug/alcohol use?");
        result.AdditionalFactors[0].Info[0].Answer.Should().BeEquivalentTo("No");
        result.AdditionalFactors[0].Info[1].Question.Should().BeEquivalentTo("Detail current medication and dosage for all household members");
        result.AdditionalFactors[0].Info[1].Answer.Should().BeEquivalentTo("Paracetamol");

        result.HealthAndWellBeing[0].Legend.Should().BeEquivalentTo("Health and well being");
        result.HealthAndWellBeing[0].Info[0].Question.Should().BeEquivalentTo("Do you or any of your household members have any self-reported vulnerabilities?");
        result.HealthAndWellBeing[0].Info[0].Answer.Should().BeEquivalentTo("Yes");
        result.HealthAndWellBeing[0].Info[1].Question.Should().BeEquivalentTo("If yes, please provide details:");
        result.HealthAndWellBeing[0].Info[1].Answer.Should().BeEquivalentTo("wheelchair/depression");
    }
}
