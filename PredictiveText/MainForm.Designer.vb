<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.suggestionWord = New Predicto.SuggestionWord()
        Me.SuspendLayout()
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Predicto"
        Me.NotifyIcon1.Visible = True
        '
        'suggestionWord
        '
        Me.suggestionWord.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.suggestionWord.LineHeight = 20
        Me.suggestionWord.LineWidth = 70
        Me.suggestionWord.Location = New System.Drawing.Point(-3, -4)
        Me.suggestionWord.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.suggestionWord.MatchedForeColor = System.Drawing.Color.FromArgb(CType(CType(166, Byte), Integer), CType(CType(216, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.suggestionWord.Name = "suggestionWord"
        Me.suggestionWord.SelectedIndex = 0
        Me.suggestionWord.SelectionBackColor = System.Drawing.Color.DarkOrange
        Me.suggestionWord.Size = New System.Drawing.Size(199, 85)
        Me.suggestionWord.StringToFind = "bar"
        Me.suggestionWord.TabIndex = 1
        Me.suggestionWord.words = CType(resources.GetObject("suggestionWord.words"), System.Collections.Generic.List(Of String))
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.Info
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(619, 362)
        Me.Controls.Add(Me.suggestionWord)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "MainForm"
        Me.ShowInTaskbar = False
        Me.Text = "Form1"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents suggestionWord As SuggestionWord
    Friend WithEvents NotifyIcon1 As NotifyIcon
End Class
