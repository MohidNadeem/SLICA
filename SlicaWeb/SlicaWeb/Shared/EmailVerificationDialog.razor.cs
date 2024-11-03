using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;


namespace SlicaWeb.Shared
{
    public partial class EmailVerificationDialog
    {

        #region Parameters
        [CascadingParameter]
        MudDialogInstance VerifyEmailAddressMudDialog { get; set; }
        #endregion

        #region Classes Objects
        UserModel blockUser = new UserModel();
        public UserModel sessionModel = new UserModel();
        HttpClient? objHttpClient = new HttpClient();

        #endregion

        #region Attributes
        private bool isShowLoader;
        private int isShowScreen = 1;
        string email = "";
        string DialogHeader = "Verify Email Address";
        #endregion


        #region Class Functions

        #region Overridden Functions
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                sessionModel = await cookieManager.GetCookie<UserModel>("LoggedInUserInfo");
                if (sessionModel == null)
                {
                    navigationManager.NavigateTo("/");
                }
                else
                {

                    email = sessionModel.Email;
                }
            }
        }

        #endregion

        #region Events Functions
        public async Task OnChangeEmail()
        {

            isShowLoader = true;
            await Task.Delay(1000);
            blockUser = new UserModel();
            isShowLoader = false;
            isShowScreen = 3;
            DialogHeader = "Change Email Address";
            StateHasChanged();
        }
        public async Task OnBackClick()
        {

            isShowLoader = true;
            await Task.Delay(1000);
            isShowLoader = false;
            isShowScreen = 1;
            DialogHeader = "Verify Email Address";
            StateHasChanged();
        }
        private async void OnLogout()
        {
            try
            {
                var sessionModel = await cookieManager.GetCookie<UserModel>("LoggedInUserInfo");

                if (sessionModel != null)
                {
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        UserID = sessionModel.Id
                    });

                    var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/UpdateLoginLog");
                }
                VerifyEmailAddressMudDialog.Close(DialogResult.Ok(true));
                await cookieManager.DeleteCookie("LoggedInUserInfo");
                navigationManager.NavigateTo("/");
            }

            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }

        }
        public async void OnResendEmail()
        {
            isShowLoader = true;
            string requestBody = JsonConvert.SerializeObject(new
            {
                UserID = sessionModel.Id,
                EmailID = sessionModel.Email,
                FirstName = sessionModel.FirstName
            });

            var response = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/SendVerificationEmail");
            dynamic responseJSON = JsonConvert.DeserializeObject(response);
            string message = responseJSON["message"];
            int status = Convert.ToInt32(responseJSON["status"]);

            if (status == 200)
            {
                isShowLoader = false;
                isShowScreen = 2;
                DialogHeader = "Verify Email Address";
                StateHasChanged();
            }
            else
            {
                isShowLoader = false;

                snackBar.Add("Something went wrong", Severity.Error);
                StateHasChanged();

            }
        }

        public async void OnSaveEmail()
        {
            try
            {
                List<string> attributes = new List<string> { "Email" };

                if (!CommonMethods.ValidateAttributes(blockUser, attributes))
                {
                    return;
                }

                if (sessionModel.Email == blockUser.Email)
                {
                    snackBar.Add("Please enter a different email", Severity.Error);
                    return;
                }
                isShowLoader = true;

                string requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id,
                    Email = blockUser.Email,
                });

                var response = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/APP/UpdateUserProfile");
                dynamic responseJSON = JsonConvert.DeserializeObject(response);
                string message = responseJSON["message"];
                int status = Convert.ToInt32(responseJSON["status"]);

                if (status == 200)
                {
                    sessionModel.Email = blockUser.Email;
                    await cookieManager.SetCookie("LoggedInUserInfo", sessionModel);

                        sessionModel.IsEmailVerified = false;
                        await cookieManager.SetCookie("LoggedInUserInfo", sessionModel);
                        email = sessionModel.Email;
                        isShowScreen = 1;
                        DialogHeader = "Verify Email Address";

                   
                }
                else
                {
                    snackBar.Add(message, Severity.Error);
                }
            }
            catch (Exception e)
            {
                snackBar.Add(e.Message.ToString(), Severity.Error);
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
            finally
            {
                isShowLoader = false;
                StateHasChanged();
            }

        }
        #endregion

        
        #endregion
    }
}