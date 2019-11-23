using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Drawing;

namespace BlogProject.Controllers
{
    public class BlogMasterController : Controller
    {
        string BlogID = "";
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString);
        // GET: BlogMaster
        public ActionResult BlogMaster()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult BlogMasterInsertUpdateData(int BlogID, string BlogTitle, string BlogDiscription, string BlogSequence, string SubmissionDate, int CatID, string IsShow, string BlogDetailTittle, string BlogImage, string BlogDetails, string ImageName, string ImageExtension)
        {

            #region Convert ByteImage To Base64 String
            byte[] DocBytes = Convert.FromBase64String(BlogImage);
            MemoryStream MS = new MemoryStream(DocBytes, 0, DocBytes.Length);
            MS.Write(DocBytes, 0, DocBytes.Length);
            #endregion

            #region BASE64 TO STRING and Save Image To Folder
            //Create a Folder in your Root directory on your solution.
            string folderPath = Server.MapPath("~/Uploads/");
            string fileName = ImageName;
            string imagePath = folderPath + fileName;
            // Your base 64 string data
            string base64StringData = BlogImage;
            string cleandata = base64StringData.Replace("data:image/png;base64,", "");
            byte[] data = System.Convert.FromBase64String(cleandata);
            MemoryStream ms = new MemoryStream(data);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            img.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            #endregion

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Result", "");
            Dic.Add("Status", "0");
            Dic.Add("Focus", "");
            try
            {

                if (BlogTitle == "")
                {
                    Dic["Result"] = "Please Enter Blog Title..!";
                    Dic["Focus"] = "txtBlogTitle";
                }

                else
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_BlogMaster_InsertUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BlogID", BlogID);
                    cmd.Parameters.AddWithValue("@BlogTitle", BlogTitle);
                    cmd.Parameters.AddWithValue("@BlogDiscription", BlogDiscription);
                    cmd.Parameters.AddWithValue("@BlogSequence", BlogSequence);
                    cmd.Parameters.AddWithValue("@SubmissionDate", SubmissionDate);
                    cmd.Parameters.AddWithValue("@CatID", CatID);
                    cmd.Parameters.AddWithValue("@IsShow", Convert.ToByte(IsShow));
                    cmd.Parameters.AddWithValue("@BlogDetailTittle", BlogDetailTittle);
                    cmd.Parameters.AddWithValue("@BlogImage", DocBytes);

                    cmd.Parameters.AddWithValue("@BlogDetails", BlogDetails);
                    cmd.Parameters.AddWithValue("@ImageName", ImageName);
                    cmd.Parameters.AddWithValue("@ImageExtension", ImageExtension);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        Dic["Result"] = dt.Rows[0]["Result"].ToString();
                        Dic["Status"] = dt.Rows[0]["Status"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                Dic["Result"] = ex.Message;
            }
            return Json(Dic);
        }

        //[HttpPost]
        //public FileResult DownloadFile(int? BlogID)
        //{
        //    byte[] bytes;

        //    string fileName, contentType;
        //    string constr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            cmd.CommandText = "Select BlogID,BlogImage,ImageName,ImageExtension From BlogMaster where BlogID='"+BlogID+"'";

        //            cmd.Connection = con;
        //            con.Open();
        //            using (SqlDataReader sdr = cmd.ExecuteReader())
        //            {
        //                sdr.Read();
        //                bytes = (byte[])sdr["BlogImage"];
        //                contentType = sdr["ImageExtension"].ToString();
        //                fileName = sdr["ImageName"].ToString();
        //            }
        //            con.Close();
        //        }
        //    }
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.Charset = "";
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.ContentType = contentType;
        //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        //    Response.BinaryWrite(bytes);
        //    Response.Flush();
        //    Response.End();
        //    return File(bytes, contentType, fileName);
        //}

        public JsonResult GetAllBlog()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            Dic.Add("RowCount", "");
            StringBuilder st = new StringBuilder();
            st.Append("<table id='mytable'  style='border:1px solid black; white-space:nowrap'></> ");

            st.Append("<tr style='background:maroon;color:white;text-transform:uppercase'>");

            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_Get", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                st.Append("<td style='font-weight:bold'>Delete</td>");
                st.Append("<td style='font-weight:bold'>Edit</td>");

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    string ColumnName = dt.Columns[i].ColumnName.ToString();

                    st.Append("<th>");
                    st.Append(ColumnName.ToString());
                    st.Append("</th>");


                }


