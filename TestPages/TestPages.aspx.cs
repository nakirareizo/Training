using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.TestPages
{
    public partial class Default : System.Web.UI.Page
    {
        private const int eventMax = 50;
        private const int eventDateMax = eventMax * 5;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        protected void btnResetEvents_Click(object sender, EventArgs e)
        {
            // clear the database
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ResetTable("REQ_EventsInGroups", conn);
                UtilityDb.ResetTable("REQ_BookedEvents", conn);
                UtilityDb.ResetTable("REQ_PrebookedEvents", conn);
                UtilityDb.ResetTable("REQ_Notes", conn);
                UtilityDb.ResetTable("REQ_SupervisorRatings", conn);
                UtilityDb.ResetTable("REQ_EventDates", conn);
                UtilityDb.ResetTable("REQ_Events", conn);
                UtilityDb.ResetTable("REQ_EventGroups", conn);
                UtilityDb.ResetTable("REQ_AttendanceToConfirm", conn);
                UtilityDb.ResetTable("REQ_AttendedEvents", conn);
                UtilityDb.ResetTable("REQ_Questions", conn);

                UtilityDb.ResetTable("ASM_AnswersCEF", conn);
                UtilityDb.ResetTable("ASM_AnswersPTA", conn);
                UtilityDb.ResetTable("ASM_SubmittedCEF", conn);
                UtilityDb.ResetTable("ASM_SubmittedPTA", conn);

            }

            // insert questions
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_Questions");
                DataRow row = db.Insert(null);
                row["Text"] = "Knowledge on the course subject matter";
                row = db.Insert(row);
                row["Text"] = "Ability to apply relevant skills from the course subject matter";
                row = db.Insert(row);
                row["Text"] = "Ability to deliver significant assignment/s and/or demonstrate understanding of the course subject matter in the work area";
                row = db.Insert(row);
                row["Text"] = "Ability to share and impart knowledge of the subject matter with others";
                db.Insert(row);
                db.EndInsert();
            }

            // insert event groups
            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_EventGroups");
                for (int eventGroupId = 1; eventGroupId <= 5; eventGroupId++)
                {
                    row = db.Insert(row);
                    row["Title"] = "EventGroup " + eventGroupId.ToString();
                    row["ID"] = eventGroupId;
                }
                db.Insert(row);
                db.EndInsert();
            }

            // insert sub groups
            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_EventGroups");
                int id = 6;
                for (int eventGroupId = 1; eventGroupId <= 5; eventGroupId++)
                {
                    for (int subGroupId = 1; subGroupId <= 5; subGroupId++)
                    {
                        row = db.Insert(row);
                        row["Title"] = "SubGroup " + eventGroupId.ToString() + "/" +
                            subGroupId.ToString();
                        row["ID"] = id;
                        row["ParentId"] = eventGroupId;
                        id++;
                    }

                }
                db.Insert(row);
                db.EndInsert();
            }

            // insert events
            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_Events");
                for (int eventId = 1; eventId <= eventMax; eventId++)
                {
                    row = db.Insert(row);
                    row["Title"] = "Event " + eventId.ToString();
                    row["ID"] = eventId;
                    row["SAP_ID"] = 10000 + eventId;
                    row["UserDefined"] = false;
                }
                db.Insert(row);
                db.EndInsert();
            }

            // insert event dates
            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_EventDates");
                for (int eventId = 1; eventId <= eventMax; eventId++)
                {
                    for (int eventDateId = 1; eventDateId <= 5; eventDateId++)
                    {
                        row = db.Insert(row);
                        row["EventID"] = eventId;
                        row["ID"] = eventDateId + eventId * 100;

                        DateTime startDate = DateTime.Today.AddDays(3 + eventDateId * 10);
                        row["StartDate"] = startDate;

                        DateTime endDate = startDate.AddDays(5);
                        row["EndDate"] = endDate;

                        row["Provider"] = "Provider " + eventDateId.ToString();
                        row["Currency"] = "USD";
                        row["Price"] = eventDateId * 1000;
                        row["TrainingType"] = "Functional";

                        row["SAP_ID"] = 10000 + (eventId + eventDateId);
                    }
                }
                db.Insert(row);
                db.EndInsert();
            }

            // insert events in groups
            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_EventsInGroups");
                Random rand = new Random();
                for (int groupId = 6; groupId <= 30; groupId++) // start from subgroups%
                {
                    for (int eventId = 1; eventId <= eventMax; eventId++)
                    {
                        // insert or not?
                        int rndNum = rand.Next();
                        if (rndNum % 10 != 0)
                            continue;

                        row = db.Insert(row);
                        row["EventID"] = eventId;
                        row["GroupID"] = groupId;
                    }
                }
                db.Insert(row);
                db.EndInsert();
            }
        }

        protected void btnResetConfirmAttendance_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ResetTable("REQ_AttendanceToConfirm", conn);
            }

            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_AttendanceToConfirm");
                for (int eventId = 1; eventId <= eventMax; eventId++)
                {
                    row = db.Insert(row);
                    row["EventID"] = eventId;
                    row["StaffUsername"] = User.Identity.Name;
                }
                db.Insert(row);
                db.EndInsert();
            }

        }

        protected void btnResetAttendedEvents_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ResetTable("REQ_AttendedEvents", conn);
            }

            using (UtilityDb db = new UtilityDb())
            {
                DataRow row = null;
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_AttendedEvents");
                for (int eventDateId = 1; eventDateId <= 1; eventDateId++)
                {
                    row = db.Insert(row);
                    row["eventDateID"] = eventDateId;
                    row["StaffUsername"] = User.Identity.Name;
                }
                db.Insert(row);
                db.EndInsert();
            }
        }

        protected void btnResetCEF_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ResetTable("ASM_QuestionGroups", conn);
                UtilityDb.ResetTable("ASM_Questions", conn);
            }

            // insert question groups
            InsertCEF();
            InsertPTA();


        }

        private void InsertCEF()
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("ASM_QuestionGroups");

                Dictionary<int, int> groupSAPIDs = new Dictionary<int, int>();
                groupSAPIDs[1] = 50000406;
                groupSAPIDs[2] = 50000416;
                groupSAPIDs[3] = 50000421;
                groupSAPIDs[4] = 50000426;

                Dictionary<int, int> groupSAPCounters = new Dictionary<int, int>();
                groupSAPCounters[1] = 0002;
                groupSAPCounters[2] = 0012;
                groupSAPCounters[3] = 0017;
                groupSAPCounters[4] = 0022;

                Dictionary<int, string> groupTitles = new Dictionary<int, string>();
                groupTitles[1] = "Trainer";
                groupTitles[2] = "Course Objectives";
                groupTitles[3] = "Course Material & Facilities";
                groupTitles[4] = "General";
                

                for (int iGroup = 1; iGroup <= 4; iGroup++)
                {
                    DataRow row = db.Insert(null);
                    row["ID"] = groupSAPIDs[iGroup];
                    row["SAPCounter"] = groupSAPCounters[iGroup];
                    row["Name"] = "CEF_Sect" + iGroup;
                    row["Title"] = groupTitles[iGroup];
                    row["DisplayOrder"] = iGroup;
                    db.Insert(row);
                }
                db.EndInsert();

                // insert CEF questions
                db.PrepareInsert("ASM_Questions");
                for (int iGroup = 1; iGroup <= 4; iGroup++)
                {
                    switch (iGroup)
                    {
                        case 1:
                            InsertQuestions(groupSAPIDs[iGroup],0003, 50000407, true, "Trainer's knowledge on the subject matter", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0004, 50000408, true, "Ability to communicate and deliver presentation well", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0005, 50000409, true, "Ability to facilitate dialogues with participants", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0006, 50000410, true, "Ability to use suitable examples when giving explanation", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0007, 50000411, true, "Ability to visual the concept to be used in daily tasks", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0008, 50000412, true, "Ability to stay focused and attract attention of participants", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0009, 50000413, true, "Adequate time was provided for questions", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0010, 50000414, true, "Recommending this trainer to conduct this course in the future", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0011, 50000415, true, "Trainer overall performance", "R", db);
                            break;
                        case 2:
                            InsertQuestions(groupSAPIDs[iGroup],0013, 50000417, true, "How well the course achieved this objective(s)", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0014, 50000418, true, "The period of training is appropriate", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0015, 50000419, true, "This training is effective and should be conducted again in the future", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0016, 50000420, true, "How do you rate the training overall?", "R", db);
                            break;
                        case 3:
                            InsertQuestions(groupSAPIDs[iGroup],0018, 50000422, true, "The materials distributed were pertinent and useful", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0019, 50000423, true, "The content was organized and easy to follow", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0020, 50000424, true, "Good training aids and audio-visual aids were used", "R", db);
                            InsertQuestions(groupSAPIDs[iGroup],0021, 50000425, true, "Facilities in training room/hall", "R", db);
                            break;

                        case 4:
                            InsertQuestions(groupSAPIDs[iGroup],0023, 50000427, false, "What suggestion would you make on the content of the course to make it better?", "T", db);
                            InsertQuestions(groupSAPIDs[iGroup],0024, 50000428, false, "Did you encounter any problem during the course? If yes please elaborate", "T", db);
                            InsertQuestions(groupSAPIDs[iGroup],0025,50000429, false, "Suggestions/Other comments", "T", db);
                            break;
                    }
                }
                db.EndInsert();

            }
        }

        private void InsertPTA()
        {
            int BSID = 50000482;
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("ASM_QuestionGroups");

                DataRow row = db.Insert(null);
                row["ID"] = BSID;
                row["SAPCounter"] = 0001;
                row["Name"] = "PTA_Sect" + 1;
                row["Title"] = "Post-Training Assessment";
                row["DisplayOrder"] = 0;
                db.Insert(row);

                db.EndInsert();

                // insert PTA questions
                db.PrepareInsert("ASM_Questions");

                InsertQuestions(BSID,0003, 50000484, true, "Knowledge on the course subject matter", "R", db);
                InsertQuestions(BSID,0004, 50000485, true, "Ability to apply relevant skills from the course subject matter", "R", db);
                InsertQuestions(BSID,0005, 50000486, true, "Ability to deliver significant assignment/s and/or demonstrate understanding of the course subject matter in work area", "R", db);
                //InsertQuestions(BSID, 50000487, true, "Ability to use suitable examples when giving explanation", "R", db);
                InsertQuestions(BSID,0006, 50000487, true, "Ability to share and impart knowledge of the subject matter with others", "R", db);
                InsertQuestions(BSID,0007, 50000488, false, "Did the staff manage to meet your objective/expectation after attending the training? If no, please state the reason.", "T", db);
                InsertQuestions(BSID,0008, 50000489, false, "Do you feel that similar skill training is still required to reinforce the staff proficiency? If yes, please state the reason & when.", "T", db);
                InsertQuestions(BSID,0009, 50000490, false, "Do you feel that the staff should attend similar program at a higher level? If yes, suggest the next program & when.", "T", db);

                db.EndInsert();

            }
        }

        private void InsertQuestions(int groupID, int SAPCounter, int ID, bool isMandatory, string title, string answerType, UtilityDb db)
        {
            DataRow row = db.Insert(null);
            row["GroupID"] = groupID;
            row["SAPCounter"] = SAPCounter;
            row["DisplayOrder"] = ID;
            row["ID"] = ID;
            row["AnswerType"] = answerType;
            row["Title"] = title;
            row["Mandatory"] = isMandatory;
            db.Insert(row);

        }




    }
}
