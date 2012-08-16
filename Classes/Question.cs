using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Classes
{
    public class Question : Base
    {
        public string Text { get; set; }
        public override string DisplayName { get { return Text; } }

        public static List<Question> GetAll()
        {
            List<Question> output = new List<Question>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_Questions ORDER BY ID";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    Question newQuestion = new Question();
                    newQuestion.LoadFromReader(dr);
                    output.Add(newQuestion);
                }
            }
            return output;
        }

        public override void LoadFromReader(SqlDataReader dr)
        {
            base.LoadFromReader(dr);
            Text = dr["Text"].ToString();
        }
    }

}
