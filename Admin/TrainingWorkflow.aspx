<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="TrainingWorkflow.aspx.cs" Inherits="BUILD.Training.Admin.TrainingWorkflow" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style2
        {
            width: 154px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="PGTITLE">
                        <asp:Label ID="lblPgTitle1" runat="server" Text="Workflow [ Training ]"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlWorkflowInformation" runat="server" Width="100%" SkinID="pnlMain">
                            <asp:Label ID="lblMsg" runat="server" Text="" SkinID="lblRedMessage"></asp:Label>
                        </asp:Panel>
            </table>
            <table width="100%">
                <tr>
                    <td>
                        <asp:TabContainer ID="tcTabs" runat="server" ActiveTabIndex="0" Width="100%">
                            <asp:TabPanel runat="server" HeaderText="Users Assignment" ID="TabPanel1">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="Label2" runat="server" SkinID="lblSectionTitle" Text="Search staff"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" width="200px">
                                                <asp:Label ID="lblSelectStaff" runat="server" Text="Select Supervisor :" SkinID="lblNormal"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtStaffSearch" runat="server" SkinID="txtNormal"></asp:TextBox><asp:Button
                                                    ID="btnSearch" runat="server" SkinID="bntButton" Text="Search" OnClick="btnSearch_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label3" runat="server" SkinID="lblSectionTitle" Text="Search results"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <asp:GridView ID="gvStaff1" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                                    Width="100%" DataKeyNames="Username" ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="gvStaff1_SelectedIndexChanged">
                                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Username" HeaderText="Username" />
                                                        <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                                        <asp:CommandField ShowSelectButton="True" />
                                                    </Columns>
                                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                    <EditRowStyle BackColor="#999999" />
                                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="Label1" runat="server" SkinID="lblSectionTitle" Text="Selected staff"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="lblStaffName" runat="server" SkinID="lblNormal" Text="Staff Name :"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblStaffNameVal" runat="server" SkinID="lblNormal"></asp:Label><asp:Label
                                                    ID="lblUserNameVal" runat="server" SkinID="lblNormal" Visible="False"></asp:Label>
                                            </td>
                                            <tr>
                                                <td align="left">
                                                    <asp:Label ID="lblStaffNo" runat="server" SkinID="lblNormal" Text="Staff No. :"></asp:Label>
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblStaffNoVal" runat="server" SkinID="lblNormal"></asp:Label>
                                                </td>
                                            </tr>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label ID="lblUserAssignmentForLeaveApplication" runat="server" SkinID="lblSectionTitle"
                                                                Text="User Assignment for Training"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            &nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" width="100px">
                                                            <input id="txtHidSUID" runat="server" class="TXTINPUT_READONLY_HIDDEN" type="hidden" /></input></input><asp:Label
                                                                ID="lblAvailableUsers" runat="server" SkinID="lblNormal" Text="Staff List"></asp:Label>
                                                        </td>
                                                        <td valign="top">
                                                            <asp:LinkButton ID="lbtnSortByStaffNo" runat="server" OnClick="lbtnSortByStaffNo_Click">Sort by Staff No.</asp:LinkButton>&nbsp;&nbsp;<asp:LinkButton
                                                                ID="lbtnSortByLoginID" runat="server" OnClick="lbtnSortByLoginID_Click">Sort by Username</asp:LinkButton>&nbsp;&nbsp;
                                                            <asp:LinkButton ID="lbtnSortByFullName" runat="server" OnClick="lbtnSortByFullName_Click">Sort by Full Name</asp:LinkButton>&nbsp;&nbsp;
                                                            <asp:Panel ID="pnlAvailableUsers" runat="server" SkinID="pnlMain" Width="100%">
                                                                <table border="0" cellpadding="0" cellspacing="0" width="99%">
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:ListBox ID="lstAvailableUsers" runat="server" Rows="10" SelectionMode="Multiple"
                                                                                SkinID="lstBox" Width="100%" BackColor="#99CCFF"></asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblTotAvailable" runat="server" SkinID="lblNormal" Text="Total Staff:"></asp:Label>&#160;&nbsp;<asp:Label
                                                                                ID="lblTotAvailableVal" runat="server" SkinID="lblValue"></asp:Label>
                                                                        </td>
                                                                        <td align="right">
                                                                            <asp:ImageButton ID="ibtnAttachUsers" runat="server" CausesValidation="False" ImageAlign="AbsMiddle"
                                                                                ImageUrl="~/images/attach.gif" OnClick="ibtnAttachUsers_Click" ToolTip="Assign selected user" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" width="100px">
                                                            &nbsp;&nbsp;
                                                        </td>
                                                        <td valign="top">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <table border="0" cellpadding="0" cellspacing="0" width="99%">
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="Label4" runat="server" SkinID="lblSectionTitle" Text="Assigned staffs"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" valign="middle">
                                                <asp:GridView ID="gvList" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                                                    DataKeyNames="Username" EmptyDataText="No record found" OnPageIndexChanging="gvList_PageIndexChanging"
                                                    OnRowCommand="gvList_RowCommand" OnRowCreated="gvList_RowCreated" SkinID="GridView"
                                                    Width="100%">
                                                    <PagerSettings PageButtonCount="20" />
                                                    <EmptyDataRowStyle ForeColor="Red" />
                                                    <Columns>
                                                        <asp:BoundField DataField="StaffID" HeaderText="Staff No." SortExpression="StaffID" />
                                                        <asp:BoundField DataField="Name" HeaderText="Full name" SortExpression="Name" />
                                                        <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lbtnRemove" runat="server" CausesValidation="False">Un-assign</asp:LinkButton></ItemTemplate>
                                                            <ItemStyle Width="80px" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTotAssigned" runat="server" SkinID="lblNormal" Text="Total Staff Assigned:"></asp:Label>&nbsp;&nbsp;<asp:Label
                                                    ID="lblTotAssignedVal" runat="server" SkinID="lblValue"></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="ibtnUnassign" runat="server" CausesValidation="False" ImageAlign="AbsMiddle"
                                                    ImageUrl="~/images/detachall.gif" OnClick="ibtnUnassign_Click" ToolTip="Un-assign all user" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                            <asp:TabPanel runat="server" HeaderText="Search Staff Assigned" ID="TabPanel2">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="Label5" runat="server" SkinID="lblSectionTitle" Text="Search Assigned staff"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="style2">
                                                <asp:Label ID="Label6" runat="server" Text="Search Staff :"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top" class="style2">
                                                <asp:Label ID="lblCreateMultiple" runat="server" Text="Search Multiple Staffs"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearchMultiple" runat="server" Height="110px" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="justify">
                                                <asp:Button ID="btnSearch1" runat="server" Text="Search" OnClick="btnSearch1_Click" /><asp:Button
                                                    ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" />
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
                                                <asp:GridView ID="gvStaffAssigned" Width="100%" runat="server" SkinID="GridView"
                                                    EmptyDataText="No record found" AllowPaging="True" AutoGenerateColumns="False"
                                                    PageSize="20">
                                                    <Columns>
                                                        <asp:BoundField DataField="StaffID" HeaderText="Staff ID" />
                                                        <asp:BoundField DataField="StaffName" HeaderText="Staff Name" />
                                                        <asp:BoundField DataField="WorkflowUnder" HeaderText="Workflow Under " />
                                                        <asp:BoundField DataField="Subsidiary" HeaderText="Subsidiary" />
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:TabPanel>
                        </asp:TabContainer>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td align="left">
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                Please wait...
                                <img id="Image2" runat="server" src="~/Images/ajax-loader.gif" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
