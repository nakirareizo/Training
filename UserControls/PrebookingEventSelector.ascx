<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrebookingEventSelector.ascx.cs"
    Inherits="TrainingRequisition.UserControls.PrebookingEventSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="Questions.ascx" TagName="Questions" TagPrefix="uc1" %>
<asp:updatepanel id="UpdatePanel1" runat="server" updatemode="Conditional">
<contenttemplate> 
<table width="100%">
    <tr>
        <td colspan="3">
            Group
            <asp:DropDownList ID="dlEventGroups" runat="server" Width="209px">
            </asp:DropDownList>
            &nbsp;Title&nbsp;
            <asp:TextBox ID="txtSearchTitle" runat="server" Width="200px"></asp:TextBox>
            <asp:Button ID="btnShowAvailable" runat="server" Text="Show" OnClick="btnShowAvailable_Click" />
        </td>
    </tr>
    <tr>
        <td>
            Available Trainings</td>
        <td />
        <td>
            &nbsp;<asp:Label ID="lblSelectedEvents" Text="Selected Training" runat="server" 
               style="color: #FF0000"></asp:Label>
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:ListBox ID="lbAvailable" runat="server" DataTextField="Title" DataValueField="Id"
                Rows="12" SelectionMode="Multiple" Width="500px" ></asp:ListBox>
            <br />
            <br />
        </td>
        <td>
            <asp:Button ID="btnAcceptAll" runat="server" Text="&gt;&gt;" OnClick="btnAcceptAll_Click" /><br />
            <asp:Button ID="btnAccept" runat="server" Text="&gt;" OnClick="btnAccept_Click" /><br />
            <asp:Button ID="btnUnselect" runat="server" Text="&lt;" OnClick="btnUnselect_Click" /><br />
            <asp:Button ID="btnUnselectAll" runat="server" Text="&lt;&lt;" OnClick="btnUnselectAll_Click" />
        </td>
        <td valign="top">
            <asp:ListBox ID="lbSelected" runat="server" DataTextField="Title" DataValueField="Id"
                Rows="12" SelectionMode="Multiple" Width="500px" 
                OnSelectedIndexChanged="lbSelected_SelectedIndexChanged" AutoPostBack="true">
            </asp:ListBox>
        </td>
    </tr>
    <tr>
        <td valign="top" colspan="3">
            <asp:Panel ID="pnlQuestions" runat="server" Visible="false">
                <table bgcolor="#E1E1E1" cellpadding="10" cellspacing="0" style="width:1032px">
                    <tr>
                        <td>
                            <uc1:Questions ID="uscQuestions" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr valign="bottom">
        <td colspan="3">
            <asp:Panel ID="pnlSuggestNewEvent" Visible="false" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Suggest A New Event"></asp:Label>
                <asp:TextBox ID="txtEventName" runat="server" Width="339px"></asp:TextBox>
                &nbsp;<asp:Button ID="btnAddUserDefinedEvent" runat="server" Text="Add >" OnClick="btnAddUserDefinedEvent_Click"
                    ValidationGroup="Suggest" />
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEventName"
                    Display="Dynamic" ErrorMessage="RequiredFieldValidator" ValidationGroup="Suggest">Please enter a title of the suggested event.</asp:RequiredFieldValidator>
            </asp:Panel>
        </td>
    </tr>
</table>
<p><asp:Label ID="lblMessage" runat="server" style="color: #FF0000"></asp:Label></p>
</contenttemplate>
</asp:updatepanel>
        

