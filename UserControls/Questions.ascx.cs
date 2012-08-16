using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition.UserControls
{
    public partial class Questions : System.Web.UI.UserControl
    {
        public class ApplyEventArgs: EventArgs
        {
            public List<Rating> Ratings { get; set; }
            public string StaffNotes { get; set; }
            public string SupervisorNotes { get; set; }
        }


        public delegate void ApplyHandler(object sender, ApplyEventArgs args);
        public event ApplyHandler OnApply;
        public delegate void FailedValidationHandler(object sender, EventArgs args);
        public event FailedValidationHandler OnFailedValidation;

        const int ColumnRatings = 1;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private Staff GetLoggedInUser()
        {
            string username = Page.User.Identity.Name;
            Staff output = Staff.GetFromUsername(username);
            return output;
        }

        public string StaffUsername
        {
            get
            {
                if (ViewState["StaffUsername"] != null)
                    return ViewState["StaffUsername"].ToString();
                return "";
            }
            set
            {
                ViewState["StaffUsername"] = value;
            }
        }

        public int? EventId
        {
            get
            {
                if (ViewState["EventId"] != null)
                    return Convert.ToInt32(ViewState["EventId"]);
                return null;
            }
            set
            {
                ViewState["EventId"] = value;
            }
        }

        public int? EventDateId
        {
            get
            {
                if (ViewState["EventDateId"] != null)
                    return Convert.ToInt32(ViewState["EventDateId"]);
                return null;
            }
            set
            {
                ViewState["EventDateId"] = value;
            }
        }

        public void Reset()
        {
           txtSupervisorNotes.Text = "";
           txtStaffNotes.Text = "";
           txtSupervisorNotes.Enabled = false;
           txtSupervisorNotes.Enabled = false;

           foreach (GridViewRow row in gvQuestions.Rows)
           {
              TableCell cell = row.Cells[ColumnRatings];
              RadioButtonList rblRatings = (RadioButtonList)cell.FindControl("rbRatings");
              rblRatings.SelectedIndex = 0;
              rblRatings.Enabled = false;
           }
        }


        public void LoadAndShowAnswers(string staffUsername, int? eventId, int? eventDateId)
        {

            StaffUsername = staffUsername;
            EventId = eventId;
            EventDateId = eventDateId;

            // try to fetch from database
            List<Rating> ratings = Rating.GetSupervisorRatings(staffUsername, eventId, eventDateId);

            // get notes
            string staffNotes = "";
            string supervisorNotes = "";
            Note.GetNotes(staffUsername, eventId, eventDateId, ref staffNotes, ref supervisorNotes);
            txtStaffNotes.Text = staffNotes;
            txtSupervisorNotes.Text = supervisorNotes;

            // enable/disable based on user privileges
            Staff loggedInUser = GetLoggedInUser();
            bool isSupervisorMode = staffUsername.ToUpper() != loggedInUser.Username.ToUpper() &&
                loggedInUser.IsSupervisor();

            txtStaffNotes.Enabled = !isSupervisorMode;
            txtSupervisorNotes.Enabled = isSupervisorMode;

            // reset all to 0 first
            int index = 0;
            foreach (GridViewRow row in gvQuestions.Rows)
            {
                TableCell cell = row.Cells[ColumnRatings];
                RadioButtonList rblRatings = (RadioButtonList)cell.FindControl("rbRatings");
                rblRatings.SelectedIndex = 0;

                if (ratings.Count > index)
                    rblRatings.SelectedIndex = ratings[index].Value;

                rblRatings.Enabled = isSupervisorMode;

                index++;
            }

            // show the right tab to the user
            if (IsStaffMode())
                tabQuestions.ActiveTabIndex = 0; // staff notes

            if (IsSupervisorMode())
            {
                if (string.IsNullOrEmpty(txtSupervisorNotes.Text))
                    tabQuestions.ActiveTabIndex = 1;// supervisor notes;

                if (Rating.HasUnrated(ratings))
                    tabQuestions.ActiveTabIndex = 2; // supervisor ratings
            }

        }

        public void GetAnswers(ref List<Rating> ratings, ref string staffNotes, ref string supervisorNotes)
        {
            staffNotes = "";
            supervisorNotes = "";

            ratings = new List<Rating>();
            foreach (GridViewRow row in gvQuestions.Rows)
            {
                TableCell cell = row.Cells[ColumnRatings];
                RadioButtonList rblRatings = (RadioButtonList)cell.FindControl("rbRatings");

                Rating rating = new Rating();
                int indexRating = 0;
                Int32.TryParse(rblRatings.SelectedValue, out indexRating);
                rating.Value = indexRating;
                ratings.Add(rating);
            }

            staffNotes = txtStaffNotes.Text;
            supervisorNotes = txtSupervisorNotes.Text;
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (!vldQuestions.IsValid)
                return; 

            List<Rating> ratings = null;
            string staffNotes = "", supervisorNotes = "";
            GetAnswers(ref ratings, ref staffNotes, ref supervisorNotes);
            Rating.SaveSupervisorRatings(ratings, StaffUsername, EventId, EventDateId);
            Note.Save(staffNotes, supervisorNotes, StaffUsername, EventId, EventDateId);
            if (OnApply != null)
            {
                ApplyEventArgs args = new ApplyEventArgs();
                args.Ratings = ratings;
                args.StaffNotes = staffNotes;
                args.SupervisorNotes = supervisorNotes;
                OnApply(this, args);
                string sScript0 = "window.alert('Your Justification/Ratings saved.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Questions.ascx", sScript0, true);
                return;
            }
        }

        protected void vldQuestions_ServerValidate(object source, ServerValidateEventArgs args)
        {
            List<Rating> ratings = null;
            string staffNotes = "";
            string supervisorNotes = "";
            GetAnswers(ref ratings, ref staffNotes, ref supervisorNotes);

            string errorMsg = "";

            if (IsSupervisorMode())
            {
                bool hasUnrated = Rating.HasUnrated(ratings);

                if (string.IsNullOrEmpty(supervisorNotes))
                    errorMsg += "Please enter your justification. ";

                // removed because the user wants to enter justification during rejection
                // and the ratings are irrelevant.
                //if (hasUnrated)
                //    errorMsg += "Please complete your rating. ";

            }

            if (IsStaffMode())
            {
                if (string.IsNullOrEmpty(staffNotes))
                    errorMsg += "Please enter your justification. ";
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                vldQuestions.Text = "<br/>" + errorMsg;
                args.IsValid = false;
                if (OnFailedValidation != null)
                    OnFailedValidation(this, null);
            }

        }



        private bool IsStaffMode()
        {
            Staff user = GetLoggedInUser();
            return user.Username.ToUpper() == StaffUsername.ToUpper();
        }

        private bool IsSupervisorMode()
        {
            Staff user = GetLoggedInUser();
            return user.IsSupervisor() && StaffUsername.ToUpper() != user.Username.ToUpper();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

    }
}
