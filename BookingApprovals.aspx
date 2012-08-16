<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookingApprovals.aspx.cs"
    Inherits="TrainingRequisition.BookingApprovals" MasterPageFile="~/Main.Master" %>

<%@ Register Src="UserControls/ApprovalEventSelector.ascx" TagName="ApprovalEventSelector"
    TagPrefix="uc1" %>
<%@ Register Src="UserControls/ApprovalStaffList.ascx" TagName="ApprovalStaffList"
    TagPrefix="uc2" %>
<%@ Register Src="UserControls/BookingOptions.ascx" TagName="BookingOptions" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style2
        {
            width: 500px;
            background-color: #DDDDDD;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<asp:panel id="pnlSearch" runat="server" width="99.9%" skinid="pnlMain">
    <table width="100%">
<tr>
<td class="PGTITLE">
    <asp:label ID="lblPgTitle" runat="server" 
        Text="HR Training Approval"></asp:label>
</td>
</tr>
<tr>
<td>
</td>
</tr>
</table>
        <uc1:ApprovalEventSelector ID="uscApprovalEventSelector" runat="server" />
        <uc2:ApprovalStaffList ID="uscStaffList" runat="server" />
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
<ProgressTemplate>
    Please Wait.. <ProgressTemplate>
                                    <img ID="Image1" runat="server" src="~/images/ajax-loader.gif" alt="Please wait..." />
                                </ProgressTemplate>
</ProgressTemplate>
</asp:UpdateProgress>
<asp:ObjectDataSource ID="dsQuestions" runat="server" SelectMethod="GetAll" TypeName="TrainingRequisition.Classes.Question">
</asp:ObjectDataSource>
<table cellpadding="0" cellspacing="5" class="style2">
<tr>
    <td>
        Questions:
    </td>
</tr>
<tr>
    <td>
        <asp:BulletedList ID="lstQuestions" runat="server" BulletStyle="Numbered" DataSourceID="dsQuestions"
            DataTextField="DisplayName" DataValueField="DisplayName">
        </asp:BulletedList>
    </td>
</tr>
</table>
            </asp:panel>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
