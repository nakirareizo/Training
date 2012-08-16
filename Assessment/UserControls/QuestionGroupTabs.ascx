<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="QuestionGroupTabs.ascx.cs"
    Inherits="TrainingRequisition.Assessments.UserControls.QuestionGroupTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="QuestionGroup.ascx" TagName="QuestionGroup" TagPrefix="uc1" %>
<asp:TabContainer ID="tcTabs" runat="server" ActiveTabIndex="0">
    <asp:TabPanel runat="server" HeaderText="TabPanel0" ID="TabPanel0">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup0" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel1">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup1" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel2">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup2" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel3">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup3" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel4">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup4" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel5">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup5" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel6">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup6" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel7">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup7" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel8">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup8" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel9">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup9" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel runat="server" HeaderText="" ID="TabPanel10">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup10" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel11">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup11" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel12">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup12" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel13">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup13" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel14">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup14" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel15">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup15" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel16">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup16" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel17">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup17" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel18">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup18" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel19">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup19" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
     <asp:TabPanel runat="server" HeaderText="" ID="TabPanel20">
        <ContentTemplate>
            <uc1:QuestionGroup ID="QuestionGroup20" runat="server" />
        </ContentTemplate>
    </asp:TabPanel>
    
</asp:TabContainer>
<asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="Save" />
<asp:Button ID="btnSubmit" runat="server" onclick="btnSubmit_Click" 
    Text="Submit" />

<asp:Label ID="lblMustAnswer" runat="server" 
    Text="Questions marked * must be answered." Visible="False"></asp:Label>


