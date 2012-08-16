using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace TrainingRequisition.Classes
{
    public class Note
    {
        public static void Save(string staffNotes, string supervisorNotes, string staffUsername,
            int? eventId, int? eventDateId)
        {
            Delete(staffUsername, eventId, eventDateId);

            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_Notes");
                DataRow row = db.Insert(null);
                Save(staffNotes, supervisorNotes, staffUsername, eventId, eventDateId, row);
                db.Insert(row);
                db.EndInsert();
            }
        }

        private static void Save(string staffNotes,
            string supervisorNotes,
            string staffUsername,
            int? eventId,
            int? eventDateId,
            DataRow row)
        {
            row["StaffNotes"] = staffNotes;
            row["SupervisorNotes"] = supervisorNotes;
            if (eventId.HasValue)
                row["EventID"] = eventId;
            if (eventDateId.HasValue)
                row["EventDateID"] = eventDateId;
            row["StaffUsername"] = staffUsername;
        }

        private static void Load(ref string staffNotes,
            ref string supervisorNotes,
            SqlDataReader dr)
        {
            if (dr["StaffNotes"] != DBNull.Value)
                staffNotes = dr["StaffNotes"].ToString();
            if (dr["SupervisorNotes"] != DBNull.Value)
                supervisorNotes = dr["SupervisorNotes"].ToString();
        }

        public static void Delete(string staffUsername,
           int? eventId, int? eventDateId)
        {
            string sql = "DELETE FROM REQ_Notes WHERE StaffUsername='" + staffUsername + "' AND ";
            if (eventId.HasValue)
                sql += "EventID = " + eventId.Value;
            //13/6/2012 added to carry forward justification
            if (eventId.HasValue && eventDateId.HasValue)
                sql += "OR EventDateID = " + eventDateId.Value;
            else if (eventDateId.HasValue)
                sql += "EventDateID = " + eventDateId.Value;

            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                UtilityDb.ExecuteSql(sql, conn);
            }
        }

        public static void GetNotes(string staffUsername,
           int? eventId, int? eventDateId, ref string staffNotes, ref string supervisorNotes)
        {
            List<Note> output = new List<Note>();
            string sql = "SELECT * FROM REQ_Notes WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND ";
            if (eventId.HasValue)
                sql += "EventID = " + eventId.Value;
            //13/6/2012 added to carry forward justification
            if (eventId.HasValue && eventDateId.HasValue)
                sql += "OR EventDateID = " + eventDateId.Value;
            else if (eventDateId.HasValue)
                sql += "EventDateID = " + eventDateId.Value;

            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                    Load(ref staffNotes, ref supervisorNotes, dr);
            }
        }
    }
}
