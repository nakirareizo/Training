<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="SupervisorListing.aspx.cs" Inherits="BUILD.Training.SupervisorListing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td class="PGTITLE">
                <asp:Label ID="lblPgTitle" runat="server" Text="Staff Attended Training List"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="left">
                <asp:Label ID="lblSort" runat="server" Text="Sort List by Staff :"></asp:Label>
                <asp:DropDownList ID="ddlStaffName" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="ddlStaffName_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td align="right" valign="bottom">
                <asp:Label ID="lblTotListed" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="lblTotListedVal" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="gvAttended" Width="100%" runat="server" SkinID="GridView" EmptyDataText="No record found"
                    AllowPaging="True" AutoGenerateColumns="False" PageSize="20" OnPageIndexChanging="gvAttended_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                        <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                        <asp:BoundField DataField="AttendedTraining" HeaderText="Training Title" />
                        <asp:BoundField DataField="TrainingDate" HeaderText="Training Date" DataFormatString="{0:dd MMM yyyy}" />
                        <asp:BoundField DataField="CompletedCEF" HeaderText="Course Evaluation Form (CEF)" />
                        <asp:BoundField DataField="CompletedPTA" HeaderText="Post Training Assessment (PTA)" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
