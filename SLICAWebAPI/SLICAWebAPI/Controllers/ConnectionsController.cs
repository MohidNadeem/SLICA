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
    public class ConnectionsController : Controller
    {

        [HttpPost]
        [Route("/CON/GetUserConnections")]
        public ActionResult GetUserConnections(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_UserConnections_Query]");
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
                            DataTable userConnections = ds.Tables[0];

                            if (!string.IsNullOrEmpty(userConnections.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(userConnections.Rows[0][0].ToString()!));
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
        [Route("/CON/GetUserRequests")]
        public ActionResult GetUserRequests(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_UserRequests_Query]");
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
                            DataTable userRequests = ds.Tables[0];

                            if (!string.IsNullOrEmpty(userRequests.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(userRequests.Rows[0][0].ToString()!));
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
        [Route("/CON/GetSendRequestData")]
        public ActionResult GetSendRequestData(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
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
                else
                {
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_SendRequestData_Query]");
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
                            DataTable sendRequest = ds.Tables[0];

                            if (!string.IsNullOrEmpty(sendRequest.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(sendRequest.Rows[0][0].ToString()!));
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
