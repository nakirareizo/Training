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
    public class AnswerCEF:AnswerBase
    {
        public int EventDateId { get; set; }
        public void LoadFromReader(SqlDataReader dr)
        {
            EventDateId = (int)dr["EventDateId"];
            base.LoadFromReader(dr);
        }

        public void Save(DataRow row)
        {
            base.Save(row);
            row["EventDateId"] = EventDateId;
        }

        internal static List<AnswerCEF> GetAll(string username, int eventDateId)
        {
            List<AnswerCEF> output = new List<AnswerCEF>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM ASM_AnswersCEF WHERE Username='{0}' AND eventDateId={1}",
                    username, eventDateId);

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AnswerCEF ans = new AnswerCEF();
                    ans.LoadFromReader(dr);
                    output.Add(ans);
                }
            }
            return output;
        }

        public static void Save(List<AnswerCEF> answers)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                DataRow row = null;
                db.PrepareInsert("ASM_AnswersCEF");
                foreach (AnswerCEF answer in answers)
                {
                    row = db.Insert(row);
                    answer.Save(row);
                }
                db.Insert(row);
                db.EndInsert();
            }
        }

        internal static void DeleteAll(string username, int eventDateId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("DELETE FROM ASM_AnswersCEF WHERE Username='{0}' AND EventDateId={1}",
                    username, eventDateId);

                UtilityDb.ExecuteSql(sql, conn);
            }
        }

        public static void RegisterSubmitted(string staffUsername, int EventDateId)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();

                // delete existing first
                string sql = "DELETE FROM ASM_SubmittedCEF WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND EventDateID = " + EventDateId;
                UtilityDb.ExecuteSql(sql, db.connection);

                db.PrepareInsert("ASM_SubmittedCEF");
                DataRow row = db.Insert(null);
                row["StaffUsername"] = staffUsername;
                row["EventDateId"] = EventDateId;
                db.Insert(row);
                db.EndInsert();
            }
        }
    }
}
