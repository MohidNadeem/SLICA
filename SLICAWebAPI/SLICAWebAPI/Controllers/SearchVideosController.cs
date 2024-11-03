using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SLICAWebAPI.Models;
using SLICAWebAPI.Shared;
using System.Collections;
using System.Data;

namespace SLICAWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchVideosController : Controller
    {

        string filename = "SearchVideosController.cs";

        [HttpPost]
        [Route("/Search/SearchVideos")]
        public ActionResult SearchVideos(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "UserID",Type="INT" },
                  new APIParameterModel {Name = "SearchText",Type="STRING" },

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
                    
                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_SearchVideos]");
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
                            DataTable searchvideos = ds.Tables[0];
                         
                            if (!string.IsNullOrEmpty(searchvideos.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(searchvideos.Rows[0][0].ToString()!));
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
        [Route("/Search/GetVideosData")]
        public async Task<ActionResult> GetVideosData(Hashtable htRequestParams)
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
                    List<string> tempvids = new List<string>();
                    tempvids =  await SharedMethod.GetCommonEnglishWords();
                    Hashtable htParams = new Hashtable();
                    htRequestParams.Add("Name", string.Join(",", tempvids.ToArray()));

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_VideosAvailable]");
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
                            DataTable foryouvideos = ds.Tables[0];
                            DataTable likedvideos = ds.Tables[1];
                            DataTable popularvideos = ds.Tables[2];

                            if (!string.IsNullOrEmpty(foryouvideos.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(foryouvideos.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }
                            if (!string.IsNullOrEmpty(likedvideos.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(likedvideos.Rows[0][0].ToString()!));
                            }
                            else
                            {
                                Data.Add(new JArray());
                            }

                            if (!string.IsNullOrEmpty(popularvideos.Rows[0][0].ToString()))
                            {
                                Data.Add(JArray.Parse(popularvideos.Rows[0][0].ToString()!));
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
        [Route("/Search/UpdateVideoAction")]

        public ActionResult UpdateVideoAction(Hashtable htRequestParams)
        {
            try
            {
                int returnStatus = 400;
                string returnContent = "";
                string returnMessage = "";
                List<APIParameterModel> lstParameters = new List<APIParameterModel>
                {
                  new APIParameterModel {Name = "VideoID",Type="INT" },
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

                    var htResult = Transaction.DataTransaction(htRequestParams, "[SP_VideoAction_Update]");
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

  

