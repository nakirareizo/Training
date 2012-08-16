using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using System.Configuration;

namespace TrainingRequisition.Assessments
{
    public partial class SendReminderEmailsCEF : System.Web.UI.Page
    {
        string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ShowEventDates();
               //Test("CEF");
               //Test("PTA");
            }
        }

        private void ShowEventDates()
        {
            Dictionary<int, EventDate> eventDates = new Dictionary<int, EventDate>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS AE WHERE NOT EXISTS "
                    + "( SELECT * FROM ASM_SubmittedCEF AS S WHERE S.EventDateID=AE.EventDateID AND S.StaffUsername=AE.StaffUsername )";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    int eventDateId = Convert.ToInt32(dr["EventDateID"]);
                    if (!eventDates.ContainsKey(eventDateId))
                    {
                        EventDate ed = EventDate.GetFromId(eventDateId);
                        eventDates[eventDateId] = ed;
                    }
                }
            }

            List<EventDate> list = new List<EventDate>();
            foreach (int key in eventDates.Keys)
            {
                list.Add(eventDates[key]);
            }
            lbEventDates.DataTextField = "DisplayName";
            lbEventDates.DataValueField = "Id";
            lbEventDates.DataSource = list;
            lbEventDates.DataBind();
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in lbEventDates.Items)
                item.Selected = true;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in lbEventDates.Items)
                item.Selected = false;
        }

        protected void SendCEFPTANotifications(string surveyType)
        {
           // surveyType is "CEF" or "PTA" only.

              // fetch the event dates
           Dictionary<int, EventDate> eventDates = new Dictionary<int, EventDate>();
           using (SqlConnection conn = UtilityDb.GetConnectionESS())
           {
              string sql = "SELECT * FROM REQ_AttendedEvents AS AE WHERE NOT EXISTS "
                  + "( SELECT * FROM ASM_Submitted" + surveyType + " AS S WHERE S.EventDateID=AE.EventDateID AND S.StaffUsername=AE.StaffUsername )";
              SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
              while (dr.Read())
              {
                 int eventDateId = Convert.ToInt32(dr["EventDateID"]);
                 if (!eventDates.ContainsKey(eventDateId))
                 {
                    EventDate ed = EventDate.GetFromId(eventDateId);
                    eventDates[eventDateId] = ed;
                 }
              }
           }

            List<AttendedEvent> allUnsubmittedEvents = new List<AttendedEvent>();
            foreach (EventDate ed in eventDates.Values)
            {
               List<AttendedEvent> unsubmittedEvents = AttendedEvent.GetUnsubmitted(surveyType, ed.Id);
               TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
               if (ed == null || ev == null)
                  continue;

               foreach (AttendedEvent ae in unsubmittedEvents)
               {
                  Staff staff = Staff.GetFromUsername(ae.StaffUsername);
                  if (staff == null)
                     continue;

                  Dictionary<string, string> replacements =
                      new Dictionary<string, string>();
                  replacements["Name"] = staff.Name;
                  replacements["EventName"] = ev.Title;
                  replacements["StartDate"] = ed.StartDate.ToString("dd/MM/yyyy");
                  replacements["EndDate"] = ed.EndDate.ToString("dd/MM/yyyy");
                  replacements["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                  replacements["EventID"] = ev.SAPId.ToString();
                  string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "Invite" + surveyType + ".htm");
                  emailTemplateFile = Server.MapPath(emailTemplateFile);
                  UtilityEmail.Send(staff.Email, "", "Your Evaluation is Required", emailTemplateFile, replacements);
               }
            }
        }

        protected void btnSendConfirmAttendance_Click(object sender, EventArgs e)
        {
            SendEmailsPerStaff();
        }

        private void SendEmailsPerStaff()
        {
            // accummulate the attended events without CEF submission
            List<AttendedEvent> allUnsubmittedEvents = new List<AttendedEvent>();
            foreach (ListItem item in lbEventDates.Items)
            {
                // go through the selected event dates and fetch all staff that have not submitted their PTA
                if (item.Selected)
                {
                    int eventDateId = Convert.ToInt32(item.Value);
                    List<AttendedEvent> unsubmittedEvents = AttendedEvent.GetUnsubmitted("CEF", eventDateId);
                    EventDate ed = EventDate.GetFromId(eventDateId);
                    TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
                    if (ed == null || ev == null)
                        continue;

                    foreach (AttendedEvent ae in unsubmittedEvents)
                    {
                        Staff staff = Staff.GetFromUsername(ae.StaffUsername);
                        if (staff == null)
                            continue;

                        Dictionary<string, string> replacements =
                            new Dictionary<string, string>();
                        replacements["Name"] = staff.Name;
                        replacements["EventName"] = ev.Title;
                        replacements["StartDate"] = ed.StartDate.ToString("dd/MM/yyyy");
                        replacements["EndDate"] = ed.EndDate.ToString("dd/MM/yyyy");
                        replacements["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        replacements["EventID"] = ev.SAPId.ToString();

                        string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "InviteCEFPerStaff.txt");
                        emailTemplateFile = Server.MapPath(emailTemplateFile);
                        UtilityEmail.Send(staff.Email,"", "Course Evaluation Required", emailTemplateFile, replacements);
                        ShowMessage("Emails have been sent.");
                    }

                }
            }

        }


        private void SendEmailsConsolidated()
        {
            // accummulate the attended events without CEF submission
            List<AttendedEvent> allUnsubmittedEvents = new List<AttendedEvent>();
            foreach (ListItem item in lbEventDates.Items)
            {
                // go through the selected event dates and fetch all staff that have not submitted their CEF
                if (item.Selected)
                {
                    int eventDateId = Convert.ToInt32(item.Value);
                    List<AttendedEvent> unsubmittedEvents = AttendedEvent.GetUnsubmitted("CEF", eventDateId);
                    allUnsubmittedEvents.AddRange(unsubmittedEvents);
                }
            }

            // extract staff from the list
            Dictionary<string, Staff> unsubmittedStaff = new Dictionary<string, Staff>();
            foreach (AttendedEvent ae in allUnsubmittedEvents)
            {
                if (!unsubmittedStaff.ContainsKey(ae.StaffUsername))
                {
                    Staff staff = Staff.GetFromUsername(ae.StaffUsername);
                    if (staff != null)
                        unsubmittedStaff[ae.StaffUsername] = staff;
                }
            }

            // go through each staff and send the email
            foreach (Staff staff in unsubmittedStaff.Values)
            {
                // display name of unsubmitted event dates
                string strEvents = "";
                foreach (AttendedEvent ae in allUnsubmittedEvents)
                {
                    if (ae.StaffUsername.ToUpper() == staff.Username.ToUpper())
                        strEvents += "\t" + ae.DisplayName + "\r\n";
                }

                Dictionary<string, string> replacements = new Dictionary<string, string>();
                replacements["Name"] = staff.Name;
                replacements["Events"] = strEvents;

                string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "InviteCEF.txt");

                emailTemplateFile = Server.MapPath(emailTemplateFile);

                UtilityEmail.Send(staff.Email, "", "Course Evaluation Required", emailTemplateFile, replacements);
                ShowMessage("Emails have been sent.");

            }
        }

        private void ShowMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                lblMessage.Visible = false;
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = msg;
            }
        }
    }
}
