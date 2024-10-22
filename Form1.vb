Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form1

    Private STUDENT_NUMBER As String

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'Time in
        Dim currentDate As DateTime = DateTime.Now
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Try 'checks if already time in
            openCon()
            Dim query As String = "SELECT COUNT(*) FROM attendance_log WHERE student_number = @student_number AND log_date = @log_date"

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@student_number ", Guna2TextBox4.Text)
                command.Parameters.AddWithValue("@log_date", datenow)

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
        ElseIf Guna2TextBox4.Text = "" Then
            'clear
            'Timein()
        End If
    End Sub




    'funtionsssss
    Public Sub SearchonPress()
        Dim txtid As String = Guna2TextBox4.Text.Trim()

        Try
            openCon()

            Dim query As String = "SELECT s.*, c.* FROM student_info s JOIN class_info c ON c.class_id = c.class_id WHERE s.student_number LIKE @searchText"

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@searchText", "%" & txtid & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then

                        Dim lname As String = If(IsDBNull(reader("last_name")), String.Empty, reader("last_name").ToString())
                        Dim gname As String = If(IsDBNull(reader("first_name")), String.Empty, reader("first_name").ToString())
                        Dim mname As String = If(IsDBNull(reader("middle_name")), String.Empty, reader("middle_name").ToString())

                        Guna2TextBox1.Text = lname & ", " & gname & " " & mname

                        Guna2TextBox2.Text = If(IsDBNull(reader("section")), String.Empty, reader("section").ToString())
                        Guna2TextBox3.Text = If(IsDBNull(reader("year")), String.Empty, reader("year").ToString())
                    Else
                        MessageBox.Show("No records found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error Searching data: " & ex.Message)
        Finally
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try



        'Dim txtid As String = Guna2TextBox4.Text.Trim

        'Try
        '    openCon()
        '    Dim query As String = "SELECT s.*, c.* FROM student_info s JOIN class_info c WHERE s.student_number LIKE @searchText "

        '    Using command As New MySqlCommand(query, con)
        '        command.Parameters.AddWithValue("@searchText", "%" & txtid & "%")

        '        ' Execute the query and read the result
        '        Using reader As MySqlDataReader = command.ExecuteReader()
        '            If reader.Read() Then
        '                ' Safely concatenate the fields
        '                Dim lname As String = If(IsDBNull(reader("last_name")), String.Empty, reader("last_name").ToString())
        '                Dim gname As String = If(IsDBNull(reader("first_name")), String.Empty, reader("first_name").ToString())
        '                Dim mname As String = If(IsDBNull(reader("middle_name")), String.Empty, reader("middle_name").ToString())

        '                Guna2TextBox1.Text = lname & ", " & gname & " " & mname  ' Display lname, gname, and mname

        '                Guna2TextBox2.Text = If(IsDBNull(reader("section")), String.Empty, reader("section").ToString())
        '                Guna2TextBox3.Text = If(IsDBNull(reader("year")), String.Empty, reader("year").ToString())
        '            Else
        '                MessageBox.Show("No records found.")
        '            End If
        '        End Using
        '    End Using

        'Catch ex As Exception
        '    ' Handle exceptions, such as database connection issues or query errors
        '    MessageBox.Show("Error Searching data: " & ex.Message)
        'Finally
        '    con.Close() ' Ensure to close the connection in the finally block if it is open
        'End Try
    End Sub

    Private Sub Timein()
        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss tt")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")

        Dim studentId As Integer
        Dim classId As Integer

        Try
            openCon()

            Dim selectQuery As String = "SELECT s.*, c.class_id, c.student_id FROM student_info s INNER JOIN class_info c
                                     WHERE s.student_number = @student_number AND s.student_id = c.student_id"
            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_number", STUDENT_NUMBER)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        studentId = Convert.ToInt32(reader("student_id"))
                        classId = Convert.ToInt32(reader("class_id"))
                    Else
                        Console.WriteLine("Student number: " & studentId & " does not exist.")
                        Return
                    End If
                End Using
            End Using

            Dim insertQuery As String = "INSERT INTO attendance_log (log_date, time_in, time_out, status, student_id, class_id) 
                                     VALUES (@log_date, @time_in, @time_out, @status, @student_id, @class_id)"
            Using insertCommand As New MySqlCommand(insertQuery, con)
                insertCommand.Parameters.AddWithValue("@student_id", studentId)
                insertCommand.Parameters.AddWithValue("@class_id", classId)
                insertCommand.Parameters.AddWithValue("@log_date", datenow)
                insertCommand.Parameters.AddWithValue("@time_in", timenow)
                insertCommand.Parameters.AddWithValue("@time_out", timenow)
                insertCommand.Parameters.AddWithValue("@status", "P") 'place holder, create if condition for present, late, and absent

                Dim rowsAffected As Integer = insertCommand.ExecuteNonQuery()
                MessageBox.Show($"Time in recorded!{Environment.NewLine}Time in at: {timenow}", "Time in")
            End Using


            'openCon()
            '    Dim query As String = "INSERT INTO attendance_log (log_date, time_in, time_out, status, student_id, class_id) VALUES (@log_date, @time_in, @time_out, @status, @student_id, @class_id)"
            '    Using command1 As New MySqlCommand(query, con)
            '        command1.Parameters.AddWithValue("@student_i", Guna2TextBox4.Text)
            '        command1.Parameters.AddWithValue("@student_name", Guna2TextBox1.Text)
            '        command1.Parameters.AddWithValue("@time_in", timenow)
            '        command1.Parameters.AddWithValue("@time_out", timenow)
            '        command1.Parameters.AddWithValue("@a_date", datenow)
            '        command1.Parameters.AddWithValue("@status", datenow) 'placeholder for now


            '        command1.ExecuteNonQuery()
            '        MessageBox.Show($"Time in recorded!{Environment.NewLine}Time in at: {timenow}", "Time in")

            '    End Using

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
        Dim studentId As Integer

        Try
            openCon()

            Dim selectQuery As String = "SELECT s.student_id, s.student_number FROM student_info s WHERE s.student_number = @student_number"
            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_number", STUDENT_NUMBER)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        studentId = Convert.ToInt32(reader("studentId"))
                    Else
                        Console.WriteLine("Student number: " & studentId & " does not exist.")
                        Return
                    End If
                End Using
            End Using

            Dim updateQuery As String = "UPDATE attendce_log SET time_out = @time_out WHERE student_id = @student_id" 'add more to where statements to specify
            Using updateCommand As New MySqlCommand(updateQuery, con)
                updateCommand.Parameters.AddWithValue("@student_id", studentId)
                updateCommand.Parameters.AddWithValue("@time_out", timenow)

                Dim rowsAffected As Integer = updateCommand.ExecuteNonQuery()
                MessageBox.Show($"Time out recorded!{Environment.NewLine}Time out at: {timenow}", "Time out")
            End Using


            'Dim currentDate As DateTime = DateTime.Now
            'Dim timenow As String = currentDate.ToString("hh:mm:ss tt")
            'Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
            'Try
            '    openCon()
            '    Dim query As String = "UPDATE attendance" &
            '              "SET time_out = @time_out " &
            '              "WHERE stud_id = @stud_id and a_date = @a_date"


            '    Using cmd As New MySqlCommand(query, con)
            '        cmd.Parameters.AddWithValue("@stud_id", Guna2TextBox4.Text)
            '        cmd.Parameters.AddWithValue("@a_date", datenow)
            '        cmd.Parameters.AddWithValue("@time_out", timenow)

            '        cmd.ExecuteNonQuery()
            '        MessageBox.Show($"Time out recorded!{Environment.NewLine}Time out at: {timenow}", "Time out")

            '    End Using

        Catch ex As Exception
            MessageBox.Show("Error inserting Time out: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged
        STUDENT_NUMBER = Guna2TextBox4.Text
    End Sub
End Class