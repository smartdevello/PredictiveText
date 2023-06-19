<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WordDatabase
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WordDatabase))
        Me.mydbDataGrid = New System.Windows.Forms.DataGridView()
        Me.wordToSearch = New System.Windows.Forms.TextBox()
        Me.btn_searchWord = New System.Windows.Forms.Button()
        Me.Delete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.IdDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ContentDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CountDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AppNameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UpdatedAtDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.WordEntityBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.mydbDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WordEntityBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'mydbDataGrid
        '
        Me.mydbDataGrid.AutoGenerateColumns = False
        Me.mydbDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.mydbDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.mydbDataGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.IdDataGridViewTextBoxColumn, Me.ContentDataGridViewTextBoxColumn, Me.CountDataGridViewTextBoxColumn, Me.AppNameDataGridViewTextBoxColumn, Me.UpdatedAtDataGridViewTextBoxColumn, Me.Delete})
        Me.mydbDataGrid.DataSource = Me.WordEntityBindingSource
        Me.mydbDataGrid.Location = New System.Drawing.Point(57, 53)
        Me.mydbDataGrid.Name = "mydbDataGrid"
        Me.mydbDataGrid.Size = New System.Drawing.Size(571, 414)
        Me.mydbDataGrid.TabIndex = 0
        '
        'wordToSearch
        '
        Me.wordToSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.wordToSearch.Location = New System.Drawing.Point(130, 14)
        Me.wordToSearch.Name = "wordToSearch"
        Me.wordToSearch.Size = New System.Drawing.Size(185, 24)
        Me.wordToSearch.TabIndex = 1
        '
        'btn_searchWord
        '
        Me.btn_searchWord.Location = New System.Drawing.Point(397, 12)
        Me.btn_searchWord.Name = "btn_searchWord"
        Me.btn_searchWord.Size = New System.Drawing.Size(112, 26)
        Me.btn_searchWord.TabIndex = 2
        Me.btn_searchWord.Text = "Search"
        Me.btn_searchWord.UseVisualStyleBackColor = True
        '
        'Delete
        '
        Me.Delete.HeaderText = "Delete"
        Me.Delete.Name = "Delete"
        Me.Delete.Text = "Delete"
        Me.Delete.UseColumnTextForButtonValue = True
        '
        'IdDataGridViewTextBoxColumn
        '
        Me.IdDataGridViewTextBoxColumn.DataPropertyName = "Id"
        Me.IdDataGridViewTextBoxColumn.HeaderText = "Id"
        Me.IdDataGridViewTextBoxColumn.Name = "IdDataGridViewTextBoxColumn"
        '
        'ContentDataGridViewTextBoxColumn
        '
        Me.ContentDataGridViewTextBoxColumn.DataPropertyName = "Content"
        Me.ContentDataGridViewTextBoxColumn.HeaderText = "Content"
        Me.ContentDataGridViewTextBoxColumn.Name = "ContentDataGridViewTextBoxColumn"
        '
        'CountDataGridViewTextBoxColumn
        '
        Me.CountDataGridViewTextBoxColumn.DataPropertyName = "Count"
        Me.CountDataGridViewTextBoxColumn.HeaderText = "Count"
        Me.CountDataGridViewTextBoxColumn.Name = "CountDataGridViewTextBoxColumn"
        '
        'AppNameDataGridViewTextBoxColumn
        '
        Me.AppNameDataGridViewTextBoxColumn.DataPropertyName = "AppName"
        Me.AppNameDataGridViewTextBoxColumn.HeaderText = "AppName"
        Me.AppNameDataGridViewTextBoxColumn.Name = "AppNameDataGridViewTextBoxColumn"
        '
        'UpdatedAtDataGridViewTextBoxColumn
        '
        Me.UpdatedAtDataGridViewTextBoxColumn.DataPropertyName = "UpdatedAt"
        Me.UpdatedAtDataGridViewTextBoxColumn.HeaderText = "UpdatedAt"
        Me.UpdatedAtDataGridViewTextBoxColumn.Name = "UpdatedAtDataGridViewTextBoxColumn"
        '
        'WordEntityBindingSource
        '
        Me.WordEntityBindingSource.DataSource = GetType(Predicto.WordEntity)
        '
        'WordDatabase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(683, 513)
        Me.Controls.Add(Me.btn_searchWord)
        Me.Controls.Add(Me.wordToSearch)
        Me.Controls.Add(Me.mydbDataGrid)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "WordDatabase"
        Me.Text = "WordDatabase"
        CType(Me.mydbDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WordEntityBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents mydbDataGrid As DataGridView
    Friend WithEvents wordToSearch As TextBox
    Friend WithEvents btn_searchWord As Button
    Friend WithEvents IdDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ContentDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents CountDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents AppNameDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents UpdatedAtDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Delete As DataGridViewButtonColumn
    Friend WithEvents WordEntityBindingSource As BindingSource
End Class
