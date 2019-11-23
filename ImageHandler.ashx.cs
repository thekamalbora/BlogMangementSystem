using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BlogProject
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        string constring = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        public void ProcessRequest(HttpContext context)
        {

            string BlogID = context.Request.QueryString["BlogID"];
            SqlConnection conn = new SqlConnection(constring);
            conn.Open();
            SqlCommand cmd = new SqlCommand("select BlogImage from BlogMaster where BlogID=" + BlogID, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            context.Response.BinaryWrite((Byte[])dr[0]);
            conn.Close();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}