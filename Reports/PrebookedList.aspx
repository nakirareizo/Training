﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="PrebookedList.aspx.cs" Inherits="BUILD.Training.Reports.PrebookedList" %>
<%@ Register Src="../UserControls/StaffList.ascx" TagName="StaffList" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
        .style1
        {
            width: 500px;
        }
        .style2
        {
            width: 100%;
            background-color: #CCCCFF;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td class="PGTITLE">
                <asp:label ID="lblPgTitle" runat="server" 
                    Text="Prebook Training List"></asp:label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
    </tr>
 </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <table width="100%">
    <tr>
        <td>
        <asp:panel id="pnlMyForm" runat="server" width="99.9%" skinid="pnlMain">
            <table cellpadding="0" cellspacing="5" class="style1">
                <tr>
                    <td>
                        <table cellpadding="5" cellspacing="5" class="style2">
                            <tr>
                                <td style="margin-left: 40px">
                                    <uc1:StaffList ID="uscStaffList" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False" CellPadding="4"
                            ForeColor="#333333" GridLines="None" Width="500px">
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <Columns>
                                <asp:BoundField DataField="EventDisplayName" HeaderText="Event Name" />
                                <asp:BoundField DataField="PreBegin" HeaderText="Start Date" />
                                <asp:BoundField DataField="PreEnd" HeaderText="End Date" />
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
            </table>
            </asp:panel>
            </td>
            </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
