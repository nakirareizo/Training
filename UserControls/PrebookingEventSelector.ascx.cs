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
using BUILD.Training.ClassLibrary.Custom;

namespace TrainingRequisition.UserControls
{
    public partial class PrebookingEventSelector : EventSelectorBase<TrainingEvent>
    {
        /// <summary>
        /// Name of parameter in URL when this page is called by the assessment module
        /// </summary>
        private const string QueryKeySuggestedEvents = "SE";

        /// <summary>
        /// ID of the "dummy" event group that is created when this page is called by assessment
        /// </summary>
        private const int SuggestedEventGroupId = -1;

        /// <summary>
        /// True for AdHoc (normal) more, false if called by assessment
        /// </summary>
        public bool IsAdHoc
        {
            get
            {
                if (ViewState["IsAdHoc"] == null)
                    return true;
                return (bool)ViewState["IsAdHoc"];
            }
            set
            {
                ViewState["IsAdHoc"] = value;
                // pnlSuggestNewEvent.Visible = !value; // set to invisible until suggested events are implemented
            }
        }

        private void SendNotificationEmail(List<TrainingEvent> submissionList, string staffUsername)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "PrebookingApprovalSupervisor.htm");
            string subject = "HR Training:Prebooked Training Waiting for Your Approval";

            emailTemplateFile = Server.MapPath(emailTemplateFile);

