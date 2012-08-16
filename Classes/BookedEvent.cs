using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition.Classes
{
    public class BookedEvent
    {
        public enum SAPStatuses
        {
            NotSubmitted = 1,
            SubmittedOK = 2,
            SubmittedError = 3
        }

        public string StaffUsername { get; set; }
        public string RequesterUsername { get; set; }
        public int EventDateId { get; set; }
        public int EventId { get; set; }
        public int Stage { get; set; }
        public int SAPStatus { get; set; }
        public DateTime RequestDate { get; set; }

        public static List<BookedEvent> GetAll(string staffUsername, int? stage)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='{0}'",
                     staffUsername.ToUpper());

                if (stage.HasValue)
                    sql += " AND stage=" + stage.Value;

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }
        public static List<BookedEvent> GetAllByStage(int stage)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE stage={0}",
                     stage);

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }
        public static List<BookedEvent> GetAllListing(string staffUsername, int? stage)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='{0}'",
                     staffUsername.ToUpper());
                if (stage.HasValue)
                    sql += " AND stage=" + stage.Value;
                //if (SelectedYear.HasValue)
                //    sql += " AND YEAR(RequestDate)=" + SelectedYear.Value;
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }
        public static List<EventDate> GetSubmissionList(int currentStage, string staffUsername)
        {
            List<EventDate> output = new List<EventDate>();

            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format(" Stage={0} AND UPPER(StaffUsername)='{1}' AND P.EventDateId=E.ID",
                    currentStage, staffUsername.ToUpper());

                // obtain a list of events to be approved, so we can return it
                string sql = "SELECT * FROM REQ_EventDates AS E WHERE Exists (SELECT * FROM REQ_BookedEvents as P WHERE " +
                    whereClause + ")";

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    EventDate pb = new EventDate();
                    pb.LoadFromReader(dr);
                    output.Add(pb);
                }
            }

            return output;
        }

        public static void UpdateSAPStatus(SAPStatuses status, string staffUsername, int eventDateId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format(" UPPER(StaffUsername)='{0}' AND EventDateID={1}",
                    staffUsername.ToUpper(), eventDateId);

                string sql = string.Format("UPDATE REQ_BookedEvents SET SAPStatus={0}, RequestDate=GetDate() WHERE {1}",
                    Convert.ToInt32(status).ToString(), whereClause);
                UtilityDb.ExecuteSql(sql, conn);
            }
        }

        public static void IncrementStage(string staffUsername, int eventDateId, int incrementBy)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format("UPPER(StaffUsername)='{0}' AND EventDateID={1}",
                    staffUsername.ToUpper(), eventDateId);

                string sql = string.Format("UPDATE REQ_BookedEvents SET Stage=Stage + ({0}) WHERE {1}",
                    incrementBy, whereClause);
                UtilityDb.ExecuteSql(sql, conn);
            }

        }

        public static void DeleteAll(string staffUsername, int stage)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                // insert existing records
                string sql = string.Format(
                    "DELETE FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='{0}' AND Stage={1}",
                    staffUsername.ToUpper(), stage);
                UtilityDb.ExecuteSql(sql, conn);
            }

        }

        public static BookedEvent FindByEventDateId(int dateId, List<BookedEvent> source)
        {
            foreach (BookedEvent item in source)
            {
                if (item.EventDateId == dateId)
                    return item;
            }
            return null;
        }

        public void Save(System.Data.DataRow row)
        {
            row["StaffUsername"] = StaffUsername;
            row["RequesterUsername"] = RequesterUsername;
            row["EventDateId"] = EventDateId;
            row["EventId"] = EventId;
            row["Stage"] = Stage;
            row["RequestDate"] = RequestDate;
            row["SAPStatus"] = SAPStatus;
        }

        public void LoadFromReader(SqlDataReader dr)
        {
            StaffUsername = dr["StaffUsername"].ToString();
            EventId = (int)dr["EventID"];
            EventDateId = (int)dr["EventDateId"];
            RequesterUsername = dr["RequesterUsername"].ToString();
            Stage = (int)dr["Stage"];
            if (dr["RequestDate"] != DBNull.Value)
                RequestDate = Convert.ToDateTime(dr["RequestDate"]);
            SAPStatus = (int)dr["SAPStatus"];
        }

        public static List<EventDate> GetAllEventDates(int stage)
        {
            List<EventDate> output = new List<EventDate>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_EventDates AS ED WHERE EXISTS (SELECT * FROM REQ_BookedEvents AS BE WHERE BE.EventDateID=ED.ID AND Stage=" + stage + ")";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    EventDate ed = new EventDate();
                    ed.LoadFromReader(dr);
                    output.Add(ed);
                }
            }
            return output;
        }

        internal static List<Staff> GetAllStaff(EventDate eventDate, int stage)
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT StaffUsername FROM REQ_BookedEvents WHERE EventDateId=" + eventDate.Id.ToString() + " AND Stage=" + stage;
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    try
                    {
                        Staff staff = Staff.GetFromUsername(dr["StaffUsername"].ToString());
                        output.Add(staff);
                    }
                    catch (Exception ex)
                    {
                        string Error = ex.Message;
                    }

                }
            }
            return output;
        }

        internal static void Reject(int eventDateID, List<string> staffUsernames)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                //string[] tables = {"REQ_BookedEvents", "REQ_SupervisorRatings", "REQ_Notes"};    // now we delete the records in the scheduler
                string[] tables = { "REQ_BookedEvents" };
                string inClause = UtilityDb.InClause(staffUsernames);

                // delete the items from database
                foreach (string table in tables)
                {
                    string SQL = "DELETE FROM " + table + " WHERE EventDateID=" + eventDateID + " AND StaffUsername in " +
                        inClause;
                    UtilityDb.ExecuteSql(SQL, conn);
                }

            }

        }

        public static BookedEvent GetByEventDateId(string staffUsername, int eventDateId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "' AND EventDateId=" + eventDateId.ToString();
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent output = new BookedEvent();
                    output.LoadFromReader(dr);
                    return output;
                }
            }
            return null;
        }

        internal static List<BookedEvent> GetAll()
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents");
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static string getRequestedUsername(EventDate toSubmit)
        {
            string Username = "";
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT RequesterUsername FROM REQ_BookedEvents WHERE EventDateID={0}", toSubmit.Id);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    Username = dr["RequesterUsername"].ToString();
                }
            }
            return Username;
        }

        internal static List<BookedEvent> getByEventID(int SelectedID)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE EventID={0}", SelectedID);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent be = new BookedEvent();
                    be.LoadFromReader(dr);
                    output.Add(be);
                }
            }
            return output;
        }

        internal static List<BookedEvent> getByUsername(string Username)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='{0}'",
                     Username.ToUpper());

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static List<BookedEvent> GetAllByStaffStage(string StaffUsername, int stage)
        {
            List<BookedEvent> output = new List<BookedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_BookedEvents WHERE UPPER(StaffUsername)='{0}' AND stage={1}",StaffUsername,
                     stage);

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    BookedEvent obj = new BookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }
    }
}
