using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;

namespace TrainingRequisition.Classes
{
    public class TrainingType
    {
        public string Name { get; set; }

        public TrainingType(string name)
        {
            Name = name;
        }

        public static List<TrainingType> GetAll()
        {
           List<TrainingType> output = new List<TrainingType>();
           string sql = "SELECT DISTINCT TrainingType FROM REQ_EventDates";
           using (SqlConnection conn = UtilityDb.GetConnectionESS())
           {
              SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
              while (dr.Read())
              {
                 TrainingType tt = new TrainingType(dr["TrainingType"].ToString());
                 output.Add(tt);
              }
           }
           return output;
          
        }
    }
}
