Imports System.Globalization
Imports System.IO
Imports System.Net
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Services
Imports Google.Apis.Sheets.v4
Imports Google.Apis.Sheets.v4.Data
Imports Newtonsoft.Json.Linq

Partial Class WDC_Training_Inquiry_index
    Inherits System.Web.UI.Page

    Protected IPAddress As String

    Private Sub WDC_Training_Inquiry_index_Load(sender As Object, e As EventArgs) Handles Me.Load
        IPAddress = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        If Request.QueryString("Inquiry") Is Nothing OrElse Request.QueryString("Inquiry").Trim() = "" Then
            Me.View3Style.Visible = True
            Me.MultiView1.SetActiveView(Me.View3)
        ElseIf Me.MultiView1.ActiveViewIndex = 0 Then
            Me.Inquiry.Attributes.Add("required", "required")
            Me.Inquiry.Attributes.Add("placeholder", "Enter WDC Course or Subject of Inquiry")
            Me.Firstname.Attributes.Add("required", "required")
            Me.Firstname.Attributes.Add("placeholder", "Enter First Name")
            Me.Lastname.Attributes.Add("required", "required")
            Me.Lastname.Attributes.Add("placeholder", "Enter Last Name")
            Me.Phone.Attributes.Add("required", "required")
            Me.Phone.Attributes.Add("placeholder", "Enter Phone No.")
            Me.Email.Attributes.Add("required", "required")
            Me.Email.Attributes.Add("placeholder", "Enter Valid Email Address")
            Me.Comment.Attributes.Add("placeholder", "Enter Your Comment or Question")

            If Me.Inquiry.Text = "" Then
                Me.Inquiry.Text = Request.QueryString("Inquiry")
            End If
            If Me.Inquiry.Text = "" Then
                Me.Inquiry.Focus()
            End If
            'Page.ClientScript.RegisterOnSubmitStatement(Me.GetType, "OnSubmitScript", "handleSubmit()")
        End If

    End Sub

    Protected Sub SubmitForm(sender As Object, e As EventArgs) Handles Submit.Click
        If Not Me.ValidRecaptcha(Request.Form("g-recaptcha-response")) Then
            Me.ErrorText.Text = "Form Security 'I'm Not A Robot' Failed!"
        ElseIf Not Me.Consent.Checked Then
            Me.ErrorText.Text = "Consent Not Given To Submit the Form"
        Else
            Try
                Try
                    SendTheEmail()
                Catch
                    Throw
                End Try
                Try
                    UpdateTheDatabase()
                Catch
                    Throw
                End Try
                Me.MultiView1.ActiveViewIndex = 1
            Catch Ex As Exception
                Me.ErrorText.Text = Ex.Message
                Me.MultiView1.ActiveViewIndex = 0
            End Try
        End If

    End Sub

    Protected Sub Form1_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        If Me.ErrorText.Text <> "" Then
            Me.ErrorMsg.Visible = True
            Me.MultiView1.ActiveViewIndex = 0
        ElseIf Me.MultiView1.ActiveViewIndex = 1 Then
            Me.ConfirmFirstName.Text = Me.Firstname.Text
        End If
    End Sub


    Private Shared Sub SetEmailAddresses(ByVal addresses As String, ByVal emailCollection As MailAddressCollection)
        Dim emails As String() = addresses.Split(New Char() {","c, ";"c})
        emailCollection.Clear()
        For Each email As String In emails
            email = email.Trim()
            If email <> "" Then
                emailCollection.Add(New MailAddress(email))
            End If
        Next
    End Sub

    Protected Sub SendTheEmail()
        Try
            Dim Msg As String
            Msg = File.ReadAllText(Server.MapPath("~/emailtemplate.html"))
            Msg = Strings.Replace(Msg, "{Inquiry}", Me.Inquiry.Text.Trim(),,, CompareMethod.Text)
            Msg = Strings.Replace(Msg, "{FirstName}", StrConv(Me.Firstname.Text.Trim(), VbStrConv.ProperCase),,, CompareMethod.Text)
            Msg = Strings.Replace(Msg, "{LastName}", StrConv(Me.Lastname.Text.Trim(), VbStrConv.ProperCase),,, CompareMethod.Text)
            Msg = Strings.Replace(Msg, "{Phone}", StrConv(Me.Phone.Text.Trim(), VbStrConv.ProperCase),,, CompareMethod.Text)
            Msg = Strings.Replace(Msg, "{Email}", Me.Email.Text.Trim(),,, CompareMethod.Text)
            Msg = Strings.Replace(Msg, "{Comment}", Strings.Replace(Me.Comment.Text.Trim(), vbNewLine, "<br>",,, CompareMethod.Text),,, CompareMethod.Text)

            'Send Email to Submitter
            Using MailClient As New SmtpClient(), Email As New MailMessage
                MailClient.Credentials = New System.Net.NetworkCredential(EmailAccountName, EmailAccountPassword)
                Email.Priority = MailPriority.Normal
                Email.HeadersEncoding = Encoding.UTF8
                Email.SubjectEncoding = Encoding.UTF8
                Email.BodyEncoding = Encoding.UTF8
                Email.IsBodyHtml = True

                Email.From = New MailAddress(EmailAccountName)
                Email.To.Add(New MailAddress(Me.Email.Text))
                SetEmailAddresses(ToRecipient, Email.ReplyToList)

                Email.Subject = Me.Page.Title & " Submission"
                Email.Body = Msg
                Email.IsBodyHtml = True
                MailClient.Send(Email)
            End Using

            'Now Send to WDC people
            Using MailClient As New SmtpClient(), Email As New MailMessage
                MailClient.Credentials = New System.Net.NetworkCredential(EmailAccountName, EmailAccountPassword)
                Email.Priority = MailPriority.Normal
                Email.HeadersEncoding = Encoding.UTF8
                Email.SubjectEncoding = Encoding.UTF8
                Email.BodyEncoding = Encoding.UTF8
                Email.IsBodyHtml = True

                Email.From = New MailAddress(EmailAccountName)
                SetEmailAddresses(ToRecipient, Email.To)
                SetEmailAddresses(CCRecipient, Email.CC)
                SetEmailAddresses(BCRecipient, Email.Bcc)
                Email.ReplyToList.Add(New MailAddress(Me.Email.Text))
                Email.Subject = "WDC Training Inquiry / Course Waitlist Submission From " & Me.Lastname.Text & ", " & Me.Firstname.Text
                Email.Body = Msg
                Email.IsBodyHtml = True
                MailClient.Send(Email)
            End Using
        Catch ex As Exception
            Me.ErrorText.Text = "Error Sending Email: " & ex.Message
            Throw
        End Try
    End Sub

    Private Sub UpdateTheDatabase()
        Dim scopes As String() = {SheetsService.Scope.Spreadsheets}
        Const applicationName As String = "WDC Training Inquiry"
        Const spreadSheetID As String = "1mzMGTMqQaa07ha07SdfiQ6fUzWusaEqwneKvjJ9RcXc"
        Const sheet As String = "WDC Training Inquiry"

        Dim credential As GoogleCredential
        Using wc As New WebClient()
            Using Stream As Stream = wc.OpenRead(Server.MapPath("~/WDC Training Inquiry-d6cc038ed663.json"))
                credential = GoogleCredential.FromStream(Stream).CreateScoped(scopes)
            End Using
        End Using


        Using service As New SheetsService(New BaseClientService.Initializer() With {
            .HttpClientInitializer = credential,
            .ApplicationName = applicationName
            })
            Dim range As String = Strings.Replace("{sheet}!A:H", "{sheet}", sheet)
            Dim valueRange As New ValueRange()

            Dim oblist As New List(Of Object) From
                {Me.Inquiry.Text, Me.Firstname.Text, Me.Lastname.Text, Me.Phone.Text, Me.Email.Text, Me.Comment.Text, Now(), Me.IPAddress}
            valueRange.Values = New List(Of IList(Of Object)) From {oblist}

            Dim appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadSheetID, range)
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED
            Dim appendReponse = appendRequest.Execute()
        End Using
    End Sub


    Private Function ValidRecaptcha(ByVal ReCaptResp As String) As Boolean
        'Const siteKey As String = "6Lfze4kUAAAAAOaaae8UeCu_OY3hTy4IlULqXPkZ"
        Const secret As String = "6Lfze4kUAAAAANU1PAELoucZQekr7-Kfs-6R67o-"
        Const siteVerify As String = "https://www.google.com/recaptcha/api/siteverify"

        Dim request As WebRequest = WebRequest.Create(siteVerify)

        Dim postString As String = String.Format(CultureInfo.CurrentCulture, "secret={0}&response={1}&remoteip={2}", secret, ReCaptResp, IPAddress)
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = postString.Length

        Using dataStream As StreamWriter = New StreamWriter(request.GetRequestStream())
            dataStream.Write(postString, 0, postString.Length)
        End Using

        Using response As WebResponse = request.GetResponse()
            Using reader As New StreamReader(response.GetResponseStream())
                Dim jResp As JObject = JObject.Parse(reader.ReadToEnd())
                Dim success As Boolean = jResp.Value(Of Boolean)("success")
                Dim score As Double = jResp.Value(Of Double)("score")
                Dim action As String = jResp.Value(Of String)("action")
                Return success AndAlso action = "WDCInquiryForm" AndAlso score >= 0.8
            End Using
        End Using

    End Function

    Protected ReadOnly Property ToRecipient As String
        Get
            Return CStr(ConfigurationManager.AppSettings("ToRecipient"))
        End Get
    End Property

    Protected ReadOnly Property CCRecipient As String
        Get
            Return CStr(ConfigurationManager.AppSettings("CCRecipient"))
        End Get
    End Property

    Protected ReadOnly Property BCRecipient As String
        Get
            Return CStr(ConfigurationManager.AppSettings("BCRecipient"))
        End Get
    End Property

    Protected ReadOnly Property EmailAccountName As String
        Get
            Return CStr(ConfigurationManager.AppSettings("Email Account Name"))
        End Get
    End Property

    Protected ReadOnly Property EmailAccountPassword As String
        Get
            Return CStr(ConfigurationManager.AppSettings("Email Account Password"))
        End Get
    End Property

    Protected Function XORDecryption(ByVal dataIn As String) As String
        Return XORDecryption(dataIn, "xyzzy")
    End Function

    Protected Function XORDecryption(ByVal dataIn As String, ByVal codeKey As String) As String
        Dim lonDataPtr As Integer
        Dim strDataOut As String = ""
        Dim intXOrValue1, intXOrValue2 As Integer
        For lonDataPtr = 1 To CType(Len(dataIn) / 2, Integer)
            'The first value to be XOr-ed comes from the data to be encrypted
            intXOrValue1 = CType(Val("&H0" & Mid(dataIn, lonDataPtr * 2 - 1, 2)), Integer)
            'The second value comes from the code key
            intXOrValue2 = Asc(Mid(codeKey, (lonDataPtr Mod Len(codeKey)) + 1, 1))
            strDataOut &= Chr(intXOrValue1 Xor intXOrValue2)
        Next lonDataPtr
        Return strDataOut
    End Function
End Class
