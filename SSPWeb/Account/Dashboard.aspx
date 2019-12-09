<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SSPWeb.Account.Dashboard" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div>
        <asp:PlaceHolder runat="server" ID="successPanel" ViewStateMode="Disabled" Visible="true">
            <table>
                <tr>
                    <td colspan ="2">
                        <asp:Label ID="Label1" runat="server" Text="CAC / GCAC    "></asp:Label>
                        <asp:TextBox ID="CAC" runat="server"></asp:TextBox>
                    </td>
                    <td colspan ="2">
                        <asp:Label ID="Label2" runat="server" Text="Organization Name    "></asp:Label>
                        <asp:TextBox ID="Org" runat="server"></asp:TextBox>
                    </td>
                    <td colspan ="2">
                        <asp:Label ID="Label3" runat="server" Text="Parent CAC / GCAC    "></asp:Label>
                        <asp:TextBox ID="Parent" runat="server"></asp:TextBox>
                    </td>
                    <td colspan ="2">
                        <asp:Label ID="Label4" runat="server" Text="MyCB Type (Student, Admin etc.)    "></asp:Label>
                        <asp:TextBox ID="Type" runat="server"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <asp:Label ID="lblSuccessMessage" runat="server" Text="" ForeColor="Green"></asp:Label>
                </tr>
                <tr>
                    <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                </tr>
            </table>
            <br/>
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click"/>
            <br/>
            <br/>
            <br/>
            <br/>
            <br/>
            <br/>

            <asp:Label ID="lblUserDetails" runat="server" Text="" Visible="False"></asp:Label>
            <br/>
            <asp:Label ID="lblCount1" runat="server" Text="" Visible="False"></asp:Label><br/>
            <asp:Label ID="lblCount2a" runat="server" Text="" Visible="False"></asp:Label><br/>
            <asp:Label ID="lblCount2b" runat="server" Text="" Visible="False"></asp:Label><br/>
            <asp:Label ID="lblCount2" runat="server" Text="" Visible="False"></asp:Label><br/>
            <asp:Label ID="lblCount3" runat="server" Text="" Visible="False"></asp:Label><br/>
            <asp:Label ID="lblCount4" runat="server" Text="" Visible="False"></asp:Label><br/>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="errorPanel" ViewStateMode="Disabled" Visible="false">
            <p class="text-danger">
                An error has occurred.
            </p>
        </asp:PlaceHolder>
    </div>
</asp:Content>