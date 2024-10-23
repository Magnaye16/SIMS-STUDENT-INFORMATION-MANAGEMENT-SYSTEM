Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form1

    Private STUDENT_NUMBER As String

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'Time in
        Dim currentDate As DateTime = DateTime.Now
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Dim studentId As Integer
        Dim studentNumber As String = Guna2TextBox4.Text ' Assuming the student number is input by the user

        ' Check if the student number is provided
        If String.IsNullOrEmpty(studentNumber) Then
            MessageBox.Show("Please enter a valid student number.")
            Return
        End If

        Try
            ' Open the connection
            openCon()

            ' First, retrieve the student_id based on the student_number
            Dim selectQuery As String = "SELECT student_id FROM student_info WHERE student_number = @student_number"

            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_number", studentNumber)

                ' Execute the reader to fetch the student_id
                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        studentId = Convert.ToInt32(reader("student_id"))
                    Else
                        ' If the student number does not exist
                        MessageBox.Show("Student number does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return ' Exit the function as the student number is invalid
                    End If
                End Using
            End Using

            ' Now that we have the student_id, check if the student has already timed in
            Dim query As String = "SELECT COUNT(*) FROM attendance_log WHERE student_id = @student_id AND log_date = @log_date"

            Using command As New MySqlCommand(query, con)
                ' Add parameters for student_id and log_date
                command.Parameters.AddWithValue("@student_id", studentId) ' Use the retrieved student_id
                command.Parameters.AddWithValue("@log_date", datenow)

                ' Execute the scalar query to check if there is already a time-in record
                Dim count As Integer = CInt(command.ExecuteScalar())

                If count > 0 Then
                    ' If a record exists, notify the user
                    MessageBox.Show("You have already timed in for today.", "Time-in Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return ' Exit the function to prevent another time-in
                Else
                    ' If no record exists, call the Timein function or insert the time-in record
                    con.Close()
                    ' Call the function to log time-in here
                    Timein()
                End If
            End Using

        Catch ex As Exception
            ' Handle exceptions by showing the error message
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            ' Ensure the connection is closed in the Finally block to handle all cases
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        'Timein()
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
    End Sub

    Private Sub Timein()
        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")

        Dim studentId As Integer
        Dim classId As Integer
        Dim studentNumber As String = Guna2TextBox4.Text ' Assume the student number is entered in a TextBox

        ' Check if the student number is provided
        If String.IsNullOrEmpty(studentNumber) Then
            MessageBox.Show("Please enter a valid student number.")
            Return
        End If

        Try
            openCon() ' Assuming this opens the database connection

            ' SQL query to select student and class ID, updated with the correct column name
            Dim selectQuery As String = "SELECT s.student_id, c.class_id 
                                   FROM student_info s 
                                   INNER JOIN class_info c ON s.student_id = c.student_id
                                   WHERE s.student_number = @student_number" ' Use the correct column name here

            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_number", studentNumber)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        studentId = Convert.ToInt32(reader("student_id"))
                        classId = Convert.ToInt32(reader("class_id"))
                    Else
                        ' Notify user if student is not found
                        MessageBox.Show("Student number does not exist.")
                        Return
                    End If
                End Using
            End Using

            ' Insert the time-in log into the attendance_log table
            Dim insertQuery As String = "INSERT INTO attendance_log (log_date, time_in, time_out, status, student_id, class_id) 
                                   VALUES (@log_date, @time_in, @time_out, @status, @student_id, @class_id)"

            Using insertCommand As New MySqlCommand(insertQuery, con)
                insertCommand.Parameters.AddWithValue("@log_date", datenow)
                insertCommand.Parameters.AddWithValue("@time_in", timenow)
                insertCommand.Parameters.AddWithValue("@time_out", DBNull.Value) ' Placeholder for time_out, assuming they will check out later
                insertCommand.Parameters.AddWithValue("@status", "P") ' "P" for Present. You can adjust this later.
                insertCommand.Parameters.AddWithValue("@student_id", studentId)
                insertCommand.Parameters.AddWithValue("@class_id", classId)

                ' Execute the insert query
                Dim rowsAffected As Integer = insertCommand.ExecuteNonQuery()
                MessageBox.Show($"Time in recorded!{Environment.NewLine}Time in at: {timenow}", "Time in")
            End Using

        Catch ex As Exception
            ' Handle errors
            MessageBox.Show("Error during time in: " & ex.Message)

        Finally
            ' Ensure connection is closed
            con.Close()
        End Try
    End Sub


    Private Sub Timeout()

        Dim currentDate As DateTime = DateTime.Now
        Dim timenow As String = currentDate.ToString("hh:mm:ss")
        Dim datenow As String = currentDate.ToString("MMMM dd, yyyy")
        Dim studentId As Integer

        Try
            openCon()

            Dim selectQuery As String = "SELECT s.student_id, s.student_number FROM student_info s WHERE s.student_number = @student_number"
            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_number", STUDENT_NUMBER)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        studentId = Convert.ToInt32(reader("student_id"))
                    Else
                        Console.WriteLine("Student number: " & studentId & " does not exist.")
                        Return
                    End If
                End Using
            End Using

            Dim updateQuery As String = "UPDATE attendance_log SET time_out = @time_out WHERE student_id = @student_id" 'add more to where statements to specify
            Using updateCommand As New MySqlCommand(updateQuery, con)
                updateCommand.Parameters.AddWithValue("@student_id", studentId)
                updateCommand.Parameters.AddWithValue("@time_out", timenow)

                Dim rowsAffected As Integer = updateCommand.ExecuteNonQuery()
                MessageBox.Show($"Time out recorded!{Environment.NewLine}Time out at: {timenow}", "Time out")
            End Using

        Catch ex As Exception
            MessageBox.Show("Error inserting Time out: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged
        STUDENT_NUMBER = Guna2TextBox4.Text.Trim
    End Sub
End Class