using Migration_Tool_GraphAPI.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
///Calender
using Migration_Tool_GraphAPI.TokenStorage;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Migration_Tool_GraphAPI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Migration_Tool_GraphAPI.Helpers;
using Owin;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System;
///
namespace Migration_Tool_GraphAPI.Helpers
{
    public static class GraphHelper
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static List<string> graphScopes =
            new List<string>(ConfigurationManager.AppSettings["ida:AppScopes"].Split(' '));

        public static async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var graphClient = GetAuthenticatedClient();

            var events = await graphClient.Me.Events.Request()
                .Select("subject,organizer,start,end")
                .OrderBy("createdDateTime DESC")
                .GetAsync();

            return events.CurrentPage;
        }

        private static GraphServiceClient GetAuthenticatedClient()
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();

                        var tokenStore = new SessionTokenStore(idClient.UserTokenCache,
                                HttpContext.Current, ClaimsPrincipal.Current);

                        var userUniqueId = tokenStore.GetUsersUniqueId(ClaimsPrincipal.Current);
                        var account = await idClient.GetAccountAsync(userUniqueId);

                        // By calling this here, the token can be refreshed
                        // if it's expired right before the Graph call is made
                        var result = await idClient.AcquireTokenSilent(graphScopes, account)
                            .ExecuteAsync();

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
        }

        public static async Task<CachedUser> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            var user = await graphClient.Me.Request()
                .Select(u => new {
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName,

                })
                .GetAsync();

            return new CachedUser
            {
                Avatar = string.Empty,
                DisplayName = user.DisplayName,
                Email = string.IsNullOrEmpty(user.Mail) ?
                    user.UserPrincipalName : user.Mail
            };
        }
        
        //public static async Task<string> GetUserPhotoAsync(string accessToken)
        //{
        //    var graphClient = new GraphServiceClient(
        //        new DelegateAuthenticationProvider(
        //            requestMessage =>
        //            {
        //                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //                return Task.CompletedTask;
        //            }));

        //    try
        //    {
        //        var photoStream = await graphClient.Me.Photo.Content.Request().GetAsync();
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await photoStream.CopyToAsync(memoryStream);
        //            var photoBytes = memoryStream.ToArray();
        //            return $"data:image/jpeg;base64,{Convert.ToBase64String(photoBytes)}";
        //        }
        //    }
        //    catch
        //    {
        //        // Return a default icon if there's no profile photo
        //        return null;
        //    }
        //}

    }
}
