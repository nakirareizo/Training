<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPages.aspx.cs" Inherits="TrainingRequisition.TestPages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .style1 {

        }
    </style>
</head>
<body>
    <h1>
        Test Page for Training Requisition Modules</h1>
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="5" class="style1">
        <tr>
            <td>
                <asp:Button ID="btnLogout" runat="server" onclick="btnLogout_Click" 
                    Text="Logout" />
&nbsp;<asp:Button ID="btnResetEvents" runat="server" Text="Reset Events" 
                    onclick="btnResetEvents_Click" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../Prebook.aspx">Prebook.aspx</a> (for ad-hoc prebooking)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../Prebook.aspx?SE=1,2,3&UN=akhiriah&BL=test.aspx">Prebook.aspx</a> (for assessment prebooking, 
                with suggested events)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../Book.aspx">Book.aspx</a> (for booking, used by staff and 
                supervisors)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../BookingApprovals.aspx">BookingApprovals.aspx</a> (to approve 
                bookings, used by HR admins)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../ConfirmAttendance.aspx">ConfirmAttendance.aspx</a> (for staff to 
                confirm attendance)
            </td>
            <td>
                <asp:Button ID="btnResetConfirmAttendance" runat="server" Text="Reset Data" 
                    onclick="btnResetConfirmAttendance_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <a href="../Reports/AttendedEvents.aspx">AttendedEvents.aspx</a> (for staff to 
                see the events they have attended)</td>
            <td>
                <asp:Button ID="btnResetAttendedEvents" runat="server" Text="Reset Data" 
                    onclick="btnResetAttendedEvents_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <a href="../Reports/SuggestedEvents.aspx">SuggestedEvents.aspx</a> (for HR Admin 
                to view events suggested by staff)</td>
            <td>
                <asp:Button ID="btnResetSuggestedEvents" runat="server" Text="Reset Data" />
            </td>
        </tr>
        <tr>
            <td>
                <a href="../Reports/BookedEvents.aspx">BookedEvents.aspx</a> (for staff to see 
                the events that have been booked)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../Reports/PrebookedEvents.aspx">PrebookedEvents.aspx</a> (for staff to 
                see the events that have been prebooked)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <b>Reminder Emails</b></td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../SendReminderEmails.aspx">SendReminderEmails.aspx</a> (please confirm 
                your attendance)</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <b>Assessments</b></td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <a href="../Assessment/CourseEvaluationForm.aspx">CourseEvaluationForm.aspx</a></td>
            <td>
                <asp:Button ID="btnResetCEF" runat="server" onclick="btnResetCEF_Click" 
                    Text="Reset Data" Width="101px" />
            </td>
        </tr>
        <tr>
            <td>
                <a href="../Assessment/PostTrainingAssessment.aspx">PostTrainingAssessment.aspx</a></td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <div>
    
    </div>
    </form>
</body>
</html>
