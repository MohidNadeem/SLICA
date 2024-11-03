
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;

namespace SlicaWeb.Pages.Account
{
    public partial class ResetPassword
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        UserModel resetUser = new UserModel();
        [Parameter]
        public string code { get; set; }
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
                else
                {
                    await VerifyCode();
                }
            }
        }
        #endregion

        #region Code Verification

        private async Task VerifyCode()
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    Code = code

                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), "", "", "/ACC/VerifyResetPasswordCode");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    string data = Convert.ToString(result["data"]);
                    resetUser = JsonConvert.DeserializeObject<List<UserModel>>(data)[0];
                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, resetUser);
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        #endregion

        #region Form Submission

        private async void OnResetFormSubmit()
        {
            try
            {
                List<string> validateParams = new List<string>
                {
                  "Email","Password","ConfirmPassword"
                };
                if (!CommonMethods.ValidateAttributes(resetUser, validateParams))
                {
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(new
                {
                    Email = resetUser.Email,
                    Password = resetUser.Password

                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), resetUser.Email, resetUser.Email, "/ACC/ResetPasswordUpdate");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    Snackbar.Add(returnMessage, Severity.Success);
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, resetUser);
                Snackbar.Add(e.Message, Severity.Error);
            }

        }
        #endregion

        #endregion
    }
}
