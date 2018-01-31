<%@ Page Title="Requests Submitted" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Submitted.aspx.cs" Inherits="Queue_success" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <h1>Requests Submitted</h1>
        <div>
            List of requests, IDs, and attachments submitted.
        </div>
        <div>
            List of failed requests.
        </div>
        <div>
            <a href="Default.aspx">Submit another request</a>
        </div>
    </div>
</asp:Content>

