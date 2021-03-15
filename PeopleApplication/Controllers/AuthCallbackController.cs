using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using PeopleApplication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PeopleApplication.Controllers
{
    /// <summary>
    /// from: https://github.com/googleapis/google-api-dotnet-client/blob/master/Src/Support/Google.Apis.Auth.Mvc/OAuth2/Mvc/Controllers/AuthCallbackController.cs
    /// </summary>

    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override FlowMetadata FlowData
        {
            get { return new AppFlowMetadata(); }
        }

        [AsyncTimeout(30000)]
        public override async Task<ActionResult> IndexAsync(AuthorizationCodeResponseUrl authorizationCode, CancellationToken taskCancellationToken)
        {
            if (string.IsNullOrEmpty(authorizationCode.Code))
            {
                var errorResponse = new TokenErrorResponse(authorizationCode);
                Logger.Info("Received an error. The response is: {0}", errorResponse);

                return OnTokenError(errorResponse);
            }

            Logger.Debug("Received \"{0}\" code", authorizationCode.Code);

            var returnUrl = Request.Url.ToString();
            returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?"));

            //Asynchronously exchanges code with a token.
            var token = await Flow.ExchangeCodeForTokenAsync(UserId, authorizationCode.Code, returnUrl,
                taskCancellationToken).ConfigureAwait(false);

            //Constructs a new credential instance with access token
            var credential = new UserCredential(Flow, UserId, token);

            try
            {
                var peopleService = new PeopleServiceService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "< YOUR APP NAME >",

                });

                #region Contacts

                PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                    peopleService.People.Connections.List("people/me");

                peopleRequest.PersonFields = "addresses,ageRanges,biographies,birthdays,calendarUrls," +
                    "clientData,coverPhotos,emailAddresses,events,externalIds,genders,imClients," +
                    "interests,locales,locations,memberships,metadata,miscKeywords,names,nicknames," +
                    "occupations,organizations,phoneNumbers,photos,relations,sipAddresses,skills,urls,userDefined";

                peopleRequest.SortOrder = PeopleResource.ConnectionsResource.ListRequest.SortOrderEnum.LASTMODIFIEDDESCENDING;
                peopleRequest.PageSize = 1000;

                ListConnectionsResponse connectionsResponse = peopleRequest.Execute();

                List<Person> connections = connectionsResponse.Connections as List<Person>;

                // get all pages
                while (!string.IsNullOrEmpty(connectionsResponse.NextPageToken))
                {
                    peopleRequest.PageToken = connectionsResponse.NextPageToken;
                    connectionsResponse = peopleRequest.Execute();
                    connections.AddRange(connectionsResponse.Connections);
                }

                #endregion

                var model = connections.Where(x => x.EmailAddresses != null && x.EmailAddresses.Any());

                return View(model);
            }
            catch (Exception exp)
            {
                Logger.Info("Received an error. The response is: {0}", exp.Message);

                return View("Error");
            }
        }

        protected override ActionResult OnTokenError(TokenErrorResponse errorResponse)
        {
            return View("Error");
        }
    }
}