Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure
Imports SQLite.CodeFirst

Partial Public Class PredictoEntities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=PredictoEntities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Dim sqliteConnectionInitializer As SqliteCreateDatabaseIfNotExists(Of PredictoEntities) = New SqliteCreateDatabaseIfNotExists(Of PredictoEntities)(modelBuilder)
        Database.SetInitializer(sqliteConnectionInitializer)
        'MyBase.OnModelCreating(modelBuilder)
    End Sub

    Public Overridable Property WordEntities() As DbSet(Of WordEntity)

End Class