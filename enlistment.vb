Imports DocumentFormat.OpenXml.Bibliography
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub Enlistment(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupTimePickers()
        PopulateDaysComboBox()
        LoadStudentClasses()
        LoadCourses()
        LoadProfessor()
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

    Private Sub LoadProfessor()
        Dim query As String = "SELECT professor_id, CONCAT(last_name, ', ', first_name) AS name FROM professor_info"

        Try
            openCon()
            Dim cmd As New MySqlCommand(query, con)

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)

            Guna2ComboBox2.DisplayMember = "name"
            Guna2ComboBox2.ValueMember = "professor_id"
            Guna2ComboBox2.DataSource = table

            Guna2ComboBox2.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox1.SelectedIndexChanged
        con.Close()

        If Guna2ComboBox1.SelectedIndex <> -1 Then
            Dim selectedCourse = Convert.ToInt32(Guna2ComboBox1.SelectedValue)

            Dim query3 = "SELECT professor_id FROM class_info WHERE course_id = @course_id"

            Try
                openCon()
                Dim cmd3 As New MySqlCommand(query3, con)
                cmd3.Parameters.AddWithValue("@course_id", selectedCourse)

                Dim result = cmd3.ExecuteScalar

                If result IsNot Nothing Then
                    Dim selectedProfessor = Convert.ToInt32(result)
                    Guna2ComboBox2.SelectedValue = selectedProfessor
                Else
                    Guna2ComboBox2.SelectedIndex = -1
                End If
            Catch ex As Exception
                MessageBox.Show($"Error: {ex.Message}")
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If
    End Sub

    Private Function IsStudentAlreadyInClass(studentId As String, selectedCourse As Integer) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM class_info WHERE student_id = @student_id AND course_id = @course_id"
        Try
            openCon()

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@student_id", Guna2TextBox4.Text)
            cmd.Parameters.AddWithValue("@course_id", Convert.ToInt32(Guna2ComboBox1.SelectedValue))

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

    Private Sub AddClassToSTudent()
        Dim selectedCourse As Integer = Convert.ToInt32(Guna2ComboBox1.SelectedValue)
        Dim selectedProfessor As Integer = Convert.ToInt32(Guna2ComboBox2.SelectedValue)
        Dim studentId As String = Guna2TextBox4.Text.Trim()

        If String.IsNullOrEmpty(Guna2TextBox2.Text) OrElse
            Guna2ComboBox1.SelectedIndex = -1 OrElse
            Guna2ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Please ensure all required fields are filled.")
            Return
        End If

        Dim yearStart As String = Guna2ComboBox3.SelectedItem.ToString()
        Dim yearEnd As String = Guna2ComboBox4.SelectedItem.ToString()
        Dim schoolYear As String = $"{yearStart} - {yearEnd}"

        If Guna2DateTimePicker1.Value.TimeOfDay >= Guna2DateTimePicker2.Value.TimeOfDay Then
            MessageBox.Show("Start time must be earlier than end time.")
            Return
        End If

        If IsStudentAlreadyInClass(studentId, selectedCourse) Then
            MessageBox.Show("Student is already in this class.")
            Return
        End If

        Dim query As String = "INSERT INTO class_info (school_year, class_day, year, section, time_start, time_end, student_id, professor_id, course_id)
                               VALUES (@school_year, @class_day, @year, @section, @time_start, @time_end, @student_id, @professor_id, @course_id)"

        Try
            openCon()

            Dim cmd As New MySqlCommand(query, con)

            cmd.Parameters.AddWithValue("@school_year", schoolYear)
            cmd.Parameters.AddWithValue("@class_day", Convert.ToInt32(Guna2ComboBox5.SelectedValue))
            cmd.Parameters.AddWithValue("@year", Convert.ToInt32(Guna2ComboBox6.SelectedItem))
            cmd.Parameters.AddWithValue("@section", Guna2TextBox2.Text)
            cmd.Parameters.AddWithValue("@time_start", Guna2DateTimePicker1.Value.TimeOfDay)
            cmd.Parameters.AddWithValue("@time_end", Guna2DateTimePicker2.Value.TimeOfDay)
            cmd.Parameters.AddWithValue("@student_id", Guna2TextBox4.Text)
            cmd.Parameters.AddWithValue("@professor_id", selectedProfessor)
            cmd.Parameters.AddWithValue("@course_id", selectedCourse)

            cmd.ExecuteNonQuery()

            MessageBox.Show("Success.")

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
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
            AddClassToSTudent()
        Else
            MessageBox.Show("Student does not exist.")
        End If
    End Sub

    Private Sub Guna2TextBox3_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox3.TextChanged
        LoadProfBaseOnSearch()
    End Sub
End Class