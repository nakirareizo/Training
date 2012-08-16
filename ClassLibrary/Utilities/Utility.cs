using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace TrainingRequisition.ClassLibrary.Utilities
{
    public class Utility
    {
        public const string ASM_PTAEndDaysAfterEvent = "ASM_PTAEndDaysAfterEvent";
        public const string ASM_PTABeginDaysAfterEvent = "ASM_PTABeginDaysAfterEvent";
        public const string REQ_BypassPTACheck = "REQ_BypassPTACheck";

        public static List<SubClass> ConvertListToSubClass<ParentClass, SubClass>(List<ParentClass> source)
    where SubClass : ParentClass
        {
            List<SubClass> output = new List<SubClass>();
            foreach (var item in source)
            {
                SubClass convertedItem = (SubClass)item;
                output.Add(convertedItem);
            }
            return output;
        }

        public static List<ParentClass> ConvertListToParent<ParentClass, SubClass>(List<SubClass> source)
    where SubClass : ParentClass
        {
            List<ParentClass> output = new List<ParentClass>();
            foreach (var item in source)
            {
                ParentClass convertedItem = (ParentClass)item;
                output.Add(convertedItem);
            }
            return output;
        }

        public static List<string> GetConfigurationValues(string name, string subName)
        {
            List<string> values = new List<string>();
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM ESS_Configuration WHERE Name='" + name + "' ";
                if (!string.IsNullOrEmpty(subName))
                    sql += " AND SubName='" + subName + "' ";

                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    string output = dr["value"].ToString();
                    if (values != null)
                        values.Add(output);
                }
            }
            return values;
        }
        public static string GetConfiguration(string name, string subName)
        {
            List<string> values = GetConfigurationValues(name, subName);
            if (values.Count == 0)
                return null;
            return values[0];
        }

        public static void SetConfiguration(string name, string subName, string value)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                // construct where clause first
                string where =
                    "Name='" + name + "'";
                if (!string.IsNullOrEmpty(subName))
                    where += " AND SubName='" + subName + "'";
                else
                    where += " AND SubName IS NULL";

                // try to update first. Most likely it exists. If it does not,
                // insert a new entry
                string select = "SELECT * FROM ESS_Configuration WHERE " + where;
                bool exists = false;
                using (SqlDataReader dr = UtilityDb.GetDataReader(select, conn))
                {
                   while (dr.Read())
                      exists = true;
                }

                if (exists)
                    UtilityDb.ExecuteSql("UPDATE ESS_Configuration SET Value='" + value + "' WHERE " + where, conn);
                else
                {
                    // insert a new one
                    using (UtilityDb db = new UtilityDb())
                    {
                        db.OpenConnectionESS();
                        db.PrepareInsert("ESS_Configuration");
                        DataRow row = db.Insert(null);
                        row["Name"] = name;
                        if (!string.IsNullOrEmpty(subName))
                            row["SubName"] = subName;
                        row["Value"] = value;
                        db.Insert(row);
                        db.EndInsert();
                    }
                }
            }                
                
            }
        }
    
}
