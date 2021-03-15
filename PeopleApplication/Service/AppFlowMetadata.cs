using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.PeopleService.v1;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace PeopleApplication.Service
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "< YOUR CLIENT ID >",
                    ClientSecret = "< YOUR CLIENT SECRETE >"
                },
                Scopes = new[] { PeopleServiceService.ScopeConstants.ContactsReadonly, 
                    PeopleServiceService.ScopeConstants.ContactsOtherReadonly }
            });

        public override string GetUserId(Controller controller)
        {

            return controller.User.Identity.GetUserId();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}