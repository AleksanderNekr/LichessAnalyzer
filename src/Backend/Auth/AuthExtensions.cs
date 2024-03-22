namespace Backend.Auth;

public static class AuthExtensions
{
    internal const string LichessAuthPolicyName = "lichess-user";
    internal const string AuthenticationScheme  = "lichess-oauth";
    internal const string CookieScheme          = "lichess-cookie";

    public static void AddLichessAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
                .AddCookie(CookieScheme)
                .AddOAuth<LichessOAuthOptions, LichessOAuthHandler>(AuthenticationScheme, _ => {});

        services.AddAuthorizationBuilder()
                .AddPolicy(LichessAuthPolicyName,
                           policyBuilder =>
                           {
                               policyBuilder.AddAuthenticationSchemes(AuthenticationScheme)
                                            .RequireAuthenticatedUser();
                           });
    }
}
