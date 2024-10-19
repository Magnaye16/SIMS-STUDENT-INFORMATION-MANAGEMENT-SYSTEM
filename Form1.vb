Imports MySql.Data.MySqlClient

Public Class Form1
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'Time in
        Dim currentDate As DateTime = DateTime.Now
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Try 'checks if already time in
            openCon()
            Dim query As String = "SELECT COUNT(*) FROM attendance WHERE empid = @empid AND a_date = @a_date"

            Using command As New MySqlCommand(query, con)
                'command.Parameters.AddWithValue("@empid", empid)
                'command.Parameters.AddWithValue("@a_date", datenow)

                Dim count As Integer = CInt(command.ExecuteScalar())

                If count > 0 Then
                    MessageBox.Show("You already time in.")
                    con.Close()
                    Return
                Else
                    con.Close()
                    Timein()
                End If
            End Using
        Catch ex As Exception
            ' Handle exceptions

        End Try
    End Sub





    Private Sub Timein()
        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss tt")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")

        Try
            openCon()
            Dim query As String = "INSERT INTO attendance (empid, fname, lname, a_date, t_in) VALUES (@empid, @fname, @lname, @a_date, @t_in)"
            Using command1 As New MySqlCommand(query, con)
                command1.Parameters.AddWithValue("@empid", LOGIN.Guna2TextBox2.Text)
                'command1.Parameters.AddWithValue("@fname", Label9.Text)
                'command1.Parameters.AddWithValue("@lname", Label12.Text)
                command1.Parameters.AddWithValue("@t_in", timenow)
                command1.Parameters.AddWithValue("@a_date", datenow)
                command1.ExecuteNonQuery()
                MessageBox.Show($"Time in recorded!{Environment.NewLine}Time in at: {timenow}", "Time in")

            End Using
        Catch ex As Exception
            MessageBox.Show("Error inserting employee: " & ex.Message)

        Finally
            con.Close()
        End Try
    End Sub
    Private Sub Timeout()
        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss tt")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Try
            openCon()
            Dim query As String = "UPDATE attendance " &
                      "SET t_out = @t_out " &
                      "WHERE empid = @empid and a_date = @a_date"


            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@empid", LOGIN.Guna2TextBox2.Text)
                cmd.Parameters.AddWithValue("@a_date", datenow)
                cmd.Parameters.AddWithValue("@t_out", timenow)

                cmd.ExecuteNonQuery()
                MessageBox.Show($"Time out recorded!{Environment.NewLine}Time out at: {timenow}", "Time out")

            End Using
        Catch ex As Exception
            MessageBox.Show("Error inserting Time out: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub
End Class