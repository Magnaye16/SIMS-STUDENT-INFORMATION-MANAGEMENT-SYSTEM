Imports Guna.UI2.WinForms
Imports Microsoft.VisualBasic.Logging
Imports MySql.Data.MySqlClient

Public Class LOGIN
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        uid_txtbx.Clear()
        studname_txtbx.Clear()
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        'Dim idcode As String = uid_txtbx.Text
        'If AuthenticateUser(idcode) Then
        '    Dim userRole As String = GetUserRole(idcode)
        '    If userRole = "emp" Then
        '        con.Close()
        '        'display empid in txtbox in login form
        '        'SetEmpid()
        '        'LOGIN.Hide()
        '        Me.Hide()
        '        'studpage.Show()

        '    ElseIf uid_txtbx.Text = "admin123" Then
        '        'punta sa admin page    
        '        'adminpage.Show()
        '    Else
        '        MsgBox("Invalid role for this user")

        '    End If
        'End If

        Try
            openCon()

            Using command As New MySqlCommand("SELECT * FROM user WHERE pword = @pass", con)
                command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = uid_txtbx.Text

                Dim adapter As New MySqlDataAdapter(command)
                Dim table As New DataTable
                adapter.Fill(table)

                If uid_txtbx.Text = "" Then
                    MsgBox("Please Fill UID Field!")
                ElseIf table.Rows.Count = 0 Then
                    MsgBox("Invalid username. Please try again.", MsgBoxStyle.Exclamation, "Login Error")
                Else
                    ' MsgBox("Login successful!", MsgBoxStyle.Information, "Success")
                    Hide()
                    'Dim imageData As Byte() = DirectCast(table.Rows(0)("picture"), Byte())

                    Select Case table.Rows(0)("role").ToString
                        'Case "Admin"
                        '    con.Close()
                        '    ADMIN_Homepage.Show()
                        'Case "Member"
                        '    con.Close()
                        '    MEM_HOMEPAGE.memID = table.Rows(0)("uid").ToString
                        '    MEM_HOMEPAGE.Show()
                        'Case "Organizer"
                        '    con.Close()
                        '    ORG_HOMEPAGE.orgID = table.Rows(0)("uid").ToString
                        '    ORG_HOMEPAGE.Show()

                    End Select

                    'Guna2TextBox1.Text = ""
                    'Guna2TextBox2.Text = ""
                End If
            End Using
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message, MsgBoxStyle.Critical, "Error")
        Finally
            con.Close()
        End Try



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
