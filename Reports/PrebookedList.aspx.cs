using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.Classes;

namespace BUILD.Training.Reports
{
    public partial class PrebookedList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uscStaffList.SelectStaff += new StaffListControlBase.SelectStaffHandler(uscStaffList_SelectStaff);
        }
        void uscStaffList_SelectStaff(object sender, StaffListControlBase.SelectStaffEventArgs args)
        {
            List<AttendanceConfirmation> prebooked = AttendanceConfirmation.GetAll(uscStaffList.SelectedStaffUsername, null,false);
            gvEvents.DataSource = prebooked;
            gvEvents.DataBind();
        }
        protected void vldDates_ServerValidate(object source, ServerValidateEventArgs args)
        {

        }
    }
}