using SLICAWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SLICAWebAPI.Shared;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Data;
using AdmissionPortalAPI.BAL;
namespace SLICAWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgoraController : ControllerBase
    {
        
        [HttpPost]
        [Route("/Agora/GenerateToken")]
        public async Task<IActionResult> GenerateToken(Hashtable htRequestParams)
        {
            var AppSetting = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            Dictionary<string, object> dic2response = new Dictionary<string, object>();
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                string channelName = "";
                string token = "";
                string? appId = AppSetting.GetSection("Agora").GetSection("AppID").Value;
                string? appCertificate = AppSetting.GetSection("Agora").GetSection("AppCertificate").Value;

                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "MeetingCode",Type="STRING" },
                  new APIParameterModel {Name = "UserID",Type="INT" }
                };
                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                Hashtable headerParams = new Hashtable();
                headerParams.Add("SessionKey", Request.Headers["SessionKey"].ToString());
                headerParams.Add("APIKey", Request.Headers["APIKey"].ToString());

                var result = SharedMethod.Validate(headerParams);
                if (Convert.ToInt32(result.status) != 200)
                {
                    return Unauthorized(result);
                }

                var validateresult = SharedMethod.ValidateAgora(htRequestParams);
                if (Convert.ToInt32(validateresult.status) != 200)
                {

                    if (Convert.ToInt32(obj.status) == 201)
                    {
                        return Ok(new { status = 200, data = obj.data, message = returnMessage });
                    }
                    return BadRequest(validateresult);
                }
                else
                {
                    channelName = "Meeting" + DateTime.Now.ToString("yyMMddHHmmss");
                    token = RTCTokenBuilder.buildToken(appId, appCertificate, channelName);
                    htRequestParams.Add("Token", token);
                    htRequestParams.Add("AppID", appId);
                    htRequestParams.Add("AppCertificate", appCertificate);
                    htRequestParams.Add("ChannelName", channelName);

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_AgoraToken_Insert]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }
                    else
                    {
                        return BadRequest(new { status = 400, message = "Token Generation Failed" });
                    }
                    if (returnStatus == 200)
                    {
                        List<JArray> Data = new List<JArray>();
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
                        else
                        {
                            return BadRequest(new { status = 400, message = "Token Generation Failed" });
                        }

                        return Ok(new { status = 200, data = Data, message = returnMessage });
                    }
                    else
                    {
                        return BadRequest(new { status = 400, message = returnMessage });
                    }
                }
            }
            catch (Exception e)
            {
                SharedMethod.LogException(e);
                return BadRequest(new { status = 400, message = e.Message });
            }
        }

        [HttpPost]
        [Route("/Agora/GetParticipants")]
        public ActionResult GetParticipants(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "MeetingCode",Type="STRING" },
                };


                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                Hashtable headerParams = new Hashtable();
                headerParams.Add("SessionKey", Request.Headers["SessionKey"].ToString());
                headerParams.Add("APIKey", Request.Headers["APIKey"].ToString());

                var result = SharedMethod.Validate(headerParams);
                if (Convert.ToInt32(result.status) != 200)
                {
                    return Unauthorized(result);
                }
                else
                {
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingParticipants_Query]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {
                        List<JArray> Data = new List<JArray>();
                        DataSet ds = (DataSet)htResult["ds"];


                        if (ds.Tables.Count > 0)
                        {
                            DataTable totalCounts = ds.Tables[0];
                            

                            if (!string.IsNullOrEmpty(totalCounts.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(totalCounts.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            
                        }
                        else
                        {
                            return BadRequest(new { status = 400, message = "No Data Found" });
                        }

                        return Ok(new { status = 200, data = Data, message = returnMessage });
                    }
                    else
                    {
                        return BadRequest(new { status = 400, message = returnMessage });
                    }
                }
            }
            catch (Exception e)
            {
                SharedMethod.LogException(e);
                return BadRequest(new { status = 400, message = e.Message });
            }
        }

    }
}
