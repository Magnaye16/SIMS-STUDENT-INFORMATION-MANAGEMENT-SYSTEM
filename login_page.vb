Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class login_page

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Try
            openCon()

            Using command As New MySqlCommand("SELECT * FROM userinfo WHERE username = @username  AND password = @pass", con)
                command.Parameters.Add("@username", MySqlDbType.VarChar).Value = Guna2TextBox4.Text
                command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = Guna2TextBox1.Text

                Dim adapter As New MySqlDataAdapter(command)
                Dim table As New DataTable
                adapter.Fill(table)

                If Guna2TextBox4.Text = "" Or
                   Guna2TextBox1.Text = "" Then
                    MsgBox("Please Fill All Fields!")
                ElseIf table.Rows.Count = 0 Then
                    MsgBox("Invalid username or password. Please try again.", MsgBoxStyle.Exclamation, "Login Error")
                Else
                    ' MsgBox("Login successful!", MsgBoxStyle.Information, "Success")
                    Hide()
                    Select Case table.Rows(0)("role").ToString
                        Case "Admin"
                            con.Close()
                            AdminHomepage.Show()
                        Case "professor"
                            con.Close()
                        Case "student"
                            con.Close()
                    End Select

                    Guna2TextBox1.Text = ""
                    Guna2TextBox4.Text = ""
                End If
            End Using
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message, MsgBoxStyle.Critical, "Error")
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        'send temporary password then delete after enter
    End Sub
End Class