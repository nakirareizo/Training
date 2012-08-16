using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BUILD.Training.ClassLibrary.Custom;
using System.Configuration;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;

namespace BUILD.Training.SuperAdmin
{
    public partial class ImportTrainingModel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnImportModel_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ResetTable("ASM_QuestionGroups", conn);
                UtilityDb.ResetTable("ASM_Questions", conn);
            }
            string trxId = Request.QueryString["ID"].ToString();
            bool Import = true;
            List<string> TrainingModel=new List<string>();
            TrainingModel.Add(ConfigurationManager.AppSettings["REQCourseEvaluationID"].ToString());
            TrainingModel.Add(ConfigurationManager.AppSettings["REQPostID"].ToString());
            foreach (string TrainingModelID in TrainingModel)
            {
                if (ConfigurationManager.AppSettings["REQCourseEvaluationID"].ToString()==TrainingModelID)
                {
                    Import = SAPHeitechREQ.ImportTrainingModel(SAPHeitechREQ.executeMode.CourseEvaluations, TrainingModelID, User.Identity.Name, trxId);
                }
                if (ConfigurationManager.AppSettings["REQPostID"].ToString() == TrainingModelID)
                {
                    Import = SAPHeitechREQ.ImportTrainingModel(SAPHeitechREQ.executeMode.PostTraining, TrainingModelID, User.Identity.Name, trxId);
                }
            }
            if (Import)
            {
                string sScript0 = "window.alert('Training model imported.');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "SaveDraft_Script_20", sScript0, true);
                return;
            }
        }
    }
}