using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CapacReport.Models;
namespace CapacReport.Controllers
{
    public class HomeController : Controller
    {
        string constr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
        SqlDataAdapter msData = new SqlDataAdapter();
        [Obsolete]
        public ActionResult Index()
        {
            DataSet db = new DataSet();
            List<SelectListItem> lsOperationNo = GetOperationNo();
            ViewData["MyData"] = lsOperationNo;


            return View();
        }

        #region getOperationNumberCombobox
        [Obsolete]
        public static List<SelectListItem> GetOperationNo()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "sp_GetOperationsList";
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["OperationDesc"].ToString(),
                            Value = sdr["ID"].ToString(),
                        });
                    }
                }
                con.Close();
            }
            return items;
        }
        #endregion

        #region TraceDataProcedure
        [Obsolete]

        public ActionResult TraceData()
        {
            List<SelectListItem> lsOperationNo = GetOperationNo();
            ViewData["MyData"] = lsOperationNo;

            var operationNo = Request["input_operationNo"];
            var StartDate = Request["input_startDate"];
            var EndDate = Request["input_endDate"];

            Glb.operation = operationNo;
            Glb.startdate = Convert.ToDateTime(StartDate);
            Glb.enddate = Convert.ToDateTime(EndDate);

            string constr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            DataSet db = new DataSet();
            SqlConnection con = new SqlConnection(constr);
            if (ModelState.IsValid)
            {
                try
                {
                    //SqlConnection con = new SqlConnection(constr);
                    SqlCommand cmd = new SqlCommand("sp_OP100DateReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Direction = ParameterDirection.Input;
                    cmd.Parameters["@StartDate"].Value = StartDate;

                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Direction = ParameterDirection.Input;
                    cmd.Parameters["@EndDate"].Value = EndDate;
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(db);

                    return View(db);
                }
                catch (Exception ex)
                {

                    return View(ex.Message);

                }
                finally
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
            ModelState.AddModelError("", "Error");
            return View(db);


            #endregion
        }



    }
}
