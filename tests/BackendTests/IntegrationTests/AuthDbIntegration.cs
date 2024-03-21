using System.Security.Claims;
using Backend.DataManagement.Users.Entities;

namespace BackendTests.IntegrationTests
{
    public class AuthDbIntegration(TestAppFactory factory) : IntegrationTestBase(factory)
    {
        [Fact]
        public async Task Test_RegisterUser_Should_SetUser()
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(
                                                 new[]
                                                 {
                                                     new Claim(ClaimTypes.NameIdentifier, "testUser"),
                                                 }));

            await AuthService.RegisterUserAsync(claims);

            User? user = await AuthService.GetCurrentUserAsync(claims, CancellationToken.None);

            Assert.NotNull(user);
            Assert.Equal("testUser", user.Name);
        }
    }
}
