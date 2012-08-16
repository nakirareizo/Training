<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostTrainingAssessment.aspx.cs"
    Inherits="TrainingRequisition.Assessments.PostTrainingAssessment" MasterPageFile="~/Main.Master" %>

<%@ Register Src="UserControls/QuestionGroup.ascx" TagName="RatingGroup" TagPrefix="uc1" %>
<%@ Register Src="UserControls/QuestionGroupTabs.ascx" TagName="QuestionGroupTabs"
    TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="UserControls/StaffList.ascx" TagName="StaffList" TagPrefix="uc3" %>
<%@ Register Src="UserControls/EventList.ascx" TagName="EventList" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <asp:updatepanel id="UpdatePanel1" runat="server" updatemode="Conditional">
    <contenttemplate>
    <table width="100%">
    <tr>
        <td class="PGTITLE">
            <asp:label ID="lblPgTitle" runat="server" 
                Text="Post-Training Assessment"></asp:label>
        </td>
    </tr>
    <tr>
       <td>
       </td>
   </tr>
 </table>
    <div>
    <table>
    <tr>
    <td>
    <uc3:StaffList ID="uscStaffList" runat="server" ShowSelf="false" />
    </td>

    </tr>
    <tr>
    <td>
      <uc4:EventList ID="uscEventList" runat="server" Visible="False" ModuleName="PTA" />
    </td>

    </tr>
    <tr>
    <td>
                    <uc2:QuestionGroupTabs ID="uscTabs" runat="server" Visible="False" />
    </td>

    </tr>
    </table>


    </div>
    <table width="100%">
    <tr>
                <td>
                <asp:updateprogress ID="updateprogress1" runat="server" associatedupdatepanelid="UpdatePanel1" >
                                 <ProgressTemplate>Please wait...
                                    <img ID="Image1" runat="server" src="~/images/ajax-loader.gif" alt="Please wait..." />
                                </ProgressTemplate>
                            </asp:updateprogress>
                </td>
                </tr>
                    </table>
                </contenttemplate>
                </asp:updatepanel>
</asp:Content>
