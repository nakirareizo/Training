using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.UserControls
{
    public partial class BookingOptions : System.Web.UI.UserControl
    {
        int defaultPTABeginDaysAfterEvent = 30;
        int defaultPTAEndDaysAfterEvent = 60;
        bool defaultREQ_BypassPTACheck = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ShowConfig();
        }

        private void ShowConfig()
        {
            string strPTABeginDaysAfterEvent =
                Utility.GetConfiguration(Utility.ASM_PTABeginDaysAfterEvent, null);
            if (string.IsNullOrEmpty(strPTABeginDaysAfterEvent))
                strPTABeginDaysAfterEvent = defaultPTABeginDaysAfterEvent.ToString();
            txtPTABeginDaysAfterEvent.Text = strPTABeginDaysAfterEvent;

            string strPTAEndDaysAfterEvent =
               Utility.GetConfiguration(Utility.ASM_PTAEndDaysAfterEvent, null);
            if (string.IsNullOrEmpty(strPTAEndDaysAfterEvent))
                strPTAEndDaysAfterEvent = defaultPTAEndDaysAfterEvent.ToString();
            txtPTAEndDaysAfterEvent.Text = strPTAEndDaysAfterEvent;

            string strBypassPTACheck =
                Utility.GetConfiguration(Utility.REQ_BypassPTACheck, null);
            if (string.IsNullOrEmpty(strBypassPTACheck))
                strBypassPTACheck = defaultREQ_BypassPTACheck.ToString();
            chkBypassPTACheck.Checked = Convert.ToBoolean(strBypassPTACheck);

            ShowMessage("");

        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int PTABeginDaysAfterEvent = 0;
            string strBeginPTADaysAfterEvent = txtPTABeginDaysAfterEvent.Text;
            if (!int.TryParse(strBeginPTADaysAfterEvent, out PTABeginDaysAfterEvent))
                PTABeginDaysAfterEvent = defaultPTABeginDaysAfterEvent;

            int PTAEndDaysAfterEvent = 0;
            string strEndPTADaysAfterEvent = txtPTAEndDaysAfterEvent.Text;
            if (!int.TryParse(strEndPTADaysAfterEvent, out PTAEndDaysAfterEvent))
                PTAEndDaysAfterEvent = defaultPTAEndDaysAfterEvent;

            bool BypassPTACheck = chkBypassPTACheck.Checked;

            // commit
            Utility.SetConfiguration(Utility.ASM_PTABeginDaysAfterEvent, null,
                PTABeginDaysAfterEvent.ToString());

            Utility.SetConfiguration(Utility.ASM_PTAEndDaysAfterEvent, null,
                PTAEndDaysAfterEvent.ToString());

            Utility.SetConfiguration(Utility.REQ_BypassPTACheck, null,
                BypassPTACheck.ToString());
            string sScript0 = "window.alert('New settings have been applied.');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "BookingOptions.aspx", sScript0, true);
            //ShowMessage("New settings have been applied.");
        }

        private void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                lblMessage.Visible = false;
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = message;
            }
        }

        protected void btnRevert_Click(object sender, EventArgs e)
        {
            ShowConfig();
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // check for valid days

            // begin days
            string strPTABeginDaysAfterEvent = txtPTABeginDaysAfterEvent.Text;
            int PTABeginDaysAfterEvent = 0;
            if (!int.TryParse(strPTABeginDaysAfterEvent, out PTABeginDaysAfterEvent) ||
                PTABeginDaysAfterEvent < 0)
            {
                args.IsValid = false;
                string sScript0 = "Please enter a valid number of between 0 and 365 days.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "BookingOptions.aspx", sScript0, true);
                //ShowMessage("Please enter a valid number of between 0 and 365 days.");
            }

            // ene days
            string strPTAEndDaysAfterEvent = txtPTAEndDaysAfterEvent.Text;
            int PTAEndDaysAfterEvent = 0;
            if (!int.TryParse(strPTAEndDaysAfterEvent, out PTAEndDaysAfterEvent) ||
                PTAEndDaysAfterEvent < 0)
            {
                args.IsValid = false;
                string sScript0 = "Please enter a valid number of between 0 and 365 days.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "BookingOptions.aspx", sScript0, true);
                //ShowMessage("Please enter a valid number of between 0 and 365 days.");
            }

         
        }


    }
}