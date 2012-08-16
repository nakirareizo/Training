using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Assessments.Classes;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.Assessments.UserControls
{
    public partial class QuestionGroupCtrl : System.Web.UI.UserControl
    {
        private const int ColumnRatings = 2;
        private const int ColumnTextAnswer = 1;
        private const int ColumnID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        public int MaxRating
        {
            get
            {
                return Convert.ToInt32(ViewState["MaxRating"]);
            }
            set
            {
                ViewState["MaxRating"] = value;
            }
        }

        public bool HideRatingColumn
        {
            get
            {
                return Convert.ToBoolean(ViewState["HideRatingColumn"]);
            }
            set
            {
                ViewState["HideRatingColumn"] = value;
            }
        }

        public List<AssessmentQuestion> Questions
        {
            get
            {
                return UtilityUI.GetListFromViewState<AssessmentQuestion>("Questions", ViewState);
            }
            set
            {
                ViewState["Questions"] = value;
            }
        }


        internal void ShowQuestions(QuestionGroup group, int maxRating, bool showTitle)
        {
            if (showTitle)
                lblTitle.Text = group.DisplayName;
            lblTitle.Visible = showTitle;

            MaxRating = maxRating;
            gvQuestions.DataSource = group.Questions;
            Questions = group.Questions;

            HideRatingColumn = true;
            gvQuestions.DataBind();
            if (HideRatingColumn)
                gvQuestions.Columns[ColumnRatings].Visible = false;

        }

        protected void gvQuestions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            AssessmentQuestion question = (AssessmentQuestion)e.Row.DataItem;
            if (question == null)
                return;
            if (question.AnswerType == AssessmentQuestion.AnswerTypes.R)
                DatabindRatingQuestion(e, question);
            else if (question.AnswerType == AssessmentQuestion.AnswerTypes.T)
                DatabindTextQuestion(e, question);
        }

        private void DatabindTextQuestion(GridViewRowEventArgs e, AssessmentQuestion question)
        {
            TextBox txtAnswer = (TextBox)e.Row.Cells[ColumnTextAnswer].FindControl("txtAnswer");
            if (txtAnswer == null)
                return;

            txtAnswer.Text = question.AnswerValue;
            RadioButtonList rb = (RadioButtonList)e.Row.Cells[ColumnRatings].FindControl("rbRatings");
            if (rb != null)
                rb.Visible = false;
        }

        private void DatabindRatingQuestion(GridViewRowEventArgs e, AssessmentQuestion question)
        {
            RadioButtonList rbRatings = (RadioButtonList)e.Row.Cells[ColumnRatings].FindControl("rbRatings");
            if (rbRatings == null)
                return;

            List<Rating> ratings = new List<Rating>();
            for (int i = 0; i <= MaxRating; i++)
            {
                Rating obj = new Rating();
                obj.Value = i;
                ratings.Add(obj);
            }
            rbRatings.DataSource = ratings;
            rbRatings.DataBind();

            // select the answer
            int currentRating = 0;
            int.TryParse(question.AnswerValue, out currentRating);
            rbRatings.SelectedIndex = currentRating;

            TextBox txtAnswer = (TextBox)e.Row.Cells[ColumnTextAnswer].FindControl("txtAnswer");
            if (txtAnswer != null)
                txtAnswer.Visible = false;

            HideRatingColumn = false;
            return;
        }

        protected void gvQuestions_DataBound(object sender, EventArgs e)
        {

        }

        internal List<AnswerBase> GetAnswers()
        {
            List<AnswerBase> output = new List<AnswerBase>();
            foreach (GridViewRow row in gvQuestions.Rows)
            {
                AnswerBase answer = new AnswerBase();
                answer.QuestionId = Convert.ToInt32(gvQuestions.DataKeys[row.RowIndex].Value.ToString());

                RadioButtonList rbRatings = (RadioButtonList)
                    row.Cells[ColumnRatings].FindControl("rbRatings");
                if (rbRatings.Visible)
                {
                    answer.AnswerType = AssessmentQuestion.AnswerTypes.R;
                    answer.Value = rbRatings.SelectedValue.ToString();
                }
                else
                {
                    answer.AnswerType = AssessmentQuestion.AnswerTypes.T;
                    TextBox txt =
                        (TextBox)row.Cells[ColumnTextAnswer].FindControl("txtAnswer");
                    answer.Value = txt.Text;
                }

                answer.Username = Page.User.Identity.Name;
                output.Add(answer);
            }
            return output;
        }

        public bool MarkBadQuestions()
        {
            bool output = false; // return true if there are bad questions

            // reset all first
            foreach (GridViewRow row in gvQuestions.Rows)
            {
                Label lblAsterisk = (Label)row.Cells[ColumnTextAnswer].FindControl("lblAsterisk");
                lblAsterisk.Visible = false;
            }

            // gather the bad questions
            List<AnswerBase> answers = GetAnswers();
            List<AssessmentQuestion> questions = Questions;
            foreach (AnswerBase answer in answers)
            {
                // find the question
                foreach (AssessmentQuestion question in questions)
                {
                    if (question.Id == answer.QuestionId && !question.ValidateAnswer(answer))
                    {
                        output = true;
                        int rowNum = 0;
                        foreach (GridViewRow row in gvQuestions.Rows)
                        {
                            Label lblAsterisk = (Label)row.Cells[ColumnTextAnswer].FindControl("lblAsterisk");
                            int rowQuestionId = Convert.ToInt32(gvQuestions.DataKeys[rowNum++].Value.ToString());
                            if (rowQuestionId == question.Id)
                            {
                                lblAsterisk.Visible = true;
                                break;
                            }
                        }
                    }

                }
            }
            return output;

        }
    }
}