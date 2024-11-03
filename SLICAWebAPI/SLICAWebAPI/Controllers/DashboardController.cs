using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SLICAWebAPI.Models;
using SLICAWebAPI.Shared;
using System.Collections;
using System.Data;

namespace SLICAWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        [HttpPost]
        [Route("/DSB/GetDashboardData")]
        public ActionResult GetDashboardData(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Dashboard_Query]");
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
                            DataTable recentCalls = ds.Tables[1];
                            DataTable recentRequests = ds.Tables[2];
                            DataTable events = ds.Tables[3];
                            DataTable leaderboard = ds.Tables[4];
                            DataTable dayVideo = ds.Tables[5];

                            if (!string.IsNullOrEmpty(totalCounts.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(totalCounts.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if ( !string.IsNullOrEmpty(recentCalls.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(recentCalls.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }

                            if (!string.IsNullOrEmpty(recentRequests.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(recentRequests.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }

                            if (!string.IsNullOrEmpty(events.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(events.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if (!string.IsNullOrEmpty(leaderboard.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(leaderboard.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if (!string.IsNullOrEmpty(dayVideo.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(dayVideo.Rows[0][0].ToString()!));
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

        [HttpPost]
        [Route("/DSB/UpdateRequestStatus")]

        public ActionResult UpdateRequestStatus(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "RequestID",Type="INT" },
                  new APIParameterModel {Name = "Status",Type="INT" },
                  //new APIParameterModel {Name = "UserID",Type="INT" },
                  //new APIParameterModel {Name = "SenderID",Type="INT" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_ConnectionRequest_Update]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {

                        return Ok(new { status = 200, message = returnMessage });
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
