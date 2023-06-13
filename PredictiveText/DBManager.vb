Imports System.Data.Entity.Migrations
Imports System.Linq

Public Module DBManager
    Public Function AddNewWord(word As WordEntity) As Boolean
        Using context As New PredictoEntities()
            context.WordEntities.Add(word)
            context.SaveChanges()
            Return True
        End Using
    End Function

    Public Function SearchWord(word As String) As List(Of WordEntity)
        Using context As New PredictoEntities()
            Dim result = (From v In context.WordEntities Where v.Content.StartsWith(word) Order By v.Count Descending).Take(5)
            Dim words As List(Of WordEntity) = New List(Of WordEntity)
            For Each value As WordEntity In result
                words.Add(value)
            Next
            Return words
        End Using
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
