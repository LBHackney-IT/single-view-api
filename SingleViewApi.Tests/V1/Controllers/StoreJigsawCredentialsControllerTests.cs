using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using System = Bogus.DataSets.System;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class StoreJigsawCredentialsControllerTests
    {
        private StoreJigsawCredentialsController _classUnderTest;
        private Mock<IStoreJigsawCredentialsUseCase> _mockStoreJigsawCredentialsUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockStoreJigsawCredentialsUseCase = new Mock<IStoreJigsawCredentialsUseCase>();
            _classUnderTest = new StoreJigsawCredentialsController(_mockStoreJigsawCredentialsUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string encryptedCredentials = "Test-creds";

            _ = _classUnderTest.StoreJigsawCredentials(encryptedCredentials);

            _mockStoreJigsawCredentialsUseCase.Verify(x => x.Execute(encryptedCredentials), Times.Once);
        }

        [Test]
        public void ControllerReturnsUnauthorisedWhenCredentialsAreIncorrect()
        {
            const string encryptedCredentials = "Incorrect-Credentials";
            UnauthorizedObjectResult expectedResult = new UnauthorizedObjectResult("Credentials are incorrect");

            _mockStoreJigsawCredentialsUseCase.Setup(e => e.Execute(encryptedCredentials))
                .Returns<string>((s) => "");

            var result = _classUnderTest.StoreJigsawCredentials(encryptedCredentials);

            result.Should().BeEquivalentTo(expectedResult);

        }



    }
}


