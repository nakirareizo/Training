<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="TrainingList.aspx.cs" Inherits="BUILD.Training.TrainingList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 118px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="formTitle" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="PGTITLE">
                        <asp:Label ID="lblPgTitle" runat="server" Text="Training Listing"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td align="right" valign="bottom">
                        <asp:Label ID="lblTotListed" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                        <asp:Label ID="lblTotListedVal" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gvTrainingList" Width="100%" runat="server" SkinID="GridView" EmptyDataText="No record found"
                            AllowPaging="True" AutoGenerateColumns="False" PageSize="20">
                            <Columns>
                                <asp:BoundField DataField="ApplicationType" HeaderText="Application Type" />
                                <asp:BoundField DataField="RequestedBy" HeaderText="Requested By" />
                                <asp:BoundField DataField="Title" HeaderText="Training Title" />
                                <asp:BoundField DataField="DateRequest" HeaderText="Training Date" DataFormatString="{0:dd MMM yyyy}" />
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                <asp:BoundField DataField="CEFStatus" 
                                    HeaderText="Course Evaluation Form (CEF) Status" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label1" runat="server" SkinID="lblSectionTitle" Text="Training Application Status"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style1">
                        <asp:Label ID="bkSaved" runat="server"><strong>SAVED</strong></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="bkSavedVal" runat="server" Text="Training Application is saved by Staff/Supervisor"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style1">
                        <asp:Label ID="bkApplied" runat="server"><strong>APPLIED</strong></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="bkAppliedVal" runat="server" Text="Training Application is applied by Staff/Supervisor"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style1">
                        <asp:Label ID="bkApproved" runat="server"><strong>APPROVED SV</strong></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="Label3" runat="server" Text="Training Application is approved by Supervisor"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style1">
                        <asp:Label ID="bkApproved1" runat="server"><strong>APPROVED HR</strong></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="bkPostedVal" runat="server" Text="Training Application is approved by Training HR."></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
