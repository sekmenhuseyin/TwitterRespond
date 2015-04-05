Imports System.Web
Imports System.Threading

Public Class Form1
    Public komut As String = ""
    Public kontrol_komut As String = ""
    Public durma_istegi As Boolean = False
    Public url_base As String = "https://twitter.com/"
    Public url_search As String = "https://twitter.com/search?f=realtime&q="
    Public search_txt As String = ""
    Public cevap_txt As String = ""
    Public search_count As Integer = 0
    Public searchstop As Boolean = False
    Public sinirliarama As Boolean = False
    Public only_chck As Boolean = False
    Public ids As New ArrayList
    Public saniye As Integer = 1000
    Public oturum_twitsayisi As Integer = 0
    Public arama_limiti_dolu As Boolean = False
    Public contur_bilgisi_alama As Boolean = False
    Public counter As New ArrayList
    Public seri_arama As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WB_1.Dock = DockStyle.Fill
        txt_username.Text = My.Settings.str_username
        txt_password.Text = My.Settings.str_password
        WB_1.ScriptErrorsSuppressed = True
        WB_C.ScriptErrorsSuppressed = True
        'resimleri_goster("no")
        WB_1.Navigate(url_base)
        komut = "send_login"
        Dim langs As Array = {"Tüm Diller", "am|Amharca (አማርኛ)", "ar|Arapça (العربية)", "bg|Bulgarca (Български)", "bn|Bengal dili (বাংলা)", "bo|Tibetçe (བོད་སྐད)", "chr|Çerokice (ᏣᎳᎩ)", "da|Danca (Dansk)", "de|Almanca (Deutsch)", "dv|Maldivce (ދިވެހި)", "el|Yunanca (Ελληνικά)", "en|İngilizce (English)", "es|İspanyolca (Español)", "fa|Farsça (فارسی)", "fi|Fince (Suomi)", "fr|Fransızca (Français)", "gu|Gucaratça (ગુજરાતી)", "iw|İbranice (עברית)", "hi|Hintçe (हिंदी)", "hu|Macarca (Magyar)", "hy|Ermenice (Հայերեն)", "in|Endonezce (Bahasa Indonesia)", "is|İzlandaca (Íslenska)", "it|İtalyanca (Italiano)", "iu|Eskimo Dili (ᐃᓄᒃᑎᑐᑦ)", "ja|Japonca (日本語)", "ka|Gürcüce (ქართული)", "km|Kmerce (ខ្មែរ)", "kn|Kannada (ಕನ್ನಡ)", "ko|Korece (한국어)", "lo|Laoca (ລາວ)", "lt|Litvanca (Lietuvių)", "ml|Malayalam dili (മലയാളം)", "my|Myanmar (မြန်မာဘာသာ)", "ne|Nepali (नेपाली)", "nl|Flemenkçe (Nederlands)", "no|Norveççe (Norsk)", "or|Oriya dili (ଓଡ଼ିଆ)", "pa|Pencapça (ਪੰਜਾਬੀ)", "pl|Lehçe (Polski)", "pt|Portekizce (Português)", "ru|Rusça (Русский)", "si|Seylanca (සිංහල)", "sv|İsveççe (Svenska)", "ta|Tamilce (தமிழ்)", "te|Telugu dili (తెలుగు)", "th|Tayca (ไทย)", "tl|Tagalogca (Tagalog)", "tr|Türkçe (Türkçe)", "ur|Urduca (ﺍﺭﺩﻭ)", "vi|Vietnamca (Tiếng Việt)", "zh|Çince (中文)"}
        txt_lang.Items.Clear()
        For Each lang As String In langs
            txt_lang.Items.Add(lang)
        Next
        txt_lang.SelectedIndex = 0
        pb_error_control.Maximum = 3 * saniye / 10

    End Sub
    Private Sub btn_site_Click(sender As Object, e As EventArgs) Handles btn_site.Click
        WB_1.Visible = True
        DGW_List.Visible = False
    End Sub

    Private Sub btn_liste_Click(sender As Object, e As EventArgs) Handles btn_liste.Click
        WB_1.Visible = False
        DGW_List.Visible = True



    End Sub

    Private Sub chck_searchlimit_CheckedChanged(sender As Object, e As EventArgs) Handles chck_searchlimit.CheckedChanged
        sinirliarama = chck_searchlimit.Checked
        If chck_searchlimit.Checked = True Then txt_searchlimit.Enabled = True Else txt_searchlimit.Enabled = False
    End Sub
    Private Sub btn_login_Click(sender As Object, e As EventArgs) Handles btn_login.Click
        If send_login() = True Then
            My.Settings.str_username = txt_username.Text
            My.Settings.str_password = txt_password.Text
            My.Settings.Save()
            komut = "send_login"
            btn_login.Enabled = False : lnk_logout.Visible = True
        Else

        End If
    End Sub

    Private Sub btn_search_Click(sender As Object, e As EventArgs) Handles btn_search.Click
        search_count = 0
        searchstop = False
        search_txt = txt_q.Text
        cevap_txt = txt_reply.Text
        ids.Clear()
        arama_limiti_dolu = False
        seri_arama = True
        If search_txt = "" Or cevap_txt = "" Then
            MessageBox.Show("Arama ve cevap metinleri boş olamaz ", "Arama uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        For Each c As Control In FlowLayoutPanel1.Controls
            If c.Text = search_txt Then remove_rows(search_txt)
        Next
        add_searchlist()
        start_search()
        komut = "start_search"
    End Sub

    Private Sub Master_Start() Handles btn_start.Click
        Dim retval As Integer = MessageBox.Show("Twit cevaplama başlatılsın mı?.", "İşlem Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
        If Not retval = vbYes Then Exit Sub

        durma_istegi = False
        only_chck = False
        If DGW_List.Rows.Count < 1 Then
            MessageBox.Show("Cavaplanacak birşey yok ", "Geçersiz işlem sonucu", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        tmr_error_control.Enabled = True
        'goto_replyform()
        kontrol_komut = "cevap_kontrol"
        lbl_islem.Text = "Twitin daha önce cevaplandığı kontrol ediliyor"

        WB_C.Navigate(url_base & DGW_List.CurrentRow.Cells("kontrol").Value)
        Dim beklemesuresi As Integer = bekleme.Value
        If bekleme.Value < beklemeust.Value Then
            beklemesuresi = CInt(Int((beklemeust.Value * Rnd()) + bekleme.Value))
        Else
            beklemesuresi = CInt(Int((bekleme.Value * Rnd()) + beklemeust.Value))
        End If
        'Timer1.Interval = beklemesuresi * saniye
        Timer1.Enabled = True
    End Sub
#Region "Fonksiyonlar"

    Sub resimleri_goster(Optional durum = "yes")
        Dim regkey As Microsoft.Win32.RegistryKey = _
My.Computer.Registry.CurrentUser
        regkey.OpenSubKey("Software\Microsoft\Internet Explorer\Main", _
        True).SetValue("Display Inline Images", durum)

    End Sub
    Function send_login() As Boolean
        Try
            Dim islem As Integer = 0
            Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("input")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("name")
                If tmp_str = "session[username_or_email]" Then
                    el.SetAttribute("value", txt_username.Text)
                    islem = islem + 1
                End If
                If tmp_str = "session[password]" Then
                    el.SetAttribute("value", txt_password.Text)
                    islem = islem + 1
                End If

            Next
            els = WB_1.Document.GetElementsByTagName("form")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("action")
                If tmp_str = "https://twitter.com/sessions" Then
                    el.InvokeMember("submit")
                    islem = islem + 1
                End If
            Next
            If islem > 2 Then Return True Else Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    Function is_login() As Boolean
        Try
            Dim islem As Integer = 0
            Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("td")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("classname")
                If InStr(tmp_str, "me") < 3 And InStr(tmp_str, "me") > 0 Then
                    islem = islem + 1
                    btn_login.Enabled = False
                    Return True
                End If

            Next

            If IsNothing(WB_1.Document.GetElementById("q")) = False Then
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False

        End Try
    End Function

    Function get_profile() As Boolean
        Try
            Dim islem As Integer = 0
            'kullanıcı resmini bul
            Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("img")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("class")
                If tmp_str = "DashboardProfileCard-avatarImage js-action-profile-avatar" Then
                    If IsNothing(el.Children(0)) = False Then
                        img_avatar.Load(el.Children(0).GetAttribute("src"))
                    End If
                End If
            Next
            'kullanıcı adını bul
            els = WB_1.Document.GetElementsByTagName("span")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("class")
                If tmp_str = "u-linkComplex-target" Then
                    lbl_username.Text = el.Children(0).InnerText
                    Return True
                End If
            Next
            'tam adını bul
            els = WB_1.Document.GetElementsByTagName("a")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.GetAttribute("class")
                If tmp_str = "u-textInheritColor" Then
                    lbl_fullname.Text = el.Children(0).InnerText
                    Return True
                End If
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function start_search() As Boolean
        Try
            Dim s_text As String = txt_q.Text
            Dim s_first As String = "%20since%3A" & Format(txt_firstdate.Value, "yyyy-MM-dd")
            Dim s_last As String = "%20until%3A" & Format(txt_lastdate.Value.AddDays(1), "yyyy-MM-dd") & "&src=typd"
            Dim s_lang As String = ""
            If Not txt_lang.SelectedIndex = 0 Then
                s_lang = "%20lang%3A" & txt_lang.Text.Substring(0, InStr(txt_lang.Text, "|") - 1)

            End If
            WB_1.Navigate(url_search & HttpUtility.UrlEncode(s_text) & s_lang & s_first & s_last)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Function next_page() As Boolean
        WB_1.Navigate("javascript:window.scroll(0,document.body.scrollHeight);")
        Thread.Sleep(10)
        Application.DoEvents()
        get_frompage()
        If searchstop = True Then
            numaralandir()
            seri_arama = False
            MessageBox.Show("Arama sonlandırıldı. Bulunan twit sayısı: " & search_count, "Arama sonucu", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        Else
            next_page()
            Return True
        End If

        'Try
        '    Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("div")
        '    Dim islem As Boolean = False
        '    For Each el As HtmlElement In els
        '        If searchstop = True Then
        '            numaralandir()
        '            seri_arama = False
        '            MessageBox.Show("Arama sonlandırıldı. Bulunan twit sayısı: " & search_count, "Arama sonucu", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            Return False
        '        End If
        '        Dim tmp_str As String = el.GetAttribute("classname")
        '        If InStr(tmp_str, "w-button-more") > 0 Then
        '            el.Children(0).InvokeMember("click")
        '            komut = "start_search"
        '            islem = True
        '            Return True
        '        End If

        '    Next
        '    If islem = False Then
        '        numaralandir()
        '        seri_arama = False
        '        MessageBox.Show("Arama sona erdi. Bulunan twit sayısı: " & search_count, "Arama sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    End If
        '    Return False

        'Catch ex As Exception
        '    Return False

        'End Try
    End Function

    Function get_frompage() As Boolean
        'Try

        Dim islem As Integer = 0
        Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("div")
        For Each el As HtmlElement In els
            Dim tmp_str1 As String = el.Parent.TagName
            Dim tmp_href As String = ""
            Dim url_kontrol As String = ""
            Dim txt_content = ""
            Dim txt_user = ""
            Dim txt_date = ""
            Dim txt_retweet = ""
            Dim txt_favorite = ""
            If searchstop = True Then Return False

            If tmp_str1 = "LI" Then
                Dim tmp_str As String = el.GetAttribute("data-tweet-id")
                'MsgBox(tmp_str)
                If tmp_str <> "" Then
                    txt_user = el.GetAttribute("data-screen-name")
                    tmp_href = el.GetAttribute("data-permalink-path") : tmp_href = Replace(tmp_href, "/status/", "/reply/")
                    Try
                        txt_date = el.Children(1).Children(0).Children(1).Children(0).GetAttribute("title")
                        txt_content = el.Children(1).Children(1).InnerText
                    Catch ex As Exception
                    End Try
                    If sinirliarama = True Then
                        If search_count + 1 > txt_searchlimit.Value Then
                            arama_limiti_dolu = True
                            Return True
                        End If
                    End If
                    counter.Clear()
                    If chck_gelismis.Checked = True Then
                        contur_bilgisi_alama = False
                        kontrol_komut = "cevap_kontrol"
                        only_chck = True
                        WB_C.Navigate(url_base & url_kontrol)
                        Dim beklemesuresi As Integer = 0
                        Do While contur_bilgisi_alama = False
                            Thread.Sleep(10)
                            beklemesuresi = beklemesuresi + 1
                            If beklemesuresi > pb_error_control.Maximum Then Exit Do
                            Application.DoEvents()
                        Loop

                    End If
                    If counter.Count > 0 Then
                        txt_retweet = counter(0)
                        If counter.Count > 1 Then
                            txt_favorite = counter(1)
                        End If
                    End If

                    If chck_tektiwit.Checked = False Then
                        DGW_List.Rows.Add(True, "", search_txt, txt_date, "@" & txt_user, txt_content, tmp_href, cevap_txt, url_kontrol, txt_retweet, txt_favorite)
                        search_count = search_count + 1
                    Else
                        If id_isexist(txt_user) = False Then
                            ids.Add(txt_user)
                            DGW_List.Rows.Add(True, "", search_txt, txt_date, txt_user, txt_content, tmp_href, cevap_txt, url_kontrol, txt_retweet, txt_favorite)
                            search_count = search_count + 1
                        End If

                    End If

                End If
            End If
        Next
        Return True
        'Catch ex As Exception
        '   Return False
        'End Try

    End Function

    Function goto_replyform() As Boolean

        Try
            Dim href As String = url_base & DGW_List.CurrentRow.Cells("twit_url").Value
            WB_1.Navigate(href)
            lbl_islem.Text = "Twit cevaplamak için hazırlanıyor"
            komut = "goto_replyform"
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Function reply() As Boolean
        Dim tmp_cevap As String = DGW_List.CurrentRow.Cells("cevap").Value
        If tmp_cevap = "" Then Return False
        Try
            Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("textarea")
            If els.Count > 0 Then
                els(0).InnerHtml = els(0).InnerHtml & " " & tmp_cevap
                Dim frms As HtmlElementCollection = WB_1.Document.GetElementsByTagName("form")
                frms(0).InvokeMember("submit")
                pb_error_control.Value = 0

                lbl_islem.Text = "Twit cevaplandı"
                oturum_twitsayisi = oturum_twitsayisi + 1
                lbl_sayi.Text = "Cevaplanan Twit Sayısı: " & oturum_twitsayisi
                DGW_List.CurrentRow.Cells("durum").Value = False
                For Each ce As DataGridViewCell In DGW_List.CurrentRow.Cells
                    ce.Style.BackColor = Color.LawnGreen
                    DGW_List.CurrentRow.Cells(0).Value = False
                Next

                Do
                    If sonraki_satir() = True Then Exit Do
                Loop

                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    Function add_searchlist() As Boolean
        Try
            For Each c As Control In FlowLayoutPanel1.Controls
                If c.Text = txt_q.Text Then Return True
            Next
            Dim slink As New LinkLabel
            slink.AutoSize = True
            slink.Location = New System.Drawing.Point(3, 10)
            slink.Name = txt_q.Text
            slink.Size = New System.Drawing.Size(59, 13)
            slink.TabIndex = 0
            slink.TabStop = True
            slink.Text = txt_q.Text
            FlowLayoutPanel1.Controls.Add(slink)
            AddHandler slink.LinkClicked, AddressOf Me.remove_searchlist
            Return True
        Catch ex As Exception
            Return False
        End Try
        'LinkLabel1
        '
    End Function
    Sub remove_searchlist(sender As Object, e As LinkLabelLinkClickedEventArgs)
        remove_rows(sender.text)
        sender.Dispose()
        numaralandir()
    End Sub

    Sub remove_rows(str As String)
        For Each r As DataGridViewRow In DGW_List.Rows
            If r.Cells("arama_key").Value = str Then
                r.Selected = True
            End If
        Next
        For Each s As DataGridViewRow In DGW_List.SelectedRows
            If s.Cells("arama_key").Value = str Then DGW_List.Rows.Remove(s)
        Next


    End Sub

    Function id_isexist(id As String) As Boolean
        For Each item As String In ids
            If item = id Then
                Return True
                Exit Function
            End If
        Next
        Return False
    End Function
    Function logout() As Boolean
        Try
            Dim els As HtmlElementCollection = WB_1.Document.GetElementsByTagName("button")
            For Each el As HtmlElement In els
                Dim tmp_str As String = el.InnerHtml
                If InStr(tmp_str, "Çıkış yap") > 0 Then
                    komut = "chck_logout"
                    el.InvokeMember("click")
                    Return True : Exit Function
                End If

            Next
        Catch ex As Exception
        End Try
        Return False
    End Function
    Function sonraki_satir(Optional zorla As Boolean = False) As Boolean
        pb_error_control.Value = 0
        If IsNothing(DGW_List.CurrentRow) = False Then
            Dim r As Integer = DGW_List.CurrentRow.Index
            If DGW_List.Rows.Count > r + 1 Then
                Dim nextRow As DataGridViewRow = DGW_List.Rows(r + 1)
                DGW_List.CurrentCell = nextRow.Cells(0)
                Return nextRow.Cells(0).Value
            Else
                Return True
            End If
        Else
            MessageBox.Show("Aktif bir twit satırı bulunamadı.", "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True
        End If

    End Function

#End Region

    Private Sub WB_1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WB_1.DocumentCompleted
        Select Case komut
            'Case "send_login"
            '    If WB_1.ReadyState = WebBrowserReadyState.Complete Then
            '        komut = ""
            '        If is_login() = True Then
            '            komut = "get_profile"
            '        End If
            '    End If
            'Case "get_profile"
            '    komut = ""
            '    get_profile()
            Case "goto_replyform"
                If WB_1.ReadyState = WebBrowserReadyState.Complete Then
                    komut = ""
                    lbl_islem.Text = "Twit cevap için hazırlandı."
                    reply()
                End If
            Case "start_search"
                If WB_1.ReadyState = WebBrowserReadyState.Complete Then
                    komut = ""
                    get_frompage()
                    If arama_limiti_dolu = False Then
                        next_page()
                    Else
                        numaralandir()
                        seri_arama = False
                        MessageBox.Show("Arama sona erdi. Bulunan twit sayısı: " & search_count, "Arama sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    End If
                End If
                'Case "logout"
                '    If WB_1.ReadyState = WebBrowserReadyState.Complete Then
                '        komut = ""
                '        logout()
                '    End If
            Case "chck_logout"
                If WB_1.ReadyState = WebBrowserReadyState.Complete Then
                    komut = ""
                    WB_1.Navigate(url_base)
                End If

        End Select
    End Sub



    Private Sub lnk_searhcstop_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnk_searhcstop.LinkClicked
        searchstop = True
    End Sub

    Private Sub lnk_logout_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnk_logout.LinkClicked
        'WB_1.Navigate(url_base)
        'komut = "logout"
        If logout() = True Then btn_login.Enabled = True : lnk_logout.Visible = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim p_val As Integer = pb_wait.Value + 100
        If p_val > pb_wait.Maximum Then
            pb_wait.Value = pb_wait.Maximum

            Dim beklemesuresi As Integer = bekleme.Value
            If bekleme.Value < beklemeust.Value Then
                ' Generate random value between 1 and 6. 
                Randomize()
                beklemesuresi = CInt(Int((beklemeust.Value * Rnd()) + bekleme.Value))
            Else
                ' Generate random value between 1 and 6. 
                Randomize()
                beklemesuresi = CInt(Int((bekleme.Value * Rnd()) + beklemeust.Value))
            End If
            pb_wait.Maximum = beklemesuresi * saniye
            p_val = 0
            'Timer1.Interval = beklemesuresi * saniye
            pb.Maximum = DGW_List.Rows.Count
            pb.Value = DGW_List.CurrentRow.Index + 1

            If Not DGW_List.CurrentRow.Cells("cevap").Value = "" Then
                kontrol_komut = "cevap_kontrol"
                lbl_islem.Text = "Twitin daha önce cevaplandığı kontrol ediliyor"
                WB_C.Navigate(url_base & DGW_List.CurrentRow.Cells("kontrol").Value)

                'goto_replyform()
            End If
            If DGW_List.CurrentRow.Index = DGW_List.Rows.Count - 1 Then
                Timer1.Enabled = False
                only_chck = False
                durma_istegi = True
                MessageBox.Show("Bir sonraki satıra geçilemiyor. Liste sonuna gelinmiş olabilir.", "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

            End If


        End If
        pb_wait.Value = p_val

    End Sub

    Private Sub btn_stop_Click(sender As Object, e As EventArgs) Handles btn_stop.Click
        durma_istegi = True
        Timer1.Enabled = False
        tmr_error_control.Enabled = False
        MessageBox.Show("Twit cevaplama durduruldu.", "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)
        lbl_islem.Text = "Twit cevaplama durduruldu"

    End Sub

    Sub numaralandir()
        For Each r As DataGridViewRow In DGW_List.Rows
            r.Cells("sirano").Value = r.Index + 1
        Next

    End Sub
    Private Sub WB_C_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WB_C.DocumentCompleted
        counter.Clear()
        Select Case kontrol_komut
            Case "cevap_kontrol"
                If WB_1.ReadyState = WebBrowserReadyState.Complete Then
                    kontrol_komut = ""
                    Dim els As HtmlElementCollection = WB_C.Document.GetElementsByTagName("span")
                    For Each el As HtmlElement In els
                        Dim tmp_str As String = el.GetAttribute("classname")
                        If InStr(tmp_str, "statnum") > 0 Then
                            counter.Add(el.InnerText)
                        End If
                    Next
                    contur_bilgisi_alama = True
                    If seri_arama = True Then Exit Sub
                    If counter.Count > 0 Then
                        DGW_List.CurrentRow.Cells("retweets").Value = counter(0)
                        If counter.Count > 1 Then
                            DGW_List.CurrentRow.Cells("favorites").Value = counter(1)
                        End If

                        For Each ce As DataGridViewCell In DGW_List.CurrentRow.Cells
                            ce.Style.BackColor = Color.Bisque
                        Next
                    End If
                    els = WB_C.Document.GetElementsByTagName("div")
                    For Each el As HtmlElement In els
                        Dim tmp_str As String = el.InnerText
                        Dim tmp_id As String = Replace(Replace(lbl_username.Text, "@", ""), " ", "")

                        If InStr(tmp_str, tmp_id) > 0 Then
                            'Cevap yazılmış
                            lbl_islem.Text = "Twit daha önce cevaplanmış"

                            For Each ce As DataGridViewCell In DGW_List.CurrentRow.Cells
                                ce.Style.ForeColor = Color.DarkRed
                                'ce.Style.BackColor = Color.DarkRed
                                DGW_List.CurrentRow.Cells(0).Value = False
                            Next
                            '****
                            If only_chck = False Then
                                sonraki_satir()
                                Master_Start()
                            End If
                            '****

                            Exit Sub
                        End If

                    Next
                    lbl_islem.Text = "Twit daha önce cevaplanmamış"
                    If only_chck = False Then
                        goto_replyform()
                    Else
                        If durma_istegi = False Then
                            Dim id_last As Integer = DGW_List.CurrentRow.Index
                            sonraki_satir()
                            Dim id_next As Integer = DGW_List.CurrentRow.Index
                            If id_last = id_next Then
                                durma_istegi = True
                            End If
                            start_info()
                        End If
                    End If

                End If


        End Select

    End Sub

    Function start_info() As Boolean
        kontrol_komut = "cevap_kontrol"
        WB_C.Navigate(url_base & DGW_List.CurrentRow.Cells("kontrol").Value)
        Return True
    End Function


    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        frmVersiyon.ShowDialog()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If lnk_logout.Visible = True Then MsgBox("Çıkış yapmadınız") : e.Cancel = True : Exit Sub
        'resimleri_goster("yes")
    End Sub

    Private Sub tmr_error_control_Tick(sender As Object, e As EventArgs) Handles tmr_error_control.Tick

        Dim yenideger As Integer = pb_error_control.Value + 1

        If yenideger > pb_error_control.Maximum Then
            yenideger = pb_error_control.Maximum
            pb_error_control.Value = 0

            If My.Computer.Network.IsAvailable Then

                If IsNothing(DGW_List.CurrentRow) = False Then
                    Dim r As Integer = DGW_List.CurrentRow.Index
                    If DGW_List.Rows.Count > r + 1 Then
                        Dim nextRow As DataGridViewRow = DGW_List.Rows(r + 1)
                        DGW_List.CurrentCell = nextRow.Cells(0)
                        pb_wait.Value = 0
                        goto_replyform()
                    End If
                End If


            Else
                pb_error_control.Value = 0
            End If

        Else

            pb_error_control.Value = yenideger
        End If


    End Sub

    Private Sub error_wait(sender As Object, e As EventArgs) Handles DkToolStripMenuItem.Click
        pb_error_control.Maximum = sender.tag * saniye / 10
    End Sub
    Public Sub ExportData()

        Dim StrExport As String = ""
        Dim filename As String = ""
        Dim sf As New SaveFileDialog
        sf.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
        sf.FileName = FormatDateTime(Now.AddDays(1), DateFormat.ShortDate)
        sf.Filter = "Excel Dosyası Virgülle Ayrılmış (*.csv)|*.csv"

        If sf.ShowDialog = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        Dim tw As IO.TextWriter = New IO.StreamWriter(sf.FileName, False, System.Text.Encoding.UTF8)
        For Each C As DataGridViewColumn In DGW_List.Columns
            StrExport &= """" & C.HeaderText & """;"
        Next
        If StrExport.Length > 1 Then StrExport = StrExport.Substring(0, StrExport.Length - 1)
        StrExport &= Environment.NewLine
        tw.Write(StrExport)
        pb.Value = 0
        pb.Maximum = DGW_List.Rows.Count
        Dim i As Integer = 0
        For Each R As DataGridViewRow In DGW_List.Rows
            StrExport = ""
            i = i + 1
            pb.Value = i
            For Each C As DataGridViewCell In R.Cells
                If IsNothing(C.Value) = False Then
                    StrExport &= """" & C.Value.ToString.Replace(vbCrLf, "").Replace(";", ",") & """;"
                Else
                    StrExport &= """" & " " & """;"
                End If
            Next
            StrExport = StrExport.Substring(0, StrExport.Length - 1)
            StrExport &= Environment.NewLine
            tw.Write(StrExport)

        Next
        MsgBox("Masasüstüne Kaydedildi")

        tw.Close()
    End Sub
    Public Sub importData()
        Try
            Dim op As New OpenFileDialog
            op.Filter = "Excel Dosyası Virgülle Ayrılmış (*.csv)|*.csv"

            op.ShowDialog()
            If op.FileName = "" Then
                Exit Sub
            End If
            Dim FILE_NAME As String = op.FileName

            Dim TextLine As String
            DGW_List.Rows.Clear()
            If System.IO.File.Exists(FILE_NAME) = True Then
                Dim objReader As New System.IO.StreamReader(FILE_NAME, System.Text.Encoding.GetEncoding("ISO-8859-9"))
                DGW_List.Rows.Clear()
                'DGW_List.Columns.Clear()
                TextLine = Replace(objReader.ReadLine(), """", "")
                Dim arr As New ArrayList
                arr.AddRange(Split(TextLine, ";"))
                'DGW_List.ColumnCount = arr.Count
                'DGW_List.Columns(0).ValueType= System .Type .
                Dim i As Integer = 0


                Do While objReader.Peek() <> -1
                    TextLine = Replace(objReader.ReadLine(), """", "")
                    arr.Clear()
                    arr.AddRange(Split(TextLine, ";"))
                    DGW_List.Rows.Add()

                    i = 0
                    For Each ar As String In arr
                        If i < DGW_List.ColumnCount Then
                            DGW_List.Rows(DGW_List.Rows.Count - 1).Cells(i).Value = ar
                            i = i + 1

                        End If
                    Next

                Loop
            Else

                MsgBox("File Does Not Exist")

            End If

        Catch ex As Exception
            MessageBox.Show("Dosya Açma Hatası. " & ex.Message, "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information)

        End Try

    End Sub


    Private Sub btn_saveexcel_Click(sender As Object, e As EventArgs) Handles btn_saveexcel.Click
        Try
            ExportData()
        Catch ex As Exception
            MsgBox("Kayıt Hatası: " & ex.Message)
        End Try
    End Sub

    Private Sub btn_openexcel_Click(sender As Object, e As EventArgs) Handles btn_openexcel.Click
        Cursor = Cursors.WaitCursor
        Try
            importData()
        Catch ex As Exception

        End Try
        Cursor = Cursors.Arrow
    End Sub

    Private Sub btn_getinfo_Click(sender As Object, e As EventArgs) Handles btn_getinfo.Click
        durma_istegi = False
        only_chck = True
        seri_arama = False
        start_info()
    End Sub
End Class
