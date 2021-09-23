using Compori.MagentoApi.Authentication;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

namespace MagentoApi.AuthenticationUnitTests
{
    [UnitTest]
    public class TokenTest
    {
        [Fact]
        public async void TestConfigure()
        {
            Mock<ITokenRequest> mockTokenRequest;
            Mock<ISystemDateTime> mockSystemDateTime;
            TokenSettings settings;
            Token sut;
            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .Setup(service => service.UtcNow)
                .Returns(default(DateTime));

            //
            // Throw null argument exception
            //
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(null));
            Assert.Equal("settings", argumentNullException.ParamName);

            //
            // Token type not supported.
            //
            settings = new TokenSettings();
            var argumentException = Assert.Throws<ArgumentException>(() => new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings));
            Assert.Contains("not supported", argumentException.Message);
            Assert.Contains("settings", argumentException.ParamName);

            //
            // Token type is integration but no access token was set.
            //
            settings = new TokenSettings() { TokenType = TokenType.Integration };
            argumentException = Assert.Throws<ArgumentException>(() => new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings));
            Assert.Contains("No access token provided for integration token type.", argumentException.Message);
            Assert.Contains("settings", argumentException.ParamName);

            //
            // Test integration token
            //
            settings = new TokenSettings() { TokenType = TokenType.Integration, AccessToken = Guid.NewGuid().ToString() };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Equal(settings.AccessToken, await sut.CreateAsync());
            Assert.Equal(settings.TokenType, sut.TokenType);
            Assert.Null(sut.BaseEndpointAddress);
            Assert.Null(sut.ExpirationPeriod);
            Assert.Null(sut.Expires);
            Assert.Null(sut.HttpAuthenticationPassword);
            Assert.Null(sut.HttpAuthenticationUser);
            Assert.True(sut.IsValid);
            Assert.Null(sut.RequestPassword);
            Assert.Null(sut.RequestUserName);
            Assert.Null(sut.UserAgent);

            //
            // Test admin and customer token
            //
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                HttpAuthenticationPassword = "httpauthpass",
                HttpAuthenticationUser = "httpauthuser",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                UserAgent = "useragent"
            };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Equal(settings.BaseEndpointAddress, sut.BaseEndpointAddress);
            Assert.Equal(settings.HttpAuthenticationPassword, sut.HttpAuthenticationPassword);
            Assert.Equal(settings.HttpAuthenticationUser, sut.HttpAuthenticationUser);
            Assert.Equal(settings.RequestPassword, sut.RequestPassword);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(Token.DefaultAdminTokenExpirationPeriod, sut.ExpirationPeriod);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.TokenType ,sut.TokenType);

            settings = new TokenSettings
            {
                TokenType = TokenType.Customer,
                BaseEndpointAddress = "endpoint",
                HttpAuthenticationPassword = "httpauthpass",
                HttpAuthenticationUser = "httpauthuser",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                UserAgent = "useragent"
            };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Equal(settings.BaseEndpointAddress, sut.BaseEndpointAddress);
            Assert.Equal(settings.HttpAuthenticationPassword, sut.HttpAuthenticationPassword);
            Assert.Equal(settings.HttpAuthenticationUser, sut.HttpAuthenticationUser);
            Assert.Equal(settings.RequestPassword, sut.RequestPassword);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(Token.DefaultCustomerTokenExpirationPeriod, sut.ExpirationPeriod);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.TokenType ,sut.TokenType);

            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                HttpAuthenticationPassword = "httpauthpass",
                HttpAuthenticationUser = "httpauthuser",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                UserAgent = "useragent",
                ExpirationPeriod = new TimeSpan(1, 2, 3)
            };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Equal(settings.BaseEndpointAddress, sut.BaseEndpointAddress);
            Assert.Equal(settings.HttpAuthenticationPassword, sut.HttpAuthenticationPassword);
            Assert.Equal(settings.HttpAuthenticationUser, sut.HttpAuthenticationUser);
            Assert.Equal(settings.RequestPassword, sut.RequestPassword);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.ExpirationPeriod, sut.ExpirationPeriod);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.TokenType ,sut.TokenType);

            settings = new TokenSettings
            {
                TokenType = TokenType.Customer,
                BaseEndpointAddress = "endpoint",
                HttpAuthenticationPassword = "httpauthpass",
                HttpAuthenticationUser = "httpauthuser",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                UserAgent = "useragent",
                ExpirationPeriod = new TimeSpan(2, 3, 4)
            };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Equal(settings.BaseEndpointAddress, sut.BaseEndpointAddress);
            Assert.Equal(settings.HttpAuthenticationPassword, sut.HttpAuthenticationPassword);
            Assert.Equal(settings.HttpAuthenticationUser, sut.HttpAuthenticationUser);
            Assert.Equal(settings.RequestPassword, sut.RequestPassword);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.ExpirationPeriod, sut.ExpirationPeriod);
            Assert.Equal(settings.RequestUserName, sut.RequestUserName);
            Assert.Equal(settings.TokenType ,sut.TokenType);

            //
            // Test not reconfigurable
            //
            settings = new TokenSettings() { TokenType = TokenType.Integration, AccessToken = Guid.NewGuid().ToString() };
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                HttpAuthenticationPassword = "httpauthpass",
                HttpAuthenticationUser = "httpauthuser",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                UserAgent = "useragent"
            };
            var invalidOperationException = Assert.Throws<InvalidOperationException>(() => sut.Configure(settings));
            Assert.Contains("Token is already configured.", invalidOperationException.Message);            
        }

        [Fact]
        public async void TestExpires()
        {
            Mock<ITokenRequest> mockTokenRequest;
            Mock<ISystemDateTime> mockSystemDateTime;
            TokenSettings settings;
            IToken sut;
            DateTime current;
            DateTime expected;

            //
            // Without setting and calling anything
            //
            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .Setup(service => service.UtcNow)
                .Returns(default(DateTime));
            settings = new TokenSettings();
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object);
            var invalidOperationException = Assert.Throws<InvalidOperationException>(() => sut.Expires);
            Assert.Contains("The token is not configured.", invalidOperationException.Message);


            //
            // Call the CreateTokenAsync and expecting expire in current + period.
            //
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                RequestUserName = "admin",
                RequestPassword = "1234567",
                ExpirationPeriod = new TimeSpan(1, 2, 3)
            };
            current = new DateTime(2019, 2, 1, 20, 11, 00);
            expected = current.Add(settings.ExpirationPeriod.Value);

            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .Setup(service => service.UtcNow)
                .Returns(current);
            mockTokenRequest
                .Setup(service => service.RequestTokenAsync(
                    It.Is<TokenType>( v => v == settings.TokenType), 
                    It.Is<string>( v => settings.BaseEndpointAddress.Equals(v)), 
                    It.Is<string>( v => settings.UserAgent != null ? settings.UserAgent.Equals(v) : v == null), 
                    It.Is<string>( v => settings.RequestUserName.Equals(v)), 
                    It.Is<string>( v => settings.RequestPassword.Equals(v)), 
                    It.Is<string>( v => settings.HttpAuthenticationUser != null ? settings.HttpAuthenticationUser.Equals(v) : v == null), 
                    It.Is<string>( v => settings.HttpAuthenticationPassword != null ? settings.HttpAuthenticationPassword.Equals(v) : v == null)))
                .ReturnsAsync(It.IsAny<string>());

            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.Null(sut.Expires);
            await sut.CreateAsync();
            Assert.Equal(expected, sut.Expires);
        }

        [Fact]
        public async void TestIsValid()
        {
            Mock<ITokenRequest> mockTokenRequest;
            Mock<ISystemDateTime> mockSystemDateTime;
            TokenSettings settings;
            IToken sut;
            Token sutConcrete;
            DateTime current;
            DateTime expected;

            //
            // Without setting and calling anything, token is not configured.
            //
            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object);
            var invalidOperationException = Assert.Throws<InvalidOperationException>(() => sut.IsValid);
            Assert.Contains("The token is not configured.", invalidOperationException.Message);


            //
            // Call the CreateTokenAsync and expecting valid
            //
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                ExpirationPeriod = new TimeSpan(1, 2, 3),
                BaseEndpointAddress = "endpoint",
                RequestUserName = "admin",
                RequestPassword = "1234567",
            };
            current = new DateTime(2019, 2, 1, 20, 11, 00);
            expected = current.Add(settings.ExpirationPeriod.Value);

            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .Setup(service => service.UtcNow)
                .Returns(current);

            mockTokenRequest
                .Setup(service => service.RequestTokenAsync(
                    It.Is<TokenType>( v => v == settings.TokenType), 
                    It.Is<string>( v => settings.BaseEndpointAddress.Equals(v)), 
                    It.Is<string>( v => settings.UserAgent != null ? settings.UserAgent.Equals(v) : v == null), 
                    It.Is<string>( v => settings.RequestUserName.Equals(v)), 
                    It.Is<string>( v => settings.RequestPassword.Equals(v)), 
                    It.Is<string>( v => settings.HttpAuthenticationUser != null ? settings.HttpAuthenticationUser.Equals(v) : v == null), 
                    It.Is<string>( v => settings.HttpAuthenticationPassword != null ? settings.HttpAuthenticationPassword.Equals(v) : v == null)
                    ))
                .ReturnsAsync(It.IsAny<string>());

            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            Assert.False(sut.IsValid);
            await sut.CreateAsync();
            Assert.True(sut.IsValid);

            //
            // Call the CreateTokenAsync and expecting valid, expire than will do it invalid
            //
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                RequestUserName = "admin",
                RequestPassword = "1234567",
                ExpirationPeriod = new TimeSpan(1, 2, 3)
            };
            current = new DateTime(2019, 2, 1, 20, 11, 00);

            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .SetupSequence(service => service.UtcNow)
                .Returns(current)
                .Returns(current)
                .Returns(current.Add(settings.ExpirationPeriod.Value));

            mockTokenRequest
                .Setup(service => service.RequestTokenAsync(
                    It.Is<TokenType>( v => v == settings.TokenType), 
                    It.Is<string>( v => settings.BaseEndpointAddress.Equals(v)), 
                    It.Is<string>( v => settings.UserAgent != null ? settings.UserAgent.Equals(v) : v == null), 
                    It.Is<string>( v => settings.RequestUserName.Equals(v)), 
                    It.Is<string>( v => settings.RequestPassword.Equals(v)), 
                    It.Is<string>( v => settings.HttpAuthenticationUser != null ? settings.HttpAuthenticationUser.Equals(v) : v == null), 
                    It.Is<string>( v => settings.HttpAuthenticationPassword != null ? settings.HttpAuthenticationPassword.Equals(v) : v == null)
                    ))
                .ReturnsAsync(It.IsAny<string>());

            sutConcrete = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            sut = sutConcrete;
            await sut.CreateAsync();    // -> first call inside of CreateAsync
            Assert.True(sut.IsValid);   // -> second call of UtcNow
            Assert.False(sut.IsValid);  // -> third call of UtcNow will do invalid
        }

        [Fact]
        public async void TestCreateAsync()
        {
            Mock<ITokenRequest> mockTokenRequest;
            Mock<ISystemDateTime> mockSystemDateTime;
            TokenSettings settings;
            IToken sut;
            DateTime current;

            settings = new TokenSettings
            {
                TokenType = TokenType.Customer,
                BaseEndpointAddress = "endpoint",
                UserAgent = "useragent",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                ExpirationPeriod = new TimeSpan(1, 2, 3)
            };
            current = new DateTime(2019, 2, 1, 20, 11, 00);
            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .Setup(service => service.UtcNow)
                .Returns(current);
            mockTokenRequest
                .Setup(service => service.RequestTokenAsync(                    
                    It.Is<TokenType>( v => v == settings.TokenType), 
                    It.Is<string>( v => settings.BaseEndpointAddress.Equals(v)), 
                    It.Is<string>( v => settings.UserAgent != null ? settings.UserAgent.Equals(v) : v == null), 
                    It.Is<string>( v => settings.RequestUserName.Equals(v)), 
                    It.Is<string>( v => settings.RequestPassword.Equals(v)), 
                    It.Is<string>( v => settings.HttpAuthenticationUser != null ? settings.HttpAuthenticationUser.Equals(v) : v == null), 
                    It.Is<string>( v => settings.HttpAuthenticationPassword != null ? settings.HttpAuthenticationPassword.Equals(v) : v == null)
                    ))
                .ReturnsAsync(It.IsAny<string>());

            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            await sut.CreateAsync();
            mockTokenRequest.Verify(service => service.RequestTokenAsync(
                settings.TokenType,
                settings.BaseEndpointAddress,
                settings.UserAgent,
                settings.RequestUserName,
                settings.RequestPassword,
                settings.HttpAuthenticationUser,
                settings.HttpAuthenticationPassword
                ), Times.Once());

            //
            //
            //
            settings = new TokenSettings
            {
                TokenType = TokenType.Admin,
                BaseEndpointAddress = "endpoint",
                UserAgent = "useragent",
                HttpAuthenticationPassword = "httpauthpass",
                RequestUserName = "requestuser",
                RequestPassword = "requestpass",
                HttpAuthenticationUser = "httpauthuser",
                ExpirationPeriod = new TimeSpan(2, 1, 22)
            };
            current = new DateTime(2019, 2, 1, 20, 11, 00);
            mockTokenRequest = new Mock<ITokenRequest>();
            mockSystemDateTime = new Mock<ISystemDateTime>();
            mockSystemDateTime
                .SetupSequence(service => service.UtcNow)
                .Returns(current)
                .Returns(current)
                .Returns(current)
                .Returns(current.Add(settings.ExpirationPeriod.Value));
            mockTokenRequest
                .Setup(service => service.RequestTokenAsync(
                    It.Is<TokenType>( v => v == settings.TokenType), 
                    It.Is<string>( v => settings.BaseEndpointAddress.Equals(v)), 
                    It.Is<string>( v => settings.UserAgent != null ? settings.UserAgent.Equals(v) : v == null), 
                    It.Is<string>( v => settings.RequestUserName.Equals(v)), 
                    It.Is<string>( v => settings.RequestPassword.Equals(v)), 
                    It.Is<string>( v => settings.HttpAuthenticationUser != null ? settings.HttpAuthenticationUser.Equals(v) : v == null), 
                    It.Is<string>( v => settings.HttpAuthenticationPassword != null ? settings.HttpAuthenticationPassword.Equals(v) : v == null)
                ))
                .ReturnsAsync(It.IsAny<string>());

            // 1. call on ISystemDateTime.UtcNow
            sut = new Token(mockTokenRequest.Object, mockSystemDateTime.Object).Configure(settings);
            await sut.CreateAsync();

            mockTokenRequest.Verify(service => service.RequestTokenAsync(
                settings.TokenType,
                settings.BaseEndpointAddress,
                settings.UserAgent,
                settings.RequestUserName,
                settings.RequestPassword,
                settings.HttpAuthenticationUser,
                settings.HttpAuthenticationPassword
                ), Times.Once());

            // 2. call on ISystemDateTime.UtcNow
            Assert.True(sut.IsValid);

            //
            // run again...
            //
            // 3. call on ISystemDateTime.UtcNow
            mockTokenRequest.Invocations.Clear();
            await sut.CreateAsync();
            mockTokenRequest.Verify(service => service.RequestTokenAsync(
                settings.TokenType,
                settings.BaseEndpointAddress,
                settings.UserAgent,
                settings.RequestUserName,
                settings.RequestPassword,
                settings.HttpAuthenticationUser,
                settings.HttpAuthenticationPassword
                ), Times.Never());

            //
            // run again... time ellapsed.
            //
            // 4. Utc Now
            mockTokenRequest.Invocations.Clear();
            await sut.CreateAsync();
            mockTokenRequest.Verify(service => service.RequestTokenAsync(
                settings.TokenType,
                settings.BaseEndpointAddress,
                settings.UserAgent,
                settings.RequestUserName,
                settings.RequestPassword,
                settings.HttpAuthenticationUser,
                settings.HttpAuthenticationPassword
                ), Times.Once());
        }
    }
}