                for (int j = 0; j < dt.Rows.Count; j++)
                {


                    st.Append("<tr>");
                    st.Append("<td><button id='btnDelete' style='background:#381818;color:white;' onclick=\"BlogDataDelete('" + dt.Rows[j]["Blog ID"].ToString() + "')\"><i class='fa fa-trash' aria-hidden='true'></i></button></td>");
                    st.Append("<td><button id='btnEdit' style='background:#29a318;color:white;' onclick=\"BlogEditRecord('" + dt.Rows[j]["Blog ID"].ToString() + "')\"><i class='fa fa-pencil' aria-hidden='true'></i></button></td>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        st.Append("<td scope=col>");

                        st.Append(dt.Rows[j][i].ToString());
                        st.Append("</td>");

                    }
                    st.Append("</tr>");

                }

                st.Append("</table>");
                Dic["Grid"] = st.ToString();
                Dic["RowCount"] = dt.Rows.Count.ToString();
            }


            else
            {
                Dic["Grid"] = "Recod Not Found";
            }
            return Json(Dic);
        }

        [HttpGet]
        public ActionResult GetBlogByCategory()
        {
            string CatID = Request.QueryString["CatID"].ToString();
            ViewBag.CatID = CatID;
            return View();
        }

        [HttpPost]
        public JsonResult GetBlogBySearchFilter(string BlogTitle)
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_BlogBySearchFilter", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BlogTitle", BlogTitle);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append("<div class='content'>");

                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                    sb.Append("</div>");
                }
                else
                {
                    sb.Append("<div class='content'>");
                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                    sb.Append("</div>");
                }
            }
            Dic["Grid"] = sb.ToString();
            return Json(Dic);
        }


        public JsonResult BlogByCategories(string CatID)
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_BlogByCatName", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CatID", CatID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                }
                else
                {
                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                }
            }
            Dic["Grid"] = sb.ToString();
            return Json(Dic);
        }
        [HttpPost]
        public JsonResult BlogDataDelete(string BlogID)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_Delete", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BlogID", BlogID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult BlogEditRecord(string BlogID)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_Edit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BlogID", BlogID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Blog( )
        {
           
            return View();
        }

        public ActionResult BlogDetails()
        {
            BlogID = Request.QueryString["BlogID"].ToString();
            ViewBag.BlogID = BlogID;
            return View();
        }

        //Sending Email
        [HttpPost]
        public ActionResult BlogDetails(string subject, string txtOpinion, string txtName, string txtEmailID, string ddlRating)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("ksbora6241@gmail.com", "ksbora6241");
                    var receiverEmail = new MailAddress(txtEmailID, "email");

                    var password = "******";
                    var sub = BlogID;
                    //var body ="Name:-"+ txtName + ""+"<br/>"+" "+"Opinion:-"+ txtOpinion + " "+"Rating:-"+ ddlRating;

                    StringBuilder Body = new StringBuilder();
                    Body.Append("<span style='font-family:Courier New;font-size:13px'> Thanks You For Your Oppion Your Oppion Is Helpfull For Our Communitiy");
                    Body.Append("<span style='font-family:Courier New;font-size:13px'> Your Valuable Oppion Are Follows Bellow");
                    Body.Append("Oppion: ");
                    Body.Append(txtOpinion.ToString());
                    Body.Append("<br /><br />");
                    Body.Append("<hr/><br />");
                    Body.Append("Rating: ");
                    Body.Append(ddlRating.ToString());
                    Body.Append("<br />");
                    var body = Body.ToString(); ;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View();
        }

        public JsonResult ShowBlogDetails()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {   sb.Append("<div class='content'>");
                    
                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                    sb.Append("</div>");
                }
                else
                {
                    sb.Append("<div class='content'>");
                    sb.Append("<h3 ><a class='blogtitlle' id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>" + dt.Rows[i]["BlogTitle"].ToString() + "</a></h3>");
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6 blogdescription'>");
                    sb.Append("<p>" + dt.Rows[i]["BlogDiscription"].ToString() + "<br><a id=" + dt.Rows[i]["BlogID"].ToString() + " href='/BlogMaster/BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "'>Read More..</a></p>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<ul class='Footer'>");
                    sb.Append("<li class='FooterList'><i class='fa fa-calendar'></i> Posted on " + dt.Rows[i]["NewSubmissionDate"].ToString() + "</li>");
                    sb.Append("<li class='FooterList''><i class='fa Example of folder-open-o fa-folder-open-o'></i>&nbsp;&nbsp;" + dt.Rows[i]["CategoryName"].ToString() + "</li>");
                    sb.Append("<li class='FooterList'><i class='fa fa-comment-o'></i> <a href='BlogDetails?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "#div1' class='sliding-link'>" + dt.Rows[i]["TotalResponse"].ToString() + "&nbsp; Response</a></li>");
                    sb.Append(" </ul>");
                    sb.Append("</div>");
                }
            }
            Dic["Grid"] = sb.ToString();
            return Json(Dic);
        }

        public ActionResult CategoryMaster()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CategoryMasterInsertUpdateData(int CatID, string CategoryName, int Sequence)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Result", "");
            Dic.Add("Status", "0");
            Dic.Add("Focus", "");
            try
            {

                if (CategoryName == "")
                {
                    Dic["Result"] = "Please Enter Blog Title..!";
                    Dic["Focus"] = "txtBlogTitle";
                }

                else
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_InsertUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CatID", CatID);
                    cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
                    cmd.Parameters.AddWithValue("@Sequence", Sequence);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        Dic["Result"] = dt.Rows[0]["Result"].ToString();
                        Dic["Status"] = dt.Rows[0]["Status"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                Dic["Result"] = ex.Message;
            }
            return Json(Dic);
        }

        public JsonResult GetAllCategory()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            Dic.Add("RowCount", "");
            StringBuilder st = new StringBuilder();
            st.Append("<table id='mytable'  style='border:1px solid black; white-space:nowrap'></> ");

            st.Append("<tr style='background:maroon;color:white;text-transform:uppercase'>");

            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                st.Append("<td style='font-weight:bold'>Delete</td>");
                st.Append("<td style='font-weight:bold'>Edit</td>");

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    string ColumnName = dt.Columns[i].ColumnName.ToString();

                    st.Append("<th>");
                    st.Append(ColumnName.ToString());
                    st.Append("</th>");


                }


                for (int j = 0; j < dt.Rows.Count; j++)
                {


                    st.Append("<tr>");
                    st.Append("<td><button id='btnDelete' style='background:#381818;color:white;' onclick=\"CategoryDataDelete(" + dt.Rows[j]["CatID"].ToString() + "," + dt.Rows[j]["Sequence"].ToString() + ")\"><i class='fa fa-trash' aria-hidden='true'></i></button></td>");
                    st.Append("<td><button id='btnEdit' style='background:#29a318;color:white;' onclick=\"CategoryEditRecord('" + dt.Rows[j]["CatID"].ToString() + "')\"><i class='fa fa-pencil' aria-hidden='true'></i></button></td>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        st.Append("<td scope=col>");

                        st.Append(dt.Rows[j][i].ToString());
                        st.Append("</td>");

                    }
                    st.Append("</tr>");

                }

                st.Append("</table>");
                Dic["Grid"] = st.ToString();
                Dic["RowCount"] = dt.Rows.Count.ToString();
            }


            else
            {
                Dic["Grid"] = "Recod Not Found";
            }
            return Json(Dic);
        }

        [HttpPost]
        public JsonResult CategoryDataDelete(string CatID, int Sequence)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_Delete", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CatID", CatID);
            cmd.Parameters.AddWithValue("@Sequence", Sequence);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CategoryEditRecord(string CatID)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_Edit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CatID", CatID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCategoryRecord()
        {

            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowCategories()
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_CategoryMaster_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();
            sb.Append("<h4 class='catheading'><i class='fa fa-list' aria-hidden='true'></i>Categories</h4>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {


                sb.Append("<div class='catagory'>");
                sb.Append("<ul class='catul'>");
                sb.Append("<li class='catlist'><a class='catanchar' href='GetBlogByCategory?CatID=" + dt.Rows[i]["CatID"].ToString() + "'><i class='fa fa-folder' aria-hidden='true'></i>" + dt.Rows[i]["CategoryName"].ToString() + "&nbsp;(" + dt.Rows[i]["CategoryPostCount"].ToString() + ")</a></li>");
                sb.Append("</ul>");
                sb.Append("</div>");


            }


            Dic["Grid"] = sb.ToString();
            return Json(Dic);

        }

        public JsonResult ShowBlogDetailsByBlogID(string BlogID)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_DetailsGetByID", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BlogID", BlogID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder StrBulider = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StrBulider.Append("<h3>" + dt.Rows[i]["BlogTitle"].ToString() + "</h3>");
                StrBulider.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                StrBulider.Append("<br/>");
                StrBulider.Append("<br/>");
                StrBulider.Append("<div class='bloguploadimg'>" + dt.Rows[i]["BlogDetails"].ToString() + "</div>");

            }

            Dic["Grid"] = StrBulider.ToString();
            return Json(Dic);
        }

        public JsonResult ShowBlogDetailsByBlogTitle()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/Searching.ashx?BlogTitle=" + dt.Rows[i]["BlogTitle"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");

                }
                else
                {
                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-md-6'>");
                    sb.Append("<img src='/Searching.ashx?BlogTitle=" + dt.Rows[i]["BlogTitle"].ToString() + "' class='img-fluid blogimg'>");
                    sb.Append("</div>");
                }
            }
            Dic["Grid"] = sb.ToString();
            return Json(Dic);
        }

        public JsonResult ShowBlogDetailsByCategoryName(string CatID)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_DetailsGetByID", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CatID", BlogID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder StrBulider = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StrBulider.Append("<h3>" + dt.Rows[i]["BlogTitle"].ToString() + "</h3>");
                StrBulider.Append("<img src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                StrBulider.Append("<br/>");
                StrBulider.Append("<br/>");
                StrBulider.Append("<div class='bloguploadimg'>" + dt.Rows[i]["BlogDetails"].ToString() + "</div>");

            }

            Dic["Grid"] = StrBulider.ToString();
            return Json(Dic);
        }

        [HttpPost]
        public JsonResult OppinionInsertUpdateData(int OppinionID, string Oppinions, string UserName, string Email, int BlogID)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Result", "");
            Dic.Add("Status", "0");
            Dic.Add("Focus", "");
            try
            {
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Blog_Oppinion_InsertUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OppinionID", OppinionID);
                    cmd.Parameters.AddWithValue("@Oppinions", Oppinions);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@BlogID", BlogID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    con.Close();
                    if (dt.Rows.Count > 0)
                    {
                        Dic["Result"] = dt.Rows[0]["Result"].ToString();
                        Dic["Status"] = dt.Rows[0]["Status"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                Dic["Result"] = ex.Message;
            }
            return Json(Dic);
        }
        [HttpPost]
        public JsonResult GetOpinionRecord()
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            Dic.Add("RowCount", "");
            StringBuilder st = new StringBuilder();
            st.Append("<table id='mytable'  style='border:1px solid black; white-space:nowrap'></> ");

            st.Append("<tr style='background:maroon;color:white;text-transform:uppercase'>");

            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Oppinion_GetALL", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                st.Append("<td style='font-weight:bold'>Delete</td>");
                st.Append("<td style='font-weight:bold'>Edit</td>");

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    string ColumnName = dt.Columns[i].ColumnName.ToString();

                    st.Append("<th>");
                    st.Append(ColumnName.ToString());
                    st.Append("</th>");


                }


                for (int j = 0; j < dt.Rows.Count; j++)
                {


                    st.Append("<tr>");
                    st.Append("<td><button id='btnDelete' style='background:#381818;color:white;' onclick=\"OpinionDataDelete('" + dt.Rows[j]["Oppinion ID"].ToString() + "')\"><i class='fa fa-trash' aria-hidden='true'></i></button></td>");
                    st.Append("<td><button id='btnEdit' style='background:#29a318;color:white;' onclick=\"OpinionEditRecord('" + dt.Rows[j]["Oppinion ID"].ToString() + "')\"><i class='fa fa-pencil' aria-hidden='true'></i></button></td>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        st.Append("<td scope=col>");

                        st.Append(dt.Rows[j][i].ToString());
                        st.Append("</td>");

                    }
                    st.Append("</tr>");

                }

                st.Append("</table>");
                Dic["Grid"] = st.ToString();
                Dic["RowCount"] = dt.Rows.Count.ToString();
            }


            else
            {
                Dic["Grid"] = "Recod Not Found";
            }
            return Json(Dic);
        }

        [HttpPost]
        public JsonResult OpinionEditRecord(string OppinionID)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Oppinio_Edit", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OppinionID", OppinionID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OpinionDataDelete(string OppinionID)
        {
            string data = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Oppinio_Delete", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OppinionID", OppinionID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            data = JsonConvert.SerializeObject(dt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowUserOppinion(string BlogID)
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Blog_Oppinion_Get", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BlogID", BlogID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='card-header font-weight-bold'>" + dt.Rows[0]["TotalResponse"].ToString() + "&nbsp;Response</div>");
            sb.Append("<div class='card-body'>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("<div class='media d-block d-md-flex mt-4'>");
                sb.Append("<img class='d-flex mb-3 mx-auto imgoppiopn' src='/avatar_2x.png' alt ='Generic placeholder image'>");
                sb.Append("<div class='media-body text-center text-md-left ml-md-3 ml-0'>");
                sb.Append("<h5 class='mt-0 font-weight-bold'>" + dt.Rows[i]["UserName"].ToString() + "</h5>");
                sb.Append("<p>" + dt.Rows[i]["Oppinions"].ToString() + "</p>");
                sb.Append("</div>");
                sb.Append("</div>");

            }
            sb.Append("</div>");
            sb.Append("</div>");
            Dic["Grid"] = sb.ToString();
            return Json(Dic);

        }
        public ActionResult OppinionIsVerify()
        {
            return View();
        }

      [HttpPost]
        public JsonResult OppinionIsVerifyByID(string ItemIds)
        {
            string data = "";
            string[] arrayIds = ItemIds.Split(',');
            for (int i = 0; i < arrayIds.Length; i++)
            {
                arrayIds[i] = arrayIds[i].Trim();
                con.Open();
                SqlCommand cmd = new SqlCommand("Sp_Oppinio_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OppinionID", arrayIds[i]);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                data = JsonConvert.SerializeObject(dt);
            }
           
            return Json(data, JsonRequestBehavior.AllowGet);
           
            
        }
        public ActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("ksbora6241@gmail.com", "ksbora6241");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "*******";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }
        [HttpPost]
        public JsonResult GetOpinionRecordIsVerified()
        {

            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            Dic.Add("RowCount", "");
            StringBuilder st = new StringBuilder();
            st.Append("<table id='mytable'  style='border:1px solid black; white-space:nowrap'></> ");

            st.Append("<tr style='background:maroon;color:white;text-transform:uppercase'>");

            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Oppinion_GetALL_IsVerified", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                st.Append("<td style='font-weight:bold'>Delete</td>");
                st.Append("<td style='font-weight:bold'>Edit</td>");

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    string ColumnName = dt.Columns[i].ColumnName.ToString();

                    st.Append("<th>");
                    st.Append(ColumnName.ToString());
                    st.Append("</th>");


                }


                for (int j = 0; j < dt.Rows.Count; j++)
                {


                    st.Append("<tr>");
                    st.Append("<td><button id='btnDelete' style='background:#381818;color:white;' onclick=\"OpinionDataDelete('" + dt.Rows[j]["Oppinion ID"].ToString() + "')\"><i class='fa fa-trash' aria-hidden='true'></i></button></td>");
                    st.Append("<td><button id='btnEdit' style='background:#29a318;color:white;' onclick=\"OpinionEditRecord('" + dt.Rows[j]["Oppinion ID"].ToString() + "')\"><i class='fa fa-pencil' aria-hidden='true'></i></button></td>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        st.Append("<td scope=col>");

                        st.Append(dt.Rows[j][i].ToString());
                        st.Append("</td>");

                    }
                    st.Append("</tr>");

                }

                st.Append("</table>");
                Dic["Grid"] = st.ToString();
                Dic["RowCount"] = dt.Rows.Count.ToString();
            }


            else
            {
                Dic["Grid"] = "Recod Not Found";
            }
            return Json(Dic);
        }

        public JsonResult GetImage()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("Grid", "");
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_BlogMaster_GetALL", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            StringBuilder st = new StringBuilder();
            st.Append("<div><table>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                st.Append("<tr>");
                st.Append("<td>");
                st.Append(dt.Rows[i]["ImageName"].ToString());
                st.Append("</td>");
                st.Append("<td>");
                st.Append("<img style='height:70px;width:80px;' src='/ImageHandler.ashx?BlogID=" + dt.Rows[i]["BlogID"].ToString() + "' class='img-fluid blogimg'>");
                st.Append("<td><input type='button' id='btndelete' value='delete' onclick=\"BlogDataDelete('" + dt.Rows[i]["BlogID"].ToString() + "')\"/></td>");
                st.Append("</td>");
                st.Append("<td><button type='submit' id='btndownload'  name='btndownload'  value=" + dt.Rows[i]["BlogID"].ToString() + "><i class='glyphicon glyphicon-download'></i>&nbsp;Download</button></td>");
                st.Append("</td>");
                st.Append("</tr>");
            }
            st.Append("</div></table>");
            Dic["Grid"] = st.ToString();
            return Json(Dic);
        }
        [HttpPost]
        public void DownloadFile(FormCollection fc)
        {
            //string gg = fc["btndownload"];
            //string[] splitString = gg.Split();
            //string BlogID =Convert.ToString(splitString[1]);
            string BlogID = fc["btndownload"];
            byte[] bytes;
            string fileName, contentType;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "Select BlogID,BlogImage,ImageName,ImageExtension From BlogMaster where BlogID=@BlogID ";//
                cmd.Parameters.AddWithValue("@BlogID", BlogID);
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["BlogImage"];
                    contentType = sdr["ImageExtension"].ToString();
                    fileName = sdr["ImageName"].ToString();
                }
                con.Close();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BufferOutput = true; ;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.End();
        }
    }
}