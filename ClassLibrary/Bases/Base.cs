using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;

namespace TrainingRequisition.ClassLibrary.Bases
{
    [Serializable]
    public abstract class Base
    {
        public int Id { get; set; }
        public int? SAPId { get; set; }
        public abstract string DisplayName { get; }

        Guid guid = Guid.NewGuid();
        public Guid UniqueIdentifier { get { return guid; } set { guid = value; } }

        bool isNew = true;
        public bool IsNew { get { return isNew; } set { isNew = value; } }

        bool isEdited = false;
        public bool IsEdited { get { return isEdited; } set { isEdited = value; } }

        public void Copy(Base source)
        {
            UniqueIdentifier = source.UniqueIdentifier;
            IsNew = source.IsNew;
            IsEdited = source.IsEdited;
            SAPId = source.SAPId;
        }

        public virtual void LoadFromReader(SqlDataReader dr)
        {
            try
            {
                // just try to get it from the standard ID column
                Id = Convert.ToInt32(dr["ID"]);
            }
            catch (Exception)
            {
            }

            try
            {
                UniqueIdentifier = (Guid)(dr["TempID"]);
            }
            catch (Exception)
            {
            }

            try
            {
                if (dr["TempID"] != DBNull.Value)
                    SAPId = Convert.ToInt32(dr["TempID"]);
            }
            catch (Exception)
            {
            }

            IsNew = false;
            IsEdited = false;
        }

        public virtual void Save(DataRow row)
        {

            try
            {
                row["TempID"] = UniqueIdentifier;
            }
            catch (Exception)
            {
            }

            try
            {
                if (SAPId.HasValue)
                    row["SAP_ID"] = SAPId;
                else
                    row["SAP_ID"] = DBNull.Value;
            }
            catch (Exception)
            {
            }

            row["ID"] = Id;
            IsNew = false;
            IsEdited = false;
        }

        public static void SaveDeleted(List<Base> source, string tableName, ref string errorMessage)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                foreach (Base item in source)
                {
                    try
                    {
                        string displayName = item.DisplayName;
                        string sql = "DELETE FROM " + tableName + " WHERE ID=" + item.Id.ToString();
                        UtilityDb.ExecuteSql(sql, conn);
                    }
                    catch (Exception ex)
                    {
                        errorMessage += "Error deleting " + item.DisplayName + ". If it contains other items, please delete them first. Message: " + ex.Message;
                    }
                }
            }
        }

        public static void SaveUpdated(List<Base> source, string tableName, ref string errorMessage)
        {
            if (source.Count == 0)
                return;

            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();

                // gather all the IDs of the edited items
                List<int> ids = new List<int>();
                foreach (Base item in source)
                    ids.Add(item.Id);

                string sql = "SELECT * FROM " + tableName + " WHERE ID in " +
                    UtilityDb.InClause(ids);

                db.PrepareUpdate(sql);
                foreach (Base item in source)
                {
                    DataRow row = db.FindRowFromID(item.Id, "ID");
                    if (row != null)
                        item.Save(row);
                }
                db.EndUpdate();
            }
        }

        protected void GetFromTempID(string tablename)
        {
            try
            {
                using (SqlConnection conn = UtilityDb.GetConnectionESS())
                {
                    string sql = "SELECT * FROM " + tablename + " WHERE TempID='" + UniqueIdentifier.ToString() + "'";
                    SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                    while (dr.Read())
                    {
                        LoadFromReader(dr);
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
        }

        public void InsertNew(string tablename)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert(tablename);
                DataRow row = db.Insert(null);
                Save(row);
                db.Insert(row);
                db.EndInsert();
            }

            // fetch back the ID of the eventObj that has been inserted
            GetFromTempID(tablename);
        }

        public void Update(string tablename)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                string sql = "SELECT * FROM " + tablename + " WHERE ID = " + Id;
                db.PrepareUpdate(sql);
                DataRow row = db.FindRowFromID(Id, "ID");
                Save(row);
                db.EndUpdate();
            }
        }

        public static void SaveNew(List<Base> source, string tableName, ref string errorMessage)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionESS();
                db.PrepareInsert(tableName);
                DataRow row = null;
                foreach (Base item in source)
                {
                    row = db.Insert(row);
                    item.Save(row);
                }
                if (row != null)
                    db.Insert(row);

                db.EndInsert();
            }
        }
    }
}

