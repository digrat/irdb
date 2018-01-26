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
                <h3>Request Details</h3>
                <asp:UpdatePanel ID="RequestDetails" runat="server" ChildrenAsTriggers="true" >
                    <ContentTemplate>
                        <label>User Name</label><br />
                        <asp:DropDownList ID="UserName" runat="server" BorderStyle="Groove" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="CreateRequestDetailsCookie">
                        </asp:DropDownList>
                        <br />
                        <label>Project Name</label><br />
                        <asp:DropDownList ID="Project" runat="server" BorderStyle="Groove" AppendDataBoundItems="true" Width="180" AutoPostBack="true" OnSelectedIndexChanged="CreateRequestDetailsCookie">
                        </asp:DropDownList>
                        <br />
                        <label>Request Type</label><br />
                        <asp:DropDownList ID="RequestType" runat="server" BorderStyle="Groove" AppendDataBoundItems="true" Width="180" AutoPostBack="true" OnSelectedIndexChanged="CreateRequestDetailsCookie">
                        </asp:DropDownList>
                        <br />
                        <label>Manual Type</label><br />
                        <asp:DropDownList ID="ManualType" runat="server" BorderStyle="Groove" AppendDataBoundItems="true" Width="180" AutoPostBack="true" OnSelectedIndexChanged="CreateRequestDetailsCookie">
                        </asp:DropDownList>
                        <br />
                        <label>DMC / WP / Chapter</label><br />
                        <asp:TextBox ID="DataModuleCode" runat="server" BorderStyle="Groove" Width="365px" AutoPostBack="true" OnTextChanged="CreateRequestDetailsCookie"/>
                        <br />
                        <label>Title</label><br />
                        <asp:TextBox ID="DataModuleTitle" runat="server" BorderStyle="Groove" Width="365px" AutoPostBack="true" OnTextChanged="CreateRequestDetailsCookie"/>
                        <br />
                        <label>Conversion Source</label><br />
                        <asp:TextBox ID="ConversionSource" runat="server" BorderStyle="Groove" Width="365px" AutoPostBack="true" OnTextChanged="CreateRequestDetailsCookie"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="vertical-align:top;padding-left:25px;padding-right:25px;text-align:left;width:20%">
                <h3>ICNs</h3>
                <div class="form-group">
                    <asp:Panel ID="IllustrationControlNumberPanel" runat="server">
                        <table style="padding:9px">
                            <tr>
                                <td>
                                    <asp:ListBox ID="IllustrationControlNumbers" runat="server" Width="125" Height="200"></asp:ListBox>
                                </td>
                                <td style="vertical-align:top">
                                    <asp:ImageButton ID="FindIllustrationControlNumberPopupOpen" runat="server" ToolTip="Look up ICN" ImageUrl="~/Images/find.png" Height="16" Width="16" /><br />
                                    <asp:ImageButton ID="NewIllustrationControlNumberPopupOpen" runat="server" ToolTip="Create new ICN" ImageUrl="~/Images/add.png" Height="16" Width="16"/><br />
                                    <asp:ImageButton ID="DeleteIllustrationControlNumber" runat="server" OnClick="DeleteIllustrationControlNumber_Click" ToolTip="Remove ICN from list" ImageUrl="~/Images/delete.png" Height="16" Width="16"/><br />
                                    <asp:TextBox ID="PlatformStore" runat="server" Visible="false" Width="16"></asp:TextBox><br />
                                    <asp:TextBox ID="FunctionalGroupStore" runat="server" Visible="false" Width="16"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Add known ICN:</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="KnownIllustrationControlNumber" runat="server" Width="120"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="AddKnownIllustrationControlNumber" runat="server" Text="Add" OnClick="AddKnownIllustrationControlNumber_Click" />
                                </td>
                            </tr>
                        </table>
                        <asp:Button ID="ValidateIllustrationControlNumbers" runat="server" Text="Check ICNs" OnClick="ValidateIllustrationControlNumbers_Click" /><br />
                        <asp:Label ID="ValidateIllustrationControlNumbersText" runat="server" Visible="false" Text="" /><br />
                        <asp:Label ID="IcnControlNumber" runat="server" visible="true" Text=""></asp:Label>
                    </asp:Panel>
                </div>
            </td>
            <td style="vertical-align:top">
                <h3>Attachments</h3>
                <div class="form-group">
                    <ajaxToolkit:AjaxFileUpload id="AjaxFileUpload" OnUploadComplete="AjaxFileUpload_UploadComplete" OnUploadCompleteAll="AjaxFileUpload_UploadCompleteAll" runat="server" Width="100%" ClearFileListAfterUpload="false" /> <br />  
                    <!-- <asp:Image id="MyThrobber" ImageUrl="~/Images/ajax-loader.gif" Style="display:None" runat="server" /> -->
                    Note: If a request is being submitted with multiple ICNs, files with names beginning with an ICN will be attached only to the ICN it is named after. All other files will be attached to all requests.<br />
                    <asp:UpdatePanel ID="UploadedFilesPanel" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <asp:GridView ID="UploadedFiles" runat="server" AutoGenerateColumns="false" EmptyDataText="There are no uploaded files." Width="100%" >
                                <Columns>
                                    <asp:TemplateField HeaderText="File Name">
                                        <ItemTemplate>
                                            <asp:Label ID="FileName" Text='<%# Bind("FileName") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <AlternatingRowStyle BackColor="#ffffff" />
                                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />  
                                <HeaderStyle BackColor="#222222" Font-Bold="false" ForeColor="#999999" />  
                                <RowStyle BackColor="#DEDFDE" ForeColor="Black" />  
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="AjaxFileUpload" EventName="UploadCompleteAll" />
                            <asp:AsyncPostBackTrigger ControlID="ClearAttachments" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div style="text-align:right">
                        <asp:Button ID="ClearAttachments" runat="server" Text="Clear Attachments" OnClick="ClearAttachments_Click" />
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td class="auto-style1">
            </td>
            <td class="auto-style2"></td>
            <td>
                <div class="form-group" style="text-align:right">
                    <asp:Button ID="Submit" runat="server" Text="Submit" OnClick="Submit_Click" />
                </div>
                <br />
            </td>
        </tr>
    </table>
    <cc1:ModalPopupExtender ID="FindPopupExtender" runat="server" PopupControlID="FindIllustrationPopup" TargetControlID="FindIllustrationControlNumberPopupOpen"
        CancelControlID="CloseFindPopup" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="FindIllustrationPopup" runat="server" CssClass="modalFindPopup" style="display:none;align-content:center">
        <table style="text-align:center">
            <tr>
                <td style="vertical-align:top">
                    <table>
                        <tr>
                            <td style="text-align:right">
                                <label>Platform</label> 
                            </td>
                            <td style="text-align:left">
                                <asp:DropDownList ID="FindPlatform" runat="server" DataTextField="field_description" AppendDataBoundItems="true" OnSelectedIndexChanged="FindPlatform_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="" Value="none"/>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right">
                                <label>Functional Group</label> 
                            </td>
                            <td style="text-align:left">
                            <asp:UpdatePanel ID="FindFunctionalGroupPanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="FindFunctionalGroup" runat="server" DataTextField="field_value" Width="45">
                                    </asp:DropDownList>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="FindPlatform" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style="text-align:right;padding-top:5px;padding-bottom:5px">
                                <asp:Button ID="FindIllustrationControlNumber" runat="server" Text="Find ICNs" OnClick="FindIllustrationControlNumber_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right;vertical-align:top">
                                <label>ICNs</label> 
                            </td>
                            <td style="text-align:left">
                            <asp:UpdatePanel ID="FindICNListPanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:ListBox ID="FindICNList" runat="server" DataTextField="illustration_control_number" AppendDataBoundItems="true" SelectionMode="Multiple" Width="200" Height="100">
                                    </asp:ListBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="FindIllustrationControlNumber" />
                                </Triggers>
                            </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align:right">
                    <asp:Button ID="CloseFindPopup" runat="server" Text="Cancel" />&nbsp;
                    <asp:Button ID="SelectFoundIllustrationControlNumbers" runat="server" Text="Select ICNs" OnClick="SelectIllustrationControlNumber_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="AddPopupExtender" runat="server" PopupControlID="AddIllustrationPopup" TargetControlID="NewIllustrationControlNumberPopupOpen"
        CancelControlID="CloseAddPopup" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="AddIllustrationPopup" runat="server" CssClass="modalAddPopup" style="display:none;align-content:center">
        <table style="text-align:center">
            <tr>
                <td style="vertical-align:top">
                    <table>
                        <tr>
                            <td style="text-align:right">
                                <label>Platform</label> 
                            </td>
                            <td style="text-align:left">
                                <asp:DropDownList ID="AddPlatform" runat="server" DataTextField="field_description" AppendDataBoundItems="true" OnSelectedIndexChanged="AddPlatform_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="" Value="none"/>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right">
                                <label>Functional Group</label> 
                            </td>
                            <td style="text-align:left">
                            <asp:UpdatePanel ID="AddFunctionalGroupPanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="AddFunctionalGroup" runat="server" DataTextField="field_value" Width="45">
                                    </asp:DropDownList>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="AddPlatform" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align:right;padding-top:10px">
                    <asp:Button ID="CloseAddPopup" runat="server" Text="Cancel" OnClick="CloseAddPopup_Click" />&nbsp;
                    <asp:Button ID="AddIllustrationControlNumber" runat="server" Text="Add ICN" OnClick="AddIllustrationControlNumber_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="ErrorLabel" runat="server" Width="600" Visible="false" />
</asp:Content>
