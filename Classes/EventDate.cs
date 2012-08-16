using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Bases;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;

namespace TrainingRequisition.Classes
{
    [Serializable]
    public class EventDate : Base
    {
        public int EventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private string EventDisplayName { get; set; }
        public string Provider { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string TrainingType { get; set; }

        // stores the parent event before it is saved to database
        // we use this because we cannot use EventId. This is
        // used in the case where the user adds an event on his own
        TrainingEvent temporaryEvent = null;
        public TrainingEvent TemporaryEvent
        {
            get
            {
                return temporaryEvent;
            }
            set
            {
                EventDisplayName = value.DisplayName;
                temporaryEvent = value;
            }
        }

        public void Copy(EventDate source)
        {
            EventId = source.EventId;
            StartDate = source.StartDate;
            EndDate = source.EndDate;
            base.Copy(source);
        }

        public void SetEventInfo(string eventDisplayName)
        {
            EventDisplayName = eventDisplayName;
        }

        public override string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(EventDisplayName))
                {
                    TrainingEvent parentEvent = TrainingEvent.GetById(EventId);
                    EventDisplayName = parentEvent.DisplayName;

                }

                string output = string.Format("{1:dd/MM/yyyy}-{2:dd/MM/yyyy}: {0}",
                    EventDisplayName, StartDate, EndDate);

                return output;
            }
        }
        public override void LoadFromReader(System.Data.SqlClient.SqlDataReader dr)
        {
            base.LoadFromReader(dr);
            EventId = Convert.ToInt32(dr["EventID"]);
            StartDate = Convert.ToDateTime(dr["StartDate"]);
            EndDate = Convert.ToDateTime(dr["EndDate"]);

            if (dr["Provider"] != DBNull.Value)
                Provider = dr["Provider"].ToString();
            if (dr["Price"] != DBNull.Value)
                Price = Convert.ToDouble(dr["Price"].ToString());
            if (dr["Currency"] != DBNull.Value)
                Currency = dr["Currency"].ToString();
            if (dr["TrainingType"] != DBNull.Value)
                TrainingType = dr["TrainingType"].ToString();
        }


        internal static EventDate GetById(int eventDateId)
        {
            string sql = "SELECT * FROM REQ_EventDates WHERE ID = " + eventDateId;
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    EventDate obj = new EventDate();
                    obj.LoadFromReader(dr);
                    return obj;
                }
                return null;
            }
        }


        public static EventDate GetFromId(int id)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_EventDates WHERE ID=" + id.ToString();
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    EventDate ed = new EventDate();
                    ed.LoadFromReader(dr);
                    return ed;
                }
            }
            return null;
        }

        internal static List<EventDate> GetAll(int eventId)
        {
            List<EventDate> output = new List<EventDate>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_EventDates WHERE EventID=" + eventId.ToString();
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

        public bool IsDueForPTA()
        {
            return true;

            // check the elapsed days
            int PTADaysAfterEvent = Convert.ToInt32
                (Utility.GetConfiguration
                (Utility.ASM_PTABeginDaysAfterEvent, null));
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(EndDate);

            if (elapsed.TotalDays > PTADaysAfterEvent)
                return true;

            return false;
        }

        public bool IsOverdueForPTA()
        {
            // check the elapsed days
            int PTADaysAfterEvent = 
                Convert.ToInt32(Utility.GetConfiguration(
                Utility.ASM_PTAEndDaysAfterEvent, null));

            DateTime now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(EndDate);

            if (elapsed.TotalDays > PTADaysAfterEvent)
                return true;

            return false;
        }
    }
}
