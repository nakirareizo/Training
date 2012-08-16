using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.Classes
{
    [Serializable]
    public class EventGroup
    {
        public EventGroup Parent { get; set; }
        public List<EventGroup> Children { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public int? ParentID { get; set; }
        public static List<EventGroup> GetAll()
        {
            List<EventGroup> output = new List<EventGroup>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                output = GetChildrenOf(null, conn);
            }
            return output;
        }

        public EventGroup(int id, int parentID, string text)
        {
            ID = id;
            ParentID = parentID;
            Title = text;
        }

        public EventGroup FindById(int Id)
        {
            if (this.ID == Id)
                return this;

            EventGroup output = null;
            foreach (EventGroup child in this.Children)
            {
                EventGroup found = child.FindById(Id);
                if (found != null)
                {
                    output = found;
                    break;
                }
            }
            return output;
        }

        public static List<EventGroup> GetChildrenOf(EventGroup parent, SqlConnection conn)
        {
            string sql = "SELECT * FROM REQ_EventGroups WHERE ParentId ";
            if (parent == null)
                sql += "IS NULL OR ParentID=0";
            else
                sql += " = " + parent.ID.ToString();

            SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
            List<EventGroup> output = new List<EventGroup>();
            while (dr.Read())
            {
                EventGroup newGroup = LoadFromReader(dr);
                newGroup.Parent = parent;
                output.Add(newGroup);
            }
            dr.Close();

            foreach (EventGroup eventGroup in output)
                eventGroup.Children = GetChildrenOf(eventGroup, conn);

            return output;
        }

        private static EventGroup LoadFromReader(SqlDataReader dr)
        {
            int id = 0;
            int parentID = 0;
            string text = "";
            EventGroup output = new EventGroup(id, parentID, text);
            output.ID = (int)dr["ID"];
            output.Title = dr["Title"].ToString();
            return output;
        }



        internal static string getTitle(int EventID)
        {
            string Title = "";
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT Title FROM REQ_Events WHERE ID={0}",EventID);
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Title = dr["Title"].ToString().ToUpper();
                    }

                }
            }
            return Title;
        }
    }
}
