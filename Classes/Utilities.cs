using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using TrainingRequisition.ClassLibrary.Utilities;
using System.Data.SqlClient;

namespace BUILD.Training.Classes
{
    [Serializable]
    public class Utilities
    {
        public string Username { get; set; }
        public string StaffID { get; set; }
        public string StaffUnderID { get; set; }
        public string StaffUnderName { get; set; }
        public string StaffName { get; set; }
        public string ConfirmStatus { get; set; }
        public string CEFStatus { get; set; }
        public string PTAStatus { get; set; }
        public string TrainingTitle { get; set; }
        public int EventDateID { get; set; }

        public static List<Utilities> getAllNotCompletedCEF()
        {
            List<Utilities> output = new List<Utilities>();
            //1. get all EventDate ID in AttendedEvents
            List<int> EventDateIDLst = AttendedEvent.GetAllEventDateID();
            List<Staff> stafflst = new List<Staff>();
            Staff staff = new Staff();
            foreach (int ID in EventDateIDLst)
            {
                //2. Get List of UnSubmited CEF
                List<AttendedEvent> UnSubmittedCEF = AttendedEvent.GetUnsubmitted("CEF", ID);
                if (UnSubmittedCEF != null)
                {
                    foreach (AttendedEvent unsubmited in UnSubmittedCEF)
                    {
                        if (unsubmited.StaffUsername != null)
                        {
                            bool exists = false;
                            //3. Get Staff Info
                            staff = Staff.getStaffInfo(unsubmited.StaffUsername);
                            if (staff != null)
                            {

                                foreach (Staff s in stafflst)
                                {
                                    if (s.Username.ToUpper() == staff.Username.ToUpper())
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                            }
                            //if staff already existed, skip
                            if (exists)
                                continue;
                        }
                        if (staff != null)
                        {
                            //if not exist add to staff list
                            stafflst.Add(staff);
                            //4.Load all to Object
                            LoadObject1(staff, unsubmited.EventName, ID, ref output);
                        }

                    }
                }
            }
            return output;

        }
        public static List<Utilities> getAllNotCompletedPTA()
        {
            List<Utilities> output = new List<Utilities>();
            //1. get all EventDate ID in AttendedEvents
            List<int> EventDateIDLst = AttendedEvent.GetAllEventDateID();
            List<Staff> SupervisorLst = new List<Staff>();
            Staff supervisor = new Staff();
            foreach (int ID in EventDateIDLst)
            {
                //2. Get List of UnSubmited PTA
                List<AttendedEvent> UnSubmittedPTA = AttendedEvent.GetUnsubmitted("PTA", ID);

                if (UnSubmittedPTA != null)
                {
                    foreach (AttendedEvent unsubmited in UnSubmittedPTA)
                    {

                        if (unsubmited.StaffUsername != null)
                        {
                            bool exists = false;
                            //3. Get Supervisor Info
                            supervisor = Staff.GetSupervisorByUsername(unsubmited.StaffUsername);
                            if (supervisor != null)
                            {
                                foreach (Staff super in SupervisorLst)
                                {
                                    if (super.Username.ToUpper() == supervisor.Username.ToUpper())
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                            }
                            //if staff already existed, skip
                            if (exists)
                                continue;
                        }
                        if (supervisor != null)
                        {
                            //4. if not exist add to staff list
                            SupervisorLst.Add(supervisor);
                            //5. get Staff Info
                            Staff staff = Staff.GetFromUsername(unsubmited.StaffUsername);
                            //6.Load all to Object
                            LoadObject3(supervisor, staff, unsubmited.EventName, ID, ref output);
                        }
                    }
                }
            }
            return output;
        }
        public static List<Utilities> getAllNotConfirmDate()
        {
            List<Utilities> output = new List<Utilities>();
            //1. get all staffs which has confirmation 
            List<AttendanceConfirmation> allConfirmations = AttendanceConfirmation.GetAll(null, null, true);
            List<Staff> stafflst = new List<Staff>();
            if (allConfirmations != null)
            {
                foreach (AttendanceConfirmation ac in allConfirmations)
                {
                    //2. get Staff info
                    bool exists = false;
                    Staff staff = Staff.getStaffInfo(ac.StaffUsername);
                    if (staff != null)
                    {
                        foreach (Staff s in stafflst)
                        {
                            if (s.Username.ToUpper() == staff.Username.ToUpper())
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                    if (exists)
                        continue;
                    if (staff != null)
                    {
                        stafflst.Add(staff);
                        //3. Load all to Object
                        LoadObject2(ac.EventDisplayName, staff, ref output);
                    }
                }
            }
            return output;
        }
        public static void LoadObject1(Staff staff, string Title, int ID, ref List<Utilities> output)
        {
            Utilities uc = new Utilities();
            uc.Username = staff.Username;
            uc.StaffID = staff.StaffID;
            uc.StaffName = staff.Name.ToUpper();
            uc.CEFStatus = "NOT COMPLETED";
            uc.TrainingTitle = Title.ToUpper();
            uc.EventDateID = ID;
            output.Add(uc);
        }
        public static void LoadObject2(string Title, Staff staff, ref List<Utilities> output)
        {
            Utilities atc = new Utilities();
            atc.Username = staff.Username;
            atc.StaffID = staff.StaffID;
            atc.StaffName = staff.Name.ToUpper();
            atc.ConfirmStatus = "NOT CONFIRM";
            atc.TrainingTitle = Title.ToUpper();
            output.Add(atc);
        }
        private static void LoadObject3(Staff supervisor, Staff staff, string Title, int ID, ref List<Utilities> output)
        {
            Utilities pta = new Utilities();
            pta.Username = supervisor.Username;
            pta.StaffID = supervisor.StaffID;
            pta.StaffName = supervisor.Name.ToUpper();
            pta.StaffUnderID = staff.StaffID;
            pta.StaffUnderName = staff.Name.ToUpper();
            pta.PTAStatus = "NOT COMPLETED";
            pta.TrainingTitle = Title.ToUpper();
            pta.EventDateID = ID;
            output.Add(pta);
        }
        internal static void getDicEventDateID(ref Dictionary<int, EventDate> eventDates, string surveyType)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS AE WHERE NOT EXISTS "
                    + "( SELECT * FROM ASM_Submitted" + surveyType + " AS S WHERE S.EventDateID=AE.EventDateID AND S.StaffUsername=AE.StaffUsername )";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    int eventDateId = Convert.ToInt32(dr["EventDateID"]);
                    EventDate ed = EventDate.GetFromId(eventDateId);

                    // if can't be found, skip
                    if (ed == null)
                        continue;

                    if (!eventDates.ContainsKey(eventDateId))
                        eventDates[eventDateId] = ed;
                }
            }
        }
        internal static List<Utilities> SearchCEFByStaff(List<string> UsernameList, ref List<Utilities> output)
        {
            foreach (string Username in UsernameList)
            {
                //1. get all EventDate ID in AttendedEvents
                List<AttendedEvent> Attended = AttendedEvent.GetAllByUsername(Username);
                if (Attended.Count > 0 || Attended != null)
                {
                    foreach (AttendedEvent ae in Attended)
                    {
                        //2. Get List of UnSubmited CEF
                        List<AttendedEvent> UnSubmitted = AttendedEvent.GetUnsubmitted("CEF", ae.StaffUsername);
                        foreach (AttendedEvent unsubmited in UnSubmitted)
                        {
                            //3. Get Staff Info
                            Staff staff = Staff.getStaffInfo(unsubmited.StaffUsername);
                            //4.Load all to Object
                            LoadObject1(staff, unsubmited.EventName, unsubmited.EventDateId, ref output);
                        }
                    }
                }
            }
            return output;
        }
        internal static List<Utilities> SearchPTAByStaff(List<string> UsernameList, ref List<Utilities> output)
        {
            List<Staff> SupervisorLst = new List<Staff>();
            foreach (string Username in UsernameList)
            {
                List<Staff> staffList = Staff.GetStaffUnder(Username, false);
                foreach (Staff staff in staffList)
                {

                    //2. Get List of UnSubmited PTA
                    List<AttendedEvent> UnSubmittedPTA = AttendedEvent.GetUnsubmitted("PTA", staff.Username);
                    if (UnSubmittedPTA != null)
                    {
                        foreach (AttendedEvent unsubmited in UnSubmittedPTA)
                        {
                            if (unsubmited.StaffUsername != null)
                            {
                                bool exists = false;
                                //3. Get Supervisor Info
                                Staff Supervisor = Staff.GetFromUsername(Username);
                                if (Supervisor != null)
                                {
                                    foreach (Staff super in SupervisorLst)
                                    {
                                        if (super.Username.ToUpper() == Supervisor.Username.ToUpper())
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }
                                }
                                //if staff already existed, skip
                                if (exists)
                                    continue;
                                if (Supervisor != null)
                                {
                                    //if not exist add to staff list
                                    SupervisorLst.Add(Supervisor);
                                    //4.Load all to Object
                                    LoadObject3(Supervisor, staff, unsubmited.EventName, unsubmited.EventDateId, ref output);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
        internal static List<Utilities> SearchATCByStaff(List<string> UsernameList, ref List<Utilities> output2)
        {
            foreach (string Username in UsernameList)
            {
                //1. get all staffs which has confirmation 
                List<AttendanceConfirmation> allConfirmations = AttendanceConfirmation.GetAll(Username, null, true);
                if (allConfirmations.Count > 0 || allConfirmations != null)
                {
                    foreach (AttendanceConfirmation ac in allConfirmations)
                    {
                        //2. get Staff info
                        Staff staff = Staff.getStaffInfo(ac.StaffUsername);
                        //3. Load all to Object
                        LoadObject2(ac.EventDisplayName, staff, ref output2);
                    }
                }
            }
            return output2;
        }
        internal static void getDicEventDateandKeys(string surveyType, ref Dictionary<int, EventDate> eventDates, bool isPTA)
        {
            using (SqlConnection conn = UtilityDb.GetConnectionESS())
            {
                string sql = "SELECT * FROM REQ_AttendedEvents AS AE WHERE NOT EXISTS "
                    + "( SELECT * FROM ASM_Submitted" + surveyType + " AS S WHERE S.EventDateID=AE.EventDateID AND S.StaffUsername=AE.StaffUsername )";
                SqlDataReader dr = UtilityDb.GetDataReader(sql, conn);
                while (dr.Read())
                {
                    int eventDateId = Convert.ToInt32(dr["EventDateID"]);
                    EventDate ed = EventDate.GetFromId(eventDateId);

                    // if can't be found, skip
                    if (ed == null)
                        continue;

                    // for PTA, check whether it is due. If not due, skip
                    if (isPTA && !ed.IsDueForPTA())
                        continue;

                    if (!eventDates.ContainsKey(eventDateId))
                        eventDates[eventDateId] = ed;
                }
            }
        }
    }
}