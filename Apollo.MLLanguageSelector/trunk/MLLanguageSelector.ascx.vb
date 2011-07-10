'
' copyright (c) 2005-2009 by Erik van Ballegoij ( erik@apollo-software.nl ) ( http://www.apollo-software.nl )
'
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports Apollo.DNN.SkinObjects.MLLanguageSelector.Components
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Host


Partial Public Class MLLanguageSelector
    Inherits Entities.Modules.PortalModuleBase

    Const MyFileName As String = "MLLanguageSelector.ascx"


#Region "Private Members"
    Private _displayType As displayTypes = displayTypes.NativeLanguageName
    Private _menuFlagPosition As menuFlagPositions = menuFlagPositions.None
    Private _altFlagLocation As String = ""
    Private _altImageType As String = "gif"
    Private mappedLanguages As String()
    Private localSupportedLanguages As LocaleCollection
    Private isAdminUser As Boolean
    Private _localResourceFile As String
    Private _tabModuleSettings As Hashtable
#End Region


#Region "Public Members"

    Private ReadOnly Property mappedDomains() As String()
        Get
            If MapDomains <> "" AndAlso MapDomains.Contains(";") Then
                Return MapDomains.Split(";"c)
            Else
                Return Nothing
            End If
        End Get
    End Property


    Public Property FlagType() As String = "simple"
    Public Property useFullLocaleCode() As Boolean = True

    Public Property tableLess() As Boolean = True

    Public Property altFlagLocation() As String
        Get
            Dim retVal As String = _altFlagLocation.Replace("\", "/")
            If (Not retVal.EndsWith("/")) And (Not retVal = "") Then
                retVal += "/"
            End If
            If (retVal.StartsWith("/")) Then
                retVal = retVal.Remove(0, 1)
            End If
            Return retVal
        End Get
        Set(ByVal Value As String)
            _altFlagLocation = Value
        End Set
    End Property

    Public Property altImageType() As String
        Get
            Dim retVal As String = _altImageType.Replace(".", "")
            Return retVal
        End Get
        Set(ByVal Value As String)
            _altImageType = Value
        End Set
    End Property

    Public Property LabelCssClass() As String = "skinobject"

    Public Property Label() As String = ""

    Public Property displayLabel() As Boolean

    Public Property menu() As Boolean = False

    Public Property Flags() As Boolean = True

    Public Property Hyperlinks() As Boolean

    Public Property Separator() As String = ""

    Public Property HideCurrent() As Boolean = False

    Public Property CssClass() As String = "skinobject"

    Public Property DisplayType() As String
        Get
            ' Return displayTypeToString(_displayType)
            Return [Enum].GetName(GetType(displayTypes), _displayType)
        End Get
        Set(ByVal Value As String)
            '  _displayType = displayTypeFromString(Value)
            Try
                _displayType = CType([Enum].Parse(GetType(displayTypes), Value), displayTypes)
            Catch
                _displayType = displayTypes.DisplayName
            End Try
        End Set
    End Property

    Public Property menuflagPosition() As String
        Get
            Return [Enum].GetName(GetType(menuFlagPositions), _menuFlagPosition)
        End Get
        Set(ByVal Value As String)
            Try
                _menuFlagPosition = CType([Enum].Parse(GetType(menuFlagPositions), Value), menuFlagPositions)
            Catch
                _menuFlagPosition = menuFlagPositions.None
            End Try
        End Set
    End Property

    Public Property Alignment() As String = "Horizontal"

    Public Property OnlyLanguageCode() As Boolean

    Public ReadOnly Property TabModuleSettings() As Hashtable
        Get
            If _tabModuleSettings Is Nothing Then

                'Get ModuleSettings
                _tabModuleSettings = (New ModuleController).GetTabModuleSettings(TabModuleId)
            End If

            Return _tabModuleSettings
        End Get
    End Property

    Public Property MapLanguages() As String = ""

    Public Property MapDomains() As String = ""

    Public Property ForceHidden() As Boolean
    Public Property UseStyleSheetForSkinobject() As Boolean = True

    Public Shadows Property LocalResourceFile() As String

        Get
            Dim fileRoot As String

            If _localResourceFile = "" Then
                fileRoot = String.Format("{0}/{1}/{2}", TemplateSourceDirectory, Services.Localization.Localization.LocalResourceDirectory, MyFileName)
            Else
                fileRoot = _localResourceFile
            End If
            Return fileRoot
        End Get
        Set(ByVal Value As String)
            _localResourceFile = Value
        End Set

    End Property

#End Region

#Region "public shared members"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' converts string to displaytype, conversion is case insensitive
    ''' </summary>
    ''' <param name="strDisplayType">Input value for conversion</param>
    ''' <returns>Displaytype corresponding with string value passed, default value is displayTypes.Displayname</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' 	[erik]	30-4-2005	added comment
    ''' 	[erik]	30-4-2005	enhanced, making use of enum members now ...
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function displayTypeFromString(ByVal strDisplayType As String) As displayTypes
        Dim returnValue As displayTypes = displayTypes.DisplayName
        Try
            returnValue = CType([Enum].Parse(GetType(displayTypes), strDisplayType, True), displayTypes)
        Catch
        End Try
        Return returnValue
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' converts displaytype to string
    ''' </summary>
    ''' <param name="displayType">inputvalue for conversion</param>
    ''' <returns>Name of displaytype as string, default value is "DisplayName"</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  30-4-2005   added comments
    '''     [erik]  30-4-2005   enhanced, making use of enum members now
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Shared Function displayTypeToString(ByVal displayType As displayTypes) As String
        Dim returnValue As String = Nothing
        Try
            returnValue = [Enum].GetName(GetType(displayTypes), displayType)
        Catch
        End Try
        If returnValue Is Nothing Then
            returnValue = [Enum].GetName(GetType(displayTypes), displayTypes.DisplayName)
        End If
        Return returnValue
    End Function

#End Region

#Region "private methods"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' tests whether locale key can be used to create a cultureinfo object
    ''' </summary>
    ''' <param name="testme"></param>
    ''' <returns>true or false</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Function testLocale(ByVal testme As String) As Boolean
        Dim tempresult As Boolean = False
        Dim tempCI As CultureInfo = Nothing
        Try
            tempCI = New CultureInfo(testme)
        Catch
        End Try
        If Not (tempCI Is Nothing) Then
            tempresult = True
        End If
        Return tempresult

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' creates a (cachable) locale collection of locales used to diplay the selector
    ''' caching is done based on moduleid for modules and skin+clientid for skinobjects
    ''' admininistrators always see all languages, therefore, isadmin is added to the cachekey
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function getLocalesToDisplay() As LocaleCollection

        'create absolutely unique cachekey for this instance. For SKO clientid is used, for module the tabmoduleid
        Dim cacheKey As String = "MLLSLocaleCollection_{0}_{1}_{2}"
        If ModuleId = -1 Then
            cacheKey = String.Format(cacheKey, PortalId.ToString, isAdminUser.ToString, ClientID)
        Else
            cacheKey = String.Format(cacheKey, PortalId.ToString, isAdminUser.ToString, TabModuleId)
        End If

        Dim obj As Object = Nothing
        'obj = DataCache.GetCache(cacheKey)
        If obj Is Nothing Then
            Dim tempLanguages As LocaleCollection = Localization.GetEnabledLocales

            ' only remove locales if user <> administrator/superuser
            If Not isAdminUser Then
                If Not (mappedLanguages Is Nothing) Then
                    If mappedLanguages.Length > 0 Then
                        For i As Integer = 0 To mappedLanguages.Length - 1
                            If mappedLanguages(i) <> "" Then
                                Dim mapTo As String() = mappedLanguages(i).Split(":"c)
                                If mapTo.Length > 0 Then
                                    tempLanguages.Remove(mapTo(i))
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            obj = tempLanguages
            If Host.PerformanceSetting <> DotNetNuke.Common.Globals.PerformanceSettings.NoCaching Then
                DataCache.SetCache(cacheKey, obj)
            End If
        End If

        Return CType(obj, LocaleCollection)
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' This is a helperfunction that strips the region from the locale, ie "English-United States" becomes "English"
    ''' This function is used for displaying names of languages withoug region information
    ''' </summary>
    ''' <param name="ci">CultureInfo object</param>
    ''' <param name="displayType">DisplayTypes enum</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  28-9-2005   added textinfo.totitlecase, to correct for capitalization
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Shared Function stripRegionFromLocale(ByVal ci As CultureInfo, ByVal displayType As displayTypes) As String
        Dim nameToStrip As String = ""
        Select Case displayType
            Case displayTypes.NativeLanguageName
                nameToStrip = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.NativeName)
            Case displayTypes.EnglishLanguageName
                nameToStrip = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.EnglishName)
            Case Else
                nameToStrip = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.DisplayName)
        End Select
        Return Regex.Replace(nameToStrip, "\(((?!-->).)*\)", "")
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Creates datasource for datalist used to display the hyperlinks and flags
    ''' </summary>
    ''' <param name="displayType">DisplayTypes Enum</param>
    ''' <returns>system.data.dataview</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  28-9-2005   added textinfo.totitlecase, to correct for capitalization
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function CreateDataSource(ByVal displayType As displayTypes) As ICollection
        Try
            Using dt As New DataTable()
                Dim dr As DataRow
                dt.Columns.Add(New DataColumn("code", GetType(String)))
                dt.Columns.Add(New DataColumn("name", GetType(String)))
                Dim i As Integer
                For i = 0 To localSupportedLanguages.Count - 1
                    Dim info As CultureInfo = CultureInfo.CreateSpecificCulture(CType(localSupportedLanguages(i).Value, Locale).Code)
                    If Not (HideCurrent AndAlso info.Name = Threading.Thread.CurrentThread.CurrentUICulture.Name) Then
                        dr = dt.NewRow()
                        dr(0) = CType(localSupportedLanguages(i).Value, Locale).Code
                        Select Case displayType
                            Case displayTypes.EnglishName
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName)
                            Case displayTypes.Lcid
                                dr(1) = info.LCID.ToString()
                            Case displayTypes.Name
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name)
                            Case displayTypes.NativeName
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName)
                            Case displayTypes.TwoLetterIsoCode
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToUpper(info.TwoLetterISOLanguageName)
                            Case displayTypes.ThreeLetterIsoCode
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToUpper(info.ThreeLetterISOLanguageName)
                            Case displayTypes.NativeLanguageName, displayTypes.EnglishLanguageName
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stripRegionFromLocale(info, displayType))
                            Case displayTypes.ResourceName
                                dr(1) = Localization.GetString(info.Name + ".Text", LocalResourceFile)
                            Case Else
                                dr(1) = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName)
                        End Select
                        dt.Rows.Add(dr)
                    End If
                Next
                Return New DataView(dt)
            End Using
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
        Return Nothing
    End Function 'CreateDataSource


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Fills the CultureDropDownlist with all languages ... 
    ''' this is a modified version of the core routine, found in localization.vb,
    ''' to allow for the extra two defined displaytypes
    ''' </summary>
    ''' <param name="list">DropDownList</param>
    ''' <param name="displayType">displayTypes</param>
    ''' <param name="selectedValue">String</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  28-9-2005   added textinfo.totitlecase, to correct for capitalization
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub LoadCultureDropDownList(ByVal list As DropDownList, ByVal displayType As displayTypes, ByVal selectedValue As String)
        Try
            ' If the drop down list already has items, clear the list
            If list.Items.Count > 0 Then
                list.Items.Clear()
            End If

            Dim i As Integer
            For i = 0 To localSupportedLanguages.Count - 1

                ' Create a CultureInfo class based on culture
                Dim info As CultureInfo = CultureInfo.CreateSpecificCulture(CType(localSupportedLanguages(i).Value, Locale).Code)
                ' Create and initialize a new ListItem
                Dim item As New ListItem() With {.Value = CType(localSupportedLanguages(i).Value, Locale).Code}
                ' Based on the display type desired by the user, select the correct property
                Select Case displayType
                    Case displayTypes.EnglishName
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName)
                    Case displayTypes.Lcid
                        item.Text = info.LCID.ToString()
                    Case displayTypes.Name
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name)
                    Case displayTypes.NativeName
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName)
                    Case displayTypes.TwoLetterIsoCode
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToUpper(info.TwoLetterISOLanguageName)
                    Case displayTypes.ThreeLetterIsoCode
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToUpper(info.ThreeLetterISOLanguageName)
                    Case displayTypes.NativeLanguageName, displayTypes.EnglishLanguageName
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stripRegionFromLocale(info, displayType))
                    Case displayTypes.ResourceName
                        item.Text = Localization.GetString(info.Name + ".Text", LocalResourceFile)
                    Case Else
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.DisplayName)
                End Select
                list.Items.Add(item)
            Next i

            ' select the default item
            If Not selectedValue Is Nothing Then
                Dim item As ListItem = list.Items.FindByValue(selectedValue)
                If Not item Is Nothing Then
                    list.SelectedIndex = -1
                    item.Selected = True
                End If
            End If

        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub    'LoadCultureDropDownList



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' generic function to load specific setting from the database. Settings are load as 
    ''' tabmodulesettings
    ''' </summary>
    ''' <param name="settingName">String</param>
    ''' <param name="defaultValue">String</param>
    ''' <returns>string</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function loadSetting(ByVal settingName As String, ByVal defaultValue As String) As String
        If CType(TabModuleSettings(settingName), String) = "" Then
            Return defaultValue
        Else
            Return CType(TabModuleSettings(settingName), String)
        End If
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' method for loading all settings
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub LoadSettings()
        Try
            ' Load settings from TabModuleSettings: specific to this instance

            If Not ModuleId = -1 Then
                DisplayType = loadSetting("displayType", DisplayType)
                Separator = loadSetting("Separator", Separator)
                CssClass = loadSetting("CssClass", CssClass)
                HideCurrent = Boolean.Parse(loadSetting("HideCurrent", HideCurrent.ToString))
                Hyperlinks = Boolean.Parse(loadSetting("Hyperlinks", Hyperlinks.ToString))
                Flags = Boolean.Parse(loadSetting("Flags", Flags.ToString))
                menu = Boolean.Parse(loadSetting("Menu", menu.ToString))
                displayLabel = Boolean.Parse(loadSetting("DisplayLabel", displayLabel.ToString))
                LabelCssClass = loadSetting("LabelCSSClass", LabelCssClass)
                Alignment = loadSetting("Alignment", Alignment)
                OnlyLanguageCode = Boolean.Parse(loadSetting("OnlyLanguageCode", OnlyLanguageCode.ToString))
                ForceHidden = Boolean.Parse(loadSetting("ForceHidden", ForceHidden.ToString))
                MapLanguages = loadSetting("MapLanguages", "")
                MapDomains = loadSetting("MapDomains", "")
                menuflagPosition = loadSetting("menuFlagPosition", "None")
                UseStyleSheetForSkinobject = Boolean.Parse(loadSetting("UseStyleSheetForSkinobject", UseStyleSheetForSkinobject.ToString))
                useFullLocaleCode = Boolean.Parse(loadSetting("useFullLocaleCode", useFullLocaleCode.ToString))
                tableLess = Boolean.Parse(loadSetting("tableLess", tableLess.ToString))
                altFlagLocation = loadSetting("altFlagLocation", "")
                altImageType = loadSetting("altImageType", "gif")
                FlagType = loadSetting("FlagType", "Simple")
            End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' handles loading of module.css when used as skinobject (for skinobjects, module.css is never loaded)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub addStyleSheet()

        ' initialize reference paths to load the cascading style sheets
        Dim objCSS As Control = Page.FindControl("CSS")
        If objCSS Is Nothing Then
            'fix for dnn 4.3.x and up
            objCSS = Page.FindControl("Head")
        End If
        Dim objLink As HtmlGenericControl
        Dim ID As String

        Dim objCSSCache As Hashtable = CType(DataCache.GetCache("CSS"), Hashtable)
        If objCSSCache Is Nothing Then
            objCSSCache = New Hashtable
        End If

        If Not objCSS Is Nothing Then
            ID = CreateValidID(TemplateSourceDirectory)
            If objCSSCache.ContainsKey(ID) = False Then
                If File.Exists(Server.MapPath(TemplateSourceDirectory) & "/module.css") Then
                    objCSSCache(ID) = TemplateSourceDirectory & "/module.css"
                Else
                    objCSSCache(ID) = ""
                End If
                If Not Host.PerformanceSetting = DotNetNuke.Common.Globals.PerformanceSettings.NoCaching Then
                    DataCache.SetCache("CSS", objCSSCache)
                End If
            End If
            If objCSSCache(ID).ToString <> "" Then
                If objCSS.FindControl(ID) Is Nothing Then
                    objLink = New HtmlGenericControl("link")
                    objLink.ID = ID
                    objLink.Attributes("rel") = "stylesheet"
                    objLink.Attributes("type") = "text/css"
                    objLink.Attributes("href") = objCSSCache(ID).ToString
                    objCSS.Controls.Add(objLink)
                End If
            End If

        End If

    End Sub

    Private Sub permRedirect(ByVal newURL As String)
        Response.Clear()
        Response.Status = "301 Moved Permanently"
        Response.AddHeader("Location", newURL)
        Response.End()
    End Sub


#End Region

#Region "public methods"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' this function is called when the datalist is databinding, and calculates the 
    ''' url to the flag image
    ''' </summary>
    ''' <param name="cultureCode">string</param>
    ''' <returns>string</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetImgURL(ByVal cultureCode As String) As String

        Dim countryCode As String
        If Not useFullLocaleCode Then
            countryCode = ""
            Dim r As New Regex("(?<=-)\w+")
            Dim m As Match = r.Match(cultureCode.Trim)
            If m.Success Then
                Try
                    countryCode = m.Captures(0).ToString
                Catch
                End Try
            End If
        Else
            countryCode = cultureCode
        End If

        If Not IO.Directory.Exists(Server.MapPath(String.Format("{0}/flags/{1}", TemplateSourceDirectory, FlagType))) Then
            FlagType = "simple"
        End If
        If FlagType.ToLowerInvariant = "modern" Then
            altImageType = "png"
        End If
        Dim flagLocation As String
        If altFlagLocation <> "" Then
            flagLocation = String.Format("{0}{1}", PortalSettings.HomeDirectory, altFlagLocation)
        Else
            flagLocation = String.Format("{0}/flags/{1}/", TemplateSourceDirectory, FlagType)
        End If
        Dim imgURL As String = "~/spacer.gif"
        If countryCode <> "" Then
            imgURL = String.Format("{0}{1}.{2}", flagLocation, countryCode, altImageType)
        End If
        Return ResolveUrl(imgURL)

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' GetParams strips the tabid, ctlkey and previous language from current querystring parameters,
    ''' and adds new language param
    ''' </summary>
    ''' <returns>string()</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Function getParams(ByVal newLanguage As String) As String()
        Dim returnValue As String = ""
        Dim coll As System.Collections.Specialized.NameValueCollection = Request.QueryString
        Dim arrValues() As String
        Dim arrKeys As String() = coll.AllKeys
        For i As Integer = 0 To arrKeys.GetUpperBound(0)
            If Not arrKeys(i) Is Nothing Then
                Select Case arrKeys(i).ToLower
                    Case "tabid", "ctl", "language"
                        'skip parameter
                    Case Else
                        If (arrKeys(i).ToLower = "portalid") And PortalSettings.ActiveTab.IsSuperTab Then
                            'skip parameter
                            'navigateURL adds portalid to querystring if tab is superTab
                        Else
                            arrValues = coll.GetValues(i)
                            For j As Integer = 0 To arrValues.GetUpperBound(0)
                                If returnValue <> "" Then returnValue += "&"
                                returnValue += String.Format("{0}={1}", arrKeys(i), Server.HtmlEncode(arrValues(j)))
                            Next
                        End If
                End Select
            End If
        Next
        If Localization.GetEnabledLocales.Count > 1 AndAlso (PortalSettings.EnableUrlLanguage = False) Then
            'because useLanguageInUrl is false, navigateUrl won't add a language param, so we need to do that ourselves
            If returnValue <> "" Then returnValue += "&"
            returnValue += "language=" + newLanguage
        End If

        'return the new querystring as a string array
        Return returnValue.Split("&"c)
    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' this function calculates the url to call when clicking on a flag or hyperlink
    ''' in essence, the "language" parameter is added to the url
    ''' Friendly URL's are taken into account, a friendly url is generated if the portal
    ''' is using friendly url's
    ''' </summary>
    ''' <param name="cultureCode"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' 	[erik]	30-4-2005	changed to use navigateURL. The parameters passed to 
    '''                         navigateURL are calculated with a new function 
    '''                         getParams. GetParams strips the tabid, ctlkey and 
    '''                         previous language from current querystring parameters,
    '''                         and adds new language param
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetNavURL(ByVal cultureCode As String) As String
        Dim returnValue As String = ""
        Dim languagecode As String
        If OnlyLanguageCode Then
            languagecode = Left(cultureCode, InStr(cultureCode, "-") - 1)
        Else
            languagecode = cultureCode
        End If

        returnValue = NavigateURL(PortalSettings.ActiveTab.TabID, PortalSettings.ActiveTab.IsSuperTab, PortalSettings, Request.QueryString("ctl"), languagecode, getParams(languagecode))

        If mappedDomains IsNot Nothing Then
            If mappedDomains.Length > 0 Then
                For Each mapping As String In mappedDomains
                    If mapping <> "" Then
                        Dim mapTo As String() = mapping.Split(":"c)
                        If mapTo.Length > 0 Then
                            If testLocale(mapTo(1)) AndAlso mapTo(1) = cultureCode Then
                                returnValue = returnValue.Replace(PortalAlias.HTTPAlias, mapTo(0))
                                Exit For
                            End If
                        End If
                    End If
                Next
            End If
        End If
        Return returnValue
    End Function

    Public Shared Function getStartClass(ByVal cultureCode As String) As String
        If cultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name Then
            Return "MLLanguageSelectionFlag_Active"
        Else
            Return "MLLanguageSelectionFlag_Inactive"
        End If
    End Function

    Public Shared Function getMouseOver(ByVal cultureCode As String) As String
        If cultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name Then
            Return ""
        Else
            Return "this.className='MLLanguageSelectionFlag_Active'"
        End If
    End Function

    Public Shared Function getMouseOut(ByVal cultureCode As String) As String
        If cultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name Then
            Return ""
        Else
            Return "this.className='MLLanguageSelectionFlag_Inactive'"
        End If
    End Function

#End Region

#Region "Event Handlers"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' the following things are done here:
    ''' * loadsettings
    ''' * decide if dropdownlist should be visible or not
    ''' * decide if label should be visible or not
    ''' * decide if flags and hyperlinks should be rendered
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  28-9-2005   changed due to different rendering (uses tables now)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'If Not Page.IsPostBack Then
            If tableLess Then
                dlLanguages.RepeatLayout = RepeatLayout.Flow
            End If
            ' make flags and hyperlinks
            If Flags Or Hyperlinks Then
                dlLanguages.DataSource = CreateDataSource(_displayType)
                dlLanguages.DataKeyField = "code"
                dlLanguages.CssClass = CssClass
                Select Case Alignment.ToLower
                    Case "vertical"
                        dlLanguages.RepeatDirection = RepeatDirection.Vertical
                    Case Else
                        dlLanguages.RepeatDirection = RepeatDirection.Horizontal
                End Select
                dlLanguages.DataBind()
                Select Case Alignment.ToLower
                    Case "vertical"
                        If tableLess Then
                            pre.Text = Localization.GetString("TableLess_Vertical_Pre", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            Me.mid.Visible = False
                            post.Text = Localization.GetString("TableLess_Vertical_Post", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                        Else
                            pre.Text = Localization.GetString("Tables_Vertical_Pre", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            Me.mid.Text = Localization.GetString("Tables_Vertical_Mid", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            post.Text = Localization.GetString("Tables_Vertical_Post", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                        End If
                    Case Else
                        If tableLess Then
                            pre.Text = Localization.GetString("TableLess_Horizontal_Pre", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            Me.mid.Visible = False
                            post.Text = Localization.GetString("TableLess_Horizontal_Post", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                        Else
                            pre.Text = Localization.GetString("Tables_Horizontal_Pre", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            Me.mid.Text = Localization.GetString("Tables_Horizontal_Mid", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                            post.Text = Localization.GetString("Tables_Horizontal_", LocalResourceFile, PortalSettings, CultureInfo.CurrentUICulture.Name, True)
                        End If
                End Select
            Else
                pre.Visible = False
                Me.mid.Visible = False
                post.Visible = False
            End If

            'make menu
            If tableLess Then
                MLLanguageMenuTable.Visible = False
                MLLanguageMenuDiv.Visible = False
                MLLanguageMenuLabelSpan.Visible = False
                MLLanguageMenuLeftFlagSpan.Visible = False
                MLLanguageMenuMenuSpan.Visible = False
                MLLanguageMenuRightFlagSpan.Visible = False
                If menu Or displayLabel Then
                    MLLanguageMenuDiv.Visible = True
                    MLLanguageMenuDiv.Attributes.Item("class") = String.Format("{0} {1}", MLLanguageMenuDiv.Attributes.Item("class"), CssClass)
                End If
                If Not Page.IsPostBack Then
                    If menu Then
                        LoadCultureDropDownList(cboSelectLanguageTL, _displayType, Threading.Thread.CurrentThread.CurrentUICulture.Name)
                        cboSelectLanguageTL.CssClass = CssClass
                        MLLanguageMenuMenuSpan.Visible = True
                        Dim imgURL As String = GetImgURL(Threading.Thread.CurrentThread.CurrentUICulture.Name)
                        If _menuFlagPosition = menuFlagPositions.Left Then
                            flagLeftImageTL.ImageUrl = imgURL
                            MLLanguageMenuLeftFlagSpan.Visible = True
                        ElseIf _menuFlagPosition = menuFlagPositions.Right Then
                            flagRIghtImageTL.ImageUrl = imgURL
                            MLLanguageMenuRightFlagSpan.Visible = True
                        End If
                    End If
                    If displayLabel Then
                        lblLabelTL.Text = Localization.GetString("Label.Text", LocalResourceFile)
                        MLLanguageMenuLabelSpan.Visible = True
                    Else
                        MLLanguageMenuLabelSpan.Visible = False
                    End If
                End If
            Else
                MLLanguageMenuDiv.Visible = False
                MLLanguageMenuTable.Visible = False
                MLLanguageMenuLeftFlagCell.Visible = False
                MLLanguageMenuRightFlagCell.Visible = False
                MLLanguageMenuMenuCell.Visible = False
                If menu Or displayLabel Then
                    MLLanguageMenuTable.Visible = True
                    MLLanguageMenuTable.Attributes.Item("class") = String.Format("{0} {1}", MLLanguageMenuTable.Attributes.Item("class"), CssClass)
                End If
                If Not Page.IsPostBack Then
                    If menu Then
                        LoadCultureDropDownList(cboSelectLanguage, _displayType, Threading.Thread.CurrentThread.CurrentUICulture.Name)
                        cboSelectLanguage.CssClass = CssClass
                        MLLanguageMenuMenuCell.Visible = True
                        Dim imgURL As String = GetImgURL(Threading.Thread.CurrentThread.CurrentUICulture.Name)
                        If _menuFlagPosition = menuFlagPositions.Left Then
                            flagLeftImage.ImageUrl = imgURL
                            MLLanguageMenuLeftFlagCell.Visible = True
                        ElseIf _menuFlagPosition = menuFlagPositions.Right Then
                            flagRIghtImage.ImageUrl = imgURL
                            MLLanguageMenuRightFlagCell.Visible = True
                        End If
                    End If

                    If displayLabel Then
                        lblLabel.Text = Localization.GetString("Label.Text", LocalResourceFile)
                        MLLanguageMenuLabelCell.Visible = True
                    Else
                        MLLanguageMenuLabelCell.Visible = False
                    End If
                End If

            End If


            'End If
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' handles the itemdatabound event of the languages datalist, used to display
    ''' flags and hyperlinks. This method puts the separator in place, and handles
    ''' the visibility of the flags and hyperlinks
    ''' </summary>
    ''' <param name="sender">DataList</param>
    ''' <param name="e">System.Web.UI.WebControls.DataListItemEventArgs</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    '''     [erik]  28-9-2005   changed due to changes in rendering (uses tables now)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub dlLanguages_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlLanguages.ItemDataBound
        Try
            Select Case e.Item.ItemType
                Case ListItemType.Separator
                    Dim strSeparator As String = ""
                    If Separator <> "" Then
                        If Separator.IndexOf("src=") <> -1 Then
                            strSeparator = Replace(Separator, "src=", "src=" & PortalSettings.ActiveTab.SkinPath)
                        Else
                            strSeparator = String.Format("<span class=""{0}"">{1}</span>", CssClass, Replace(Separator, " ", "&nbsp;"))
                        End If
                    End If

                    Dim lblSeperator As Label = CType(e.Item.FindControl("lblSeperator"), Label)
                    lblSeperator.Visible = False
                    If Hyperlinks Or Flags Then
                        lblSeperator.Text = strSeparator
                        lblSeperator.Visible = True
                    End If

                Case ListItemType.Item, ListItemType.AlternatingItem
                    Dim MLLanguageSelectionItemFlag As System.Web.UI.HtmlControls.HtmlTableCell = CType(e.Item.FindControl("MLLanguageSelectionItemFlag"), System.Web.UI.HtmlControls.HtmlTableCell)
                    Dim MLLanguageSelectionItemURL As System.Web.UI.HtmlControls.HtmlTableCell = CType(e.Item.FindControl("MLLanguageSelectionItemURL"), System.Web.UI.HtmlControls.HtmlTableCell)
                    Dim MLLanguageSelectionItemTable As System.Web.UI.HtmlControls.HtmlTable = CType(e.Item.FindControl("MLLanguageSelectionItemTable"), System.Web.UI.HtmlControls.HtmlTable)
                    MLLanguageSelectionItemFlag.Visible = False
                    MLLanguageSelectionItemURL.Visible = False
                    If Hyperlinks Then
                        MLLanguageSelectionItemURL.Visible = True
                    End If
                    If Flags Then
                        MLLanguageSelectionItemFlag.Visible = True
                    End If
                    MLLanguageSelectionItemTable.Attributes.Item("class") = String.Format("{0} {1}", MLLanguageSelectionItemTable.Attributes.Item("class"), CssClass)
            End Select
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' selection changed of language selector ... reload page
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	30-4-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub cboSelectLanguage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSelectLanguage.SelectedIndexChanged, cboSelectLanguageTL.SelectedIndexChanged
        ' Set the current culture
        Try

            'Redirect to same page to update all controls for newly selected culture
            Response.Redirect(GetNavURL(CType(sender, System.Web.UI.WebControls.DropDownList).SelectedValue), False)
        Catch exc As Exception
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub


#End Region




    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' added functionality for mapping languages
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[erik]	28-9-2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        If Not Page.IsPostBack Then
            LoadSettings()
        End If

        ' first we check if the requested language must be mapped
        isAdminUser = (DotNetNuke.Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()))
        If Not isAdminUser Then
            'administrators will not be mapped
            If MapLanguages <> "" Then
                'first split in separate mappings
                mappedLanguages = MapLanguages.Split(";"c)
                If (Not (mappedLanguages Is Nothing)) Then
                    If mappedLanguages.Length <> 0 Then
                        For i As Integer = 0 To mappedLanguages.Length - 1
                            If mappedLanguages(i) <> "" Then
                                Dim mapTo As String() = mappedLanguages(i).Split(":"c)
                                If mapTo.Length > 0 Then
                                    If mapTo(0).ToLower = CultureInfo.CurrentCulture.Name.ToLower Then
                                        If testLocale(mapTo(1)) Then
                                            Response.Redirect(GetNavURL(mapTo(1)), True)
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            If (Request.QueryString("language") = "") Then
                If MapDomains <> "" Then
                    'first split in separate mappings
                    If (Not (mappedDomains Is Nothing)) Then
                        For Each mapping As String In mappedDomains
                            If mapping <> "" Then
                                Dim mapTo As String() = mapping.Split(":"c)
                                If mapTo.Length > 0 Then
                                    If mapTo(0).ToLower = Request.ServerVariables("SERVER_NAME").ToLower Then
                                        If testLocale(mapTo(1)) Then
                                            Dim navUrl As String = GetNavURL(mapTo(1))
                                            permRedirect(navUrl)
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If

            End If
        End If


        localSupportedLanguages = getLocalesToDisplay()
        If localSupportedLanguages.Count = 1 AndAlso (CType(localSupportedLanguages(0).Key, String) = Threading.Thread.CurrentThread.CurrentUICulture.Name) AndAlso HideCurrent Then
            ForceHidden = True
            Hyperlinks = False
            Flags = False
            menu = False
            Label = ""
        End If
        If ForceHidden Then
            Hyperlinks = False
            Flags = False
            menu = False
            Label = ""
        Else
            If ModuleId = -1 Then
                'if used as skinobject, use the module stylesheet anyway...
                If UseStyleSheetForSkinobject Then
                    addStyleSheet()
                End If
            End If
        End If
    End Sub




End Class