using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.Reports;
using TrainingRequisition.Classes;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;

namespace BUILD.Training.Classes
{
    [Serializable]
    public class StaffAttendedEvents
    {
        public string StaffID { get; set; }
        public string StaffName { get; set; }
        public string AttendedTraining { get; set; }
        public string TrainingDate { get; set; }
        public string CompletedCEF { get; set; }
        public string CompletedPTA { get; set; }

        public static List<StaffAttendedEvents> getAll(string SupervisorUsername)
        {
            List<StaffAttendedEvents> output = new List<StaffAttendedEvents>();
            //get all staffs under supervisor
            List<Staff> staffList = Staff.GetStaffUnder(SupervisorUsername, false);
            //get Attended Training foreach staff
            foreach (Staff staff in staffList)
            {
                List<AttendedEvent> ae = AttendedEvent.GetAllByUsername(staff.Username);
                foreach (AttendedEvent Event in ae)
                {
                    bool CompletedCEF;
                    bool CompletedPTA;
                    //check whether completed CEF foreach Attended Training
                    if (CheckCompletedCEF(Event.EventDateId, staff.Username))
                        CompletedCEF = true;
                    else
                        CompletedCEF = false;
                    //check whether PTA foreach Attended Training 
                    if (CheckCompletedPTA(Event.EventDateId, staff.Username))
                        CompletedPTA = true;
                    else
                        CompletedPTA = false;
                    //LoadtoObject
                    LoadToObject(ref output, Event, staff, CompletedCEF, CompletedPTA);
                }
            }
            return output;
        }

        private static bool CheckCompletedPTA(int EventDateID, string StaffUsername)
        {
            bool Completed = false;
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM ASM_SubmittedPTA WHERE UPPER(StaffUsername)='{0}' AND EventDateID={1}", StaffUsername.ToUpper(), EventDateID);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    if (dr.HasRows)
                        Completed = true;
                }
            }
            return Completed;
        }

        private static bool CheckCompletedCEF(int EventDateID, string StaffUsername)
        {
            bool Completed = false;
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = string.Format("SELECT * FROM ASM_SubmittedCEF WHERE UPPER(StaffUsername)='{0}' AND EventDateID={1}", StaffUsername.ToUpper(), EventDateID);
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    if (dr.HasRows)
                        Completed = true;
                }
            }
            return Completed;
        }

        private static void LoadToObject(ref List<StaffAttendedEvents> output, AttendedEvent Event, Staff staff, bool CEF, bool PTA)
        {
            StaffAttendedEvents sae = new StaffAttendedEvents();
            sae.StaffID = staff.StaffID;
            sae.StaffName = staff.Name.ToUpper();
            sae.AttendedTraining = Event.EventName.ToUpper();
            sae.TrainingDate = Event.EventDatesDisplay;
            if (CEF == true)
                sae.CompletedCEF = "COMPLETED";
            else
                sae.CompletedCEF = "NOT COMPLETED";
            if (PTA == true)
                sae.CompletedPTA = "COMPLETED";
            else
                sae.CompletedPTA = "NOT COMPLETED";
            output.Add(sae);
        }

        internal static void getAllbyFilter(List<StaffAttendedEvents> StaffsAttendedList, ref List<StaffAttendedEvents> lstResult, string SelectedName)
        {
            IEnumerable<StaffAttendedEvents> lstStaffFilter = from sae in StaffsAttendedList where sae.StaffName.ToUpper() == SelectedName.ToUpper() select sae;
            foreach (StaffAttendedEvents item in lstStaffFilter)
            {
                lstResult.Add(item);
            }
        }
    }
}