using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Reports
{
    public partial class PrebookedEvents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uscStaffList.SelectStaff += new StaffListControlBase.SelectStaffHandler(uscStaffList_SelectStaff);
            if (!IsPostBack)
            {
                Staff staff = Staff.GetFromUsername(User.Identity.Name);
            }
        }

        private void ShowReport()
        {
            int beginStage = 1;
            int endStage = 3;

            List<TrainingEvent> prebookedEvents = new List<TrainingEvent>();
            for (int iStage = beginStage; iStage <= endStage; iStage++)
            {
                List<TrainingEvent> prebookedEventsAdHoc = PrebookedEvent.GetSubmissionList(iStage, uscStaffList.SelectedStaffUsername, true);
                List<TrainingEvent> prebookedEventsFromAppraisal = PrebookedEvent.GetSubmissionList(iStage, uscStaffList.SelectedStaffUsername, false);
                prebookedEvents.AddRange(prebookedEventsAdHoc);
                prebookedEvents.AddRange(prebookedEventsFromAppraisal);
            }

            gvEvents.DataSource = prebookedEvents;
            gvEvents.DataBind();
        }

        void uscStaffList_SelectStaff(object sender, StaffListControlBase.SelectStaffEventArgs args)
        {
            ShowReport();
        }



        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            ShowReport();

        }



    }
}
