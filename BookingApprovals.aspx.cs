using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;

namespace TrainingRequisition
{
    public partial class BookingApprovals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uscApprovalEventSelector.SelectEvent += new TrainingRequisition.UserControls.ApprovalEventSelector.SelectEventHandler(uscApprovalEventSelector_SelectEvent);

            uscStaffList.ApproveRejectEvent += new UserControls.ApprovalStaffList.ApproveRejectEventHandler(uscStaffList_ApproveRejectEvent);
        }

        void uscStaffList_ApproveRejectEvent()
        {
           uscApprovalEventSelector.LoadAndShowEvents();
           uscStaffList.LoadAndShowStaff(null);
        }

        void uscApprovalEventSelector_SelectEvent(object sender, TrainingRequisition.UserControls.ApprovalEventSelector.SelectEventArgs args)
        {
            EventDate eventDate = args.SelectedEventDate;
            uscStaffList.LoadAndShowStaff(eventDate);
        }
    }
}
