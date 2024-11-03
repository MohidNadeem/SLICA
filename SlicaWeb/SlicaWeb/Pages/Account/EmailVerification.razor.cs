using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SlicaWeb.Common;
using SlicaWeb.Models;
using System.Drawing;
using Color = MudBlazor.Color;

namespace SlicaWeb.Pages.Account
{
    public partial class EmailVerification
    {
        #region Parameters
        [Parameter]
        public string? userId { get; set; }
        #endregion

        #region Classes Objects
        HttpClient? objHttpClient = new HttpClient();
        public UserModel sessionModel = new UserModel();

        #endregion

        #region Attributes
        string message = "Link either expired or invalid. Please contact support.";
        string icon = "icon-warning-redo-line";
        Color iconColor = Color.Warning;
        #endregion


        #region Functions

        #region Overridden Functions
        protected override void OnParametersSet()
        {
            userId = userId ?? "";
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (userId == "")
                {
                    icon = "icon-warning-redo-line";
                    message = "Link either expired or invalid. Please contact support.";
                    iconColor = Color.Warning;
                }
                else
                {
                    string requestBody = JsonConvert.SerializeObject(new
                    {
                        UserID = userId
                    });

                    var response = await Common.CommonMethods.PostDataAsync(requestBody.ToString(), userId, userId, "/ACC/UpdateEmailVerificationStatus");
                    dynamic responseJSON = JsonConvert.DeserializeObject(response);
                    string message = responseJSON["message"];
                    int status = Convert.ToInt32(responseJSON["status"]);

                    if (status == 200)
                    {
                        icon = "icon-success-redo-line";
                        this.message = message;
                        iconColor = Color.Success;
                        sessionModel = await cookieManager.GetCookie<UserModel>("LoggedInUserInfo");

                        if (sessionModel != null)
                        {
                            sessionModel.IsEmailVerified = true;
                            await cookieManager.SetCookie("LoggedInUserInfo", sessionModel);
                        }
                    }
                    else
                    {
                        icon = "icon-warning-redo-line";
                        this.message = message;
                        iconColor = Color.Warning;
                    }

                }


            }
            catch (Exception ex)
            {
                await CommonMethods.LogExceptionAsync(ex, new UserModel());
            }
            finally
            {
                StateHasChanged();
            }
        }

        #endregion

        #endregion

    }
}
