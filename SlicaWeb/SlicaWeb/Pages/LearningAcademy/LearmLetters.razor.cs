using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SlicaWeb.Models;

namespace SlicaWeb.Pages.LearningAcademy
{
    public partial class LearmLetters
    {
        public List<char> alphabets = new List<char>(new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });
        private UserModel sessionModel = new UserModel();

        public char GetNextAlphabet(char currentAlphabet)
        {
            int currentIndex = alphabets.IndexOf(currentAlphabet);
            int nextIndex = (currentIndex + 1) % alphabets.Count;
            return alphabets[nextIndex];
        }

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

                }
            }
        }


    }
}
