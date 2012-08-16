using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;
using TrainingRequisition.Classes;
using BUILD.Training.ClassLibrary.Custom;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.UserControls;

namespace TrainingRequisition
{
    public partial class ConfirmAttendance : System.Web.UI.Page
    {
        private const int ColumnCheckbox = 0;
        private const int ColumnEventDates = 2;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowEvents();
            }
        }

        private void ShowEvents()
        {
            string staffUsername = User.Identity.Name;
            List<AttendanceConfirmation> unfiltered = AttendanceConfirmation.GetAll(staffUsername, null,true);


            List<AttendanceConfirmation> toShow = new List<AttendanceConfirmation>();
            foreach (AttendanceConfirmation ac in unfiltered)
            {
               bool hasFutureDate = false;
               List<EventDate> lstEd = ac.PrebookedEvent.EventDates;
               foreach (EventDate ed in lstEd)
               {
                  if (ed.StartDate.CompareTo(DateTime.Now) >= 0)
                  {
                     hasFutureDate = true;
                     break;
                  }
                  
               }
               if (hasFutureDate)
                  toShow.Add(ac);
            }

            gvEvents.DataSource = toShow;
            gvEvents.DataBind();

            pnlEvents.Visible = toShow.Count > 0;
            if (toShow.Count == 0)
                ShowMessage("You currently have no events to confirm. Please come again later.");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ExecuteSql("DELETE FROM REQ_AttendanceToConfirm WHERE StaffUsername='" + User.Identity.Name + "'", conn);
            }

            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_AttendanceToConfirm");
                DataRow row = db.Insert(null);
                row["StaffUsername"] = User.Identity.Name;
                row["EventId"] = 1;
                db.Insert(row);
                db.EndInsert();
            }
        }

        protected void gvEvents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                RadioButtonList rbEventDates = (RadioButtonList)row.Cells[ColumnEventDates].FindControl("rbEventDates");
                AttendanceConfirmation ac = (AttendanceConfirmation)row.DataItem;
                foreach (EventDate ed in ac.PrebookedEvent.EventDates)
                {
                    string strDates = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                        ed.StartDate, ed.EndDate);
                    ListItem item = new ListItem(strDates, ed.Id.ToString());
                    rbEventDates.Items.Add(item);
                }
            }
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll(true);
        }

        private void SelectAll(bool select)
        {
            foreach (GridViewRow row in gvEvents.Rows)
            {
                CheckBox cb = (CheckBox)row.Cells[0].FindControl("chkSelect");
                cb.Checked = select;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            SelectAll(false);
        }

        protected bool HasUnselectedDates()
        {
            foreach (GridViewRow row in gvEvents.Rows)
            {
                CheckBox cb = (CheckBox)row.Cells[ColumnCheckbox].FindControl("chkSelect");
                if (cb.Checked)
                {
                    RadioButtonList rbEventDates = (RadioButtonList)row.Cells[ColumnEventDates].FindControl("rbEventDates");
                    if (rbEventDates.SelectedIndex == -1)
                        return true;
                }
            }
            return false;
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            // validate that dates have been selected
            if (HasUnselectedDates())
            {
                ShowMessage("Please select a date for all the events you are confirming.");
                return;
            }
           using(UtilityDb db = new UtilityDb())
           {
              db.OpenConnectionESS();
              db.PrepareInsert("REQ_BookedEvents");
              foreach (GridViewRow row in gvEvents.Rows)
              {
                 CheckBox cb = (CheckBox)row.Cells[0].FindControl("chkSelect");
                 if (cb.Checked)
                 {
                    RadioButtonList rbEventDates = (RadioButtonList)row.Cells[ColumnEventDates].FindControl("rbEventDates");
                    int eventDateId = Convert.ToInt32(rbEventDates.SelectedValue);
                    string staffUsername = gvEvents.DataKeys[row.RowIndex].Values["StaffUsername"].ToString();
                    int eventId = Convert.ToInt32(gvEvents.DataKeys[row.RowIndex].Values["EventId"].ToString());

                    Staff staff = Staff.GetFromUsername(staffUsername);
                    Staff supervisor = staff.GetSupervisor();

                    // check whether a booking already exists for this event date 
                    List<BookedEvent> existing = BookedEvent.GetAll(staffUsername, null);
                    bool exists = false;
                    foreach (BookedEvent be in existing)
                    {
                       if (be.EventDateId == eventDateId)
                       {
                          exists = true;
                          break;
                       }
                    }

                    // not found? convert to booked event
                    if (!exists)
                    {
                       BookedEvent be = new BookedEvent();
                       be.RequestDate = DateTime.Now;
                       be.StaffUsername = staffUsername;
                       be.RequesterUsername = staffUsername;
                       be.Stage = 1; // submit to supervisor
                       be.EventDateId = eventDateId;
                       be.EventId = eventId;
                       DataRow dataRow = db.Insert(null);
                       be.Save(dataRow);
                       db.Insert(dataRow);
                       // send notification to supervisor
                       List<EventDate> lstEd= new List<EventDate>();
                       EventDate ed = EventDate.GetFromId(eventDateId);
                       lstEd.Add(ed);
                       BookingEventSelector.SendNotificationEmail(lstEd, Server, staffUsername);

                    }
                   
                    // delete from db
                    AttendanceConfirmation.Delete(staffUsername, eventId);
                 }
                 db.EndInsert();
              }
            }

            ShowEvents();
            string sScript0 = "window.alert('Your Prebook(s) Training confirmed and submitted.');";
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Prebook.aspx", sScript0, true);
            return;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Visible = !string.IsNullOrEmpty(message);
            lblMessage.Text = message;
        }

        /// <summary>
        /// Todo: send confirmation to SAP
        /// </summary>
        /// <param name="staffUsername"></param>
        /// <param name="eventId"></param>
        /// <param name="eventDateId"></param>

        protected void btnCancel_Click(object sender, EventArgs e)
        {
           // validate that dates have been selected
           if (HasUnselectedDates())
           {
              ShowMessage("Please select a date for all the Training you are cancelling.");
              return;
           }
           //Insert into DB
           #region "DELETE INSAP and DELETE in DB"
              string errorMessage = "";
              string TotalerrorMessage = "";
              foreach (GridViewRow row in gvEvents.Rows)
              {
                 CheckBox cb = (CheckBox)row.Cells[0].FindControl("chkSelect");
                 if (cb.Checked)
                 {
                    RadioButtonList rbEventDates = (RadioButtonList)row.Cells[ColumnEventDates].FindControl("rbEventDates");
                    int eventDateId = Convert.ToInt32(rbEventDates.SelectedValue);
                    string staffUsername = gvEvents.DataKeys[row.RowIndex].Values["StaffUsername"].ToString();
                    int eventId = Convert.ToInt32(gvEvents.DataKeys[row.RowIndex].Values["EventId"].ToString());
                    AttendanceConfirmation ac = AttendanceConfirmation.GetFromEventTypeID(staffUsername, eventId);
                    Staff staff = Staff.GetFromUsername(staffUsername);
                    Staff supervisor = staff.GetSupervisor();
                    string trxId = Request.QueryString["ID"].ToString();
                    SAPHeitechREQ.DeleteConfirmAttendance(staff, eventId, trxId, ref errorMessage);
                    if (errorMessage.Contains("SUCCESS"))
                    {
                       // delete from db
                       AttendanceConfirmation.Delete(staffUsername, eventId);
                    }
                    else
                    {
                       TotalerrorMessage += errorMessage;
                    }
                 }
              }
           #endregion
              if (!string.IsNullOrEmpty(TotalerrorMessage))
              {
                 string sScript0 = "window.alert('"+TotalerrorMessage+" deleted.');";
                 ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ConfirmAttendance.aspx", sScript0, true);
                 return;
              }
              else
              {
                 ShowEvents();
                 string sScript0 = "window.alert('Your Prebook(s) Training deleted.');";
                 ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ConfirmAttendance.aspx", sScript0, true);
                 return;
              }
        }

    }
}
