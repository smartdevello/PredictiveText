Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Partial Public Class SuggestionList
    Inherits ListBox

    Public Property SelectionBackColor As Color = Color.DarkOrange
    Public Property MatchedForeColor As Color = Color.FromArgb(166, 216, 255)
    Private _stringToFind As String

    Public Property stringToFind As String
        Get
            Return _stringToFind
        End Get
        Set(value As String)
            _stringToFind = value
            Invalidate()
        End Set
    End Property


    Private Const WM_KILLFOCUS As Integer = &H8
    Private Const WM_VSCROLL As Integer = &H115
    Private Const WM_HSCROLL As Integer = &H114

    Public Sub New()
        MyBase.New()
        SetStyle(ControlStyles.Opaque Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        DrawMode = DrawMode.OwnerDrawFixed
    End Sub

    <DllImport("uxtheme", ExactSpelling:=True)>
    Private Shared Function DrawThemeParentBackground(ByVal hWnd As IntPtr, ByVal hdc As IntPtr, ByRef pRect As Rectangle) As Integer

    End Function

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g = e.Graphics
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
        'g.SmoothingMode = SmoothingMode.AntiAlias
        'g.InterpolationMode = InterpolationMode.HighQualityBilinear

        Dim rec = ClientRectangle
        Dim hdc As IntPtr = g.GetHdc()
        DrawThemeParentBackground(Me.Handle, hdc, rec)
        g.ReleaseHdc(hdc)

        Using reg As Region = New Region(e.ClipRectangle)

            If Items.Count > 0 Then

                For i As Integer = 0 To Items.Count - 1
                    rec = GetItemRectangle(i)

                    If e.ClipRectangle.IntersectsWith(rec) Then

                        If (SelectionMode = SelectionMode.One AndAlso SelectedIndex = i) OrElse (SelectionMode = SelectionMode.MultiSimple AndAlso SelectedIndices.Contains(i)) OrElse (SelectionMode = SelectionMode.MultiExtended AndAlso SelectedIndices.Contains(i)) Then
                            OnDrawItem(New DrawItemEventArgs(g, Font, rec, i, DrawItemState.Selected, ForeColor, BackColor))
                        Else
                            OnDrawItem(New DrawItemEventArgs(g, Font, rec, i, DrawItemState.[Default], ForeColor, BackColor))
                        End If

                        reg.Complement(rec)
                    End If
                Next
            End If
        End Using
    End Sub

    Protected Overrides Sub OnDrawItem(ByVal e As DrawItemEventArgs)
        If e.Index < 0 Then Return


        Dim rec = e.Bounds
        Dim g = e.Graphics

        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        'g.SmoothingMode = SmoothingMode.AntiAlias
        'g.InterpolationMode = InterpolationMode.HighQualityBilinear

        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            rec.Y = rec.Y + 2
            Using sb As SolidBrush = New SolidBrush(SelectionBackColor)
                g.FillRectangle(sb, rec)
            End Using
        End If

        If Not String.IsNullOrEmpty(stringToFind) Then

            Dim MyString As String = GetItemText(Items(e.Index))
            Dim strings As String() = MyString.Split(New String() {stringToFind}, StringSplitOptions.None)
            Dim rect As Rectangle = e.Bounds

            For i As Integer = 0 To strings.Length - 1
                Dim s As String = strings(i)

                If s <> "" Then
                    Dim width As Integer = CInt(e.Graphics.MeasureString(s, e.Font, e.Bounds.Width, StringFormat.GenericTypographic).Width)
                    rect.Width = width
                    TextRenderer.DrawText(e.Graphics, s, e.Font, New Point(rect.X, rect.Y), ForeColor)
                    rect.X += width
                End If

                If i < strings.Length - 1 Then
                    Dim width2 As Integer = CInt(e.Graphics.MeasureString(stringToFind, e.Font, e.Bounds.Width, StringFormat.GenericTypographic).Width)
                    rect.Width = width2
                    TextRenderer.DrawText(e.Graphics, stringToFind, e.Font, New Point(rect.X, rect.Y), ForeColor, MatchedForeColor)
                    rect.X += width2
                End If
            Next
        End If
    End Sub

    Protected Overrides Sub OnSelectedIndexChanged(ByVal e As EventArgs)
        MyBase.OnSelectedIndexChanged(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        Invalidate()
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg <> WM_KILLFOCUS AndAlso (m.Msg = WM_HSCROLL OrElse m.Msg = WM_VSCROLL) Then Invalidate()
        MyBase.WndProc(m)
    End Sub
End Class
