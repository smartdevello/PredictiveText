Imports System.ComponentModel
Imports System.IO

Public Class WordDatabase
    Private Sub WordDatabase_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        'Dim reader As StreamReader = My.Computer.FileSystem.OpenTextFileReader("3000.txt")
        'Dim a As String

        'Do
        '    a = reader.ReadLine
        '    Dim currentWord

        '    If a.Length >= 5 Then
        '        currentWord = New WordEntity With {
        '            .Id = Guid.NewGuid().ToString(),
        '            .Content = a,
        '            .Count = 0,
        '            .AppName = "Visual Studio",
        '            .UpdatedAt = DateTime.Now
        '        }
        '        DBManager.AddNewWord(currentWord)
        '    End If

        'Loop Until a Is Nothing

        'reader.Close()


        Dim searched = DBManager.SearchWord("", 30)
        mydbDataGrid.DataSource = searched
        mydbDataGrid.Columns(0).Visible = False
        mydbDataGrid.Columns(2).ReadOnly = True
        mydbDataGrid.Columns(3).Visible = False
        mydbDataGrid.Columns(4).ReadOnly = True
    End Sub

    Private Sub btn_searchWord_Click(sender As Object, e As EventArgs) Handles btn_searchWord.Click
        Dim searched = DBManager.SearchWord(wordToSearch.Text, 30)
        mydbDataGrid.DataSource = searched
    End Sub

    Private Sub mydbDataGrid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles mydbDataGrid.CellValueChanged
        If e.ColumnIndex <> 1 Then Return
        If e.RowIndex < 0 Then Return

        Dim changedVal As String = mydbDataGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        Dim id As String = mydbDataGrid.Rows(e.RowIndex).Cells(0).Value
        DBManager.AddorUpdateWord(changedVal, "Visual Studio")
    End Sub

    Private Sub mydbDataGrid_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles mydbDataGrid.CellValidating
        'If (e.ColumnIndex == mydbDataGrid.Columns("")) Then
    End Sub

    Private Sub mydbDataGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles mydbDataGrid.CellContentClick
        If mydbDataGrid.Columns(e.ColumnIndex).Name = "Delete" Then
            If (MessageBox.Show("Are you sure to delete this record ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
                Dim id As String = mydbDataGrid.Rows(e.RowIndex).Cells(0).Value
                DBManager.DeleteWord(id)
                mydbDataGrid.Rows.RemoveAt(e.RowIndex)
            End If
        End If
    End Sub
End Class