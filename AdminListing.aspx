<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="AdminListing.aspx.cs" Inherits="BUILD.Training.AdminListing" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 128px;
        }
        .style3
        {
            width: 116px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td class="PGTITLE">
                <asp:Label ID="lblPgTitle" runat="server" Text="Staff Training Applications List"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="left" class="style1">
                        <asp:Label ID="Label4" runat="server" Text="Search Staff :"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top" class="style1">
                        <asp:Label ID="lblCreateMultiple" runat="server" Text="Search Multiple Staffs"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtSearchMultiple" runat="server" Height="110px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="justify">
                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" />
                        <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" Text="Clear" />
                        <asp:Button ID="btnShowAll" runat="server" Text="Show All Staffs" OnClick="btnShowAll_Click" />
                        <asp:Button ID="btnAdvance" runat="server" Text="Show Advance Filter" OnClick="btnAdvance_Click" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:UpdateProgress ID="updateprogress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                Data being process, please wait...
                                <img id="Image1" runat="server" src="~/images/ajax-loader.gif" alt="" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </td>
                </tr>
            </table>
            <div id="divFilter" runat="server" visible="false">
                <table width="100%">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label2" runat="server" SkinID="lblSectionTitle" Text="Advance filter"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TabContainer ID="tcTabs" runat="server" ActiveTabIndex="1" Width="100%">
                                <asp:TabPanel runat="server" HeaderText="Training Application Type" ID="TabPanel1">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlApps" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlApps_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="All" Value="All"></asp:ListItem>
                                                        <asp:ListItem Text="Prebook" Value="Prebook"></asp:ListItem>
                                                        <asp:ListItem Text="Book" Value="Book"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel runat="server" HeaderText="Training Title" ID="TabPanel2">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlTitle" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTitle_SelectedIndexChanged"
                                                        Visible="False">
                                                    </asp:DropDownList>
                                                    <asp:ListBox ID="lstbxTitle" runat="server" Rows="10" SelectionMode="Multiple" SkinID="lstBox"
                                                        Width="100%" BackColor="#99CCFF" Visible="False" AutoPostBack="True"></asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <asp:Button ID="btnSelectTitle" runat="server" Text="Select Title" OnClick="btnSelectTitle_Click"
                                                        Visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel runat="server" HeaderText="Training Status" ID="TabPanel3">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"
                                                        Style="height: 22px">
                                                        <asp:ListItem Selected="True" Text="All" Value="All"></asp:ListItem>
                                                        <asp:ListItem Text="SAVED" Value="SAVED"></asp:ListItem>
                                                        <asp:ListItem Text="APPLIED" Value="APPLIED"></asp:ListItem>
                                                        <asp:ListItem Text="APPROVED SV" Value="APPROVED SV"></asp:ListItem>
                                                        <asp:ListItem Text="PENDING" Value="PENDING"></asp:ListItem>
                                                        <asp:ListItem Text="APPROVED HR" Value="APPROVED HR"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel runat="server" HeaderText="SAP Status" ID="TabPanel4">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlSAP" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSAP_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="All" Value="All"></asp:ListItem>
                                                        <asp:ListItem Text="PENDING" Value="PENDING"></asp:ListItem>
                                                        <asp:ListItem Text="POSTED" Value="POSTED"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel runat="server" HeaderText="Undefined Workflow" ID="TabPanel5">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlUndefined" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUndefined_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="All" Value="All"></asp:ListItem>
                                                        <asp:ListItem Text="NOT DEFINED" Value="NOT DEFINED"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel runat="server" HeaderText="Training Applications Under Supervisors"
                                    ID="TabPanel6">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td align="justify">
                                                    <asp:DropDownList ID="ddlSupervisors" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSupervisors_SelectedIndexChanged"
                                                        Visible="False">
                                                    </asp:DropDownList>
                                                    <asp:ListBox ID="lstbxSupervisors" runat="server" Rows="10" SelectionMode="Multiple"
                                                        SkinID="lstBox" Width="100%" BackColor="#99CCFF" Visible="False" AutoPostBack="True">
                                                    </asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <asp:Button ID="btnSelectSuper" runat="server" Text="Select Supervisor" OnClick="btnSelectSuper_Click"
                                                        Visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:TabPanel>
                            </asp:TabContainer>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divgv" runat="server" visible="false">
                <table width="100%">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label5" runat="server" SkinID="lblSectionTitle" Text="Staff Training Application Information"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td id="tdSortDate" runat="server" align="left" visible="false" colspan="2">
                            <asp:Label ID="lblSortDate" runat="server" Text="Sort Listing by date requested :"></asp:Label>
                            <asp:DropDownList ID="ddlSortDate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortDate_SelectedIndexChanged">
                            </asp:DropDownList>
                            OR Sort Listing by Month :<asp:DropDownList ID="ddlMonths" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlMonths_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Value="0">Please Select</asp:ListItem>
                                <asp:ListItem Value="1" Text="January"></asp:ListItem>
                                <asp:ListItem Value="2" Text="February"></asp:ListItem>
                                <asp:ListItem Value="3" Text="March"></asp:ListItem>
                                <asp:ListItem Value="4" Text="April"></asp:ListItem>
                                <asp:ListItem Value="5" Text="May"></asp:ListItem>
                                <asp:ListItem Value="6" Text="June"></asp:ListItem>
                                <asp:ListItem Value="7" Text="July"></asp:ListItem>
                                <asp:ListItem Value="8" Text="August"></asp:ListItem>
                                <asp:ListItem Value="9" Text="September"></asp:ListItem>
                                <asp:ListItem Value="10" Text="October"></asp:ListItem>
                                <asp:ListItem Value="11" Text="November"></asp:ListItem>
                                <asp:ListItem Value="12" Text="December"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right" valign="bottom">
                            <asp:Label ID="lblTotListed" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                            <asp:Label ID="lblTotListedVal" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="gvAdminList" runat="server" Width="100%" SkinID="GridView" AllowPaging="True"
                                OnPageIndexChanging="gvAdminList_PageIndexChanging" OnRowCommand="gvAdminList_RowCommand"
                                OnRowCreated="gvAdminList_RowCreated" OnRowDataBound="gvAdminList_RowDataBound"
                                EmptyDataText="No record found" AutoGenerateColumns="False" PageSize="20" DataKeyNames="StaffID">
                                <Columns>
                                    <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                    <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                                    <asp:BoundField DataField="SupervisorName" HeaderText="Supervisor Name" />
                                    <asp:BoundField DataField="ApplicationType" HeaderText="Application Type" />
                                    <asp:BoundField DataField="Title" HeaderText="Training Title" />
                                    <asp:BoundField DataField="DateRequest" HeaderText="Training Date" DataFormatString="{0:dd MMM yyyy}" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:BoundField DataField="PostedSAPStatus" HeaderText="SAP Status" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbtnAssign" runat="server" CausesValidation="False">Assign Workflow</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="100%">
        <tr>
            <td colspan="2" align="left">
                <asp:Button ID="btnConvertExcel" runat="server" Text="Convert to Excel File" OnClick="btnConvertExcel_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label1" runat="server" SkinID="lblSectionTitle" Text="Training Application Status"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
                <asp:Label ID="bkSaved" runat="server"><strong>SAVED</strong></asp:Label>
            </td>
            <td align="left">
                <asp:Label ID="bkSavedVal" runat="server" Text="Training Application is saved by Staff/Supervisor"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
                <asp:Label ID="bkApplied" runat="server"><strong>APPLIED</strong></asp:Label>
            </td>
            <td align="left">
                <asp:Label ID="bkAppliedVal" runat="server" Text="Training Application is applied by Staff/Supervisor"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
                <asp:Label ID="bkApproved" runat="server"><strong>APPROVED SV</strong></asp:Label>
            </td>
            <td align="left">
                <asp:Label ID="Label3" runat="server" Text="Training Application is approved by Supervisor"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
                <asp:Label ID="bkApproved1" runat="server"><strong>APPROVED HR</strong></asp:Label>
            </td>
            <td align="left">
                <asp:Label ID="bkPostedVal" runat="server" Text="Training Application is approved by Training HR."></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
                <asp:Label ID="bkPending" runat="server"><strong>PENDING</strong></asp:Label>
            </td>
            <td align="left">
                <asp:Label ID="bkPendingVal" runat="server" Text="Training Application is pending"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
