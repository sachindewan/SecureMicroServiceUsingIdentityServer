using System;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movie.Client.HttpHandler
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory ClientFactory;
        private readonly ClientCredentialsTokenRequest ClientCredentialsTokenRequest;
        private readonly IHttpContextAccessor ContextAccessor;
        public AuthenticationDelegatingHandler(IHttpClientFactory clientFactory, ClientCredentialsTokenRequest clientCredentialsTokenRequest, IHttpContextAccessor contextAccessor)
        {
            ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            ClientCredentialsTokenRequest = clientCredentialsTokenRequest ?? throw new ArgumentNullException(nameof(clientCredentialsTokenRequest));
            ContextAccessor = contextAccessor;
            ;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
                var client = ClientFactory.CreateClient("IDPClient");

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(ClientCredentialsTokenRequest,
                    cancellationToken: cancellationToken);
                if (tokenResponse.IsError)
                {
                    throw new HttpRequestException("Some thing went wrong while requesting the access token");
                }

                request.SetBearerToken(tokenResponse.AccessToken);
                // below code added for testing purpose for access and id token
                if (ContextAccessor.HttpContext != null)
                {
                    var tok = await ContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                    var idToken = await ContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
                }
                return await base.SendAsync(request, cancellationToken);
        }
    }
}
