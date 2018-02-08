<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Illustration Request Database</h1>
    </div>
    <asp:Table ID="Table1" runat="server" Height="86px" Width="100%">
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                <h2>Helpful Links</h2><br />
                <asp:BulletedList ID="HelfulLinks" runat="server">
                    <asp:ListItem><a href="Documents/Illustration Request Database 8-28-2017.pdf">How to use the Illustration Request Database</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/IPS-PRO-0003 Technical Publications Illustration Request Procedure.docx">Illustration request process</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/TRAINING-A-04-10-0005-00A-010B-Z.pdf">Illustration rules (S1000D)</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/TRAINING-A-02-50-0005-00A-010B-Z.pdf">Illustration style guide</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/Isodraw shortcut keys_Consolidated.pdf">Isodraw: Quick keys</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/Updating-Adding Library Items.pdf">Isodraw: Updating library</a></asp:ListItem>
                </asp:BulletedList>
                <h2>MIL Standards</h2><br />
                <asp:BulletedList runat="server">
                    <asp:ListItem><a href="Documents/MIL-HDBK-1222E.pdf">MIL-HDBK-1222E</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/MIL-PRF-63029J.pdf">MIL-PRF-63029J</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/MIL-STD-40051_2C.pdf">MIL-STD-40051_2C</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/MIL-STD-3031A.pdf">MIL-STD-3031A</a></asp:ListItem>
                    <asp:ListItem><a href="Documents/S1000D Issue 4.0.pdf">S1000D Issue 4.0</a></asp:ListItem>
                </asp:BulletedList>
                <asp:Table runat="server" BorderStyle="Ridge">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Project</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Assigned Illustrator</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Work Phone Number</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell></asp:TableCell>
                        <asp:TableCell><a href="mailto:trussell@pinnaclesolutionsinc.com">Tim Russell</a></asp:TableCell>
                        <asp:TableCell>(859) 335-0009 x4006</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>MH-47G</asp:TableCell>
                        <asp:TableCell><a href="mailto:sreeves@pinnaclesolutionsinc.com">Scott Reeves</a></asp:TableCell>
                        <asp:TableCell>(859) 335-0009 x4014</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>MH-60M</asp:TableCell>
                        <asp:TableCell><a href="mailto:leaster@pinnaclesolutionsinc.com">Lance Easter</a></asp:TableCell>
                        <asp:TableCell>(859) 335-0009 x4013</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>MH-6M</asp:TableCell>
                        <asp:TableCell><a href="mailto:sbays@pinnaclesolutionsinc.com">Sophie Bays</a></asp:TableCell>
                        <asp:TableCell>(859) 335-0009 x4018</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>Waltonen</asp:TableCell>
                        <asp:TableCell><a href="mailto:athomas@pinnaclesolutionsinc.com">Andre Thomas</a></asp:TableCell>
                        <asp:TableCell>(248) 469-6109</asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
            <asp:TableCell VerticalAlign="Top">
                <h2>Release Notes</h2><br />
                <asp:BulletedList runat="server">
                    <asp:ListItem>Initial release</asp:ListItem>
                </asp:BulletedList>
                <h2>Development Wishlist</h2>
                <asp:BulletedList runat="server">
                    <asp:ListItem>Switch from Access backend to SQL Server backend</asp:ListItem>
                    <asp:ListItem>Add Windows client</asp:ListItem>
                </asp:BulletedList>

            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
</asp:Content>
