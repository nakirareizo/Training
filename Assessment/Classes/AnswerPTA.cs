using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Assessments.Classes
{
    public class AnswerPTA:AnswerBase
    {
        public int EventDateId { get; set; }
        public string SupervisorUsername { get; set; }
        public void LoadFromReader(SqlDataReader dr)
        {
            EventDateId = (int)dr["EventDateId"];
            SupervisorUsername = dr["SupervisorUsername"].ToString();
            base.LoadFromReader(dr);
        }

        public void Save(DataRow row)
        {
            base.Save(row);
            row["EventDateId"] = EventDateId;
            row["SupervisorUsername"] = SupervisorUsername;
        }

        internal static List<AnswerPTA> GetAll(string username, int EventDateId)
        {
            List<AnswerPTA> output = new List<AnswerPTA>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM ASM_AnswersPTA WHERE Username='{0}' AND EventDateId={1}",
                    username, EventDateId);

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AnswerPTA ans = new AnswerPTA();
                    ans.LoadFromReader(dr);
                    output.Add(ans);
                }
            }
            return output;
        }

        internal static void Save(List<AnswerPTA> answers)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                DataRow row = null;
                db.PrepareInsert("ASM_AnswersPTA");
                foreach (AnswerPTA answer in answers)
                {
                    row = db.Insert(row);
                    answer.Save(row);
                }
                db.Insert(row);
                db.EndInsert();
            }
        }

        public static void RegisterSubmitted(string staffUsername, int EventDateId)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();

                // delete existing first
                string sql = "DELETE FROM ASM_SubmittedPTA WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND EventDateID = " + EventDateId;
                UtilityDb.ExecuteSql(sql, db.connection);

                db.PrepareInsert("ASM_SubmittedPTA");
                DataRow row = db.Insert(null);
                row["StaffUsername"] = staffUsername;
                row["EventDateId"] = EventDateId;
                db.Insert(row);
                db.EndInsert();
            }
        }

        internal static void DeleteAll(string username, int EventDateId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("DELETE FROM ASM_AnswersPTA WHERE Username='{0}' AND EventDateId={1}",
                    username, EventDateId);

                UtilityDb.ExecuteSql(sql, conn);
            }
        }
    }
}
