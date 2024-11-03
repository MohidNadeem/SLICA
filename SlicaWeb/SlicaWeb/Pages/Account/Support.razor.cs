using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;
using SlicaWeb.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;

namespace SlicaWeb.Pages.Account
{
    public partial class Support
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<IssueModel> issueDetails = new List<IssueModel>();
        private HubConnection hubConnection;
        static IssueModel issue = new IssueModel();
        EditContext issueContext = new EditContext(issue);

        private string _searchString;

        #endregion


        #region Class Functions

        #region Overridden Functions
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                sessionModel = await CookieManager.GetCookie<UserModel>("LoggedInUserInfo");
                if (sessionModel == null)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {

                    await GetSupportData();
                }
            }
        }

        #endregion
        
        #region Support Data Related Functions
        private async Task GetSupportData()
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/GetSupportData");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    issueDetails = JsonConvert.DeserializeObject<List<IssueModel>>(data[0].ToString())!;
                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
                Snackbar.Add(e.Message, Severity.Error);
            }

        }

        private Func<IssueModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Subject.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            if (x.Status.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Category.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.ReportedDate!.Value.ToString("dd-MMM-yyyy HH:mm").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            if (x.ResolvedDate!.Value.ToString("dd-MMM-yyyy HH:mm").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion
        async Task ReportIssue()
        {
            try
            {
                issueContext.Validate();

                List<string> validateParams = new List<string>
                {
                  "Email","Subject","Description","Name","CategoryID","PhoneNo"
                };
                if (!CommonMethods.ValidateAttributes(issue, validateParams))
                {
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    Subject = issue.Subject,
                    Description = issue.Description,
                    CategoryID =  issue.CategoryID,
                    Email = issue.Email,
                    PhoneNo=issue.PhoneNo,
                    Name=issue.Name
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/InsertIssue");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {

                    issue = new IssueModel();
                    Snackbar.Add(returnMessage, Severity.Success);
                    await GetSupportData();

                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }

            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
        }

        #endregion
    }
}
