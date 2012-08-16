using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Configuration;
using System.Xml;
using System.Data;
using BUILD.Training.ClassLibrary.Custom;

namespace BUILD.Training
{
   public partial class BookingListing : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         string StaffUsername = HttpContext.Current.User.Identity.Name;
         Staff staff =Staff.GetFromUsername(StaffUsername);
         string trxid = Request.QueryString["ID"].ToString();
         BindgvSAPList(staff.StaffIDPadded,trxid);
         //SAPHeitechREQ.LoadBookeTrainingList(staff.StaffIDPadded, trxid);
      }

      private void BindgvSAPList(string StaffIDPadded, string trxid)
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
                                "<OBJID>"+ StaffIDPadded +"</OBJID>" +
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
               if (sTYPE !="E") //SUCCESSFULL
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
                     gvSAPList.DataSource = dsNET;
                     gvSAPList.DataMember = "GVSAPLIST";
                     gvSAPList.DataBind();
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
   }
}