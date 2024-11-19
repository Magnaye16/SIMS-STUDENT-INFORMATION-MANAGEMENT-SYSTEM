Imports DocumentFormat.OpenXml.Bibliography
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub enlistment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupTimePickers()
        PopulateDaysComboBox()
        LoadStudentClasses()
        LoadCourses()
        LoadProfessor()
        PopulateYearComboBox()
    End Sub

    Private Sub SetupTimePickers()
        DateTimePicker1.Format = DateTimePickerFormat.Time
        DateTimePicker2.Format = DateTimePickerFormat.Time

        DateTimePicker1.ShowUpDown = True
        DateTimePicker2.ShowUpDown = True

        DateTimePicker1.Value = DateTimePicker1.Value.AddMinutes(-DateTimePicker1.Value.Minute)
        DateTimePicker2.Value = DateTimePicker2.Value.AddMinutes(-DateTimePicker2.Value.Minute)
        DateTimePicker1.Value = DateTimePicker1.Value.AddSeconds(-DateTimePicker1.Value.Second)
        DateTimePicker2.Value = DateTimePicker2.Value.AddSeconds(-DateTimePicker2.Value.Second)
    End Sub

    Private Sub PopulateYearComboBox()
        ComboBox5.Items.Clear()
        ComboBox6.Items.Clear()

        Dim startYear As Integer = DateTime.Now.Year
        Dim years As New List(Of Integer)

        For i As Integer = 0 To 10 ' Populate with 10 years starting from the current year
            years.Add(startYear + i)
        Next

        ComboBox5.DataSource = New BindingSource(years, Nothing)
        ComboBox6.DataSource = New BindingSource(years, Nothing)

        ComboBox5.SelectedIndex = -1
        ComboBox6.SelectedIndex = -1
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

        ComboBox7.DataSource = New BindingSource(daysOfWeek, Nothing)
        ComboBox7.DisplayMember = "Key"
        ComboBox7.ValueMember = "Value"
        ComboBox7.SelectedIndex = -1
    End Sub

    Private Sub SearchStudent(searchTerm As String)
        Dim query As String = "SELECT last_name, first_name, middle_name FROM student_info WHERE student_id LIKE @searchTerm"

        If String.IsNullOrWhiteSpace(searchTerm) Then
            TextBox3.Clear()
            Return
        End If

        Try
            openCon()
            Using command As New MySqlCommand(query, con)

                command.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()

                    If reader.Read() Then
                        TextBox3.Text = $"{reader("last_name")}, {reader("first_name")} {reader("middle_name")}"
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
            SELECT s.student_id, s.last_name,  s.first_name, s.middle_name,
                   c.section AS Section, ci.code AS Classcode
            FROM class_info c
            INNER JOIN student_info s ON s.student_id = c.student_id
            INNER JOIN course_info ci ON ci.course_id = c.course_id"
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

            DataGridView1.DataSource = table
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub LoadStudentBaseOnSearch()
        Dim searchTerm As String = TextBox1.Text.Trim()

        Dim query As String = "
        SELECT s.student_id, s.last_name, s.first_name, s.middle_name,
               c.section AS Section, ci.code AS Classcode
        FROM class_info c
        INNER JOIN student_info s ON s.student_id = c.student_id
        INNER JOIN course_info ci ON ci.course_id = c.course_id
        WHERE s.student_id LIKE @searchTerm"

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

            DataGridView1.DataSource = table

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


            ComboBox1.DisplayMember = "code"
            ComboBox1.ValueMember = "course_id"
            ComboBox1.DataSource = table

            ComboBox1.SelectedIndex = -1
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

            ComboBox2.DisplayMember = "name"
            ComboBox2.ValueMember = "professor_id"
            ComboBox2.DataSource = table

            ComboBox2.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        con.Close()

        If ComboBox1.SelectedIndex <> -1 Then
            Dim selectedCourse = Convert.ToInt32(ComboBox1.SelectedValue)

            Dim query3 = "SELECT professor_id FROM class_info WHERE course_id = @course_id"

            Try
                openCon()
                Dim cmd3 As New MySqlCommand(query3, con)
                cmd3.Parameters.AddWithValue("@course_id", selectedCourse)

                Dim result = cmd3.ExecuteScalar

                If result IsNot Nothing Then
                    Dim selectedProfessor = Convert.ToInt32(result)
                    ComboBox2.SelectedValue = selectedProfessor
                Else
                    ComboBox2.SelectedIndex = -1
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
            cmd.Parameters.AddWithValue("@student_id", TextBox1.Text)
            cmd.Parameters.AddWithValue("@course_id", Convert.ToInt32(ComboBox1.SelectedValue))

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
        Dim selectedCourse As Integer = Convert.ToInt32(ComboBox1.SelectedValue)
        Dim selectedProfessor As Integer = Convert.ToInt32(ComboBox2.SelectedValue)
        Dim studentId As String = TextBox1.Text.Trim()

        If String.IsNullOrEmpty(TextBox4.Text) OrElse
       ComboBox1.SelectedIndex = -1 OrElse
       ComboBox2.SelectedIndex = -1 OrElse
       ComboBox5.SelectedIndex = -1 OrElse
       ComboBox6.SelectedIndex = -1 OrElse
       ComboBox7.SelectedIndex = -1 OrElse
       ComboBox8.SelectedIndex = -1 Then
            MessageBox.Show("Please ensure all required fields are filled.")
            Return
        End If

        Dim yearStart As String = ComboBox5.SelectedItem.ToString()
        Dim yearEnd As String = ComboBox6.SelectedItem.ToString()
        Dim schoolYear As String = $"{yearStart} - {yearEnd}"

        If DateTimePicker1.Value.TimeOfDay >= DateTimePicker2.Value.TimeOfDay Then
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
            cmd.Parameters.AddWithValue("@class_day", Convert.ToInt32(ComboBox7.SelectedValue))
            cmd.Parameters.AddWithValue("@year", Convert.ToInt32(ComboBox8.SelectedItem))
            cmd.Parameters.AddWithValue("@section", TextBox4.Text)
            cmd.Parameters.AddWithValue("@time_start", DateTimePicker1.Value.TimeOfDay)
            cmd.Parameters.AddWithValue("@time_end", DateTimePicker2.Value.TimeOfDay)
            cmd.Parameters.AddWithValue("@student_id", TextBox1.Text)
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

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        SearchStudent(TextBox1.Text)
        LoadStudentBaseOnSearch()
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged
        Dim selectedYear As Integer = Convert.ToInt32(ComboBox5.SelectedItem)

        Dim nextYear As Integer = selectedYear + 1

        If ComboBox6.Items.Contains(nextYear) Then
            ComboBox6.SelectedItem = nextYear
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

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim studentNumber As String = TextBox1.Text.Trim()

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
End Class