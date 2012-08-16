using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.UserControls
{

    public partial class StaffListForBooking : StaffListControlBase
    {

        public enum eBookingMode
        {
            Book,
            Prebook
        }

        public eBookingMode BookingMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowStaff(pnlSelectStaff, dlStaff, GetPreselectedUsername(Page));
                if (!String.IsNullOrEmpty(dlStaff.SelectedValue))
                    OnSelectStaff(dlStaff);
            }
        }

        protected void RemoveStaffWithNoPendingApprovals(List<Staff> staffList)
        {
            if (!string.IsNullOrEmpty(GetPreselectedUsername(Page)))
            {
                chkHide.Visible = false;
                return;
            }

            if (chkHide.Checked)
            {
                staffList.RemoveAll(delegate(Staff staff)
                {
                    bool hasPending = false;
                    const int stage = 1; // submitted by staff but awaiting approval by supervisor
                    if (BookingMode == eBookingMode.Book)
                    {
                        List<BookedEvent> events = BookedEvent.GetAll(staff.Username, stage);
                        hasPending = events.Count > 0;
                    }
                    else
                    {
                        const string suggestedEventQueryKey = "SE";
                        bool isAdHoc = Page.Request.QueryString[suggestedEventQueryKey] == null;
                        List<PrebookedEvent> events = PrebookedEvent.GetAll(staff.Username, stage, isAdHoc);
                        hasPending = events.Count > 0;
                    }
                    return !hasPending;
                });
            }
        }

        protected override void ShowStaffUnderSupevisor(DropDownList dlStaff, string preselectedUsername)
        {
            // show staff list
            List<Staff> staffList = Staff.GetStaffUnder(CurrentStaff.Username, true);

            RemoveStaffWithNoPendingApprovals(staffList);

            // add himself
            staffList.Insert(0, CurrentStaff);

            dlStaff.DataSource = staffList;
            dlStaff.DataBind();

            PreselectStaff(dlStaff, preselectedUsername);
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            // raise an event with the StaffUsername as the parameter
            OnSelectStaff(dlStaff);
        }

        protected void chkHide_CheckedChanged(object sender, EventArgs e)
        {
            ShowStaff(pnlSelectStaff, dlStaff, GetPreselectedUsername(Page));
        }





    }
}