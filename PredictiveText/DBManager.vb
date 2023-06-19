Imports System.ComponentModel
Imports System.Data.Entity.Migrations
Imports System.Linq

Public Module DBManager
    Public Function AddNewWord(word As WordEntity) As Boolean
        Using context As New PredictoEntities()
            context.WordEntities.Add(word)
            context.SaveChanges()
            Return True
        End Using
        Return False
    End Function

    Public Function SearchWord(word As String, Optional maxCount As Integer = 5) As BindingList(Of WordEntity)
        Using context As New PredictoEntities()
            Dim result
            If word <> "" Then
                result = (From v In context.WordEntities Where v.Content.StartsWith(word) Order By v.Count Descending).Take(maxCount)
            Else
                result = (From v In context.WordEntities Order By v.Count Descending).Take(maxCount)
            End If
            Dim words As BindingList(Of WordEntity) = New BindingList(Of WordEntity)
            For Each value As WordEntity In result
                words.Add(value)
            Next
            Return words
        End Using
    End Function
    Public Function DeleteWord(id As String) As Boolean
        Using context As New PredictoEntities()
            Dim currentWord = context.WordEntities.Where(Function(item) item.Id = id).FirstOrDefault()
            If currentWord IsNot Nothing Then
                context.WordEntities.Remove(currentWord)
                context.SaveChanges()
                Return True
            End If
        End Using
        Return False
    End Function


    Public Function AddorUpdateWord(word As String, appName As String) As WordEntity
        Using context As New PredictoEntities()
            Dim currentWord = context.WordEntities.Where(Function(item) item.Content = word).FirstOrDefault()
            If currentWord IsNot Nothing Then
                currentWord.Count = currentWord.Count + 1
                currentWord.UpdatedAt = DateTime.Now
                context.WordEntities.AddOrUpdate(currentWord)
            Else
                currentWord = New WordEntity With {
                    .Id = Guid.NewGuid().ToString(),
                    .Content = word,
                    .Count = 1,
                    .AppName = appName,
                    .UpdatedAt = DateTime.Now
                }
                context.WordEntities.Add(currentWord)
            End If
            context.SaveChanges()
            Return currentWord
        End Using
    End Function
End Module
