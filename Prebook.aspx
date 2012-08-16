<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Prebook.aspx.cs" Inherits="TrainingRequisition.Prebook" MasterPageFile="~/Main.Master" %>

<%@ Register Src="UserControls/StaffListForBooking.ascx" TagName="StaffListForBooking" TagPrefix="uc1" %>
<%@ Register Src="UserControls/PrebookingEventSelector.ascx" TagName="EventSelector" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="formTitle" runat="server">
    <ContentTemplate>
    <table width="100%">
        <tr>
            <td class="PGTITLE">
                <asp:label ID="lblPgTitle" runat="server" Enabled="true"></asp:label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
    </tr>
 </table>
 </ContentTemplate>
    </asp:UpdatePanel>
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
    <div id="divSearch" runat="server">
 <table width="100%">
    <tr>
        <td>
            <uc1:StaffListForBooking ID="uscStaffList" runat="server" BookingMode="Prebook"/>
        </td>
    </tr>
  </table>
 </div>
    <asp:Panel ID="pnlEvents" Visible="false" runat="server" width="99.9%" SkinID="pnlMain">
    <table width="100%">
    <tr>
        <td>
        <uc2:EventSelector ID="uscEventSelector" runat="server" />
        </td>
    </tr>
    <tr>
    <td>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="this.onclick=new Function('return false;');" OnClick="btnSubmit_Click" />
         <asp:Button ID="btnReject" runat="server" Text="Reject" Visible="false"  onclick="btnReject_Click" />
         <asp:HyperLink ID="lnkBackToAssessment" runat="server"><< Back to Competency Assessment Form</asp:HyperLink>
        </td>
    </tr>
    <tr>
<td>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
<ProgressTemplate>
    Please Wait.. <ProgressTemplate>
                                    <img ID="Image1" runat="server" src="~/images/ajax-loader.gif" alt="Please wait..." />
                                </ProgressTemplate>
</ProgressTemplate>
</asp:UpdateProgress>
</td>
</tr>
        </table>
    </asp:Panel>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
