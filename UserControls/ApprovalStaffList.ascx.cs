using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using BUILD.Training.ClassLibrary.Custom;

namespace TrainingRequisition.UserControls
{
    public partial class ApprovalStaffList : System.Web.UI.UserControl
    {
        protected const int ColumnUsername = 1;
        protected const int ColumnJustification = 4;
        protected const int ColumnRatings = 5;

        public delegate void ApproveRejectEventHandler();
        public event ApproveRejectEventHandler ApproveRejectEvent;


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public EventDate CurrentEventDate
        {
            get
            {
                return (EventDate)ViewState["EventDate"];
            }
            set
            {
                ViewState["EventDate"] = value;
            }
        }

        internal void LoadAndShowStaff(EventDate eventDate)
        {
            if (eventDate == null)
            {
                gvStaff.DataSource = new List<Staff>();
                gvStaff.DataBind();
                return;
            }

            CurrentEventDate = eventDate;
            int stage = 2;
            List<Staff> lstStaff = BookedEvent.GetAllStaff(eventDate, stage);
            List<Staff> BookedStaff = new List<Staff>();
            if (lstStaff != null)
            {
                // filter the staff to only the admin's subsidiaries
                //List<string> subsidiaries = Staff.GetAdminSubsidiaries(Page.User.Identity.Name);

                foreach (Staff staff in lstStaff)
                {
                    //bool isInSubsidiary = false;
                    //foreach (string subsidiary in subsidiaries)
                    //{
                    //    if (subsidiary == staff.Subsidiary)
                    //    {
                    //        isInSubsidiary = true;
                    //        break;
                    //    }
                    //}
                    //if (isInSubsidiary)
                    BookedStaff.Add(staff);
                }
            }
            gvStaff.DataSource = BookedStaff;
            gvStaff.DataBind();

            // show questions for ratings
            pnlButtons.Visible = true;
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll(true);
        }

        private void SelectAll(bool select)
        {
            foreach (GridViewRow row in gvStaff.Rows)
            {
                CheckBox cb = (CheckBox)row.Cells[0].FindControl("chkSelect");
                cb.Checked = select;
            }

        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            SelectAll(false);
            string sScript0 = "window.alert('Book Training(s) cleared.');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ApprovalStaffList.ascx", sScript0, true);
            return;
        }

        protected List<string> GetSelectedUsernames()
        {
            List<string> output = new List<string>();

            foreach (GridViewRow row in gvStaff.Rows)
            {
                bool selected = IsSelected(row);
                if (selected)
                {
                    string username = gvStaff.DataKeys[row.RowIndex].Values["Username"].ToString();
                    output.Add(username);
                }
            }
            return output;
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            List<string> usernames = GetSelectedUsernames();

            string errorMessage = "";
            foreach (string username in usernames)
            {
                List<EventDate> eventDate = new List<EventDate>();

                // TODO: send to SAP
                string trxId = Request.QueryString["ID"].ToString();
                bool result = SAPHeitechREQ.SubmitBook(username, CurrentEventDate.Id, CurrentEventDate.EventId, ref errorMessage, trxId);
                if (result)
                {
                    // update its status
                    BookedEvent.UpdateSAPStatus(
                        BookedEvent.SAPStatuses.SubmittedOK,
                        username, CurrentEventDate.Id);

                    // increment stage
                    BookedEvent.IncrementStage(username, CurrentEventDate.Id, 1);

                    // add to success list
                    eventDate.Add(CurrentEventDate);
                    string sScript0 = "window.alert('Book Training approved.');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ApprovalStaffList.ascx", sScript0, true);
                }

                // send approval email
                // use back the same method in BookingEventSelector
                if (eventDate.Count > 0)
                    BookingEventSelector.SendApproveRejectMail(username, Server, eventDate, true, false);
            }

            if (!errorMessage.Contains("SUCCESS"))
            {
                string sScript0 = "window.alert('" + errorMessage + ".');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "BookApproval.aspx", sScript0, true);
            }

            if (ApproveRejectEvent != null)
                ApproveRejectEvent();


        }

        private bool IsSelected(GridViewRow row)
        {
            CheckBox cb = (CheckBox)row.Cells[0].FindControl("chkSelect");
            if (cb.Checked)
                return true;
            return false;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            List<string> usernames = GetSelectedUsernames();
            BookedEvent.Reject(CurrentEventDate.Id, usernames);

            // send rejection email
            List<EventDate> eventDate = new List<EventDate>();
            eventDate.Add(CurrentEventDate);

            // use back the same method in BookingEventSelector
            foreach (string username in usernames)
                BookingEventSelector.SendApproveRejectMail(username, Server, eventDate, false, false);

            if (ApproveRejectEvent != null)
                ApproveRejectEvent();
            string sScript0 = "window.alert('Book Training(s) rejected.');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);

        }

        protected void gvStaff_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (CurrentEventDate == null)
                return;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int eventDateId = CurrentEventDate.Id;
                string staffUsername = gvStaff.DataKeys[e.Row.RowIndex].Value.ToString();

                string staffNotes = "";
                string supervisorNotes = "";
                Note.GetNotes(staffUsername, null, eventDateId, ref staffNotes, ref supervisorNotes);

                // show justification
                Label lblJustification = (Label)e.Row.Cells[ColumnJustification].FindControl("lblJustification");
                lblJustification.Text = supervisorNotes;

                // show ratings
                string strRatings = "";
                List<Rating> lstRatings = Rating.GetSupervisorRatings(staffUsername, null, eventDateId);
                int iRating = 1;
                foreach (Rating r in lstRatings)
                {
                    string rating = "";
                    for (int i = 0; i < r.Value; i++)
                        rating += "*";

                    strRatings += iRating.ToString() + "-" + rating + "<br/>";

                    iRating++;
                }
                Label lblRatings = (Label)e.Row.Cells[ColumnRatings].FindControl("lblRatings");
                lblRatings.Text = strRatings;


            }

        }
    }
}