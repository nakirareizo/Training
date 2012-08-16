using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BUILD.Training.Classes;
using Telerik.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Classes;
using BUILD.Classes.Base;

namespace BUILD.Training
{
    public partial class AdminListing : System.Web.UI.Page
    {
        #region "Global Declaration"
        public List<Listing> lstTraining
        {
            get
            {
                return (List<Listing>)ViewState["lstTraining"];
            }
            set
            {
                ViewState["lstTraining"] = value;
            }
        }
        public List<Listing> lstSearch
        {
            get
            {
                return (List<Listing>)ViewState["lstSearch"];
            }
            set
            {
                ViewState["lstSearch"] = value;
            }
        }
        public List<Listing> lstApps
        {
            get
            {
                return (List<Listing>)ViewState["lstApps"];
            }
            set
            {
                ViewState["lstApps"] = value;
            }
        }
        public List<Listing> lstTitle
        {
            get
            {
                return (List<Listing>)ViewState["lstTitle"];
            }
            set
            {
                ViewState["lstTitle"] = value;
            }
        }
        public List<Listing> lstStatus
        {
            get
            {
                return (List<Listing>)ViewState["lstStatus"];
            }
            set
            {
                ViewState["lstStatus"] = value;
            }
        }
        public List<Listing> lstSAPStatus
        {
            get
            {
                return (List<Listing>)ViewState["lstSAPStatus"];
            }
            set
            {
                ViewState["lstSAPStatus"] = value;
            }
        }
        public List<Listing> lstNotDefined
        {
            get
            {
                return (List<Listing>)ViewState["lstNotDefined"];
            }
            set
            {
                ViewState["lstNotDefined"] = value;
            }
        }
        public List<Listing> lstSupervisors
        {
            get
            {
                return (List<Listing>)ViewState["lstSupervisors"];
            }
            set
            {
                ViewState["lstSupervisors"] = value;
            }
        }
        public List<Listing> lstDates
        {
            get
            {
                return (List<Listing>)ViewState["lstDates"];
            }
            set
            {
                ViewState["lstDates"] = value;
            }
        }
        public List<Listing> lstMonths
        {
            get
            {
                return (List<Listing>)ViewState["lstMonths"];
            }
            set
            {
                ViewState["lstMonths"] = value;
            }
        }
        public ListingFilter.Filter filter
        {
            get
            {
                return (ListingFilter.Filter)ViewState["filter"];
            }
            set
            {
                ViewState["filter"] = value;
            }
        }
        ListingFilter.Filter Mode = new ListingFilter.Filter();
        public enum FilterMode
        {
            ShowAll,
            Advance
        }
        public FilterMode SelectedFilterMode
        {
            get
            {
                return (FilterMode)ViewState["SelectedFilterMode"];
            }
            set
            {
                ViewState["SelectedFilterMode"] = value;
            }
        }
        #endregion
        #region "Object Function"
        protected void Page_Load(object sender, EventArgs e)
        {
            string StaffUsername = Page.User.Identity.Name;
            
            if (Staff.IsAdmin(StaffUsername))
            {
                //LoadYear();
            }
            else
            {
                string sScript0 = "window.alert('Ypu don't have authority to view this page.');location.replace('../Default.aspx');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "AdminListing.aspx", sScript0, true);
                return;
            }
        }
        #region "button"
        protected void btnSearch_Click(object sender, EventArgs e)
        {

            if (!CheckEmptyTextBox())
            {
                divFilter.Visible = false;
                divgv.Visible = true;
                tdSortDate.Visible = true;
                //ViewState["lstTraining"] = lstTraining;
                List<Listing> output = new List<Listing>();
                //string SelecteddllItem = ddlSearch.SelectedValue.ToString().ToUpper();
                string SerachTerm = txtSearch.Text;
                if (!string.IsNullOrEmpty(txtSearchMultiple.Text))
                {
                    char[] delimiter = { '\n' };
                    string[] split = txtSearchMultiple.Text.Split(delimiter);
                    foreach (string part in split)
                    {
                        List<string> lstStaff = new List<string>();
                        List<Staff> staff = Staff.Search(part);
                        foreach (Staff s in staff)
                        {
                            lstStaff.Add(s.Username);
                        }
                        foreach (string StaffUsername in lstStaff)
                        {
                            output = Listing.Search(StaffUsername, ref output);
                        }
                    }
                }
                else
                {
                    List<string> lstStaff = new List<string>();
                    List<Staff> staff = Staff.Search(SerachTerm);
                    foreach (Staff s in staff)
                    {
                        lstStaff.Add(s.Username);
                    }
                    foreach (string StaffUsername in lstStaff)
                    {
                        output = Listing.Search(StaffUsername, ref output);
                    }
                }
                gvAdminList.DataSource = output;
                gvAdminList.DataBind();
                this.lblTotListedVal.Text = output.Count.ToString();
                ViewState["lstTraining"] = output;
                List<string> TitleLst = ListingFilter.LoadTrainingTitle(output);
                ddlTitle.DataSource = TitleLst;
                ddlTitle.DataBind();
                ddlTitle.Items.Add("All");
                ddlTitle.SelectedValue = "All";
                Mode = ListingFilter.Filter.All;
                ViewState["filter"] = Mode;
                //Load TrainingDate
                LoadSortDate(output);
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearchMultiple.Text = "";
        }
        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            FilterMode Mode = FilterMode.ShowAll;
            ViewState["SelectedFilterMode"] = Mode;
            ShowHide(Mode);
            ShowAll();
        }
        protected void btnAdvance_Click(object sender, EventArgs e)
        {
            FilterMode Mode = FilterMode.Advance;
            ViewState["SelectedFilterMode"] = Mode;
            ShowHide(Mode);
            //1. Get all Training Supervisor
            List<Staff> supervisorlst = Staff.GetAllSupervisor();
            supervisorlst.Sort(delegate(Staff S1, Staff S2) { return S1.Name.CompareTo(S2.Name); });
            lstbxSupervisors.DataSource = supervisorlst;
            lstbxSupervisors.DataTextField = "NameUserID";
            lstbxSupervisors.DataValueField = "Username";
            lstbxSupervisors.DataBind();
            lstbxSupervisors.Visible = true;
            //2. Get all Training Title
            List<TrainingEvent> TrainingTitleLst = TrainingEvent.getAllTrainingTitle();
            TrainingTitleLst.Sort(delegate(TrainingEvent TE1, TrainingEvent TE2) { return TE1.Title.CompareTo(TE2.Title); });
            lstbxTitle.DataSource = TrainingTitleLst;
            lstbxTitle.DataTextField = "Title";
            lstbxTitle.DataValueField = "ID";
            lstbxTitle.DataBind();
            lstbxTitle.Visible = true;
        }
        protected void btnConvertExcel_Click(object sender, EventArgs e)
        {
            string Filename = "";
            Filename = GetgvDataAndFilename(Filename);
            //Listing.ExportGridViewtToExcel(Filename, gvAdminList, Response);
            //GridViewExportUtility.Export(Filename, gvAdminList, Response);
            GridViewExportUtility.ExportToExcel(Filename, gvAdminList, Response);
        }
        #endregion
        #region Filter
        protected void ddlApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlApps.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    ViewState["lstTraining"] = lstTraining;
                    string SearchItem = ddlApps.SelectedValue.ToString();
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.Type);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstApps"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.Type;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void ddlTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlTitle.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    ViewState["lstTraining"] = lstTraining;
                    string SearchItem = ddlTitle.SelectedValue.ToString();
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.Title);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstTitle"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.Title;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlStatus.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    ViewState["lstTraining"] = lstTraining;
                    string SearchItem = ddlStatus.SelectedValue.ToString();
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.Status);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstStatus"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.Status;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void ddlSAP_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlSAP.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    ViewState["lstTraining"] = lstTraining;
                    string SearchItem = ddlSAP.SelectedValue.ToString();
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.SAP);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstSAPStatus"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.SAP;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void ddlUndefined_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlUndefined.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    string SearchItem = ddlUndefined.SelectedValue.ToString();
                    ViewState["lstTraining"] = lstTraining;
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.NotDefined);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstNotDefined"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.NotDefined;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void ddlSupervisors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedValue = ddlSupervisors.SelectedValue.ToString().ToUpper();
            if (SelectedFilterMode == FilterMode.Advance)
            {
                ShowAllFilter();
            }
            else
            {
                if (SelectedValue == "ALL")
                {
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    this.lblTotListedVal.Text = lstTraining.Count.ToString();
                    Mode = ListingFilter.Filter.All;
                    ViewState["filter"] = Mode;
                }
                else
                {
                    string SearchItem = ddlSupervisors.SelectedValue.ToString();
                    ViewState["lstTraining"] = lstTraining;
                    List<Listing> lstResult = new List<Listing>();
                    lstResult.Clear();
                    ListingFilter.getResultFilter(ref lstResult, lstTraining, SearchItem, ListingFilter.Filter.Supervisors);
                    gvAdminList.DataSource = lstResult;
                    gvAdminList.DataBind();
                    ViewState["lstSupervisors"] = lstResult;
                    this.lblTotListedVal.Text = lstResult.Count.ToString();
                    Mode = ListingFilter.Filter.Supervisors;
                    ViewState["filter"] = Mode;
                }
            }
        }
        protected void btnSelectTitle_Click(object sender, EventArgs e)
        {
            ShowAllFilter();
        }
        protected void btnSelectSuper_Click(object sender, EventArgs e)
        {
            ShowAllFilter();
        }
        protected void ddlSortDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime SelectedDate = DateTime.Parse(ddlSortDate.SelectedValue.ToString());
            List<Listing> DateRequestedList = new List<Listing>();
            ListingFilter.FilterbyDateRequested(SelectedDate, lstTraining, ref DateRequestedList);
            gvAdminList.DataSource = DateRequestedList;
            gvAdminList.DataBind();
            this.lblTotListedVal.Text = DateRequestedList.Count.ToString();
            ViewState["lstDates"] = DateRequestedList;
            Mode = ListingFilter.Filter.Date;
            ViewState["filter"] = Mode;
        }
        protected void ddlMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedMonth = Convert.ToInt32(ddlMonths.SelectedValue.ToString());
            List<Listing> MonthFilteredList = new List<Listing>();
            ListingFilter.FilterbyMonth(SelectedMonth, lstTraining, ref MonthFilteredList);
            gvAdminList.DataSource = MonthFilteredList;
            gvAdminList.DataBind();
            this.lblTotListedVal.Text = MonthFilteredList.Count.ToString();
            ViewState["lstMonths"] = MonthFilteredList;
            //set filter type
            Mode = ListingFilter.Filter.Month;
            ViewState["filter"] = Mode;
        }
        #endregion
        #region "gridview"
        protected void gvAdminList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Retrieve the LinkButton control from column.
                LinkButton lbtnAssign = (LinkButton)e.Row.Cells[8].Controls[1];
                // Set the LinkButton's CommandArgument property with the row's index.
                lbtnAssign.CommandArgument = e.Row.RowIndex.ToString();
            }
        }
        protected void gvAdminList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == (DataControlRowState.Alternate | DataControlRowState.Normal)))
                {
                    /* Get the value of the field that is going to determine what is disabled and what is enabled. */
                    string StatusValue = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SupervisorName"));

                    /* compare the value */
                    if (StatusValue.ToUpper() != "NOT DEFINED")
                    {
                        /* find the control you want to disable or enable */
                        LinkButton lbtnAssign = (LinkButton)e.Row.FindControl("lbtnAssign");

                        /* check if control exists. */
                        if (lbtnAssign != null)
                        {
                            /* enable/disable or do whatever you want with it */
                            lbtnAssign.Visible = false;
                        }
                    }
                }
            }
        }
        protected void gvAdminList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandSource.GetType().Name.ToString().ToUpper() == "LINKBUTTON")
                {
                    if (((LinkButton)e.CommandSource).ID.ToUpper() == "LBTNASSIGN")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = gvAdminList.Rows[index];
                        string SelectedStaffID = gvAdminList.DataKeys[index].Value.ToString();
                        Response.BufferOutput = true;
                        Response.Redirect("~/Training/Admin/TrainingWorkflow.aspx?ID=" + Request.QueryString["ID"].ToString(), true);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void gvAdminList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["filter"] = filter;
            List<Listing> SelectedData = new List<Listing>();
            switch (filter)
            {
                case ListingFilter.Filter.All:
                    SelectedData = lstTraining;
                    break;
                case ListingFilter.Filter.Type:
                    SelectedData = lstApps;
                    break;
                case ListingFilter.Filter.Status:
                    SelectedData = lstStatus;
                    break;
                case ListingFilter.Filter.SAP:
                    SelectedData = lstSAPStatus;
                    break;
                case ListingFilter.Filter.Title:
                    SelectedData = lstTitle;
                    break;
                case ListingFilter.Filter.NotDefined:
                    SelectedData = lstNotDefined;
                    break;
                case ListingFilter.Filter.Supervisors:
                    SelectedData = lstSupervisors;
                    break;
                case ListingFilter.Filter.Date:
                    SelectedData = lstDates;
                    break;
                case ListingFilter.Filter.Month:
                    SelectedData = lstMonths;
                    break;
            }
            gvAdminList.PageIndex = e.NewPageIndex;
            this.gvAdminList.DataSource = SelectedData;
            this.gvAdminList.DataBind();
            gvAdminList.SelectedIndex = -1;

        }
        #endregion
        #endregion
        #region "Custom Function"
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
        private bool CheckEmptyTextBox()
        {
            bool Empty = false;
            if (txtSearch.Text == "" && txtSearchMultiple.Text == "")
            {
                Empty = true;
            }
            return Empty;
        }
        private void ShowHide(FilterMode Mode)
        {
            switch (Mode)
            {
                case FilterMode.ShowAll:
                    divFilter.Visible = true;
                    divgv.Visible = true;
                    ddlSupervisors.Visible = true;
                    ddlTitle.Visible = true;
                    TabPanel3.Visible = true;
                    TabPanel4.Visible = true;
                    TabPanel5.Visible = true;
                    tdSortDate.Visible = true;
                    lstbxTitle.Visible = false;
                    lstbxSupervisors.Visible = false;
                    btnSelectTitle.Visible = false;
                    btnSelectSuper.Visible = false;
                    break;
                case FilterMode.Advance:
                    divFilter.Visible = true;
                    divgv.Visible = true;
                    lstbxTitle.Visible = true;
                    lstbxSupervisors.Visible = true;
                    btnSelectTitle.Visible = true;
                    btnSelectSuper.Visible = true;
                    ddlSupervisors.Visible = false;
                    ddlTitle.Visible = false;
                    TabPanel3.Visible = false;
                    TabPanel4.Visible = false;
                    TabPanel5.Visible = false;
                    tdSortDate.Visible = false;
                    break;
            }

        }
        private void ShowAll()
        {
            List<Listing> AdminTrainingLst = Listing.getAll();
            var sortedList = AdminTrainingLst.OrderBy(staff => staff.StaffName);
            sortedList.ToList();
            sortedList.Distinct<Listing>().ToList<Listing>();
            List<Listing> FilteredList = sortedList.ToList<Listing>();
            gvAdminList.DataSource = FilteredList;
            gvAdminList.DataBind();
            ViewState["lstTraining"] = FilteredList;
            this.lblTotListedVal.Text = FilteredList.Count.ToString();
            //Load Training Title
            List<string> TitleLst = ListingFilter.LoadTrainingTitle(FilteredList);
            ddlTitle.DataSource = TitleLst;
            ddlTitle.DataBind();
            ddlTitle.Items.Insert(0, "All");
            ddlTitle.SelectedIndex = 0;
            //set filter type
            Mode = ListingFilter.Filter.All;
            ViewState["filter"] = Mode;
            //Load ddlSupervisors
            LoadSupervisors(FilteredList);
            //Load TrainingDate
            LoadSortDate(FilteredList);
        }
        private void LoadSortDate(List<Listing> AdminTrainingLst)
        {
            var sortedList = AdminTrainingLst.OrderBy(c => c.DateRequest);
            sortedList.ToList();
            List<string> Datelist = new List<string>();
            foreach (Listing lst in sortedList)
            {
                if (lst.DateRequest.HasValue)
                {
                    string Date = Convert.ToString(lst.DateRequest.Value.ToString("dd MMM yyyy"));
                    Datelist.Add(Date);
                }
            }
            List<string> DateListFiltered = Datelist.Distinct<string>().ToList<string>();
            ddlSortDate.DataSource = DateListFiltered;
            ddlSortDate.DataBind();
            ddlSortDate.Items.Insert(0, "Please Select");
        }
        private void ShowAllFilter()
        {

            List<Listing> output = new List<Listing>();
            string SelectedValue = "";
            switch (tcTabs.ActiveTabIndex)
            {
                case 0:
                    SelectedValue = ddlApps.SelectedValue.ToString();
                    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.Type, ref output);
                    Mode = ListingFilter.Filter.Type;
                    ViewState["filter"] = Mode;
                    break;
                case 1:
                    SelectedValue = lstbxTitle.SelectedValue.ToString();
                    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.Title, ref output);
                    Mode = ListingFilter.Filter.Title;
                    ViewState["filter"] = Mode;
                    break;
                //case 2:
                //    SelectedValue = ddlStatus.SelectedValue.ToString();
                //    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.Status, ref output);
                //    Mode = ListingFilter.Filter.Status;
                //    ViewState["filter"] = Mode;
                //    break;
                //case 3:
                //    SelectedValue = ddlSAP.SelectedValue.ToString();
                //    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.SAP, ref output);
                //    Mode = ListingFilter.Filter.SAP;
                //    ViewState["filter"] = Mode;
                //    break;
                //case 4:
                //    SelectedValue = ddlUndefined.SelectedValue.ToString();
                //    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.NotDefined, ref output);
                //    Mode = ListingFilter.Filter.NotDefined;
                //    ViewState["filter"] = Mode;
                //    break;
                case 2:
                    SelectedValue = lstbxSupervisors.SelectedValue.ToString();
                    ListingFilter.getAdvanceFilterResult(SelectedValue, ListingFilter.Filter.Supervisors, ref output);
                    Mode = ListingFilter.Filter.Supervisors;
                    ViewState["filter"] = Mode;
                    break;
            }
            gvAdminList.DataSource = output;
            gvAdminList.DataBind();
            switch (Mode)
            {
                case ListingFilter.Filter.Type:
                    ViewState["lstApps"] = output;
                    break;
                case ListingFilter.Filter.Status:
                    ViewState["lstStatus"] = output;
                    break;
                case ListingFilter.Filter.SAP:
                    ViewState["lstSAPStatus"] = output;
                    break;
                case ListingFilter.Filter.Title:
                    ViewState["lstTitle"] = output;
                    break;
                case ListingFilter.Filter.NotDefined:
                    ViewState["lstNotDefined"] = output;
                    break;
                case ListingFilter.Filter.Supervisors:
                    ViewState["lstSupervisors"] = output;
                    break;
            }
            lblTotListedVal.Text = output.Count.ToString();
        }
        private void LoadSupervisors(List<Listing> AdminTrainingLst)
        {
            List<string> lstSupervisors = new List<string>();
            List<string> SupervisorsFiltered = new List<string>();
            foreach (Listing item in AdminTrainingLst)
            {
                lstSupervisors.Add(item.SupervisorName);
            }
            SupervisorsFiltered = lstSupervisors.Distinct<string>().ToList<string>();
            SupervisorsFiltered.Sort();
            ddlSupervisors.DataSource = SupervisorsFiltered;
            ddlSupervisors.DataBind();
            ddlSupervisors.Items.Insert(0, "All");
            ddlSupervisors.SelectedIndex = 0;
        }
        private string GetgvDataAndFilename(string Filename)
        {
            ViewState["filter"] = filter;
            List<Listing> SelectedData = new List<Listing>();
            switch (filter)
            {
                case ListingFilter.Filter.All:
                    gvAdminList.DataSource = lstTraining;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.Type:
                    gvAdminList.DataSource = lstApps;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for" + ddlApps.SelectedValue.ToString().ToUpper() + "( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.Status:
                    gvAdminList.DataSource = lstStatus;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for Staffs Training Status( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.SAP:
                    gvAdminList.DataSource = lstSAPStatus;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for Training SAP Status( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.Title:
                    gvAdminList.DataSource = lstTitle;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report Staff On Selected Training( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.NotDefined:
                    gvAdminList.DataSource = lstNotDefined;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for Undefined Workflow Staffs( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.Supervisors:
                    gvAdminList.DataSource = lstSupervisors;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report Based Workflow Supervisors( " + DateTime.Now.ToString("dd-MMM-yyyy") + ")";
                    break;
                case ListingFilter.Filter.Date:
                    gvAdminList.DataSource = lstDates;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for Date( " + ddlSortDate.SelectedValue.ToString() + ")";
                    break;
                case ListingFilter.Filter.Month:
                    gvAdminList.DataSource = lstMonths;
                    gvAdminList.DataBind();
                    Filename = "Training Application Status Report for Month( " + ddlMonths.SelectedItem.ToString() + ")";
                    break;
            }
            return Filename;
        }
        #endregion
    }
}