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


    public class SelectEventArgs : EventArgs
    {
        public int EventDateId { get; set; }
        public string StaffUsername { get; set; }
    }


    public delegate void SelectEventHandler(object sender, SelectEventArgs args);

    public partial class EventList : System.Web.UI.UserControl
    {
        public string ModuleName { get; set; } // whether CEF/PTA
        public event SelectEventHandler SelectEvent;

        private const string KeyStaffUsername = "StaffUsername";

        public string StaffUsername
        {
            get
            {
                if (ViewState[KeyStaffUsername] == null)
                    return null;
                return ViewState[KeyStaffUsername].ToString();
            }
            set
            {
                ViewState[KeyStaffUsername] = value;
            }
        }

        public void ShowEvents(string username)
        {
           if (string.IsNullOrEmpty(username))
           {
              dlEvents.Items.Clear();
           }

            List<EventDate> eventDates = GetUnsubmittedEvents(username);
            dlEvents.DataSource = eventDates;
            dlEvents.DataBind();
            StaffUsername = username;
        }

        private List<EventDate> GetUnsubmittedEvents(string username)
        {
            List<EventDate> output = new List<EventDate>();
            List<AttendedEvent> attendedEvents = AttendedEvent.GetUnsubmitted(ModuleName, username);

            this.Visible = attendedEvents.Count > 0;
            foreach (AttendedEvent attendedEvent in attendedEvents)
            {
                EventDate ed = EventDate.GetFromId(attendedEvent.EventDateId);
                if (ModuleName.ToLower() == "pta")
                {
                    if (ed.IsDueForPTA())
                        output.Add(ed);
                }
                else
                    output.Add(ed);
            }
            return output;
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            // raise an event with the StaffUsername as the parameter
            OnSelectEvent();
        }

        protected virtual void OnSelectEvent()
        {
            if (SelectEvent != null)
            {
                SelectEventArgs args = new SelectEventArgs();
                args.EventDateId = Convert.ToInt32(dlEvents.SelectedValue);
                args.StaffUsername = StaffUsername;
                SelectEvent(this, args);
            }
        }
    }
}