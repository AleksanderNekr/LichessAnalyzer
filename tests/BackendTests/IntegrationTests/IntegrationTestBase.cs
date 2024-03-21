using Backend.Auth;
using Backend.DataManagement.Users;
using Backend.DataManagement.Users.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BackendTests.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<TestAppFactory>
{
    protected readonly AuthService     AuthService;
    protected readonly UsersRepository UsersRepo;

    protected IntegrationTestBase(TestAppFactory factory)
    {
        IServiceScope scope     = factory.Services.CreateScope();
        var           dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

        UsersRepo   = new UsersRepository(dbContext, MoqLogger<UsersRepository>());
        AuthService = new AuthService(UsersRepo, MoqLogger<AuthService>());
    }
    
    private static ILogger<T> MoqLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }
}
