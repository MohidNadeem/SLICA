using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using SlicaWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace SlicaWeb.Common
{
    public class CommonMethods
    {
        public static bool ValidateAttributes<T>(T model, List<string> attributeNames)
        {
            var context = new ValidationContext(model);

            foreach (var attributeName in attributeNames)
            {
                var attribute = typeof(T).GetProperty(attributeName);

                if (attribute != null)
                {
                    // Check if the attribute has validation attributes applied.
                    var validationAttributes = attribute.GetCustomAttributes(typeof(ValidationAttribute), true);

                    foreach (var validationAttribute in validationAttributes)
                    {
                        var result = ((ValidationAttribute)validationAttribute).GetValidationResult(attribute.GetValue(model), context);

                        if (result != ValidationResult.Success)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static async Task<string> PostDataAsync(string requestBody, string apiKey, string sessionKey, string endPoint)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
                    httpClient.DefaultRequestHeaders.Add("SessionKey", sessionKey);

                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(AppConfiguration.configuration.GetValue<string>("APIBaseURL") + endPoint, content);

                     string returnString = await response.Content.ReadAsStringAsync();
                    return returnString;
                }
            }
            catch (Exception ex)
            {
                 return null;
            }
        }
        public static async Task LogExceptionAsync(Exception exception, UserModel objLoggedInUser)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var stackTrace = new System.Diagnostics.StackTrace(exception, true);
                    var frame = stackTrace.GetFrame(0);

                    string fileName = "";
                    if(frame.GetFileName() != null)
                    {
                        fileName = frame.GetFileName();
                    }
                    
                    var methodName = frame.GetMethod().Name;
                    var parameters = GetMethodParameters(frame);

                    var logObject = JsonConvert.SerializeObject(new
                    {
                        StackTrace = exception.StackTrace,
                        FileName = fileName,
                        ExceptionText = exception.Message,
                        MethodName = methodName,
                        Parameters = parameters,
                        UserID = objLoggedInUser.Id
                    });


                    var content = new StringContent(logObject.ToString(), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(AppConfiguration.configuration.GetValue<string>("APIBaseURL") + "/APP/InsertException", content);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private static string GetMethodParameters(StackFrame frame)
        {
            var method = frame.GetMethod();
            var parameters = method.GetParameters();

            var parameterValues = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var parameterName = parameter.Name;
                var parameterValue = frame.GetMethod().GetParameters()[i].DefaultValue; 

                parameterValues.Append($"{parameterName}={parameterValue}");

                if (i < parameters.Length - 1)
                {
                    parameterValues.Append("&");
                }
            }

            return parameterValues.ToString();
        }


        private static readonly string publicKey = "santhosh";
        private static readonly string secretKey = "engineer";
        public static string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                return "";
            }
            string ToReturn = "";

            byte[] secretkeyByte = { };
            secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publicKey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(plainText);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                ToReturn = Convert.ToBase64String(ms.ToArray());
            }
            return ToReturn;
        }

        public static string Decrypt(string encryptedText)
        {
            if(encryptedText == null)
            {
                return "";
            }
            string ToReturn = "";

            byte[] privatekeyByte = { };
            privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publicKey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = new byte[encryptedText.Replace(" ", "+").Length];
            inputbyteArray = Convert.FromBase64String(encryptedText.Replace(" ", "+"));
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                ToReturn = encoding.GetString(ms.ToArray());
            }
            return ToReturn;
        }
        


    }
}
