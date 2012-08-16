using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Assessments.Classes;
using AjaxControlToolkit;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Assessments.UserControls
{
    public partial class QuestionGroupTabs : System.Web.UI.UserControl
    {
        public class SaveAnswersEventArgs : EventArgs
        {
            public List<AnswerBase> Answers { get; set; }
        }

        public delegate void SaveAnswersHandler(object sender, SaveAnswersEventArgs args);
        public delegate void SubmitAnswersHandler(object sender, SaveAnswersEventArgs args);

        public event SaveAnswersHandler OnSaveAnswers;
        public event SubmitAnswersHandler OnSubmitAnswers;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ShowQuestions(List<QuestionGroup> groups)
        {
            Visible = groups.Count > 0;
            int i = 0;
            foreach (QuestionGroup group in groups)
            {
                TabPanel tab = tcTabs.Tabs[i];
                tab.HeaderText = group.DisplayName;
                tab.Visible = true;

                QuestionGroupCtrl rg =
                    (QuestionGroupCtrl)tab.FindControl("QuestionGroup" + i.ToString());
                rg.ShowQuestions(group, 5, false);

                i++;
            }

            // make the rest of the table invisible
            for (int j = i; j < tcTabs.Tabs.Count; j++)
            {
                tcTabs.Tabs[j].Visible = false;
            }
        }

        internal List<AnswerBase> GetAnswers()
        {
            List<AnswerBase> output = new List<AnswerBase>();
            for (int iTab = 0; iTab < tcTabs.Tabs.Count; iTab++)
            {
                TabPanel tab = tcTabs.Tabs[iTab];
                if (tab.Visible)
                {
                    QuestionGroupCtrl questionGroup =
                        (QuestionGroupCtrl)tab.FindControl("QuestionGroup" +
                        iTab.ToString());

                    List<AnswerBase> answers = questionGroup.GetAnswers();
                    output.AddRange(answers);
                }
            }
            return output;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (OnSaveAnswers == null)
                return;

            SaveAnswersEventArgs args = new SaveAnswersEventArgs();
            args.Answers = GetAnswers();
            OnSaveAnswers(this, args);
            string sScript0 = "window.alert('Current process saved.');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ApprovalStaffList.ascx", sScript0, true);
            return;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveAnswersEventArgs args = new SaveAnswersEventArgs();
            args.Answers = GetAnswers();

            if (!ValidateAnswers(args.Answers))
                return;

            if (OnSaveAnswers == null)
                return;

            OnSubmitAnswers(this, args);
        }

        private bool ValidateAnswers(List<AnswerBase> answers)
        {
            List<int> badQuestionIDs = new List<int>();
            bool hasBlankAnswers = false;

            // mark the bad questions
            for (int i = 0; i < tcTabs.Tabs.Count; i++)
            {
                if (!tcTabs.Tabs[i].Visible)
                    continue;

                TabPanel tab = tcTabs.Tabs[i];
                QuestionGroupCtrl rg =
                    (QuestionGroupCtrl)tab.FindControl("QuestionGroup" + i.ToString());
                if (rg.MarkBadQuestions())
                    hasBlankAnswers = true;
            }
            lblMustAnswer.Visible = hasBlankAnswers;

            return !hasBlankAnswers;
        }
    }
}