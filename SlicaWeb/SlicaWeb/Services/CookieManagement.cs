using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using SlicaWeb.Common;


namespace SlicaWeb.Services
{
    // CookieService.cs

    
    public class CookieManagement
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieManagement(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetCookie<T>(string name, T value)
        {
            var jsonString = JsonSerializer.Serialize(value);
            var encryptedValue = CommonMethods.Encrypt(jsonString);
            await _jsRuntime.InvokeVoidAsync("cookies.setCookie", name, encryptedValue);
        }

        public async Task<T> GetCookie<T>(string name)
        {
            var encryptedValue = await _jsRuntime.InvokeAsync<string>("cookies.getCookie", name);
            if (encryptedValue == null)
            {
                return default; // or throw an exception, depending on your requirements
            }
            var jsonString = CommonMethods.Decrypt(encryptedValue);

            return JsonSerializer.Deserialize<T>(jsonString);
        }

        public async Task DeleteCookie(string name)
        {
            await _jsRuntime.InvokeVoidAsync("cookies.deleteCookie", name);
        }

        
    }

}
