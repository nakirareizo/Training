<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="TrainingRequisition.Admin.Admin" MasterPageFile="~/Main.Master"  %>
<%@ OutputCache Duration="1" VaryByParam="None" %>
<%@ Register src="../UserControls/BookingOptions.ascx" tagname="BookingOptions" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    
      function pageLoad() {
      }
    
    </script>
    </asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
    <table width="100%">
<tr>
<td class="PGTITLE">
    <asp:label ID="lblPgTitle" runat="server" 
        Text="Post-Training Configuration"></asp:label>
</td>
</tr>
<tr>
<td>
</td>
</tr>
</table>
        <uc1:BookingOptions ID="BookingOptions1" runat="server" />
    </div>
</asp:Content>
