<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="Utility.aspx.cs" Inherits="BUILD.Training.Utility" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 149px;
        }
        .style3
        {
            width: 227px;
        }
        .style4
        {
            width: 8px;
        }
        .style5
        {
            width: 209px;
        }
        .style6
        {
            width: 409px;
        }
        .style7
        {
            width: 277px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td class="PGTITLE">
                <asp:Label ID="lblPgTitle" runat="server" Text="Admin Monitoring on Staff Application Status"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divSearch" runat="server">
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
                        </td>
                    </tr>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <asp:UpdateProgress ID="updateprogress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                <ProgressTemplate>
                                    Data being process, please wait...
                                    <img id="Image1" runat="server" src="~/images/ajax-loader.gif" alt="" />
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </td>
                    </tr>
                </table>
            </div>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        <asp:TabContainer ID="tcTabs" runat="server" ActiveTabIndex="3" Width="100%" AutoPostBack="true"
                            OnActiveTabChanged="tcTabs_ActiveTabChanged">
                            <asp:TabPanel runat="server" HeaderText="CEF Status" ID="TabPanel1">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" SkinID="lblSectionTitle" Text="Course Evaluation Form  Status"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="bottom">
                                                <asp:Label ID="lblTotListed" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                                                <asp:Label ID="lblTotListedVal" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvCEF" Width="100%" runat="server" SkinID="GridView" DataKeyNames="EventDateID"
                                                    EmptyDataText="No record found" AllowPaging="True" AutoGenerateColumns="False"
                                                    PageSize="20" OnRowCommand="gvCEF_RowCommand" OnRowCreated="gvCEF_RowCreated"
                                                    OnPageIndexChanging="gvCEF_PageIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="EventDateID" HeaderText="Event Date ID" Visible="False" />
                                                        <asp:BoundField DataField="Username" HeaderText="Username" />
                                                        <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                                        <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                                                        <asp:BoundField DataField="TrainingTitle" HeaderText="Training Attended" />
                                                        <asp:BoundField DataField="CEFStatus" HeaderText="CEF Status" />
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                Select to Send Email</HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lbtnNotify" runat="server" CausesValidation="False">Select</asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="80px" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" HeaderText="PTA Status" ID="TabPanel2">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" SkinID="lblSectionTitle" Text="Post Training Assessment Status"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                                                <asp:Label ID="lblTotAvailableVal2" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvPTA" Width="100%" runat="server" SkinID="GridView" DataKeyNames="EventDateID"
                                                    EmptyDataText="No record found" AllowPaging="True" AutoGenerateColumns="False"
                                                    PageSize="20" OnRowCommand="gvPTA_RowCommand" OnRowCreated="gvPTA_RowCreated"
                                                    OnPageIndexChanging="gvPTA_PageIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="EventDateID" HeaderText="Event Date ID" Visible="False" />
                                                        <asp:BoundField DataField="Username" HeaderText="Username" />
                                                        <asp:BoundField DataField="StaffID" HeaderText="Supervisor ID" />
                                                        <asp:BoundField DataField="StaffName" HeaderText="Supervisor Name" />
                                                        <asp:BoundField DataField="StaffUnderID" HeaderText="Staff ID" />
                                                        <asp:BoundField DataField="StaffUnderName" HeaderText="Staff Name" />
                                                        <asp:BoundField DataField="TrainingTitle" HeaderText="Training Attended" />
                                                        <asp:BoundField DataField="PTAStatus" HeaderText="PTA Status" />
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                Select to Send Email</HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lbtnNotify" runat="server" CausesValidation="False">Select</asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="80px" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" HeaderText="Prebook Confirmation Status" ID="TabPanel3">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" SkinID="lblSectionTitle" Text="Prebook Confirmation Status"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                                                <asp:Label ID="lblTotListedVal1" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvATC" Width="100%" runat="server" SkinID="GridView" DataKeyNames="Username"
                                                    EmptyDataText="No record found" AllowPaging="True" AutoGenerateColumns="False"
                                                    PageSize="20" OnSelectedIndexChanged="gvATC_SelectedIndexChanged" OnPageIndexChanging="gvATC_PageIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="Username" HeaderText="Username" Visible="False" />
                                                        <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                                        <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                                                        <asp:BoundField DataField="TrainingTitle" HeaderText="Applied Training" />
                                                        <asp:BoundField DataField="ConfirmStatus" HeaderText="Confirm?" />
                                                        <asp:CommandField ShowSelectButton="True" />
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblNotify" runat="server"><strong style = "color :#FF0000">**Click Select link button to send email notification to particular staff</strong></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" HeaderText="Pending Training Application Approval" ID="TabPanel4">
                                <HeaderTemplate>
                                Pending Training Application Approval
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div id="divSupervisor" runat="server">
                                        <table width="100%">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="Label8" runat="server" SkinID="lblSectionTitle" Text="Pending Training Application Approval"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="left">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" class="style5">
                                                    <asp:Label ID="Label5" runat="server" Text="Select Training Application Type :"></asp:Label>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlTrainingType" runat="server">
                                                        <asp:ListItem Value="0" Selected="True">Please Select</asp:ListItem>
                                                        <asp:ListItem Value="Prebook">PREBOOK</asp:ListItem>
                                                        <asp:ListItem Value="Book">BOOK</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Button ID="btnShowAllSuper" runat="server" Text="Show All" OnClick="btnShowAllSuper_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Label ID="lblUncompleted" runat="server" Text="Supervisor have Training(s) pending approval :"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:ListBox ID="lstSupervisors" runat="server" Rows="10" SelectionMode="Multiple"
                                                        SkinID="lstBox" Width="100%"></asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" class="style4" colspan="2">
                                                    <asp:Label ID="lblTotAvailable" runat="server" SkinID="lblNormal" Text="Total Staff:"></asp:Label>
                                                    &nbsp;
                                                    <asp:Label ID="lblTotAvailableVal" runat="server" SkinID="lblValue"></asp:Label>
                                                    <asp:Button ID="btnShowPendingApp" runat="server" Text="Show Pending Applications"
                                                        OnClick="btnShowPendingApp_Click" Style="margin-bottom: 0px" Width="162px" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="divStaffAppList" runat="server" visible="False">
                                        <table width="100%">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="Label10" runat="server" SkinID="lblSectionTitle" Text="Staff Training Applications for selected supervisor(s)"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <asp:Label ID="Label11" runat="server" SkinID="lblSectionTitle" Text="Workflow for Supervisor"></asp:Label>
                                                </td>
                                                <td align="left" id="tdWorkflowDetail" runat="server" visible="false">
                                                    <asp:Label ID="lblSection" runat="server" Text="Selected Workflow Details" SkinID="lblSectionTitle"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:ListBox ID="lstbxSelectedSuper" runat="server" Rows="10" SkinID="lstBox" Height="115px"
                                                                    BackColor="#99CCFF" AutoPostBack="True" OnSelectedIndexChanged="lstbxSelectedSuper_SelectedIndexChanged"
                                                                    Width="243px"></asp:ListBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <div id="divWorkflowDetail" runat="server" visible="false">
                                                        <table width="100%">
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="lblSuperName" runat="server" Text="Supervisor Name :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblSuperNameVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="StaffID" runat="server" Text="Staff ID :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblStaffIDVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="lblSubsidiary" runat="server" Text="Subsidiary :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblSubsidiaryVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="lblTotalStaff" runat="server" Text="Total Staff in Workflow :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblTotalStaffVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="lblPendingApp" runat="server" Text="Total Pending Applications :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblPendingAppVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" width="170px">
                                                                    <asp:Label ID="lblType" runat="server" Text="Applications Type :"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblTypeVal" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="bottom" colspan="2">
                                                    <asp:Label ID="lblTotListedApp" runat="server" Text="Total Record:"></asp:Label>&nbsp;&nbsp;
                                                    <asp:Label ID="lblTotListedAppVal" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:GridView ID="gvStaffAppList" Width="100%" runat="server" SkinID="GridView" EmptyDataText="No record found"
                                                        OnPageIndexChanging="gvStaffAppList_PageIndexChanging" AllowPaging="True" AutoGenerateColumns="False"
                                                        PageSize="20">
                                                        <Columns>
                                                            <asp:BoundField DataField="ApplicationType" HeaderText="Application Type" />
                                                            <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                                            <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                                                            <asp:BoundField DataField="SupervisorName" HeaderText="Supervisor Name" />
                                                            <asp:BoundField DataField="RequestedBy" HeaderText="Requested By" />
                                                            <asp:BoundField DataField="Title" HeaderText="Training Title" />
                                                            <asp:BoundField DataField="DateRequest" HeaderText="Training Date" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="Status" HeaderText="Status" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" class="style7">
                                                    <asp:Button ID="btnBack" runat="server" Text="Back to Supervisor List" OnClick="btnBack_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>
                        </asp:TabContainer>
                    </td>
                </tr>
                <tr id="trShowAll" runat="server" visible="false">
                    <td align="left" class="style3">
                        <asp:Label ID="lblSearchAll" runat="server">Click for show All for selected Tab</asp:Label>
                    </td>
                    <td align="left">
                        <asp:Button ID="btnShowAll" runat="server" Text="Show All" OnClick="btnShowAll_Click" />
                    </td>
                </tr>
                <tr id="trSendReminder" runat="server" visible="false">
                    <td align="left" class="style3">
                        <asp:Label ID="Label9" runat="server">Send Reminder to All Staff</asp:Label>
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddlSurveyType" runat="server">
                            <asp:ListItem Value="1" Selected="True">CEF</asp:ListItem>
                            <asp:ListItem Value="2">PTA</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnReminderAll" runat="server" Text="Send Reminder" OnClick="btnReminderAll_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
