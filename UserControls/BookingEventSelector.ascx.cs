using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;
using System.Data;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Configuration;

namespace TrainingRequisition.UserControls
{
    public partial class BookingEventSelector : EventSelectorBase<EventDate>
    {
        private const int requiredGracePeriod = 5; // number of days after today of the earliest event that can be booked


        private new void ShowSelectedEvents(ListBox lbSelected, Questions uscQuestions)
        {
            base.ResetQuestions(uscQuestions);
            base.ShowSelectedEvents(lbSelected);

        }

        /// <summary>
        /// Display available events but filter out events that have been transfered
        /// into selected events.
        /// </summary>
        private int ShowAvailableEvents()
        {
            List<EventDate> filtered = FilterAvailableEvents();
            lbAvailable.DataSource = filtered;
            lbAvailable.DataBind();
            return filtered.Count;
        }



        private void LoadSelectedEvents()
        {
            int stage = GetCurrentStage();

            List<BookedEvent> bookedEvents = BookedEvent.GetAll(CurrentStaffUsername, stage);
            List<EventDate> selectedEvents = new List<EventDate>();
            foreach (BookedEvent bookedEvent in bookedEvents)
            {
                EventDate eventObj = EventDate.GetById(bookedEvent.EventDateId);
                if (eventObj != null)
                    selectedEvents.Add(eventObj);
            }
            SelectedEvents = selectedEvents;
        }

        public void LoadAndShowSelectedEvents(string StaffUsername)
        {

            CurrentStaffUsername = StaffUsername;
            LoadSelectedEvents();
            ShowSelectedEvents(lbSelected, uscQuestions);
            lblMessage.Text = "";

            ShowSelectedEventLabel(lblSelectedEvents, StaffUsername);
        }
        protected void ShowSelectedEventLabel(Label lblSelectedEvents, string currentStaffUsername)
        {
            if (currentStaffUsername.ToUpper() == Page.User.Identity.Name.ToUpper())
                lblSelectedEvents.Text = "Select Training to proceed book";
            else
                lblSelectedEvents.Text = "Select Training to approve book";
        }

        protected void btnAcceptAll_Click(object sender, EventArgs e)
        {
            SelectAllAvailableEvents();
            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
            ShowAvailableEventDetails();
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            SelectAvailableEventsFromListbox(lbAvailable);
            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
            ShowAvailableEventDetails();
        }

        protected void btnUnselectAll_Click(object sender, EventArgs e)
        {
            UnselectAllEvents();
            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
            ShowSelectedEventDetails();
        }

        protected void btnUnselect_Click(object sender, EventArgs e)
        {
            UnselectEventsInListbox(lbSelected);

            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected, uscQuestions);
            ShowSelectedEventDetails();

        }



        public void ShowSelectedEvents()
        {
            LoadSelectedEvents();
            ShowSelectedEvents(lbSelected);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            uscQuestions.OnApply += new Questions.ApplyHandler(uscQuestions_OnApply);
            uscSelectedEventDetails.AddEvent += new EventDetails.AddEventHandler(uscSelectedEventDetails_AddEvent);
            uscQuestions.OnFailedValidation += new Questions.FailedValidationHandler(uscQuestions_OnFailedValidation);
            uscSelectedEventDetails.FailedValidation += new EventDetails.FailedValidatonHandler(uscSelectedEventDetails_FailedValidation);
            if (!IsPostBack)
            {
                ShowEventGroups();
                ResetFromToDates();
            }
        }

        void uscSelectedEventDetails_FailedValidation(object sender, EventArgs args)
        {
            uscSelectedEventDetails.Visible = true;
        }

        void uscQuestions_OnFailedValidation(object sender, EventArgs args)
        {
            uscQuestions.Visible = true;
        }

