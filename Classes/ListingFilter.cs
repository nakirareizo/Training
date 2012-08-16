using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Entities;

namespace BUILD.Training.Classes
{
    public class ListingFilter : Listing
    {
        public enum Filter
        {
            All,
            Type,
            Status,
            SAP,
            Title,
            NotDefined,
            Supervisors,
            Date,
            Month
        }

        internal static void getResultFilter(ref List<Listing> lstResult, List<Listing> lstTraining, string SearchItem, Filter filter)
        {
            switch (filter)
            {
                case Filter.Type:
                    IEnumerable<Listing> lstTypeFilter = from r in lstTraining where r.ApplicationType == SearchItem.ToUpper() select r;
                    foreach (Listing item in lstTypeFilter)
                    {
                        lstResult.Add(item);
                    }
                    break;
                case Filter.Status:
                    IEnumerable<Listing> lstStatusFilter = from r in lstTraining where r.Status == SearchItem.ToUpper() select r;
                    foreach (Listing item in lstStatusFilter)
                    {
                        lstResult.Add(item);
                    }
                    break;
                case Filter.SAP:
                    IEnumerable<Listing> lstSAPFilter = from r in lstTraining where r.PostedSAPStatus == SearchItem.ToUpper() select r;
                    foreach (Listing item in lstSAPFilter)
                    {
                        lstResult.Add(item);
                    }
                    break;
                case Filter.Title:
                    IEnumerable<Listing> lstTitleFilter = from r in lstTraining where r.Title.ToUpper() == SearchItem.ToUpper() select r;
                    foreach (Listing item in lstTitleFilter)
                    {
                        lstResult.Add(item);
                    }
                    break;
                case Filter.NotDefined:
                    IEnumerable<Listing> lstNotDefined = from r in lstTraining where r.SupervisorName.ToUpper() == SearchItem.ToUpper() select r;
                    var distinctCustomers = lstNotDefined.GroupBy(s => s.StaffID).Select(s => s.First());
                    foreach (Listing item in distinctCustomers)
                    {
                        lstResult.Add(item);
                    }
                    break;
                case Filter.Supervisors:
                    IEnumerable<Listing> lstSupervisors = from r in lstTraining where r.SupervisorName == SearchItem.ToUpper() select r;
                    foreach (Listing item in lstSupervisors)
                    {
                        lstResult.Add(item);
                    }
                    break;
            }
        }


        internal static void FilterbyDateRequested(DateTime SelectedDate, List<Listing> lstTraining, ref List<Listing> DateRequestedList)
        {
            foreach (Listing lst in lstTraining)
            {

                if (lst.DateRequest.HasValue)
                    if (lst.DateRequest.Value.ToString("ddMMyyyy") == SelectedDate.ToString("ddMMyyyy"))
                    {
                        DateRequestedList.Add(lst);
                    }
            }
        }
        internal static void FilterbyMonth(int SelectedMonth, List<Listing> lstTraining, ref List<Listing> MonthFilteredList)
        {
            foreach (Listing lst in lstTraining)
            {
                if (lst.DateRequest.HasValue)
                    if (lst.DateRequest.Value.Month== SelectedMonth)
                    {
                        MonthFilteredList.Add(lst);
                    }
            }
        }
        internal static List<string> LoadTrainingTitle(List<Listing> AdminTrainingLst)
        {
            List<string> output = new List<string>();
            List<string> outputFiltered = new List<string>();
            foreach (Listing item in AdminTrainingLst)
            {
                output.Add(item.Title);
            }
            outputFiltered = output.Distinct<string>().ToList<string>();
            outputFiltered.Sort();
            return outputFiltered;
        }

        #region "Directly Advance Filter Used"

        internal static void getAdvanceFilterResult(string SelectedValue, Filter filter, ref List<Listing> output)
        {
            switch (filter)
            {
                case Filter.Type:
                    getbyFilterType(SelectedValue, ref output);
                    break;
                case Filter.Status:
                    getbyFilterStatus(SelectedValue, ref output);
                    break;
                case Filter.SAP:
                    getbyFilterSAPStatus(SelectedValue, ref output);
                    break;
                case Filter.Title:
                    int SelectedID = Convert.ToInt32(SelectedValue);
                    getbyFilterTitle(SelectedID, ref output);
                    break;
                case Filter.NotDefined:
                    getbyFilterWorkflow(SelectedValue, ref output);
                    break;
                case Filter.Supervisors:
                    getByFilterSupervisor(SelectedValue, ref output);
                    break;
            }
        }

        protected static void getbyFilterType(string SelectedValue, ref List<Listing> output)
        {
            List<PrebookedEvent> pbkEventsLst = new List<PrebookedEvent>();
            List<BookedEvent> bkEventsLst = new List<BookedEvent>();
            if (SelectedValue.ToUpper() == "PREBOOK")
            {
                pbkEventsLst = PrebookedEvent.GetAll();
            }
            else
            {
                bkEventsLst = BookedEvent.GetAll();
            }
            BindToObject(ref output, null, bkEventsLst, pbkEventsLst, Mode.Admin, false);
        }

