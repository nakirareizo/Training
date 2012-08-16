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
using MainLib;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;
using System.Web;
using System.Threading;

namespace BUILD.Training.TrainingSAP
{
    public partial class SAPTrainingImporter : System.Web.UI.Page
    {
        #region "Global Declarartion"
        MainLib.core oCore = new MainLib.core();
        MainLib.sobject oSAP = new MainLib.sobject();
        string sProfileID = "";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
           if (!IsPostBack)
           {
              DoProcess();
             string sScript0 = "window.alert('Import Process completed.');location.replace('/Default.aspx?ID=" + Request.QueryString["ID"].ToString() + "');";
              ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Page_Load_Script_10", sScript0, true);
           }
                  
        }

        private void DoProcess()
        {

           {
              DeleteAll();
              string Username = HttpContext.Current.User.Identity.Name.ToString();
              string TrxId = Request.QueryString["ID"].ToString();
              sProfileID = LoadProfile(Username, TrxId);
              LoadEventGroupSAP();
              LoadTrainingDetail();
              LoadEventDate();
           }
        }

        private static void DeleteAll()
        {

           MainLib.core oCore = new MainLib.core();
           using (SqlConnection conn = UtilityDb.GetConnectionESS())
           {
              UtilityDb.ResetTable("REQ_EventGroups", conn);
              UtilityDb.ResetTable("REQ_EventsInGroups", conn);
              UtilityDb.ResetTable("REQ_Events", conn);
              UtilityDb.ResetTable("REQ_EventDates", conn);
              UtilityDb.ResetTable("REQ_AttendedEvents", conn);
              UtilityDb.ResetTable("REQ_AttendanceToConfirm", conn);
              DeleteOrphanNotes(conn);
           }
           oCore.LogEvent("ImportSAPTraining.aspx", "DeleteAll", "DeleteTables Failed", "3");
        }

        private static void DeleteOrphanNotes(SqlConnection conn)
        {
           string sql = "DELETE FROM REQ_Notes WHERE NOT EXISTS (SELECT * FROM REQ_BookedEvents AS B WHERE B.StaffUsername=REQ_Notes.StaffUsername AND B.EventDateID=REQ_Notes.EventDateID) AND NOT EXISTS (SELECT * FROM REQ_PrebookedEvents AS P WHERE P.StaffUsername=REQ_Notes.StaffUsername AND P.EventID=REQ_Notes.EventID);";
           UtilityDb.ExecuteSql(sql, conn);

           sql = "DELETE FROM REQ_SupervisorRatings WHERE NOT EXISTS (SELECT * FROM REQ_BookedEvents AS B WHERE B.StaffUsername=REQ_SupervisorRatings.StaffUsername AND B.EventDateID=REQ_SupervisorRatings.EventDateID) AND NOT EXISTS (SELECT * FROM REQ_PrebookedEvents AS P WHERE P.StaffUsername=REQ_SupervisorRatings.StaffUsername AND P.EventID=REQ_SupervisorRatings.EventID);";
           UtilityDb.ExecuteSql(sql, conn);
        }

