<%@ Page Title="Illustration Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .modalBackground
        {
            background-color: Black;
            filter: alpha(opacity=90);
            opacity: 0.8;
        }
        .modalAddPopup
        {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            width: 350px;
            height: 100px;
        }
        .modalFindPopup
        {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            width: 350px;
            height: 225px;
        }

    </style>

    <h1>Create Request</h1>
    <table>
        <tr>
            <td style="vertical-align:top">
                <h3>Attachments</h3>
                <div class="form-group">
                    <ajaxToolkit:AjaxFileUpload id="AjaxFileUpload" OnUploadComplete="AjaxFileUpload_UploadComplete" OnUploadCompleteAll="AjaxFileUpload_UploadCompleteAll" runat="server" Width="100%" ClearFileListAfterUpload="false" /> <br />  
                    <!-- <asp:Image id="MyThrobber" ImageUrl="~/Images/ajax-loader.gif" Style="display:None" runat="server" /> -->
                    Note: If a request is being submitted with multiple ICNs, files with names beginning with an ICN will be attached only to the ICN it is named after. All other files will be attached to all requests.<br />
                    <asp:UpdatePanel ID="UploadedFilesPanel" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <asp:GridView ID="UploadedFiles" runat="server" AutoGenerateColumns="false" >
                                <AlternatingRowStyle BackColor="LightSkyBlue" />
                                <Columns>
                                    <asp:TemplateField HeaderText="File Name">
                                        <ItemTemplate>
                                            <asp:Label ID="FileName" Text='<%# Bind("FileName") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />  
                                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />  
                                <RowStyle BackColor="#DEDFDE" ForeColor="Black" />  
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="AjaxFileUpload" EventName="UploadCompleteAll" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
    <asp:Label ID="ErrorLabel" runat="server" Width="600" Visible="false" />
</asp:Content>
