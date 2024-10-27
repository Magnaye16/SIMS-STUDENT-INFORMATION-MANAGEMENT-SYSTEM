Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form1

    Private STUDENT_ID As String
    Dim currentDate As DateTime = DateTime.Now
    Dim timenow As String = currentDate.ToString("HH:mm:ss") ' 24-hour format
    Dim datenow As String = currentDate.ToString("yyyy-MM-dd")

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        'Timein()
    End Sub
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        'timeout
        Timeout()
    End Sub
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Guna2TextBox1.Text = ""
        Guna2TextBox2.Text = ""
        Guna2TextBox3.Text = ""
        Guna2TextBox4.Clear()
    End Sub
    Private Sub Guna2TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles Guna2TextBox4.KeyDown
        'search then autofill
        If e.KeyCode = Keys.Enter Then
            SearchonPress()
            'checkandtimein()
            'TimeIn()
        ElseIf Guna2TextBox4.Text = "" Then
            'clear
            Guna2TextBox1.Text = ""
            Guna2TextBox2.Text = ""
            Guna2TextBox3.Text = ""
            Guna2TextBox4.Clear()
        End If
    End Sub
    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged
        STUDENT_ID = Guna2TextBox4.Text.Trim
    End Sub

    Public Sub SearchonPress()

        If String.IsNullOrEmpty(STUDENT_ID) Then
            MessageBox.Show("Please enter a valid student number.")
            Return
        End If

        Try

            openCon()

            Dim checkQuery As String = "SELECT COUNT(*) FROM student_info WHERE student_id = @studentId"
            Using checkCommand As New MySqlCommand(checkQuery, con)
                checkCommand.Parameters.AddWithValue("@studentId", STUDENT_ID)
                Dim exists As Integer = Convert.ToInt32(checkCommand.ExecuteScalar())
                If exists = 0 Then
                    MessageBox.Show("No records found.")
                    Return
                End If
            End Using

            Dim query As String = "SELECT s.last_name, s.first_name, s.middle_name, c.section, c.year " &
                          "FROM student_info s JOIN class_info c ON s.student_id = c.student_id " &
                          "WHERE s.student_id = @studentId"

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@studentId", STUDENT_ID)

                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Guna2TextBox1.Text = $"{reader("last_name")}, {reader("first_name")} {reader("middle_name")}"
                        Guna2TextBox2.Text = reader("section").ToString()
                        Guna2TextBox3.Text = reader("year").ToString()
                        con.Close()
                        TimeIn()
                    Else
                        MessageBox.Show("No records found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error Searching data: " & ex.Message)
        End Try


        'If String.IsNullOrEmpty(STUDENT_ID) Then
        '    MessageBox.Show("Please enter a valid student number.")
        '    Return
        'End If

        'Try
        '    openCon()

        '    Dim query As String = "SELECT s.*, c.* FROM student_info s JOIN class_info c ON s.class_id = c.class_id WHERE s.student_id LIKE @searchText"

        '    Using command As New MySqlCommand(query, con)
        '        command.Parameters.AddWithValue("@searchText", "%" & STUDENT_ID & "%")

        '        Using reader As MySqlDataReader = command.ExecuteReader()
        '            If reader.Read() Then

        '                Dim lname As String = If(IsDBNull(reader("last_name")), String.Empty, reader("last_name").ToString())
        '                Dim gname As String = If(IsDBNull(reader("first_name")), String.Empty, reader("first_name").ToString())
        '                Dim mname As String = If(IsDBNull(reader("middle_name")), String.Empty, reader("middle_name").ToString())

        '                Guna2TextBox1.Text = lname & ", " & gname & " " & mname

        '                Guna2TextBox2.Text = If(IsDBNull(reader("section")), String.Empty, reader("section").ToString())
        '                Guna2TextBox3.Text = If(IsDBNull(reader("year")), String.Empty, reader("year").ToString())
        '            Else
        '                MessageBox.Show("No records found.")
        '            End If
        '        End Using
        '    End Using

        'Catch ex As Exception
        '    MessageBox.Show("Error Searching data: " & ex.Message)
        'Finally
        '    If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
        '        con.Close()
        '    End If
        'End Try
    End Sub

    Private Sub TimeIn()
        Dim classId As Integer
        Dim classStartTime As TimeSpan
        Dim studentStatus As String
        Dim studentStatusDisplay As String
        Dim gracePeriodMinutes As Integer = 15 ' Grace period for lateness
        Dim timeInNow As TimeSpan = currentDate.TimeOfDay
        Dim attendanceType As Char

        Try
            openCon()

            Dim selectQuery As String = "SELECT s.student_id, c.class_id AS CID, c.time_start, c.class_day
                                 FROM student_info s 
                                 JOIN class_info c ON s.student_id = c.student_id 
                                 WHERE s.student_id = @student_id"

            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_id", STUDENT_ID)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then

                        classId = Convert.ToInt32(reader("CID"))

                        Dim classDay As Integer = Convert.ToInt32(reader("class_day"))
                        Dim currentDay As Integer = CInt(DateTime.Now.DayOfWeek) + 1

                        If classDay = currentDay Then
                            classId = Convert.ToInt32(reader("CID"))
                            classStartTime = TimeSpan.Parse(reader("time_start").ToString())
                            attendanceType = "C"

                            If timeInNow <= classStartTime Then
                                studentStatus = "P" ' Present
                                studentStatusDisplay = "Present"
                            ElseIf timeInNow <= classStartTime.Add(TimeSpan.FromMinutes(gracePeriodMinutes)) Then
                                studentStatus = "L" ' Late
                                studentStatusDisplay = "Late"
                            Else
                                studentStatus = "A" ' Absent
                                studentStatusDisplay = "Absent"
                            End If

                        Else
                            studentStatus = "P"
                            studentStatusDisplay = "Recorded"
                            classId = 0
                            attendanceType = "N"
                        End If
                    End If
                End Using
            End Using

            Dim checkQquery As String = "SELECT COUNT(*) FROM attendance_log WHERE student_id = @student_id AND log_date = @log_date"

            Using command As New MySqlCommand(checkQquery, con)
                ' Add parameters for student_id and log_date
                command.Parameters.AddWithValue("@student_id", STUDENT_ID) ' Use the retrieved student_id
                command.Parameters.AddWithValue("@log_date", datenow)

                ' Execute the scalar query to check if there is already a time-in record
                Dim count As Integer = CInt(command.ExecuteScalar())

                If count > 0 Then

                    MessageBox.Show("You're present today!'.", "Time-in Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                Else
                    Dim insertQuery As String = "INSERT INTO attendance_log (log_date, time_in, time_out, status, attendance_type, student_id, class_id) 
                                         VALUES (@log_date, @time_in, @time_out, @status, @attendance_type, @student_id, @class_id)"

                    Using insertCommand As New MySqlCommand(insertQuery, con)
                        insertCommand.Parameters.AddWithValue("@log_date", datenow)
                        insertCommand.Parameters.AddWithValue("@time_in", timenow)
                        insertCommand.Parameters.AddWithValue("@time_out", DBNull.Value) ' Placeholder for time_out
                        insertCommand.Parameters.AddWithValue("@status", studentStatus)
                        insertCommand.Parameters.AddWithValue("@attendance_type", attendanceType)
                        insertCommand.Parameters.AddWithValue("@student_id", STUDENT_ID)
                        insertCommand.Parameters.AddWithValue("@class_id", classId)

                        Dim rowsAffected As Integer = insertCommand.ExecuteNonQuery()
                        MessageBox.Show($"Recorded!{Environment.NewLine}Time in at: {timenow}{Environment.NewLine}Status: {studentStatusDisplay}", "Time in")
                    End Using
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error during time in: " & ex.Message)
        Finally
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Timeout()

        Try
            openCon()

            ' Retrieve student_id based on STUDENT_NUMBER (assuming it's available)
            Dim selectQuery As String = "SELECT student_id FROM student_info WHERE student_id = @student_id"
            Using selectCommand As New MySqlCommand(selectQuery, con)
                selectCommand.Parameters.AddWithValue("@student_id", STUDENT_ID) ' Assuming STUDENT_NUMBER is a known variable

                ' Execute the query and retrieve student_id
                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        'studentId = Convert.ToInt32(reader("student_id")) ' Retrieve and assign studentId
                    Else
                        MessageBox.Show("Student ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return ' Exit if the student_id is not found
                    End If
                End Using
            End Using

            ' Update attendance_log to set time_out for the retrieved student_id
            Dim updateQuery As String = "UPDATE attendance_log SET time_out = @time_out WHERE student_id = @student_id AND log_date = @log_date AND time_out IS NULL"
            Using updateCommand As New MySqlCommand(updateQuery, con)
                updateCommand.Parameters.AddWithValue("@student_id", STUDENT_ID) ' Use the retrieved studentId
                updateCommand.Parameters.AddWithValue("@time_out", timenow) ' Correctly formatted time
                updateCommand.Parameters.AddWithValue("@log_date", datenow) ' Correctly formatted date

                Dim rowsAffected As Integer = updateCommand.ExecuteNonQuery()
                If rowsAffected > 0 Then
                    MessageBox.Show($"Time out recorded!{Environment.NewLine}Time out at: {timenow}", "Time out")
                Else
                    MessageBox.Show("No matching record found to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error inserting Time out: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub checkandtimein()
        Try
            ' Open the database connection
            openCon()

            ' Query to retrieve the student_id based on the student_number
            Dim selectQuery As String = "SELECT student_id FROM student_info WHERE student_id = @student_id"

            Using selectCommand As New MySqlCommand(selectQuery, con)
                ' Add the student_number parameter
                selectCommand.Parameters.AddWithValue("@student_id", STUDENT_ID)
            End Using

            ' Now that we have the student_id, check if the student has already timed in
            Dim query As String = "SELECT COUNT(*) FROM attendance_log WHERE student_id = @student_id AND log_date = @log_date"

            Using command As New MySqlCommand(query, con)
                ' Add parameters for student_id and log_date
                command.Parameters.AddWithValue("@student_id", STUDENT_ID) ' Use the retrieved student_id
                command.Parameters.AddWithValue("@log_date", datenow)

                ' Execute the scalar query to check if there is already a time-in record
                Dim count As Integer = CInt(command.ExecuteScalar())

                If count > 0 Then
                    ' If a record exists, notify the user
                    MessageBox.Show("You're present today!'.", "Time-in Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return ' Exit the function to prevent another time-in
                Else
                    ' If no record exists, proceed to log time-in
                    con.Close()
                    TimeIn() ' Call your time-in function here
                End If
            End Using

        Catch ex As Exception
            ' Handle exceptions and show the error message
            MessageBox.Show("An error occurred in checking: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            ' Ensure the connection is closed in the Finally block to handle all cases
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub
End Class