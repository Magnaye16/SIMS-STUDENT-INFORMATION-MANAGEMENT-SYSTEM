Imports MySql.Data.MySqlClient
Module Module1
    Public con As New MySqlConnection
    Public cmd As New MySqlCommand

    Sub openCon()
        'con.ConnectionString = "server=34.228.230.132; username=root; password=Alex0987654321!; database=SMS_DB"
        con.ConnectionString = "server=Localhost; username=root; password=; database=SMS_DB"
        con.Open()

    End Sub

End Module