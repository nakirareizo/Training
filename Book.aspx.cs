using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition
{
    public partial class Book : System.Web.UI.Page
    {
        private const string QueryKeySuggestedEvents = "SE";

        protected void Page_Load(object sender, EventArgs e)
        {

            //lblPgTitle.Text = Staff.GetPageTitle(Page.User.Identity.Name.ToString(),"BOOK");
            uscStaffList.SelectStaff += new StaffListControlBase.SelectStaffHandler(uscStaffList_SelectStaff);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string Username = "";
            if (Staff.IsSupervisor(Page.User.Identity.Name))
            {
                Username = (uscStaffList.FindControl("dlStaff") as DropDownList).SelectedValue;
                btnReject.Visible = true;
            }
            if (Username.ToUpper() == Page.User.Identity.Name.ToUpper() || Username == "")
            {
                lblPgTitle.Text = "Book Training";
            }
            else if (Username.ToUpper() != Page.User.Identity.Name.ToUpper())
            {
                lblPgTitle.Text = "Book Approval";
            }
        }

        void uscStaffList_SelectStaff(object sender, StaffListControlBase.SelectStaffEventArgs args)
        {
            lblSubmitError.Text = "";
            pnlEvents.Visible = true;
            uscEventSelector.LoadAndShowSelectedEvents(args.StaffUsername);

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (uscStaffList.CurrentStaff.IsSupervisor() == false)
            {
                //ShowError("Your Book Training(s) saved.");
                uscEventSelector.Save();
                string sScript0 = "window.alert('Your Book Training(s) saved.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
            else
            {
                //ShowError("Your changes has been saved.");
                uscEventSelector.Save();
                string sScript0 = "window.alert('Your changes has been saved.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            string errorMessage = "";
            if (!uscEventSelector.Reject(ref errorMessage))
            {
                //ShowError(errorMessage);
                string sScript0 = "window.alert('" + errorMessage + "');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
            else
            {
                uscEventSelector.Reject(ref errorMessage);
                string sScript0 = "window.alert('Training(s) has been rejected.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int SelectedItem = (uscEventSelector.FindControl("lbSelected") as ListBox).Items.Count;
            if (SelectedItem > 0)
            {
                if (!BypassPTACheck())
                {
                    if (HasPendingQuestionnaires(uscEventSelector.CurrentStaffUsername, "PTA"))
                    {
                        //ShowError("Booking is prevented because this staff still has pending Post-Training Assessment.");
                        string sScript0 = "window.alert('Booking Training is prevented because this staff still has pending Post-Training Assessment.');";
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Bbook.aspx", sScript0, true);
                        return;
                    }

                    if (HasPendingQuestionnaires(uscEventSelector.CurrentStaffUsername, "CEF"))
                    {
                        //ShowError("Booking is prevented because this staff still has pending Course Evaluation Forms.");
                        string sScript0 = "window.alert('Booking Training is prevented because this staff still has pending Course Evaluation Forms.');";
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Bbook.aspx", sScript0, true);
                        return;
                    }
                }

                string errorMessage = "";
                if (!uscEventSelector.Submit(ref errorMessage))
                {
                    //ShowError(errorMessage);
                    string sScript0 = "window.alert('" + errorMessage + "');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                    return;
                }
            }
            else
            {
                string sScript0 = "window.alert('Please Select available Training(s) first before submit.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
            }
        }

        private bool BypassPTACheck()
        {
            string value = Utility.GetConfiguration(Utility.REQ_BypassPTACheck, null);
            if (!string.IsNullOrEmpty(value))
            {
                bool output = Convert.ToBoolean(value);
                return output;
            }
            return false;
        }

        private void ShowError(string message)
        {
            lblSubmitError.Visible = !string.IsNullOrEmpty(message);
            lblSubmitError.Text = message;

        }

        private bool HasPendingQuestionnaires(string staffUsername, string moduleName)
        {
            List<AttendedEvent> attendedEvents = AttendedEvent.GetUnsubmitted(moduleName, staffUsername);

            if (moduleName.ToLower() == "cef")
            {
                return attendedEvents.Count > 0; // there is no need to check elapsed days for CEF
            }
            else if (moduleName.ToLower() == "pta")
            {
                // if any of the unsubmitted events are past due, return true.
                foreach (AttendedEvent ae in attendedEvents)
                {
                    EventDate ed = EventDate.GetById(ae.EventDateId);
                    if (ed != null && ed.IsOverdueForPTA())
                        return true;
                }
                return false;
            }

            return false;
        }
    }
}
