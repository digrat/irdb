<%@ Page Title="Queue" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- idea for table taken from site
        http://www.c-sharpcorner.com/UploadFile/009464/bind-gridview-with-ms-access-database-in-Asp-Net-C-Sharp/ -->
    <div class="row">
        <h1>Request Queue</h1>
        <div>
            <asp:DropDownList ID="UserList" runat="server" AppendDataBoundItems="true">
                <asp:ListItem Text="Select User" Value="none"/>
            </asp:DropDownList>
            <asp:Button ID="Search" runat="server" Text="Filter" OnClick="Search_Click"/>
            <asp:GridView ID="GridView1"
                runat="server" 
                AllowSorting="true" 
                AutoGenerateColumns="false"
                CellPadding="3" 
                DataKeyNames="request_index_number" 
                EmptyDataText="There are no requests in the queue." 
                OnSorting="GridView1_Sorting">
                <Columns>
                    <asp:TemplateField HeaderText="Request Index Number" SortExpression="request_index_number">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("request_index_number") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("request_index_number") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ICN" SortExpression="filename">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("filename") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("filename") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status" SortExpression="request_status">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("request_status") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("request_status") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Requested By" SortExpression="requested_by">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("requested_by") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("requested_by") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DMC/WP/Chapter" SortExpression="data_module_code">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("data_module_code") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("data_module_code") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Request Date" SortExpression="request_date">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("request_date") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("request_date") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Illustrator" SortExpression="illustrator_assigned">
                        <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("illustrator_assigned") %>'>
                                </asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("illustrator_assigned") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle BackColor="#ffffff" />
                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />  
                <HeaderStyle BackColor="#222222" Font-Bold="false" ForeColor="#999999" />  
                <RowStyle BackColor="#DEDFDE" ForeColor="Black" />  

                <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />  
                <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />  
                <SortedAscendingCellStyle BackColor="#F1F1F1" />  
                <SortedAscendingHeaderStyle BackColor="#594B9C" />  
                <SortedDescendingCellStyle BackColor="#CAC9C9" />  
                <SortedDescendingHeaderStyle BackColor="#33276A" />  
            </asp:GridView>
        </div>
        <div>
            <asp:GridView ID="GridView2" runat="server">

            </asp:GridView>
        </div>
    </div>
</asp:Content>
