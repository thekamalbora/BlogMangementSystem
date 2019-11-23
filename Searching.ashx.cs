using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BlogProject
{
    /// <summary>
    /// Summary description for Searching
    /// </summary>
    public class Searching : IHttpHandler
    {
        string constring = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        public void ProcessRequest(HttpContext context)
        {

            string CatID = context.Request.QueryString["CatID"];
            SqlConnection conn = new SqlConnection(constring);
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from CategoryMaster where CatID=" + CatID, conn);
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