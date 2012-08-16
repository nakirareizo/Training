<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SendReminderEmailsPTA.aspx.cs"
    Inherits="TrainingRequisition.Assessments.SendReminderEmailsPTA" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <h1>
            Reminder Emails</h1>
        <h2>
            Post-Training Assessment</h2>
        <p>
            The following events still have unanswered assessments:</p>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <p>
                    <asp:ListBox ID="lbEventDates" runat="server" DataTextField="Title" DataValueField="ID"
                        Height="204px" SelectionMode="Multiple" Width="311px"></asp:ListBox>
                </p>
                <p>
                    <asp:Button ID="btnSelectAll" runat="server" OnClick="btnSelectAll_Click" Text="Select All" />
                    <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" Text="Clear" />
                    <asp:Button ID="btnSendConfirmAttendance" runat="server" OnClick="btnSendConfirmAttendance_Click"
                        Text="Send Emails" />
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </p>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        Please wait....
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </ContentTemplate>
        </asp:UpdatePanel>
        <p>
            &nbsp;</p>
        <p>
            <br />
        </p>
    </div>
    </asp:Content>
