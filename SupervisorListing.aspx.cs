using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BUILD.Training.Classes;
using TrainingRequisition.ClassLibrary.Entities;

namespace BUILD.Training
{
    public partial class SupervisorListing : System.Web.UI.Page
    {
        #region "Global Declarations"
        public List<StaffAttendedEvents> StaffsAttendedList
        {
            get
            {
                return (List<StaffAttendedEvents>)ViewState["StaffsAttendedList"];
            }
            set
            {
                ViewState["StaffsAttendedList"] = value;
            }
        }
        #endregion
        #region "Object Function"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string StaffUsername = Page.User.Identity.Name;
                bool supervisor = Staff.IsSupervisor(StaffUsername);
                if (supervisor)
                {
                    List<StaffAttendedEvents> AttendedList = StaffAttendedEvents.getAll(StaffUsername);
                    //bind attended list to grid view
                    this.gvAttended.DataSource = AttendedList;
                    this.gvAttended.DataBind();
                    ViewState["StaffsAttendedList"] = AttendedList;
                    this.lblTotListedVal.Text = AttendedList.Count.ToString();
                    //get all date and bind to ddlSortDate
                    //LoadDateList(AttendedList);
                    //get All Staff under workflow;
                    LoadStaffName(AttendedList);

                }
                else
                {
                    string sScript0 = "alert('You are not authorized to access this page. Only Supervisor can view this page.'); location.replace('../Default.aspx');";
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Page_Load_Script_10", sScript0, true);
                    return;
                }
            }
        }

        protected void gvAttended_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttended.PageIndex = e.NewPageIndex;
            this.gvAttended.DataSource = StaffsAttendedList;
            this.gvAttended.DataBind();
            gvAttended.SelectedIndex = -1;
        }
        //protected void ddlSortDate_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string SelectedDate = ddlSortDate.SelectedValue.ToString();
        //    if (SelectedDate.ToUpper() != "PLEASE SELECT")
        //    {
        //        List<StaffAttendedEvents> lstResult = new List<StaffAttendedEvents>();
        //        StaffAttendedEvents.getAllbyFilter(StaffsAttendedList, ref lstResult, SelectedDate);
        //        this.gvAttended.DataSource = lstResult;
        //        this.gvAttended.DataBind();
        //        ViewState["StaffsAttendedList"] = lstResult;
        //        this.lblTotListedVal.Text = lstResult.Count.ToString();
        //    }
        //}
        protected void ddlStaffName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedName = ddlStaffName.SelectedValue.ToString();
            if (SelectedName.ToUpper() != "PLEASE SELECT")
            {
                List<StaffAttendedEvents> lstResult = new List<StaffAttendedEvents>();
                StaffAttendedEvents.getAllbyFilter(StaffsAttendedList, ref lstResult, SelectedName);
                this.gvAttended.DataSource = lstResult;
                this.gvAttended.DataBind();
                this.lblTotListedVal.Text = lstResult.Count.ToString();
            }
            if (SelectedName.ToUpper() == "ALL")
            {
                this.gvAttended.DataSource = StaffsAttendedList;
                this.gvAttended.DataBind();
                this.lblTotListedVal.Text = StaffsAttendedList.Count.ToString();
            }
        }
        #endregion
        #region "Custom Functions"
        //private void LoadDateList(List<StaffAttendedEvents> AttendedList)
        //{
        //    var sortedList = AttendedList.OrderBy(c => c.TrainingDate);
        //    sortedList.ToList();
        //    sortedList.Distinct<StaffAttendedEvents>().ToList<StaffAttendedEvents>();
        //    List<string> DateList = new List<string>();
        //    foreach (StaffAttendedEvents sae in sortedList)
        //    {
        //        string Date = sae.TrainingDate;
        //        DateList.Add(Date);
        //    }
        //    List<string> DateListFiltered = DateList.Distinct<string>().ToList<string>();
        //    ddlSortDate.DataSource = DateListFiltered;
        //    ddlSortDate.DataBind();
        //    ddlSortDate.Items.Insert(0, "Please Select");
        //}
        private void LoadStaffName(List<StaffAttendedEvents> AttendedList)
        {
            List<string> lstStaffName = new List<string>();
            foreach (StaffAttendedEvents sae in AttendedList)
            {
                lstStaffName.Add(sae.StaffName);
            }
            ddlStaffName.DataSource = lstStaffName;
            ddlStaffName.DataBind();
            ddlStaffName.Items.Insert(0, "Please Select");
            ddlStaffName.Items.Insert(1, "All");
        }
        #endregion
    }
}