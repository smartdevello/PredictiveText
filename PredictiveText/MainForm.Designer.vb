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
        Me.suggestionList = New Predicto.SuggestionList()
        Me.SuspendLayout()
        '
        'suggestionList
        '
        Me.suggestionList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.suggestionList.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.suggestionList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.suggestionList.Font = New System.Drawing.Font("Consolas", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.suggestionList.FormattingEnabled = True
        Me.suggestionList.ItemHeight = 15
        Me.suggestionList.Location = New System.Drawing.Point(0, 2)
        Me.suggestionList.MatchedForeColor = System.Drawing.Color.FromArgb(CType(CType(166, Byte), Integer), CType(CType(216, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.suggestionList.Name = "suggestionList"
        Me.suggestionList.SelectionBackColor = System.Drawing.Color.DarkOrange
        Me.suggestionList.Size = New System.Drawing.Size(191, 90)
        Me.suggestionList.TabIndex = 0
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.Info
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(193, 100)
        Me.Controls.Add(Me.suggestionList)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "MainForm"
        Me.ShowInTaskbar = False
        Me.Text = "Form1"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents suggestionList As SuggestionList
End Class
