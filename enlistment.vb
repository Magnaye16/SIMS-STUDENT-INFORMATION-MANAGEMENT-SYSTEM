Imports System.Security.Authentication.ExtendedProtection
Imports DocumentFormat.OpenXml.Bibliography
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub LoadClassInfo()
        Dim query As String = "
    SELECT 
        ci.class_id, 
        ci.school_year, 
        ci.section, 
        ci.time_start, 
        ci.time_end, 
        co.code AS CourseCode, 
        CONCAT(pi.last_name, ', ', pi.first_name, ' ', pi.middle_name) AS ProfessorName
    FROM 
        class_info ci
    INNER JOIN 
        course_info co ON ci.course_id = co.course_id
    INNER JOIN 
        professor_info pi ON ci.professor_id = pi.professor_id"

        Try
            ' Open the connection
            openCon()

            ' Execute the query and fill the DataTable
            Dim adapter As New MySqlDataAdapter(query, con)
            Dim table As New DataTable()

            adapter.Fill(table)

            ' Set the DataGridView data source to the table
            Guna2DataGridView2.DataSource = table

        Catch ex As Exception
            ' Handle any errors that occur
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            ' Ensure the connection is closed
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub



    Private Sub CapitalizeFirstLetter(sender As Object, e As EventArgs) Handles _
    Guna2TextBox1.TextChanged, Guna2TextBox2.TextChanged, Guna2TextBox3.TextChanged


        Dim textBox As Guna.UI2.WinForms.Guna2TextBox = CType(sender, Guna.UI2.WinForms.Guna2TextBox)

        ' Capitalize the first letter of each word in the text
        textBox.Text = CapitalizeWords(textBox.Text)

        ' Move the cursor to the end of the text after capitalization
        textBox.SelectionStart = textBox.Text.Length
    End Sub

    ' Function to capitalize the first letter of each word in a string
    Private Function CapitalizeWords(input As String) As String
        ' Split the input string into words
        Dim words As String() = input.Split(" "c)

        ' Capitalize each word
        For i As Integer = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1).ToLower()
            End If
        Next

        ' Join the words back into a single string and return it
        Return String.Join(" ", words)
    End Function
    Private Sub Guna2TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox4.KeyPress
        ' Allow digits (0-9), hyphen (-), and control keys (like Backspace)
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "-"c AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True ' Cancel the key press if it's not a number, hyphen, or control key
        End If

        ' Ensure that only one hyphen can be typed (if required)
        ' Optionally, ensure the hyphen can only be typed at the beginning of the input
        If e.KeyChar = "-"c Then
            ' If there is already a hyphen or the hyphen is not at the start, cancel the input
            If Guna2TextBox4.Text.Contains("-") OrElse Guna2TextBox4.SelectionStart > 0 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Guna2TextBox5_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox5.KeyPress
        ' Allow digits (0-9), hyphen (-), and control keys (like Backspace)
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "-"c AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True ' Cancel the key press if it's not a number, hyphen, or control key
        End If

        If e.KeyChar = "-"c Then
            ' If there is already a hyphen or the hyphen is not at the start, cancel the input
            If Guna2TextBox5.Text.Contains("-") OrElse Guna2TextBox5.SelectionStart > 0 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Guna2Button9_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        TabControl1.SelectedTab = TabPage1
    End Sub
    Private Sub Enlistment(sender As Object, e As EventArgs) Handles MyBase.Load
        TabControl1.SelectedTab = TabPage1

        SetupTimePickers()
        PopulateDaysComboBox()
        LoadStudentClasses()
        LoadCourses()
        LoadCourseCodesIntoComboBox()
        PopulateYearComboBox()
        LoadClassInfo()
        LoadSectionsToComboBox()
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

            Dim schoolYear As String = Guna2ComboBox3.SelectedItem?.ToString() & "-" & Guna2ComboBox4.SelectedItem?.ToString() ' School year selected from ComboBox
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

    Private Sub ClearForm()
        ' Clear all ComboBoxes
        Guna2ComboBox7.SelectedIndex = -1  ' Clears the selected item in ComboBox7 (course)
        Guna2ComboBox5.SelectedIndex = -1  ' Clears the selected item in ComboBox5 (day)
        Guna2ComboBox6.SelectedIndex = -1  ' Clears the selected item in ComboBox6 (section)

        ' Clear the TextBox
        Guna2TextBox5.Clear()  ' Clears the text in TextBox5 (professor ID)
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
        LoadStudentBaseOnSearch()
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
            AddClassToStudent()
        Else
            MessageBox.Show("Student does not exist.")
        End If
    End Sub

    Private Sub AddClassInfo()
        ' Gather input values from the controls
        Dim courseCode As String = Guna2ComboBox7.SelectedItem.ToString() ' Course code selected from ComboBox
        Dim schoolYear As String = Guna2ComboBox3.SelectedItem?.ToString() & "-" & Guna2ComboBox4.SelectedItem?.ToString() ' School year selected from ComboBox
        Dim day As String = Guna2ComboBox5.SelectedItem.ToString() ' Day selected from ComboBox
        Dim section As String = Guna2ComboBox6.SelectedItem.ToString() ' Section selected from ComboBox
        Dim professorId As String = Guna2TextBox5.Text ' Professor ID from TextBox

        ' Check if all fields are filled
        If String.IsNullOrEmpty(courseCode) Or String.IsNullOrEmpty(schoolYear) Or
       String.IsNullOrEmpty(day) Or String.IsNullOrEmpty(section) Or
       String.IsNullOrEmpty(professorId) Then
            MessageBox.Show("Please fill all the fields.")
            Return
        End If

        ' SQL query to insert new class info into the class_info table
        Dim query As String = "INSERT INTO class_info (course_id, school_year, day, section, professor_id) " &
                          "VALUES ((SELECT course_id FROM course_info WHERE code = @courseCode), " &
                          "@schoolYear, @day, @section, @professorId)"

        Try
            ' Open database connection
            openCon()

            ' Set up the command to execute the query
            Using cmd As New MySqlCommand(query, con)
                ' Add parameters to the command
                cmd.Parameters.AddWithValue("@courseCode", courseCode)
                cmd.Parameters.AddWithValue("@schoolYear", schoolYear)
                cmd.Parameters.AddWithValue("@day", day)
                cmd.Parameters.AddWithValue("@section", section)
                cmd.Parameters.AddWithValue("@professorId", professorId)

                ' Execute the query
                cmd.ExecuteNonQuery()
            End Using

            ' Notify user of success
            MessageBox.Show("Class information added successfully!")

        Catch ex As Exception
            ' Show error message in case of failure
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            ' Close the database connection
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub UpdateProfessorId()
        ' Gather input values from the controls
        Dim courseCode As String = Guna2ComboBox7.SelectedItem?.ToString() ' Course code from ComboBox
        Dim schoolYear As String = Guna2ComboBox3.SelectedItem?.ToString() & "-" & Guna2ComboBox4.SelectedItem?.ToString() ' School year selected from ComboBox
        Dim day As String = Guna2ComboBox5.SelectedItem?.ToString() ' Day from ComboBox
        Dim section As String = Guna2ComboBox6.SelectedItem?.ToString() ' Section from ComboBox
        Dim professorId As String = Guna2TextBox5.Text ' Professor ID from TextBox

        ' Check if all fields are filled
        If String.IsNullOrEmpty(courseCode) Or String.IsNullOrEmpty(schoolYear) Or
       String.IsNullOrEmpty(day) Or String.IsNullOrEmpty(section) Or
       String.IsNullOrEmpty(professorId) Then
            MessageBox.Show("Please fill all the fields.")
            Return
        End If

        ' SQL query to select the class_id based on the selected values
        Dim query As String = "SELECT class_id FROM class_info " &
                          "WHERE course_id = (SELECT course_id FROM course_info WHERE code = @courseCode) " &
                          "AND school_year = @schoolYear AND day = @day AND section = @section"

        Dim classId As String = String.Empty

        Try
            ' Open database connection
            openCon()

            ' Set up the command to execute the query to get the class_id
            Using cmd As New MySqlCommand(query, con)
                ' Add parameters to the command
                cmd.Parameters.AddWithValue("@courseCode", courseCode)
                cmd.Parameters.AddWithValue("@schoolYear", schoolYear)
                cmd.Parameters.AddWithValue("@day", day)
                cmd.Parameters.AddWithValue("@section", section)

                ' Execute the query and get the class_id
                Dim result As Object = cmd.ExecuteScalar()

                If result IsNot Nothing Then
                    classId = result.ToString()
                Else
                    MessageBox.Show("No matching class found.")
                    Return
                End If
            End Using

            ' SQL query to update the professor_id for the found class_id
            Dim updateQuery As String = "UPDATE class_info SET professor_id = @professorId WHERE class_id = @classId"

            ' Update the professor_id for the found class_id
            Using cmd As New MySqlCommand(updateQuery, con)
                ' Add parameters to the update command
                cmd.Parameters.AddWithValue("@classId", classId)
                cmd.Parameters.AddWithValue("@professorId", professorId)

                ' Execute the update query
                cmd.ExecuteNonQuery()
            End Using

            ' Notify user of success
            MessageBox.Show("Professor ID updated successfully!")

        Catch ex As Exception
            ' Show error message in case of failure
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            ' Close the database connection
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub LoadSectionsToComboBox()
        Dim query As String = "SELECT DISTINCT section FROM class_info"

        Try
            ' Open the connection
            openCon()

            ' Execute the query and fill the DataTable
            Dim adapter As New MySqlDataAdapter(query, con)
            Dim table As New DataTable()

            adapter.Fill(table)

            ' Clear existing items in the ComboBox
            Guna2ComboBox6.Items.Clear()

            ' Add each distinct section to the ComboBox
            For Each row As DataRow In table.Rows
                Guna2ComboBox6.Items.Add(row("section").ToString())
            Next

            ' Optionally, reset the ComboBox selection to none
            Guna2ComboBox6.SelectedIndex = -1

        Catch ex As Exception
            ' Handle any errors that occur
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            ' Ensure the connection is closed
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub Guna2TextBox5_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox5.TextChanged
        SearchProfessorById(Guna2TextBox5.Text)
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        AddClassInfo()
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        ClearForm()
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        UpdateProfessorId()
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub Guna2Button9_Click_1(sender As Object, e As EventArgs) Handles Guna2Button9.Click
        TabControl1.SelectedTab = TabPage2
    End Sub
End Class