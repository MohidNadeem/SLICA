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
using static MudBlazor.Colors;

namespace SlicaWeb.Pages.Connections
{
    public partial class SendRequests
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<RequestModel> requestDetails = new List<RequestModel>();
        private HubConnection hubConnection;
        private string _searchString;
        private int _filterType = 1;
        private static string _userName;
        public static List<string> users = new List<string>();

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
                    await GetSendRequestData();

                }

            }
        }

        #endregion


        #region Meeting Data Related Functions

        public  async Task<IEnumerable<string>> SearchUser(string value)
        {
                _userName = value;
            await GetSendRequestData();
            return new List<string> ();
        }

        private  async Task GetSendRequestData()
        {
            
                try
                {
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        UserID = sessionModel.Id,
                        UserName = _userName??"".Trim()
                    });

                    var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/CON/GetSendRequestData");
                    dynamic result = JsonConvert.DeserializeObject(jsonStr);
                    int returnStatus = result["status"];
                    string returnMessage = result["message"];
                    if (returnStatus == 200)
                    {
                        JArray data = result["data"];
                        requestDetails = JsonConvert.DeserializeObject<List<RequestModel>>(data[0].ToString())!;
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

        private Func<RequestModel, bool> _quickFilter => x =>
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



        #region CRUD Functions

        async Task UpdateRequest(RequestModel currentRequest, int status)
        {
            try
            {
                if (status == 3)
                {
                    var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseOnEscapeKey = true, CloseButton = true };
                    var parameters = new DialogParameters();
                    parameters.Add("OnDisconnect", EventCallback.Factory.Create<int>(this, GetSendRequestData));
                    parameters.Add("ID", currentRequest.Id);

                    DialogService.Show<DisconnectDialog>("Disconnect User", parameters, options);

                }
                else
                {
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        RequestID = currentRequest.Id,
                        Status = status,
                        UserID = currentRequest.userId,
                        SenderID = sessionModel.Id
                    });

                    var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/DSB/UpdateRequestStatus");
                    dynamic result = JsonConvert.DeserializeObject(jsonStr);
                    int returnStatus = result["status"];
                    string returnMessage = result["message"];
                    if (returnStatus == 200)
                    {
                        Snackbar.Add(returnMessage, Severity.Success);
                        await GetSendRequestData();
                        if (status == 1)
                        {
                            string receiverName = sessionModel.FirstName + " " + sessionModel.LastName;
                            NotificationModel notification = new NotificationModel { Message = receiverName + " accepted your request. You and " + receiverName + " are now in each other connections.", Type = 3, UserId = currentRequest.userId };
                            await NotificationManager.SendNotificationAsync(notification);
                        }
                        else if (status == 0)
                        {

                            string senderName = sessionModel.FirstName + " " + sessionModel.LastName;
                            NotificationModel notification = new NotificationModel { Message = senderName + " sent you a connection request.", Type = 2, UserId = currentRequest.userId };
                            await NotificationManager.SendNotificationAsync(notification);
                        }
                    }
                    else
                    {
                        Snackbar.Add(returnMessage, Severity.Error);
                    }
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
        }
        async Task OpenBioDialog(string Bio,string Name,string FileName)
        {
            var options = new DialogOptions { CloseButton = true,MaxWidth=MaxWidth.Small,FullWidth=true };
            var parameters = new DialogParameters();
            parameters.Add("FileName", FileName);
            parameters.Add("Name", Name);
            parameters.Add("Bio", Bio);
            DialogService.Show<BioDetailsDialog>("Bio Details", parameters, options);

        }
        #endregion
        #endregion
    }
}
