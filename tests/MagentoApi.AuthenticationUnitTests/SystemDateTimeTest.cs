using Compori.MagentoApi.Authentication;
using System;
using Xunit;
using Xunit.Categories;

namespace MagentoApi.AuthenticationUnitTests
{
    [UnitTest]
    public class SystemDateTimeTest
    {
        [Fact]
        public void TestUtcNow()
        {
            var sut = new SystemDateTime() as ISystemDateTime;
            var now = DateTime.UtcNow;
            Assert.True(sut.UtcNow >= now);
            Assert.Equal(DateTimeKind.Utc, sut.UtcNow.Kind);
        }
    }
}
