<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Questions.ascx.cs" Inherits="TrainingRequisition.UserControls.Questions" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:TabContainer ID="tabQuestions" runat="server" ActiveTabIndex="1">
    <asp:TabPanel runat="server" HeaderText="Staff Justification" ID="tabStaffNotes">
        <ContentTemplate>
            <asp:TextBox ID="txtStaffNotes" runat="server" Height="104px" TextMode="MultiLine"
                Width="428px" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel ID="tabSupervisorNotes" runat="server" HeaderText="Supervisor Justification">
        <ContentTemplate>
            <asp:TextBox ID="txtSupervisorNotes" runat="server" Height="103px" TextMode="MultiLine"
                Width="427px" />
        </ContentTemplate>
    </asp:TabPanel>
    <asp:TabPanel ID="tabRatings" runat="server" HeaderText="Ratings">
        <ContentTemplate>
            <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="False" CellPadding="4"
                DataSourceID="dsQuestions" ForeColor="#333333" GridLines="None" 
                Width="530px">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="DisplayName" HeaderText="Question" ReadOnly="True" SortExpression="DisplayName" />
                    <asp:TemplateField HeaderText="Rating">
                        <ItemTemplate>
                            <asp:RadioButtonList ID="rbRatings" runat="server" DataSourceID="dsRatings" DataTextField="Value"
                                DataValueField="Value" RepeatDirection="Horizontal">
                            </asp:RadioButtonList>
                            <asp:ObjectDataSource ID="dsRatings" runat="server" SelectMethod="GetAll" TypeName="TrainingRequisition.Classes.Rating">
                            </asp:ObjectDataSource>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            </asp:GridView>
        </ContentTemplate>
    </asp:TabPanel>
</asp:TabContainer>
<p>
    <asp:Button ID="btnApply" runat="server" onclick="btnApply_Click" 
        Text="Save Notes/Ratings" ValidationGroup="ValidateQuestions" style="height: 26px" />
    <asp:CustomValidator ID="vldQuestions" runat="server" Display="Dynamic" 
        onservervalidate="vldQuestions_ServerValidate" 
        ValidationGroup="ValidateQuestions"></asp:CustomValidator>
</p>
<asp:ObjectDataSource ID="dsQuestions" runat="server" SelectMethod="GetAll" TypeName="TrainingRequisition.Classes.Question">
</asp:ObjectDataSource>
