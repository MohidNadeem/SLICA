
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;

namespace SlicaWeb.Pages.Account
{
    public partial class ForgotPassword
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        UserModel forgotUser = new UserModel();
        private bool isShow = false;

        #endregion

        #region Class Functions
        #region Overridden Functions

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                sessionModel = await CookieManager.GetCookie<UserModel>("LoggedInUserInfo");
                if (sessionModel != null && sessionModel.Id > 0)
                {
                    NavigationManager.NavigateTo("/dashboard");
                }
            }
        }
        #endregion

        #region Form Submission

        public async Task SentEmail()
        {
            try
            {
                List<string> validateParams = new List<string>
                {
                  "Email"
                };
                if (!CommonMethods.ValidateAttributes(forgotUser, validateParams))
                {
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(new
                {                   
                    Email = forgotUser.Email,
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), forgotUser.Email, forgotUser.Email, "/ACC/ForgotPassword");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];

                if (returnStatus == 200)
                {
                    isShow = true;
                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, forgotUser);
                Snackbar.Add(e.Message, Severity.Error);
            }

        }
        #endregion
        #endregion

    }
}
