using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using TrainingRequisition.ClassLibrary.Utilities;
using BUILD.Training.Classes;
using BUILD.Training.Admin.Classes;

namespace BUILD.Training.Admin
{
    public partial class TrainingWorkflow : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        MainLib.core oCore = new MainLib.core();
        protected string sOrderBy1;
        protected string sOrderBy;
        public List<Staff> BindList
        {
            get
            {
                return UtilityUI.GetListFromViewState<Staff>("BindStaffList", ViewState);
            }
            set
            {
                ViewState["BindStaffList"] = value;
            }
        }
        public DataTable dt
        {
            get
            {
                return (DataTable)ViewState["dt"];
            }
            set
            {
                ViewState["dt"] = value;
            }
        }
        public List<Listing> LstNotDefined
        {
            get
            {
                return (List<Listing>)ViewState["LstNotDefined"];
            }
            set
            {
                ViewState["LstNotDefined"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ListOrder"] = "0";
                if (Session["OrderBy"] != null)
                {
                    sOrderBy = Session["OrderBy"].ToString();
                }
                else
                {
                    sOrderBy = "fldOid ASC";
                }
                Session.Remove("OrderBy");
                sOrderBy = "fldOid ASC";
                Session["Col_1"] = "fldOid";
                Session["Order_1"] = "ASC";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<Staff> staff = new List<Staff>();
            List<string> adminSubsidiaries = new List<string>();
            staff = Staff.Search(txtStaffSearch.Text);
            gvStaff1.DataSource = staff;
            gvStaff1.DataBind();
        }

        protected void btnSearch1_Click(object sender, EventArgs e)
        {
            if (!CheckEmptyTextBox())
            {
                List<AssignedStaff> output = new List<AssignedStaff>();
                string SerachTerm = txtSearch.Text;
                if (!string.IsNullOrEmpty(txtSearchMultiple.Text))
                {
                    char[] delimiter = { '\n' };
                    string[] split = txtSearchMultiple.Text.Split(delimiter);
                    foreach (string part in split)
                    {
                        AssignedStaff AS = AssignedStaff.getAllByUsername(part.Trim());
                        output.Add(AS);
                    }
                }
                else
                {
                    AssignedStaff AS = AssignedStaff.getAllByUsername(SerachTerm);
                    output.Add(AS);
                }
                gvStaffAssigned.DataSource = output;
                gvStaffAssigned.DataBind();
                this.lblTotListedVal.Text = output.Count.ToString();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearchMultiple.Text = "";
        }

        protected void ibtnUnassign_Click(object sender, ImageClickEventArgs e)
        {
            if (this.gvList.Rows.Count == 0)
            {
                string sScript0 = "window.alert('No user to be un-assigned');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ibtnUnassign_Click_Script_10", sScript0, true);
            }
            else
            {
                string Supervisor = this.lblUserNameVal.Text;
                Staff.ClearWorkflow(Supervisor);
                BindGridview(lblUserNameVal.Text);
                List<string> StaffAssigned = Staff.GetStaffAssigned();
                LoadAvailableStaff(StaffAssigned, "0", true);
            }
        }

        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvList.PageIndex = e.NewPageIndex;
            this.gvList.DataSource = BindList;
            this.gvList.DataBind();
            gvList.SelectedIndex = -1;
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandSource.GetType().Name.ToString().ToUpper() == "LINKBUTTON")
                {
                    if (((LinkButton)e.CommandSource).ID.ToUpper() == "LBTNREMOVE")
                    {
                        #region " Button un-assign "
                        try
                        {
                            int index = Convert.ToInt32(e.CommandArgument);
                            #region " For rows that is not hidden, comment the region if the row is hidden "
                            GridViewRow row = this.gvList.Rows[index];
                            string ChosenID = row.Cells[2].Text;
                            Staff.Unassigned(ChosenID);
                            BindGridview(lblUserNameVal.Text);
                            List<string> StaffAssigned = Staff.GetStaffAssigned();
                            LoadAvailableStaff(StaffAssigned, "0", true);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            oCore.LogEvent("TrainingWorkflow.aspx", "gvList_RowCommand", ex.Message, "1");
                            this.lblMsg.Text = ex.Message;
                        }
                        finally
                        {
                            if (conn != null)
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }
                                conn.Dispose();
                            }
                            if (cmd != null)
                            {
                                cmd.Dispose();
                            }
                        }
                        this.gvList.PageIndex = 0;
                        this.gvList.SelectedIndex = -1;
                        return;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentClaim.aspx", "gvList_RowCommand", ex.Message, "1");
                this.lblMsg.Text = ex.Message;
                this.UpdatePanel1.Update();
            }
        }

        protected void gvList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    #region " Retrieve the LinkButton control from the first column. "
                    LinkButton lbtnRemove = (LinkButton)e.Row.Cells[3].Controls[1];
                    #endregion
                    #region " Set the LinkButton's CommandArgument property with the row's index. "
                    lbtnRemove.CommandArgument = e.Row.RowIndex.ToString();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("WFUserAssignmentSS.aspx", "gvList_RowCreated", ex.Message, "1");
                this.lblMsg.Text = ex.Message;
                this.UpdatePanel1.Update();
            }
        }

        protected void gvStaff1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUsername = gvStaff1.SelectedDataKey.Value.ToString();
            Staff SelectedStaff = Staff.getStaffInfo(selectedUsername);
            BindLabels(SelectedStaff);
            BindGridview(this.lblUserNameVal.Text);
            List<string> StaffAssigned = Staff.GetStaffAssigned();
            Session["ListOrder"] = "0";
            LoadAvailableStaff(StaffAssigned, "0", true);
        }

        private void BindLabels(Staff SelectedStaff)
        {
            lblStaffNameVal.Text = SelectedStaff.Name.ToUpper();
            lblStaffNoVal.Text = SelectedStaff.StaffID;
            lblUserNameVal.Text = SelectedStaff.Username;
        }

        private void LoadAvailableStaff(List<string> StaffAssigned, string sortBy, bool isDefault)
        {
            List<Staff> StaffAvailable = Staff.GetAvailableStaffs(StaffAssigned, sortBy, isDefault);
            this.lstAvailableUsers.DataSource = StaffAvailable;
            if (isDefault)
            {
                lstAvailableUsers.DataTextField = "IDNameUser";
                lstAvailableUsers.DataValueField = "Username";
            }
            else
            {
                if (sortBy == "0")
                {
                    lstAvailableUsers.DataTextField = "NameUserID";
                    lstAvailableUsers.DataValueField = "Username";
                }
                if (sortBy == "1")
                {
                    lstAvailableUsers.DataTextField = "UserNameID";
                    lstAvailableUsers.DataValueField = "Username";
                }
                if (sortBy == "2")
                {
                    lstAvailableUsers.DataTextField = "IDNameUser";
                    lstAvailableUsers.DataValueField = "Username";
                }
            }
            this.lstAvailableUsers.DataBind();
            lblTotAvailableVal.Text = Convert.ToString(StaffAvailable.Count);
        }

        protected void lbtnSortByFullName_Click(object sender, EventArgs e)
        {
            List<string> StaffAssigned = Staff.GetStaffAssigned();
            LoadAvailableStaff(StaffAssigned, "0", false);
        }

        protected void lbtnSortByLoginID_Click(object sender, EventArgs e)
        {
            List<string> StaffAssigned = Staff.GetStaffAssigned();
            LoadAvailableStaff(StaffAssigned, "1", false);
        }

        protected void lbtnSortByStaffNo_Click(object sender, EventArgs e)
        {
            List<string> StaffAssigned = Staff.GetStaffAssigned();
            LoadAvailableStaff(StaffAssigned, "2", false);
        }

        protected void ibtnAttachUsers_Click(object sender, ImageClickEventArgs e)
        {
            bool noneSelected = true;
            List<Staff> selectedstaff = new List<Staff>();
            foreach (ListItem item in lstAvailableUsers.Items)
            {
                if (item.Selected)
                {
                    Staff staff = Staff.GetFromUsername(item.Value);
                    selectedstaff.Add(staff);
                    noneSelected = false;
                }
            }
            Staff.SaveToDB(this.lblUserNameVal.Text, selectedstaff);
            BindGridview(this.lblUserNameVal.Text);
            lblTotAssignedVal.Text = Convert.ToString(gvList.Rows.Count);
            List<string> StaffAssigned = Staff.GetStaffAssigned();
            LoadAvailableStaff(StaffAssigned, "0", true);
            if (noneSelected)
            {
                string sScript0 = "window.alert('Please select a user.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ibtnAttachUsers_Click_Script_10", sScript0, true);
                return;
            }
        }

        private void BindGridview(string SelectedSupervisor)
        {
            List<Staff> BindList = new List<Staff>();
            List<string> selectedstaff = Staff.StaffUnderSupervisor(SelectedSupervisor);
            foreach (string item in selectedstaff)
            {
                Staff staff = Staff.GetFromUsername(item);
                BindList.Add(staff);
            }
            gvList.DataSource = BindList;
            gvList.DataBind();
            ViewState["BindStaffList"] = BindList;
            lblTotAssignedVal.Text = Convert.ToString(BindList.Count);
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
    }
}