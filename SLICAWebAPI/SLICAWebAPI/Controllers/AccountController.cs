using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SLICAWebAPI.Models;
using SLICAWebAPI.Shared;
using System.Collections;
using System.Data;
using System.Globalization;

namespace SLICAWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        string filename = "AccountController.cs";
        [HttpPost]
        [Route("/ACC/RegisterUser")]
        public ActionResult RegisterUser(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "FirstName",Type="STRING" },
                  new APIParameterModel {Name = "LastName",Type="STRING" },
                  new APIParameterModel {Name = "Email",Type="EMAIL" },
                  new APIParameterModel {Name = "Password",Type="PASSWORD" }
                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                else
                {
                    htRequestParams["Password"] = SharedMethod.Encrypt(htRequestParams["Password"].ToString());

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_RegisterUser]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {
                        DataSet ds = (DataSet)htResult["ds"];
                        DataTable dt = new DataTable();
                        string data = "";
                        if (ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                        if (dt.Columns.Count > 0)
                        {
                            data = dt.Rows[0][0].ToString();
                        }
                        return Ok(new { status = 200, data = data, message = returnMessage });
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
        [Route("/ACC/AuthenticateUser")]
        public ActionResult AuthenticateUser(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "Email",Type="EMAIL" },
                  new APIParameterModel {Name = "Password",Type="PASSWORD" }
                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                else
                {
                    htRequestParams["Password"] = SharedMethod.Encrypt(htRequestParams["Password"].ToString());
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_AuthenticateUser]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {
                        DataSet ds = (DataSet)htResult["ds"];
                        DataTable dt = new DataTable();
                        string data = "";
                        if (ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                        if (dt.Columns.Count > 0)
                        {
                            data = dt.Rows[0][0].ToString();
                        }
                        return Ok(new { status = 200, data = data, message = returnMessage });
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
        [Route("/ACC/UpdateLoginLog")]
        public ActionResult UpdateLoginLog(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_LoginLog_Update]");
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
        [Route("/ACC/ForgotPassword")]
        public ActionResult ForgotPassword(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "Email",Type="EMAIL" }
                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                else
                {
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_ForgotPassword_Query]");
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
        [Route("/ACC/VerifyResetPasswordCode")]
        public ActionResult VerifyResetPasswordCode(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "Code",Type="STRING" },
                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                else
                {
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_VerifyResetPasswordCode_Query]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {
                        DataSet ds = (DataSet)htResult["ds"];
                        DataTable dt = new DataTable();
                        string data = "";
                        if (ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                        if (dt.Columns.Count > 0)
                        {
                            data = dt.Rows[0][0].ToString();
                        }
                        return Ok(new { status = 200, data = data, message = returnMessage });
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
        [Route("/ACC/ResetPasswordUpdate")]
        public ActionResult ResetPasswordUpdate(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "Email",Type="EMAIL" },
                  new APIParameterModel {Name = "Password",Type="STRING" },

                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                else
                {
                    htRequestParams["Password"] = SharedMethod.Encrypt(htRequestParams["Password"].ToString());

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_ResetPassword_Update]");
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
        [Route("/ACC/GetUserDetails")]
        public ActionResult GetUserDetails(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_GetUserDetails_Query]");
                    if (htResult.Contains("Message"))
                    {
                        returnContent = htResult["Message"].ToString();
                        returnStatus = Convert.ToInt32(returnContent.Split(":")[0]);
                        returnMessage = returnContent.Split(":")[1];
                    }

                    if (returnStatus == 200)
                    {
                        DataSet ds = (DataSet)htResult["ds"];
                        DataTable dt = new DataTable();
                        string data = "";
                        if (ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                        if (dt.Columns.Count > 0)
                        {
                            data = dt.Rows[0][0].ToString();
                        }
                        return Ok(new { status = 200, data = data, message = returnMessage });
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
        [Route("/ACC/UpdateUserProfile")]
        public ActionResult UpdateUserProfile(Hashtable htRequestParams)
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_UserProfile_Update]");
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
        [Route("/ACC/DeleteAccount")]
        public ActionResult DeleteAccount(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_DeleteAccount_Query]");
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
        [Route("/APP/InsertException")]
        public ActionResult InsertException(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";



                var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Exception_Insert]");
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
            catch (Exception e)
            {
                SharedMethod.LogException(e);
                return BadRequest(new { status = 400, message = e.Message });
            }
        }

        [HttpPost]
        [Route("/ACC/ChangePassword")]
        public ActionResult ChangePassword(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "Password",Type="PASSWORD" },
                  new APIParameterModel {Name = "OldPassword",Type="PASSWORD" },


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
                    htRequestParams["Password"] = SharedMethod.Encrypt(htRequestParams["Password"].ToString());
                    htRequestParams["OldPassword"] = SharedMethod.Encrypt(htRequestParams["OldPassword"].ToString());

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_ChangePassword_Update]");
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
        [Route("/ACC/InsertNotification")]
        public ActionResult InsertNotification(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "SenderID",Type="INT" },
                  new APIParameterModel {Name = "Type",Type="INT" },
                  new APIParameterModel {Name = "NotificationMessage",Type="STRING" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Notification_Insert]");
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
        [Route("/ACC/DeleteNotification")]
        public ActionResult DeleteNotification(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "ID",Type="INT" },
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Notification_Delete]");
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
        [Route("/ACC/UpdateNotification")]
        public ActionResult UpdateNotification(Hashtable htRequestParams)
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Notification_Update]");
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
        [Route("/ACC/UpdateEmailVerificationStatus")]
        public ActionResult UpdateEmailVerificationStatus(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="string" },
                };

                var obj = SharedMethod.ValidateParameters(htRequestParams, lstParameters);
                if (Convert.ToInt32(obj.status) != 200)
                {
                    return BadRequest(obj);
                }
                

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_EmailVerificationStatus_Update]");
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
                        return BadRequest(new { status = 400, message = "Email Verification Failed" });
                    }
                
            }
            catch (Exception e)
            {
                SharedMethod.LogException(e);
                return BadRequest(new { status = 400, message = e.Message });
            }
        }


        [HttpPost]
        [Route("/ACC/SendVerificationEmail")]
        public ActionResult SendVerificationEmail(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "EmailID",Type="EMAIL" },
                  new APIParameterModel {Name = "FirstName",Type="STRING" },
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_SendVerificationEmail_Query]");
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
        [Route("/ACC/GetNotification")]
        public ActionResult GetNotification(Hashtable htRequestParams)
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Notification_Query]");
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
                            DataTable notifications = ds.Tables[0];

                            if (!string.IsNullOrEmpty(notifications.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(notifications.Rows[0][0].ToString()!));
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
        [Route("/ACC/ValidateSession")]
        public ActionResult ValidateSession(Hashtable htRequestParams)
        {
            try
            {
                
               
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
                        return Ok(new { status = 200, message = "Authorization Success",emailverification = result.emailverification });
                    
                }
            }
            catch (Exception e)
            {
                SharedMethod.LogException(e);
                return BadRequest(new { status = 400, message = e.Message });
            }
        }

        [HttpPost]
        [Route("/ACC/InsertIssue")]
        public ActionResult InsertIssue(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "CategoryID",Type="INT" },
                  new APIParameterModel {Name = "Name",Type="STRING" },
                  new APIParameterModel {Name = "Subject",Type="STRING" },
                  new APIParameterModel {Name = "Description",Type="STRING" },
                  new APIParameterModel {Name = "PhoneNo",Type="STRING" },
                  new APIParameterModel {Name = "Email",Type="EMAIL" },

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


                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Issue_Insert]");
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
        [Route("/ACC/GetSupportData")]
        public ActionResult GetSupportData(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_SupportData_Query]");
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

        [HttpPost]
        [Route("/ACC/InsertMessage")]
        public ActionResult InsertMessage(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "ReceiverID",Type="INT" },
                  new APIParameterModel {Name = "SenderID",Type="INT" },
                  new APIParameterModel {Name = "CreatedDate",Type="DATETIME" },
                  new APIParameterModel {Name = "MessageText",Type="STRING" },

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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_Message_Insert]");
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
        [Route("/ACC/GetUserChat")]
        public ActionResult GetUserChat(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                                    new APIParameterModel {Name = "ChatUserID",Type="INT" },

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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_UserChat_Query]");
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
                            DataTable chat = ds.Tables[0];

                            if (!string.IsNullOrEmpty(chat.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(chat.Rows[0][0].ToString()!));
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
        [Route("/ACC/GetConnectedUser")]
        public ActionResult GetConnectedUser(Hashtable htRequestParams)
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
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_ConnectedUser_Query]");
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
                            DataTable connecteduser = ds.Tables[0];

                            if (!string.IsNullOrEmpty(connecteduser.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(connecteduser.Rows[0][0].ToString()!));
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