        //get all the Event Group in SAP catalog
        private void LoadEventGroupSAP()
        {
           string EventGroupID = string.Empty;
           string ParentID = string.Empty;
           string Title = string.Empty;
           List<EventGroup> groups = new List<EventGroup>();
           string sXMLInput = string.Empty;
           //DateTime dBeginDate = new DateTime(DateTime.Now.Year,1, 1);
           //DateTime dEndDate = new DateTime(9999, 12, 31);
           //string trxId = Request.QueryString["ID"].ToString();
           // string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxId);
           string sResultFromSAP = string.Empty;

           try
           {
              sXMLInput = "<COMM>" +
                          "<REMARK>BAPI_BUS_EVENTGROUP_LIST</REMARK>" +
                          "<UID>" + ConfigurationManager.AppSettings["UsernameAdmin"].ToString() + "</UID>" +
                          "<PF>" + sProfileID + "</PF>" +
                          "<RFC>BAPI_BUS_EVENTGROUP_LIST</RFC>" +
                          "<R_IF>1</R_IF>" +
                          "<R_OF>1</R_OF>" +
                          "<R_IT>0</R_IT>" +
                          "<R_OT>1</R_OT>" +
                          "<INPUT>" +
                              "<IFLD>" +
                                  "<OBJID>" + ConfigurationManager.AppSettings["TrainingGroup"].ToString() + "</OBJID>" +
                                  "<BEGIN_DATE>" + ConfigurationManager.AppSettings["BeginDate"].ToString() + "</BEGIN_DATE>" +
                                  "<END_DATE>" + ConfigurationManager.AppSettings["EndDate"].ToString() + "</END_DATE>" +
                                  "<PLVAR>01</PLVAR>" +
                              "</IFLD>" +
                              "<OFLD>" +
                                  "<RETURN>" +
                                      "<TRIGGER>1</TRIGGER>" +
                                  "</RETURN>" +
                              "</OFLD>" +
                              "<ITBL></ITBL>" +
                              "<OTBL>" +
                                  "<EVENTGROUP_LIST></EVENTGROUP_LIST>" +
                              "</OTBL>" +
                          "</INPUT>" +
                      "</COMM>";
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventGroupSAP", sXMLInput, "BAPI_BUS_EVENTGROUP_LIST");
              if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
              {
                 sResultFromSAP = oSAP.RETMSG.ToString();
                 XmlDocument xmlDocument = new XmlDocument();
                 xmlDocument.LoadXml(sResultFromSAP);
                 XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/COMM/OTBL/EVENTGROUP_LIST/DATA");
                 string sTYPE = xmlDocument.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                 if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                 {
                    if (xmlNodeList.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList)
                       {
                          EventGroupID = xmlNode["EGRID"].InnerText.ToString();
                          ParentID = xmlNode["FATHER"].InnerText.ToString();
                          Title = xmlNode["EGSTX"].InnerText.ToString();
                          EventGroup newGroup = new EventGroup(Convert.ToInt32(EventGroupID),
                          Convert.ToInt32(ParentID), Title);
                          groups.Add(newGroup);
                       }
                    }
                    using (UtilityDb db = new UtilityDb())
                    {
                       db.OpenConnectionESS();

                       db.PrepareInsert("REQ_EventGroups");
                       foreach (EventGroup item in groups)
                       {
                          DataRow row = db.Insert(null);
                          row["ID"] = item.ID;
                          row["TITLE"] = item.Title.ToString();
                          if (item.ParentID.HasValue)
                             row["ParentID"] = item.ParentID.Value;
                          else
                             row["ParentID"] = DBNull.Value;
                          db.Insert(row);
                       }
                       db.EndInsert();
                    }
                 }
              }
           }
           catch (Exception ex)
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventGroupSAP", "Catch", "BAPI_BUS_EVENTGROUP_LIST");
           }
           finally
           {
           }
        }

        private void LoadTrainingDetail()
        {
           try
           {
              List<int> ID = new List<int>();
              using (SqlConnection conn = UtilityDb.GetConnectionESS())
              {
                 string sql = "SELECT * FROM REQ_EventGroups";
                 SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                 while (dr.Read())
                 {
                    int IDs = Convert.ToInt32(dr["ID"].ToString());
                    int ParentID = Convert.ToInt32(dr["ParentID"].ToString());
                    ID.Add(IDs);
                    if (ParentID == 0)
                       ID.Remove(IDs);
                 }
              }
              foreach (int EventGroupID in ID)
              {
                 //LoadEventTypeInGroupList(EventTypeID);
                 LoadTrainingDetail(EventGroupID);
              }
           }
           catch
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventTypeInGroup", "Catch", "REQ_EventGroups table");
           }
        }

        private void LoadTrainingDetail(int EventGroupID)
        {
           string sXMLInput = string.Empty;
           //DateTime dBeginDate = new DateTime(DateTime.Now.Year, 1, 1);
           //DateTime dEndDate = new DateTime(9999, 12, 31);
           //string trxId = Request.QueryString["ID"].ToString();
           //string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxId);
           string sResultFromSAP = string.Empty;
           try
           {
              sXMLInput = "<COMM>" +
                          "<REMARK>ZRFC_GET_EVENTTYPE_LIST</REMARK>" +
                          "<UID>" + ConfigurationManager.AppSettings["UsernameAdmin"].ToString() + "</UID>" +
                          "<PF>" + sProfileID + "</PF>" +
                          "<RFC>ZRFC_GET_EVENTTYPE_LIST</RFC>" +
                          "<R_IF>1</R_IF>" +
                          "<R_OF>0</R_OF>" +
                          "<R_IT>1</R_IT>" +
                          "<R_OT>1</R_OT>" +
                          "<INPUT>" +
                              "<IFLD>" +
                                  "<I_PLVAR>01</I_PLVAR>" +
                                  "<I_OBJID>" + EventGroupID + "</I_OBJID>" +
                                  "<I_BEGIN_DATE>" + ConfigurationManager.AppSettings["BeginDate"].ToString() + "</I_BEGIN_DATE>" +
                                  "<I_END_DATE>" + ConfigurationManager.AppSettings["EndDate"].ToString() + "</I_END_DATE>" +
                              "</IFLD>" +
                              "<OFLD></OFLD>" +
                              "<ITBL>" +
                                  "<T_HRP1001></T_HRP1001>" +
                                  "<T_EVENTTYPE_LIST></T_EVENTTYPE_LIST>" +
                                  "<T_EVENT_LIST></T_EVENT_LIST>" +
                                  "<T_PREBOOK_TAB></T_PREBOOK_TAB>" +
                                  "<T_PARTIC_TAB></T_PARTIC_TAB>" +
                                  "<T_RETURN></T_RETURN>" +
                              "</ITBL>" +
                              "<OTBL>" +
                                  "<T_HRP1001></T_HRP1001>" +
                                  "<T_EVENTTYPE_LIST></T_EVENTTYPE_LIST>" +
                                  "<T_EVENT_LIST></T_EVENT_LIST>" +
                                  "<T_PREBOOK_TAB></T_PREBOOK_TAB>" +
                                  "<T_PARTIC_TAB></T_PARTIC_TAB>" +
                                  "<T_RETURN></T_RETURN>" +
                              "</OTBL>" +
                          "</INPUT>" +
                      "</COMM>";
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventTypeInGroup", sXMLInput, "ZRFC_GET_EVENTTYPE_LIST");
              if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
              {
                 sResultFromSAP = oSAP.RETMSG.ToString();
                 XmlDocument xmlDocument = new XmlDocument();
                 List<int> EventTypeList = new List<int>();
                 xmlDocument.LoadXml(sResultFromSAP);
                 XmlNodeList xmlNodeList1 = xmlDocument.SelectNodes("/COMM/OTBL/T_HRP1001/DATA");
                 XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("/COMM/OTBL/T_EVENTTYPE_LIST/DATA");
                 XmlNodeList xmlNodeList3 = xmlDocument.SelectNodes("/COMM/OTBL/T_EVENT_LIST/DATA");
                 XmlNodeList xmlNodeList4 = xmlDocument.SelectNodes("/COMM/OTBL/T_PREBOOK_TAB/DATA");
                 XmlNodeList xmlNodeList5 = xmlDocument.SelectNodes("/COMM/OTBL/T_PARTIC_TAB/DATA");
                 XmlNodeList xmlNodeList6 = xmlDocument.SelectNodes("/COMM/OTBL/T_RETURN/DATA");
                 string sTYPE = xmlDocument.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                 if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                 {
                    #region "EventTypeInGroup"
                    EventTypeList.Clear();
                    foreach (XmlNode xmlNode in xmlNodeList1)
                    {
                       string EventTypeID = xmlNode["SOBID"].InnerText.ToString();
                       EventTypeList.Add(Convert.ToInt32(EventTypeID));
                    }
                    using (UtilityDb db = new UtilityDb())
                    {
                       db.OpenConnectionESS();
                       db.PrepareInsert("REQ_EventsInGroups");
                       foreach (int EventTypeID in EventTypeList)
                       {
                          DataRow row = db.Insert(null);
                          row["EventID"] = EventTypeID;
                          row["GroupID"] = EventGroupID;
                          db.Insert(row);
                       }
                       db.EndInsert();
                    }
                    #endregion
                    #region "EvenType"
                    if (xmlNodeList2.Count > 0)
                    {
                       int EventTypeID = 0;
                       string StartDate = "";
                       string EndDate = "";
                       foreach (XmlNode xmlNode in xmlNodeList2)
                       {
                          EventTypeID = Convert.ToInt32(xmlNode["ETYID"].InnerText.ToString());
                          Title = xmlNode["ETSTX"].InnerText.ToString();
                          StartDate = xmlNode["ETBEG"].InnerText.ToString();
                          EndDate = xmlNode["ETEND"].InnerText.ToString();

                          using (UtilityDb db = new UtilityDb())
                          {
                             db.OpenConnectionESS();
                             db.PrepareInsert("REQ_Events");
                             DataRow row = db.Insert(null);
                             row["ID"] = EventTypeID;
                             row["SAP_ID"] = EventTypeID;
                             row["Title"] = Title;
                             row["StartDate"] = StartDate;
                             row["EndDate"] = EndDate;
                             row["TempID"] = DBNull.Value;
                             row["UserDefined"] = DBNull.Value;
                             row["ExportedToExcel"] = DBNull.Value;
                             db.Insert(row);
                             db.EndInsert();
                          }
                       }
                       oCore.LogEvent("ImportSAPTraining.aspx", "Insert Data in REQ_Events ", "Catch", "BAPI_BUS_EVENTTYPE_LIST");
                    }
                    #endregion
                    #region "PrebookingLists"
                    if (xmlNodeList4.Count > 0)
                    {
                       string StaffID = "";
                       String PreBegin = "";
                       String PreEnd = "";
                       int EventTypeID = 0;
                       string StaffUsername = "";
                       //List<string> StaffIDList = new List<string>();

                       foreach (XmlNode xmlNode in xmlNodeList4)
                       {
                          EventTypeID = Convert.ToInt32(xmlNode["EVETID"].InnerText.ToString());
                          StaffID = xmlNode["PARID"].InnerText.ToString();
                          PreBegin = xmlNode["PREBEG"].InnerText.ToString();
                          PreEnd = xmlNode["PREEND"].InnerText.ToString();
                          string trimmedStaffID = StaffID.TrimStart('0');
                          StaffUsername = getUserName(trimmedStaffID);

                          if (!AlreadyBooked(StaffUsername, EventTypeID))
                          {
                             using (UtilityDb db = new UtilityDb())
                             {
                                db.OpenConnectionESS();
                                db.PrepareInsert("REQ_AttendanceToConfirm");
                                DataRow row = db.Insert(null);
                                row["EventID"] = EventTypeID;
                                row["StaffUsername"] = StaffUsername;
                                row["StartDate"] = PreBegin;
                                row["EndDate"] = PreEnd;
                                db.Insert(row);
                                db.EndInsert();
                             }
                          }
                       }

                    }
                    #endregion
                    #region "BookingLists"
                    if (xmlNodeList5.Count > 0)
                    {
                       string StaffUsername = "";
                       int EventDateID = 0;
                       foreach (XmlNode xmlNode in xmlNodeList5)
                       {
                          EventDateID = Convert.ToInt32(xmlNode["EVEID"].InnerText.ToString());
                          string StaffID = xmlNode["PARID"].InnerText.ToString();
                          string trimmedStaffID = StaffID.TrimStart('0');
                          StaffUsername = getUserName(trimmedStaffID);

                          using (UtilityDb db = new UtilityDb())
                          {
                             db.OpenConnectionESS();
                             db.PrepareInsert("REQ_AttendedEvents");
                             DataRow row = db.Insert(null);
                             row["EventDateID"] = EventDateID;
                             row["StaffUsername"] = StaffUsername;
                             db.Insert(row);
                             db.EndInsert();
                          }
                       }
                       oCore.LogEvent("ImportSAPTraining.aspx", "insert data in REQ_AttendedEvents ", "Catch", "ZRFC_GET_PARTICIPANTS");
                    }
                    #endregion
                 }
              }
           }
           catch (Exception ex)
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventTypeInGroupList", "Catch", "ZRFC_GET_EVENTTYPE_LIST");
           }
           finally
           {
           }
        }

        private void LoadEventDate()
        {
           try
           {
              //load EventID and EventType ID

              List<int> EventTypeIDs = new List<int>();
              using (SqlConnection conn = UtilityDb.GetConnectionESS())
              {
                 string sql = "SELECT * FROM REQ_Events";
                 SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                 while (dr.Read())
                 {
                    int eventTypeID = Convert.ToInt32(dr["ID"].ToString());
                    EventTypeIDs.Add(eventTypeID);
                 }
              }

              foreach (int eventTypeID in EventTypeIDs)
              {
                 LoadEventDateID(eventTypeID);
              }
           }
           catch
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventDate", "Catch", "REQ_Events Table");
           }
        }

        private void LoadEventDateID(int eventTypeID)
        {
           string sXMLInput = string.Empty;
           //DateTime dBeginDate = new DateTime(DateTime.Now.Year - 1,1, 1);
           //DateTime dEndDate = new DateTime(DateTime.Now.Year + 1, 12, 31);
           //string trxId = Request.QueryString["ID"].ToString();
           //string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxId);
           string sResultFromSAP = string.Empty;
           try
           {
              sXMLInput = "<COMM>" +
                          "<REMARK>BAPI_BUS_EVENT_LIST</REMARK>" +
                          "<UID>" + ConfigurationManager.AppSettings["UsernameAdmin"].ToString() + "</UID>" +
                          "<PF>" + sProfileID + "</PF>" +
                          "<RFC>BAPI_BUS_EVENT_LIST</RFC>" +
                          "<R_IF>1</R_IF>" +
                          "<R_OF>1</R_OF>" +
                          "<R_IT>0</R_IT>" +
                          "<R_OT>1</R_OT>" +
                          "<INPUT>" +
                              "<IFLD>" +
                                  "<PLVAR>01</PLVAR>" +
                                  "<OBJID>" + eventTypeID + "</OBJID>" +
                                  "<BEGIN_DATE>" + ConfigurationManager.AppSettings["BeginDate"].ToString() + "</BEGIN_DATE>" +
                                  "<END_DATE>" + ConfigurationManager.AppSettings["EndDate"].ToString() + "</END_DATE>" +
                                  "<LOCATION_ID></LOCATION_ID>" +
                                  "<LANGUAGE>EN</LANGUAGE>" +
                              "</IFLD>" +
                              "<OFLD>" +
                                  "<RETURN>" +
                                      "<TRIGGER>1</TRIGGER>" +
                                  "</RETURN>" +
                              "</OFLD>" +
                              "<ITBL></ITBL>" +
                              "<OTBL>" +
                                  "<EVENT_LIST></EVENT_LIST>" +
                              "</OTBL>" +
                          "</INPUT>" +
                      "</COMM>";
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventDateID", sXMLInput, "BAPI_BUS_EVENT_LIST");
              if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
              {
                 sResultFromSAP = oSAP.RETMSG.ToString();
                 XmlDocument xmlDocument = new XmlDocument();
                 Dictionary<int, int> EventList = new Dictionary<int, int>();

                 xmlDocument.LoadXml(sResultFromSAP);
                 XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/COMM/OTBL/EVENT_LIST/DATA");
                 string sTYPE = xmlDocument.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                 if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                 {
                    if (xmlNodeList.Count > 0)
                    {
                       EventList.Clear();
                       int EventID = 0;
                       foreach (XmlNode xmlNode in xmlNodeList)
                       {
                          EventID = Convert.ToInt32(xmlNode["EVEID"].InnerText.ToString());
                          LoadEventsinfo(EventID, eventTypeID);
                       }
                    }
                 }

              }
           }
           catch (Exception ex)
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventDateID", "Catch", "BAPI_BUS_EVENT_LIST");
           }
           finally
           {
           }
        }

        //Load EventDates Info based on EventID and EbentTypeID
        private void LoadEventsinfo(int EventID, int EventTypeID)
        {
           List<TrainingEvent> Events = new List<TrainingEvent>();
           string sXMLInput = string.Empty;
           //DateTime dBeginDate = new DateTime(DateTime.Now.Year - 1,1, 1);
           //DateTime dEndDate = new DateTime(DateTime.Now.Year + 1, 12, 31);
           //string trxId = Request.QueryString["ID"].ToString();
           //string sProfileID = LoadProfile(HttpContext.Current.User.Identity.Name, trxId);
           string sResultFromSAP = string.Empty;
           string Provider = "";
           string Currency = "";
           float Price = 0;
           string sTrainingType = string.Empty;
           string sTitle = string.Empty;
           try
           {
              sXMLInput = "<COMM>" +
                          "<REMARK>ZRFC_GET_EVENT_INFO</REMARK>" +
                          "<UID>" + ConfigurationManager.AppSettings["UsernameAdmin"].ToString() + "</UID>" +
                          "<PF>" + sProfileID + "</PF>" +
                          "<RFC>ZRFC_GET_EVENT_INFO</RFC>" +
                          "<R_IF>1</R_IF>" +
                          "<R_OF>0</R_OF>" +
                          "<R_IT>0</R_IT>" +
                          "<R_OT>1</R_OT>" +
                          "<INPUT>" +
                              "<IFLD>" +
                                  "<I_PLVAR>01</I_PLVAR>" +
                                  "<I_OBJID>" + EventID + "</I_OBJID>" +
                                  "<I_BEGIN_DATE>" + ConfigurationManager.AppSettings["BeginDate"].ToString() + "</I_BEGIN_DATE>" +
                                  "<I_END_DATE>" + ConfigurationManager.AppSettings["EndDate"].ToString() + "</I_END_DATE>" +
                                  "<I_LANGUAGE>EN</I_LANGUAGE>" +
                                  "<I_EVENTTYPEID>" + EventTypeID + "</I_EVENTTYPEID>" +
                              "</IFLD>" +
                              "<OFLD></OFLD>" +
                              "<ITBL></ITBL>" +
                              "<OTBL>" +
                                  "<T_EVENT_PRICE></T_EVENT_PRICE>" +
                                  "<T_EVENT_ORGAN></T_EVENT_ORGAN>" +
                                  "<T_EVENT_COST></T_EVENT_COST>" +
                                  "<T_RETURN></T_RETURN>" +
                                  "<T_EVENT_LIST></T_EVENT_LIST>" +
                                  "<T_EVENT_DESC></T_EVENT_DESC>" +
                                  "<T_EVENT_RESOU></T_EVENT_RESOU>" +
                                  "<T_EVENT_QUALI></T_EVENT_QUALI>" +
                                  "<T_HRP1037></T_HRP1037>" +
                                  "<T_EVENT_SCHEDULE></T_EVENT_SCHEDULE>" +
                              "</OTBL>" +
                          "</INPUT>" +
                      "</COMM>";
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventsInfo", sXMLInput, "ZRFC_GET_EVENT_INFO");
              if (oSAP.ExeProc(sXMLInput, ConfigurationManager.ConnectionStrings["ConnStr1"].ToString()) == true)
              {
                 sResultFromSAP = oSAP.RETMSG.ToString();
                 XmlDocument xDoc = new XmlDocument();
                 DateTime EndDate = DateTime.MinValue;
                 DateTime StartDate = DateTime.MaxValue;
                 xDoc.LoadXml(sResultFromSAP);
                 XmlNodeList xmlNodeList = xDoc.SelectNodes("/COMM/OTBL/T_EVENT_PRICE/DATA");
                 XmlNodeList xmlNodeList1 = xDoc.SelectNodes("/COMM/OTBL/T_EVENT_ORGAN/DATA");
                 XmlNodeList xmlNodeList2 = xDoc.SelectNodes("/COMM/OTBL/T_EVENT_COST/DATA");
                 XmlNodeList xmlNodeList3 = xDoc.SelectNodes("/COMM/OTBL/T_EVENT_LIST/DATA");
                 XmlNodeList xmlNodeList4 = xDoc.SelectNodes("/COMM/OTBL/T_HRP1037/DATA");
                 XmlNodeList xmlNodeList5 = xDoc.SelectNodes("/COMM/OTBL/T_EVENT_SCHEDULE/DATA");
                 string sTYPE = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                 if (sTYPE.ToUpper() != "E") //SUCCESSFULL
                 {
                    if (xmlNodeList.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList)
                       {
                          string sPrice = xmlNode["IKOST"].InnerText.ToString();
                          Price = float.Parse(sPrice);
                          Currency = xmlNode["IWAER"].InnerText.ToString();
                       }
                    }
                    if (xmlNodeList1.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList1)
                       {
                          Provider = xmlNode["ORGTX"].InnerText.ToString();
                       }
                    }
                    if (xmlNodeList3.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList3)
                       {
                          int iEventID = Convert.ToInt32(xmlNode["EVEID"].InnerText.ToString());
                          if (iEventID == EventID)
                             sTitle = xmlNode["EVSTX"].InnerText.ToString();
                       }
                    }
                    if (xmlNodeList4.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList4)
                       {
                          sTrainingType = xmlNode["LSTAR"].InnerText.ToString().ToUpper();
                       }
                    }
                    if (xmlNodeList5.Count > 0)
                    {
                       foreach (XmlNode xmlNode in xmlNodeList5)
                       {
                          string Day = xmlNode["DAYTXT"].InnerText.ToString().ToUpper();
                          DateTime EventDate = Convert.ToDateTime(xmlNode["EVDAT"].InnerText.ToString());
                          if (EventDate.CompareTo(EndDate) >= 0)
                          {
                             EndDate = EventDate;
                          }
                          if (EventDate.CompareTo(StartDate) <= 0)
                          {
                             StartDate = EventDate;
                          }
                       }
                    }
                    //string TrainingType = xDoc.GetElementsByTagName("TYPE").Item(0).InnerText.ToUpper();
                    using (UtilityDb db = new UtilityDb())
                    {
                       db.OpenConnectionESS();

                       db.PrepareInsert("REQ_EventDates");
                       DataRow row = db.Insert(null);
                       row["ID"] = EventID;
                       row["SAP_ID"] = EventID;
                       row["TempID"] = DBNull.Value;
                       row["EventId"] = EventTypeID;
                       row["StartDate"] = StartDate;
                       row["EndDate"] = EndDate;
                       if (Provider != "")
                          row["Provider"] = Provider;
                       else
                          row["Provider"] = DBNull.Value;
                       if (Currency != "")
                          row["Currency"] = Currency;
                       else
                          row["Currency"] = DBNull.Value;
                       if (Price != 0)
                          row["Price"] = Price;
                       else
                          row["Price"] = DBNull.Value;
                       if (sTrainingType != "")
                          row["TrainingType"] = sTrainingType;
                       else
                          row["TrainingType"] = DBNull.Value;
                       db.Insert(row);
                       db.EndInsert();
                    }
                 }
              }
           }
           catch (Exception ex)
           {
              oCore.LogEvent("ImportSAPTraining.aspx", "LoadEventsinfo", "Catch", "ZRFC_GET_EVENT_INFO");
           }
           finally
           {
           }
        }

        private bool AlreadyBooked(string StaffUsername, int EventTypeID)
        {
           using (SqlConnection conn = UtilityDb.GetConnectionESS())
           {
              string sql = string.Format("SELECT COUNT(*) AS Total FROM REQ_BookedEvents WHERE StaffUsername='{0}' AND EventID={1}",
                 StaffUsername, EventTypeID);
              int totalCount = (int)UtilityDb.ExecuteScalar(sql, conn);

              bool output = totalCount > 0;
              return output;
           }

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

        //get Username in Usermaster based on staffID
        private string getUserName(string ID)
        {
           string StaffUsername = "";
           using (SqlConnection conn = UtilityDb.GetConnectionUser())
           {
              string sql = "SELECT fldUid FROM UserMaster WHERE fldOid='" + ID + "'";
              SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
              while (dr.Read())
              {
                 StaffUsername = dr["fldUid"].ToString();
              }
           }
           return StaffUsername;
        }

    }
}