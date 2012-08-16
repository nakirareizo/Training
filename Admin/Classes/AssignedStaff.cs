using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Entities;

namespace BUILD.Training.Admin.Classes
{
    [Serializable]
    public class AssignedStaff
    {
        public string StaffID { get; set; }
        public string StaffName { get; set; }
        public string WorkflowUnder { get; set; }
        public string Subsidiary { get; set; }

        public static AssignedStaff getAllByUsername(string StaffUsername)
        {
            AssignedStaff output = new AssignedStaff();
            Staff staff = Staff.GetFromUsername(StaffUsername);
            Staff supervisor = Staff.GetSupervisorByUsername(StaffUsername);
            AssignedStaff AS = new AssignedStaff();
            AS.StaffID = staff.StaffID;
            AS.StaffName = staff.Name.ToUpper();
            AS.WorkflowUnder = supervisor.Name.ToUpper();
            AS.Subsidiary = staff.Subsidiary;
            output = AS;
            return output;
        }
    }
}