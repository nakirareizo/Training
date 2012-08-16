using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Assessments.Classes;
using TrainingRequisition.Classes;

namespace TrainingRequisition.Assessments.UserControls
{
    public class SelectStaffEventArgs : EventArgs
    {
        public string StaffUsername { get; set; }
    }


    public delegate void SelectStaffHandler(object sender, SelectStaffEventArgs args);

    public partial class StaffList : System.Web.UI.UserControl
    {
        private const string KeyCurrentStaff = "CurrentStaff";

        public enum eBookingMode
        {
            Book,
            Prebook
        }

        public eBookingMode BookingMode { get; set; }

        bool showSelf = true;
        public bool ShowSelf { get { return showSelf; } set { showSelf = value; } }

        public Staff CurrentStaff
        {
            get
            {
                if (ViewState[KeyCurrentStaff] == null)
                    return null;
                return (Staff)ViewState[KeyCurrentStaff];
            }
            set
            {
                ViewState[KeyCurrentStaff] = value;
            }
        }

        public event SelectStaffHandler SelectStaff;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowStaff();
            }
        }

        public void ShowStaff()
        {
            string username = Page.User.Identity.Name;
            Staff currentStaff = Staff.GetFromUsername(username);
            CurrentStaff = currentStaff;

            bool isSupervisor = currentStaff.IsSupervisor();

            pnlSelectStaff.Visible = isSupervisor;

            if (isSupervisor)
            {
                ShowStaffUnderSupevisor(ShowSelf);
            }
            else
            {
                SelectStaffEventArgs args = new SelectStaffEventArgs();
                args.StaffUsername = currentStaff.Username;
                SelectStaff(this, args);
            }

        }

        private void ShowStaffUnderSupevisor(bool includeSelf)
        {
            // show staff list
            List<Staff> staffList = Staff.GetStaffUnder(CurrentStaff.Username, false);
            //20/6/2012
            //filter out staff with AttendedEvents Only 
            List<Staff> staffListfiltered = new List<Staff>();
            foreach (Staff staff in staffList)
            {
                List<AttendedEvent> AE = AttendedEvent.GetAllByUsername(staff.Username);
                if (AE.Count > 0)
                    staffListfiltered.Add(staff);
            }
            // add himself
            if (includeSelf)
                staffListfiltered.Insert(0, CurrentStaff);

            dlStaff.DataSource = staffListfiltered;
            dlStaff.DataBind();
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            // raise an event with the StaffUsername as the parameter
            OnSelectStaff();
        }

        protected virtual void OnSelectStaff()
        {
            if (SelectStaff != null)
            {
                SelectStaffEventArgs args = new SelectStaffEventArgs();
                args.StaffUsername = dlStaff.SelectedValue;
                SelectStaff(this, args);
            }
        }

        protected void chkHide_CheckedChanged(object sender, EventArgs e)
        {
            ShowStaff();
        }





    }
}