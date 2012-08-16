using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Bases;
using System.Data;

namespace TrainingRequisition.Classes
{
    public class Rating
    {
        public int Value { get; set; }


        public List<Rating> GetAll()
        {
            List<Rating> output = new List<Rating>();
            for (int i = 0; i <= 5; i++)
            {
                Rating newRating = new Rating();
                newRating.Value = i;
                output.Add(newRating);
            }
            return output;
        }

        public static bool HasUnrated(List<Rating> ratings)
        {
            bool hasUnrated = false;
            foreach (Rating item in ratings)
            {
                if (item.Value == 0)
                {
                    hasUnrated = true;
                    break;
                }
            }
            return hasUnrated;
        }

        public static List<Rating> GetSupervisorRatings(string staffUsername, int? eventId, int? eventDateId)
        {
            List<Rating> output = new List<Rating>();
            string sql = "SELECT * FROM REQ_SupervisorRatings WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND ";
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
                {
                    Rating obj = new Rating();
                    obj.Value = Convert.ToInt32(dr["Value"]);
                    output.Add(obj);
                }
            }
            return output;
        }

        public static void SaveSupervisorRatings(List<Rating> ratings, string staffUsername, int? eventId, int? eventDateId)
        {
            DeleteSupervisorRatings(staffUsername, eventId, eventDateId);

            if (ratings.Count == 0)
                return;

            // insert one by one
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert("REQ_SupervisorRatings");
                DataRow row = null;
                foreach (var rating in ratings)
                {
                    row = db.Insert(row);
                    row["Value"] = rating.Value;
                    if (eventId.HasValue)
                        row["EventID"] = eventId;
                    if (eventDateId.HasValue)
                        row["EventDateID"] = eventDateId;
                    row["StaffUsername"] = staffUsername;
                }
                if (row != null)
                    db.Insert(row);
                db.EndInsert();
            }
        }

        public static void DeleteSupervisorRatings(string staffUsername, int? eventId, int? eventDateId)
        {
            // delete all ratings first
            string sql = "DELETE FROM REQ_SupervisorRatings WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND ";
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
    }
}
