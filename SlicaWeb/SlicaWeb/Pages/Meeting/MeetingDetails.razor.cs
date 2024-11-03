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
    public partial class MeetingDetails
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<InviteeModel> inviteeDetails = new List<InviteeModel>();
        [Parameter]
        public int MeetingID { get; set; }
        private HubConnection hubConnection;
        private string _searchString;
        public List<UserModel> connects { get; set; } = new List<UserModel>();
        private MeetingModel meeting { get; set; } = new MeetingModel();

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

                    await GetMeetingDetails(MeetingID);
                }
            }
        }

        #endregion


        #region Meeting Details Related Functions
        private async Task GetMeetingDetails(int meetingID)
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    MeetingID = meetingID,
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/GetMeetingDetailsData");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    meeting = JsonConvert.DeserializeObject<MeetingModel>(data[0][0]!.ToString())!;
                    inviteeDetails = JsonConvert.DeserializeObject<List<InviteeModel>>(data[1].ToString())!;
                    connects = JsonConvert.DeserializeObject<List<UserModel>>(data[2].ToString())!;

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

        private Func<InviteeModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            if (x.Status.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            return false;
        };

        #endregion

        #region Dialog Related Functions
        private void OpenSendInviteDialog()
        {
            var options = new DialogOptions { MaxWidth=MaxWidth.Small,FullWidth=true,CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters();
            parameters.Add("OnCreate", EventCallback.Factory.Create<int>(this, SendInviteCallBack));
            parameters.Add("MeetingID", MeetingID);
            parameters.Add("Connects", connects);

            DialogService.Show<SendInviteDialog>("Send Invite", parameters, options);
        }

        async Task SendInviteCallBack()
        {
            await GetMeetingDetails(MeetingID);
        }
        #endregion


        private async Task ResendEmail(int UserID)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    MeetingID = meeting.Id,
                    UserID = sessionModel.Id,
                    UserIDs = UserID.ToString()
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/SendInvite");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {


                    Snackbar.Add(returnMessage, Severity.Success);

                    string hostname = sessionModel.FirstName + " " + sessionModel.LastName;
                    NotificationModel notification = new NotificationModel { Message = hostname + " sent you a meeting invite. Please check meeting details on meeting hub screen.", Type = 4, UserId = UserID };
                    await NotificationManager.SendNotificationAsync(notification);
                    await GetMeetingDetails(meeting.Id);
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
