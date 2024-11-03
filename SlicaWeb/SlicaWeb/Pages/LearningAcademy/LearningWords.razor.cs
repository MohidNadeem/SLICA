using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlicaWeb.Common;
using SlicaWeb.Models;

namespace SlicaWeb.Pages.LearningAcademy
{
    public partial class LearningWords
    {
        public static bool resetValueOnEmptyText;
        public static bool coerceText;
        public static bool coerceValue;
        public static string value2;
        public static List<string> states = new List<string>();
        public List<VideoModel> popularVideos = new List<VideoModel>();
        public List<VideoModel> likedVideos = new List<VideoModel>();
        public List<VideoModel> foryouVideos = new List<VideoModel>();
        private UserModel sessionModel = new UserModel();
        public List<VideoModel> searchVideos = new List<VideoModel>();


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                sessionModel = await CookieManager.GetCookie<UserModel>("LoggedInUserInfo");
                if (sessionModel == null)
                {
                    navigationManager.NavigateTo("/");
                }
                else
                {
                    await GetVideosData();
                }
            }
        }
        private async Task GetVideosData()
        {

            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    UserID = sessionModel.Id
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Search/GetVideosData");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    JArray data = result["data"];
                    foryouVideos = JsonConvert.DeserializeObject<List<VideoModel>>(data[0].ToString())!;
                    likedVideos = JsonConvert.DeserializeObject<List<VideoModel>>(data[1].ToString())!;
                    popularVideos = JsonConvert.DeserializeObject<List<VideoModel>>(data[2].ToString())!;
                    StateHasChanged();
                }
                else
                {
                    popularVideos = new List<VideoModel>();
                }
            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
                Snackbar.Add(e.Message, Severity.Error);
            }

        }

        public  async Task<IEnumerable<string>> Search2(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        UserID = sessionModel.Id,
                        SearchText = value
                    });

                    var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Search/SearchVideos");
                    dynamic result = JsonConvert.DeserializeObject(jsonStr);
                    int returnStatus = result["status"];
                    string returnMessage = result["message"];
                    if (returnStatus == 200)
                    {
                        JArray data = result["data"];
                        searchVideos = JsonConvert.DeserializeObject<List<VideoModel>>(data[0].ToString())!;
                        StateHasChanged();
                    }
                    else
                    {
                        searchVideos = new List<VideoModel>();
                    }
                }
                else
                {
                    searchVideos = new List<VideoModel>();
                }
            }        
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, sessionModel);
                Snackbar.Add(e.Message, Severity.Error);
            }

            

            return new List<string>();



        }

        async Task UpdateVideoView(int videoID)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    VideoID = videoID,
                    IsView = true,
                    UserID = sessionModel.Id,
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Search/UpdateVideoAction");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    await GetVideosData();
                    
                }
                

            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
        }
        async Task UpdateVideoLike(int videoID,bool value)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new
                {
                    VideoID = videoID,
                    IsLike = !value,
                    UserID = sessionModel.Id,
                });

                var jsonStr = await CommonMethods.PostDataAsync(requestBody.ToString(), sessionModel.APIKey, sessionModel.SessionKey, "/Search/UpdateVideoAction");
                dynamic result = JsonConvert.DeserializeObject(jsonStr);
                int returnStatus = result["status"];
                string returnMessage = result["message"];
                if (returnStatus == 200)
                {
                    await GetVideosData();

                }


            }
            catch (Exception e)
            {
                await CommonMethods.LogExceptionAsync(e, new UserModel());
            }
        }

    }

}


