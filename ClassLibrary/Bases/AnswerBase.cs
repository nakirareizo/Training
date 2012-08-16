using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace TrainingRequisition.ClassLibrary.Bases
{
    public class AnswerBase
    {
        public int QuestionId { get; set; }
        public string Username { get; set; }
        public AssessmentQuestion.AnswerTypes AnswerType { get; set; }
        public string Value { get; set; }

        public void LoadFromReader(SqlDataReader dr)
        {
            QuestionId = (int)dr["QuestionId"];
            Username = dr["Username"].ToString();

            string strAnswerType = dr["AnswerType"].ToString();
            AnswerType = (AssessmentQuestion.AnswerTypes)Enum.Parse(typeof(AssessmentQuestion.AnswerTypes), strAnswerType, true);
            Value= dr["Value"].ToString();
        }

        public void Copy(AnswerBase source)
        {
            QuestionId = source.QuestionId;
            Username = source.Username;
            AnswerType = source.AnswerType;
            Value = source.Value;
        }

        protected void Save(DataRow row)
        {
            row["QuestionID"] = QuestionId;
            row["Username"] = Username;
            row["AnswerType"] = AnswerType;
            row["Value"] = Value;
            
        }
    }
}
