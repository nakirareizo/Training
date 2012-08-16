<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ImportTrainingModel.aspx.cs" Inherits="BUILD.Training.SuperAdmin.ImportTrainingModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:updatepanel id="UpdatePanel1" runat="server" updatemode="Conditional">
        <contenttemplate> 
        <table width="100%">
    <tr>
        <td class="PGTITLE">
            <asp:label ID="lblPgTitle" runat="server" 
                Text="Import Training Model"></asp:label>
        </td>
    </tr>
    <tr>
       <td>
       </td>
   </tr>
 </table>
    <table width="100%">
<tr>
 <td align="left">
    <asp:Label ID="lblImport" Text="Import new model from SAP" runat="server"></asp:Label>
    </td>
    </tr>
    <tr>
      <td align="left">
        <asp:Button ID="btnImportModel" runat="server" Style="text-align: left"
            Text="Import Model" onclick="btnImportModel_Click" />
    </td>
    </tr> 
    <tr>
        <td align="left">
                <asp:updateprogress ID="updateprogress1" runat="server" associatedupdatepanelid="UpdatePanel1" >
                                 <ProgressTemplate>
                                 Import in progress, please wait...
                                    <img ID="Image1" runat="server" src="~/images/ajax-loader.gif" alt="" />
                                </ProgressTemplate>
                            </asp:updateprogress>
                </td>
        </tr>
    </table>
    </contenttemplate>
    </asp:updatepanel>
</asp:Content>
