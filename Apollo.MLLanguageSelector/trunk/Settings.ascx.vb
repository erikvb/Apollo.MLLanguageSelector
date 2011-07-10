'
' copyright (c) 2005-2007 by Erik van Ballegoij ( erik@apollo-software.nl ) ( http://www.apollo-software.nl )
'
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System.Globalization
Imports Apollo.DNN.SkinObjects.MLLanguageSelector.Components
Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Entities.Portals


Public MustInherit Class MLLSSettings
    Inherits Entities.Modules.ModuleSettingsBase

    Private objModules As Entities.Modules.ModuleController


#Region "private methods"

    Private Function NewListItem(ByVal textValue As String) As ListItem
        Dim item As ListItem = New ListItem() With {.Value = textValue, .Text = Localization.GetString(textValue + ".Text", LocalResourceFile)}
        Return item
    End Function

    Private Sub loadAlignment(ByVal selected As String)
        If ddlAlignment.Items.Count > 0 Then
            ddlAlignment.Items.Clear()
        End If
        ddlAlignment.Items.Add(NewListItem("Horizontal"))
        ddlAlignment.Items.Add(NewListItem("Vertical"))
        ddlAlignment.SelectedValue = selected
    End Sub

    Private Sub loadFlagPositioning(ByVal selected As String)
        If ddlMenuFlagPosition.Items.Count > 0 Then
            ddlMenuFlagPosition.Items.Clear()
        End If
        ddlMenuFlagPosition.Items.Add(NewListItem("None"))
        ddlMenuFlagPosition.Items.Add(NewListItem("Left"))
        ddlMenuFlagPosition.Items.Add(NewListItem("Right"))
        ddlMenuFlagPosition.SelectedValue = selected
    End Sub

    Private Sub loadFlagType(ByVal selected As String)
        If drpFlagType.Items.Count > 0 Then
            drpFlagType.Items.Clear()
        End If
        drpFlagType.Items.Add(NewListItem("Simple"))
        drpFlagType.Items.Add(NewListItem("Modern"))
        drpFlagType.SelectedValue = selected
    End Sub

    Private Sub loadDisplayTypes(ByVal selected As String)
        If ddlDisplayTypes.Items.Count > 0 Then
            ddlDisplayTypes.Items.Clear()
        End If

        Dim i As displayTypes
        For Each i In [Enum].GetValues(GetType(displayTypes))
            ddlDisplayTypes.Items.Add(NewListItem([Enum].GetName(GetType(displayTypes), i)))
        Next i

        ddlDisplayTypes.SelectedValue = selected
    End Sub

    Private Function loadSetting(ByVal settingName As String, ByVal defaultValue As String) As String
        Dim strRetValue As String = defaultValue
        If TabModuleSettings.Contains(settingName) Then
            If TabModuleSettings(settingName).ToString <> "" Then
                strRetValue = TabModuleSettings(settingName).ToString
            End If
        End If
        Return strRetValue
    End Function

    Private Sub updateSetting(ByVal settingName As String, ByVal value As String)
        If objModules Is Nothing Then
            objModules = New Entities.Modules.ModuleController
        End If
        ' Update TabModuleSettings
        objModules.UpdateTabModuleSetting(TabModuleId, settingName, value)
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' test whether localecode can be used as cultureinfo
    ''' </summary>
    ''' <param name="testme"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Function testLocale(ByVal testme As String) As Boolean
        Dim tempresult As Boolean
        Dim tempCI As CultureInfo = Nothing
        Try
            tempCI = New CultureInfo(testme)
        Catch
        End Try
        If tempCI IsNot Nothing Then
            tempresult = True
        Else
            tempresult = False
        End If
        Return tempresult

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' test whether domain is valid portal alias
    ''' </summary>
    ''' <param name="testMe"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	27-7-2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Function testDomain(ByVal testMe As String) As Boolean
        Dim tempresult As Boolean

        If testMe.IndexOf("/") = -1 Then
            tempresult = True
        Else
            tempresult = False
        End If
        If tempresult Then
            For Each portalAliasKey As String In PortalAliasController.GetPortalAliasLookup.Keys

                If testMe = PortalAliasController.GetPortalAliasLookup.Item(portalAliasKey).HTTPAlias Then
                    tempresult = True
                    Exit For
                End If
            Next
        End If
        Return tempresult

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' serverside validation routine for maplanguages command
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="args"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub valMapLanguages_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valMapLanguages.ServerValidate
        Dim tempMapLanguages As String() = args.Value.Split(";"c)
        Dim mappedFrom As New ArrayList
        args.IsValid = False
        If tempMapLanguages.Length <> 0 Then
            For i As Integer = 0 To tempMapLanguages.Length - 1
                If tempMapLanguages(i) <> "" Then
                    Dim mapTo As String() = tempMapLanguages(i).Split(":"c)
                    If mapTo.Length = 2 Then
                        If testLocale(mapTo(0)) And testLocale(mapTo(1)) Then
                            If mappedFrom.BinarySearch(mapTo(1)) = -1 Then
                                args.IsValid = True
                                mappedFrom.Add(mapTo(0))
                            Else
                                args.IsValid = False
                                Exit Sub
                            End If
                        Else
                            args.IsValid = False
                            Exit Sub
                        End If
                    Else
                        args.IsValid = False
                        Exit Sub
                    End If
                End If
            Next
        Else
            args.IsValid = True
        End If
    End Sub



    Private Sub valMapDomains_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valMapDomains.ServerValidate
        Dim tempMapDomains As String() = args.Value.Split(";"c)
        args.IsValid = False
        If tempMapDomains.Length <> 0 Then
            For i As Integer = 0 To tempMapDomains.Length - 1
                If tempMapDomains(i) <> "" Then
                    Dim mapTo As String() = tempMapDomains(i).Split(":"c)
                    If mapTo.Length = 2 Then
                        If testDomain(mapTo(0)) And testLocale(mapTo(1)) Then
                            args.IsValid = True
                        Else
                            args.IsValid = False
                            Exit Sub
                        End If
                    Else
                        args.IsValid = False
                        Exit Sub
                    End If
                End If
            Next
        Else
            args.IsValid = True
        End If

    End Sub
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' helper sub for btnGenerateNow_Click
    ''' </summary>
    ''' <param name="xmldoc"></param>
    ''' <param name="parentElement"></param>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub AddSettingToXML(ByVal xmldoc As XmlDocument, ByRef parentElement As XmlElement, ByVal name As String, ByVal value As String)
        Try
            Dim SettingElem As XmlElement = xmldoc.CreateElement("Setting")
            Dim nameElem As XmlElement = xmldoc.CreateElement("Name")
            nameElem.InnerText = name
            Dim valueElem As XmlElement = xmldoc.CreateElement("Value")
            valueElem.InnerText = value
            SettingElem.AppendChild(nameElem)
            SettingElem.AppendChild(valueElem)
            parentElement.AppendChild(SettingElem)
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' used to generate attributes for ascx or skin.xml files
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub btnGenerateNow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateNow.Click
        Try

            Select Case rblGenerationType.SelectedValue
                Case "ascx"

                    Dim tempString As String = "<dnn:MLLANGUAGESELECTOR runat=""server"" id=""dnnMLLANGUAGESELECTOR"" "
                    If ddlDisplayTypes.SelectedValue <> "NativeLanguageName" Then
                        tempString += String.Format("displayType=""{0}"" ", ddlDisplayTypes.SelectedValue)
                    End If
                    If txtSeparator.Text.Trim <> "" Then
                        tempString += String.Format("Separator=""{0}"" ", txtSeparator.Text.Trim)
                    End If
                    If txtCssClass.Text.Trim.ToLower <> "skinobject" Then
                        tempString += String.Format("CssClass=""{0}"" ", txtCssClass.Text.Trim)
                    End If
                    If cbHideCurrent.Checked = True Then
                        tempString += "HideCurrent=""True"" "
                    End If
                    If ddlAlignment.SelectedValue <> "Horizontal" Then
                        tempString += String.Format("Alignment=""{0}"" ", ddlAlignment.SelectedValue)
                    End If
                    If cbHyperlinks.Checked = True Then
                        tempString += "Hyperlinks=""True"" "
                    End If
                    If cbFlags.Checked = True Then
                        tempString += "Flags=""True"" "
                    End If
                    If cbMenu.Checked = False Then
                        tempString += "Menu=""False"" "
                    End If
                    If ddlMenuFlagPosition.SelectedValue <> "None" Then
                        tempString += String.Format("menuFlagPosition=""{0}"" ", ddlMenuFlagPosition.SelectedValue)
                    End If
                    If cbDisplayLabel.Checked = True Then
                        tempString += "DisplayLabel=""True"" "
                    End If
                    If txtLabelCSSClass.Text.Trim.ToLower <> "skinobject" Then
                        tempString += String.Format("LabelCSSClass=""{0}"" ", txtLabelCSSClass.Text.Trim)
                    End If
                    If cbOnlyLanguageCode.Checked = True Then
                        tempString += "OnlyLanguageCode=""True"" "
                    End If
                    If cbForceHidden.Checked = True Then
                        tempString += "ForceHidden=""True"" "
                    End If
                    If txtMapLanguages.Text.Trim <> "" Then
                        tempString += String.Format("MapLanguages=""{0}"" ", txtMapLanguages.Text.Trim)
                    End If

                    If cbUseStyleSheetForSkinObject.Checked = False Then
                        tempString += "UseStyleSheetForSkinobject=""False"" "
                    End If
                    If cbUseFullLocaleCode.Checked = False Then
                        tempString += "UseStyleSheetForSkinobject=""False"" "
                    End If
                    If cbTableLess.Checked = True Then
                        tempString += "tableLess=""True"" "
                    End If
                    If txtAltFlagLocation.Text.Trim <> "" Then
                        tempString += String.Format("altFlagLocation=""{0}"" ", txtAltFlagLocation.Text.Trim)
                    End If
                    If Not (txtaltImageType.Text.ToLower = "gif" Or txtaltImageType.Text.Trim = "") Then
                        tempString += String.Format("altImageType=""{0}"" ", txtaltImageType.Text.Trim)
                    End If
                    If txtMapDomains.Text.Trim <> "" Then
                        tempString += String.Format("MapDomains=""{0}"" ", txtMapDomains.Text.Trim)
                    End If
                    If drpFlagType.SelectedValue <> "Simple" Then
                        tempString += String.Format("FlagType=""{0} ", drpFlagType.SelectedValue)
                    End If

                    tempString += "/>"
                    txtSKOAttributes.Text = tempString.Trim
                Case "xml"
                    'xml
                    Dim myXML As New XmlDocument
                    Dim ObjectElem As XmlElement = myXML.CreateElement("Object")
                    Dim settingsElem As XmlElement = myXML.CreateElement("Settings")
                    Dim tokenElem As XmlElement = myXML.CreateElement("Token")
                    tokenElem.InnerText = "[MLLANGUAGESELECTOR]"
                    ObjectElem.AppendChild(tokenElem)

                    If ddlDisplayTypes.SelectedValue <> "NativeLanguageName" Then
                        AddSettingToXML(myXML, settingsElem, "DisplayType", ddlDisplayTypes.SelectedValue)
                    End If
                    If txtSeparator.Text.Trim <> "" Then
                        AddSettingToXML(myXML, settingsElem, "Separator", txtSeparator.Text.Trim)
                    End If
                    If txtCssClass.Text.Trim.ToLower <> "skinobject" Then
                        AddSettingToXML(myXML, settingsElem, "CssClass", txtCssClass.Text.Trim)
                    End If
                    If cbHideCurrent.Checked = False Then
                        AddSettingToXML(myXML, settingsElem, "HideCurrent", "False")
                    End If
                    If ddlAlignment.SelectedValue <> "Horizontal" Then
                        AddSettingToXML(myXML, settingsElem, "Alignment", ddlAlignment.SelectedValue)
                    End If
                    If cbHyperlinks.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "Hyperlinks", "True")
                    End If
                    If cbFlags.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "Flags", "True")
                    End If
                    If cbMenu.Checked = False Then
                        AddSettingToXML(myXML, settingsElem, "Menu", "False")
                    End If
                    If ddlMenuFlagPosition.SelectedValue <> "None" Then
                        AddSettingToXML(myXML, settingsElem, "menuFlagPosition", ddlMenuFlagPosition.SelectedValue)
                    End If
                    If cbDisplayLabel.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "DisplayLabel", "True")
                    End If
                    If txtLabelCSSClass.Text.Trim.ToLower <> "skinobject" Then
                        AddSettingToXML(myXML, settingsElem, "LabelCSSClass", txtLabelCSSClass.Text.Trim)
                    End If
                    If cbOnlyLanguageCode.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "OnlyLanguageCode", "True")
                    End If
                    If cbForceHidden.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "ForceHidden", "True")
                    End If
                    If txtMapLanguages.Text.Trim <> "" Then
                        AddSettingToXML(myXML, settingsElem, "MapLanguages", txtMapLanguages.Text.Trim)
                    End If
                    If txtMapDomains.Text.Trim <> "" Then
                        AddSettingToXML(myXML, settingsElem, "MapDomains", txtMapDomains.Text.Trim)
                    End If

                    If cbUseStyleSheetForSkinObject.Checked = False Then
                        AddSettingToXML(myXML, settingsElem, "UseStyleSheetForSkinobject", "False")
                    End If
                    If cbUseFullLocaleCode.Checked = False Then
                        AddSettingToXML(myXML, settingsElem, "UseStyleSheetForSkinobject", "False")
                    End If
                    If cbTableLess.Checked = True Then
                        AddSettingToXML(myXML, settingsElem, "tableLess", "True")
                    End If
                    If txtAltFlagLocation.Text.Trim <> "" Then
                        AddSettingToXML(myXML, settingsElem, "altFlagLocation", txtAltFlagLocation.Text.Trim)
                    End If
                    If Not (txtaltImageType.Text.ToLower = "gif" Or txtaltImageType.Text.Trim = "") Then
                        AddSettingToXML(myXML, settingsElem, "altImageType", txtaltImageType.Text.Trim)
                    End If
                    If drpFlagType.SelectedValue <> "Simple" Then
                        AddSettingToXML(myXML, settingsElem, "FlagType", drpFlagType.SelectedValue)
                    End If


                    ObjectElem.AppendChild(settingsElem)

                    txtSKOAttributes.Text = ObjectElem.OuterXml

                Case "html"
                    Dim tempString As String = "<object id=""dnnMLLANGUAGESELECTOR"" codetype=""dotnetnuke/server"" codebase=""MLLANGUAGESELECTOR"">"
                    If ddlDisplayTypes.SelectedValue <> "NativeLanguageName" Then
                        tempString += String.Format("<param name=""NativeLanguageName"" value=""{0}"" />", ddlDisplayTypes.SelectedValue)
                    End If
                    If txtSeparator.Text.Trim <> "" Then
                        tempString += String.Format("<param name=""Separator"" value=""{0}"" />", txtSeparator.Text.Trim)
                    End If
                    If txtCssClass.Text.Trim.ToLower <> "skinobject" Then
                        tempString += String.Format("<param name=""CssClass"" value=""{0}"" />", txtCssClass.Text.Trim)
                    End If
                    If cbHideCurrent.Checked = True Then
                        tempString += String.Format("<param name=""HideCurrent"" value=""True""  />", txtSeparator.Text.Trim)
                    End If
                    If ddlAlignment.SelectedValue <> "Horizontal" Then
                        tempString += String.Format("<param name=""Alignment"" value=""{0}"" />", ddlAlignment.SelectedValue)
                    End If
                    If cbHyperlinks.Checked = True Then
                        tempString += "<param name=""Hyperlinks"" value=""True"" />"
                    End If
                    If cbFlags.Checked = True Then
                        tempString += "<param name=""Flags"" value=""True"" />"
                    End If
                    If cbMenu.Checked = False Then
                        tempString += "<param name=""Menu"" value=""False"" />"
                    End If
                    If ddlMenuFlagPosition.SelectedValue <> "None" Then
                        tempString += String.Format("<param name=""menuFlagPosition"" value=""{0}"" />", ddlMenuFlagPosition.SelectedValue)
                    End If
                    If cbDisplayLabel.Checked = True Then
                        tempString += "<param name=""DisplayLabel"" value=""True"" />"
                    End If
                    If txtLabelCSSClass.Text.Trim.ToLower <> "skinobject" Then
                        tempString += String.Format("<param name=""LabelCSSClass"" value=""{0}"" />", txtLabelCSSClass.Text.Trim)
                    End If
                    If cbOnlyLanguageCode.Checked = True Then
                        tempString += "<param name=""OnlyLanguageCode"" value=""True"" />"
                    End If
                    If cbForceHidden.Checked = True Then
                        tempString += String.Format("<param name=""ForceHidden"" value=""True"" />", txtSeparator.Text.Trim)
                    End If
                    If txtMapLanguages.Text.Trim <> "" Then
                        tempString += String.Format("<param name=""MapLanguages"" value=""{0}"" />", txtMapLanguages.Text.Trim)
                    End If
                    If cbUseStyleSheetForSkinObject.Checked = False Then
                        tempString += "<param name=""UseStyleSheetForSkinobject"" value=""Flase"" />"
                    End If
                    If cbUseFullLocaleCode.Checked = False Then
                        tempString += "<param name=""UseStyleSheetForSkinobject"" value=""False"" />"
                    End If
                    If cbTableLess.Checked = True Then
                        tempString += "<param name=""tableLess"" value=""True"" />"
                    End If
                    If txtAltFlagLocation.Text.Trim <> "" Then
                        tempString += String.Format("<param name=""altFlagLocation"" value=""{0}"" />", txtAltFlagLocation.Text.Trim)
                    End If
                    If Not (txtaltImageType.Text.ToLower = "gif" Or txtaltImageType.Text.Trim = "") Then
                        tempString += String.Format("<param name=""altImageType"" value=""{0}"" />", txtaltImageType.Text.Trim)
                    End If
                    If txtMapDomains.Text.Trim <> "" Then
                        tempString += String.Format("<param name=""MapDomains"" value=""{0}"" />", txtMapDomains.Text.Trim)
                    End If
                    If drpFlagType.SelectedValue <> "Simple" Then
                        tempString += String.Format("<param name=""FlagType"" value=""{0}"" />", drpFlagType.SelectedValue)
                    End If

                    tempString += "</object>"
                    txtSKOAttributes.Text = tempString.Trim


            End Select
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try

    End Sub
#End Region

#Region "base overrides"

    Public Overrides Sub LoadSettings()
        Try
            If Not Page.IsPostBack Then
                ' Load settings from TabModuleSettings: specific to this instance

                loadDisplayTypes(loadSetting("displayType", "NativeLanguageName"))
                loadAlignment(loadSetting("Alignment", "Horizontal"))
                loadFlagPositioning(loadSetting("menuFlagPosition", "None"))
                loadFlagType(loadSetting("FlagType", "Simple"))
                txtSeparator.Text = loadSetting("Separator", "")
                txtCssClass.Text = loadSetting("CssClass", "skinobject")
                cbHideCurrent.Checked = Boolean.Parse(loadSetting("HideCurrent", "False"))
                cbHyperlinks.Checked = Boolean.Parse(loadSetting("Hyperlinks", "False"))
                cbFlags.Checked = Boolean.Parse(loadSetting("Flags", "True"))
                cbMenu.Checked = Boolean.Parse(loadSetting("Menu", "False"))
                cbDisplayLabel.Checked = Boolean.Parse(loadSetting("DisplayLabel", "False"))
                txtLabelCSSClass.Text = loadSetting("LabelCSSClass", "skinobject")
                cbOnlyLanguageCode.Checked = Boolean.Parse(loadSetting("OnlyLanguageCode", "False"))
                cbForceHidden.Checked = Boolean.Parse(loadSetting("ForceHidden", "False"))
                txtMapLanguages.Text = loadSetting("MapLanguages", "")
                txtMapDomains.Text = loadSetting("MapDomains", "")

                cbUseStyleSheetForSkinObject.Checked = Boolean.Parse(loadSetting("UseStyleSheetForSkinobject", "True"))
                cbUseFullLocaleCode.Checked = Boolean.Parse(loadSetting("useFullLocaleCode", "True"))
                cbTableLess.Checked = Boolean.Parse(loadSetting("tableLess", "True"))
                txtAltFlagLocation.Text = loadSetting("altFlagLocation", "")
                txtaltImageType.Text = loadSetting("altImageType", "gif")
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Public Overrides Sub UpdateSettings()
        Try
            If Page.IsValid Then
                updateSetting("displayType", ddlDisplayTypes.SelectedValue)
                updateSetting("Alignment", ddlAlignment.SelectedValue)
                updateSetting("menuFlagPosition", ddlMenuFlagPosition.SelectedValue)
                updateSetting("Separator", txtSeparator.Text.Trim)
                updateSetting("CssClass", txtCssClass.Text)
                updateSetting("HideCurrent", cbHideCurrent.Checked.ToString)
                updateSetting("Hyperlinks", cbHyperlinks.Checked.ToString)
                updateSetting("Flags", cbFlags.Checked.ToString)
                updateSetting("Menu", cbMenu.Checked.ToString)
                updateSetting("DisplayLabel", cbDisplayLabel.Checked.ToString)
                updateSetting("LabelCSSClass", txtLabelCSSClass.Text)
                updateSetting("OnlyLanguageCode", cbOnlyLanguageCode.Checked.ToString)
                updateSetting("ForceHidden", cbForceHidden.Checked.ToString)
                updateSetting("MapLanguages", txtMapLanguages.Text)
                updateSetting("MapDomains", txtMapDomains.Text)


                updateSetting("UseStyleSheetForSkinobject", cbUseStyleSheetForSkinObject.Checked.ToString)
                updateSetting("useFullLocaleCode", cbUseFullLocaleCode.Checked.ToString)
                updateSetting("tableLess", cbTableLess.Checked.ToString)
                updateSetting("altFlagLocation", txtAltFlagLocation.Text)
                updateSetting("altImageType", txtaltImageType.Text)
                updateSetting("FlagType", drpFlagType.SelectedValue)

                DataCache.RemoveCache(String.Format("MLLSLocaleCollection_{0}_0", PortalId))
                DataCache.RemoveCache(String.Format("MLLSLocaleCollection_{0}_1", PortalId))

                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), False)
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub
#End Region





End Class