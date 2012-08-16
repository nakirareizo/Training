<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuggestedEvents.aspx.cs"
    Inherits="TrainingRequisition.Reports.SuggestedEvents" EnableEventValidation="false"
    MasterPageFile="~/Main.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 600px;
            background-color: #CCCCFF;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="5" class="style1">
        <tr>
            <td>
                List of events that have been suggested by staff. Events without dates are suggested
                during pre-booking. Events with dates are suggested during booking.
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:CheckBox ID="chkHideExportedEvents" runat="server" AutoPostBack="True" Checked="True"
                    OnCheckedChanged="chkHideExportedEvents_CheckedChanged" Text="Hide events that have been exported to Excel" />
            </td>
        </tr>
        <tr>
            <td style="margin-left: 40px">
                <asp:GridView ID="gvEvents" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CellPadding="4" DataKeyNames="EventID,EventDateID"
                    DataSourceID="dsEvents" ForeColor="#333333" GridLines="None" OnRowDataBound="gvEvents_RowDataBound"
                    Width="600px">
                    <RowStyle BackColor="#EFF3FB" />
                    <Columns>
                        <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                        <asp:BoundField DataField="Provider" HeaderText="Provider" SortExpression="Provider" />
                        <asp:BoundField DataField="Price" DataFormatString="{0:0.00}" HeaderText="Price (RM)"
                            SortExpression="Price">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TrainingType" HeaderText="Training Type" SortExpression="TrainingType" />
                        <asp:BoundField DataField="StartDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Start Date"
                            SortExpression="StartDate" />
                        <asp:BoundField DataField="EndDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="End Date"
                            SortExpression="EndDate" />
                        <asp:TemplateField HeaderText="Staff Name">
                            <ItemTemplate>
                                <asp:Label ID="lblStaffName" runat="server" Text="Label"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td style="margin-left: 40px">
                <asp:Button ID="btnExcel" runat="server" OnClick="btnExcel_Click" Text="Export to Excel" />
            </td>
        </tr>
    </table>
    </div>
    <asp:SqlDataSource ID="dsEvents" runat="server" ConnectionString="<%$ ConnectionStrings:ESSConnectionString %>"
        SelectCommand="SELECT REQ_Events.ID AS EventID, REQ_Events.Title, REQ_Events.Provider, REQ_Events.Price, REQ_Events.TrainingType, REQ_EventDates.StartDate, REQ_EventDates.EndDate, REQ_EventDates.ID AS EventDateID FROM REQ_Events LEFT OUTER JOIN REQ_EventDates ON REQ_Events.ID = REQ_EventDates.EventId WHERE (REQ_Events.UserDefined = 1) AND (REQ_Events.ExportedToExcel IS NULL) AND (@HideExported = 1) OR (REQ_Events.UserDefined = 1) AND (@HideExported = 0) ORDER BY REQ_Events.Title">
        <SelectParameters>
            <asp:ControlParameter ControlID="chkHideExportedEvents" Name="HideExported" PropertyName="Checked" />
        </SelectParameters>
        <FilterParameters>
            <asp:ControlParameter ControlID="chkHideExportedEvents" DefaultValue="True" Name="HideExported"
                PropertyName="Checked" />
        </FilterParameters>
    </asp:SqlDataSource>
</asp:Content>
