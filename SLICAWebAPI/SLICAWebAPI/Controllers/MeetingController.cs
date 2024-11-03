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
    public class MeetingController : Controller
    {

        [HttpPost]
        [Route("/MEET/InsertMeeting")]
        public ActionResult InsertMeeting(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "Title",Type="STRING" },
                  new APIParameterModel {Name = "Duration",Type="INT" },
                  new APIParameterModel {Name = "StartDateTime",Type="DATETIME" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Meeting_Insert]");
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


        [HttpPost]
        [Route("/MEET/UpdateMeeting")]
        public ActionResult UpdateMeeting(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "ID",Type="INT" },
                  new APIParameterModel {Name = "Title",Type="STRING" },
                  new APIParameterModel {Name = "Duration",Type="INT" },
                  new APIParameterModel {Name = "StartDateTime",Type="DATETIME" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Meeting_Update]");
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

        [HttpPost]
        [Route("/MEET/SendInvite")]
        public ActionResult SendInvite(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "UserIDs",Type="STRING" },
                  new APIParameterModel {Name = "MeetingID",Type="INT" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_InviteMeetingMembers_Query]");
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


        [HttpPost]
        [Route("/MEET/GetMeetingHubData")]
        public ActionResult GetMeetingHubData(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingHub_Query]");
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
                            DataTable upComingMeetings = ds.Tables[0];

                            if (!string.IsNullOrEmpty(upComingMeetings.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(upComingMeetings.Rows[0][0].ToString()!));
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
        [Route("/MEET/GetMeetingDetailsData")]
        public ActionResult GetMeetingDetailsData(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "MeetingID",Type="INT" },
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingDetails_Query]");
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
                            DataTable meetingDetials = ds.Tables[0];
                            DataTable inviteeDetails = ds.Tables[1];
                            DataTable connectDetails = ds.Tables[2];

                            if (!string.IsNullOrEmpty(meetingDetials.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(meetingDetials.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if (!string.IsNullOrEmpty(inviteeDetails.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(inviteeDetails.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if (!string.IsNullOrEmpty(connectDetails.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(connectDetails.Rows[0][0].ToString()!));
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
        [Route("/MEET/GetMeetingLogData")]
        public ActionResult GetMeetingLogData(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "Type",Type="INT" },

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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingLog_Query]");
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
                            DataTable meetingLog = ds.Tables[0];

                            if (!string.IsNullOrEmpty(meetingLog.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(meetingLog.Rows[0][0].ToString()!));
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
        [Route("/MEET/UpdateMeetingPinStatus")]

        public ActionResult UpdateMeetingPinStatus(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "MeetingID",Type="INT" },
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingPinStatus_Update]");
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

        [HttpPost]
        [Route("/MEET/UpdateMeetingStatus")]

        public ActionResult UpdateMeetingStatus(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "MeetingID",Type="INT" },
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "HostID",Type="INT" }

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_MeetingStatus_Update]");
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
