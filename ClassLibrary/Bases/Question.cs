using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.ClassLibrary.Bases
{
    [Serializable]
    public class AssessmentQuestion: Arrangeable
    {
        public enum AnswerTypes
        {
            R,
            T
        }
        //public int? ID { get; set; }
        public int? GroupId { get; set; }
        public int SAPCounter { get; set; }
        public float Weight { get; set; }
        public QuestionGroup Group { get; set; }
        public AnswerTypes AnswerType { get; set; }
        public string AnswerValue { get; set; }
        public bool Mandatory { get; set; }
        public string Title { get; set; }
        public override string DisplayName
        {
            get { return Title; }
        }

        public override void LoadFromReader(System.Data.SqlClient.SqlDataReader dr)
        {
            if (dr["GroupID"] != DBNull.Value)
                GroupId = Convert.ToInt32(dr["GroupID"]);
            string strAnswerType = dr["AnswerType"].ToString();
            AnswerType = (AnswerTypes)Enum.Parse(typeof(AnswerTypes), strAnswerType, true);
            Title = dr["Title"].ToString();
            Mandatory = Convert.ToBoolean(dr["Mandatory"]);
            SAPCounter = Convert.ToInt32(dr["SAPCounter"]);
            Weight = Convert.ToSingle(dr["Weight"]);
            base.LoadFromReader(dr);   
        }

        public static List<AssessmentQuestion> GetAll()
        {
            return new List<AssessmentQuestion>();
        }

        public bool ValidateAnswer(AnswerBase answer)
        {
            if (!Mandatory)
                return true; // if not mandatory always return true

            bool isBlankAnswer = false;
            if (answer.AnswerType == AssessmentQuestion.AnswerTypes.R &&
                answer.Value == "0")
                isBlankAnswer = true;
            else if (answer.AnswerType == AssessmentQuestion.AnswerTypes.T &&
                string.IsNullOrEmpty(answer.Value))
                isBlankAnswer = true;
            return !isBlankAnswer;
        }

        public override void Save(System.Data.DataRow row)
        {
            row["GroupID"] = GroupId;
            row["DisplayOrder"] = SAPCounter;
            row["AnswerType"] = AnswerType;
            row["Title"] = Title;
            row["Mandatory"] = Mandatory;
            row["SAPCounter"] = SAPCounter;
            row["Weight"] = Weight;
            base.Save(row);
        }
    }

    
}
