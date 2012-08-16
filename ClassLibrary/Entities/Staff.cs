using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data;

namespace TrainingRequisition.ClassLibrary.Entities
{
    [Serializable]
    public class Staff
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Department { get; set; }
        public string Subsidiary { get; set; }
        public string StaffID { get; set; }
        public string NameUserID
        {
            get
            {
                return Name + "-" + " [" + Username + "] " + "[" + StaffID + "]";
            }
        }
        public string UserNameID
        {
            get
            {
                return Username + "-" + " [" + Name + "] " + "[" + StaffID + "]";
            }
        }
        public string IDNameUser
        {
            get
            {
                return StaffID + "-" + " [" + Name + "] " + "[" + Username + "]";
            }
        }
        public string StaffIDPadded
        {
            get
            {
                string output = StaffID;
                const int length = 8;
                int toAdd = length - StaffID.Length;
                for (int i = 0; i < toAdd; i++)
                {
                    output = "0" + output;
                }
                return output;
            }
        }
        public string Email { get; set; }

        public static List<Staff> GetStaffUnder(string username, bool fetchForAdmin)
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "";

                // if username belongs to an admin, fetch all staff under the admin

                List<string> subsidiaries = Staff.GetAdminSubsidiaries(username);
                if (subsidiaries.Count > 0 && fetchForAdmin)
                {
                    using (SqlConnection conn2 = UtilityDb.GetConnectionUser())
                    {
                        foreach (string subsidiary in subsidiaries)
                        {
                            sql = "SELECT * FROM UserMaster WHERE UPPER(fldDept)='" + subsidiary.ToUpper() + "'";
                            using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn2))
                            {
                                while (dr.Read())
                                {
                                    Staff newStaff = Staff.GetFromUsername(dr["fldUid"].ToString());
                                    output.Add(newStaff);
                                }
                            }
                        }
                    }
                }

                // fetch from the supervisor table
                sql = "SELECT * FROM APRSupervisors WHERE UPPER(FormType)='TRAINING REQUISITION' AND UPPER(SupervisorUsername)='" +
                username.ToUpper() + "'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff newStaff = Staff.GetFromUsername(dr["StaffUsername"].ToString());
                        bool exists = false;
                        if (newStaff != null)
                        {
                            foreach (Staff staff in output)
                            {
                                if (staff.StaffID == newStaff.StaffID)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                                output.Add(newStaff);
                        }
                    }
                }

            }

            // sort alphabetically
            Staff.SortByName(output);
            return output;
        }

        public static void SortByName(List<Staff> unsorted)
        {
            unsorted.Sort(delegate(Staff a, Staff b)
            {
                return a.Name.CompareTo(b.Name);
            });
        }

        public static List<string> GetAdminSubsidiaries(string username)
        {
            List<string> subsidiaries = new List<string>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT * FROM APRAdmins WHERE UPPER(Username)='" + username.ToUpper() + "'";

                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        subsidiaries.Add(dr["Subsidiary"].ToString());
                    }
                }
            }
            return subsidiaries;
        }

        private void LoadFromReader(SqlDataReader dr)
        {
            Name = dr["fldLName"].ToString();
            Username = dr["fldUid"].ToString();
            Department = dr["fldDept"].ToString();
            Subsidiary = dr["fldDept"].ToString();
            StaffID = dr["fldOid"].ToString();
            Email = dr["fldEmail"].ToString();
        }

        public bool IsSupervisor()
        {
            List<Staff> staff = GetStaffUnder(Username, false);
            return staff.Count > 0;
        }
        public static bool IsSupervisor(string Username)
        {

            bool Supervisor = true;
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = string.Format("SELECT * FROM APRSupervisors WHERE UPPER(FormType)='TRAINING REQUISITION' AND UPPER(SupervisorUsername)='{0}' ", Username.ToUpper());
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        if (dr.HasRows)
                        {
                            Supervisor = true;
                        }
                        else
                        {
                            Supervisor = false;
                        }
                    }
                }
            }
            return Supervisor;
        }
        public static bool IsAdmin(string StaffUsername)
        {
            bool Output = false;
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = string.Format("SELECT * FROM APRAdmins WHERE UPPER(Username)='{0}' ", StaffUsername.ToUpper());
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        if (dr.HasRows)
                            Output = true;
                        else
                            Output = false;
                    }
                }
            }
            return Output;
        }

        public static Staff GetFromUsername(string username)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = "SELECT * FROM UserMaster WHERE UPPER(fldUid)='" + username.ToUpper() + "'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff output = new Staff();
                        output.LoadFromReader(dr);
                        return output;
                    }
                }
            }
            return null;
        }

        public Staff GetSupervisor()
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT * FROM APRSupervisors WHERE UPPER(FormType)='TRAINING REQUISITION' AND UPPER(StaffUsername)='" +
                    Username.ToUpper() + "'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff supervisor = Staff.GetFromUsername(dr["SupervisorUsername"].ToString());
                        output.Add(supervisor);
                    }
                }
            }

            if (output.Count > 0)
                return output[0];

            return null;
        }

        public static List<Staff> GetAllSupervisor()
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT DISTINCT SupervisorUsername FROM APRSupervisors WHERE UPPER(FormType)='TRAINING REQUISITION'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff supervisor = Staff.GetFromUsername(dr["SupervisorUsername"].ToString());
                        if (supervisor != null)
                            output.Add(supervisor);
                    }
                }
            }
            return output;
        }

        public static Staff GetSupervisorByUsername(string StaffUsername)
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT SupervisorUsername FROM APRSupervisors WHERE UPPER(FormType)='TRAINING REQUISITION' AND UPPER(StaffUsername)='" +
                    StaffUsername.ToUpper() + "'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff supervisor = Staff.GetFromUsername(dr["SupervisorUsername"].ToString());
                        output.Add(supervisor);
                    }
                }
            }

            if (output.Count > 0)
                return output[0];

            return null;
        }

        public static List<Staff> GetAll()
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = "SELECT * FROM UserMaster";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff staff = new Staff();
                        staff.LoadFromReader(dr);
                        output.Add(staff);
                    }
                }
            }
            return output;
        }

        public static List<string> GetStaffAssigned()
        {
            List<string> output = new List<string>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT DISTINCT StaffUsername FROM APRSupervisors Where UPPER(FormType)='TRAINING REQUISITION'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff staff = new Staff();
                        string username = LoadUsername(dr);
                        output.Add(username);
                    }
                }
            }
            return output;
        }

        public static string LoadUsername(SqlDataReader dr)
        {
            string Username = dr["StaffUsername"].ToString();
            return Username;
        }

        public static List<Staff> GetAvailableStaffs(List<string> AssignedStaff, string sortBy, bool isDefault)
        {
            List<Staff> output = new List<Staff>();
            StringBuilder builder = new StringBuilder();
            string sql = "";
            if (AssignedStaff.Count > 0)
            {
                if (isDefault)
                {
                    foreach (string staff in AssignedStaff)
                    {
                        builder.Append("'" + staff + "'" + ", ");
                    }
                    string result = builder.ToString();
                    string newresult = result.Substring(0, result.LastIndexOf(','));
                    if (!string.IsNullOrEmpty(newresult))
                        sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster WHERE fldUid NOT IN (" + newresult + ") ORDER BY fldOid";
                    else
                        sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster ORDER BY fldOid";
                }
                else
                {
                    #region "sort by fldLname(fullname)"
                    if (sortBy == "0")
                    {
                        foreach (string staff in AssignedStaff)
                        {
                            builder.Append("'" + staff + "'" + ", ");
                        }
                        string result = builder.ToString();
                        string newresult = result.Substring(0, result.LastIndexOf(','));
                        if (!string.IsNullOrEmpty(newresult))
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster WHERE fldUid NOT IN (" + newresult + ") ORDER BY fldLname";
                        else
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster ORDER BY fldLname";
                    }
                    #endregion
                    #region "sort by fldUid(Username)"
                    if (sortBy == "1")
                    {
                        foreach (string staff in AssignedStaff)
                        {
                            builder.Append("'" + staff + "'" + ", ");
                        }
                        string result = builder.ToString();
                        string newresult = result.Substring(0, result.LastIndexOf(','));
                        if (!string.IsNullOrEmpty(newresult))
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster WHERE fldUid NOT IN (" + newresult + ") ORDER BY fldUid";
                        else
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster ORDER BY fldUid";
                    }
                    #endregion
                    #region "sort by fldOid(Staff No.)"
                    if (sortBy == "2")
                    {
                        foreach (string staff in AssignedStaff)
                        {
                            builder.Append("'" + staff + "'" + ", ");
                        }
                        string result = builder.ToString();
                        string newresult = result.Substring(0, result.LastIndexOf(','));
                        if (!string.IsNullOrEmpty(newresult))
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster WHERE fldUid NOT IN (" + newresult + ") ORDER BY fldOid";
                        else
                            sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster ORDER BY fldOid";
                    }
                    #endregion
                }
            }
            else
            {
                sql = "SELECT fldOid,fldUid,fldLname FROM UserMaster ORDER BY fldOid";
            }
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff staff = new Staff();
                        staff.Load(dr);
                        output.Add(staff);
                    }
                }
            }
            return output;
        }

        public static List<Staff> Search(string searchTerm)
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = "SELECT * FROM UserMaster ";

                string where = "";
                if (!string.IsNullOrEmpty(searchTerm))
                    where += string.Format("(UPPER(fldUid) LIKE '%{0}%' OR " +
                    "UPPER(fldLname) LIKE '%{0}%' OR UPPER(fldOid) LIKE '%{0}%') ", searchTerm.ToUpper());

                if (!string.IsNullOrEmpty(where))
                    sql += " WHERE " + where;

                sql += " ORDER BY fldLname";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    try
                    {

                        Staff user = new Staff();
                        user.Load(dr);
                        output.Add(user);
                    }
                    catch (Exception ex)
                    {
                        //oCore.LogEvent("SAPHeitech.aspx", ex.Message, "APRUser.Search - UserID=" + dr["fldUid"].ToString(), "1");
                    }

                }
            }
            return output;
        }

        public static List<Staff> SearchListing(string searchTerm)
        {
            List<Staff> output = new List<Staff>();
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = "SELECT * FROM UserMaster ";

                string where = "";
                if (!string.IsNullOrEmpty(searchTerm))
                    where += string.Format("(UPPER(fldUid) LIKE '%{0}%' OR " +
                    "UPPER(fldLname) LIKE '%{0}%' OR UPPER(fldOid) LIKE '%{0}%') ", searchTerm.ToUpper());

                if (!string.IsNullOrEmpty(where))
                    sql += " WHERE " + where;

                sql += " ORDER BY fldLname";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    try
                    {

                        Staff user = new Staff();
                        user.Load(dr);
                        output.Add(user);
                    }
                    catch (Exception ex)
                    {
                        //oCore.LogEvent("SAPHeitech.aspx", ex.Message, "APRUser.Search - UserID=" + dr["fldUid"].ToString(), "1");
                    }

                }
            }
            return output;
        }

        private void Load(SqlDataReader dr)
        {
            Username = dr["fldUid"].ToString();
            StaffID = dr["fldOid"].ToString();
            Name = dr["fldLname"].ToString();
        }

        public static void SaveToDB(string SupervisorName, List<Staff> selectedstaff)
        {
            using (UtilityDb db = new UtilityDb())
            {
                db.OpenConnectionAppraisal();

                db.PrepareInsert("APRSupervisors");
                foreach (Staff staff in selectedstaff)
                {
                    DataRow row = db.Insert(null);
                    row["FormType"] = "Training Requisition";
                    row["SupervisorUsername"] = SupervisorName;
                    row["StaffUsername"] = staff.Username;
                    db.Insert(row);
                }
                db.EndInsert();
            }
        }
        //workflow Training Assignment
        public static void Unassigned(string Username)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                SqlCommand cmd = new SqlCommand();
                string sql = "DELETE FROM APRSupervisors Where UPPER(StaffUsername)='" + Username.ToUpper() + "' AND UPPER(Formtype)='TRAINING REQUISITION'";
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        public static List<string> StaffUnderSupervisor(string Supervisor)
        {
            List<string> output = new List<string>();
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT StaffUsername FROM APRSupervisors Where UPPER(Formtype)='TRAINING REQUISITION' AND SupervisorUsername='" + Supervisor + "' ";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff staff = new Staff();
                        string username = LoadUsername(dr);
                        output.Add(username);
                    }
                }
            }
            return output;
        }
        //for Assign and Unassign Training Workflows
        public static void ClearWorkflow(string Supervisor)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                SqlCommand cmd = new SqlCommand();
                string sql = "DELETE FROM APRSupervisors Where UPPER(SupervisorUsername)='" + Supervisor.ToUpper() + "' AND UPPER(Formtype)='TRAINING REQUISITION'";
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetPageTitle(string StaffUsername, string FromPage)
        {
            string PgTitle = "";
            using (SqlConnection conn = UtilityDb.GetConnectionAppraisal())
            {
                string sql = "SELECT DISTINCT SupervisorUsername FROM APRSupervisors Where UPPER(SupervisorUsername)='" + StaffUsername.ToUpper() + "' ";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {

                        string Username = dr["SupervisorUsername"].ToString();
                        if (!string.IsNullOrEmpty(Username))
                        {
                            if (FromPage.ToUpper() == "BOOK")
                            {
                                PgTitle = "Book Approval";
                                break;
                            }
                            if (FromPage.ToUpper() == "PREBOOK")
                            {
                                PgTitle = "Prebook Approval";
                            }
                        }
                    }
                }
                return PgTitle;
            }
        }

        public static Staff getStaffInfo(string selectedUsername)
        {
            Staff output = new Staff();
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = string.Format("SELECT fldOid,fldLname,fldUid FROM UserMaster WHERE UPPER(fldUid) = '{0}'", selectedUsername.ToUpper());
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        output.LoadStaffInfo(dr);
                    }
                }
            }
            return output;
        }

        private void LoadStaffInfo(SqlDataReader dr)
        {
            StaffID = dr["fldOid"].ToString();
            Name = dr["fldLname"].ToString();
            Username = dr["fldUid"].ToString();
        }


        internal static Staff getFromName(string StaffName)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionUser())
            {
                string sql = "SELECT * FROM UserMaster WHERE UPPER(fldLname)='" + StaffName.ToUpper() + "'";
                using (SqlDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    while (dr.Read())
                    {
                        Staff output = new Staff();
                        output.LoadFromReader(dr);
                        return output;
                    }
                }
            }
            return null;
        }
    }
    #region IComparer<Staff> Members
    class Staff_SortByName : IComparer<Staff>
    {
        public int Compare(Staff Staff1, Staff Staff2)
        {
            return string.Compare(Staff1.Name, Staff2.Name);
        }
    }

    #endregion
}
