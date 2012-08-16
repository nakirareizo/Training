using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using BUILD.Training.Classes;
using System.Configuration;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;

namespace BUILD.Training
{
    public partial class Utility : System.Web.UI.Page
    {
        #region "Global Declarartion"
        MainLib.core oCore = new MainLib.core();
        MainLib.sobject oSAP = new MainLib.sobject();
        string EmailTemplateFolder = ConfigurationManager.AppSettings["EmailTemplateFolder"].ToString();
        public List<Utilities> lstCEF
        {
            get
            {
                return (List<Utilities>)ViewState["lstCEF"];
            }
            set
            {
                ViewState["lstCEF"] = value;
            }
        }
        public List<Utilities> lstPTA
        {
            get
            {
                return (List<Utilities>)ViewState["lstPTA"];
            }
            set
            {
                ViewState["lstPTA"] = value;
            }
        }
        public List<Utilities> lstATC
        {
            get
            {
                return (List<Utilities>)ViewState["lstATC"];
            }
            set
            {
                ViewState["lstATC"] = value;
            }
        }
        public List<Listing> lstUnApproved
        {
            get
            {
                return (List<Listing>)ViewState["lstUnApproved"];
            }
            set
            {
                ViewState["lstUnApproved"] = value;
            }
        }
        #endregion
        #region "Object Functions"
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void lstbxSelectedSuper_SelectedIndexChanged(object sender, EventArgs e)
        {
            tdWorkflowDetail.Visible = true;
            divWorkflowDetail.Visible = true;
            string SelectedSupervisor = lstbxSelectedSuper.SelectedValue.ToString();
            string Total = "";
            List<Listing> ListedApps = lstUnApproved;
            List<Listing> lstResult = new List<Listing>();
            Listing.FilterAppsbySupervisor(ref lstResult, ListedApps, SelectedSupervisor, ref Total);
            gvStaffAppList.DataSource = lstResult;
            gvStaffAppList.DataBind();
            ShowWorkflowDetail(SelectedSupervisor, Total);
        }
        protected void tcTabs_ActiveTabChanged(object sender, EventArgs e)
        {
            switch (tcTabs.ActiveTabIndex)
            {
                case 0:
                case 1:
                    trSendReminder.Visible = true;
                    divSearch.Visible = true;
                    trShowAll.Visible = true;
                    break;
                default:
                    trSendReminder.Visible = false;
                    divSearch.Visible = false;
                    trShowAll.Visible = false;
                    if (tcTabs.ActiveTabIndex == 2)
                    {
                        divSearch.Visible = true;
                        trShowAll.Visible = true;
                    }
                    break;
            }
        }
        #region "Buttons"
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<string> SearchTerms = getSearchTerm();
            switch (tcTabs.ActiveTabIndex)
            {
                case 0:
                    DoCEFSeacrh(SearchTerms);
                    break;
                case 1:
                    DoPTASearch(SearchTerms);
                    break;
                case 2:
                    DoATCSearch(SearchTerms);
                    break;
                default:
                    string sScript0 = "window.alert('Search engine only can be use for CEF,POST and Confirmation on Trainings date only. Please select correct Tab.')";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
                    break;
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearchMultiple.Text = "";

        }
        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            switch (tcTabs.ActiveTabIndex)
            {
                case 0:
                    ShowAllCEF();
                    break;
                case 1:
                    ShowAllPTA();
                    break;
                case 2:
                    ShowAllATC();
                    break;
                default:
                    string sScript0 = "window.alert('Please Show All next drop down list for pending approval tab.')";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
                    break;
            }
        }
        protected void btnReminderAll_Click(object sender, EventArgs e)
        {
            string surveyType = ddlSurveyType.SelectedItem.ToString();
            DoReminderSurvey(surveyType);
        }
        protected void btnShowAllSuper_Click(object sender, EventArgs e)
        {
            getListUncompletedSupervisor();
        }
        protected void btnUncompleted_Click(object sender, EventArgs e)
        {
            bool noneSelected = true;
            List<Staff> selectedstaff = new List<Staff>();
            foreach (ListItem item in lstSupervisors.Items)
            {
                if (item.Selected)
                {
                    Staff staff = Staff.GetFromUsername(item.Value);
                    selectedstaff.Add(staff);
                    noneSelected = false;
                }
            }
            foreach (Staff staff in selectedstaff)
            {

            }
            if (noneSelected)
            {
                string sScript0 = "window.alert('Please select a user.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ibtnAttachUsers_Click_Script_10", sScript0, true);
                return;
            }
        }
        protected void btnShowPendingApp_Click(object sender, EventArgs e)
        {
            Listing.TrainingType SelectedType = (Listing.TrainingType)Enum.Parse(typeof(Listing.TrainingType), ddlTrainingType.SelectedValue);
            bool isBack = false;
            ShowHideDiv(isBack);
            bool noneSelected = true;
            List<Staff> selectedsupervisorlst = new List<Staff>();
            //get all selectedsupervisor in listbox
            foreach (ListItem item in lstSupervisors.Items)
            {
                if (item.Selected)
                {
                    Staff staff = Staff.GetFromUsername(item.Value);
                    selectedsupervisorlst.Add(staff);
                    noneSelected = false;
                }
            }
            //if no supervisor being selected, pop-up msg
            if (selectedsupervisorlst.Count == 0)
            {
                string sScript0 = "window.alert('Please select supervisor(s) in List Box.')";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
                return;
            }
            //foreach supervisor selected find unapproved training apps which stage=1
            List<Listing> StaffTrainingsLst = new List<Listing>();
            if (selectedsupervisorlst.Count > 0)
                foreach (Staff supervisor in selectedsupervisorlst)
                {
                    List<Staff> stafflst = Staff.GetStaffUnder(supervisor.Username, false);
                    if (stafflst.Count > 0)
                        foreach (Staff staff in stafflst)
                        {
                            int stage = 1;
                            Listing.getAllbyTypeUsernameandStage(SelectedType, staff.Username, stage, ref StaffTrainingsLst);
                        }
                }
            //Bind to girdview
            gvStaffAppList.DataSource = StaffTrainingsLst;
            gvStaffAppList.DataBind();
            ViewState["lstUnApproved"] = StaffTrainingsLst;
            this.lblTotListedAppVal.Text = StaffTrainingsLst.Count.ToString();
            //bind selected supervisors to listbox
            lstbxSelectedSuper.DataSource = selectedsupervisorlst;
            lstbxSelectedSuper.DataTextField = "Name";
            lstbxSelectedSuper.DataValueField = "Name";
            lstbxSelectedSuper.DataBind();
            if (selectedsupervisorlst.Count == 1)
            {
                tdWorkflowDetail.Visible = true;
                divWorkflowDetail.Visible = true;
                foreach (Staff staff in selectedsupervisorlst)
                {
                    ShowWorkflowDetail(staff.Name, StaffTrainingsLst.Count.ToString());
                }
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            bool isBack = true;
            ShowHideDiv(isBack);
        }
        #endregion
        #region "GV"
        #region "gvCEF"
        protected void gvCEF_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandSource.GetType().Name.ToString().ToUpper() == "LINKBUTTON")
                {
                    if (((LinkButton)e.CommandSource).ID.ToUpper() == "LBTNNOTIFY")
                    {
                        try
                        {
                            int index = Convert.ToInt32(e.CommandArgument);
                            int selectedID = Convert.ToInt32(gvCEF.DataKeys[index].Value.ToString());
                            GridViewRow row = this.gvCEF.Rows[index];
                            string SelectedUsername = row.Cells[1].Text.ToString();
                            string surveyType = "CEF";
                            DoSurveyReminderByStaff(selectedID, SelectedUsername, surveyType);
                        }
                        catch (Exception ex)
                        {
                            oCore.LogEvent("Utility.aspx", "gvCEF_RowCommand", ex.Message, "1");
                        }
                        finally
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentClaim.aspx", "gvList_RowCommand", ex.Message, "1");
            }
        }
        protected void gvCEF_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    #region " Retrieve the LinkButton control from the first column. "
                    LinkButton lbtnNotify = (LinkButton)e.Row.Cells[6].Controls[1];
                    #endregion
                    #region " Set the LinkButton's CommandArgument property with the row's index. "
                    lbtnNotify.CommandArgument = e.Row.RowIndex.ToString();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentSS.aspx", "gvList_RowCreated", ex.Message, "1");
            }
        }
        protected void gvCEF_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCEF.PageIndex = e.NewPageIndex;
            this.gvCEF.DataSource = lstCEF;
            this.gvCEF.DataBind();
            gvCEF.SelectedIndex = -1;
        }
        #endregion
        #region "gvPTA"
        protected void gvPTA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandSource.GetType().Name.ToString().ToUpper() == "LINKBUTTON")
                {
                    if (((LinkButton)e.CommandSource).ID.ToUpper() == "LBTNNOTIFY")
                    {
                        try
                        {
                            int index = Convert.ToInt32(e.CommandArgument);
                            int selectedID = Convert.ToInt32(gvCEF.DataKeys[index].Value.ToString());
                            GridViewRow row = this.gvCEF.Rows[index];
                            string SelectedUsername = row.Cells[1].Text.ToString();
                            string surveyType = "PTA";
                            DoSurveyReminderByStaff(selectedID, SelectedUsername, surveyType);

                        }
                        catch (Exception ex)
                        {
                            oCore.LogEvent("Utility.aspx", "gvPTA_RowCommand", ex.Message, "1");
                        }
                        finally
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentClaim.aspx", "gvList_RowCommand", ex.Message, "1");
            }
        }
        protected void gvPTA_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    #region " Retrieve the LinkButton control from the first column. "
                    LinkButton lbtnNotify = (LinkButton)e.Row.Cells[6].Controls[1];
                    #endregion
                    #region " Set the LinkButton's CommandArgument property with the row's index. "
                    lbtnNotify.CommandArgument = e.Row.RowIndex.ToString();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentSS.aspx", "gvList_RowCreated", ex.Message, "1");
            }
        }
        protected void gvPTA_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPTA.PageIndex = e.NewPageIndex;
            this.gvPTA.DataSource = lstPTA;
            this.gvPTA.DataBind();
            gvPTA.SelectedIndex = -1;
        }
        #endregion
        #region "gvATC"
        protected void gvATC_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCEF.PageIndex = e.NewPageIndex;
            this.gvCEF.DataSource = lstATC;
            this.gvCEF.DataBind();
            gvCEF.SelectedIndex = -1;
        }
        protected void gvATC_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUsername = gvATC.SelectedDataKey.Value.ToString();
            List<AttendanceConfirmation> allConfirmations = AttendanceConfirmation.GetAll(selectedUsername, null, true);

            // list out the staff
            List<Staff> lstStaff = new List<Staff>();
            foreach (AttendanceConfirmation ac in allConfirmations)
            {
                Staff newStaff = Staff.GetFromUsername(ac.StaffUsername);
                lstStaff.Add(newStaff);
            }
            SendEmailsConfirmAttendance(allConfirmations, lstStaff);
            string sScript0 = "window.alert('Email Notification has been sent to selected staff.')";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
            return;
        }
        #endregion
        #region "gvStaffAppList"
        protected void gvStaffAppList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCEF.PageIndex = e.NewPageIndex;
            this.gvCEF.DataSource = lstUnApproved;
            this.gvCEF.DataBind();
            gvCEF.SelectedIndex = -1;
        }
        #endregion
        #endregion
        #endregion
        #region "Custom"
        private bool CheckEmptyTextBox()
        {
            bool Empty = false;
            if (txtSearch.Text == "" && txtSearchMultiple.Text == "")
            {
                Empty = true;
            }
            return Empty;
        }
        private List<string> getSearchTerm()
        {
            List<string> output = new List<string>();
            if (!CheckEmptyTextBox())
            {
                string SerachTerm = txtSearch.Text;
                if (!string.IsNullOrEmpty(txtSearchMultiple.Text))
                {
                    char[] delimiter = { '\n' };
                    string[] split = txtSearchMultiple.Text.Split(delimiter);
                    foreach (string part in split)
                    {
                        List<Staff> staff = Staff.Search(part);
                        foreach (Staff s in staff)
                        {
                            output.Add(s.Username);
                        }
                    }
                }
                else
                {
                    output.Add(txtSearch.Text);
                }
            }
            else
            {
                string sScript0 = "window.alert('Please insert names,username,staff no in textbox to search.')";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
            }
            return output;
        }
        private void DoATCSearch(List<string> UsernameList)
        {
            List<Utilities> output = new List<Utilities>();
            output = Utilities.SearchATCByStaff(UsernameList, ref output);
            gvATC.DataSource = output;
            gvATC.DataBind();
            this.lblTotListedVal1.Text = output.Count.ToString();
        }
        private void DoPTASearch(List<string> UsernameList)
        {
            List<Utilities> output = new List<Utilities>();
            output = Utilities.SearchPTAByStaff(UsernameList, ref output);
            gvPTA.DataSource = output;
            gvPTA.DataBind();
            this.lblTotAvailableVal2.Text = output.Count.ToString();
        }
        private void DoCEFSeacrh(List<string> UsernameList)
        {
            List<Utilities> output = new List<Utilities>();
            output = Utilities.SearchCEFByStaff(UsernameList, ref output);
            gvCEF.DataSource = output;
            gvCEF.DataBind();
            this.lblTotListedVal.Text = output.Count.ToString();
        }
        private void ShowAllCEF()
        {
            //get all Unsubmitted CEF
            List<Utilities> CEFList = Utilities.getAllNotCompletedCEF();
            gvCEF.DataSource = CEFList;
            gvCEF.DataBind();
            ViewState["lstCEF"] = CEFList;
            this.lblTotListedVal.Text = CEFList.Count.ToString();
        }
        private void ShowAllPTA()
        {
            //get all Unsubmitted PTA
            List<Utilities> PTAList = Utilities.getAllNotCompletedPTA();
            gvPTA.DataSource = PTAList;
            gvPTA.DataBind();
            ViewState["lstPTA"] = PTAList;
            this.lblTotAvailableVal2.Text = PTAList.Count.ToString();
        }
        private void ShowAllATC()
        {
            //get all Staff that has confirmation on prebook
            List<Utilities> ATCList = Utilities.getAllNotConfirmDate();
            gvATC.DataSource = ATCList;
            gvATC.DataBind();
            ViewState["lstATC"] = ATCList;
            this.lblTotListedVal1.Text = ATCList.Count.ToString();
        }
        //sendreminderConfirmationAttendance
        private void SendEmailsConfirmAttendance(List<AttendanceConfirmation> allConfirmations, List<Staff> lstStaff)
        {
            Dictionary<int, TrainingEvent> eventCache = new Dictionary<int, TrainingEvent>();

            foreach (Staff staff in lstStaff)
            {
                // list of all events to be confirmed by the staff
                List<TrainingEvent> events = new List<TrainingEvent>();
                foreach (AttendanceConfirmation ac in allConfirmations)
                {
                    if (ac.StaffUsername.ToUpper() == staff.Username.ToUpper())
                    {
                        // check in cache first so we don't have to load the event each time
                        TrainingEvent ev = null;
                        if (eventCache.ContainsKey(ac.EventID))
                            ev = eventCache[ac.EventID];
                        else
                        {
                            ev = TrainingEvent.GetById(ac.EventID);
                            if (ev != null)
                                eventCache[ev.Id] = ev;
                        }

                        if (ev != null)
                            events.Add(ev);
                    }
                }

                if (events.Count == 0)
                    continue;

                string strEvents = "";
                strEvents = "<table border='1' cellpadding='10px'>";
                strEvents += "<tr><td>Title</td></tr>";
                if (events.Count > 0)
                {
                    foreach (TrainingEvent item in events)
                    {
                        string strRow = "<tr>";
                        strRow += "<td>" + item.Title.ToString() + "</td>";
                        strRow += "</tr>";
                        strEvents += strRow;
                    }
                }
                strEvents += "</table>";
                Dictionary<string, string> replacements = new Dictionary<string, string>();
                replacements["Name"] = staff.Name;
                replacements["Events"] = strEvents;
                replacements["CurrentDate"] = DateTime.Now.ToString("dd MMM yyyy");

                string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "ConfirmAttendance.htm");

                emailTemplateFile = Server.MapPath(emailTemplateFile);

                UtilityEmail.Send(staff.Email, "", "Attendance Confirmation Required", emailTemplateFile, replacements);

            }
        }
        private void DoReminderSurvey(string surveyType)
        {
            // surveyType is "CEF" or "PTA" only.
            // fetch the event dates
            bool isPTA = surveyType.ToUpper() == "PTA";
            Dictionary<int, EventDate> eventDates = new Dictionary<int, EventDate>();
            Utilities.getDicEventDateandKeys(surveyType, ref eventDates, isPTA);
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

                    Staff supervisor = staff.GetSupervisor();
                    if (supervisor == null)
                        continue;

                    Dictionary<string, string> replacements =
                        new Dictionary<string, string>();
                    replacements["Name"] = staff.Name;
                    replacements["SupervisorName"] = supervisor.Name;
                    replacements["EventName"] = ev.Title;
                    replacements["StartDate"] = ed.StartDate.ToString("dd/MM/yyyy");
                    replacements["EndDate"] = ed.EndDate.ToString("dd/MM/yyyy");
                    replacements["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                    replacements["EventID"] = ev.SAPId.ToString();
                    string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "Invite" + surveyType + ".htm");
                    emailTemplateFile = Server.MapPath(emailTemplateFile);

                    if (isPTA)
                        UtilityEmail.Send(supervisor.Email, staff.Email, "Your Post-Training Assessment is Required", emailTemplateFile, replacements);
                    else
                        UtilityEmail.Send(staff.Email, supervisor.Email, "Your Course Evaluation is Required", emailTemplateFile, replacements);
                }
            }
        }
        private void DoSurveyReminderByStaff(int selectedID, string SelectedUsername, string surveyType)
        {
            List<AttendedEvent> unsubmittedEvents = new List<AttendedEvent>();
            if (surveyType == "PTA")
            {
                unsubmittedEvents = AttendedEvent.GetUnsubmitted(surveyType, selectedID);
            }
            else
            {
                unsubmittedEvents = AttendedEvent.GetUnsubmitted(surveyType, SelectedUsername);
            }
            EventDate ed = EventDate.GetById(selectedID);
            TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
            foreach (AttendedEvent ae in unsubmittedEvents)
            {
                Staff staff = new Staff();
                Staff supervisor = new Staff();
                if (surveyType == "PTA")
                {
                    staff = Staff.GetFromUsername(ae.StaffUsername);
                }
                else
                {
                    staff = Staff.GetFromUsername(SelectedUsername);
                }
                if (staff == null)
                    continue;
                if (surveyType == "PTA")
                {
                    supervisor = Staff.GetFromUsername(SelectedUsername);
                }
                else
                {
                    supervisor = staff.GetSupervisor();
                }
                if (supervisor == null)
                    continue;

                Dictionary<string, string> replacements =
                    new Dictionary<string, string>();
                replacements["Name"] = staff.Name;
                replacements["SupervisorName"] = supervisor.Name;
                replacements["EventName"] = ev.Title;
                replacements["StartDate"] = ed.StartDate.ToString("dd/MM/yyyy");
                replacements["EndDate"] = ed.EndDate.ToString("dd/MM/yyyy");
                replacements["CurrentDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                replacements["EventID"] = ev.SAPId.ToString();
                string emailTemplateFile = System.IO.Path.Combine(EmailTemplateFolder, "Invite" + surveyType + ".htm");
                emailTemplateFile = Server.MapPath(emailTemplateFile);
                if (surveyType == "PTA")
                    UtilityEmail.Send(supervisor.Email, staff.Email, "Your Post-Training Assessment is Required", emailTemplateFile, replacements);
                else
                    UtilityEmail.Send(staff.Email, supervisor.Email, "Your Course Evaluation is Required", emailTemplateFile, replacements);
            }
            string sScript0 = "window.alert('Email Notification has been sent to selected staff.')";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
            return;
        }
        private void getListUncompletedSupervisor()
        {
            List<Staff> super = new List<Staff>();
            Staff Supervisor = new Staff();
            string sScript0 = String.Empty;
            if (ddlTrainingType.SelectedValue == "0")
            {
                sScript0 = "window.alert('Please select Training Type.')";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
                return;
            }
            if (ddlTrainingType.SelectedValue.ToUpper() == "BOOK")
            {
                List<BookedEvent> booked = BookedEvent.GetAllByStage(1);
                foreach (BookedEvent be in booked)
                {
                    if (be.StaffUsername != "")
                    {
                        bool exists = false;
                        Supervisor = Staff.GetSupervisorByUsername(be.StaffUsername);
                        if (Supervisor != null)
                        {

                            foreach (Staff staff in super)
                            {
                                if (staff.Username.ToUpper() == Supervisor.Username.ToUpper())
                                {
                                    exists = true;
                                    break;
                                }
                            }
                        }
                        if (exists)
                            continue;
                    }
                    if (Supervisor != null)
                        super.Add(Supervisor);
                }
            }
            if (ddlTrainingType.SelectedValue.ToUpper() == "PREBOOK")
            {
                List<PrebookedEvent> prebook = PrebookedEvent.GetAllbyStage(1);
                foreach (PrebookedEvent pe in prebook)
                {
                    if (pe.StaffUsername != "")
                    {
                        bool exists = false;
                        Supervisor = Staff.GetSupervisorByUsername(pe.StaffUsername);
                        if (Supervisor != null)
                        {

                            foreach (Staff staff in super)
                            {
                                if (staff.Username.ToUpper() == Supervisor.Username.ToUpper())
                                {
                                    exists = true;
                                    break;
                                }
                            }
                        }

                        if (exists)
                            continue;
                    }
                    if (Supervisor != null)
                        super.Add(Supervisor);
                }
            }
            // Sort Employees by their names.  
            Staff_SortByName eName = new Staff_SortByName();
            super.Sort(eName);
            //super.Sort(delegate(Staff S1, Staff S2) { return S2.Name.CompareTo(S1.Name); });
            lstSupervisors.DataSource = super;
            lblTotAvailableVal.Text = Convert.ToString(super.Count);
            lstSupervisors.DataTextField = "NameUserID";
            lstSupervisors.DataValueField = "Username";
            lstSupervisors.DataBind();
        }
        private void ShowHideDiv(bool isBack)
        {
            if (isBack)
            {
                divSupervisor.Visible = true;
                divStaffAppList.Visible = false;
                ClearLabels();
            }
            else
            {
                divStaffAppList.Visible = true;
                divSupervisor.Visible = false;
            }
        }
        private void ClearLabels()
        {
            lblSuperNameVal.Text = "";
            lblStaffIDVal.Text = "";
            lblSubsidiaryVal.Text = "";
            lblPendingAppVal.Text = "";
            lblTotalStaffVal.Text = "";
            lblTypeVal.Text = "";
        }
        private void ShowWorkflowDetail(string SelectedSupervisor, string Total)
        {
            lblSuperNameVal.Text = SelectedSupervisor;
            lblPendingAppVal.Text = Total;
            lblTotListedAppVal.Text = Total;
            Listing.TrainingType SelectedType = (Listing.TrainingType)Enum.Parse(typeof(Listing.TrainingType), ddlTrainingType.SelectedValue);
            if (SelectedType == Listing.TrainingType.Prebook)
                lblTypeVal.Text = "PREBOOK";
            if (SelectedType == Listing.TrainingType.Book)
                lblTypeVal.Text = "BOOK";
            Staff super = Staff.getFromName(SelectedSupervisor);
            if (super != null)
            {
                lblStaffIDVal.Text = super.StaffID;
                lblSubsidiaryVal.Text = super.Subsidiary;
                List<Staff> stafflist = Staff.GetStaffUnder(super.Username, false);
                lblTotalStaffVal.Text = stafflist.Count.ToString();
            }
        }
        #endregion
    }
}