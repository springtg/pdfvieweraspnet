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
        If args.Length < 9 Then
          Console.WriteLine("Usage: pdfcmdline png <filename> <outputdir> <pagenumber> <dpi> <password> <searchtext> <searchdirection>")
          Exit Sub
        End If
        Dim myResponse As String = PDFLibHelper.GetPageFromPDF(args(2), args(3), args(4), args(5), args(6), args(7), args(8))
        If myResponse = PDFLibHelper.BAD_PASSWORD Then
          Console.WriteLine(myResponse)
        Else
          Console.WriteLine("png=" & myResponse)
          Console.WriteLine("page=" & args(4))
        End If

      End If
      If args(1) = "bookmark" Then
        If args.Length < 3 Then
          Console.WriteLine("Usage: pdflibcmdline bookmark <filename> [<password>] [<pageNumberOnly>] ")
          Exit Sub
        End If
        Console.WriteLine("bookmark=" & PDFLibHelper.BuildHTMLBookmarks(args(2), If(args.Length > 3, args(3), ""), If(args.Length > 4, (args(4) = 1), False)))
      End If
    Catch ex As Exception
      Console.WriteLine("exception=" & ex.ToString)
    End Try

  End Sub

End Module
