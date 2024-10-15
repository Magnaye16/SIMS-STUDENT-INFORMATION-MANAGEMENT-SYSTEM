Imports Guna.UI2.WinForms
Imports Microsoft.VisualBasic.Logging
Imports MySql.Data.MySqlClient

Public Class LOGIN
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        uid_txtbx.Clear()
        studname_txtbx.Clear()
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Dim idcode As String = uid_txtbx.Text
        If AuthenticateUser(idcode) Then
            Dim userRole As String = GetUserRole(idcode)
            If userRole = "emp" Then
                con.Close()
                'display empid in txtbox in login form
                'SetEmpid()
                'LOGIN.Hide()
                Me.Hide()
                'studpage.Show()

            ElseIf uid_txtbx.Text = "admin123" Then
                'punta sa admin page    
                'adminpage.Show()
            Else
                MsgBox("Invalid role for this user")

            End If
        End If
    End Sub

    Private Function AuthenticateUser(qrcode As String) As Boolean
        openCon()
        Dim query As String = "SELECT * FROM logins WHERE qrcode = @qrcode"

        Using cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@qrcode", qrcode)
            Try
                Dim count As Integer = CInt(cmd.ExecuteScalar())
                Return count > 0
                con.Close()
                'SetEmpid()

            Catch ex As Exception
                MessageBox.Show("Error connecting to the database: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End Using

    End Function

    Private Function GetUserRole(qrcode As String) As String
        Dim query As String = "SELECT roles FROM logins WHERE qrcode = @qrcode"
        Using command As New MySqlCommand(query, con)
            command.Parameters.AddWithValue("@qrcode", qrcode)
            Dim result As Object = command.ExecuteScalar()
            If result IsNot Nothing Then
                Return result.ToString()
            End If
        End Using
        Return String.Empty
    End Function
End Class
