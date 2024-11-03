using Heron.MudCalendar;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlicaWeb.Common;
using SlicaWeb.Models;

namespace SlicaWeb.Pages.Dashboard
{
    partial class Home
    {


        #region Attributes
        private UserModel sessionModel = new UserModel();
        private DashboardModel dashboardDetails  = new DashboardModel();
        private HubConnection hubConnection;
        private string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";
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
                 
                    await GetDashboardData();
                    StateHasChanged();
                }
            }
        }
        
        #endregion

        #region Dashboard Data Related Functions
        private async Task GetDashboardData()
        {
            dashboardDetails.RecentMeetings = new List<MeetingModel>();
            dashboardDetails.RecentRequests = new List<RequestModel>();
            dashboardDetails.Leaderboard = new List<LeaderBoardModel>();

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/DSB/GetDashboardData");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    dashboardDetails = JsonConvert.DeserializeObject<List<DashboardModel>>(data[0].ToString())[0]!;
                    dashboardDetails.RecentMeetings = JsonConvert.DeserializeObject<List<MeetingModel>>(data[1].ToString())!;
                    dashboardDetails.RecentRequests = JsonConvert.DeserializeObject<List<RequestModel>>(data[2].ToString())!;
                    dashboardDetails.Events = JsonConvert.DeserializeObject<List<CustomCalendarItem>>(data[3].ToString())!;
                    dashboardDetails.Leaderboard = JsonConvert.DeserializeObject<List<LeaderBoardModel>>(data[4].ToString())!;
                    dashboardDetails.DayVideo = JsonConvert.DeserializeObject<VideoModel>(data[5][0].ToString())!;


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

        #endregion


        #region CRUD
        async Task UpdateRequest(RequestModel currentRequest,int status)
        {
            try
            {                
                var requestBody = JsonConvert.SerializeObject(new
                {
                    RequestID = currentRequest.Id,
                    Status = status,
                    UserID =0,
                    SenderID = 0
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/DSB/UpdateRequestStatus");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {


                    Snackbar.Add(returnMessage, Severity.Success);
                    await GetDashboardData();
                    if(status==1)
                    {
                        string receiverName = sessionModel.FirstName + " " + sessionModel.LastName;
                        NotificationModel notification = new NotificationModel { Message = receiverName+" accepted your request. You and "+receiverName+" are now in each other connections.", Type = 3, UserId = currentRequest.SenderID };
                        await NotificationManager.SendNotificationAsync(notification);
                    }
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
        #endregion
    }

}
