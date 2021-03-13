using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using PeopleApplication.Service;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PeopleApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// from: https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-asp.net-mvc
        /// </summary>
        public async Task<ActionResult> GoogleContacts(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                var peopleService = new PeopleServiceService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = "Yashar App"
                });

                PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                    peopleService.People.Connections.List("people/me");
                peopleRequest.PersonFields = "addresses,ageRanges,biographies,birthdays,calendarUrls," +
                    "clientData,coverPhotos,emailAddresses,events,externalIds,genders,imClients," +
                    "interests,locales,locations,memberships,metadata,miscKeywords,names,nicknames," +
                    "occupations,organizations,phoneNumbers,photos,relations,sipAddresses,skills,urls,userDefined";
                ListConnectionsResponse connectionsResponse = peopleRequest.Execute();
                IList<Person> connections = connectionsResponse.Connections;

                return View();
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }
    }
}