        void uscSelectedEventDetails_AddEvent(object sender, EventDetails.EventDetailsArgs args)
        {
            TrainingEvent addedEvent = args.CurrentEvent;
            EventDate eventDate = args.CurrentEventDate;

            bool isNewSuggestion = eventDate.IsNew && eventDate.TemporaryEvent == null;

            addedEvent.EventDates.Add(eventDate);
            eventDate.TemporaryEvent = addedEvent;

            if (isNewSuggestion)
            {
                // add the new event
                // assign a temporary ID to it first so the listbox won't get confused later
                eventDate.Id = Int32.MaxValue - SelectedEvents.Count;
                SelectedEvents.Add(eventDate);
            }
            else
            {
                // update the current event
                EventDate existingDate = GetEventFromList(eventDate.Id, SelectedEvents);
                existingDate.Copy(eventDate);
                existingDate.TemporaryEvent = addedEvent;
                existingDate.IsEdited = true;
                existingDate.TemporaryEvent.IsEdited = true;

                // replace it back in the list
                List<EventDate> dates = SelectedEvents;
                for (int i = 0; i < dates.Count; i++)
                {
                    if (dates[i].Id == existingDate.Id)
                        dates[i] = existingDate;
                }
                SelectedEvents = dates;

            }
            ShowSelectedEvents(lbSelected);
            uscSelectedEventDetails.Visible = false;
        }

        private void ResetFromToDates()
        {
            DateTime fromDate = DateTime.Now.AddDays(requiredGracePeriod);
            this.txtFrom_CalendarExtender.SelectedDate = fromDate;
            this.txtTo_CalendarExtender.SelectedDate = fromDate.AddMonths(3).AddDays(-1);
        }

        void uscQuestions_OnApply(object sender, Questions.ApplyEventArgs args)
        {
            lbSelected.SelectedIndex = -1;
            ShowMessage("", lblMessage);
        }

        private void ShowEventGroups()
        {
            List<EventGroup> eventGroups = EventGroup.GetAll();

            // show the tree in the list box
            ShowEventGroups(eventGroups, 0, dlEventGroups);

        }

        protected void btnShowAvailable_Click(object sender, EventArgs e)
        {
            DateTime? fromDate = UtilityUI.GetDate(txtFrom);
            DateTime? toDate = UtilityUI.GetDate(txtTo);
            DateTime today = DateTime.Now.Date;
            //if user not insert date
            if (fromDate == null || toDate == null)
            {
                string sScript0 = "window.alert('Please enter valid dates. You can click on the dates to pick from a calendar.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
                // msg += "Please enter valid dates. You can click on the dates to pick from a calendar.";
            }
            //fromDate less than 5days from today
            if (fromDate.Value.CompareTo(today.AddDays(requiredGracePeriod)) < 0)
            {
                string sScript0 = "window.alert('From date must be at least " + requiredGracePeriod + " days from today.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
                //msg += "From date must be at least " + requiredGracePeriod + " days from today. ";
            }
            //toDate > fromDATE
            if (toDate.Value.CompareTo(fromDate) < 0)
            {
                string sScript0 = "window.alert('To date cannot be earlier than From date.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
                //msg += "To date cannot be earlier than From date. ";
            }
            // special handling for suggested event groups
            EventGroup selectedEventGroup = GetSelectedEventGroup(dlEventGroups);
            List<TrainingEvent> events = TrainingEvent.GetAvailableEventsWithDates(selectedEventGroup, CurrentStaffUsername, txtSearchTitle.Text, fromDate.Value, toDate.Value);

            // traverse through the events and pick up the event dates only
            List<EventDate> eventDates = new List<EventDate>();
            foreach (var trainingEvent in events)
            {
                foreach (var eventDate in trainingEvent.EventDates)
                    eventDates.Add(eventDate);
            }
            AvailableEvents = eventDates;

            int count = ShowAvailableEvents();
            if (count == 0)
            {
                string sScript0 = "window.alert('No Trainings for selected duration.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
            }
        }


