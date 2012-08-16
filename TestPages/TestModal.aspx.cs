using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrainingRequisition.TestPages
{
    public partial class TestModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox1.Text = "test" + DateTime.Now.ToShortTimeString();
        }
    }
}
