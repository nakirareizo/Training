<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="QuestionGroup.ascx.cs"
    Inherits="TrainingRequisition.Assessments.UserControls.QuestionGroupCtrl" %>
<p>
    <table cellpadding="10px" style="background-color:#EEEEEE">
        <tr>
            <td>
                <b>
                    <asp:Label ID="lblTitle" runat="server"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="False" CellPadding="4"
                    ForeColor="#333333" GridLines="None" Width="510px" 
                    onrowdatabound="gvQuestions_RowDataBound" DataKeyNames="Id" 
                    ondatabound="gvQuestions_DataBound">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="False" />
                        <asp:TemplateField HeaderText="Question" SortExpression="DisplayOrder">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DisplayName") %>'></asp:Label>
                                <asp:Label ID="lblAsterisk" runat="server" Text="*" Visible="false"/>
                                <br />
                                <asp:TextBox ID="txtAnswer" runat="server" Height="98px" TextMode="MultiLine" 
                                    Width="487px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rating">
                            <ItemTemplate>
                                <asp:RadioButtonList ID="rbRatings" runat="server" RepeatDirection="Horizontal" DataTextField="Value" DataValueField="Value" Width="250px">
                                </asp:RadioButtonList>
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
                <asp:ObjectDataSource ID="dsRatings" runat="server"></asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</p>
