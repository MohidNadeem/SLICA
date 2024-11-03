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

namespace SlicaWeb.Pages.Meeting
{
    public partial class MeetingLog
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<MeetingModel> meetingDetails = new List<MeetingModel>();
        private HubConnection hubConnection;
        private string _searchString;
        private int _filterType = 1;

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

                    await GetMeetingLogData();
                }
            }
        }

        #endregion


        #region Meeting Data Related Functions
        private async Task GetMeetingLogData()
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    Type = _filterType
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/GetMeetingLogData");
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

            if (x.Host.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (x.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (x.Participant.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Status.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Date!.Value.ToString("dd-MMM-yyyy").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.StartTime != null && x.StartTime!.Value.ToString("hh:mm tt").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.EndTime != null && x.EndTime!.Value.ToString("hh:mm tt").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            

            return false;
        };

        private async Task OnValueChanged()
        {
            await GetMeetingLogData();
        }
        #endregion



        #region CRUD Functions

        private void OnDetailsClick(int id)
        {
            NavigationManager.NavigateTo("meeting-details/" + id.ToString());
        }
        #endregion
        #endregion
    }
}
