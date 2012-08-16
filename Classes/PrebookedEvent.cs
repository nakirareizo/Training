using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;

namespace TrainingRequisition.Classes
{
    public class PrebookedEvent
    {
        public enum SAPStatuses
        {
            NotSubmitted = 1,
            SubmittedOK = 2,
            SubmittedError = 3
        }

        public string StaffUsername { get; set; }
        public int EventId { get; set; }
        public bool IsAdHoc { get; set; }
        public string RequesterUsername { get; set; }
        public int Stage { get; set; }
        public DateTime? RequestDate { get; set; }
        public int SAPStatus { get; set; }

        public void LoadFromReader(SqlDataReader dr)
        {
            StaffUsername = dr["StaffUsername"].ToString();
            EventId = (int)dr["EventID"];
            RequesterUsername = dr["RequesterUsername"].ToString();
            Stage = (int)dr["Stage"];
            if (dr["RequestDate"] != DBNull.Value)
                RequestDate = Convert.ToDateTime(dr["RequestDate"]);
            IsAdHoc = (bool)dr["IsAdHoc"];
            SAPStatus = (int)dr["SAPStatus"];
        }

        public static List<PrebookedEvent> GetAll(string staffUsername, int stage, bool isAdHoc)
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents WHERE StaffUsername='{0}' AND Stage={1} AND IsAdHoc='{2}'",
                staffUsername, stage, isAdHoc.ToString());
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        public static List<PrebookedEvent> GetAllListing(string staffUsername, int? SelectedYear)
        {
           
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents WHERE StaffUsername='{0}'",
                staffUsername);
            //if (SelectedYear.HasValue)
            //    sql += " AND YEAR(RequestDate)=" + SelectedYear.Value;
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        public void Save(DataRow dest)
        {
            dest["StaffUsername"] = StaffUsername;
            dest["EventId"] = EventId;
            dest["RequesterUsername"] = RequesterUsername;
            dest["Stage"] = Stage;
            dest["IsAdHoc"] = IsAdHoc;
            if (RequestDate.HasValue)
                dest["RequestDate"] = RequestDate.Value;
        }

        public static void DeleteAll(string staffUsername, int stage, bool isAdHoc)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                // insert existing records
                string sql = string.Format("DELETE FROM REQ_PrebookedEvents WHERE StaffUsername='{0}' AND Stage={1} AND IsAdHoc='{2}'",
                    staffUsername, stage, isAdHoc);
                UtilityDb.ExecuteSql(sql, conn);
            }
        }

        public static PrebookedEvent FindByEventId(int eventId, List<PrebookedEvent> source)
        {
            foreach (PrebookedEvent find in source)
            {
                if (find.EventId == eventId)
                    return find;
            }
            return null;
        }

        public static List<TrainingEvent> GetSubmissionList(int currentStage, string staffUsername, bool isAdHoc)
        {
            List<TrainingEvent> output = new List<TrainingEvent>();

            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format(" Stage={0} AND StaffUsername='{1}' AND IsAdHoc='{2}' AND P.EventId=E.ID",
                    currentStage, staffUsername, isAdHoc);


                // obtain a list of events to be approved, so we can return it
                string sql = "SELECT * FROM REQ_Events AS E WHERE Exists (SELECT * FROM REQ_PrebookedEvents as P WHERE " +
                    whereClause + ") ORDER BY E.Title";

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    TrainingEvent pb = new TrainingEvent();
                    pb.LoadFromReader(dr);
                    output.Add(pb);
                }
            }

            return output;

        }

        public static void IncrementStage(int currentStage,
            string staffUsername, bool isAdHoc, int eventId, int incrementBy)
        {

            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format(" Stage={0} AND StaffUsername='{1}' AND IsAdHoc='{2}' AND EventID={3}",
                    currentStage, staffUsername, isAdHoc, eventId);

                string sql = string.Format("UPDATE REQ_PrebookedEvents SET Stage=Stage + ({0}) WHERE {1}",
                    incrementBy, whereClause);
                UtilityDb.ExecuteSql(sql, conn);
            }

        }

        internal static void UpdateSAPStatus(SAPStatuses status, string staffUsername, bool isAdHoc, int eventId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string whereClause = string.Format(" StaffUsername='{0}' AND IsAdHoc='{1}' AND EventID={2}",
                    staffUsername, isAdHoc, eventId);

                string sql = string.Format("UPDATE REQ_PrebookedEvents SET SAPStatus={0}, RequestDate=GetDate() WHERE {1}",
                    Convert.ToInt32(status).ToString(), whereClause);
                UtilityDb.ExecuteSql(sql, conn);
            }
        }

        internal static List<PrebookedEvent> GetAll()
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents");
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static List<PrebookedEvent> GetAllbyStage(int Stage)
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents Where Stage={0}",Stage);
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static string GetRequestedUsername(TrainingEvent toSubmit)
        {
            string Username = "";
            string sql = string.Format("SELECT RequesterUsername FROM REQ_PrebookedEvents Where EventID={0}", toSubmit.Id);
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    Username = dr["RequesterUsername"].ToString();
                }
            }
            return Username;
        }

        internal static List<PrebookedEvent> getByEventID(int SelectedID)
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents Where EventID={0}", SelectedID);
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static List<PrebookedEvent> getByUsername(string Username)
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents Where UPPER(StaffUsername)='{0}'", Username.ToUpper());
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }

        internal static List<PrebookedEvent> getAllbyStaffStage(string StaffUsername, int stage)
        {
            List<PrebookedEvent> output = new List<PrebookedEvent>();
            string sql = string.Format("SELECT * FROM REQ_PrebookedEvents Where UPPER(StaffUsername)='{0}' AND Stage={1}",StaffUsername, stage);
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    PrebookedEvent obj = new PrebookedEvent();
                    obj.LoadFromReader(dr);
                    output.Add(obj);
                }
            }
            return output;
        }
    }
}