        /// <summary>
        /// Submit all selected events at the current approval level
        /// to the next approval level
        /// </summary>
        public bool Submit(ref string errorMessage)
        {
            List<int> selectedEventIDs = new List<int>();
            foreach (ListItem item in lbSelected.Items)
            {
                if (item.Selected)
                    selectedEventIDs.Add(Convert.ToInt32(item.Value));
            }
            // first save any unsaved events
            Save();

            // then update the saved items' approval level to +1
            Staff loggedInUser = GetLoggedInUser();
            string staffUsername = CurrentStaffUsername;

            int currentStage = GetCurrentStage();

            List<EventDate> preSubmissionList = BookedEvent.GetSubmissionList(
                currentStage, staffUsername);
            List<EventDate> submissionList = FilterOnlySelectedEvents(preSubmissionList, selectedEventIDs);
            // check first that all notes and ratings have been entered
            List<EventDate> badEvents =
                ValidateNotesAndRatings(loggedInUser, staffUsername, submissionList);
            if (badEvents.Count > 0)
            {
                if (loggedInUser.Username.ToUpper() == CurrentStaffUsername.ToUpper())
                    errorMessage = "Please complete your justification for ";
                else
                    errorMessage = "Please complete your justification and ratings for ";

                int i = 0;
                foreach (EventDate item in badEvents)
                {
                    if (i > 0)
                        errorMessage += ", ";
                    errorMessage += item.DisplayName;
                    i++;
                }

                errorMessage += " before submitting.";
                ShowMessage(errorMessage, lblMessage);
                return false;
            }


            // compose email
            foreach (EventDate toSubmit in submissionList)
            {
                BookedEvent.IncrementStage(
                    staffUsername,
                    toSubmit.Id,
                    1);
                //On Behalf Mode
                string RequestedUsername = BookedEvent.getRequestedUsername(toSubmit);
                if (Staff.IsSupervisor(loggedInUser.Username.ToString()) && loggedInUser.Username.ToUpper() == RequestedUsername.ToUpper())
                {
                    SendOnBehallfNotificationEmail(toSubmit, Server, staffUsername);
                }
            }

            // if staff mode
            if (loggedInUser.Username.ToUpper() == CurrentStaffUsername.ToUpper())
            {
                SendNotificationEmail(submissionList, Server, staffUsername);
            }
            ShowAvailableEvents();
            LoadAndShowSelectedEvents(CurrentStaffUsername);
            //ShowMessage("The events have been submitted for approval", lblMessage);

            if (currentStage == 0)
            {
                string sScript0 = "window.alert('The booked Training(s) have been submitted for Supervisor approval.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Bbook.aspx", sScript0, true);
                ShowMessage("The booked Training(s) have been submitted for Supervisor approval.", lblMessage);
            }
            if (currentStage == 1)
            {
                string sScript0 = "window.alert('The booked Training(s) have been submitted for HR approval.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Bbook.aspx", sScript0, true);
                ShowMessage("The booked Training(s) have been submitted for HR approval.", lblMessage);

            }
            return true;
        }

        public static void SendNotificationEmail(List<EventDate> submissionList, HttpServerUtility server, string staffUsername)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "BookingApprovalSupervisor.htm");
            string subject = "HR Training:Booked Training Waiting for Your Approval";

            emailTemplateFile = server.MapPath(emailTemplateFile);

            Staff staff = Staff.GetFromUsername(staffUsername);
            Staff supervisor = staff.GetSupervisor();

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td><td>Provider</td><td>Start Date</td><td>End Date</td></tr>";
            foreach (EventDate item in submissionList)
            {
                TrainingEvent ev = TrainingEvent.GetById(item.EventId);

                if (ev != null)
                {
                    strEvents += "<tr>";
                    //string staffNotes = "";
                    //string supervisorNotes = "";
                    //Note.GetNotes(staffUsername, null, item.Id, ref staffNotes, ref supervisorNotes);

                    strEvents += "<td>" + ev.DisplayName +
                        "</td><td>" +
                        item.Provider +
                        "</td><td>" +
                        item.StartDate.ToString("dd/MM/yyyy") +
                        "</td><td>" +
                        item.EndDate.ToString("dd/MM/yyyy") +
                       "</td>";

                    strEvents += "</tr>";

                }
            }
            strEvents += "</table>";

            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements["StaffName"] = staff.Name;
            replacements["Name"] = supervisor.Name;
            replacements["Events"] = strEvents;
            replacements["CurrentDate"] = DateTime.Now.ToString("dd MM yyyy");
            UtilityEmail.Send(supervisor.Email, "", subject, emailTemplateFile, replacements);
        }

        private void SendOnBehallfNotificationEmail(EventDate submission, HttpServerUtility server, string staffUsername)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "OnBehalfBook.htm");
            string subject = "HR Training:Booked Training On Behalf by your Supervisor";

            emailTemplateFile = server.MapPath(emailTemplateFile);

            Staff staff = Staff.GetFromUsername(staffUsername);
            Staff supervisor = staff.GetSupervisor();

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td><td>Provider</td><td>Start Date</td><td>End Date</td></tr>";
            TrainingEvent ev = TrainingEvent.GetById(submission.EventId);

            if (ev != null)
            {
                strEvents += "<tr>";
                //string staffNotes = "";
                //string supervisorNotes = "";
                //Note.GetNotes(staffUsername, null, item.Id, ref staffNotes, ref supervisorNotes);

                strEvents += "<td>" + ev.DisplayName +
                    "</td><td>" +
                    submission.Provider +
                    "</td><td>" +
                    submission.StartDate.ToString("dd/MM/yyyy") +
                    "</td><td>" +
                    submission.EndDate.ToString("dd/MM/yyyy") +
                   "</td>";

                strEvents += "</tr>";

            }

            strEvents += "</table>";

            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements["StaffName"] = staff.Name;
            replacements["SupervisorName"] = supervisor.Name;
            replacements["Events"] = strEvents;
            replacements["CurrentDate"] = DateTime.Now.ToString("dd MM yyyy");
            UtilityEmail.Send(supervisor.Email, "", subject, emailTemplateFile, replacements);
        }

        private int GetCurrentStage()
        {
            int currentStage = 0;
            Staff loggedInUser = GetLoggedInUser();
            if (loggedInUser.Username.ToUpper() != CurrentStaffUsername.ToUpper())
                currentStage = 1;
            return currentStage;
        }

        /// <summary>
        /// Return a list of bad events
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="staffUsername"></param>
        /// <param name="submissionList"></param>
        /// <returns></returns>
        protected List<EventDate> ValidateNotesAndRatings(Staff loggedInUser, string staffUsername, List<EventDate> submissionList)
        {
            List<EventDate> badEvents = new List<EventDate>();
            foreach (EventDate toSubmit in submissionList)
            {
                List<Rating> ratings = Rating.GetSupervisorRatings(staffUsername, null, toSubmit.Id);
                string staffNote = "";
                string supervisorNote = "";
                Note.GetNotes(staffUsername, null, toSubmit.Id, ref staffNote, ref supervisorNote);

                // if the user is approving for other people, he must enter 
                // the supervisor ratings and notes
                if (CurrentStaffUsername.ToUpper() != loggedInUser.Username.ToUpper())
                {
                    if (string.IsNullOrEmpty(supervisorNote) || Rating.HasUnrated(ratings))
                        badEvents.Add(toSubmit);
                }
                else
                {
                    if (string.IsNullOrEmpty(staffNote))
                        badEvents.Add(toSubmit);
                }

            }
            return badEvents;
        }

        public static void SendApproveRejectMail(string staffUsername, HttpServerUtility server, List<EventDate> events, bool approve,
           bool ShowSupervisorJustification)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = "";
            string subject = "";
            if (approve)
            {
                emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "BookingApproved.htm");
                subject = "HR Training:Book Training Approval";
            }
            else
            {
                emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "BookingRejected.htm");
                subject = "HR Training:Book Training Rejection";
            }
            emailTemplateFile = server.MapPath(emailTemplateFile);

            Staff user = Staff.GetFromUsername(staffUsername);

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td><td>Provider</td><td>Start Date</td><td>End Date</td><td>Notes</td></tr>";
            foreach (EventDate item in events)
            {
                TrainingEvent ev = TrainingEvent.GetById(item.EventId);

                if (ev != null)
                {
                    string staffNotes = "";
                    string supervisorNotes = "";
                    Note.GetNotes(staffUsername, null, item.Id, ref staffNotes, ref supervisorNotes);
                    string strRow = "<tr>";
                    strRow += "<td>" + ev.DisplayName + "</td>";
                    strRow += "<td>" + item.Provider + "</td>";
                    strRow += "<td>" + item.StartDate.ToString("dd/MM/yyyy") + "</td>";
                    strRow += "<td>" + item.EndDate.ToString("dd/MM/yyyy") + "</td>";
                    if (!string.IsNullOrEmpty(supervisorNotes))
                        strRow += "<td>" + supervisorNotes + "</td>";
                    else
                        strRow += "<td>N/A</td>";
                    strRow += "</tr>";
                    strEvents += strRow;
                }
            }
            strEvents += "</table>";
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements["Name"] = user.Name;
            replacements["Events"] = strEvents;
            replacements["CurrentDate"] = DateTime.Now.ToString("dd MM yyyy");

            Staff supervisor = user.GetSupervisor();
            string supervisorEmail = "";
            if (supervisor != null)
                supervisorEmail = supervisor.Email;

            UtilityEmail.Send(user.Email, supervisorEmail, subject, emailTemplateFile, replacements);
        }

        /// <summary>
        /// Save the selected events into database
        /// </summary>
        public bool Save()
        {
            Staff loggedInUser = GetLoggedInUser();
            List<EventDate> selectedEvents = SelectedEvents;

            int currentStage = GetCurrentStage();

            // fetch list of existing bookings first
            List<BookedEvent> existing = BookedEvent.GetAll(CurrentStaffUsername, currentStage);

            //// identify which events in the existing database do not exist in the selected list
            //List<BookedEvent> toBeDeletedList = new List<BookedEvent>();
            ////List<EventDate> toBeInsertedList = new List<EventDate>();
            //foreach (BookedEvent dbItem in existing)
            //{
            //    bool exists = false;
            //    foreach (EventDate selected in selectedEvents)
            //    {
            //        if (dbItem.EventDateId == selected.Id)
            //        {
            //            exists = true;
            //            break;
            //        }
            //    }
            //    if (!exists)
            //        toBeDeletedList.Add(dbItem);
            //}

            //// delete the events
            //List<string> lstStaffUsernames = new List<string>();
            //lstStaffUsernames.Add(CurrentStaffUsername);
            //foreach (BookedEvent bookedEvent in toBeDeletedList)
            //{
            //    BookedEvent.Reject(bookedEvent.EventDateId, lstStaffUsernames);
            //}

            //delete existing bookings       
            BookedEvent.DeleteAll(CurrentStaffUsername, currentStage);

            // insert all bookings
            InsertBookedEvents(loggedInUser, selectedEvents, currentStage, existing);

            // go through the deleted bookings and send notification email
            //SendRejectionEmail(toBeDeletedList);

            LoadAndShowSelectedEvents(CurrentStaffUsername);
            ShowMessage("Book Training(s) have been saved.", lblMessage);
            return true;
        }

        public bool Reject(ref string errorMessage)
        {
            Staff loggedInUser = GetLoggedInUser();
            List<EventDate> selectedEvents = SelectedEvents;
            int currentStage = GetCurrentStage();

            // fetch list of existing bookings first
            List<BookedEvent> existing = BookedEvent.GetAll(CurrentStaffUsername, currentStage);
            List<BookedEvent> toBeDeletedList = new List<BookedEvent>();
            foreach (BookedEvent dbItem in existing)
            {
                bool exists = false;
                foreach (EventDate selected in selectedEvents)
                {
                    if (dbItem.EventDateId == selected.Id)
                    {
                        exists = true;
                        toBeDeletedList.Add(dbItem);
                        break;
                    }
                }
            }
            // delete the events
            List<string> lstStaffUsernames = new List<string>();
            lstStaffUsernames.Add(CurrentStaffUsername);
            foreach (BookedEvent bookedEvent in toBeDeletedList)
            {
                BookedEvent.Reject(bookedEvent.EventDateId, lstStaffUsernames);
            }
            // delete existing bookings       
            BookedEvent.DeleteAll(CurrentStaffUsername, currentStage);
            // go through the deleted bookings and send notification email
            SendRejectionEmail(toBeDeletedList);

            LoadAndShowSelectedEvents(CurrentStaffUsername);
            ShowMessage("Book Training(s) have been Rejected.", lblMessage);
            return true;
        }
        private void SendRejectionEmail(List<BookedEvent> toBeDeletedList)
        {
            List<BookedEvent> rejectedBooking = new List<BookedEvent>();
            Staff loggedInUser = GetLoggedInUser();
            // take only events that were originally requested by the staff, 
            // not by the supervisor
            foreach (BookedEvent item in toBeDeletedList)
            {
                if (loggedInUser.Username.ToUpper() != item.StaffUsername.ToUpper())
                {
                    rejectedBooking.Add(item);
                }
            }
            if (rejectedBooking.Count == 0)
            {
                return;
            }

            // convert to events
            List<EventDate> rejectedEvents = new List<EventDate>();
            foreach (BookedEvent notifyEvent in rejectedBooking)
            {
                EventDate eventObj = EventDate.GetById(notifyEvent.EventDateId);
                if (eventObj != null)
                    rejectedEvents.Add(eventObj);
            }

            SendApproveRejectMail(CurrentStaffUsername, Server, rejectedEvents, false, true);

        }

        private void InsertBookedEvents(Staff loggedInUser, List<EventDate> selectedEvents,
            int stage, List<BookedEvent> existing)
        {
            // add back the selected events
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_BookedEvents");
                DataRow row = null;
                foreach (EventDate eventDateObj in selectedEvents)
                {
                    // if this is a newly suggested event, save the event object too
                    if (eventDateObj.IsNew)
                    {
                        eventDateObj.TemporaryEvent.InsertNew("REQ_Events");
                        eventDateObj.EventId = eventDateObj.TemporaryEvent.Id;
                        eventDateObj.InsertNew("REQ_EventDates");
                    }
                    else if (eventDateObj.IsEdited)
                    {
                        eventDateObj.Update("REQ_EventDates");
                        eventDateObj.TemporaryEvent.Update("REQ_Events");
                    }

                    // if the booking already exists, use it
                    BookedEvent pb = BookedEvent.FindByEventDateId(eventDateObj.Id, existing);

                    // create new if necessary
                    if (pb == null)
                    {
                        pb = new BookedEvent();
                        pb.EventId = eventDateObj.EventId;
                        pb.EventDateId = eventDateObj.Id;
                        pb.Stage = stage;
                        pb.RequesterUsername = loggedInUser.Username;
                        pb.StaffUsername = CurrentStaffUsername;
                        pb.RequestDate = DateTime.Now;
                    }

                    row = db.Insert(row);
                    pb.Save(row);
                }

                if (row != null)
                {
                    db.Insert(row);
                }
                db.EndInsert();
            }
        }

        protected void lbSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedEventDetails();
            ShowJustification();
        }

        protected void vldSearch_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string msg = "";
            DateTime? fromDate = UtilityUI.GetDate(txtFrom);
            DateTime? toDate = UtilityUI.GetDate(txtTo);

            if (fromDate == null || toDate == null)
            {
                string sScript0 = "window.alert('Please enter valid dates. You can click on the dates to pick from a calendar.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
                // msg += "Please enter valid dates. You can click on the dates to pick from a calendar.";
            }
            else
            {
                DateTime today = DateTime.Now.Date;
                if (fromDate.Value.CompareTo(today.AddDays(requiredGracePeriod)) < 0)
                {
                    string sScript0 = "window.alert('From date must be at least " + requiredGracePeriod + " days from today.');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                    return;
                    //msg += "From date must be at least " + requiredGracePeriod + " days from today. ";
                }
                if (toDate.Value.CompareTo(fromDate) < 0)
                {
                    string sScript0 = "window.alert('To date cannot be earlier than From date.');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                    return;
                    //msg += "To date cannot be earlier than From date. ";
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                vldSearch.ErrorMessage = msg;
                args.IsValid = false;
            }

        }

        private void ShowJustification()
        {
            string dateId = lbSelected.SelectedValue;
            EventDate date = GetEventFromList(Convert.ToInt32(dateId), SelectedEvents);
            //13/6/2012 added to carry forward justifiication
            EventDate EventDateID = EventDate.GetById(Convert.ToInt32(dateId));
            uscQuestions.LoadAndShowAnswers(CurrentStaffUsername, EventDateID.EventId, Convert.ToInt32(dateId));
            uscQuestions.Visible = true;
        }

        private void ShowAvailableEventDetails()
        {
            uscAvailableEventDetails.Visible = false;
            foreach (ListItem item in lbAvailable.Items)
            {
                if (item.Selected)
                {
                    EventDate selectedEvent = GetEventFromList(Convert.ToInt32(item.Value),
                        AvailableEvents);
                    if (selectedEvent != null)
                    {
                        ShowEventDetails(selectedEvent, uscAvailableEventDetails);
                        uscAvailableEventDetails.Visible = true;
                    }
                    break;
                }
            }
        }

        private void ShowEventDetails(EventDate selectedEvent, EventDetails dest)
        {
            dest.CurrentEventDate = selectedEvent;

            TrainingEvent parentEvent = null;
            bool allowEdit = false;

            // for user-defined events, allow the user to edit details
            if (selectedEvent.TemporaryEvent != null)
            {
                // this is when the event has just been added and not yet saved
                parentEvent = selectedEvent.TemporaryEvent;
                allowEdit = true;
            }
            else
            {
                parentEvent = TrainingEvent.GetById(selectedEvent.EventId);
                if (parentEvent.UserDefined)
                    allowEdit = true;
            }
            dest.CurrentEvent = parentEvent;
            dest.ReadOnly = !allowEdit;
        }

        private void ShowSelectedEventDetails()
        {
            uscSelectedEventDetails.Visible = false;
            foreach (ListItem item in lbSelected.Items)
            {
                if (item.Selected)
                {
                    EventDate selectedEvent = GetEventFromList(Convert.ToInt32(item.Value),
                        SelectedEvents);
                    if (selectedEvent != null)
                    {
                        ShowEventDetails(selectedEvent, uscSelectedEventDetails);
                        uscSelectedEventDetails.Visible = true;
                    }
                    break;
                }
            }
        }

        protected void btnSuggest_Click(object sender, EventArgs e)
        {
            uscSelectedEventDetails.CurrentEvent = null;
            uscSelectedEventDetails.CurrentEventDate = null;
            uscQuestions.Visible = false;
            tcSelectedEvent.ActiveTabIndex = 0;
        }

        protected void lbAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowAvailableEventDetails();
        }
    }
}