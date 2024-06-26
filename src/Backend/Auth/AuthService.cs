﻿using System.Security.Claims;
using Backend.DataManagement.Users.Entities;
using Backend.DataManagement.Users.Services;

namespace Backend.Auth;

public class AuthService(
    UsersRepository usersRepository,
    ILogger<AuthService>   logger)
{
    public async Task RegisterUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        string username = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

        User? registered = await usersRepository.TryCreateUserAsync(
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

        User? user = await usersRepository.FindByNameAsync(username, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {Name} is not in DB", username);

            return DeleteUserResult.UserNotInDb;
        }

        bool deleted = await usersRepository.TryDeleteUserAsync(user.Id, cancellationToken);
        if (deleted)
        {
            logger.LogDebug("User {Id}:{Name} deleted", user.Id, user.Name);

            return DeleteUserResult.SuccessDelete;
        }

        logger.LogError("Unsuccessful attempt to delete user {Id}:{Name}", user.Id, user.Name);

        return DeleteUserResult.Fail;
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal claims, CancellationToken cancellationToken)
    {
        string? username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        if (username is null)
        {
            return null;
        }
        
        return await usersRepository.FindByNameAsync(username, cancellationToken: cancellationToken);
    }
}

public enum DeleteUserResult
{
    NotAuthenticated,
    UserNotInDb,
    SuccessDelete,
    Fail
}
