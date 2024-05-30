<%@ Page Language="C#" CodeBehind="Login.aspx.cs" MasterPageFile="Site.Master" Inherits="WebFormsIdentity.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <h2>Login</h2>
            <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" DisplayRememberMe="False">
            </asp:Login>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <h2>Welcome, <asp:LoginName ID="LoginName1" runat="server" />!</h2>
            <asp:LoginStatus ID="LoginStatus1" runat="server" />
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>