            Staff staff = Staff.GetFromUsername(staffUsername);
            Staff supervisor = staff.GetSupervisor();

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td></tr>";
            foreach (TrainingEvent ev in submissionList)
            {

                if (ev != null)
                {
                    strEvents += "<tr>";
                    strEvents += "<td>" + ev.DisplayName + "</td>";
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

        private void SendOnBehalfNotificationEmail(TrainingEvent submission, string staffUsername)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "OnBehalfPrebook.htm");
            string subject = "HR Training:Prebooked Training On Behalf by your Supervisor";

            emailTemplateFile = Server.MapPath(emailTemplateFile);

            Staff staff = Staff.GetFromUsername(staffUsername);
            Staff supervisor = staff.GetSupervisor();

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td></tr>";
            if (submission != null)
            {
                strEvents += "<tr>";
                strEvents += "<td>" + submission.DisplayName + "</td>";
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

        private void ShowSelectedEvents(ListBox lbSelected)
        {
            base.ShowSelectedEvents(lbSelected);
            ResetQuestions();
        }

        /// <summary>
        /// Display available events but filter out events that have been transfered
        /// into selected events.
        /// </summary>
        private void ShowAvailableEvents()
        {
            EventGroup selectedEventGroup = GetSelectedEventGroup(dlEventGroups);
            if (selectedEventGroup.ID == SuggestedEventGroupId)
                AvailableEvents = GetSuggestedEvents();
            else
                AvailableEvents = TrainingEvent.GetAvailableEvents(selectedEventGroup, CurrentStaffUsername, txtSearchTitle.Text, true);

            List<TrainingEvent> filtered = FilterAvailableEvents();
            lbAvailable.DataSource = filtered;
            lbAvailable.DataBind();
        }


        private void LoadSelectedEvents()
        {
            int stage = GetCurrentStage();

            List<PrebookedEvent> prebookedEvents = PrebookedEvent.GetAll(CurrentStaffUsername, stage, IsAdHoc);
            List<TrainingEvent> selectedEvents = new List<TrainingEvent>();
            foreach (PrebookedEvent prebookedEvent in prebookedEvents)
            {
                TrainingEvent eventObj = TrainingEvent.GetById(prebookedEvent.EventId);
                selectedEvents.Add(eventObj);
            }
            SelectedEvents = selectedEvents;
        }

        public void LoadAndShowSelectedEvents(string StaffUsername)
        {
            CurrentStaffUsername = StaffUsername;
            LoadSelectedEvents();
            ShowSelectedEvents(lbSelected);

            ShowSelectedEventLabel(lblSelectedEvents, StaffUsername);

        }

        protected void ShowSelectedEventLabel(Label lblSelectedEvents, string currentStaffUsername)
        {
            if (currentStaffUsername.ToUpper() == Page.User.Identity.Name.ToUpper())
                lblSelectedEvents.Text = "Select Training to proceed prebook";
            else
                lblSelectedEvents.Text = "Select Training to Approve prebook";
        }

        protected void btnAcceptAll_Click(object sender, EventArgs e)
        {
            SelectAllAvailableEvents();
            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            SelectAvailableEventsFromListbox(lbAvailable);

            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
        }



        protected void btnAddUserDefinedEvent_Click(object sender, EventArgs e)
        {
            SelectUserDefinedEvent();
            ShowSelectedEvents(lbSelected);
        }

        private void SelectUserDefinedEvent()
        {
            TrainingEvent userDefinedEvent = CreateUserDefinedEvent();
            if (userDefinedEvent != null)
            {
                SelectEvent(userDefinedEvent);
                txtEventName.Text = "";
            }
        }
        protected void btnUnselectAll_Click(object sender, EventArgs e)
        {
            UnselectAllEvents();
            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);
        }

        protected void btnUnselect_Click(object sender, EventArgs e)
        {
            UnselectEventsInListbox(lbSelected);

            ShowAvailableEvents();
            ShowSelectedEvents(lbSelected);

        }

        private TrainingEvent CreateUserDefinedEvent()
        {
            string suggestedTitle = txtEventName.Text;
            TrainingEvent newEvent = new TrainingEvent();
            newEvent.Title = suggestedTitle;
            newEvent.UserDefined = true;
            return newEvent;

        }

        public void ShowSelectedEvents()
        {
            LoadSelectedEvents();
            ShowSelectedEvents(lbSelected);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            uscQuestions.OnApply += new Questions.ApplyHandler(uscQuestions_OnApply);
            if (!IsPostBack)
            {
                ShowEventGroups();
            }
        }

        void uscQuestions_OnApply(object sender, Questions.ApplyEventArgs args)
        {
            //lbSelected.SelectedIndex = -1;
            //ResetQuestions();

            ShowMessage("", lblMessage);
        }

        private void ShowEventGroups()
        {
            List<EventGroup> eventGroups = EventGroup.GetAll();

            // if not adhoc mode, add new event group called "Suggested Courses"
            if (!IsAdHoc)
            {
                EventGroup suggested = new EventGroup(0, 0, "");
                suggested.Parent = null;
                suggested.ID = SuggestedEventGroupId;
                suggested.Title = "Suggested Training";
                suggested.Children = new List<EventGroup>();
                eventGroups.Insert(0, suggested);
            }

            // show the tree in the list box
            ShowEventGroups(eventGroups, 0, dlEventGroups);

            if (!IsAdHoc)
            {
                ShowAvailableEvents();
            }

        }

        protected void btnShowAvailable_Click(object sender, EventArgs e)
        {
            // special handling for suggested event groups
            ShowAvailableEvents();
        }

        private List<TrainingEvent> GetSuggestedEvents()
        {
            string strSuggestedEvents = Request.QueryString[QueryKeySuggestedEvents].ToString();
            char[] delimiter = { ',' };
            string[] arSuggestedEvents = strSuggestedEvents.Split(delimiter);
            List<TrainingEvent> suggestedEvents = new List<TrainingEvent>();
            foreach (string strEventId in arSuggestedEvents)
            {
                int eventId = 0;
                if (Int32.TryParse(strEventId, out eventId))
                {
                    TrainingEvent suggestedEvent = TrainingEvent.GetById(eventId);
                    suggestedEvents.Add(suggestedEvent);
                }
            }
            return suggestedEvents;
        }


        /// <summary>
        /// Submit all selected events at the current approval level
        /// to the next approval level
        /// </summary>
        public bool Submit(string trxId)
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

            List<TrainingEvent> preSubmissionList = PrebookedEvent.GetSubmissionList(
                currentStage, staffUsername, IsAdHoc);

            List<TrainingEvent> submissionList = FilterOnlySelectedEvents(preSubmissionList, selectedEventIDs);
            string errorMessage = "";
            // check first that all notes and ratings have been entered
            if (IsAdHoc)
            {
                List<TrainingEvent> badEvents =
                    ValidateNotesAndRatings(loggedInUser, staffUsername, submissionList);
                if (badEvents.Count > 0)
                {
                    if (loggedInUser.Username.ToUpper() == staffUsername.ToUpper())
                    {
                        errorMessage = "Please complete your justification for ";
                    }
                    else
                    {
                        errorMessage = "Please complete your justification and ratings for ";
                    }
                    int i = 0;
                    foreach (TrainingEvent item in badEvents)
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
                if (currentStage == 0)
                {
                    string sScript0 = "window.alert('Your training prebook has been submitted for supervisor approval.');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                }
            }

            bool supervisorMode = loggedInUser.Username.ToUpper() != CurrentStaffUsername.ToUpper();

            //SUPERVISOR MODE==TURE
            if (supervisorMode)
            {
                bool result = SAPHeitechREQ.SubmitPrebook(submissionList, CurrentStaffUsername, ref errorMessage, trxId);
                if (result)
                {
                    string RequestedUsername = "";
                    foreach (TrainingEvent toSubmit in submissionList)
                    {
                        PrebookedEvent.UpdateSAPStatus(
                        PrebookedEvent.SAPStatuses.SubmittedOK,
                        staffUsername,
                        IsAdHoc,
                        toSubmit.Id);

                        PrebookedEvent.IncrementStage(
                           currentStage,
                           staffUsername,
                           IsAdHoc,
                           toSubmit.Id,
                           1);
                        RequestedUsername = PrebookedEvent.GetRequestedUsername(toSubmit);

                        if (loggedInUser.Username.ToUpper() == RequestedUsername.ToUpper())
                            SendOnBehalfNotificationEmail(toSubmit, staffUsername);
                    }

                    string sScript0 = "window.alert('Prebook Training(s)  has been submitted.');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                }
            }
            //STAFF MODE==TRUE
            else
            {
                foreach (TrainingEvent toSubmit in submissionList)
                {
                    PrebookedEvent.IncrementStage(
                       currentStage,
                       staffUsername,
                       IsAdHoc,
                       toSubmit.Id,
                       1);
                }
                SendNotificationEmail(submissionList, staffUsername);

            }

            if (supervisorMode)
            {
                SendApproveRejectMail(submissionList, true);
            }

            ShowAvailableEvents();
            LoadAndShowSelectedEvents(CurrentStaffUsername);
            ShowMessage("", lblMessage);
            return true;
        }

        private int GetCurrentStage()
        {
            int currentStage = 0;
            Staff loggedInUser = GetLoggedInUser();
            if (loggedInUser.Username.ToUpper() != CurrentStaffUsername.ToUpper())
                currentStage = 1;
            return currentStage;
        }

        protected List<TrainingEvent> ValidateNotesAndRatings(Staff loggedInUser, string staffUsername, List<TrainingEvent> submissionList)
        {
            List<TrainingEvent> badEvents = new List<TrainingEvent>();
            foreach (TrainingEvent toSubmit in submissionList)
            {
                List<Rating> ratings = Rating.GetSupervisorRatings(staffUsername, toSubmit.Id, null);
                string staffNote = "";
                string supervisorNote = "";
                Note.GetNotes(staffUsername, toSubmit.Id, null, ref staffNote, ref supervisorNote);

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

        private void SendApproveRejectMail(List<TrainingEvent> events, bool approve)
        {
            string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
            string emailTemplateFile = "";
            string subject = "";
            if (approve)
            {
                emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "PrebookingApproved.htm");
                subject = "HR Training:Prebook Training Approval";
            }
            else
            {
                emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "PrebookingRejected.htm");
                subject = "HR Training:Prebook Training Rejection";
            }
            emailTemplateFile = Server.MapPath(emailTemplateFile);

            Staff user = Staff.GetFromUsername(CurrentStaffUsername);

            string strEvents = "";
            strEvents = "<table border='1' cellpadding='10px'>";
            strEvents += "<tr><td>Title</td><td>Notes</td></tr>";
            foreach (TrainingEvent item in events)
            {
                string staffNotes = "";
                string supervisorNotes = "";

                Note.GetNotes(CurrentStaffUsername, item.Id, null, ref staffNotes, ref supervisorNotes);
                string strRow = "<tr>";

                strRow += "<td>" + item.DisplayName + "</td>";
                if (!string.IsNullOrEmpty(supervisorNotes))
                    strRow += "<td>" + supervisorNotes + "</td>";
                else
                    strRow += "<td>N/A</td>";
                strRow += "</tr>";

                strEvents += strRow;

            }
            strEvents += "</table>";
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements["Name"] = user.Name;
            replacements["Events"] = strEvents;
            replacements["CurrentDate"] = DateTime.Now.ToString("dd MMM yyyy");
            replacements["EndOfYear"] = "31 Dec " + DateTime.Now.Year;
            UtilityEmail.Send(user.Email, "", subject, emailTemplateFile, replacements);
        }

        public bool Save()
        {
            Staff loggedInUser = GetLoggedInUser();
            List<TrainingEvent> selectedEvents = SelectedEvents;

            int currentStage = GetCurrentStage();

            // fetch list of existing prebooks first
            List<PrebookedEvent> existing =
                PrebookedEvent.GetAll(CurrentStaffUsername, currentStage, IsAdHoc);

            //// identify which events in the existing database do not exist in the selected list
            //List<PrebookedEvent> toBeDeletedList = new List<PrebookedEvent>();
            //List<TrainingEvent> toBeInsertedList = new List<TrainingEvent>();
            //foreach (PrebookedEvent dbItem in existing)
            //{
            //    bool exists = false;
            //    foreach (TrainingEvent selected in selectedEvents)
            //    {
            //        if (dbItem.EventId == selected.Id)
            //        {
            //            exists = true;
            //            break;
            //        }
            //    }
            //    if (!exists)
            //        toBeDeletedList.Add(dbItem);
            //}

            // delete existing prebooks       
            PrebookedEvent.DeleteAll(CurrentStaffUsername, currentStage, IsAdHoc);

            // insert all prebooks
            InsertPrebookedEvents(loggedInUser, selectedEvents, currentStage, existing);

            //// go through the deleted prebooks and send notification email
            //SendRejectionEmail(toBeDeletedList);

            LoadAndShowSelectedEvents(CurrentStaffUsername);
            //if (CurrentStaffUsername.ToUpper() != Page.User.Identity.Name.ToUpper() && toBeDeletedList.Count > 0)
            //{
            //    ShowMessage("Prebook Training(s) have been Rejected.", lblMessage);
            //}
            //else
            //{
            ShowMessage(" Prebook Training(s) or changes have been saved.", lblMessage);
            //}
            return true;
        }

        internal bool Reject(ref string errorMessage)
        {
            Staff loggedInUser = GetLoggedInUser();
            List<TrainingEvent> selectedEvents = SelectedEvents;

            int currentStage = GetCurrentStage();

            // fetch list of existing prebooks first
            List<PrebookedEvent> existing =
                PrebookedEvent.GetAll(CurrentStaffUsername, currentStage, IsAdHoc);
            // identify which events in the existing database do not exist in the selected list
            List<PrebookedEvent> toBeDeletedList = new List<PrebookedEvent>();
            foreach (PrebookedEvent dbItem in existing)
            {
                bool exists = false;
                foreach (TrainingEvent selected in selectedEvents)
                {
                    if (dbItem.EventId == selected.Id)
                    {
                        exists = true;
                        toBeDeletedList.Add(dbItem);
                        break;
                    }
                }
            }
            //go through the deleted prebooks and send notification email
            SendRejectionEmail(toBeDeletedList);
            LoadAndShowSelectedEvents(CurrentStaffUsername);
            return true;
        }

        private void SendRejectionEmail(List<PrebookedEvent> toBeDeletedList)
        {
            List<PrebookedEvent> rejectedPrebook = new List<PrebookedEvent>();
            Staff loggedInUser = GetLoggedInUser();
            // take only events that were originally requested by the staff, 
            // not by the supervisor
            foreach (PrebookedEvent item in toBeDeletedList)
            {
                if (loggedInUser.Username.ToUpper() != item.StaffUsername.ToUpper())
                {
                    rejectedPrebook.Add(item);
                }
            }
            if (rejectedPrebook.Count == 0)
            {
                return;
            }
            // convert to events
            List<TrainingEvent> rejectedEvents = new List<TrainingEvent>();
            foreach (PrebookedEvent notifyEvent in rejectedPrebook)
            {
                TrainingEvent eventObj = TrainingEvent.GetById(notifyEvent.EventId);
                if (eventObj != null)
                    rejectedEvents.Add(eventObj);
            }

            SendApproveRejectMail(rejectedEvents, false);

        }

        private void InsertPrebookedEvents(Staff loggedInUser, List<TrainingEvent> selectedEvents,
            int stage, List<PrebookedEvent> existing)
        {
            // add back the selected events
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_PrebookedEvents");
                DataRow row = null;
                foreach (TrainingEvent eventObj in selectedEvents)
                {
                    TrainingEvent selectedEvent = eventObj;

                    // if this is a new eventObj, add a new record
                    if (selectedEvent.IsNew)
                        selectedEvent.InsertNew("REQ_Events");

                    // if the prebook already exists, use it
                    PrebookedEvent pb = PrebookedEvent.FindByEventId(selectedEvent.Id, existing);

                    // if the prebook is new, create new
                    if (pb == null)
                    {
                        pb = new PrebookedEvent();
                        pb.EventId = eventObj.Id;
                        pb.Stage = stage;
                        pb.RequesterUsername = loggedInUser.Username;
                        pb.StaffUsername = CurrentStaffUsername;
                        pb.IsAdHoc = IsAdHoc;
                    }

                    row = db.Insert(row);
                    pb.Save(row);
                }

                if (row != null)
                    db.Insert(row);

                db.EndInsert();
            }
        }

        protected void lbSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowJustification();
        }

        protected void ResetQuestions()
        {

            pnlQuestions.Visible = true;

            if (lbSelected.SelectedItem == null)
                pnlQuestions.Visible = false;

            // if more than one selected, do not show questions
            int count = 0;
            foreach (ListItem item in lbSelected.Items)
            {
                if (item.Selected)
                    count++;
            }
            if (count > 1)
            {
                pnlQuestions.Visible = false;
            }

        }

        protected void btnJustify_Click(object sender, EventArgs e)
        {
            ShowJustification();
        }

        private void ShowJustification()
        {
            if (!IsAdHoc)
                return;

            if (lbSelected.SelectedItem == null)
                return;

            string eventId = lbSelected.SelectedValue;
            TrainingEvent eventObj = GetEventFromList(Convert.ToInt32(eventId), SelectedEvents);
            if (eventObj == null)
                return;

            ResetQuestions();

            if (pnlQuestions.Visible)
                uscQuestions.LoadAndShowAnswers(CurrentStaffUsername, Convert.ToInt32(eventId), null);
        }
    }
}