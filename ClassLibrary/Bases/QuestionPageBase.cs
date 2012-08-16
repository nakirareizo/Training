using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingRequisition.ClassLibrary.Bases
{
    public class QuestionPageBase: System.Web.UI.Page
    {
         public List<QuestionGroup> QuestionGroups
        {
            get
            { return (List<QuestionGroup>)ViewState["QuestionGroups"]; }
            set
            { ViewState["QuestionGroups"] = value; }
        }

        protected List<QuestionGroup> LoadQuestionGroups(string[] groupNames)
        {
            List<QuestionGroup> output = new List<QuestionGroup>();
            foreach (string groupName in groupNames)
            {
                QuestionGroup group = QuestionGroup.GetByName(groupName);
                group.LoadQuestions();
                output.Add(group);
            }
            return output;
        }

        protected static void AttachAnswersToQuestions(List<QuestionGroup> groups, List<AnswerBase> answers)
        {
            foreach (AnswerBase answer in answers)
            {
                foreach (QuestionGroup group in groups)
                {
                    AssessmentQuestion question = group.FindQuestionById(answer.QuestionId);
                    if (question != null)
                    {
                        question.AnswerValue = answer.Value;
                        break;
                    }
                }
            }
        }
    }
}
