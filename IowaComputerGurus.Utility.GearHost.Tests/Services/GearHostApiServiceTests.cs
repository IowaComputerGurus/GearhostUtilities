using System.Collections.Generic;
using System.Collections.Specialized;
using IowaComputerGurus.Utility.GearHost.Models;
using IowaComputerGurus.Utility.GearHost.Providers;
using IowaComputerGurus.Utility.GearHost.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IowaComputerGurus.Utility.GearHost.Tests.Services
{
    [TestClass]
    public class GearHostApiServiceTests
    {
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IHttpClientService> _httpClientServiceMock;
        private IGearHostApiService _service;
        private readonly string TestKey = "TestKey";
        private readonly string TestUrl = "http://Test/";

        [TestInitialize]
        public void Setup()
        {
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _service = new GearHostApiService(_configurationProviderMock.Object, _httpClientServiceMock.Object);
        }

        private void ConfigureTestConfig()
        {
            var config = new NameValueCollection {{"GearHostApiKey", TestKey}, {"GearHostApiBaseUrl", TestUrl}};
            _configurationProviderMock.Setup(cp => cp.AppSettings).Returns(config);
        }

        #region Property Tests

        [TestMethod]
        public void ApiKeyShouldReturnValueFromConfiguration()
        {
            //Arrange
            var config = new NameValueCollection {{"GearHostApiKey", "TestKey"}};
            _configurationProviderMock.Setup(cp => cp.AppSettings).Returns(config);

            //Act
            var result = _service.ApiKey;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestKey", result);
        }

        [TestMethod]
        public void ApiBaseUrlShouldReturnValueFromConfiguration()
        {
            //Arrange
            var config = new NameValueCollection {{"GearHostApiBaseUrl", "TestKey"}};
            _configurationProviderMock.Setup(cp => cp.AppSettings).Returns(config);

            //Act
            var result = _service.BaseApiUrl;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestKey", result);
        }

        #endregion

        #region GetDatabases Tests

        [TestMethod]
        public void GetDatabasesShouldCreateAndPassProperUrlToClientService()
        {
            //Arrange
            ConfigureTestConfig();
            var expectedUrl = TestUrl + "databases";

            //Act
            var result = _service.GetDatabases();

            //Assert
            _httpClientServiceMock.Verify(
                s =>
                    s.MakeHttpGetRequest<GearHostGetDatabasesResponse>(expectedUrl, It.IsAny<string>(),
                        It.IsAny<List<KeyValuePair<string, string>>>()));
        }

        [TestMethod]
        public void GetDatabasesShouldPassEmptyAcceptStringToHttpGetMethod()
        {
            //Arrange
            ConfigureTestConfig();

            //Act
            var result = _service.GetDatabases();

            //Assert
            _httpClientServiceMock.Verify(
                s =>
                    s.MakeHttpGetRequest<GearHostGetDatabasesResponse>(It.IsAny<string>(), string.Empty,
                        It.IsAny<List<KeyValuePair<string, string>>>()));
        }

        [TestMethod]
        public void GetDatabasesShouldCreateAndPassAuthorizeHeaderToHttpGetMethod()
        {
            //Arrange
            ConfigureTestConfig();
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", $"bearer {TestKey}")
            };

            //Act
            var result = _service.GetDatabases();

            //Assert
            _httpClientServiceMock.Verify(
                s =>
                    s.MakeHttpGetRequest<GearHostGetDatabasesResponse>(It.IsAny<string>(), It.IsAny<string>(), headers));
        }

        #endregion
    }
}