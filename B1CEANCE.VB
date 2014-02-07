Imports System.Data
Imports NTSInformatica.CLN__STD
Imports NTSInformatica.CLD__BASE
Imports System.Data.SqlClient


Public Class FR1CEANCE
  Inherits FROCEANCE

  Public bAttivaDocnet As Boolean = False
  Public WithEvents tlb4DArch As NTSBarButtonItem
  Public WithEvents tlb4DVis As NTSBarButtonItem

#Region "Overrides"

  Public Overrides Sub InitControls()
    Try

      'Leggo le verticali attive
      bAttivaDocnet = CBool(oMenu.GetSettingBus("4DOCNET", "OPZIONI", ".", "AttivaDocNet", "0", " ", "0"))

      '4Docnet - Inizializzo i tasti Toolbar
      If bAttivaDocnet Then

        Me.tlb4DArch = New NTSInformatica.NTSBarButtonItem
        Me.tlb4DVis = New NTSInformatica.NTSBarButtonItem
        Me.NtsBarManager1.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.tlb4DArch, Me.tlb4DVis})
        Me.tlbMain.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.tlb4DArch), New DevExpress.XtraBars.LinkPersistInfo(Me.tlb4DVis)})

        '
        'tlb4DArch
        '
        Me.tlb4DArch.Caption = "Archivia 4Docnet"
        Me.tlb4DArch.Name = "tlb4DArch"
        Me.tlb4DArch.Visible = True

        '
        'tlb4DVis
        '
        Me.tlb4DVis.Caption = "Visualizza 4Docnet"
        Me.tlb4DVis.Name = "tlb4DVis"
        Me.tlb4DVis.Visible = True

        Try
          tlb4DArch.Glyph = Bitmap.FromFile(oApp.ChildImageDir & "\tlb4dArch.gif")
          tlb4DVis.Glyph = Bitmap.FromFile(oApp.ChildImageDir & "\tlb4dVis.gif")
        Catch ex As Exception
          'Non eseguo il Catch: se non c'è il GIF semplicemente non carico niente
        End Try

      End If

      MyBase.InitControls()

    Catch ex As Exception
      '-------------------------------------------------
      Dim strErr As String = GestError(ex, Me, "", oApp.InfoError, oApp.ErrorLogFile, True)
      '-------------------------------------------------
    End Try
  End Sub

  Public Overrides Sub GestisciEventiEntity(ByVal sender As Object, ByRef e As NTSEventArgs)
    Dim strParam As String = ""
    Dim strSost As String = ""

    If bAttivaDocnet Then
      If (e.TipoEvento & StrDup(10, " "c)).Substring(0, 10) = "ELABORAZI:" Then
        ''evnt.RetValue = "CCC" 'posso restituire un valore al entity BE
        ''oApp.MsgBoxErr("GestisciEventiEntity personalizzato: " & evnt.Message & " - " & evnt.TipoEvento & " - Restituito valore 'ccc' – Codice        passato(" & evnt.Message.Substring(11))")

        strSost = e.Message.ToString
        strSost = strSost.Substring(10)
        strSost = strSost.Replace("|", ";")

        strParam = strSost
        ChiamaVbIDoc(strParam)

      End If
    End If

    MyBase.GestisciEventiEntity(sender, e)
  End Sub
#End Region

#Region "4Docnet"
  Public Overridable Sub ChiamaVbIDoc(ByRef strParam As String)

    Try
      'chiamo form 
      If bAttivaDocnet Then
        oMenu.RunChild("BS4DVNET", "CLS4DVNET", "Documentale", DittaCorrente, "", "", Nothing, strParam, True, True)
        oMenu.CloseChild("BS4DVNET", "CLS4DVNET")
      End If

    Catch ex As Exception
      Dim strErr As String = CLN__STD.GestError(ex, Nothing, "", oApp.InfoError, oApp.ErrorLogFile, True)
    End Try

  End Sub

  Function tlbIDoc_Click(ByRef strSelez As String) As Object
    Dim oPar As CLE__CLDP = Nothing
    Dim strParam As String = ""
    Dim strCodCesp As String = ""
    Dim dtDatReg As Date = NTSCDate("01/01/1900")
    Dim strNumReg As String = "0"
    Try
      'controlli
      If strSelez = "4DINSER" Then
        If Not Salva() = True Then
          Return Nothing
        End If
      End If
      If Trim(edAe_codcesp.Text) = "0" Then
        oApp.MsgBoxErr("Manca Codice Cespite!")
        Return Nothing
      End If
      If dsAnce Is Nothing Then Return Nothing
      If dsAnce.Tables("ANACESP") Is Nothing Then Return Nothing
      If dsAnce.Tables("ANACESP").Rows.Count = 0 Then Return Nothing

      'valorizza parametri
      strCodCesp = dsAnce.Tables("ANACESP").Rows(0)!ae_codcesp.ToString

      'compongo stringa parametri
      strParam = ";" & strSelez & ";" & "FMANCE;" & CStr(strCodCesp)

      ChiamaVbIDoc(strParam)

    Catch ex As Exception
      Dim strErr As String = CLN__STD.GestError(ex, Nothing, "", oApp.InfoError, oApp.ErrorLogFile, True)
    End Try
    Return Nothing
  End Function

  Public Overridable Sub tlb4DArch_ItemClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs) Handles tlb4DArch.ItemClick
    tlbIDoc_Click("4DINSER")
  End Sub

  Public Overridable Sub tlb4DVis_ItemClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs) Handles tlb4DVis.ItemClick
    tlbIDoc_Click("4DVISUA")
  End Sub

#End Region

End Class


