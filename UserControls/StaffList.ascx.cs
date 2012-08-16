using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.UserControls
{

    public partial class StaffList : StaffListControlBase
    {
        public bool ShowSelectButton { get; set; }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowStaff(pnlSelectStaff, dlStaff, GetPreselectedUsername(Page));
                btnSelect.Visible = true;
            }
        }

        /// <summary>
        /// This only works properly if the ShowSelectButton property has been set to False.
        /// </summary>
        public string SelectedStaffUsername
        {
            get
            {
                if (string.IsNullOrEmpty(dlStaff.SelectedValue))
                    return Page.User.Identity.Name;
                else return dlStaff.SelectedValue;
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            // raise an event with the StaffUsername as the parameter
            OnSelectStaff(dlStaff);
        }

    }
}