<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SuggestionWord
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private _SelectedIndex As Integer = 0
    Public Property SelectedIndex As Integer
        Get
            Return _SelectedIndex
        End Get
        Set(value As Integer)
            _SelectedIndex = value
            Invalidate()
        End Set
    End Property

    Private _words As List(Of String)
    Public Property words As List(Of String)
        Get
            Return _words
        End Get
        Set(value As List(Of String))
            _words = value
            Invalidate()
        End Set
    End Property

    Private _strintToFind As String = "bar"
    Public Property StringToFind As String
        Get
            Return _strintToFind
        End Get
        Set(value As String)
            _strintToFind = value
            Invalidate()
        End Set
    End Property

    Public Property LineHeight As Integer = 20
    Public Property LineWidth As Integer = Me.Width

    Public Property SelectionBackColor As Color = Color.DarkOrange
    Public Property MatchedForeColor As Color = Color.FromArgb(166, 216, 255)



    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g = e.Graphics
        Dim x As Integer = 5, y As Integer = 5
        Dim brush = New SolidBrush(ForeColor)
        Dim selectionBrush = New SolidBrush(SelectionBackColor)
        Dim matchedBrush = New SolidBrush(MatchedForeColor)

        If SelectedIndex >= 0 Then g.FillRectangle(selectionBrush, New Rectangle(x, SelectedIndex * LineHeight + y, Me.Width, LineHeight))

        Dim rect As Rectangle
        For i As Integer = 0 To words.Count - 1
            rect.X = x
            rect.Y = y
            rect.Width = Me.Width
            rect.Height = LineHeight

            If Not String.IsNullOrEmpty(StringToFind) Then
                Dim strings As String() = words(i).Split(New String() {StringToFind}, StringSplitOptions.None)

                For j As Integer = 0 To strings.Length - 1
                    Dim s As String = strings(j)
                    If s <> "" Then
                        Dim width As Integer = CInt(g.MeasureString(s, Font, Me.Width, StringFormat.GenericTypographic).Width)
                        rect.Width = width
                        TextRenderer.DrawText(e.Graphics, s, Font, New Point(rect.X, rect.Y), ForeColor)
                        rect.X += width
                    End If
                    If j < strings.Length - 1 Then
                        Dim width As Integer = CInt(g.MeasureString(StringToFind, Font, Me.Width, StringFormat.GenericTypographic).Width)
                        rect.Width = width
                        TextRenderer.DrawText(e.Graphics, StringToFind, Font, New Point(rect.X, rect.Y), ForeColor, MatchedForeColor)
                        rect.X += width
                    End If
                Next
            End If

            y = y + LineHeight
        Next




    End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'SuggestionWord
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Name = "SuggestionWord"
        Me.Size = New System.Drawing.Size(264, 98)
        Me.ResumeLayout(False)

    End Sub
End Class
