<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestModal.aspx.cs" Inherits="TrainingRequisition.TestPages.TestModal" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript">
    
      function pageLoad() {
      }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
        <br />
        <asp:Button ID="Button1" runat="server" Text="Button" />
        <asp:ModalPopupExtender ID="Button1_ModalPopupExtender" runat="server" 
            DynamicServicePath="" Enabled="True" TargetControlID="Button1"
            PopupControlID="pnlModal" OkControlID="btnOK" >
        </asp:ModalPopupExtender>
    </div>
    <asp:Panel ID="pnlModal" runat="server">
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="btnOK" runat="server" Text="Close" />
    </asp:Panel>
    </form>
</body>
</html>
