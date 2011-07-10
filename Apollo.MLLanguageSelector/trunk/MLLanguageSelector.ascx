<%@ Control Language="vb" AutoEventWireup="false" Codebehind="MLLanguageSelector.ascx.vb"
    Inherits="Apollo.DNN.SkinObjects.MLLanguageSelector.MLLanguageSelector"  %>
<%@ Register TagPrefix="solpart" Namespace="Solpart.WebControls" Assembly="SolpartWebControls" %>
<asp:Literal ID="pre" runat="server"></asp:Literal>
<div class="MLLanguageMenuDiv" id="MLLanguageMenuDiv" runat="server">
    <span class="MLLanguageMenuLabelSpan" id="MLLanguageMenuLabelSpan" runat="server">
        <asp:Label ID="lblLabelTL" runat="server"></asp:Label></span> <span class="MLLanguageMenuLeftFlagSpan"
            id="MLLanguageMenuLeftFlagSpan" runat="server">
            <asp:Image ID="flagLeftImageTL" runat="server"></asp:Image></span> <span class="MLLanguageMenuMenuSpan"
                id="MLLanguageMenuMenuSpan" runat="server">
                <asp:DropDownList ID="cboSelectLanguageTL" runat="server" CssClass="NormalTextBox"
                    AutoPostBack="True">
                </asp:DropDownList></span> <span class="MLLanguageMenuRightFlagSpan" id="MLLanguageMenuRightFlagSpan"
                    runat="server">
                    <asp:Image ID="flagRIghtImageTL" runat="server"></asp:Image></span></div>
<table class="MLLanguageMenuTable" id="MLLanguageMenuTable" cellspacing="0" runat="server">
    <tr>
        <td class="MLLanguageMenuLabelCell" id="MLLanguageMenuLabelCell" runat="server">
            <asp:Label ID="lblLabel" runat="server"></asp:Label></td>
        <td class="MLLanguageMenuLeftFlagCell" id="MLLanguageMenuLeftFlagCell" runat="server">
            <asp:Image ID="flagLeftImage" runat="server"></asp:Image></td>
        <td id="MLLanguageMenuMenuCell" runat="server" class="MLLanguageMenuMenuCell">
            <asp:DropDownList ID="cboSelectLanguage" runat="server" AutoPostBack="True" CssClass="NormalTextBox">
            </asp:DropDownList></td>
        <td id="MLLanguageMenuRightFlagCell" runat="server" class="MLLanguageMenuRightFlagCell">
            <asp:Image ID="flagRIghtImage" runat="server"></asp:Image></td>
    </tr>
</table>
<asp:Literal ID="mid" runat="server"></asp:Literal><asp:DataList ID="dlLanguages"
    runat="server" RepeatDirection="Horizontal">
    <ItemStyle Wrap="False" VerticalAlign="Middle"></ItemStyle>
    <ItemTemplate>
        <table id="MLLanguageSelectionItemTable" runat="server" cellspacing="0" class="MLLanguageSelectionItemTable"
            visible="<%# not tableLess %>">
            <tr class="MLLanguageSelectionItemRow">
                <td id="MLLanguageSelectionItemFlag" runat="server" class="MLLanguageSelectionItemFlag">
                    <asp:HyperLink ID="hlImgLanguageLink" runat="server" NavigateUrl='<%# getNavURL( DataBinder.Eval(Container, "DataItem.code"))%>'>
                        <asp:Image ID="hlImgLanguage" ImageUrl='<%# getImgURL( DataBinder.Eval(Container, "DataItem.code"))%>'
                            AlternateText='<%# DataBinder.Eval(Container, "DataItem.name") %>' class='<%# getStartClass( DataBinder.Eval(Container, "DataItem.code"))%>'
                            runat="server" onmouseover='<%# getMouseOver( DataBinder.Eval(Container, "DataItem.code"))%>'
                            onmouseout='<%# getMouseOut( DataBinder.Eval(Container, "DataItem.code"))%>' />
                    </asp:HyperLink>
                </td>
                <td id="MLLanguageSelectionItemURL" runat="server" class="MLLanguageSelectionItemURL">
                    <asp:HyperLink ID="hlLanguage" runat="server" NavigateUrl='<%# getNavURL( DataBinder.Eval(Container, "DataItem.code"))%>'>
						<%# DataBinder.Eval(Container, "DataItem.name") %>
                    </asp:HyperLink>
                </td>
            </tr>
        </table>
        <asp:HyperLink ID="Hyperlink1" runat="server" NavigateUrl='<%# getNavURL( DataBinder.Eval(Container, "DataItem.code"))%>'
            Visible="<%# Flags and TableLess %>">
            <asp:Image ID="Image1" ImageUrl='<%# getImgURL( DataBinder.Eval(Container, "DataItem.code"))%>'
                AlternateText='<%# DataBinder.Eval(Container, "DataItem.name") %>' class='<%# getStartClass( DataBinder.Eval(Container, "DataItem.code"))%>'
                runat="server" onmouseover='<%# getMouseOver( DataBinder.Eval(Container, "DataItem.code"))%>'
                onmouseout='<%# getMouseOut( DataBinder.Eval(Container, "DataItem.code"))%>' />
        </asp:HyperLink>
        <asp:HyperLink ID="Hyperlink2" runat="server" NavigateUrl='<%# getNavURL( DataBinder.Eval(Container, "DataItem.code"))%>'
            Visible="<%# Hyperlinks and TableLess %>">
			<%# DataBinder.Eval(Container, "DataItem.name") %>
        </asp:HyperLink>
    </ItemTemplate>
    <SeparatorTemplate>
        <asp:Label ID="lblSeperator" runat="server"></asp:Label>
    </SeparatorTemplate>
</asp:DataList><asp:Literal ID="post" runat="server"></asp:Literal>
