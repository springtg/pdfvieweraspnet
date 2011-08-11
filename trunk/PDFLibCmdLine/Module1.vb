Imports System.Drawing

Module Module1

  Sub Main()
    Try
      Dim args() As String = Environment.GetCommandLineArgs
      If args.Length < 3 Then
        Console.WriteLine("Usage: pdflibcmdline <command> <filename> <password>")
        Exit Sub
      End If
      If args(1) = "pass" Then
        Console.WriteLine("pass=" & PDFLibHelper.IsPasswordValid(args(2), If(args.Length > 3, args(3), "")))
      End If
      If args(1) = "passreq" Then
        Console.WriteLine("passreq=" & PDFLibHelper.IsPasswordRequired(args(2)))
      End If
      If args(1) = "count" Then
        Console.WriteLine("count=" & PDFLibHelper.GetPDFPageCount(args(2), If(args.Length > 3, args(3), "")))
      End If
      If args(1) = "dpi" Then
        Console.WriteLine("dpi=" & PDFLibHelper.GetOptimalDPI(args(2), args(3), New System.Drawing.Size(args(4), args(5)), If(args.Length > 6, args(6), "")))
      End If
      If args(1) = "png" Then
        If args.Length < 10 Then
          Console.WriteLine("Usage: pdfcmdline png <filename> <outputdir> <pagenumber> <dpi> <password> <searchtext> <searchdirection> <useMuPDF>")
          Exit Sub
        End If
        Dim myResponse As List(Of String) = PDFLibHelper.GetPageFromPDF(args(2), args(3), args(4), Size.Empty, args(5), args(6), args(7), args(8), args(9))
        For Each s In myResponse
          Console.WriteLine(s)
        Next
      End If
      If args(1) = "bookmark" Then
        If args.Length < 3 Then
          Console.WriteLine("Usage: pdflibcmdline bookmark <filename> [<password>] [<pageNumberOnly>] ")
          Exit Sub
        End If
        Console.WriteLine("bookmark=" & PDFLibHelper.BuildHTMLBookmarks(args(2), If(args.Length > 3, args(3), ""), If(args.Length > 4, (args(4) = 1), False)))
      End If
      If args(1) = "info" Then
        Dim myList As New List(Of DictionaryEntry)
        myList = PDFLibHelper.GetPDFInfo(args(2), If(args.Length > 3, args(3), ""))
        For Each item In myList
          Console.WriteLine(String.Format("{0}={1}", item.Key, item.Value))
        Next
      End If
      If args(1) = "pngauto" Then
        If args.Length < 11 Then
          Console.WriteLine("Usage: pdfcmdline png <filename> <outputdir> <pagenumber> <width> <height> <password> <searchtext> <searchdirection> <useMuPDF>")
          Exit Sub
        End If
        Dim myResponse As List(Of String) = PDFLibHelper.GetPageFromPDF(args(2), args(3), args(4), New Size(args(5), args(6)), 0, args(7), args(8), args(9), args(10))
        For Each s In myResponse
          Console.WriteLine(s)
        Next
      End If
    Catch ex As Exception
      Console.WriteLine("exception=" & ex.ToString)
    End Try

  End Sub

End Module
