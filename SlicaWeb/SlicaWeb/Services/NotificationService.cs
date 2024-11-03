using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;

namespace SlicaWeb.Services
{
    public class NotificationService
    {
        public readonly HubConnection hubConnection;
        public readonly NavigationManager navigationManager;
        public readonly CookieManagement cookieManager;


        public NotificationService(NavigationManager navigation, CookieManagement cookie)
        {
            navigationManager = navigation;
            cookieManager = cookie;
            hubConnection = new HubConnectionBuilder().WithUrl(navigationManager.ToAbsoluteUri("/NotificationHub")).Build();
            hubConnection.StartAsync();
        }

        public async Task SendNotificationAsync(NotificationModel notification)
        {
            var sessionModel = await cookieManager.GetCookie<UserModel>("LoggedInUserInfo");

            try
            {

                var requestBody = JsonConvert.SerializeObject(new
                {
                    NotificationMessage = notification.Message,
                    UserID = notification.UserId,
                    SenderID = notification.SenderId == null ? 0 : notification.SenderId,
                    Type = notification.Type
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/InsertNotification");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];

                if (returnStatus == 200)
                {
                    await hubConnection.SendAsync("SendNotification", notification);
                }
                
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
            }

        }

        public async Task SendParticipantNotification(int meetingcode)
        {
                    await hubConnection.SendAsync("SendParticipantNotification", meetingcode);
        }

        public async Task HostEndMeeting(int meetingcode)
        {
            await hubConnection.SendAsync("HostEndMeeting", meetingcode);
        }

        public async Task TimeEndMeeting(int meetingcode)
        {
            await hubConnection.SendAsync("TimeEndMeeting", meetingcode);
        }

        public async Task SendMessageAsync(MessageModel message)
        {
            var sessionModel = await cookieManager.GetCookie<UserModel>("LoggedInUserInfo");

            try
            {

                var requestBody = JsonConvert.SerializeObject(new
                {
                    MessageText = message.Message,
                    SenderID = message.SenderID,
                    ReceiverID = message.ReceiverID,
                    CreatedDate = message.CreatedDate
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/InsertMessage");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];

                if (returnStatus == 200)
                {
                    await hubConnection.SendAsync("SendMessage", message);
                }

            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
            }

        }

    }
}
