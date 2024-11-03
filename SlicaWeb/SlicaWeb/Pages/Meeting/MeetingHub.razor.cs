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

namespace SlicaWeb.Pages.Meeting
{
    public partial class MeetingHub
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<MeetingModel> meetingDetails = new List<MeetingModel>();
        private HubConnection hubConnection;
        private string _searchString;
        private string _meetingCode;

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

                    await GetMeetingHubData();
                }
            }
        }

        #endregion
        private Func<MeetingModel, int, string> _rowStyleFunc => (x, i) =>
        {
            if (x.IsPin)
                return "background-color:#CFD8DC";

            return "";
        };

        #region Meeting Data Related Functions
        private async Task GetMeetingHubData()
        {
            
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/GetMeetingHubData");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    meetingDetails = JsonConvert.DeserializeObject<List<MeetingModel>>(data[0].ToString())!;
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

        private Func<MeetingModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Host.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            if (x.Status.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.StartDateTime!.Value.ToString("dd-MMM-yyyy HH:mm").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion
        async Task MarkAsPin( int meetingID)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    MeetingID = meetingID
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/UpdateMeetingPinStatus");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {


                    Snackbar.Add(returnMessage, Severity.Success);
                    await GetMeetingHubData();
                    
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

        #region Dialog Related Functions
        private void OpenCreateMeetingDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters();
            parameters.Add("OnCreate", EventCallback.Factory.Create<int>(this, GetMeetingHubData));

            DialogService.Show<MeetingDialog>("Create Meeting",parameters, options);
        }
        #endregion

        #region CRUD Functions
        private async Task EditMeeting(MeetingModel currentMeeting)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters();
            parameters.Add("meetingParameter", currentMeeting);
            parameters.Add("OnUpdate", EventCallback.Factory.Create<int>(this,GetMeetingHubData));
            DialogService.Show<UpdateMeetingDialog>("Edit Meeting",parameters, options);
        }

        private void OnDetailsClick(int id)
        {
            NavigationManager.NavigateTo("meeting-details/"+id.ToString());
        }
        #endregion
        #endregion
    }
}
