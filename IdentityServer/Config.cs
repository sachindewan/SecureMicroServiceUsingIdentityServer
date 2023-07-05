using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Test;
using static System.Net.WebRequestMethods;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new("roles","users roles", new List<string>(){"role"}) 
        };

        public static List<TestUser> GetTestUsers() => TestUsers.Users;
        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("MoviesAPI")
            {
                Scopes = { "MoviesAPI" }
            }
        };

        public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
        {
            new ApiScope("MoviesAPI")
        };

        public static IEnumerable<Client> GetClients() => new List<Client>()
        {
            new Client()
            {
                ClientId = "MoviesAPI",
                ClientSecrets = new List<Secret>() {new Secret("MoviesAPI_secrets".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes = { "MoviesAPI", "Api" },

            },
             new Client()
            {
                ClientId = "movies_mvc_client",
                ClientName = "Movies Mvc web app",
                ClientSecrets = new List<Secret>() {new Secret("secrets".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes = { IdentityServerConstants.StandardScopes.Profile ,IdentityServerConstants.StandardScopes.OpenId,"roles"},
                RedirectUris = new List<string>()
                {
                    "https://localhost:5002/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "https://localhost:5002/signout-callback-oidc"
                }

            }
        };
    }
}
