using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using TrainingRequisition.ClassLibrary.Entities;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.Classes
{
    public class AttendedEvent : Base
    {
        public int EventDateId { get; set; }
        public string StaffUsername { get; set; }

        public EventDate AttendedEventDate { get; set; }
        public TrainingEvent AttendedEventType { get; set; }
        public Staff AttendingStaff { get; set; }

        public override string DisplayName
        {
            get
            {
                return AttendedEventDate.DisplayName;
            }
        }

        public string EventName
        {
            get
            {
                return AttendedEventType.DisplayName;
            }
        }

        public string EventDatesDisplay
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                    AttendedEventDate.StartDate, AttendedEventDate.EndDate);
            }
        }

        public string Provider
        {
            get
            {
                return AttendedEventDate.Provider;
            }
        }
        public string Price
        {
            get
            {
                return AttendedEventDate.Currency + "" + AttendedEventDate.Price;
            }
        }

        public override void LoadFromReader(System.Data.SqlClient.SqlDataReader dr)
        {
            EventDateId = Convert.ToInt32(dr["EventDateID"]);
            StaffUsername = dr["StaffUsername"].ToString();
            base.LoadFromReader(dr);
        }

        public void LoadDataObjects()
        {
            AttendedEventDate = EventDate.GetById(EventDateId);
            AttendingStaff = Staff.GetFromUsername(StaffUsername);
            AttendedEventType = TrainingEvent.GetById(AttendedEventDate.EventId);
        }
        public static List<AttendedEvent> GetAllByUsername(string staffUsername)
        {
            List<AttendedEvent> output = new List<AttendedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "'";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AttendedEvent ae = new AttendedEvent();
                    ae.LoadFromReader(dr);
                    ae.LoadDataObjects();
                    output.Add(ae);
                }

            }
            return output;
        }
        //24/6/2012 added to use in Staff Listing page
        public static AttendedEvent GetByEventDateIDAndUsername(int EventDateID, string StaffUsername)
        {
            AttendedEvent output = new AttendedEvent();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_AttendedEvents WHERE UPPER(StaffUsername)='{0}' AND EventDateID={1}", StaffUsername.ToUpper(), EventDateID);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    output.LoadFromReader(dr);
                    output.LoadDataObjects();
                }

            }
            return output;
        }
        public static List<AttendedEvent> GetAll(string staffUsername, DateTime? fromDate, DateTime? toDate)
        {
            List<AttendedEvent> output = new List<AttendedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEventsHistory WHERE UPPER(StaffUsername)='" + staffUsername.ToUpper() + "'";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AttendedEvent ae = new AttendedEvent();
                    ae.LoadFromReader(dr);
                    ae.LoadDataObjects();
                    if (ae.FitsDates(fromDate, toDate))
                        output.Add(ae);
                }
            }

            // order by start date
            output.Sort(CompareByStartDate);
            return output;
        }

        private static int CompareByStartDate(AttendedEvent x, AttendedEvent y)
        {
            if (x.AttendedEventDate == null)
                x.LoadDataObjects();
            if (y.AttendedEventDate == null)
                y.LoadDataObjects();

            return x.AttendedEventDate.StartDate.CompareTo(y.AttendedEventDate.StartDate);
        }

        private bool FitsDates(DateTime? fromDate, DateTime? toDate)
        {
            if (AttendedEventDate == null)
                return false;

            if (fromDate.HasValue && toDate.HasValue)
            {
                return AttendedEventDate.StartDate.CompareTo(fromDate.Value) >= 0 &&
                    AttendedEventDate.StartDate.CompareTo(toDate.Value) <= 0;
            }

            if (fromDate.HasValue && AttendedEventDate.StartDate.CompareTo(fromDate.Value) < 0)
                return false;

            if (toDate.HasValue && AttendedEventDate.StartDate.CompareTo(toDate.Value) > 0)
                return false;

            return true;
        }

        public static List<AttendedEvent> GetUnsubmitted(string moduleName, int eventDateId)
        {
            List<AttendedEvent> output = new List<AttendedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS A WHERE A.EventDateID=" + eventDateId.ToString() + " AND NOT EXISTS " +
                    "(SELECT * FROM ASM_Submitted" + moduleName + " AS S WHERE S.EventDateID=A.EventDateID AND S.StaffUserName=A.StaffUserName)";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AttendedEvent ae = new AttendedEvent();
                    ae.LoadFromReader(dr);
                    ae.LoadDataObjects();
                    output.Add(ae);
                }
            }
            return output;
        }

        /// <summary>
        /// Get all events whose feedbacks have not been submitted
        /// </summary>
        /// <param name="p"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static List<AttendedEvent> GetUnsubmitted(string moduleName, string staffUsername)
        {
            List<AttendedEvent> output = new List<AttendedEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS A WHERE UPPER(A.StaffUsername)='" + staffUsername.ToUpper() + "' AND NOT EXISTS " +
                    "(SELECT * FROM ASM_Submitted" + moduleName + " AS S WHERE S.StaffUserName=A.StaffUserName AND S.EventDateID=A.EventDateID)";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    AttendedEvent ae = new AttendedEvent();
                    ae.LoadFromReader(dr);
                    ae.LoadDataObjects();
                    output.Add(ae);
                }
            }
            return output;
        }


        internal static List<int> GetAllEventDateID()
        {
            List<int> output = new List<int>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT EventDateID FROM REQ_AttendedEvents";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    int ID = Convert.ToInt32(dr["EventDateID"].ToString());
                    output.Add(ID);
                }

            }
            return output;
        }
    }
}
