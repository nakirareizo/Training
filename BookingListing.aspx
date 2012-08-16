<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="BookingListing.aspx.cs" Inherits="BUILD.Training.BookingListing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%" runat="server">
<tr>
<td>
<asp:GridView ID="gvSAPList" runat="server" AutoGenerateColumns="False" 
      CellPadding="4" ForeColor="#333333" GridLines="None" >
   <AlternatingRowStyle BackColor="White" />
   <Columns>
      <asp:BoundField DataField="Title" HeaderText="Training Name" 
         SortExpression="Title" />
      <asp:BoundField DataField="BookedDate" HtmlEncode="false" DataFormatString="{0:D}"  HeaderText="Booked Date" SortExpression="BookedDate" />
      <asp:BoundField DataField="TrainingLoc" HeaderText="Training Location" 
         SortExpression="TrainingLoc" />
      <asp:BoundField DataField="StartDate" HtmlEncode="false" DataFormatString="{0:D}" HeaderText="Start Date" SortExpression="StartDate" />
      <asp:BoundField DataField="EndDate" HtmlEncode="false" DataFormatString="{0:D}" HeaderText="End Date" SortExpression="EndDate" />
      <asp:BoundField DataField="TrainingType" HeaderText="Training Type" 
         SortExpression="TrainingType" />
   </Columns>
   <EditRowStyle BackColor="#2461BF" />
   <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
   <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
   <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
   <RowStyle BackColor="#EFF3FB" />
   <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
   <SortedAscendingCellStyle BackColor="#F5F7FB" />
   <SortedAscendingHeaderStyle BackColor="#6D95E1" />
   <SortedDescendingCellStyle BackColor="#E9EBEF" />
   <SortedDescendingHeaderStyle BackColor="#4870BE" />
   </asp:GridView>
</td>
</tr>
</table>
</asp:Content>
