using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;
namespace SlicaWeb.Pages.Account
{
    public partial class Login
    {
        #region Attributes
        UserModel signInUser = new UserModel();
        private UserModel sessionModel = new UserModel();
        public bool IsLoad = false;

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
                    if(sessionModel.IsEmailVerified)
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

        private async void OnSignInFormSubmit()
        {
            try
            {
                IsLoad = true;
                StateHasChanged();
                List<string> validateParams = new List<string>
                {
                  "Email","Password"
                };
                if (!CommonMethods.ValidateAttributes(signInUser, validateParams))
                {
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(new
                {
                    Email = signInUser.Email,
                    Password = signInUser.Password,
                    SessionExpiry = AppConfiguration.configuration.GetValue<int>("SessionExpiry")

                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), signInUser.Email, signInUser.Email, "/ACC/AuthenticateUser");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    string data = Convert.ToString(result["data"]);

                    UserModel objLoggedInUser = JsonConvert.DeserializeObject<List<UserModel>>(data)[0];

                    await CookieManager.SetCookie("LoggedInUserInfo", objLoggedInUser);
                    if(objLoggedInUser.IsEmailVerified)
                    {
                        NavigationManager.NavigateTo("/dashboard");

                    }
                    else
                    {
                        NavigationManager.NavigateTo("/email-verification");

                    }
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, signInUser);
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
