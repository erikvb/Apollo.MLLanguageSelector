<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Settings.ascx.vb" Inherits="Apollo.DNN.SkinObjects.MLLanguageSelector.MLLSSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table id="Table1" cellspacing="0" cellpadding="4" width="500" border="0">
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plDisplayType" text="Display type:" controlname="ddlDisplayTypes"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:DropDownList ID="ddlDisplayTypes" runat="server">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plSeparator" text="Seperator" controlname="txtSeperator" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtSeparator" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plCssClass" text="CSS Class Name:" controlname="txtCssClass" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtCssClass" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plHideCurrent" text="Hide current:" controlname="cbHideCurrent" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbHideCurrent" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plAlignment" text="Alignment:" controlname="ddlAllignment" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:DropDownList ID="ddlAlignment" runat="server">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plHyperlinks" text="Show Hyperlinks:" controlname="cbHyperlinks" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbHyperlinks" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plFlags" text="Show Flags:" controlname="cbFlags" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbFlags" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plMenu" text="Show Drop Down Menu:" controlname="cbMenu" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbMenu" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plMenuFlagPosition" text="Position of active language flag:" controlname="cbMenuFlagPosition"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:DropDownList ID="ddlMenuFlagPosition" runat="server">
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plDisplayLabel" text="Show Label:" controlname="cbDisplayLabel" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbDisplayLabel" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plLabelCssClass" text="Label CSS Class:" controlname="txtLabelCSSClass"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtLabelCSSClass" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plOnlyLanguageCode" text="Use only language code:" controlname="cbOnlyLanguageCode"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbOnlyLanguageCode" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plForceHidden" text="Hide selector:" controlname="cbForceHidden" runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbForceHidden" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plMapLanguages" text="Map Languages:" controlname="txtMapLanguages"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtMapLanguages" runat="server" TextMode="MultiLine" Width="256px"
                Height="64px"></asp:TextBox><asp:CustomValidator ID="valMapLanguages" runat="server"
                    CssClass="normalred" ErrorMessage="<br>Syntax is not correct. Either u used a non existent language code, wrong punctiation or a circular reference"
                    Display="Dynamic" ControlToValidate="txtMapLanguages" EnableClientScript="False"
                    resourceKey="valMapLanguages.Error"></asp:CustomValidator></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plMapDomains" runat="server" controlname="txtMapLanguages" text="Map Domains:">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtMapDomains" runat="server" Height="64px" Width="256px" TextMode="MultiLine"></asp:TextBox>
            <asp:CustomValidator ID="valMapDomains" runat="server" resourceKey="valMapDomains.Error"
                EnableClientScript="False" ControlToValidate="txtMapDomains" Display="Dynamic"
                ErrorMessage="<br>Syntax is not correct. Either u used a non existent language code, wrong punctiation or a circular reference"
                CssClass="normalred"></asp:CustomValidator></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plUseStyleSheetForSkinObject" runat="server" controlname="cbUseStyleSheetForSkinObject"
                text="Use Style Sheet For Skinobject:">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbUseStyleSheetForSkinObject" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plUseFullLocaleCode" runat="server" controlname="cbUseFullLocaleCode"
                text="Use Full Locale Code:">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbUseFullLocaleCode" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plTableLess" runat="server" controlname="cbTableLess" text="Tableless:">
            </dnn:label></td>
        <td valign="top">
            <asp:CheckBox ID="cbTableLess" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plAltFlagLocation" runat="server" controlname="txtAltFlagLocation"
                text="Alternate Flags Location:">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtAltFlagLocation" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plAltImageType" runat="server" controlname="txtaltImageType" text="Alternate Image Type:">
            </dnn:label></td>
        <td valign="top">
            <asp:TextBox ID="txtaltImageType" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plFlagType" runat="server" controlname="drpFlagType" text="Image Style:"> 
            </dnn:label></td>
        <td valign="top">
        <asp:DropDownList ID="drpFlagType" runat="server"></asp:DropDownList>
            </td>
    </tr>
</table>
<dnn:sectionhead id="dshSkinObjectSettings" text="Skin Object Settings" runat="server"
    cssclass="Head" section="tblSKOSettings" resourcekey="SKOSettings" includerule="false"
    isExpanded="False">
</dnn:sectionhead>
<table id="tblSKOSettings" cellspacing="0" cellpadding="4" width="500" border="0"
    runat="server">
    <tr>
        <td class="subHead" valign="top" width="200">
            <dnn:label id="plGenerateType" text="Generate for:" controlname="ddlDisplayTypes"
                runat="server">
            </dnn:label></td>
        <td valign="top">
            <asp:RadioButtonList ID="rblGenerationType" runat="server" RepeatDirection="Horizontal"
                CssClass="subhead">
                <asp:ListItem Value="ascx" Selected="True">ascx</asp:ListItem>
                <asp:ListItem Value="xml">xml</asp:ListItem>
                 <asp:ListItem Value="html">html (DNN 5)</asp:ListItem>
           </asp:RadioButtonList></td>
        <td valign="top">
            <asp:Button ID="btnGenerateNow" runat="server" Text="Generate Now" ResourceKey="btnGenerateNow.Text">
            </asp:Button></td>
    </tr>
    <tr>
        <td class="subHead" valign="top" width="200">
        </td>
        <td valign="top" colspan="2">
            <asp:TextBox ID="txtSKOAttributes" runat="server" TextMode="MultiLine" Width="256px"
                Height="256px"></asp:TextBox></td>
    </tr>
</table>
