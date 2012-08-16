<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmAttendance.aspx.cs" Inherits="TrainingRequisition.ConfirmAttendance" MasterPageFile="~/Main.Master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="100%">
<tr>
<td class="PGTITLE">
    <asp:label ID="lblPgTitle" runat="server" 
        Text="Confirmation Prebook"></asp:label>
</td>
</tr>
<tr>
<td>
</td>
</tr>
</table>
<asp:Panel ID="pnlEvents" runat="server" width="99.9%" skinid="pnlMain">
<table>
<tr>
<td align="left">
<asp:Label ID="lblNote1" runat="server" >You have been booked for the following events. Please select your preferred dates and confirm your attendance. </asp:Label>
</td>
</tr>
<tr>
<td align="left">
<asp:Label ID="lblNote2" runat="server" Visible="false"><strong style = "color :#FF0000">Note</strong>: <b>ESS ADMIN</b> will send e-mail reminders until you confirm your attendance. If you are not able to attend the training below, please e-mail to <b><a href="mailto: essadmin@heitech.com.my">ESS Admin</a></b> , to cancel your booking. In your e-mail, please state the training title.</asp:Label>
</td>
</tr>
<tr>
<td>
<asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False" 
                    CellPadding="4" DataKeyNames="EventId,StaffUsername" ForeColor="#333333" 
                    GridLines="None" onrowdatabound="gvEvents_RowDataBound" Width="500px">
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <Columns>
                        <asp:TemplateField ItemStyle-VerticalAlign="Top" ItemStyle-Width="50px" >
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                            <ItemStyle VerticalAlign="Top" Width="50px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="EventDisplayName" HeaderText="Training Name" 
                           ItemStyle-VerticalAlign="Top"  >
                        <ItemStyle VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Available Dates" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Top" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:RadioButtonList ID="rbEventDates" runat="server">
                                </asp:RadioButtonList>
                            </ItemTemplate>
                            <ItemStyle VerticalAlign="Top" Width="200px" />
                        </asp:TemplateField>
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
<td>
<asp:Button ID="btnSelectAll" runat="server" onclick="btnSelectAll_Click" 
                    Text="Select All" />
                    <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
                    Text="Clear" />
                     <asp:Button ID="btnConfirm" runat="server" onclick="btnConfirm_Click" 
                    Text="Confirm" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
      onclick="btnCancel_Click" />
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
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
