using Backend.DataManagement.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace BackendTests.IntegrationTests;

public abstract class TestAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                                                       .WithImage("postgres:latest")
                                                       .WithDatabase("postgres")
                                                       .WithUsername("postgres")
                                                       .WithPassword("postgres")
                                                       .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(
            services =>
            {
                ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                    serviceDescriptor => serviceDescriptor.ServiceType == typeof(DbContextOptions<UsersContext>));
                if (dbContextDescriptor is not null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<UsersContext>(options =>
                                                    {
                                                        options.UseNpgsql(_dbContainer.GetConnectionString());
                                                    });
            });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
