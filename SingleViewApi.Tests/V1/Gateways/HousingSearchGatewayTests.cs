using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class HousingSearchGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private HousingSearchGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]

        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://housingsearch.api";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new HousingSearchGateway(mockClient, baseUrl);

        }

        [Test]

        public void ARequestIsMade()
        {
            // Arrange
            const string searchText = "Search-Text";
            const string userToken = "User token";
            const int page = 1;

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}?page={page}")
                .WithHeaders("Authorization", userToken);
            // Act
            _ = _classUnderTest.GetSearchResultsBySearchText(searchText, page, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]

        public async Task GetSearchResultsBySearchTextReturnsNullIfThereAreNoResults()
        {
            //Arrange

            const string searchText = "Test";
            const string userToken = "User token";
            const int page = 1;

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}?page={page}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(searchText));

            //Act
            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, page, userToken);

            //Assert

            searchResults.Should().BeNull();
        }

        [Test]
        public async Task GetSearchResultsBySearchTextReturnsNullIfUserIsUnAuthorised()
        {
            // Arrange
            const string searchText = "Test";
            const string userToken = "User token";
            const int page = 1;

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}?page={page}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(searchText));

            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, page, userToken);

            searchResults.Should().BeNull();
        }

        [Test]
        public async Task GetSearchResultsBySearchTextReturnsNullIfApiIsDown()
        {
            // Arrange
            const string searchText = "Test";
            const string userToken = "User token";
            const int page = 1;

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}?page={page}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(searchText));

            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, page, userToken);

            searchResults.Should().BeNull();
        }

        [Test]

        public async Task DataFromApiIsReturned()
        {
            const string searchText = "Test";
            const string userToken = "User token";
            const int page = 1;

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}?page={page}")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json", "{\r\n    \"results\": {\r\n        \"persons\": [\r\n            {\r\n                \"id\": \"cbd7d7b7-c68a-4741-898d-36492b85f189\",\r\n                \"title\": \"Mrs\",\r\n                \"firstname\": \"test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1990-12-12\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"6bc70564-b4f3-4dd1-ac36-26f3572d1d61\",\r\n                        \"type\": \"Commercial Let\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2022-10-10T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Psp 36 Nelson Mandela House 124 Cazenove Road Hackney London N16 6AJ\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"58597f46-bab7-45e5-a525-a17999916b71\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1999-01-01\",\r\n                \"personTypes\": [\r\n                    \"HouseholdMember\",\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"c48fd0c1-4ac7-2efa-abf4-c437473c8ab8\",\r\n                        \"type\": \"Freehold (Serv)\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2006-12-14T00:00:00Z\",\r\n                        \"endDate\": \"1900-01-01T00:00:00Z\",\r\n                        \"assetFullAddress\": \"62 Dove Row E2 8RJ\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    },\r\n                    {\r\n                        \"id\": \"46bc6010-7f51-49e2-842f-04d457eda29c\",\r\n                        \"type\": \"Temp Decant\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"1999-09-09T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 3 Narford Road Hackney London E5 8RD\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"9ae759a8-2305-4406-bd13-686a866224a6\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"2001-01-01\",\r\n                \"personTypes\": null,\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"8508e0ae-7981-9f07-1a22-a2c585acb810\",\r\n                        \"type\": \"Temp Hostel Lse\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2001-09-17T00:00:00\",\r\n                        \"endDate\": \"2002-05-12T00:00:00\",\r\n                        \"assetFullAddress\": \"8b 67a Stoke Newington Road N16 8AH\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": \"9995360516\",\r\n                        \"isActive\": false\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"01914132-1e1f-48de-a51b-818bd99e16b7\",\r\n                \"title\": \"Miss\",\r\n                \"firstname\": \"test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1990-11-22\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"1d7e6192-5697-51c1-760b-593a9e61303b\",\r\n                        \"type\": \"Private Garage\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2020-07-06T00:00:00.0000000Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 12 Pitcairn House  St Thomass Square E9 6PT\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"c7e72569-d211-48e5-8a78-5294e4ea880f\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Testy\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1969-01-01\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"00c69bed-d61a-4184-ae36-d598d22ae63c\",\r\n                        \"type\": \"Asylum Seeker\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2022-01-01T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 2 Cresset House Retreat Place Hackney London E9 6RW\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"02afae46-6515-471a-8763-acd99caabae4\",\r\n                \"title\": \"Mrs\",\r\n                \"firstname\": \"second\",\r\n                \"middleName\": null,\r\n                \"surname\": \"test\",\r\n                \"preferredFirstname\": \"\",\r\n                \"preferredSurname\": \"\",\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"2001-01-01T00:00:00.0000000Z\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"64c74310-3234-aa93-3701-ab5fe0a4295a\",\r\n                        \"type\": \"Temp Hostel\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2007-07-23T00:00:00\",\r\n                        \"endDate\": \"2007-08-12T00:00:00\",\r\n                        \"assetFullAddress\": \"Flat 41 Glebelands 16 Clissold Road N16 9ER\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": false\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"6991c25f-c835-459a-87f5-a65fd9d31cce\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Edit\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1969-01-01\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": []\r\n            },\r\n            {\r\n                \"id\": \"d3c76382-2b0d-42f4-9172-a81c5fd75d2a\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Tony\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Test\",\r\n                \"preferredFirstname\": \"Tony\",\r\n                \"preferredSurname\": \"Test\",\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1985-07-24\",\r\n                \"personTypes\": null,\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"957cc50e-2dc4-e782-a013-c0a331884e49\",\r\n                        \"type\": \"Secure\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"1987-02-02T00:00:00Z\",\r\n                        \"endDate\": \"2000-11-12T00:00:00Z\",\r\n                        \"assetFullAddress\": \"189 Lea View House  Springfield E5 9EA\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": false\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"f3d1dd30-b98e-4bc2-b6e6-a10f2abc770b\",\r\n                \"title\": \"Master\",\r\n                \"firstname\": \"nicola\",\r\n                \"middleName\": null,\r\n                \"surname\": \"test\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1990-10-22\",\r\n                \"personTypes\": [\r\n                    \"Tenant\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"0d4386c1-13ef-6642-cedd-24e804b9ddf6\",\r\n                        \"type\": \"Tenant Garage\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2019-02-25T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 35 Blandford Court  St Peters Way N1 4SA\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"c36ec7a3-8c64-48e8-978b-2a00568b192c\",\r\n                \"title\": \"Mrs\",\r\n                \"firstname\": \"test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Testagain\",\r\n                \"preferredFirstname\": \"Test\",\r\n                \"preferredSurname\": \"Testagain\",\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1999-01-16\",\r\n                \"personTypes\": null,\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"67bb8b7d-db11-ef67-5b10-8072c84bb7b5\",\r\n                        \"type\": \"Secure\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"1986-01-13T00:00:00\",\r\n                        \"endDate\": \"1900-01-01T00:00:00\",\r\n                        \"assetFullAddress\": \"5 Marlowe House  Milton Gardens Estate N16 8TW\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"00755752-60fb-4366-ae3a-880bc71545f7\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Mc Tester Face\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1970-06-09\",\r\n                \"personTypes\": [\r\n                    \"HouseholdMember\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"a6311713-6745-b291-9207-211ecc320c04\",\r\n                        \"type\": \"Secure\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2015-07-13T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"32 Broadway House  Jackman Street E8 4QY\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    },\r\n                    {\r\n                        \"id\": \"46bc6010-7f51-49e2-842f-04d457eda29c\",\r\n                        \"type\": \"Temp Decant\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"1999-09-09T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 3 Narford Road Hackney London E5 8RD\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            },\r\n            {\r\n                \"id\": \"8b2faa61-0257-4b42-b3a3-8f5cfd7b0459\",\r\n                \"title\": \"Mr\",\r\n                \"firstname\": \"Test\",\r\n                \"middleName\": null,\r\n                \"surname\": \"Mc Tester Face\",\r\n                \"preferredFirstname\": null,\r\n                \"preferredSurname\": null,\r\n                \"totalBalance\": 0.0,\r\n                \"dateOfBirth\": \"1970-06-09\",\r\n                \"personTypes\": [\r\n                    \"HouseholdMember\"\r\n                ],\r\n                \"isPersonCautionaryAlert\": false,\r\n                \"isTenureCautionaryAlert\": false,\r\n                \"tenures\": [\r\n                    {\r\n                        \"id\": \"a6311713-6745-b291-9207-211ecc320c04\",\r\n                        \"type\": \"Secure\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"2015-07-13T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"32 Broadway House  Jackman Street E8 4QY\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    },\r\n                    {\r\n                        \"id\": \"46bc6010-7f51-49e2-842f-04d457eda29c\",\r\n                        \"type\": \"Temp Decant\",\r\n                        \"totalBalance\": 0.0,\r\n                        \"startDate\": \"1999-09-09T00:00:00Z\",\r\n                        \"endDate\": null,\r\n                        \"assetFullAddress\": \"Gge 3 Narford Road Hackney London E5 8RD\",\r\n                        \"postCode\": null,\r\n                        \"paymentReference\": null,\r\n                        \"isActive\": true\r\n                    }\r\n                ]\r\n            }\r\n        ]\r\n    },\r\n    \"total\": 737\r\n}");

            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, page, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();

            Assert.AreEqual("test", searchResults.Results.Persons[0].Firstname);
            Assert.AreEqual("test", searchResults.Results.Persons[0].Surname);
            Assert.AreEqual(737, searchResults.Total);

        }


    }
}
