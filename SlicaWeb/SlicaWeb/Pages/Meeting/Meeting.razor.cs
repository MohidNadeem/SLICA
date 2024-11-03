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
using Microsoft.JSInterop;
using System.Reflection;

namespace SlicaWeb.Pages.Meeting
{
    public partial class Meeting:IDisposable
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private HubConnection hubConnection;
        private AgoraTokenModel meeting = new AgoraTokenModel();
        private List<UserModel> participants = new List<UserModel>();
        [Inject] private IJSRuntime? JsRuntime { get; set; }

        private DotNetObjectReference<Meeting>? _objRef;

        [Parameter]
        public int MeetingCode { get; set; }
        public bool IsShowError = false;
        public bool IsShowMeeting = false;
        public string Message = "";
        #endregion


        #region Class Functions

        #region Overridden Functions
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                sessionModel = await CookieManager.GetCookie<UserModel>("LoggedInUserInfo");
                if (sessionModel == null)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/NotificationHub")).Build();
                    hubConnection.On<int>("ReceiveParticipant", (meetingcode) =>
                    {

                        if (MeetingCode == meetingcode)
                        {
                           GetParticipants();
                        }

                    });

                    hubConnection.On<int>("ReceiveTimeEndMeeting", (meetingcode) =>
                    {

                        if (MeetingCode == meetingcode && sessionModel.Id == meeting.CreatedBy)
                        {
                            EndMeeting();
                        }

                    });

                    hubConnection.On<int>("ReceiveHostEndMeeting", (meetingcode) =>
                    {

                        if (MeetingCode == meetingcode && sessionModel.Id != meeting.CreatedBy)
                        {
                            EndMeeting(true);
                        }

                    });
                    if (hubConnection.State == HubConnectionState.Disconnected)
                    {
                        await hubConnection.StartAsync();
                    }
                    await BundleAndSendDotNetHelper();

                    await GenerateToken();

                }
            }
        }

        #endregion
        private async Task GenerateToken()
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    MeetingCode = Convert.ToString(MeetingCode),
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Agora/GenerateToken");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200 || returnStatus == 201)
                {
                    JArray data = result["data"];
                    meeting = JsonConvert.DeserializeObject<AgoraTokenModel>(data[0][0].ToString())!;
                    await NotificationManager.SendParticipantNotification(MeetingCode);
                    IsShowMeeting = true;
                    IsShowError = false;
                    StateHasChanged();
                }
                else
                {
                    Message = returnMessage;
                    IsShowMeeting = false;
                    IsShowError = true;
                    StateHasChanged();

                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
                SnackBar.Add(e.Message, Severity.Error);
            }

        }
        private void Dashboard()
        {
            NavigationManager.NavigateTo("/dashboard");
        }

        private async Task PreviousPage()
        {
            await jsRuntime.InvokeAsync<object>("history.back");
        }

        private async Task GetParticipants()
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    MeetingCode = Convert.ToString(MeetingCode)
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Agora/GetParticipants");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200 || returnStatus == 201)
                {
                    JArray data = result["data"];
                    participants = JsonConvert.DeserializeObject<List<UserModel>>(data[0].ToString())!;
                    await InvokeAsync(StateHasChanged);
                    
                }
                else
                {
                    SnackBar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
                SnackBar.Add(e.Message, Severity.Error);
            }

        }
        [JSInvokable]
        public async Task EndMeeting(bool ishostend = false)
        {

            var requestBody = JsonConvert.SerializeObject(new
            {
                UserID = sessionModel.Id,
                HostID =  meeting.CreatedBy,
                MeetingID =  MeetingCode
            });

            var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/MEET/UpdateMeetingStatus");
            dynamic result = JsonConvert.DeserializeObject(jsonStr);
            int returnStatus = result["status"];
            string returnMessage = result["message"];


            if (sessionModel.Id == meeting.CreatedBy)
            {
                await NotificationManager.HostEndMeeting(MeetingCode);
            }
            else
            {
                if (ishostend)
                {
                    SnackBar.Add("Host ends a meeting",Severity.Info);
                    NavigationManager.NavigateTo("/dashboard",true);
                }
            }
        }

        [JSInvokable]
        public async Task TimeEndMeeting()
        {
            await NotificationManager.TimeEndMeeting(MeetingCode);
        }

        private async Task BundleAndSendDotNetHelper()
        {
            _objRef = DotNetObjectReference.Create(this);
            if (JsRuntime != null)
            {
                await JsRuntime.InvokeAsync<string>("SetDotNetHelper", _objRef);
            }
        }

        public void Dispose()
        {
            _objRef?.Dispose();
        }

        #endregion
    }
}