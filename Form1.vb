Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form1
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'Time in
        Dim currentDate As DateTime = DateTime.Now
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Try 'checks if already time in
            openCon()
            Dim query As String = "SELECT COUNT(*) FROM attendance WHERE stud_id = @stud_id AND a_date = @a_date"

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@stud_id", Guna2TextBox4.Text)
                command.Parameters.AddWithValue("@a_date", datenow)

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
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        'timeout
        Timeout()
    End Sub
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Guna2TextBox1.Clear()
        Guna2TextBox2.Clear()
        Guna2TextBox3.Clear()
        Guna2TextBox4.Clear()
    End Sub
    Private Sub Guna2TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles Guna2TextBox4.KeyDown
        'search then autofill
        If e.KeyCode = Keys.Enter Then
            SearchonPress()
        ElseIf guna2TextBox1.Text And guna2TextBox2.Text And guna2TextBox3.Text <> "" Then
            'clear
            Timein()
        End If
    End Sub




    'funtionsssss
    Public Sub SearchonPress()
        Dim txtid As String = Guna2TextBox4.Text.Trim

        Try
            openCon()
            Dim query As String = "SELECT * FROM crudStud WHERE stud_ID LIKE @searchText "

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@searchText", "%" & txtid & "%")

                ' Execute the query and read the result
                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        ' Safely concatenate the fields
                        Dim lname As String = If(IsDBNull(reader("lname")), String.Empty, reader("lname").ToString())
                        Dim gname As String = If(IsDBNull(reader("gname")), String.Empty, reader("gname").ToString())
                        Dim mname As String = If(IsDBNull(reader("mname")), String.Empty, reader("mname").ToString())

                        Guna2TextBox1.Text = lname & ", " & gname & " " & mname  ' Display lname, gname, and mname

                        Guna2TextBox2.Text = If(IsDBNull(reader("section")), String.Empty, reader("section").ToString())
                        Guna2TextBox3.Text = If(IsDBNull(reader("year")), String.Empty, reader("year").ToString())
                    Else
                        MessageBox.Show("No records found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Handle exceptions, such as database connection issues or query errors
            MessageBox.Show("Error Searching data: " & ex.Message)
        Finally
            con.Close() ' Ensure to close the connection in the finally block if it is open
        End Try
    End Sub

    Private Sub Timein()
        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss tt")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")

        Try
            openCon()
            Dim query As String = "INSERT INTO attendance (stud_id, student_name, time_in, time_out, a_date, status) VALUES (@stud_id, @student_name, @time_in, @time_out, @a_date, @status)"
            Using command1 As New MySqlCommand(query, con)
                command1.Parameters.AddWithValue("@stud_id", Guna2TextBox4.Text)
                command1.Parameters.AddWithValue("@student_name", Guna2TextBox1.Text)
                command1.Parameters.AddWithValue("@time_in", timenow)
                command1.Parameters.AddWithValue("@time_out", timenow)
                command1.Parameters.AddWithValue("@a_date", datenow)
                command1.Parameters.AddWithValue("@status", datenow) 'placeholder for now


                command1.ExecuteNonQuery()
                MessageBox.Show($"Time in recorded!{Environment.NewLine}Time in at: {timenow}", "Time in")

            End Using
        Catch ex As Exception
            MessageBox.Show("Error Time in: " & ex.Message)

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
                      "SET time_out = @time_out " &
                      "WHERE stud_id = @stud_id and a_date = @a_date"


            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@stud_id", Guna2TextBox4.Text)
                cmd.Parameters.AddWithValue("@a_date", datenow)
                cmd.Parameters.AddWithValue("@time_out", timenow)

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