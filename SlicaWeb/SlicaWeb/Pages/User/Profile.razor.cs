
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;
using SlicaWeb.Shared;

namespace SlicaWeb.Pages.User
{
    public partial class Profile
    {
        #region Attributes
        private UserModel sessionModel = new UserModel();
        static UserModel profileUser = new UserModel();
        EditContext userProfileContext = new EditContext(profileUser) ;
        bool boolean = true;
        public bool IsTwoFactor { get; set; } = false;
        IBrowserFile file;
        int maxFileSize = 5 * 1024 * 1024;
        string [] allowedExtenstions = new string[] { ".png", ".jpg", ".jpeg" };

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
                    userProfileContext.OnFieldChanged += editcontext_onfieldchanged;
                   await  GetUserDetail();
                }
               
            }
        }
        #endregion

        #region Header Detail Function

        private async Task GetUserDetail()
        {
            try
            {


                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id

                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/GetUserDetails");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    string data = Convert.ToString(result["data"]);
                    profileUser = JsonConvert.DeserializeObject<List<UserModel>>(data)[0];
                    IsTwoFactor = profileUser.IsTwoFactor;
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

                await CommonMethods.LogExceptionAsync(e, profileUser);
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        #endregion

        #region Dialog Related Function
        private void editcontext_onfieldchanged(object sender, FieldChangedEventArgs e)
        {
            boolean = false;
            StateHasChanged();

        }
        private void OpenDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogService.Show<DeleteAccountDialog>("Delete Profile", options);
        }

        private void OpenPasswordDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

            DialogService.Show<ChangePasswordDialog>("Change Password", options);
        }
        #endregion


        private void SelectFiles(InputFileChangeEventArgs e)
        {
            file = e.File;
            boolean = false;
        }


        #region Form Submission
        async Task Save()
        {
            try
            {

                userProfileContext.Validate();

                List<string> validateParams = new List<string>
                {
                  "Email","FirstName","LastName"
                };
                if (!CommonMethods.ValidateAttributes(profileUser, validateParams))
                {
                    return;
                }
                var fileName = profileUser.FileName;
                //Upload and Replace Profile Photo
                if (file != null)
                {
                    var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\Images\\Profile"); 

                    if (file.Size > maxFileSize)
                    {
                        Snackbar.Add($"File exceeds the maximum allowed file size.", Severity.Error);
                        return;
                    }

                    var fileExtension = Path.GetExtension(file.Name);
                    if (!allowedExtenstions.Contains(fileExtension))
                    {
                        Snackbar.Add($"File type not allowed", Severity.Error);
                        return;
                    }

                    fileName = sessionModel.FirstName + "_" + sessionModel.Id.ToString()+fileExtension;
                    var path = Path.Combine(uploadDirectory, fileName);
                    await using var fs = new FileStream(path, FileMode.Create);
                    await file.OpenReadStream(maxFileSize).CopyToAsync(fs);

                }
                var requestBody = JsonConvert.SerializeObject(new
                {
                    Email = profileUser.Email,
                    FirstName = profileUser.FirstName,
                    LastName = profileUser.LastName,
                    Bio = profileUser.Bio,
                    UserID= sessionModel.Id,
                    FileName = fileName
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/UpdateUserProfile");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];

                if (returnStatus == 200)
                {
                    
                    sessionModel.Email = profileUser.Email;
                    sessionModel.FirstName = profileUser.FirstName;
                    sessionModel.LastName = profileUser.LastName;
                    sessionModel.Bio = profileUser.Bio;
                    await CookieManager.SetCookie("LoggedInUserInfo",sessionModel);
                    Snackbar.Add(returnMessage, Severity.Success);                
                    StateHasChanged();
                    if (file != null)
                    {
                        await Task.Delay(2000);
                        NavigationManager.NavigateTo(NavigationManager.Uri, true);
                    }
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, profileUser);
                Snackbar.Add(e.Message, Severity.Error);
            }


           
        }
        async Task RemovePhoto()
        {

            var requestBody = JsonConvert.SerializeObject(new
            {
                FileName = "",
                UserID = sessionModel.Id,

            });

            var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/UpdateUserProfile");
            dynamic result = JsonConvert.DeserializeObject(jsonStr);
            int returnStatus = result["status"];
            string returnMessage = result["message"];

            if (returnStatus == 200)
            {
                profileUser.FileName = "";
                Snackbar.Add(returnMessage, Severity.Success);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add("Error occured",Severity.Error);
            }

        }
        #endregion

        #region 2FA
        async ValueTask UpdateTwoFactorValue(bool isChecked)
        {
            try
            {

                var requestBody = JsonConvert.SerializeObject(new
                {
                    Is2FA = isChecked,
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/ACC/UpdateUserProfile");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];

                if (returnStatus == 200)
                {
                    IsTwoFactor = isChecked;
                    sessionModel.IsTwoFactor = isChecked;
                    await CookieManager.SetCookie("LoggedInUserInfo", sessionModel);

                    Snackbar.Add(returnMessage, Severity.Success);

                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add(returnMessage, Severity.Error);
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, profileUser);
                Snackbar.Add(e.Message, Severity.Error);
            }



        }

        #endregion

        public async Task SendNotification()
        {
            NotificationModel notification = new NotificationModel { Message="Test Real Time Notification Received",Type=1,UserId=1002};
            
           await NotificationManager.SendNotificationAsync(notification);
        }
        #endregion
    }
}
