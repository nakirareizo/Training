using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Reports
{
    public partial class BookedEvents : System.Web.UI.Page
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
            int beginStage = 1;
            int endStage = 3;
            if (vldDates.IsValid)
            {
                DateTime? fromDate = UtilityUI.GetDate(txtFromDate);
                DateTime? toDate = UtilityUI.GetDate(txtToDate);

                List<EventDate> eventDates = new List<EventDate>();
                for (int iStage = beginStage; iStage <= endStage; iStage++)
                {
                    List<EventDate> bookedEventDates = BookedEvent.GetSubmissionList(iStage, uscStaffList.SelectedStaffUsername);
                    foreach (EventDate ed in bookedEventDates)
                    {
                        bool showEventDate = true;
                        if (fromDate.HasValue && ed.StartDate.CompareTo(fromDate.Value) < 0)
                            showEventDate = false;
                        if (toDate.HasValue && ed.EndDate.CompareTo(toDate.Value) > 0)
                            showEventDate = false;
                        if (showEventDate)
                            eventDates.Add(ed);
                    }
                }

                gvEvents.DataSource = eventDates;
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
