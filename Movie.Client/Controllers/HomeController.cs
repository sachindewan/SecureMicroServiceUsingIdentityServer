using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movie.Client.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movie.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = clientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserInfo()
        {
            var client  = _httpClientFactory.CreateClient("IDPClient");
            var info = await client.GetDiscoveryDocumentAsync();
            if (info.IsError)
            {
                throw new Exception("getting error while getting access token");
            }

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse =  await client.GetUserInfoAsync(new UserInfoRequest() { Address = info.UserInfoEndpoint, Token = accessToken });
            IDictionary<string,string> claimsDictionary = new Dictionary<string,string>();
            foreach (var claims in userInfoResponse.Claims)
            {
                claimsDictionary.Add(claims.Type, claims.Value);
            }

            return View(new UserInfoViewModel(claimsDictionary));
        }
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class UserInfoViewModel
    {
        public IDictionary<string, string> _dictionary = null;

        public UserInfoViewModel(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }
    }
}
