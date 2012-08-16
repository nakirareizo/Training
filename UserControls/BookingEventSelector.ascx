<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookingEventSelector.ascx.cs"
    Inherits="TrainingRequisition.UserControls.BookingEventSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="Questions.ascx" TagName="Questions" TagPrefix="uc1" %>
<%@ Register Src="EventDetails.ascx" TagName="EventDetails" TagPrefix="uc2" %>
<style type="text/css">
    .style2
    {
        width: 980px;
    }
    .style3
    {
        width: 450px;
    }
</style>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table width="100%">
            <tr>
                <td>
                    <asp:Panel ID="pnlSearch" runat="server" Width="99.9%" SkinID="pnlMain">
                        <table cellpadding="5px">
                            <tr bgcolor="#EEEEEE" valign="top">
                                <td>
                                    Training Groups
                                </td>
                                <td />
                                From<td>
                                    <asp:TextBox ID="txtFrom" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="txtFrom_CalendarExtender" runat="server" Enabled="True"
                                        TargetControlID="txtFrom" Format="d/M/yyyy">
                                    </asp:CalendarExtender>
                                    <asp:CustomValidator ID="vldSearch" runat="server" Display="Dynamic" OnServerValidate="vldSearch_ServerValidate"
                                        ValidationGroup="Search"></asp:CustomValidator>
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr bgcolor="#EEEEEE" valign="top">
                                <td>
                                    <asp:DropDownList ID="dlEventGroups" runat="server" Width="250px">
                                    </asp:DropDownList>
                                </td>
                                <td />
                                To<td>
                                    <asp:TextBox ID="txtTo" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="txtTo_CalendarExtender" runat="server" Enabled="True" TargetControlID="txtTo"
                                        Format="d/M/yyyy">
                                    </asp:CalendarExtender>
                                    &nbsp;<br />
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr bgcolor="#EEEEEE" valign="top">
                                <td>
                                    Title &nbsp<asp:TextBox ID="txtSearchTitle" runat="server" Width="185px"></asp:TextBox>
                                </td>
                                <td />
                                &nbsp;<td>
                                    <asp:Button ID="btnShowAvailable" runat="server" Text="Show" OnClick="btnShowAvailable_Click"
                                        ValidationGroup="Search" />
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    Available Trainings
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" valign="top">
                                    <asp:ListBox ID="lbAvailable" runat="server" DataTextField="DisplayName" DataValueField="Id"
                                        Rows="10" SelectionMode="Multiple" Width="500px" Height="200px" AutoPostBack="true"
                                        OnSelectedIndexChanged="lbAvailable_SelectedIndexChanged"></asp:ListBox>
                                </td>
                                <td class="style3">
                                    <uc2:EventDetails ID="uscAvailableEventDetails" runat="server" Visible="false" ReadOnly="True" />
                                </td>
                            </tr>
                            <tr align="right">
                                <td colspan="3">
                                    <asp:Button ID="btnAcceptAll" runat="server" Text="Select All" OnClick="btnAcceptAll_Click" />
                                    <asp:Button ID="btnAccept" runat="server" Text="Select" OnClick="btnAccept_Click" />
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Label ID="lblSelectedEvents" Text="Selected Training" runat="server" Style="color: #FF0000"></asp:Label>
                                </td>
                                <td class="style3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr valign="top">
                                <td colspan="3" valign="top">
                                    <asp:ListBox ID="lbSelected" runat="server" DataTextField="DisplayName" DataValueField="Id"
                                        Rows="12" SelectionMode="Multiple" Width="497px" OnSelectedIndexChanged="lbSelected_SelectedIndexChanged"
                                        AutoPostBack="True" Height="200px"></asp:ListBox>
                                </td>
                                <td rowspan="2" class="style3">
                                    <asp:TabContainer ID="tcSelectedEvent" runat="server">
                                        <asp:TabPanel ID="tabSelectedEventDetails" HeaderText="Details" runat="server">
                                            <ContentTemplate>
                                                <table width="500px" style="width: 412px">
                                                    <tr>
                                                        <td>
                                                            <uc2:EventDetails ID="uscSelectedEventDetails" runat="server" Visible="False" ReadOnly="False" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:TabPanel>
                                        <asp:TabPanel ID="tabSelectedEventJustification" HeaderText="Justification" runat="server">
                                            <ContentTemplate>
                                                <uc1:Questions ID="uscQuestions" runat="server" />
                                            </ContentTemplate>
                                        </asp:TabPanel>
                                    </asp:TabContainer>
                                </td>
                            </tr>
                            <tr>
                            <td colspan="4"></td>
                            </tr>
                            <tr>
                            <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4" style="text-align: center">
                                    <asp:Button ID="btnUnselect" runat="server" Text="Remove" OnClick="btnUnselect_Click" />
                                    &nbsp;<asp:Button ID="btnUnselectAll" runat="server" Text="Remove All" OnClick="btnUnselectAll_Click" />
                                    <asp:Button ID="btnSuggest" runat="server" OnClick="btnSuggest_Click" Text="Suggest"
                                        Visible="False" />
                                    <asp:Label ID="lblMessage" runat="server" Text="" Style="color: #FF3300"></asp:Label>
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
