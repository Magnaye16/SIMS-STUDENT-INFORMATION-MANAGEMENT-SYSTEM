Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports AForge.Video
Imports AForge.Video.DirectShow
Imports MySql.Data.MySqlClient
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.IO
Imports Guna.UI2.WinForms

Public Class ATTENDANCE

    Dim CAMERA As VideoCaptureDevice
    Dim bmp As Bitmap

    Dim classId As Integer
    Dim classStartTime As TimeSpan
    Dim studentStatus As String
    Dim studentStatusDisplay As String
    Dim gracePeriodMinutes As Integer = 15 ' Grace period for lateness
    Dim timeInNow As TimeSpan = currentDate.TimeOfDay
    Dim attendanceType As Char

    Private STUDENT_ID As String
    Dim currentDate As DateTime = DateTime.Now
    Dim timenow As String = currentDate.ToString("HH:mm:ss") ' 24-hour format
    Dim datenow As String = currentDate.ToString("yyyy-MM-dd")

    Private Sub ATTENDANCE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Camstart() 'load camera
        'Camstart1()
    End Sub

    Private Sub Guna2ImageButton1_Click(sender As Object, e As EventArgs) Handles Guna2ImageButton1.Click
        Try
            ' Display confirmation dialog
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            ' Check the user's response
            If result = DialogResult.Yes Then
                ' Stop and release the camera properly
                If CAMERA IsNot Nothing AndAlso CAMERA.IsRunning Then
                    CAMERA.SignalToStop()
                    CAMERA.WaitForStop() ' Ensures the camera stops completely
                    CAMERA.Stop()        ' Explicitly stop the camera
                    CAMERA = Nothing     ' Release the camera object
                End If

                ' Exit the application
                Application.Exit()
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while stopping the camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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
            'Checkandtimein()
            'TimeIn()
        ElseIf Guna2TextBox4.Text = "" Then
            'clear
            Guna2TextBox1.Text = ""
            Guna2TextBox2.Text = ""
            Guna2TextBox3.Text = ""
            Guna2TextBox4.Clear()
        End If
    End Sub
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        ' Validate if any required textbox is empty
        If String.IsNullOrWhiteSpace(Guna2TextBox1.Text) OrElse
       String.IsNullOrWhiteSpace(Guna2TextBox2.Text) OrElse
       String.IsNullOrWhiteSpace(Guna2TextBox3.Text) OrElse
       String.IsNullOrWhiteSpace(Guna2TextBox4.Text) Then

            MessageBox.Show("All fields are required. Please fill out all the information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox1.Text = ""
            Guna2TextBox2.Text = ""
            Guna2TextBox3.Text = ""
            Guna2TextBox4.Clear()
            Return ' Exit the method if validation fails
        End If

        ' Proceed if all textboxes are filled
        Savepic()
        checkforpics()
        TimeIn()
    End Sub

    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged
        STUDENT_ID = Guna2TextBox4.Text.Trim
    End Sub




    'funtionssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss
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

            'Dim query As String = "SELECT s.last_name, s.first_name, s.middle_name, c.section, c.school_year " &
            '              "FROM student_info s JOIN class_info c JOIN class_members m ON s.student_id = m.student_id  " &
            '              "WHERE s.student_id = @studentId"

            Dim query As String = "SELECT s.last_name, s.first_name, s.middle_name, c.section, c.school_year " &
                      "FROM student_info s " &
                      "JOIN class_members m ON s.student_id = m.student_id " &
                      "JOIN class_info c ON m.class_id = c.class_id " &
                      "WHERE s.student_id = @studentId"

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@studentId", STUDENT_ID)

                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Guna2TextBox1.Text = $"{reader("last_name")}, {reader("first_name")} {reader("middle_name")}"
                        Guna2TextBox2.Text = reader("section").ToString()
                        Guna2TextBox3.Text = reader("school_year").ToString()
                        con.Close()
                        'TimeIn()
                        'Savepic()
                        'checkforpics()
                    Else
                        MessageBox.Show("No records found.")
                    End If
                End Using
            End Using

        Catch ex As Exception
            'MessageBox.Show("Error Searching data: " & ex.Message)

        End Try
    End Sub

    Private Sub TimeIn()

        Try
            'openCon()

            'Dim selectQuery As String = "SELECT s.student_id, c.class_id AS CID, c.time_start, c.class_day
            '                     FROM student_info s 
            '                     JOIN class_info c ON s.student_id = c.student_id 
            '                     WHERE s.student_id = @student_id"

            'Using selectCommand As New MySqlCommand(selectQuery, con)
            '    selectCommand.Parameters.AddWithValue("@student_id", STUDENT_ID)

            '    Using reader As MySqlDataReader = selectCommand.ExecuteReader()
            '        If reader.Read() Then

            '            classId = Convert.ToInt32(reader("CID"))

            '            Dim classDay As Integer = Convert.ToInt32(reader("class_day"))
            '            Dim currentDay As Integer = CInt(DateTime.Now.DayOfWeek) + 1

            '            If classDay = currentDay Then
            '                classId = Convert.ToInt32(reader("CID"))
            '                classStartTime = TimeSpan.Parse(reader("time_start").ToString())
            '                attendanceType = "C"

            '                If timeInNow <= classStartTime Then
            '                    studentStatus = "P" ' Present
            '                    studentStatusDisplay = "Present"
            '                ElseIf timeInNow <= classStartTime.Add(TimeSpan.FromMinutes(gracePeriodMinutes)) Then
            '                    studentStatus = "L" ' Late
            '                    studentStatusDisplay = "Late"
            '                Else
            '                    studentStatus = "A" ' Absent
            '                    studentStatusDisplay = "Absent"
            '                End If

            '            Else
            '                studentStatus = "P"
            '                studentStatusDisplay = "Recorded"
            '                classId = 0
            '                attendanceType = "N"
            '            End If
            '        End If
            '    End Using
            'End Using

            openCon()  ' Ensure that the connection is properly opened

            Dim selectQuery As String = "SELECT s.student_id, c.class_id AS CID, c.time_start " &
                                     "FROM student_info s " &
                                     "JOIN class_members m ON s.student_id = m.student_id " &
                                     "JOIN class_info c ON m.class_id = c.class_id " &
                                     "WHERE s.student_id = @studentId"

            Using selectCommand As New MySqlCommand(selectQuery, con)
                ' Use the correct parameter name here
                selectCommand.Parameters.AddWithValue("@studentId", STUDENT_ID)

                Using reader As MySqlDataReader = selectCommand.ExecuteReader()
                    If reader.Read() Then
                        ' Retrieve class information
                        classId = Convert.ToInt32(reader("CID"))
                        classStartTime = TimeSpan.Parse(reader("time_start").ToString())
                        attendanceType = "C"

                        ' Adjust the logic for time comparison without class_day
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
                        ' Handle case where no data is found for the student
                        studentStatus = "N" ' Not Recorded
                        studentStatusDisplay = "Not Recorded"
                        classId = 0
                        attendanceType = "N"
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

    Private Sub Checkandtimein()
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


    Public Sub checkforpics()
        If Guna2PictureBox2.Image IsNot Nothing Then
            ' Transfer the image from Guna2PictureBox2 to Guna2PictureBox3
            If Guna2PictureBox3.Image IsNot Nothing Then
                ' Dispose of the image in Guna2PictureBox3
                Guna2PictureBox3.Image.Dispose()
                Guna2PictureBox3.Image = Nothing
            End If

            ' Clone the image from Guna2PictureBox2 to Guna2PictureBox3
            Guna2PictureBox3.Image = CType(Guna2PictureBox2.Image.Clone(), Image)
        End If
        If Guna2PictureBox1.Image IsNot Nothing Then
            If Guna2PictureBox2.Image IsNot Nothing Then
                ' Dispose of the current image in Guna2PictureBox2
                Guna2PictureBox2.Image.Dispose()
            End If

            ' Clone the image from Guna2PictureBox1 to Guna2PictureBox2
            Guna2PictureBox2.Image = CType(Guna2PictureBox1.Image.Clone(), Image)
        End If
        Guna2PictureBox1.Image = Guna2PictureBox4.Image
    End Sub

    Private Sub Captured(sender As Object, eventsArgs As NewFrameEventArgs)
        bmp = DirectCast(eventsArgs.Frame.Clone(), Bitmap)
        Guna2PictureBox4.Image = DirectCast(eventsArgs.Frame.Clone(), Bitmap)
    End Sub
    Private Sub Savepic()
        'Dim filename, filepath As String
        'filename = generatefilename()
        'filepath = "C:\Users\Ericka Louise\source\repos\SIMS-STUDENT-INFORMATION-MANAGEMENT-SYSTEM\pics\" + filename + ".jpg"

        If Guna2PictureBox1.Image IsNot Nothing Then
            Dim newBitmap As Bitmap = Guna2PictureBox1.Image
            'newBitmap.Save(filepath, ImageFormat.Png)
            'Label5.Text = filepath
            Guna2PictureBox1.Image = Guna2PictureBox1.Image

        End If
        'CAMERA.SignalToStop()
        'MsgBox("picture saved")
    End Sub
    Private Function generatefilename() As String
        Return System.DateTime.Now.ToString("yyyyMMdd") + "_" + Guna2TextBox1.Text
    End Function
    Private Sub Camstart()
        ' Create a collection of video capture devices
        Dim videoDevices As New FilterInfoCollection(FilterCategory.VideoInputDevice)

        ' Check if there are any video capture devices
        If videoDevices.Count = 0 Then
            MessageBox.Show("No camera found.")
            Exit Sub
        End If

        ' Select the first available camera
        'Dim CAMERA As New VideoCaptureDevice(videoDevices(1).MonikerString)
        CAMERA = New VideoCaptureDevice(videoDevices(1).MonikerString)

        ' Attach the event handler to process new frames
        AddHandler CAMERA.NewFrame, New NewFrameEventHandler(AddressOf Captured)

        ' Start the camera
        CAMERA.Start()
    End Sub

    'Private Sub Camstart1()
    '    Dim cameras As VideoCaptureDeviceForm = New VideoCaptureDeviceForm
    '    If cameras.ShowDialog = DialogResult.OK Then
    '        CAMERA = cameras.VideoDevice
    '        AddHandler CAMERA.NewFrame, New NewFrameEventHandler(AddressOf Captured)
    '        CAMERA.Start()
    '    ElseIf cameras.ShowDialog = DialogResult.Cancel Then
    '        Me.Close()
    '    End If
    'End Sub
End Class