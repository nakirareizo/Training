using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Assessments.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.Assessments.UserControls;
using TrainingRequisition.Classes;
using BUILD.Training.ClassLibrary.Custom;

namespace TrainingRequisition.Assessments
{
    public partial class CourseEvaluationForm : QuestionPageBase
    {
        private const string KeyEventDateId = "ED";
        public int EventDateId
        {
            get
            {
                return (int)ViewState["EventDateId"];
            }
            set
            {
                ViewState["EventDateId"] = value;
            }
        }

      

        protected void Page_Load(object sender, EventArgs e)
        {
            uscEventList.SelectEvent += new Assessments.UserControls.SelectEventHandler(uscEventList_SelectEvent);
            uscTabs.OnSaveAnswers += new QuestionGroupTabs.SaveAnswersHandler(uscTabs_OnSaveAnswers);
            uscTabs.OnSubmitAnswers += new QuestionGroupTabs.SubmitAnswersHandler(uscTabs_OnSubmitAnswers);
            if (!IsPostBack)
            {
                if (Request.QueryString[KeyEventDateId] != null)
                {
                    uscEventList.Visible = false;
                    EventDateId = Convert.ToInt32(Request.QueryString[KeyEventDateId]);
                    LoadAndShowQuestions();
                }
                else
                {
                    uscEventList.ShowEvents(User.Identity.Name);
                }

            }
        }

        void uscTabs_OnSubmitAnswers(object sender, QuestionGroupTabs.SaveAnswersEventArgs args)
        {
            string sScript0 = string.Empty;
            List<AnswerCEF> answers = Save(args.Answers);
            // TODO: send to SAP then update database to indicate that is has been submitted
            string trxId = Request.QueryString["ID"].ToString();
            string errorMessage = "";
            string Username = User.Identity.Name;
            EventDate ed = EventDate.GetById(EventDateId);
            TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
            string StaffUsername = User.Identity.Name;
            string SuperUsername = string.Empty;
            List<AnswerBase> baseAnswers = Utility.ConvertListToParent<AnswerBase, AnswerCEF>(answers);
            List<QuestionGroup> questionGroups = QuestionGroups;
            AttachAnswersToQuestions(questionGroups, baseAnswers);

            if (SAPHeitechREQ.SendTrainingSAP(SAPHeitechREQ.executeMode.CourseEvaluations, SuperUsername, StaffUsername,ev, ed.StartDate, ed.EndDate, ed.Id, ed.Provider, questionGroups, trxId, ref errorMessage))
            {
                //succeed
                AnswerCEF.RegisterSubmitted(User.Identity.Name, EventDateId);
                uscEventList.ShowEvents(User.Identity.Name);
                uscTabs.Visible = false;
                sScript0 = "window.alert('Course Evaluations has been Submitted. ');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "OnSubmit-SUCCESS", sScript0, true);
                return;
                
            }
            //FAIL
            sScript0 = "window.alert('" + errorMessage + "');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "btnApproved_Click-SUCCESS", sScript0, true);
            return;
        }

        void uscTabs_OnSaveAnswers(object sender, QuestionGroupTabs.SaveAnswersEventArgs args)
        {
            Save(args.Answers);
            LoadAndShowQuestions();
        }

        private List<AnswerCEF> Save(List<AnswerBase> baseAnswers)
        {
            List<AnswerCEF> answers = new List<AnswerCEF>();
            foreach (AnswerBase baseAnswer in baseAnswers)
            {
                AnswerCEF answer = new AnswerCEF();
                answer.Copy(baseAnswer);
                answer.EventDateId = EventDateId;
                answers.Add(answer);
            }

            AnswerCEF.DeleteAll(User.Identity.Name, EventDateId);
            AnswerCEF.Save(answers);
            return answers;
        }

        void uscEventList_SelectEvent(object sender, SelectEventArgs args)
        {
            BookedEvent be = BookedEvent.GetByEventDateId(User.Identity.Name, args.EventDateId);
            EventDateId = args.EventDateId;
            LoadAndShowQuestions();
        }

        private void LoadAndShowQuestions()
        {
            string[] groupNames = { "CEF_Sect1", "CEF_Sect2", "CEF_Sect3", "CEF_Sect4" };
            List<QuestionGroup> groups = LoadQuestionGroups(groupNames);
            QuestionGroups = groups;


            // fetch existing answers
            List<AnswerCEF> answers = AnswerCEF.GetAll(User.Identity.Name, EventDateId);
            List<AnswerBase> answersBase = Utility.ConvertListToParent<AnswerBase, AnswerCEF>(answers);
            AttachAnswersToQuestions(groups, answersBase);

            uscTabs.ShowQuestions(groups);
        }

        protected static void AttachAnswersToQuestions(List<QuestionGroup> groups, List<AnswerBase> answers)
        {
            foreach (AnswerBase answer in answers)
            {
                foreach (QuestionGroup group in groups)
                {
                    TrainingRequisition.ClassLibrary.Bases.AssessmentQuestion question = group.FindQuestionById(answer.QuestionId);
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
