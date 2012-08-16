<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Book.aspx.cs" Inherits="TrainingRequisition.Book"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/Main.Master" %>

<%@ Register Src="UserControls/StaffListForBooking.ascx" TagName="StaffListForBooking"
    TagPrefix="uc1" %>
<%@ Register Src="UserControls/BookingEventSelector.ascx" TagName="EventSelector"
    TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="UserControls/EventDetails.ascx" TagName="EventDetails" TagPrefix="uc3" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="PGTITLE">
                        <asp:Label ID="lblPgTitle" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label1" runat="server" SkinID="lblSectionTitle" Font-Bold="true">Note</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: left">
                        <asp:Label ID="Label4" runat="server" SkinID="lblNormal"> You must select Training(s) at least <strong style = "color :#FF0000">5 days</strong> forward from today. </asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td>
                        <uc1:StaffListForBooking ID="uscStaffList" runat="server" BookingMode="Book" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlEvents" Visible="false" runat="server">
                <uc2:EventSelector ID="uscEventSelector" runat="server" />
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="this.onclick=new Function('return false;');" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnReject" runat="server" Text="Reject" Visible="false" 
                    onclick="btnReject_Click" />
                <asp:Label ID="lblSubmitError" runat="server"></asp:Label>
            </asp:Panel>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
               
                <ProgressTemplate>
                     Please Wait...<img id="Image1" runat="server" src="~/images/ajax-loader.gif" alt="Please wait..." />
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
