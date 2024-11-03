using SlicaWeb.Models;
using SlicaWeb.Common;
using MudBlazor;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SlicaWeb.Pages.Account
{
    partial class Register
    {
        #region Attributes
        UserModel signUpUser = new UserModel();
        private UserModel sessionModel = new UserModel();
        public bool IsLoad =false;
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
                    if (sessionModel.IsEmailVerified)
                    {
                        NavigationManager.NavigateTo("/dashboard");
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/email-verification");

                    }
                }
            }
        }
        #endregion
        #region Form Submission
        private async void OnSignUpFormSubmit()
        {
            try
            {
                IsLoad = true;
                StateHasChanged();
                List<string> validateParams = new List<string>
                {
                  "FirstName","LastName","Email","Password","ConfirmPassword","IsAgreePolicy"
                };
                if (!CommonMethods.ValidateAttributes(signUpUser, validateParams))
                {
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(new 
                {
                    FirstName = signUpUser.FirstName,
                    LastName = signUpUser.LastName,
                    Email = signUpUser.Email,
                    Password =signUpUser.Password,
                    SessionExpiry = AppConfiguration.configuration.GetValue<int>("SessionExpiry")
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), signUpUser.Email, signUpUser.Email, "/ACC/RegisterUser");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                string data = Convert.ToString(result["data"]);

                if (returnStatus == 200)
                {
                    var objLoggedInUser = JsonConvert.DeserializeObject<List<UserModel>>(data)[0];
                    await CookieManager.SetCookie("LoggedInUserInfo", objLoggedInUser);
                    NavigationManager.NavigateTo("/email-verification");
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch
            (Exception e)
            {
               await  CommonMethods.LogExceptionAsync(e, signUpUser);
               Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                IsLoad = false;
                StateHasChanged();
            }
        }
        #endregion


        #endregion
    }
}
