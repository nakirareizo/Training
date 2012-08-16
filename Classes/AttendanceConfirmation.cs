using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition.Classes
{
   public class AttendanceConfirmation : Base
   {
      public int EventID { get; set; }
      public string StaffUsername { get; set; }
      public string PreBegin { get; set; }
      public string PreEnd { get; set; }
      public DateTime LastUpdate { get; set; }
      public TrainingEvent PrebookedEvent { get; set; }
      public Staff PrebookedStaff { get; set; }

      public String EventDisplayName { get { return PrebookedEvent.DisplayName; } }

      public String StartDate { get { return PrebookedEvent.StartDate; } }
      public String EndDate { get { return PrebookedEvent.EndDate.ToString().Contains("9999") ? "-" : PrebookedEvent.EndDate; } }

      public override void LoadFromReader(System.Data.SqlClient.SqlDataReader dr)
      {
         EventID = Convert.ToInt32(dr["EventID"]);
         StaffUsername = dr["StaffUsername"].ToString();
         PreBegin = dr["StartDate"].ToString();
         PreEnd = dr["EndDate"].ToString();
         LastUpdate = Convert.ToDateTime( dr["LastUpdate"].ToString());
         base.LoadFromReader(dr);
      }

      public static void Delete(string staffUsername, int eventId)
      {
         using (SqlConnection conn = UtilityDb.GetConnectionESS())
         {
            string sql = "DELETE FROM REQ_AttendanceToConfirm WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND EventID=" + eventId.ToString();
            UtilityDb.ExecuteSql(sql, conn);
         }
      }

      public static List<AttendanceConfirmation> GetAll(string staffUsername, int? eventId,
           bool ShowWithEventDatesOnly)
      {
         List<AttendanceConfirmation> unfiltered = new List<AttendanceConfirmation>();
         using (SqlConnection conn = UtilityDb.GetConnectionESS())
         {
            string sql = "SELECT * FROM REQ_AttendanceToConfirm";
            bool hasUsername = !String.IsNullOrEmpty(staffUsername);
            bool hasEventId = eventId.HasValue;
            if (hasUsername || hasEventId)
               sql += " WHERE ";
            if (hasUsername)
               sql += "UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' ";
            if (hasUsername && hasEventId)
               sql += " AND ";
            if (hasEventId)
               sql += "EventID=" + eventId.Value.ToString() + " ";

            SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);

            while (dr.Read())
            {
               AttendanceConfirmation ac = new AttendanceConfirmation();
               ac.LoadFromReader(dr);

               unfiltered.Add(ac);
            }
            dr.Close();
         }

         // retrieve data objects
         List<AttendanceConfirmation> output = new List<AttendanceConfirmation>();
         foreach (AttendanceConfirmation ac in unfiltered)
         {
            if (ShowWithEventDatesOnly)
            {
               ac.LoadDataObjects();
               if (ac.PrebookedEvent.EventDates.Count > 0)
                  output.Add(ac);
            }
            else
            {
               ac.LoadDataObjects();
               output.Add(ac);
            }
         }

         return output;
      }

      public static AttendanceConfirmation GetFromEventTypeID(string staffUsername, int? eventId)
      {
         using (SqlConnection conn = UtilityDb.GetConnectionESS())
         {
            
            string sql = "SELECT * FROM REQ_AttendanceToConfirm";
            bool hasUsername = !String.IsNullOrEmpty(staffUsername);
            bool hasEventId = eventId.HasValue;
            if (hasUsername || hasEventId)
               sql += " WHERE ";
            if (hasUsername)
               sql += "UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' ";
            if (hasUsername && hasEventId)
               sql += " AND ";
            if (hasEventId)
               sql += "EventID=" + eventId.Value.ToString() + " ";
            using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
            {
               while (dr.Read())
               {
                  AttendanceConfirmation output = new AttendanceConfirmation();
                  output.LoadFromReader(dr);
                  return output;
               }
            }
         }
         return null;
      }

      private void LoadDataObjects()
      {
         TrainingEvent te = new TrainingEvent();
         te = TrainingEvent.GetById(EventID);
         te.EventDates = EventDate.GetAll(EventID);
         PrebookedEvent = te;
         PrebookedStaff = Staff.GetFromUsername(StaffUsername);
      }

      public override string DisplayName
      {
         get
         {
            return "Event " + EventID.ToString() + ", Staff " + StaffUsername;
         }
      }

   }
}
