using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition.Reports
{
    public partial class SuggestedEvents : System.Web.UI.Page
    {
        private const int ColumnStaffName = 6;

        public List<int> EventIdsInGrid
        {
            get
            {
                return UtilityUI.GetListFromViewState<int>("EventIds", ViewState);
            }
            set
            {
                ViewState["EventIds"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void gvEvents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int rowIndex = e.Row.RowIndex;
                string strEventDateId = gvEvents.DataKeys[rowIndex].Values["EventDateID"].ToString();
                string strEventId = gvEvents.DataKeys[rowIndex].Values["EventID"].ToString();

                // get the staff username(s) associated with the event/event date
                List<string> staffUsernames = new List<string>();
                using (SqlConnection conn = UtilityDb.GetConnectionESS())
                {
                    string sql;

                    // try to use event date first, if it is not specified then use event
                    if (!string.IsNullOrEmpty(strEventDateId))
                        sql = "SELECT * FROM REQ_BookedEvents WHERE EventDateID=" + strEventDateId;
                    else
                        sql = "SELECT * FROM REQ_PrebookedEvents WHERE EventID=" + strEventId;

                    SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                    while (dr.Read())
                        staffUsernames.Add(dr["StaffUserName"].ToString());
                }

                // at this time we have a list of staff user names for this event/date
                // now get the names from the database
                List<string> staffNames = new List<string>();
                foreach (string username in staffUsernames)
                {
                    Staff staff = Staff.GetFromUsername(username);
                    if (staff == null)
                        continue;

                    staffNames.Add(staff.Name);
                }

                // concantenate the staff names to be displayed
                string strStaffNames = "";
                int iStaff = 0;
                foreach (string staffName in staffUsernames)
                {
                    strStaffNames += staffName;
                    if (iStaff > 0)
                        strStaffNames += "<br/>";
                }

                // show it on the row
                Label lblStaffName = (Label)e.Row.Cells[ColumnStaffName].FindControl("lblStaffName");
                lblStaffName.Text = strStaffNames;

                // add it to the event date IDs list
                List<int> list = EventIdsInGrid;
                list.Add(Convert.ToInt32(strEventId));
                EventIdsInGrid = list;

            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {


        }


        protected void btnExcel_Click(object sender, EventArgs e)
        {
            EventIdsInGrid = new List<int>();
            UtilityUI.ExportGridViewtToExcel(gvEvents, Response);
            UpdateExportStatus();

        }

        private void UpdateExportStatus()
        {
            gvEvents.AllowPaging = false;
            gvEvents.DataBind();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                List<int> eventIds = EventIdsInGrid;
                DateTime now = DateTime.Now;
                foreach (int id in eventIds)
                {
                    string sql = "UPDATE REQ_Events SET ExportedToExcel='" +
                        now.ToString("yyyyMMdd") + "' WHERE ID=" + id.ToString() ;
                    UtilityDb.ExecuteSql(sql, conn);
                }
            }
        }

        protected void chkHideExportedEvents_CheckedChanged(object sender, EventArgs e)
        {
            gvEvents.DataBind();
        }


    }
}