        protected static void getbyFilterStatus(string SelectedValue, ref List<Listing> output)
        {
            List<BookedEvent> bkEventsLst = BookedEvent.GetAll();
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAll();
            List<BookedEvent> filteredbk = new List<BookedEvent>();
            List<PrebookedEvent> filteredpk = new List<PrebookedEvent>();
            switch (SelectedValue)
            {
                case "SAVED":
                    //get all booked with stage 0 put to new list
                    foreach (BookedEvent bk in bkEventsLst)
                    {
                        if (bk != null)
                            if (bk.Stage == 0)
                            {
                                filteredbk.Add(bk);
                            }
                    }
                    //get all prebooked with stage 0 put to new list
                    foreach (PrebookedEvent pk in pbkEventsLst)
                    {
                        if (pk != null)
                            if (pk.Stage == 0)
                            {
                                filteredpk.Add(pk);
                            }
                    }

                    break;
                case "APPLIED":
                    //get all booked with stage 1 put to new list
                    foreach (BookedEvent bk in bkEventsLst)
                    {
                        if (bk != null)
                            if (bk.Stage == 1)
                            {
                                filteredbk.Add(bk);
                            }
                    }
                    //get all prebooked with stage 1 put to new list
                    foreach (PrebookedEvent pk in pbkEventsLst)
                    {
                        if (pk != null)
                            if (pk.Stage == 1)
                            {
                                filteredpk.Add(pk);
                            }
                    }
                    break;
                case "APPROVED SV":
                    //get all booked with stage 2 put to new list
                    foreach (BookedEvent bk in bkEventsLst)
                    {
                        if (bk != null)
                            if (bk.Stage == 2)
                            {
                                filteredbk.Add(bk);
                            }
                    }
                    //get all prebooked with stage 2 put to new list
                    foreach (PrebookedEvent pk in pbkEventsLst)
                    {
                        if (pk != null)
                            if (pk.Stage == 2)
                            {
                                filteredpk.Add(pk);
                            }
                    }
                    break;
                case "APPROVED HR":
                    //get all booked with stage 3 put to new list
                    foreach (BookedEvent bk in bkEventsLst)
                    {
                        if (bk != null)
                            if (bk.Stage == 3)
                            {
                                filteredbk.Add(bk);
                            }
                    }
                    break;
            }
            //foreach filtered booked bind to object
            foreach (BookedEvent fbk in filteredbk)
            {
                LoadObjectForStatus(TrainingType.Book, ref output, SelectedValue, fbk, null);
            }
            //foreach filtered prebooked bind to object
            foreach (PrebookedEvent fpk in filteredpk)
            {
                LoadObjectForStatus(TrainingType.Prebook, ref output, SelectedValue, null, fpk);
            }
        }
        //for filter by Training status
        private static void LoadObjectForStatus(TrainingType Type, ref List<Listing> output, string SelectedValue, BookedEvent bk, PrebookedEvent pk)
        {
            Listing newList = new Listing();
            Staff staff = new Staff();
            Staff supervisor = new Staff();
            string TrainingName = "";
            switch (Type)
            {
                case TrainingType.Book:
                    staff = Staff.getStaffInfo(bk.StaffUsername);
                    if (staff != null)
                    {
                        newList.StaffID = staff.StaffID;
                        newList.StaffName = staff.Name.ToUpper();
                    }
                    supervisor = Staff.GetSupervisorByUsername(staff.Username);
                    if (supervisor != null)
                    {
                        newList.SupervisorName = supervisor.Name.ToUpper();
                    }
                    else
                    {
                        newList.SupervisorName = "NOT DEFINED";
                    }
                    newList.ApplicationType = "BOOK";
                    TrainingName = EventGroup.getTitle(bk.EventId);
                    if (TrainingName != null || TrainingName != "")
                        newList.Title = TrainingName.ToUpper();
                    newList.DateRequest = bk.RequestDate;
                    newList.Status = SelectedValue.ToUpper();
                    if (bk.SAPStatus == 0)
                        newList.PostedSAPStatus = "PENDING";
                    if (bk.SAPStatus == 2)
                        newList.PostedSAPStatus = "POSTED";
                    output.Add(newList);
                    break;
                case TrainingType.Prebook:
                    staff = Staff.getStaffInfo(pk.StaffUsername);
                    if (staff != null)
                    {
                        newList.StaffID = staff.StaffID;
                        newList.StaffName = staff.Name.ToUpper();
                    }
                    supervisor = Staff.GetSupervisorByUsername(staff.Username);
                    if (supervisor != null)
                    {
                        newList.SupervisorName = supervisor.Name.ToUpper();
                    }
                    else
                    {
                        newList.SupervisorName = "NOT DEFINED";
                    }
                    newList.ApplicationType = "PREBOOK";
                    TrainingName = EventGroup.getTitle(pk.EventId);
                    if (TrainingName != null || TrainingName != "")
                        newList.Title = TrainingName.ToUpper();
                    newList.DateRequest = pk.RequestDate;
                    newList.Status = SelectedValue.ToUpper();
                    if (pk.SAPStatus == 0)
                        newList.PostedSAPStatus = "PENDING";
                    if (pk.SAPStatus == 2)
                        newList.PostedSAPStatus = "POSTED";
                    output.Add(newList);
                    break;
            }

        }

