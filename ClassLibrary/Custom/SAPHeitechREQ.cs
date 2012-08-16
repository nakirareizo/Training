using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Xml;
using System.Configuration;
using System.Data;
using MainLib;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Assessments.Classes;
using TrainingRequisition.ClassLibrary.Bases;
using System.Text.RegularExpressions;

namespace BUILD.Training.ClassLibrary.Custom
{
    public class SAPHeitechREQ
    {
        public enum executeMode
        {
            CourseEvaluations,
            PostTraining
        }
        private static string GetPaddedStaffID(string username)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = string.Format("SELECT * FROM UserMaster WHERE fldUid='" + username + "'");
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {

                    while (dr.Read())
                    {
                        string padded = dr["fldOid"].ToString();
                        while (padded.Length < 8)
                            padded = "0" + padded;
                        return padded;
                    }
                }

            }
            return "";
        }

        internal static bool SubmitPrebook(List<TrainingEvent> submissionList, string CurrentStaffUsername,
            ref string errorMessage, string trxid)
        {
            string paddedStaffID = GetPaddedStaffID(CurrentStaffUsername);
            foreach (TrainingEvent eventType in submissionList)
            {
                string sXMLInput = string.Empty;
                string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxid);
                MainLib.core oCore = new MainLib.core();
                MainLib.sobject oSAP = new MainLib.sobject();
                DateTime dBeginDate = DateTime.Now;
                DateTime dEndDate = new DateTime(DateTime.Now.Year, 12, 31).AddMonths(12);
                string sResultFromSAP = string.Empty;
                try
                {
                    sXMLInput = "<COMM>" +
                                "<REMARK>BAPI_PREBOOK_ATTENDANCE</REMARK>" +
                                "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                                "<PF>" + sProfileID + "</PF>" +
                                "<RFC>BAPI_PREBOOK_ATTENDANCE</RFC>" +
                                "<R_IF>1</R_IF>" +
                                "<R_OF>1</R_OF>" +
                                "<R_IT>0</R_IT>" +
                                "<R_OT>0</R_OT>" +
                                "<INPUT>" +
                                    "<IFLD>" +
                                        "<PLVAR>01</PLVAR>" +
                                        "<ATTENDEEID>" + paddedStaffID + "</ATTENDEEID>" +
                                        "<ATTENDEETYPE>P</ATTENDEETYPE>" +
                                        "<EVENTTYPEID>" + eventType.Id + "</EVENTTYPEID>" +
                                        "<PREBOOK_LANGUAGE>EN</PREBOOK_LANGUAGE>" +
                                        "<PREBOOK_LOCATION></PREBOOK_LOCATION>" +
                                        "<PREBOOK_PRIORITY>10</PREBOOK_PRIORITY>" +
                                        "<PREBOOK_COUNT>1</PREBOOK_COUNT>" +
                                        "<BEGIN_DATE>" + dBeginDate.ToString("yyyyMMdd") + "</BEGIN_DATE>" +
                                        "<END_DATE>" + dEndDate.ToString("yyyyMMdd") + "</END_DATE>" +
                                    "</IFLD>" +
                                    "<OFLD>" +
                                        "<RETURN>" +
                                            "<TRIGGER>1</TRIGGER>" +
                                        "</RETURN>" +
                                    "</OFLD>" +
                                    "<ITBL></ITBL>" +
                                    "<OTBL></OTBL>" +
                                "</INPUT>" +
                            "</COMM>";
                    if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                    {
                        sResultFromSAP = oSAP.RETMSG.ToString();
                        oCore.LogEvent("SAPHeitechREQ.aspx", "SubmitPrebook", sResultFromSAP, "1");
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(sResultFromSAP);
                        string sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                        string msg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                        if (sTYPE == "") //SUCCESSFULL
                        {
                            errorMessage = "SUCCESS";
                            return true;
                           
                        }
                        else if (sTYPE == "E")//Unsuccessfull
                        {
                           errorMessage = "SAP ERROR: " + msg + ". Please contact system administrator.";
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    oCore.LogEvent("SAPHeitechREQ.aspx", "ImportRequiredRating", "Catch", "BAPI_PREBOOK_ATTENDANCE");
                }
                finally
                {
                }
            }
            return true;
        }

        private static string LoadProfile(string loggedinUsername, string trxid)
        {

            try
            {
                using (SqlConnection conn = UtilityDb.GetConnectionUser())
                {

                    string sql = "SELECT Profiles.fldPfID AS profileID, Profiles.fldPfName, Profiles.fldEnabled, " +
                       "ProfileUsers.fldUid, ProfileTrx.fldTrxID FROM Profiles " +
                       "INNER JOIN ProfileUsers ON ProfileUsers.fldPfID = Profiles.fldPfID " +
                       "INNER JOIN ProfileTrx ON ProfileTrx.fldPfID = Profiles.fldPfID " +
                       "Where UPPER(ProfileUsers.fldUid) = '" + loggedinUsername.ToUpper() + "'" +
                       "AND ProfileTrx.fldTrxID = '" + trxid + "'";


                    using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                    {
                        while (dr.Read())
                        {
                            if (dr["profileID"] != DBNull.Value)
                                return dr["profileID"].ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return null;

        }

        internal static bool SubmitBook(string username, int eventDateID, int eventTypeID, ref string errorMessage, string trxid)
        {
            string StaffID = GetPaddedStaffID(username);
            string sXMLInput = string.Empty;
            string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxid);
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            string sResultFromSAP = string.Empty;
            try
            {
                sXMLInput = "<COMM>" +
                            "<REMARK>BAPI_BOOK_ATTENDANCE</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>BAPI_BOOK_ATTENDANCE</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>1</R_OF>" +
                            "<R_IT>0</R_IT>" +
                            "<R_OT>0</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<PLVAR>01</PLVAR>" +
                                    "<ATTENDEEID>" + StaffID + "</ATTENDEEID>" +
                                    "<ATTENDEETYPE>P</ATTENDEETYPE>" +
                                    "<EVENTID>" + eventDateID + "</EVENTID>" +
                                    "<ATTENDANCE_PRIORITY>10</ATTENDANCE_PRIORITY>" +
                                    "<ATTENDANCE_COUNT>1</ATTENDANCE_COUNT>" +
                                    "<EVENTTYPEID>" + eventTypeID + "</EVENTTYPEID>" +
                                "</IFLD>" +
                                "<OFLD>" +
                                    "<RETURN>" +
                                        "<TRIGGER>1</TRIGGER>" +
                                    "</RETURN>" +
                                "</OFLD>" +
                                "<ITBL></ITBL>" +
                                "<OTBL></OTBL>" +
                            "</INPUT>" +
                        "</COMM>";
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    oCore.LogEvent("SAPHeitechREQ.aspx", "SubmitBook", sResultFromSAP, "1");
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(sResultFromSAP);
                    string sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    string msg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    if (sTYPE == "") //SUCCESSFULL
                    {
                        errorMessage = "SUCCESS";
                        return true;                       
                    }
                    else if (sTYPE == "E")//Unsuccessfull
                    {
                       errorMessage = "SAP ERROR: " + msg + ". Please contact system administrator.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("SAPHeitechREQ.aspx", "SubmitBook", "Catch", "BAPI_BOOK_ATTENDANCE");
            }
            finally
            {
            }
            return false;

        }

        internal static bool DeleteConfirmAttendance(Staff staff,int EventID, string trxId, ref string errorMessage)
        {
            string StaffID = GetPaddedStaffID(staff.Username);
            string sXMLInput = string.Empty;
            string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxId);
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            string sResultFromSAP = string.Empty;
            try
            {
                sXMLInput = "<COMM>" +
                            "<REMARK>ZRFC_PREBOOK_DELETE</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>ZRFC_PREBOOK_DELETE</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>0</R_OF>" +
                            "<R_IT>1</R_IT>" +
                            "<R_OT>1</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<I_PLVAR>01</I_PLVAR>" +
                                    "<I_ETTYP>D</I_ETTYP>" +
                                    "<I_ETYID>" + EventID + "</I_ETYID>" +
                                    "<I_PARID>" + staff.StaffIDPadded + "</I_PARID>" +
                                "</IFLD>" +
                                "<OFLD></OFLD>" +
                                "<ITBL>" +
                                    "<T_RETURN></T_RETURN>" +
                                "</ITBL>" +
                                "<OTBL>" +
                                    "<T_RETURN></T_RETURN>" +
                                "</OTBL>" +
                            "</INPUT>" +
                        "</COMM>";
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    oCore.LogEvent("SAPHeitechREQ.aspx", "ConfirmAttendance", sResultFromSAP, "1");
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(sResultFromSAP);
                    string sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    string sMsg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    if (sTYPE != "E") //SUCCESSFULL
                    {
                       errorMessage = "SUCCESS";
                       return true;
                       
                    }
                    else//Unsuccessfull
                    {
                       errorMessage = sMsg;
                       return false;
                    }
                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("SAPHeitechREQ.aspx", "ConfirmAttendance", "Catch", "BAPI_BOOK_ATTENDANCE");
            }
            finally
            {
            }
            return false;
        }

        public static bool SendTrainingSAP(executeMode mode, string SuperUsername, string StaffUsername, TrainingEvent ev, DateTime StartDate, DateTime EndDate, int EventDateID, string Provider, List<QuestionGroup> questionGroups, string trxId, ref string errorMessage)
        {
            if (mode == executeMode.CourseEvaluations)
            {
                return SendToSAP_CourseEvaluation(SuperUsername, StaffUsername, ev, StartDate, EndDate, EventDateID, Provider, questionGroups, trxId, ref errorMessage);
                if (errorMessage.Contains("SUCCESS"))
                {
                    return true;
                }
            }
            else if (mode == executeMode.PostTraining)
            {
                return SendToSAP_PostTraining(SuperUsername, StaffUsername, ev, StartDate, EndDate, EventDateID, Provider, questionGroups, trxId, ref errorMessage);

                if (errorMessage.Contains("SUCCESS"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool SendToSAP_CourseEvaluation(string SuperUsername, string StaffUsername, TrainingEvent ev, DateTime StartDate, DateTime EndDate, int EventDateID, string Provider, List<QuestionGroup> questionGroups, string trxId, ref string errorMessage)
        {
            bool result = false;
            #region "COURSE EVALUATIONS"
            Staff Staff = Staff.GetFromUsername(StaffUsername);
            string CourseEvaluationsID = ConfigurationManager.AppSettings["REQCourseEvaluationID"].ToString();
            //get TrainingSAP_ID for Course Evaluations;
            string TrainingSAP_ID = GetTrainingSAP_ID(executeMode.CourseEvaluations, CourseEvaluationsID, StartDate, EndDate, Staff.StaffIDPadded, Staff.Name, EventDateID, Provider, SuperUsername, StaffUsername, ref errorMessage, trxId);
            if (string.IsNullOrEmpty(TrainingSAP_ID))
                return false;
            //Send data/elements to SAP BAPI/ZRFC
            result = RunModelChange(executeMode.CourseEvaluations, questionGroups, TrainingSAP_ID, SuperUsername, StaffUsername, trxId, Staff.StaffIDPadded, Staff.Name, EventDateID, Provider, ref errorMessage);
            if (!result)
                return false;
            result = RunModelChangeText(executeMode.CourseEvaluations, questionGroups, TrainingSAP_ID, EventDateID, SuperUsername, StaffUsername, ev, trxId, ref errorMessage);
            if (!result)
                return false;
            result = RunModelUpdate(TrainingSAP_ID, StaffUsername, ref errorMessage, trxId);
            if (!result)
                return false;
            #endregion
            return result;
        }

        public static bool SendToSAP_PostTraining(string SuperUsername, string StaffUsername, TrainingEvent ev, DateTime StartDate, DateTime EndDate, int EventDateID, string Provider, List<QuestionGroup> questionGroups, string trxId, ref string errorMessage)
        {
            bool result = false;
            #region "POST TRAINING"
            Staff Staff = Staff.GetFromUsername(StaffUsername);
            Staff Supervisor = Staff.GetFromUsername(SuperUsername);
            string PostTrainingID = ConfigurationManager.AppSettings["REQPostID"].ToString();
            //get PostTrainingSAP_ID for Post-Training Assessment;
            string PostTrainingSAP_ID = GetTrainingSAP_ID(executeMode.PostTraining, PostTrainingID, StartDate, EndDate, Staff.StaffIDPadded, Staff.Name, EventDateID, Provider, SuperUsername, StaffUsername, ref errorMessage, trxId);
            if (string.IsNullOrEmpty(PostTrainingSAP_ID))
                return false;
            //Update table based on retrun ID;
            result = RunModelChange(executeMode.PostTraining, questionGroups, PostTrainingSAP_ID, SuperUsername, StaffUsername, trxId, Staff.StaffIDPadded, Staff.Name, EventDateID, Provider, ref errorMessage);
            if (!result)
                return false;
            result = RunModelChangeText(executeMode.PostTraining, questionGroups, PostTrainingSAP_ID, EventDateID, SuperUsername, StaffUsername, ev, trxId, ref errorMessage);
            if (!result)
                return false;
            //Change status to COMPLETED for Post-Training Assessment;
            result = RunModelUpdate(PostTrainingSAP_ID, SuperUsername, ref errorMessage, trxId);
            if (!result)
                return false;
            #endregion
            return result;
        }
        //Get TrainingSAPID
        private static string GetTrainingSAP_ID(executeMode mode, string TrainingModelID, DateTime StartDate, DateTime EndDate, string StaffID, string StaffName, int EventDateID, string Provider, string SuperUsername, string StaffUsername, ref string returnMessage, string trxId)
        {
           string newProvider = FilterXMLExceptions(Provider);
            Staff Staff = Staff.GetFromUsername(StaffUsername);
            Staff Supervisor = Staff.GetFromUsername(SuperUsername);
            #region "BAPI_APPRAISAL_CREATE"
            string sID = "";
            string sProfileID = "";
            string sXMLInput = string.Empty;
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            if (mode == executeMode.CourseEvaluations)
            {
                sProfileID = LoadProfile(StaffUsername, trxId);

            }
            else
            {
                sProfileID = LoadProfile(SuperUsername, trxId);
            }
            string sResultFromSAP = string.Empty;
            string sTYPE = string.Empty;
            string sMsg = string.Empty;
            string titleModel = "";
            //find title for model based trainingID
            if (mode == executeMode.PostTraining)
                titleModel = "POST-TRAINING ASSESSMENT";

            else
                titleModel = "COURSE EVALUATIONS";

            try
            {
                string headerXML = "<COMM>" +
                            "<REMARK>BAPI_APPRAISAL_CREATE</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>BAPI_APPRAISAL_CREATE</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>1</R_OF>" +
                            "<R_IT>1</R_IT>" +
                            "<R_OT>0</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<PLAN_VERSION>01</PLAN_VERSION>" +
                                    "<APPRAISAL_MODEL_ID>" + TrainingModelID + "</APPRAISAL_MODEL_ID>" +
                                    "<START_DATE>" + StartDate.ToString("yyyyMMdd") + "</START_DATE>" +
                                    "<END_DATE>" + EndDate.ToString("yyyyMMdd") + "</END_DATE>" +
                                    "<TEXT>" + titleModel + "</TEXT>" +
                                    "<CREATION_DATE>" + DateTime.Now.ToString("yyyyMMdd") + "</CREATION_DATE>" +
                                    "<ANONYMOUS></ANONYMOUS>" +
                                    "<NOCOMMIT></NOCOMMIT>" +
                                "</IFLD>" +
                                "<OFLD>" +
                                    "<APPRAISAL_ID></APPRAISAL_ID>" +
                                    "<RETURN>" +
                                        "<TRIGGER>1</TRIGGER>" +
                                    "</RETURN>" +
                                "</OFLD>";

                string InputTableData = "";
                if (mode == executeMode.CourseEvaluations)
                {
                    InputTableData = "<ITBL>" +
                                    "<APPRAISERS>" +
                                       "<DATA>" +
                                            "<PLAN_VERSION>01</PLAN_VERSION>" +
                                            "<TYPE>P</TYPE>" +
                                            "<ID>" + StaffID + "</ID>" +
                                            "<NAME>" + StaffName + "</NAME>" +
                                       "</DATA>" +
                                    "</APPRAISERS>" +
                                    "<APPRAISEES>" +
                                       "<DATA>" +
                                            "<PLAN_VERSION>01</PLAN_VERSION>" +
                                            "<TYPE>E</TYPE>" +
                                            "<ID>" + EventDateID + "</ID>" +
                                            "<NAME>" + newProvider + "</NAME>" +
                                       "</DATA>" +
                                    "</APPRAISEES>" +
                                "</ITBL>";
                }
                if (mode == executeMode.PostTraining)
                {
                    InputTableData = "<ITBL>" +
                                    "<APPRAISERS>" +
                                       "<DATA>" +
                                            "<PLAN_VERSION>01</PLAN_VERSION>" +
                                            "<TYPE>P</TYPE>" +
                                            "<ID>" + Supervisor.StaffIDPadded + "</ID>" +
                                            "<NAME>" + Supervisor.Name + "</NAME>" +
                                       "</DATA>" +
                                    "</APPRAISERS>" +
                                    "<APPRAISEES>" +
                                       "<DATA>" +
                                            "<PLAN_VERSION>01</PLAN_VERSION>" +
                                            "<TYPE>P</TYPE>" +
                                            "<ID>" + Staff.StaffIDPadded + "</ID>" +
                                            "<NAME>" + Staff.Name + "</NAME>" +
                                       "</DATA>" +
                                    "</APPRAISEES>" +
                                "</ITBL>";
                }


                string footer = "<OTBL></OTBL>" +
                              "</INPUT>" +
                          "</COMM>";
                sXMLInput = headerXML + InputTableData + footer;
                oCore.LogEvent("SAPHeitech.aspx", "GetTrainingSAP_ID", sXMLInput, "BAPI_APPRAISAL_CREATE");
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    oCore.LogEvent("SAPHeitech.aspx", "GetAppraisalSAP_ID", sResultFromSAP, "3");
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(sResultFromSAP);
                    sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    sMsg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                    {
                        if (xDoc.GetElementsByTagName("APPRAISAL_ID").Item(0).InnerText.ToUpper() != null)
                        {
                            sID = xDoc.GetElementsByTagName("APPRAISAL_ID").Item(0).InnerText;
                            return sID;
                        }
                    }
                    else//Unsuccessfull
                    {
                       returnMessage = "Error while SAP GetTrainingID. SAP Error:" + sMsg + ". Please contact system administrator.";
                        sID = "";
                    }

                }
                return sID;
            }
            catch (Exception ex)
            {
                oCore.LogEvent("SAPHeitech.aspx", "GetAppraisalSAP_ID", ex.Message, "3");
                return sID;
            }
            finally
            {
            }

            #endregion
        }

        private static string FilterXMLExceptions(string Provider)
        {
           string newProvider="";
           if (!string.IsNullOrEmpty(Provider))
           {
              newProvider = Provider.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
           }
           return newProvider;
        }
        //Update/Change Module
        private static bool RunModelChange(executeMode mode, List<QuestionGroup> questionGroups, string TrainingSAP_ID, string SuperUsername, string StaffUsername, string trxId, string StaffID, string StaffName, int EventDateID, string Provider, ref string errorMessage)
        {
           string newProvider = FilterXMLExceptions(Provider);
            Staff Staff = Staff.GetFromUsername(StaffUsername);
            Staff Supervisor = Staff.GetFromUsername(SuperUsername);
            string sProfileID = "";
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            if (mode == executeMode.CourseEvaluations)
            {
                sProfileID = LoadProfile(StaffUsername, trxId);
            }
            else
            {
                sProfileID = LoadProfile(SuperUsername, trxId);
            }
            string output = "";
            string sResultFromSAP = string.Empty;
            string sTYPE = string.Empty;
            #region "headerXML"
            string headerXML = "<COMM>" +
                            "<REMARK>BAPI_APPRAISAL_CHANGE</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>BAPI_APPRAISAL_CHANGE</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>1</R_OF>" +
                            "<R_IT>1</R_IT>" +
                            "<R_OT>1</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<PLAN_VERSION>01</PLAN_VERSION>" +
                                    "<APPRAISAL_ID>" + TrainingSAP_ID + "</APPRAISAL_ID>" +
                                    "<TEXT></TEXT>" +
                                    "<CREATION_DATE>" + DateTime.Now.ToString("yyyyMMdd") + "</CREATION_DATE>" +
                                    "<ANONYMOUS></ANONYMOUS>" +
                                    "<NOCOMMIT></NOCOMMIT>" +
                                "</IFLD>" +
                                "<OFLD>" +
                                    "<RETURN>" +
                                        "<TRIGGER>1</TRIGGER>" +
                                    "</RETURN>" +
                                "</OFLD>";
            #region input table
            string InputTable = "";
            if (mode == executeMode.CourseEvaluations)
            {
                InputTable = "<ITBL>" +
                                "<APPRAISERS>" +
                                   "<DATA>" +
                                        "<PLAN_VERSION>01</PLAN_VERSION>" +
                                        "<TYPE>P</TYPE>" +
                                        "<ID>" + StaffID + "</ID>" +
                                        "<NAME>" + StaffName + "</NAME>" +
                                   "</DATA>" +
                                "</APPRAISERS>" +
                                "<APPRAISEES>" +
                                   "<DATA>" +
                                        "<PLAN_VERSION>01</PLAN_VERSION>" +
                                        "<TYPE>E</TYPE>" +
                                        "<ID>" + EventDateID + "</ID>" +
                                        "<NAME>" + newProvider + "</NAME>" +
                                   "</DATA>" +
                                "</APPRAISEES>";
            }
            if (mode == executeMode.PostTraining)
            {
                InputTable = "<ITBL>" +
                                "<APPRAISERS>" +
                                   "<DATA>" +
                                        "<PLAN_VERSION>01</PLAN_VERSION>" +
                                        "<TYPE>P</TYPE>" +
                                        "<ID>" + Supervisor.StaffIDPadded + "</ID>" +
                                        "<NAME>" + Supervisor.Name + "</NAME>" +
                                   "</DATA>" +
                                "</APPRAISERS>" +
                                "<APPRAISEES>" +
                                   "<DATA>" +
                                        "<PLAN_VERSION>01</PLAN_VERSION>" +
                                        "<TYPE>P</TYPE>" +
                                        "<ID>" + Staff.StaffIDPadded + "</ID>" +
                                        "<NAME>" + Staff.Name + "</NAME>" +
                                   "</DATA>" +
                                "</APPRAISEES>";
            }
            #endregion
            #endregion
            headerXML = headerXML + InputTable;
            string AppraisalData = "";
            float overallTotal = 0;
            int groupCount = 0;
            foreach (QuestionGroup group in questionGroups)
            {
                float rating = group.Rating();
                if (rating > 0)
                {
                    overallTotal += (rating * group.Weight);
                    groupCount++;
                }
            }

            float overallAverage = overallTotal / groupCount;


            AppraisalData = "<APPRAISAL_DATA>";

            // send ratings
            foreach (QuestionGroup group in questionGroups)
            {
                string GroupSapCounter = group.SAPCounter.ToString().PadLeft(4, '0');
                string xmlElement = GenerateElementXML("BG", group.Id.ToString(), GroupSapCounter, "0000", group.Weight, group.Rating(), "");
                AppraisalData += xmlElement;
                foreach (AssessmentQuestion question in group.Questions)
                {
                    string questionSAPCounter = question.SAPCounter.ToString().PadLeft(4, '0');

                    if (question.AnswerType == AssessmentQuestion.AnswerTypes.R)
                    {
                        xmlElement = GenerateElementXML("BK", question.Id.ToString(), questionSAPCounter, GroupSapCounter, question.Weight, Convert.ToSingle(question.AnswerValue), "");
                    }
                    else
                    {
                        if (mode == executeMode.PostTraining)
                        {
                            if (question.AnswerType == AssessmentQuestion.AnswerTypes.T)
                            {
                                string NoteRating = question.AnswerValue.ToString();
                                int rate = 0;
                                if (!string.IsNullOrEmpty(NoteRating))
                                {
                                    rate = 1;
                                }
                                xmlElement = GenerateElementXML("BK", question.Id.ToString(), questionSAPCounter, GroupSapCounter, question.Weight, rate, "");
                            }
                        }
                    }
                    AppraisalData += xmlElement;
                }
            }

            AppraisalData += "</APPRAISAL_DATA>" +
                             "</ITBL>";
            string footer = "<OTBL>" +
                                 "<APPRAISERS></APPRAISERS>" +
                                 "<APPRAISEES></APPRAISEES>" +
                                 "<APPRAISAL_DATA></APPRAISAL_DATA>" +
                            "</OTBL>" +
                             "</INPUT>" +
                             "</COMM>";
            output = headerXML + AppraisalData + footer;
            oCore.LogEvent("SAPHeitech.aspx", "RunModelChange", output, "BAPI_APPRAISAL_CHANGE");
            if (oSAP.ExeProc(output, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
            {
                sResultFromSAP = oSAP.RETMSG.ToString();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(sResultFromSAP);
                sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                errorMessage = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                {
                    if (errorMessage.Contains("SUCCESS"))
                        return true;
                }
                else//UNSUCCESSFULL(ERROR)
                {
                    errorMessage = "Error while change the ratings. SAP erro: " + errorMessage +" Please contact system administrator";
                    oCore.LogEvent("SAPHeitech.aspx", "RunModelChange", errorMessage, "BAPI_APPRAISAL_CHANGE");
                    return false;
                }
            }
            return false;
        }
        //Update/Change NOTEs Module
        private static bool RunModelChangeText(executeMode mode, List<QuestionGroup> questionGroups, string TrainingSAP_ID, int EventDateID, string SuperUsername, string StaffUsername, TrainingEvent ev, string trxId, ref string errorMessage)
        {
            bool result = false;
            Staff Staff = Staff.GetFromUsername(StaffUsername);
            Staff Supervisor = Staff.GetFromUsername(SuperUsername);
            #region "POST TRAINING ASSESSMENT"
            if (mode == executeMode.PostTraining)
            {
                int SAPCounter = 0;
                string LineCounter = ""; ;
                string notes = ""; ;
                string str = "0000";
                string TDName = "";
                SAPCounter = 1;
                LineCounter = string.Format("{0}", SAPCounter.ToString().PadLeft(4, '0'));
                notes = ev.DisplayName.ToString().ToUpper() + "-(BUSINESS EVENT ID :" + EventDateID.ToString() + ")";
                List<string> ListofNote = Split(132, notes);
                str = "^^^^" + str;
                TDName = "01BA" + TrainingSAP_ID + "1045" + str + LineCounter;
                result = ChangeTextinModel(mode, SuperUsername, StaffUsername, TDName, ListofNote, trxId, ref errorMessage);
                if (!result)
                    return false;
                foreach (QuestionGroup group in questionGroups)
                {
                    foreach (AssessmentQuestion question in group.Questions)
                    {
                        if (question.AnswerType == AssessmentQuestion.AnswerTypes.T)
                        {
                            string strQG = "0000";
                            SAPCounter = question.SAPCounter;
                            LineCounter = string.Format("{0}", SAPCounter.ToString().PadLeft(4, '0'));
                            notes = question.AnswerValue;
                            List<string> ListofNotes = Split(132, notes);
                            strQG = "^^^^" + strQG;
                            TDName = "01BA" + TrainingSAP_ID + "1045" + strQG + LineCounter;
                            result = ChangeTextinModel(mode, SuperUsername, StaffUsername, TDName, ListofNotes, trxId, ref errorMessage);
                            if (!result)
                                return false;
                        }
                    }
                }
            }
            #endregion
            #region "COURSE EVALUATIONS"
            if (mode == executeMode.CourseEvaluations)
            {
                int SAPCounter = 0;
                string LineCounter = ""; ;
                string notes = "";
                string TDName = "";
                foreach (QuestionGroup group in questionGroups)
                {
                    foreach (AssessmentQuestion question in group.Questions)
                    {
                        if (question.AnswerType == AssessmentQuestion.AnswerTypes.T)
                        {
                            string str = "0000";
                            SAPCounter = question.SAPCounter;
                            LineCounter = string.Format("{0}", SAPCounter.ToString().PadLeft(4, '0'));
                            notes = question.AnswerValue;
                            List<string> ListofNotes = Split(132, notes);
                            str = "^^^^" + str;
                            TDName = "01BA" + TrainingSAP_ID + "1045" + str + LineCounter;
                            result = ChangeTextinModel(mode, SuperUsername, StaffUsername, TDName, ListofNotes, trxId, ref errorMessage);
                            if (!result)
                                return false;
                        }
                    }
                }
            }
            #endregion
            return result;
        }

        private static bool ChangeTextinModel(executeMode mode, string SuperUsername, string StaffUsername, string TDName, List<string> notes, string trxId, ref string errorMessage)
        {
            bool result = false;
            #region "BAPI_APPRAISAL_CHANGE"
            string sXMLInput = string.Empty;
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            string sProfileID = "";
            if (mode == executeMode.CourseEvaluations)
            {
                sProfileID = LoadProfile(StaffUsername, trxId);
            }
            else
            {
                sProfileID = LoadProfile(SuperUsername, trxId);
            }
            string sResultFromSAP = string.Empty;
            string sTYPE = string.Empty;
            try
            {
                string header = "<COMM>" +
                            "<REMARK>ZRFC_SAVE_TEM_NOTES</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>ZRFC_SAVE_TEM_NOTES</RFC>" +
                            "<R_IF>0</R_IF>" +
                            "<R_OF>0</R_OF>" +
                            "<R_IT>1</R_IT>" +
                            "<R_OT>1</R_OT>" +
                            "<INPUT>" +
                                "<IFLD></IFLD>" +
                                "<OFLD></OFLD>" +
                                "<ITBL>" +
                                    "<T_HEADER>" +
                                       "<DATA>" +
                                            "<TDOBJECT>HR_APPNOTE</TDOBJECT>" +
                                            "<TDNAME>" + TDName + "</TDNAME>" +
                                            "<TDID>APP1</TDID>" +
                                            "<TDSPRAS>EN</TDSPRAS>" +
                                            "<TDTITLE></TDTITLE>" +
                                            "<TDFORM>HR_TEM_NOTE_DISP</TDFORM>" +
                                            "<TDSTYLE>HR_TEM1</TDSTYLE>" +
                                            "<TDVERSION>00000</TDVERSION>" +
                                            "<TDFUSER></TDFUSER>" +
                                       "</DATA>" +
                                    "</T_HEADER>";
                string tLineData = "";
                tLineData = "<T_LINES>";
                int myCounter = 0;
                foreach (string noteLine in notes)
                {
                    if (myCounter == 0)
                    {
                        tLineData += "<DATA>" +
                                    "<TDFORMAT>*</TDFORMAT>" +
                                    "<TDLINE>" + noteLine + "</TDLINE>" +
                                 "</DATA>";
                    }
                    if (myCounter > 0)
                    {
                        tLineData += "<DATA>" +
                                    "<TDFORMAT></TDFORMAT>" +
                                    "<TDLINE>" + noteLine + "</TDLINE>" +
                                 "</DATA>";
                    }
                    myCounter++;
                }
                tLineData += "</T_LINES>";
                string footer = "<T_RETURN>" +
                                  "<TRIGGER>1</TRIGGER>" +
                              "</T_RETURN>" +
                  "</ITBL>" +
                  "<OTBL>" +
                      "<T_HEADER></T_HEADER>" +
                      "<T_LINE></T_LINE>" +
                      "<T_RETURN></T_RETURN>" +
                  "</OTBL>" +
              "</INPUT>" +
          "</COMM>";
                sXMLInput = header + tLineData + footer;
                oCore.LogEvent("SAPHeitech.aspx", "GetTrainingSAP_ID", sXMLInput, "BAPI_APPRAISAL_CREATE");
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    oCore.LogEvent("SAPHeitech.aspx", "GetAppraisalSAP_ID", sResultFromSAP, "3");
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(sResultFromSAP);
                    sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    errorMessage = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    if (sTYPE.ToUpper() != "E") //UNSUCCESSFULL
                    {
                        result = true;

                    }
                    else
                    {
                        errorMessage = "Error while change the ratings text. SAP error:" + errorMessage + " Please contact system administrator";
                        oCore.LogEvent("SAPHeitech.aspx", "RunModelChange", errorMessage, "BAPI_APPRAISAL_CHANGE");
                        result = false;
                    }

                }
            }
            catch (Exception ex)
            {
                oCore.LogEvent("SAPHeitechREQ.aspx", "RUNModelChangeText", ex.Message, "3");
                result = false;
            }
            #endregion
            return result;
        }

        static private List<string> Split(int maxlength, string notes)
        {
            List<string> output = new List<string>();
            char[] delimiter = { '\r', '\n' };
            string[] lines = notes.Split(delimiter);
            foreach (string line in lines)
            {
                if (line.Length > maxlength)
                {
                    int start = 0;
                    while (start < line.Length)
                    {
                        int end = Math.Min(maxlength + start, line.Length);
                        string lineOutput = line.Substring(start, end - start);
                        output.Add(lineOutput);
                        start = end;
                    }
                }
                else
                {
                    output.Add(line);
                }
            }
            return output;
        }

        //Change Module Status
        private static bool RunModelUpdate(string TrainingSAP_ID, string loggedinUsername, ref string errorMessage, string trxId)
        {
            string sXMLInput = string.Empty;
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            string sProfileID = LoadProfile(loggedinUsername, trxId);
            string sResultFromSAP = string.Empty;
            string StatusUpdate = string.Empty;
            string Status = string.Empty;
            string sTYPE = string.Empty;
            string sMsg = string.Empty;
            try
            {
                sXMLInput = "<COMM>" +
                            "<REMARK>BAPI_APPRAISAL_STATUS_CHANGE</REMARK>" +
                            "<UID>" + loggedinUsername + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>BAPI_APPRAISAL_STATUS_CHANGE</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>1</R_OF>" +
                            "<R_IT>0</R_IT>" +
                            "<R_OT>0</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<PLAN_VERSION>01</PLAN_VERSION>" +
                                    "<APPRAISAL_ID>" + TrainingSAP_ID + "</APPRAISAL_ID>" +
                                    "<STATUS>03</STATUS>" +
                                    "<NOCOMMIT></NOCOMMIT>" +
                                "</IFLD>" +
                                "<OFLD>" +
                                    "<RETURN>" +
                                        "<TRIGGER>1</TRIGGER>" +
                                    "</RETURN>" +
                                "</OFLD>" +
                                "<ITBL></ITBL>" +
                                "<OTBL></OTBL>" +
                            "</INPUT>" +
                        "</COMM>";
                oCore.LogEvent("SAPHeitech.aspx", "RunModelChange", sXMLInput, "BAPI_APPRAISAL_UPDATE");
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(sResultFromSAP);
                    errorMessage = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    sMsg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                    if (sTYPE.ToUpper() != "E")//SUCCESSFULL
                    {
                        if (errorMessage.Contains("SUCCESS"))
                            return true;
                    }
                    else//UNSUCCESSFULL(ERROR)
                    {
                        errorMessage = "Error while change the form's status. SAP error:" + sMsg ;
                        oCore.LogEvent("SAPHeitech.aspx", "RunModelUpdate", sMsg, "BAPI_APPRAISAL_STATUS_CHANGE");
                        return false;
                    }
                    return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        private static string GenerateElementXML(string elementType, string elementSAP_ID, string elementSAP_Counter, string parentSAP_Counter, float weight, float rating, string notes)
        {
            string sWeight = weight.ToString("#.##");
            string sRating = rating.ToString("#.##");
            string xmlElement = "<DATA>";
            xmlElement += "<COUNTER>" + elementSAP_Counter + "</COUNTER>";
            xmlElement += "<ELEMENT_TYPE>" + elementType + "</ELEMENT_TYPE>";
            xmlElement += "<ELEMENT_ID>" + elementSAP_ID + "</ELEMENT_ID>";
            xmlElement += "<ELEMENT_TEXT>" + "</ELEMENT_TEXT>";
            xmlElement += "<WEIGHTING>" + weight + "</WEIGHTING>";
            xmlElement += "<RATING>" + sRating + "</RATING>";
            xmlElement += "<RATING_TEXT>" + notes + "</RATING_TEXT>";
            xmlElement += "<NOT_RATED>" + "</NOT_RATED>";
            xmlElement += "<INPUT_TYPE>" + "</INPUT_TYPE>";
            xmlElement += "<PARENT>" + parentSAP_Counter + "</PARENT>";
            xmlElement += "</DATA>";
            return xmlElement;
        }

        public static bool ImportTrainingModel(executeMode mode, string TrainingModelID, string loggedinUsername, string trxId)
        {
            const int PTAFakeGroupID = 1;
            bool result = false;
            #region "BAPI_APPRAISAL_GETDETAIL"
            string sXMLInput = string.Empty;
            string sResultFromSAP = string.Empty;
            XmlDocument xDoc = new XmlDocument();
            string sMsg = string.Empty;
            MainLib.core oCore = new MainLib.core();
            MainLib.sobject oSAP = new MainLib.sobject();
            string sProfileID = LoadProfile(loggedinUsername, trxId);
            try
            {
                sXMLInput = "<COMM>" +
                            "<REMARK>ZRFC_APPRAISAL_MODEL_GETDETAIL</REMARK>" +
                            "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                            "<PF>" + sProfileID + "</PF>" +
                            "<RFC>ZRFC_APPRAISAL_MODEL_GETDETAIL</RFC>" +
                            "<R_IF>1</R_IF>" +
                            "<R_OF>1</R_OF>" +
                            "<R_IT>0</R_IT>" +
                            "<R_OT>1</R_OT>" +
                            "<INPUT>" +
                                "<IFLD>" +
                                    "<PLAN_VERSION>01</PLAN_VERSION>" +
                                    "<APPRAISAL_MODEL_ID>" + TrainingModelID + "</APPRAISAL_MODEL_ID>" +
                                    "<KEY_DATE>" + DateTime.Now.ToString("yyyyMMdd") + "</KEY_DATE>" +
                                "</IFLD>" +
                                "<OFLD>" +
                                    "<NAME></NAME>" +
                                    "<FORM_ID></FORM_ID>" +
                                    "<FORM_TEXT></FORM_TEXT>" +
                                    "<KIND_ID></KIND_ID>" +
                                    "<KIND_TEXT></KIND_TEXT>" +
                                    "<APPRAISERS_ROLE></APPRAISERS_ROLE>" +
                                    "<APPRAISEES_ROLE></APPRAISEES_ROLE>" +
                                    "<START_DATE></START_DATE>" +
                                    "<END_DATE></END_DATE>" +
                                    "<RETURN>" +
                                        "<TRIGGER>1</TRIGGER>" +
                                    "</RETURN>" +
                                "</OFLD>" +
                                "<ITBL></ITBL>" +
                                "<OTBL>" +
                                        "<ALLOWED_APPRAISER_TYPES></ALLOWED_APPRAISER_TYPES>" +
                                        "<ALLOWED_APPRAISEE_TYPES></ALLOWED_APPRAISEE_TYPES>" +
                                        "<APPRAISAL_DATA></APPRAISAL_DATA>" +
                                        "<T_DESCRIP></T_DESCRIP>" +
                                "</OTBL>" +
                            "</INPUT>" +
                        "</COMM>";
            #endregion
                oCore.LogEvent("SAPHeitechREQ.aspx", "ImportTrainingModel", sXMLInput, "BAPI_APPRAISAL_GETDETAIL");
                //if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true) 
                if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
                {
                    sResultFromSAP = oSAP.RETMSG.ToString();
                    oCore.LogEvent("SAPHeitechREQ.aspx", "ImportTrainingModel", sResultFromSAP, "3");
                    Dictionary<string, string> descriptions = new Dictionary<string, string>();
                    List<QuestionGroup> QuestionGroups = new List<QuestionGroup>();
                    List<AssessmentQuestion> Questions = new List<AssessmentQuestion>();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sResultFromSAP);
                    XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/COMM/OTBL/APPRAISAL_DATA/DATA");
                    XmlNodeList xmlNodeList1 = xmlDocument.SelectNodes("/COMM/OTBL/T_DESCRIP/DATA");
                    using (UtilityDb db = new UtilityDb())
                    {
                        db.OpenConnectionESS();
                        if (xmlNodeList1.Count > 0)
                        {
                            result = true;
                            foreach (XmlNode xmlNode in xmlNodeList1)
                            {
                                string sElement_TypeDes = xmlNode["ELEMENT_TYPE"].InnerText.ToString();
                                string sElement_TypeID = xmlNode["ELEMENT_ID"].InnerText.ToString();
                                string sElement_TypeTLine = xmlNode["TLINE"].InnerText.ToString();

                                string description = "";
                                if (descriptions.ContainsKey(sElement_TypeID))
                                    description = descriptions[sElement_TypeID];

                                description += " " + sElement_TypeTLine;
                                descriptions[sElement_TypeID] = description;
                            }
                        }
                        if (xmlNodeList.Count > 0)
                        {
                            result = true;
                            QuestionGroup qg = new QuestionGroup();
                            foreach (XmlNode xmlNode in xmlNodeList)
                            {

                                string sElement_Type = xmlNode["ELEMENT_TYPE"].InnerText.ToString();
                                string sElement_Text = xmlNode["ELEMENT_TEXT"].InnerText.ToString();
                                string sWeighting = xmlNode["WEIGHTING"].InnerText.ToString();
                                string sParentCounter = xmlNode["PARENT"].InnerText.ToString();
                                string sCounter = xmlNode["COUNTER"].InnerText.ToString();
                                string sElementID = xmlNode["ELEMENT_ID"].InnerText.ToString();
                                string strTrainingNoteIDs = ConfigurationManager.AppSettings["TrainingNotes"];
                                char[] delim = { ',' };
                                string[] trainingNoteIDs = strTrainingNoteIDs.Split(delim);

                                switch (sElement_Type)
                                {
                                    case "BS":
                                        if (mode == executeMode.PostTraining)
                                        {
                                            QuestionGroup qgPTA = new QuestionGroup();
                                            qgPTA.Id = PTAFakeGroupID;
                                            qgPTA.Name = "PTA_Sect1";
                                            qgPTA.Title = "Post-Training Assessment";
                                            qgPTA.DisplayOrder = 1;
                                            qgPTA.Weight = 1;
                                            qgPTA.SAPCounter = 1;
                                            QuestionGroups.Add(qgPTA);
                                        }
                                        break;

                                    case "BG":
                                        qg = new QuestionGroup();
                                        qg.Id = Convert.ToInt32(sElementID);
                                        qg.Title = sElement_Text;

                                        if (qg.Title.ToUpper().Contains("TRAINER"))
                                        {
                                            qg.Name = "CEF_Sect1";
                                        }
                                        if (qg.Title.ToUpper().Contains("COURSE OBJECTIVE"))
                                        {
                                            qg.Name = "CEF_Sect2";
                                        }
                                        if (qg.Title.ToUpper().Contains("COURSE MATERIAL & FACILITIES"))
                                        {
                                            qg.Name = "CEF_Sect3";
                                        }
                                        if (qg.Title.ToUpper().Contains("GENERAL"))
                                        {
                                            qg.Name = "CEF_Sect4";
                                        }
                                        qg.DisplayOrder = Convert.ToInt32(sCounter);
                                        qg.Weight = Convert.ToSingle(sWeighting);
                                        qg.SAPCounter = Convert.ToInt32(sCounter);
                                        QuestionGroups.Add(qg);
                                        break;

                                    case "BK":
                                        AssessmentQuestion q = new AssessmentQuestion();
                                        q.Id = Convert.ToInt32(sElementID);
                                        q.GroupId = qg.Id;
                                        if (mode == executeMode.PostTraining)
                                            q.GroupId = PTAFakeGroupID;
                                        q.DisplayOrder = Convert.ToInt32(sCounter);
                                        q.AnswerType = AssessmentQuestion.AnswerTypes.R;
                                        q.Mandatory = true;
                                        q.Title = "Untitled";
                                        foreach (string find in trainingNoteIDs)
                                        {
                                            if (find == q.Id.ToString())
                                            {
                                                q.AnswerType = AssessmentQuestion.AnswerTypes.T;
                                                q.Mandatory = false;
                                                break;
                                            }
                                        }
                                        q.Weight = Convert.ToSingle(sWeighting);
                                        q.SAPCounter = Convert.ToInt32(sCounter);
                                        if (descriptions.ContainsKey(sElementID))
                                            q.Title = descriptions[sElementID];
                                        Questions.Add(q);
                                        break;
                                }

                            }
                        }
                        db.PrepareInsert("ASM_QuestionGroups");
                        foreach (QuestionGroup qg in QuestionGroups)
                        {
                            DataRow row = db.Insert(null);
                            qg.Save(row);
                            db.Insert(row);
                        }
                        db.EndInsert();
                        result = true;
                        db.PrepareInsert("ASM_Questions");
                        foreach (AssessmentQuestion q in Questions)
                        {
                            DataRow row = db.Insert(null);
                            q.Save(row);
                            db.Insert(row);
                        }
                        db.EndInsert();
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }

            finally
            {
            }
        }

        public static void LoadBookeTrainingList(string StaffIDPadded, string trxid)
       {
          string sXMLInput = string.Empty;
          string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxid);
          MainLib.core oCore = new MainLib.core();
          MainLib.sobject oSAP = new MainLib.sobject();
          DateTime dBeginDate = new DateTime(DateTime.Now.Year, 01, 01);
          DateTime dEndDate = new DateTime(DateTime.Now.Year, 12, 31).AddMonths(12);
          string sResultFromSAP = string.Empty;
          try
          {
             sXMLInput = "<COMM>" +
                         "<REMARK>BAPI_ATTENDEE_BOOK_LIST</REMARK>" +
                         "<UID>" + HttpContext.Current.User.Identity.Name + "</UID>" +
                         "<PF>" + sProfileID + "</PF>" +
                         "<RFC>BAPI_ATTENDEE_BOOK_LIST</RFC>" +
                         "<R_IF>1</R_IF>" +
                         "<R_OF>1</R_OF>" +
                         "<R_IT>1</R_IT>" +
                         "<R_OT>1</R_OT>" +
                         "<INPUT>" +
                             "<IFLD>" +
                                 "<OBJID>" + StaffIDPadded + "</OBJID>" +
                                 "<OTYPE>P</OTYPE>" +
                                 "<BEGIN_DATE>" + dBeginDate.ToString("yyyyMMdd") + "</BEGIN_DATE>" +
                                 "<END_DATE>" + dEndDate.ToString("yyyyMMdd") + "</END_DATE>" +
                                 "<PLVAR>01</PLVAR>" +
                             "</IFLD>" +
                             "<OFLD>" +
                                 "<ATTENDEE_NAME></ATTENDEE_NAME>" +
                                 "<RETURN>" +
                                     "<TRIGGER>1</TRIGGER>" +
                                 "</RETURN>" +
                             "</OFLD>" +
                             "<ITBL>" +
                               "<ATTENDEE_BOOK_LIST></ATTENDEE_BOOK_LIST>" +
                             "</ITBL>" +
                             "<OTBL>" +
                               "<ATTENDEE_BOOK_LIST></ATTENDEE_BOOK_LIST>" +
                             "</OTBL>" +
                         "</INPUT>" +
                     "</COMM>";
             if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
             {
                sResultFromSAP = oSAP.RETMSG.ToString();
                oCore.LogEvent("BookingListing.aspx", "BindSAPList", sResultFromSAP, "1");
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(sResultFromSAP);
                XmlNodeList xmlNodeList = xDoc.SelectNodes("/COMM/OTBL/ATTENDEE_BOOK_LIST/DATA");
                string sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                string msg = xDoc.GetElementsByTagName("MESSAGE").Item(0).InnerText.ToUpper();
                if (sTYPE != "E") //SUCCESSFULL
                {
                   DataSet dsNET = new DataSet();
                   DataTable dtNET = dsNET.Tables.Add("GVSAPLIST");
                   dtNET.Columns.Add("Title", Type.GetType("System.String"));
                   dtNET.Columns.Add("BookedDate", Type.GetType("System.String"));
                   dtNET.Columns.Add("TrainingLoc", Type.GetType("System.String"));
                   dtNET.Columns.Add("StartDate", Type.GetType("System.String"));
                   dtNET.Columns.Add("EndDate", Type.GetType("System.String"));
                   dtNET.Columns.Add("TrainingType", Type.GetType("System.String"));
                   if (xmlNodeList.Count > 0)
                   {
                      foreach (XmlNode xmlNode in xmlNodeList)
                      {
                         DataRow dRow = dtNET.NewRow();
                         DataSet dsSAP = new DataSet();
                         dRow["Title"] = xmlNode["EVSTX"].InnerText.ToString();
                         dRow["BookedDate"] = Convert.ToDateTime(xmlNode["BUDAT"].InnerText.ToString()).ToShortDateString();
                         dRow["TrainingLoc"] = xmlNode["LOCTX"].InnerText.ToString();
                         dRow["StartDate"] = Convert.ToDateTime(xmlNode["EVBEG"].InnerText.ToString()).ToShortDateString();
                         dRow["EndDate"] = Convert.ToDateTime(xmlNode["EVEND"].InnerText.ToString()).ToShortDateString();
                         dRow["TrainingType"] = xmlNode["EVSHT"].InnerText.ToString();
                         dtNET.Rows.Add(dRow);
                         msg = "SUCCESS";
                      }
                     
                   }
                }
                else if (sTYPE == "E")//Unsuccessfull
                {
                   msg = "SAP ERROR: " + msg + ". Please contact system administrator.";
                }
             }
          }
          catch (Exception ex)
          {
             oCore.LogEvent("BookingListing.aspx", "BindSAPList", "Catch", "ATTENDEE_BOOK_LIST");
          }
          finally
          {
          }
       }

    }
}