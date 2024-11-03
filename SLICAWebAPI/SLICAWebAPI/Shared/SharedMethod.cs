using System.Collections;
using SLICAWebAPI.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace SLICAWebAPI.Shared
{
    [ApiController]

    public  class SharedMethod
    {
        private static readonly string publicKey = "santhosh";
        private static readonly string secretKey = "engineer";
        public static string Encrypt(string plainText)
        {

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
        public static void LogException(Exception exception, string filename = "")
        {
            try
            {
                // Extract information from the stack trace
                var stackTrace = new System.Diagnostics.StackTrace(exception, true);
                var frame = stackTrace.GetFrame(0);
                string methodName = frame.GetMethod().Name;
                string parameters = "";
                string fileName = filename;
                // Create a Hashtable with exception details
                Hashtable htParams = new Hashtable
                {
                { "StackTrace", exception.StackTrace },
                { "FileName", fileName },
                { "ExceptionText", exception.Message },
                { "MethodName", methodName },
                { "Parameters", parameters }
                };

                // Use DataTransaction to log the exception
                var htResult = Transaction.DataTransaction(htParams, "[SP_Exception_Insert]");

            }
            catch (Exception ex)
            {
            }
        }

        private static string GetMethodParameters(StackFrame frame)
        {
            var method = frame.GetMethod();
            var parameters = method.GetParameters();

            // Extract parameter names and values
            var parameterValues = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var parameterName = parameter.Name;
                var parameterValue = frame.GetMethod().GetParameters()[i].DefaultValue; // Modify this as needed

                parameterValues.Append($"{parameterName}={parameterValue}");

                if (i < parameters.Length - 1)
                {
                    parameterValues.Append("&");
                }
            }

            return parameterValues.ToString();
        }

        public static dynamic ValidateParameters(Hashtable inputParameters, List<APIParameterModel> requiredParameters)
        {
            List<string> missingParameters = new List<string>();
            List<string> invalidParameters = new List<string>();

            foreach (var parameter in requiredParameters)
            {
                if (!inputParameters.ContainsKey(parameter.Name))
                {
                    missingParameters.Add(parameter.Name);
                }
                else
                {
                    string parameterValue = inputParameters[parameter.Name].ToString();
                    if (!IsValidType(parameterValue, parameter.Type))
                    {
                        invalidParameters.Add(parameter.Name);
                    }
                }
            }

            if (missingParameters.Count > 0 || invalidParameters.Count > 0)
            {
                string errorMessage = "Validation failed. ";

                if (missingParameters.Count > 0)
                {
                    errorMessage += $"Missing parameters: {string.Join(", ", missingParameters)}. ";
                }

                if (invalidParameters.Count > 0)
                {
                    errorMessage += $"Invalid parameters: {string.Join(", ", invalidParameters)}.";
                }

                return  new { status = 400, message = errorMessage };
            }

            return new { status = 200, message = "Success" };
        }

        private static bool IsValidType(string value, string expectedType)
        {
            if (value == null)
            {
                return true; // Consider null as valid for any type
            }

            switch (expectedType.ToUpper())
            {
                case "STRING":
                    return value is String;
                case "INT":
                    return Int32.TryParse(value, out var intvalue);
                case "BOOL":
                    return Boolean.TryParse(value, out var boolvalue);
                case "EMAIL":
                    return IsValidEmail(value.ToString());
                case "PASSWORD":
                    return IsValidPassword(value.ToString());
                case "DATETIME":
                    return DateTime.TryParse(value,out var datetimevalue);
                default:
                    return false;
            }
        }

        private static bool IsValidEmail(string email)
        {
            // Use the provided regex for email validation
            string emailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, emailRegex);
        }

        private static bool IsValidPassword(string password)
        {
            // Use the provided regex for password validation
            string passwordRegex = "^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+{}\\[\\]:;<>,.?~\\-])(?=.*[0-9]).{6,18}$";
            return Regex.IsMatch(password, passwordRegex);
        }
        
         public async static  Task<List<string>> GetCommonEnglishWords()
         {
                try
                {
                    // Create an instance of HttpClient
                    using (HttpClient client = new HttpClient())
                    {
                        // Set the request headers
                        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "b0b1cb3060msh8087176c0bf6e54p1e3bf6jsn48b34f76f578");
                        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "1000-most-common-words.p.rapidapi.com");

                        // Set the URL and parameters
                        string url = "https://1000-most-common-words.p.rapidapi.com/words/english";
                        string queryParams = "?words_limit=20";
                        string requestUrl = url + queryParams;

                        // Send the GET request
                        HttpResponseMessage response = await client.GetAsync(requestUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response content
                            string responseBody = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON response into a list of strings
                            List<string> words = JsonConvert.DeserializeObject<List<string>>(responseBody);
                           if(words == null)
                           {
                             words = new List<string> { "ABS", "BAR", "bind" };
                           }
                           return words;
                        }
                        else
                        {
                            // Print the error status code if the request fails
                            Console.WriteLine($"Error: {response.StatusCode}");
                            return new List<string> { "ABS", "BAR", "bind" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return null;
                }
         }

        public static dynamic Validate(Hashtable htParams)
        {
            try
            {

                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                string emailverification = "false";

                var htResult = Transaction.DataTransaction(htParams, "[SP_Authorize_Query]");
                if (htResult.Contains("Message"))
                {
                    returnContent = htResult["Message"].ToString();
                    returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                    returnMessage = returnContent.Split(":")[1];
                    emailverification = returnContent.Split(":")[2];
                }

                if (returnStatus == 200)
                {

                    return new { status = 200, message = returnMessage,emailverification = emailverification };
                }
                else
                {
                    return new { status = 401, message = returnMessage };
                }
            }
            catch (Exception ex)
            {
                return new { status = 401, message = "Authorization Failed" };

            }
        }
        public static dynamic ValidateAgora(Hashtable htParams)
        {
            try
            {

                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";

                var htResult = Transaction.DataTransaction(htParams, "[SP_AgoraValidations_Query]");
                if (htResult.Contains("Message"))
                {
                    returnContent = htResult["Message"].ToString();
                    returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                    returnMessage = returnContent.Split(":")[1];
                }
                List<JArray> Data = new List<JArray>();

                if (returnStatus == 201)
                {
                    DataSet ds = (DataSet)htResult["ds"];


                    if (ds.Tables.Count > 0)
                    {
                        DataTable agoraToken = ds.Tables[0];

                        if (!string.IsNullOrEmpty(agoraToken.Rows[0][0].ToString()))
                        {
                            Data.Add(JArray.Parse(agoraToken.Rows[0][0].ToString()!));
                        }
                        else
                        {
                            Data.Add(new JArray());
                        }

                    }

                }
                return new { status = returnStatus, data = Data, message = returnMessage };
            }
            catch (Exception ex)
            {
                return new { status = 400, message = "Invalid Meeting" };

            }
        }


    }
}

