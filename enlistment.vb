Imports System.Security.Authentication.ExtendedProtection
Imports DocumentFormat.OpenXml.Bibliography
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub Enlistment(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupTimePickers()
        PopulateDaysComboBox()
        LoadStudentClasses()
        LoadCourses()
        LoadCourseCodesIntoComboBox()
        PopulateYearComboBox()
    End Sub

    Private Sub SetupTimePickers()
        Guna2DateTimePicker1.Format = DateTimePickerFormat.Time
        Guna2DateTimePicker2.Format = DateTimePickerFormat.Time

        Guna2DateTimePicker1.ShowUpDown = True
        Guna2DateTimePicker2.ShowUpDown = True

        Guna2DateTimePicker1.Value = Guna2DateTimePicker1.Value.AddMinutes(-Guna2DateTimePicker1.Value.Minute)
        Guna2DateTimePicker2.Value = Guna2DateTimePicker2.Value.AddMinutes(-Guna2DateTimePicker2.Value.Minute)
        Guna2DateTimePicker1.Value = Guna2DateTimePicker1.Value.AddSeconds(-Guna2DateTimePicker1.Value.Second)
        Guna2DateTimePicker2.Value = Guna2DateTimePicker2.Value.AddSeconds(-Guna2DateTimePicker2.Value.Second)
    End Sub

    Private Sub PopulateYearComboBox()
        Guna2ComboBox3.Items.Clear()
        Guna2ComboBox4.Items.Clear()

        Dim startYear As Integer = DateTime.Now.Year
        Dim years As New List(Of Integer)

        For i As Integer = 0 To 10 ' Populate with 10 years starting from the current year
            years.Add(startYear + i)
        Next

        Guna2ComboBox3.DataSource = New BindingSource(years, Nothing)
        Guna2ComboBox4.DataSource = New BindingSource(years, Nothing)

        Guna2ComboBox3.SelectedIndex = -1
        Guna2ComboBox4.SelectedIndex = -1
    End Sub

    Private Sub PopulateDaysComboBox()
        ' Create a dictionary to map days to integer values
        Dim daysOfWeek As New Dictionary(Of String, Integer) From {
        {"Sunday", 1},
        {"Monday", 2},
        {"Tuesday", 3},
        {"Wednesday", 4},
        {"Thursday", 5},
        {"Friday", 6},
        {"Saturday", 7}
    }

        Guna2ComboBox5.DataSource = New BindingSource(daysOfWeek, Nothing)
        Guna2ComboBox5.DisplayMember = "Key"
        Guna2ComboBox5.ValueMember = "Value"
        Guna2ComboBox5.SelectedIndex = -1
    End Sub

    Private Sub SearchStudent(searchTerm As String)
        Dim query As String = "SELECT last_name, first_name, middle_name FROM student_info WHERE student_id LIKE @searchTerm"

        If String.IsNullOrWhiteSpace(searchTerm) Then
            Guna2TextBox1.Clear()
            Return
        End If

        Try
            openCon()
            Using command As New MySqlCommand(query, con)

                command.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()

                    If reader.Read() Then
                        Guna2TextBox1.Text = $"{reader("last_name")}, {reader("first_name")} {reader("middle_name")}"
                    End If

                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub LoadStudentClasses()
        Dim query As String = "
        SELECT 
            s.student_id, 
            s.last_name, 
            s.first_name, 
            s.middle_name,
            c.section AS Section, 
            ci.code AS Classcode
        FROM 
            class_members cm
        INNER JOIN 
            student_info s ON s.student_id = cm.student_id
        INNER JOIN 
            class_info c ON cm.class_id = c.class_id
        INNER JOIN 
            course_info ci ON c.course_id = ci.course_id;


"

        Try
            openCon()

            Dim adapter As New MySqlDataAdapter(query, con)
            Dim table As New DataTable()

            adapter.Fill(table)

            table.Columns.Add("Full Name", GetType(String))

            For Each row As DataRow In table.Rows
                row("Full Name") = $"{row("last_name")}, {row("first_name")} {row("middle_name")}"
            Next

            table.Columns("Full Name").SetOrdinal(1)
            table.Columns.Remove("last_name")
            table.Columns.Remove("first_name")
            table.Columns.Remove("middle_name")

            Guna2DataGridView1.DataSource = table
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub LoadCourseCodesIntoComboBox()
        Dim query As String = "SELECT code FROM course_info"
        Try
            openCon()
            Dim adapter As New MySqlDataAdapter(query, con)
            Dim table As New DataTable()
            adapter.Fill(table)

            Guna2ComboBox7.DisplayMember = "code"
            Guna2ComboBox7.ValueMember = "code"
            Guna2ComboBox7.DataSource = table

            Guna2ComboBox7.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub AddClassToClassInfo()
        Try
            ' Ensure the connection is open
            con.Open()

            ' Validate required fields
            If String.IsNullOrEmpty(Guna2TextBox2.Text) OrElse Guna2ComboBox7.SelectedIndex = -1 Then
                MessageBox.Show("Please ensure all required fields are filled.")
                Return
            End If

            Dim schoolYear As Integer = Convert.ToInt32(Guna2ComboBox3.SelectedItem)
            Dim section As String = Guna2TextBox2.Text
            Dim professorId As String = Guna2TextBox3.Text ' From the professor text box
            Dim courseCode As String = Guna2ComboBox7.SelectedItem.ToString()
            Dim timeStart As String = Guna2DateTimePicker1.Value.ToString("HH:mm:ss")
            Dim timeEnd As String = Guna2DateTimePicker2.Value.ToString("HH:mm:ss")
            Dim courseId As Integer

            ' Get course_id from the selected course code
            Dim courseQuery As String = "SELECT course_id FROM course_info WHERE code = @code"
            Using cmd As New MySqlCommand(courseQuery, con)
                cmd.Parameters.AddWithValue("@code", courseCode)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    courseId = CInt(result)
                Else
                    Throw New Exception("Course not found.")
                End If
            End Using

            ' Insert new class into class_info
            Dim insertClassQuery As String = "INSERT INTO class_info (school_year, section, time_start, time_end, professor_id, course_id) 
                                          VALUES (@school_year, @section, @time_start, @time_end, @professor_id, @course_id)"
            Using cmd As New MySqlCommand(insertClassQuery, con)
                cmd.Parameters.AddWithValue("@school_year", schoolYear)
                cmd.Parameters.AddWithValue("@section", section)
                cmd.Parameters.AddWithValue("@time_start", timeStart)
                cmd.Parameters.AddWithValue("@time_end", timeEnd)
                cmd.Parameters.AddWithValue("@professor_id", professorId)
                cmd.Parameters.AddWithValue("@course_id", courseId)
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Class added successfully.")

        Catch ex As MySqlException
            MessageBox.Show("Database error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub SearchProfessorById(professorId As String)
        Dim query As String = "SELECT last_name, first_name, middle_name FROM professor_info WHERE professor_id = @professorId"

        If String.IsNullOrWhiteSpace(professorId) Then
            Guna2TextBox3.Clear()
            Return
        End If

        Try
            openCon()
            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@professorId", professorId)

                Using reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        Guna2TextBox3.Text = $"{reader("last_name")}, {reader("first_name")} {reader("middle_name")}"
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub



    Private Sub LoadStudentBaseOnSearch()
        Dim searchTerm As String = Guna2TextBox4.Text.Trim()

        Dim query As String = "
        SELECT 
            s.student_id, 
            s.last_name, 
            s.first_name, 
            s.middle_name,
            c.section AS Section, 
            ci.code AS Classcode
        FROM 
            class_members cm
        INNER JOIN 
            student_info s ON s.student_id = cm.student_id
        INNER JOIN 
            class_info c ON cm.class_id = c.class_id
        INNER JOIN 
            course_info ci ON c.course_id = ci.course_id
        WHERE 
            s.student_id LIKE @searchTerm;
"

        Try
            openCon()

            Dim adapter As New MySqlDataAdapter(query, con)
            adapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Dim table As New DataTable()
            adapter.Fill(table)

            table.Columns.Add("Full Name", GetType(String))

            For Each row As DataRow In table.Rows
                row("Full Name") = $"{row("last_name")}, {row("first_name")} {row("middle_name")}"
            Next

            table.Columns("Full Name").SetOrdinal(1)

            table.Columns.Remove("last_name")
            table.Columns.Remove("first_name")
            table.Columns.Remove("middle_name")

            Guna2DataGridView1.DataSource = table

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub LoadProfBaseOnSearch()
        Dim searchTerm As String = Guna2TextBox3.Text.Trim()

        Dim query As String = "
        SELECT s.professor_id, s.last_name, s.first_name, s.middle_name,
               c.section AS Section, ci.code AS Classcode
        FROM class_info c
        INNER JOIN professor_info s ON s.professor_id = c.professor_id
        INNER JOIN course_info ci ON ci.course_id = c.course_id
        WHERE s.professor_id LIKE @searchTerm"

        Try
            openCon()

            Dim adapter As New MySqlDataAdapter(query, con)
            adapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

            Dim table As New DataTable()
            adapter.Fill(table)

            table.Columns.Add("Full Name", GetType(String))

            For Each row As DataRow In table.Rows
                row("Full Name") = $"{row("last_name")}, {row("first_name")} {row("middle_name")}"
            Next

            table.Columns("Full Name").SetOrdinal(1)

            table.Columns.Remove("last_name")
            table.Columns.Remove("first_name")
            table.Columns.Remove("middle_name")

            Guna2DataGridView1.DataSource = table

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub LoadCourses()
        Dim query As String = "SELECT course_id, code FROM course_info"
        Try
            openCon()
            Dim adapter As New MySqlDataAdapter(query, con)
            Dim table As New DataTable()
            adapter.Fill(table)


            Guna2ComboBox1.DisplayMember = "code"
            Guna2ComboBox1.ValueMember = "course_id"
            Guna2ComboBox1.DataSource = table

            Guna2ComboBox1.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub AddClassToStudent()
        Try
            ' Ensure the connection is open
            con.Open()

            ' Validate required fields
            If String.IsNullOrEmpty(Guna2TextBox2.Text) OrElse Guna2ComboBox1.SelectedIndex = -1 Then
                MessageBox.Show("Please ensure all required fields are filled.")
                Return
            End If

            Dim courseCode As String = Guna2ComboBox1.Text
            Dim section As String = Guna2TextBox2.Text
            Dim studentId As String = Guna2TextBox4.Text.Trim()
            Dim courseId As Integer
            Dim classId As Integer

            ' Step 1: Get course_id from the course_info table
            Dim courseQuery As String = "SELECT course_id FROM course_info WHERE code = @code"
            Using cmd As New MySqlCommand(courseQuery, con)
                cmd.Parameters.AddWithValue("@code", courseCode)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    courseId = CInt(result)
                Else
                    Throw New Exception("Course not found")
                End If
            End Using

            ' Step 2: Check if the combination of course_id and section exists in class_info
            Dim checkClassQuery As String = "SELECT class_id FROM class_info WHERE course_id = @course_id AND section = @section"
            Using cmd As New MySqlCommand(checkClassQuery, con)
                cmd.Parameters.AddWithValue("@course_id", courseId)
                cmd.Parameters.AddWithValue("@section", section)
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    classId = CInt(result)
                Else
                    ' If class does not exist, insert a new entry into class_info
                    Dim insertClassQuery As String = "INSERT INTO class_info (course_id, section) VALUES (@course_id, @section)"
                    Using cm As New MySqlCommand(insertClassQuery, con)
                        cm.Parameters.AddWithValue("@course_id", courseId)
                        cm.Parameters.AddWithValue("@section", section)
                        cm.ExecuteNonQuery()
                        ' Get the last inserted class_id
                        classId = CInt(New MySqlCommand("SELECT LAST_INSERT_ID()", con).ExecuteScalar())
                    End Using
                End If
            End Using

            ' Step 3: Check if the student is already enrolled in the class
            Dim checkEnrollmentQuery As String = "SELECT COUNT(*) FROM class_members WHERE student_id = @student_id AND class_id = @class_id"
            Using cmd As New MySqlCommand(checkEnrollmentQuery, con)
                cmd.Parameters.AddWithValue("@student_id", studentId)
                cmd.Parameters.AddWithValue("@class_id", classId)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("Student is already enrolled in this class.")
                    Return ' Exit if student is already enrolled
                End If
            End Using

            ' Step 4: Insert student_id and class_id into class_members if not already enrolled
            Dim insertMemberQuery As String = "INSERT INTO class_members (student_id, class_id) VALUES (@student_id, @class_id)"
            Using cmd As New MySqlCommand(insertMemberQuery, con)
                cmd.Parameters.AddWithValue("@student_id", studentId)
                cmd.Parameters.AddWithValue("@class_id", classId)
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Student added to the class successfully.")

        Catch ex As MySqlException
            MessageBox.Show("Database error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Close connection if open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub



    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged
        SearchStudent(Guna2TextBox4.Text)
        LoadStudentBaseOnSearch
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox3.SelectedIndexChanged
        Dim selectedYear As Integer = Convert.ToInt32(Guna2ComboBox3.SelectedItem)

        Dim nextYear As Integer = selectedYear + 1

        If Guna2ComboBox4.Items.Contains(nextYear) Then
            Guna2ComboBox4.SelectedItem = nextYear
        End If
    End Sub

    Private Function IsStudentExisting(studentNumber As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM student_info WHERE student_id = @student_id"
        Try
            openCon()

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@student_id", studentNumber)

            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

            Return count > 0
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
            Return False
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Function

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Dim studentNumber = Guna2TextBox4.Text.Trim

        If String.IsNullOrEmpty(studentNumber) Then
            MessageBox.Show("Please enter a valid student number.")
            Return
        End If

        If IsStudentExisting(studentNumber) Then
            AddClassToSTudent()
        Else
            MessageBox.Show("Student does not exist.")
        End If
    End Sub

    Private Sub Guna2TextBox5_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox5.TextChanged
        SearchProfessorById(Guna2TextBox3.Text)
    End Sub
End Class