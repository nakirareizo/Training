using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using TrainingRequisition.ClassLibrary.Entities;
using System.Web.UI.WebControls;

namespace TrainingRequisition.ClassLibrary.Bases
{
    public class StaffListControlBase : UserControl
    {
        public class SelectStaffEventArgs : EventArgs
        {
            public string StaffUsername { get; set; }
        }

        const string PreselectedUsernameKey = "UN";

        protected string GetPreselectedUsername(Page page)
        {
            if (page.Request.QueryString[PreselectedUsernameKey] != null)
                return page.Request.QueryString[PreselectedUsernameKey].ToString();
            return "";
        }

        public delegate void SelectStaffHandler(object sender, SelectStaffEventArgs args);

        protected const string KeyCurrentStaff = "CurrentStaff";

        public Staff CurrentStaff
        {
            get
            {
                if (ViewState[KeyCurrentStaff] == null)
                    return null;
                return (Staff)ViewState[KeyCurrentStaff];
            }
            set
            {
                ViewState[KeyCurrentStaff] = value;
            }
        }

        public event SelectStaffHandler SelectStaff;

        protected void ShowStaff(Panel pnlSelectStaff, DropDownList dlStaff, string preselectedUsername)
        {
            string username = Page.User.Identity.Name;
            Staff currentStaff = Staff.GetFromUsername(username);
            CurrentStaff = currentStaff;

            bool isSupervisor = currentStaff.IsSupervisor();

            pnlSelectStaff.Visible = isSupervisor;

            if (isSupervisor)
            {
                ShowStaffUnderSupevisor(dlStaff, preselectedUsername);
            }
            else
            {
                SelectStaffEventArgs args = new SelectStaffEventArgs();
                args.StaffUsername = currentStaff.Username;
                SelectStaff(this, args);
            }

        }

        protected virtual void OnSelectStaff(DropDownList dlStaff)
        {
           if (SelectStaff != null)
           {
              SelectStaffEventArgs args = new SelectStaffEventArgs();
              args.StaffUsername = dlStaff.SelectedValue;
              SelectStaff(this, args);
           }
        }

        protected virtual void ShowStaffUnderSupevisor(DropDownList dlStaff, string preselectedUsername)
        {
            // show staff list
            List<Staff> staffList = Staff.GetStaffUnder(CurrentStaff.Username, false);

            // add himself
            staffList.Insert(0, CurrentStaff);

            dlStaff.DataSource = staffList;
            dlStaff.DataBind();

            PreselectStaff(dlStaff, preselectedUsername);

        }

        protected static void PreselectStaff(DropDownList dlStaff, string preselectedUsername)
        {
            if (!string.IsNullOrEmpty(preselectedUsername))
            {
                foreach (ListItem item in dlStaff.Items)
                {
                    if (item.Value.ToUpper() == preselectedUsername.ToUpper())
                        item.Selected = true;
                    else
                        item.Selected = false;
                }
            }
        }


    }
}
