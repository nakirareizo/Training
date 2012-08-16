using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition
{
    public partial class Prebook : System.Web.UI.Page
    {
        private const string QueryKeySuggestedEvents = "SE";
        private const string QueryKeyBacklink = "BL";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Staff.IsSupervisor(Page.User.Identity.Name.ToUpper()))
            {
                btnReject.Visible = true;
            }
            this.lblPgTitle.Text = "Prebook Training";
            uscStaffList.SelectStaff += new StaffListControlBase.SelectStaffHandler(uscStaffList_SelectStaff);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            uscEventSelector.IsAdHoc = Request.QueryString[QueryKeySuggestedEvents] == null;
            // show back link if specified
            if (Request.QueryString[QueryKeyBacklink] != null)
            {
                lnkBackToAssessment.NavigateUrl = Request.QueryString[QueryKeyBacklink].ToString();
            }
            else
                lnkBackToAssessment.Visible = false;
            if (IsPostBack)
            {
                string Username = (uscStaffList.FindControl("dlStaff") as DropDownList).SelectedValue;
                if (Username.ToUpper() == Page.User.Identity.Name.ToUpper() || Username == "")
                {
                    lblPgTitle.Text = "";
                    this.lblPgTitle.Text = "Prebook Training";
                }
                else if (Username.ToUpper() != Page.User.Identity.Name.ToUpper())
                {
                    lblPgTitle.Text = "";
                    this.lblPgTitle.Text = "Prebook Approval";
                }
            }
        }

        void uscStaffList_SelectStaff(object sender, StaffListControlBase.SelectStaffEventArgs args)
        {
            pnlEvents.Visible = true;
            uscEventSelector.LoadAndShowSelectedEvents(args.StaffUsername);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (uscStaffList.CurrentStaff.IsSupervisor() == false)
            {
                uscEventSelector.Save();
                string sScript0 = "window.alert('Your Prebook Training(s) saved.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
            }
            else
            {
                uscEventSelector.Save();
                string sScript0 = "window.alert('Your changes has been saved.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int SelectedItem = (uscEventSelector.FindControl("lbSelected") as ListBox).Items.Count;
            if (SelectedItem > 0)
            {
                string trxId = Request.QueryString["ID"].ToString();
                uscEventSelector.Submit(trxId);
            }
            else
            {
                string sScript0 = "window.alert('Please Select available Training(s) first before submit.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
                return;
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            string errorMessage = "";
            if (!uscEventSelector.Reject(ref errorMessage))
            {
                //ShowError(errorMessage);
                string sScript0 = "window.alert('" + errorMessage + "');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
            else
            {
                uscEventSelector.Reject(ref errorMessage);
                string sScript0 = "window.alert('Training(s) has been rejected.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Book.aspx", sScript0, true);
                return;
            }
        }
    }
}
