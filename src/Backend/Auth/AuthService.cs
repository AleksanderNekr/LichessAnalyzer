using System.Security.Claims;
using Backend.DataManagement.Users.Entities;
using Backend.DataManagement.Users.Services;

namespace Backend.Auth;

public class AuthService(
    UsersManagementService usersManagementService,
    ILogger<AuthService>   logger)
{
    public async Task RegisterUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        string username = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

        User? registered = await usersManagementService.TryCreateUserAsync(
                               Guid.NewGuid(),
                               username,
                               cancellationToken);
        if (registered is not null)
        {
            logger.LogDebug("Successfully registered {@User}", registered);
        }
    }

    public async ValueTask<DeleteUserResult> DeleteUserAccountAsync(ClaimsPrincipal claims, CancellationToken cancellationToken = default)
    {
        string? username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        if (username is null)
        {
            logger.LogError("No name id for deleting user in claims\nUser: {@User}", claims);

            return DeleteUserResult.NotAuthenticated;
        }

        User? user = await usersManagementService.FindByNameAsync(username, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {Name} is not in DB", username);

            return DeleteUserResult.UserNotInDb;
        }

        bool deleted = await usersManagementService.TryDeleteUserAsync(user.Id, cancellationToken);
        if (deleted)
        {
            logger.LogDebug("User {Id}:{Name} deleted", user.Id, user.Name);

            return DeleteUserResult.SuccessDelete;
        }

        logger.LogError("Unsuccessful attempt to delete user {Id}:{Name}", user.Id, user.Name);

        return DeleteUserResult.Fail;
    }
}

public enum DeleteUserResult
{
    NotAuthenticated,
    UserNotInDb,
    SuccessDelete,
    Fail
}
