using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.ClassLibrary.Bases
{
    [Serializable]
    public class QuestionGroup: Arrangeable
    {
        int DisplayOrder { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public int SAPCounter { get; set; }
        public float Weight { get; set; }

        public override string DisplayName
        {
            get { return Title; }
        }

        public float Rating()
        {
            float groupTotal = 0;
            int questionCount = 0;
            foreach (AssessmentQuestion question in Questions)
            {
                if (question.AnswerType == AssessmentQuestion.AnswerTypes.R)
                {
                    questionCount++;
                    groupTotal = groupTotal + (Convert.ToInt32(question.AnswerValue) * question.Weight);
                }
            }

            if (questionCount > 0)
            {
                float rating = groupTotal / questionCount;
                return rating;
            }

            return 0;

        }

        public override void LoadFromReader(System.Data.SqlClient.SqlDataReader dr)
        {
            Name = dr["Name"].ToString();
            if (dr["Title"] != DBNull.Value)
                Title = dr["Title"].ToString();
            SAPCounter = Convert.ToInt32(dr["SAPCounter"]);
            Weight = Convert.ToSingle(dr["Weight"]);
            base.LoadFromReader(dr);
        }


        public static QuestionGroup GetByName(string name)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM ASM_QuestionGroups WHERE Name='" +
                    name + "'";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    QuestionGroup group = new QuestionGroup();
                    group.LoadFromReader(dr);
                    return group;
                }
            }
            return null;
        }

        public void LoadQuestions()
        {
            Questions = new List<AssessmentQuestion>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM ASM_Questions WHERE GroupID=" +
                    Id + " ORDER BY DisplayOrder";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AssessmentQuestion question = new AssessmentQuestion();
                    question.LoadFromReader(dr);
                    question.Group = this;
                    Questions.Add(question);
                }
            }
        }

        public AssessmentQuestion FindQuestionById(int questionId)
        {
            foreach (AssessmentQuestion question in Questions)
            {
                if (question.Id == questionId)
                    return question;
            }
            return null;
        }

        public override void Save(System.Data.DataRow row)
        {
            row["Name"] = Name;
            row["Title"] = Title;
            row["DisplayOrder"] = DisplayOrder;
            row["SAPCounter"] = SAPCounter;
            row["Weight"] = Weight;
            base.Save(row);
        }
    }
}
