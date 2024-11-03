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
using SlicaWeb.Pages.Meeting;

namespace SlicaWeb.Pages.Connections
{
    public partial class MyConnections
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        private List<ConnectionModel> connectionDetails = new List<ConnectionModel>();
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

                    await GetMyConnectionData();
                }
            }
        }

        #endregion


        #region Meeting Data Related Functions
        private async Task GetMyConnectionData()
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/CON/GetUserConnections");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    connectionDetails = JsonConvert.DeserializeObject<List<ConnectionModel>>(data[0].ToString())!;
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

        private Func<ConnectionModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (x.ConnectedSince!.Value.ToString("dd-MMM-yyyy HH:mm").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

       
        #endregion



        #region CRUD Functions

        private async void OnDisconnect(int id)
        {
            try
            {
                var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseOnEscapeKey = true, CloseButton = true };
                var parameters = new DialogParameters();
                parameters.Add("OnDisconnect", EventCallback.Factory.Create<int>(this, GetMyConnectionData));
                parameters.Add("ID", id);

                DialogService.Show<DisconnectDialog>("Disconnect User", parameters, options);

            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
        }

        async Task OpenBioDialog(string Bio, string Name, string FileName)
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
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
