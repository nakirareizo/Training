using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;
using System.Reflection;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace BUILD.Training.Classes
{
    [Serializable]
    public class Listing
    {
        public enum TrainingType
        {
            Prebook,
            Book
        }
        public enum Mode
        {
            Admin,
            Staff
        }
        public string StaffID { get; set; }
        public string StaffName { get; set; }
        public string ApplicationType { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string PostedSAPStatus { get; set; }
        public string SupervisorName { get; set; }
        public DateTime? DateRequest { get; set; }
        public string RequestedBy { get; set; }
        public string CEFStatus { get; set; }

        public static List<Listing> getAll()
        {
            List<Listing> output = new List<Listing>();
            List<BookedEvent> bkEventsLst = BookedEvent.GetAll();
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAll();
            BindToObject(ref output, null, bkEventsLst, pbkEventsLst, Mode.Admin, false);
            return output;
        }

        public static List<Listing> getAllbyUsername(string StaffUsername)
        {
            List<Listing> output = new List<Listing>();
            Staff staff = Staff.getStaffInfo(StaffUsername);
            List<BookedEvent> bkEventsLst = BookedEvent.GetAllListing(StaffUsername, null);
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAllListing(StaffUsername, null);
            BindToObject(ref output, staff, bkEventsLst, pbkEventsLst, Mode.Staff, false);
            return output;
        }

        internal static void getAllbyTypeUsernameandStage(TrainingType Type, string StaffUsername, int stage, ref List<Listing> TrainingLst)
        {
            Staff staff = Staff.getStaffInfo(StaffUsername);
            if (staff != null && stage == 1)
            {
                List<BookedEvent> bkEventsLst = new List<BookedEvent>();
                List<PrebookedEvent> pbkEventsLst = new List<PrebookedEvent>();
                if (Type == TrainingType.Book)
                    bkEventsLst = BookedEvent.GetAllByStaffStage(StaffUsername, stage);
                if (Type == TrainingType.Prebook)
                    pbkEventsLst = PrebookedEvent.getAllbyStaffStage(StaffUsername, stage);
                if (bkEventsLst != null || pbkEventsLst != null)
                    BindToObject(ref TrainingLst, staff, bkEventsLst, pbkEventsLst, Mode.Staff, true);
            }
        }

        public static void BindToObject(ref List<Listing> output, Staff staff, List<BookedEvent> bkEventsLst, List<PrebookedEvent> pbkEventsLst, Mode mode, bool IsUnapproved)
        {
            foreach (BookedEvent booked in bkEventsLst)
            {
                string TrainingName = EventGroup.getTitle(booked.EventId);
                if (!string.IsNullOrEmpty(TrainingName) || TrainingName != "")
                {
                    Listing newBookedList = new Listing();
                    if (mode == Mode.Admin)
                        newBookedList.LoadAll(TrainingType.Book, TrainingName, booked, null);
                    if (mode == Mode.Staff)
                    {
                        AttendedEvent ae = AttendedEvent.GetByEventDateIDAndUsername(booked.EventDateId, booked.StaffUsername);
                        string CEF = "";
                        if (ae.EventDateId != 0 && ae.StaffUsername != null)
                        {
                            if (CheckCompletedPTA(ae.EventDateId, ae.StaffUsername))
                                CEF = "COMPLETED";
                            else
                                CEF = "NOT COMPLETED";
                        }
                        else
                        {
                            CEF = "NOT COMPLETED";
                        }
                        newBookedList.LoadbyStaff(staff, TrainingType.Book, TrainingName, booked.Stage, booked.SAPStatus, booked.RequestDate, booked.RequesterUsername, IsUnapproved, CEF);
                    }
                    output.Add(newBookedList);
                }
            }
            foreach (PrebookedEvent prebooked in pbkEventsLst)
            {
                string CEF = "N/A";
                string TrainingName = EventGroup.getTitle(prebooked.EventId);
                if (!string.IsNullOrEmpty(TrainingName) || TrainingName != "")
                {
                    Listing newPrebookList = new Listing();
                    if (mode == Mode.Admin)
                        newPrebookList.LoadAll(TrainingType.Prebook, TrainingName, null, prebooked);
                    if (mode == Mode.Staff)
                        newPrebookList.LoadbyStaff(staff, TrainingType.Prebook, TrainingName, prebooked.Stage, prebooked.SAPStatus, prebooked.RequestDate, prebooked.RequesterUsername, IsUnapproved, CEF);
                    output.Add(newPrebookList);
                }
            }
        }

        public void LoadAll(TrainingType Type, string TrainingName, BookedEvent booked, PrebookedEvent prebooked)
        {
            Staff staff = new Staff();
            Staff supervisor = new Staff();
            switch (Type)
            {
                case TrainingType.Prebook:
                    DateRequest = prebooked.RequestDate;
                    staff = Staff.getStaffInfo(prebooked.StaffUsername);
                    if (staff != null)
                    {
                        StaffID = staff.StaffID;
                        StaffName = staff.Name;
                    }
                    supervisor = Staff.GetSupervisorByUsername(prebooked.StaffUsername);
                    if (supervisor != null)
                    {
                        SupervisorName = supervisor.Name.ToUpper();
                    }
                    else
                    {
                        SupervisorName = "NOT DEFINED";
                    }
                    ApplicationType = "PREBOOK";
                    Title = TrainingName.ToUpper();
                    Status = getTrainingStatus(prebooked.Stage, prebooked.SAPStatus, TrainingType.Prebook);
                    if (prebooked.SAPStatus == 1)
                        PostedSAPStatus = "PENDING";
                    if (prebooked.SAPStatus == 2)
                        PostedSAPStatus = "POSTED";
                    break;
                case TrainingType.Book:
                    DateRequest = booked.RequestDate;
                    staff = Staff.getStaffInfo(booked.StaffUsername);
                    if (staff != null)
                    {
                        StaffID = staff.StaffID;
                        StaffName = staff.Name;
                    }
                    supervisor = Staff.GetSupervisorByUsername(booked.StaffUsername);
                    if (supervisor != null)
                    {
                        SupervisorName = supervisor.Name.ToUpper();
                    }
                    else
                    {
                        SupervisorName = "NOT DEFINED";
                    }
                    ApplicationType = "BOOK";
                    Title = TrainingName.ToUpper();
                    Status = getTrainingStatus(booked.Stage, booked.SAPStatus, TrainingType.Book);

                    if (booked.SAPStatus == 0)
                        PostedSAPStatus = "PENDING";
                    if (booked.SAPStatus == 2)
                        PostedSAPStatus = "POSTED";
                    break;
            }

        }

        public void LoadbyStaff(Staff staff, TrainingType Type, string TrainingName, int stage, int SAPStatus, DateTime? RequestedDate, string RequestedUsername, bool IsUnapproved, string CEF)
        {
            if (IsUnapproved)
            {
                Staff supervisor = Staff.GetSupervisorByUsername(staff.Username);
                if (supervisor != null)
                    SupervisorName = supervisor.Name.ToUpper();
            }
            CEFStatus = CEF;
            Staff RequestedStaff = Staff.getStaffInfo(RequestedUsername);
            RequestedBy = RequestedStaff.Name.ToUpper();
            StaffID = staff.StaffID;
            StaffName = staff.Name;
            Title = TrainingName.ToUpper();
            DateRequest = RequestedDate;
            if (Type == TrainingType.Book)
            {
                ApplicationType = "BOOK";
                Status = getTrainingStatus(stage, SAPStatus, TrainingType.Book);
                if (SAPStatus == 0)
                    PostedSAPStatus = "PENDING";
                if (SAPStatus == 2)
                    PostedSAPStatus = "POSTED";
            }
            if (Type == TrainingType.Prebook)
            {
                ApplicationType = "PREBOOK";
                Status = getTrainingStatus(stage, SAPStatus, TrainingType.Prebook);
                if (SAPStatus == 1)
                    PostedSAPStatus = "PENDING";
                if (SAPStatus == 2)
                    PostedSAPStatus = "POSTED";
            }


        }

        private static string getTrainingStatus(int stage, int SAPStatus, TrainingType Type)
        {
            string Status = "";
            if (Type == TrainingType.Book)
            {
                switch (stage)
                {
                    case 0:
                        Status = "SAVED";
                        break;
                    case 1:
                        Status = "APPLIED";
                        break;
                    case 2:
                        Status = "APPROVED SV";
                        break;
                    case 3:
                        if (SAPStatus == 1)
                            Status = "PENDING";
                        if (SAPStatus == 2)
                            Status = "APPROVED HR";
                        break;
                }
            }
            if (Type == TrainingType.Prebook)
            {
                switch (stage)
                {
                    case 0:
                        Status = "SAVED";
                        break;
                    case 1:
                        Status = "APPLIED";
                        break;
                    case 2:
                        if (SAPStatus == 1)
                            Status = "PENDING";
                        if (SAPStatus == 2)
                            Status = "APPROVED SV";
                        break;
                }
            }
            return Status;
        }

        internal static List<Listing> Search(string SearchItem, ref List<Listing> output)
        {
            List<BookedEvent> bkEventsLst = BookedEvent.GetAllListing(SearchItem, null);
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAllListing(SearchItem, null);
            BindToObject(ref output, null, bkEventsLst, pbkEventsLst, Mode.Admin, false);
            return output;
        }

        public static bool CheckCompletedCEF(int EventDateID, string StaffUsername)
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

        internal static void FilterAppsbySupervisor(ref List<Listing> lstResult, List<Listing> ListedApps, string SelectedSupervisor, ref string Total)
        {
            IEnumerable<Listing> lstAppsFiltered = from r in ListedApps where r.SupervisorName.ToUpper() == SelectedSupervisor.ToUpper() select r;
            foreach (Listing App in lstAppsFiltered)
            {
                if (App != null)
                    lstResult.Add(App);
            }
            Total = lstResult.Count.ToString();
        }

        public static void ExportGridViewtToExcel(string Filename, GridView gv, HttpResponse response)
        {
            try
            {
                gv.AllowPaging = false;
                gv.DataBind();

                response.Clear();
                response.AddHeader("content-disposition", "attachment;filename=" + Filename + ".xls");
                response.Charset = "";
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.ContentType = "application/vnd.xls";
                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                gv.RenderControl(htmlWrite);
                response.Write(stringWrite.ToString());

                //HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {

            }
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
    }
}