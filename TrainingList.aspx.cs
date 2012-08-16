using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BUILD.Training.Classes;

namespace BUILD.Training
{
    public partial class TrainingList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string StaffUsername = Page.User.Identity.Name;
                List<Listing> TrainingLst = Listing.getAllbyUsername(StaffUsername);
                gvTrainingList.DataSource = TrainingLst;
                gvTrainingList.DataBind();
                this.lblTotListedVal.Text = TrainingLst.Count.ToString();
            }
        }
    }
}