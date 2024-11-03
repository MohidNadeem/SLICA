using SlicaWeb.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlicaWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SlicaWeb.Common;

namespace SlicaWeb.Pages.Chat
{
    public partial class Chat
    {
        #region Attributes
        private HubConnection hubConnection;
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public string CurrentUserEmail { get; set; }
        public UserModel sessionModel = new UserModel();
        private List<MessageModel> messages = new List<MessageModel>();
        private List<UserModel> connectedUsers = new List<UserModel>();
        [Inject] private IJSRuntime? JsRuntime { get; set; }
        public UserModel currentChatUser;
        #endregion

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
                    hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/NotificationHub")).Build();

                    
                    hubConnection.On<MessageModel>("ReceiveMessage", (message) =>
                    {
                        if ((currentChatUser.Id == message.ReceiverID && sessionModel.Id == message.SenderID) || (currentChatUser.Id == message.SenderID && sessionModel.Id == message.ReceiverID))
                        {

                            if ((currentChatUser.Id == message.ReceiverID && sessionModel.Id == message.SenderID))
                            {
                                messages.Add( message);
                            }
                            else if ((currentChatUser.Id == message.SenderID && sessionModel.Id == message.ReceiverID))
                            {
                                messages.Add(message);
                            }
                            this.InvokeAsync(StateHasChanged);
                        }
                    });
                    if (hubConnection.State == HubConnectionState.Disconnected)
                    {
                        await hubConnection.StartAsync();
                    }
                    await GetChatData();
                    StateHasChanged();
                }
            }
            await JsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");

        }
        private async Task GetChatData()
        {
            
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/GetConnectedUser");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    connectedUsers = JsonConvert.DeserializeObject<List<UserModel>>(data[0].ToString())!;                 
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
        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && currentChatUser != null)
            {
                var chatHistory = new MessageModel()
                {
                    Message = CurrentMessage,
                    SenderID = sessionModel.Id,
                    CreatedDate = DateTime.Now,
                    ReceiverID = currentChatUser.Id,
                    ReceiverFileName = currentChatUser.FileName,
                    ReceiverFirstName = currentChatUser.FirstName,
                    ReceiverLastName = currentChatUser.LastName,
                    SenderFileName = sessionModel.FileName,
                    SenderFirstName = sessionModel.FirstName,
                    SenderLastName = sessionModel.LastName,
                };
                await NotificationManager.SendMessageAsync(chatHistory);
                CurrentMessage = string.Empty;
            }
        }
        
        async Task LoadUserChat(UserModel currentUser)
        {
            currentChatUser = currentUser;
            await LoadChat();
        }

        private async Task LoadChat()
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    ChatUserID = currentChatUser.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/GetUserChat");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    messages = JsonConvert.DeserializeObject<List<MessageModel>>(data[0].ToString())!;
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

    }
}
