using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.Assessments.Classes
{
    public class SubmittedEvent : Base
    {
        public int EventDateId { get; set; }
        public string StaffUsername { get; set; }
        public override string DisplayName { get { return EventDateId.ToString() + "/" + StaffUsername; } }

        public override void LoadFromReader(SqlDataReader dr)
        {
            EventDateId = Convert.ToInt32(dr["EventDateID"]);
            StaffUsername = dr["StaffUsername"].ToString();
            base.LoadFromReader(dr);
        }

        public static List<SubmittedEvent> GetAll(string staffUsername, int eventDateId, string moduleName)
        {
            List<SubmittedEvent> output = new List<SubmittedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM ASM_Submitted" + moduleName + " WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND EventDateID=" + eventDateId.ToString();
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    SubmittedEvent se = new SubmittedEvent();
                    se.LoadFromReader(dr);
                    output.Add(se);
                }
            }
            return output;
        }
    }
}
