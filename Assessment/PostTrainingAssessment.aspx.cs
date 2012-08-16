using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Assessments.Classes;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.ClassLibrary.Utilities;
using BUILD.Training.ClassLibrary.Custom;
using TrainingRequisition.Classes;

namespace TrainingRequisition.Assessments
{
    public partial class PostTrainingAssessment : QuestionPageBase
    {
        private const string KeyEventDateId = "EventDateId";
        private const string KeyStaffUsername = "StaffUsername";

        public string StaffUsername
        {
            get { return ViewState[KeyStaffUsername].ToString(); }
            set { ViewState[KeyStaffUsername] = value; }
        }

        public int EventDateId
        {
            get { return Convert.ToInt32(ViewState[KeyEventDateId].ToString()); }
            set { ViewState[KeyEventDateId] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            uscStaffList.SelectStaff += new Assessments.UserControls.SelectStaffHandler(uscStaffList_SelectStaff);
            uscEventList.SelectEvent += new Assessments.UserControls.SelectEventHandler(uscEventList_SelectEvent);
            uscTabs.OnSaveAnswers += new Assessments.UserControls.QuestionGroupTabs.SaveAnswersHandler(uscTabs_OnSaveAnswers);
            uscTabs.OnSubmitAnswers += new Assessments.UserControls.QuestionGroupTabs.SubmitAnswersHandler(uscTabs_OnSubmitAnswers);
            if (!IsPostBack)
            {

            }
        }

        private List<AnswerPTA> Save(List<AnswerBase> baseAnswers)
        {
            List<AnswerPTA> answers = new List<AnswerPTA>();
            foreach (AnswerBase baseAnswer in baseAnswers)
            {
                AnswerPTA answer = new AnswerPTA();
                answer.Copy(baseAnswer);
                answer.EventDateId = EventDateId;
                answer.SupervisorUsername = User.Identity.Name;
                answer.Username = StaffUsername;
                answers.Add(answer);
            }

            AnswerPTA.DeleteAll(StaffUsername, EventDateId);
            AnswerPTA.Save(answers);
            return answers;
        }

        void uscTabs_OnSubmitAnswers(object sender, Assessments.UserControls.QuestionGroupTabs.SaveAnswersEventArgs args)
        {
           
            string sScript0 = string.Empty;
            List<AnswerPTA> answers = Save(args.Answers);
            // TODO: send to SAP then update database to indicate that is has been submitted
            string trxId = Request.QueryString["ID"].ToString();
            string errorMessage = "";
            EventDate ed = EventDate.GetById(EventDateId);
            TrainingEvent ev = TrainingEvent.GetById(ed.EventId);
            string supervisorusername = User.Identity.Name;
            List<AnswerBase> baseAnswers = Utility.ConvertListToParent<AnswerBase, AnswerPTA>(answers);
            List<QuestionGroup> questionGroups = QuestionGroups;
            AttachAnswersToQuestions(questionGroups, baseAnswers);

            if (SAPHeitechREQ.SendTrainingSAP(SAPHeitechREQ.executeMode.PostTraining, supervisorusername, StaffUsername,ev, ed.StartDate, ed.EndDate, ed.Id, ed.Provider, questionGroups, trxId, ref errorMessage))
            {
                AnswerPTA.RegisterSubmitted(StaffUsername, EventDateId);
                uscStaffList.ShowStaff();
                uscEventList.ShowEvents(null);
                uscEventList.Visible = false;
                uscTabs.Visible = false;
               
                
                //succeed
                sScript0 = "window.alert('Post-Training Assessment has been Submitted. ');";
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "OnSubmit-SUCCESS", sScript0, true);
                return;
            }

            //FAIL
            sScript0 = "window.alert('" + errorMessage + "');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "btnApproved_Click-SUCCESS", sScript0, true);
            return;
           
        }

        void uscTabs_OnSaveAnswers(object sender, Assessments.UserControls.QuestionGroupTabs.SaveAnswersEventArgs args)
        {
            Save(args.Answers);
            LoadAndShowQuestions(StaffUsername, EventDateId);
        }

        void uscEventList_SelectEvent(object sender, Assessments.UserControls.SelectEventArgs args)
        {
            LoadAndShowQuestions(args.StaffUsername, args.EventDateId);
        }

        void uscStaffList_SelectStaff(object sender, Assessments.UserControls.SelectStaffEventArgs args)
        {
            uscEventList.ShowEvents(args.StaffUsername);
            uscTabs.Visible = false;
        }

        private void LoadAndShowQuestions(string staffUsername, int eventDateId)
        {
            EventDateId = eventDateId;
            StaffUsername = staffUsername;

            string[] groupNames = { "PTA_Sect1"};
            List<QuestionGroup> groups = LoadQuestionGroups(groupNames);
            QuestionGroups = groups;

            List<AnswerPTA> answers = AnswerPTA.GetAll(staffUsername, EventDateId);
            List<AnswerBase> answersBase = Utility.ConvertListToParent<AnswerBase, AnswerPTA>(answers);
            AttachAnswersToQuestions(groups, answersBase);

            uscTabs.ShowQuestions(groups);

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
