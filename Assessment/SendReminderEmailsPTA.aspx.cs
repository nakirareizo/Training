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
    public partial class SendReminderEmailsPTA : System.Web.UI.Page
    {
        string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowEventDates();
            }
        }

        private void ShowEventDates()
        {
            Dictionary<int, EventDate> eventDates = new Dictionary<int, EventDate>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS AE WHERE NOT EXISTS "
                    + "( SELECT * FROM ASM_SubmittedPTA AS S WHERE S.EventDateID=AE.EventDateID AND S.StaffUsername=AE.StaffUsername )";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    int eventDateId = Convert.ToInt32(dr["EventDateID"]);
                    if (!eventDates.ContainsKey(eventDateId))
                    {
                        EventDate ed = EventDate.GetFromId(eventDateId);
                        if (ed.IsDueForPTA())
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

        protected void btnSendConfirmAttendance_Click(object sender, EventArgs e)
        {
            SendEmailsPerStaff();
        }

        private void SendEmailsPerStaff()
        {
            // accummulate the attended events without PTA submission
            List<AttendedEvent> allUnsubmittedEvents = new List<AttendedEvent>();
            foreach (ListItem item in lbEventDates.Items)
            {
                // go through the selected event dates and fetch all staff that have not submitted their PTA
                if (item.Selected)
                {
                    int eventDateId = Convert.ToInt32(item.Value);
                    List<AttendedEvent> unsubmittedEvents = AttendedEvent.GetUnsubmitted("PTA", eventDateId);
                    EventDate ed = EventDate.GetFromId(eventDateId);
                    TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
                    if (ed == null || ev == null)
                        continue;

                    foreach (AttendedEvent ae in unsubmittedEvents)
                    {
                        Staff staff = Staff.GetFromUsername(ae.StaffUsername);
                        if (staff == null)
                            continue;
                        Staff supervisor = staff.GetSupervisor();
                        if (supervisor == null)
                            continue;

                        Dictionary<string, string> replacements = 
                            new Dictionary<string, string>();
                        replacements["Name"] = supervisor.Name;
                        replacements["StaffName"] = staff.Name;
                        replacements["EventName"] = ev.Title;
                        replacements["StartDate"] = ed.StartDate.ToString("dd/MM/yyyy");
                        replacements["EndDate"] = ed.EndDate.ToString("dd/MM/yyyy");
                        replacements["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                        replacements["EventID"] = ev.SAPId.ToString();

                        string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "InvitePTAPerStaff.txt");
                        emailTemplateFile = Server.MapPath(emailTemplateFile);
                        UtilityEmail.Send(supervisor.Email,"", "Post-Training Assessment Required", emailTemplateFile, replacements);
                        ShowMessage("Emails have been sent.");
                       
                    }
                    
                }
            }

        }

        //private void SendEmailsConsolidated()
        //{
        //    // accummulate the attended events without PTA submission
        //    List<AttendedEvent> allUnsubmittedEvents = new List<AttendedEvent>();
        //    foreach (ListItem item in lbEventDates.Items)
        //    {
        //        // go through the selected event dates and fetch all staff that have not submitted their PTA
        //        if (item.Selected)
        //        {
        //            int eventDateId = Convert.ToInt32(item.Value);
        //            List<AttendedEvent> unsubmittedEvents = AttendedEvent.GetUnsubmitted("PTA", eventDateId);
        //            allUnsubmittedEvents.AddRange(unsubmittedEvents);
        //        }
        //    }

        //    // extract staff from the list
        //    Dictionary<string, Staff> unsubmittedStaff = new Dictionary<string, Staff>();
        //    foreach (AttendedEvent ae in allUnsubmittedEvents)
        //    {
        //        if (!unsubmittedStaff.ContainsKey(ae.StaffUsername))
        //        {
        //            Staff staff = Staff.GetFromUsername(ae.StaffUsername);
        //            if (staff != null)
        //                unsubmittedStaff[ae.StaffUsername] = staff;
        //        }
        //    }

        //    // aggregate the staff by supervisor
        //    Dictionary<Staff, List<Staff>> supervisors = new Dictionary<Staff, List<Staff>>();
        //    foreach (Staff staff in unsubmittedStaff.Values)
        //    {
        //        Staff supervisor = staff.GetSupervisor();

        //        // is the supervisor already recorded?
        //        Staff foundSupervisor = null;
        //        foreach (Staff find in supervisors.Keys)
        //        {
        //            if (find.Username == supervisor.Username)
        //            {
        //                foundSupervisor = find;
        //                break;
        //            }

        //        }
        //        if (foundSupervisor == null)
        //        {
        //            foundSupervisor = supervisor;
        //            supervisors[foundSupervisor] = new List<Staff>();
        //        }

        //        supervisors[foundSupervisor].Add(staff);
        //    }

        //    // go through each supervisor and send the email
        //    foreach (Staff supervisor in supervisors.Keys)
        //    {
        //        List<Staff> subordinates = supervisors[supervisor];


        //        //// display names of staff
        //        string strStaff = "";
        //        foreach (Staff subordinate in subordinates)
        //            strStaff += "\t" + subordinate.Name + "\r\n";

        //        Dictionary<string, string> replacements = new Dictionary<string, string>();
        //        replacements["Name"] = supervisor.Name;
        //        replacements["Staff"] = strStaff;

        //        string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "InvitePTA.txt");

        //        emailTemplateFile = Server.MapPath(emailTemplateFile);

        //        UtilityEmail.Send(supervisor.Email, "Post-Training Assessment Required", emailTemplateFile, replacements);
        //        ShowMessage("Emails have been sent.");

        //    }
        //}

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