        protected static void getbyFilterTitle(int SelectedID, ref List<Listing> output)
        {
            List<BookedEvent> bkEventsLst = BookedEvent.getByEventID(SelectedID);
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.getByEventID(SelectedID);
            BindToObject(ref output, null, bkEventsLst, pbkEventsLst, Mode.Admin, false);
        }

        protected static void getByFilterSupervisor(string SelectedValue, ref List<Listing> output)
        {
            List<Staff> StaffList = Staff.GetStaffUnder(SelectedValue, false);
            foreach (Staff staff in StaffList)
            {
                List<BookedEvent> bkEventsLst = BookedEvent.getByUsername(staff.Username);
                List<PrebookedEvent> pbkEventsLst = PrebookedEvent.getByUsername(staff.Username);
                BindToObject(ref output, null, bkEventsLst, pbkEventsLst, Mode.Admin, false);

            }
        }

        protected static void getbyFilterSAPStatus(string SelectedValue, ref List<Listing> output)
        {
            List<BookedEvent> bkEventsLst = BookedEvent.GetAll();
            List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAll();
            BindToObjectSAPStatus(ref output, SelectedValue, bkEventsLst, pbkEventsLst);

        }
        //for filter by Training SAP status
        private static void BindToObjectSAPStatus(ref List<Listing> output, string SelectedValue, List<BookedEvent> bkEventsLst, List<PrebookedEvent> pbkEventsLst)
        {
            foreach (BookedEvent booked in bkEventsLst)
            {
                Listing newBookedList = new Listing();
                string TrainingName = EventGroup.getTitle(booked.EventId);
                switch (booked.SAPStatus)
                {
                    case 0:
                        if (SelectedValue == "SAVED")
                            newBookedList.LoadAll(TrainingType.Book, TrainingName, booked, null);
                        break;
                    case 1:
                        if (SelectedValue == "APPLIED")
                            newBookedList.LoadAll(TrainingType.Book, TrainingName, booked, null);
                        break;
                    case 2:
                        if (SelectedValue == "APPROVED SV")
                            newBookedList.LoadAll(TrainingType.Book, TrainingName, booked, null);
                        break;
                    case 3:
                        if (SelectedValue == "APPROVED HR")
                            newBookedList.LoadAll(TrainingType.Prebook, TrainingName, booked, null);
                        break;
                }
                output.Add(newBookedList);
            }
            foreach (PrebookedEvent prebooked in pbkEventsLst)
            {
                Listing newPrebookList = new Listing();
                string TrainingName = EventGroup.getTitle(prebooked.EventId);
                switch (prebooked.SAPStatus)
                {
                    case 0:
                        if (SelectedValue == "SAVED")
                            newPrebookList.LoadAll(TrainingType.Prebook, TrainingName, null, prebooked);
                        break;
                    case 1:
                        if (SelectedValue == "APPLIED")
                            newPrebookList.LoadAll(TrainingType.Prebook, TrainingName, null, prebooked);
                        break;
                    case 2:
                        if (SelectedValue == "APPROVED SV")
                            newPrebookList.LoadAll(TrainingType.Prebook, TrainingName, null, prebooked);
                        break;
                }
                output.Add(newPrebookList);
            }
        }

        protected static void getbyFilterWorkflow(string SelectedValue, ref List<Listing> output)
        {
            if (SelectedValue == "NOT DEFINED")
            {
                List<BookedEvent> bkEventsLst = BookedEvent.GetAll();
                List<PrebookedEvent> pbkEventsLst = PrebookedEvent.GetAll();
                foreach (BookedEvent bk in bkEventsLst)
                {
                    Staff supervisor = Staff.GetSupervisorByUsername(bk.StaffUsername);
                    Listing newBookedList = new Listing();
                    if (supervisor == null)
                    {
                        string TrainingName = EventGroup.getTitle(bk.EventId);
                        newBookedList.LoadAll(TrainingType.Book, TrainingName, bk, null);
                        output.Add(newBookedList);
                    }
                }
                foreach (PrebookedEvent pk in pbkEventsLst)
                {
                    Staff supervisor = Staff.GetSupervisorByUsername(pk.StaffUsername);
                    Listing newPrebookList = new Listing();
                    if (supervisor == null)
                    {
                        string TrainingName = EventGroup.getTitle(pk.EventId);
                        newPrebookList.LoadAll(TrainingType.Prebook, TrainingName, null, pk);
                        output.Add(newPrebookList);
                    }

                }
            }
        }
        #endregion
    }
}