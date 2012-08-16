using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;
using TrainingRequisition.ClassLibrary.Bases;

namespace TrainingRequisition.Classes
{
    [Serializable]
    public class TrainingEvent : Base
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool UserDefined { get; set; }
        public DateTime? ExportedToExcel { get; set; }
        public override string DisplayName
        {
            get
            {
                return Title;
            }
        }
        public List<EventDate> EventDates = new List<EventDate>();

        /// <summary>
        /// Get all available events in a eventObj group.
        /// </summary>
        /// <param name="eventGroup"></param>
        /// <returns></returns>
        public static List<TrainingEvent> GetAvailableEvents(EventGroup eventGroup,
            string staffUsername, string searchTitle, bool checkAgainstPrebookingTable)
        {
            List<int> eventGroupIds = new List<int>();
            CollectTreeIds(eventGroupIds, eventGroup);

            string sql = "SELECT * FROM REQ_Events AS C WHERE EXISTS " +
                "(SELECT * FROM REQ_EventsInGroups AS CG WHERE CG.EventId = C.ID ";

            // add in clause for the event groups
            if (eventGroupIds.Count > 0)
            {
                string inClause = "";
                int i = 0;
                foreach (int groupId in eventGroupIds)
                {
                    if (i > 0)
                        inClause += ",";
                    inClause += groupId.ToString();
                    i++;
                }
                inClause = "(" + inClause + ")";
                sql += " AND CG.GroupId IN " + inClause;
            }

            sql += ") ";

            // if search term is specified
            if (!String.IsNullOrEmpty(searchTitle))
            {
                sql += " AND C.Title LIKE '%" + searchTitle + "%' ";
            }

            // filter out events that have been pre-booked by this staff
            if (checkAgainstPrebookingTable)
            {
                string bookingTable = "REQ_PrebookedEvents";
                if (!string.IsNullOrEmpty(staffUsername))
                {
                    sql += string.Format(" AND NOT EXISTS(SELECT * FROM {0} AS PB WHERE PB.StaffUsername='{1}' AND PB.EventID=C.ID) ",
                        bookingTable, staffUsername);
                }
                sql += " ORDER BY C.Title";
            }

            // fetch output from database from the sql generated above.
            List<TrainingEvent> output = new List<TrainingEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    TrainingEvent eventObj = new TrainingEvent();
                    eventObj.LoadFromReader(dr);
                    output.Add(eventObj);
                }
            }
            return output;

        }

        public override void LoadFromReader(SqlDataReader dr)
        {
            base.LoadFromReader(dr);
            this.Title = dr["Title"].ToString();
            this.StartDate = dr["StartDate"].ToString();
            this.EndDate = dr["EndDate"].ToString();
            this.UserDefined = Convert.ToBoolean(dr["UserDefined"]);
            if (dr["ExportedToExcel"] != DBNull.Value)
                ExportedToExcel = Convert.ToDateTime(dr["ExportedToExcel"]);
            IsNew = false;
        }

        /// <summary>
        /// Collect all Ids in a eventgroup and its children and grandchildren
        /// </summary>
        /// <param name="eventGroupIds"></param>
        /// <param name="eventGroup"></param>
        private static void CollectTreeIds(List<int> eventGroupIds, EventGroup eventGroup)
        {
            eventGroupIds.Add(eventGroup.ID);
            foreach (EventGroup child in eventGroup.Children)
                CollectTreeIds(eventGroupIds, child);
        }

        public static List<TrainingEvent> GetByDate(EventGroup group, DateTime? dateFrom, DateTime? dateTo)
        {
            List<TrainingEvent> output = new List<TrainingEvent>();


            return output;
        }

        public static TrainingEvent GetById(int EventId)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM REQ_Events WHERE ID={0}", EventId);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    TrainingEvent output = new TrainingEvent();
                    output.LoadFromReader(dr);
                    return output;
                }
            }
            return null;
        }


        internal static List<TrainingEvent> GetAvailableEventsWithDates(EventGroup selectedEventGroup, string staffUsername,
            string searchTitle, DateTime fromDate, DateTime toDate)
        {
            List<TrainingEvent> output = GetAvailableEvents(selectedEventGroup, staffUsername, searchTitle, false);

            // attach dates to the events
            foreach (TrainingEvent parentEvent in output)
                parentEvent.AttachEventDates(fromDate, toDate, null, false, true, staffUsername);


            return output;
        }

        public void AttachEventDates(DateTime? fromDate, DateTime? toDate, int? eventDateId, bool bookedEventsOnly, bool unbookedEventsOnly, string bookingStaffUsername)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_EventDates AS ED WHERE ED.EventId=" + Id;
                string subsql = "SELECT * FROM REQ_BookedEvents AS BE WHERE BE.EventDateID=ED.ID AND BE.StaffUsername='" + bookingStaffUsername + "'";

                if (bookedEventsOnly)
                    sql += " AND EXISTS(" + subsql + ")";

                if (unbookedEventsOnly)
                    sql += " AND NOT EXISTS(" + subsql + ")";

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    EventDate date = new EventDate();
                    date.LoadFromReader(dr);

                    bool attach = false;

                    {
                    }

                    // if dates are unspecified automatically include all dates
                    if (!fromDate.HasValue && !toDate.HasValue && !eventDateId.HasValue)
                        attach = true;

                    // select by date ID
                    else if (eventDateId.HasValue && date.Id == eventDateId)
                        attach = true;

                    // dates are specified, so do the filtration
                    else if (date.StartDate.CompareTo(fromDate) >= 0 &&
                        date.StartDate.CompareTo(toDate) <= 0)
                        attach = true;

                    if (attach)
                    {
                        // attach extra info from the event
                        date.SetEventInfo(DisplayName);
                        EventDates.Add(date);
                    }
                }
            }
        }

        public static List<TrainingEvent> getAllTrainingTitle()
        {
            List<TrainingEvent> output = new List<TrainingEvent>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT DISTINCT Title,ID FROM REQ_Events");
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    TrainingEvent te = new TrainingEvent();
                    te.Title = dr["Title"].ToString();
                    te.ID = Convert.ToInt32(dr["ID"].ToString());
                    if (te != null)
                        output.Add(te);
                }
            }
            return output;
        }
    }
}
