using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.Classes;
using System.Data;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Reports
{
    public partial class AttendedEvents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uscStaffList.SelectStaff += new StaffListControlBase.SelectStaffHandler(uscStaffList_SelectStaff);
        }

        void uscStaffList_SelectStaff(object sender, StaffListControlBase.SelectStaffEventArgs args)
        {
        }
        
        protected void btnClearDates_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
        }

        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            if (vldDates.IsValid)
            {
                DateTime? fromDate = UtilityUI.GetDate(txtFromDate);
                DateTime? toDate = UtilityUI.GetDate(txtToDate);
                List<AttendedEvent> attended = AttendedEvent.GetAll(uscStaffList.SelectedStaffUsername, fromDate, toDate);
                gvEvents.DataSource = attended;
                gvEvents.DataBind();
            }
        }

        protected void vldDates_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string errorMsg = UtilityUI.ValidateFromToDates(txtFromDate, txtToDate, false, false);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                args.IsValid = false;
                vldDates.Text = errorMsg;
            }
        }

        
    }
}
