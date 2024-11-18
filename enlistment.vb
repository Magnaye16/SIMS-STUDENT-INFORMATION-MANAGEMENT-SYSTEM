Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub enlistment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupTimePickers()
        LoadStudentClasses()
        LoadCourses()
        LoadProfessor()
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

    'Private Sub AddClassToSTudent()
    '    Dim selectedCourse As Integer = Convert.ToInt32(ComboBox1.SelectedValue)
    '    Dim selectedProfessor As Integer = Convert.ToInt32(ComboBox2.SelectedValue)

    '    If String.IsNullOrEmpty(SchoolYearTextBox.Text) OrElse
    '   String.IsNullOrEmpty(ClassDayTextBox.Text) OrElse
    '   String.IsNullOrEmpty(YearTextBox.Text) OrElse
    '   String.IsNullOrEmpty(SectionTextBox.Text) OrElse
    '   String.IsNullOrEmpty(TimeStartTextBox.Text) OrElse
    '   String.IsNullOrEmpty(TimeEndTextBox.Text) OrElse
    '   ComboBoxStudent.SelectedIndex = -1 OrElse
    '   ComboBoxProfessor.SelectedIndex = -1 OrElse
    '   ComboBoxCourse.SelectedIndex = -1 Then
    '        MessageBox.Show("Please ensure all required fields are filled.")
    '        Return
    '    End If

    '    Dim query As String = "INSERT INTO class_info (school_year, class_day, year, section, time_start, time_end, student_id, professor_id, course_id)
    '                           VALUES (@school_year, @class_day, @year, @section, @time_start, @time_end, @student_id, @professor_id, @course_id)"

    '    Try
    '        openCon()

    '        Dim cmd As New MySqlCommand(query, con)

    '        cmd.Parameters.AddWithValue("@school_year", schoolYear)
    '        cmd.Parameters.AddWithValue("@class_day", classDay)
    '        cmd.Parameters.AddWithValue("@year", Year)
    '        cmd.Parameters.AddWithValue("@section", section)
    '        cmd.Parameters.AddWithValue("@time_start", timeStart)
    '        cmd.Parameters.AddWithValue("@time_end", timeEnd)
    '        cmd.Parameters.AddWithValue("@student_id", studentId)
    '        cmd.Parameters.AddWithValue("@professor_id", selectedProfessor)
    '        cmd.Parameters.AddWithValue("@course_id", selectedCourse)

    '        cmd.ExecuteNonQuery()

    '        MessageBox.Show("Data inserted successfully.")

    '    Catch ex As Exception
    '        MessageBox.Show($"Error: {ex.Message}")
    '    Finally
    '        If con.State = ConnectionState.Open Then
    '            con.Close()
    '        End If
    '    End Try

    'End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        SearchStudent(TextBox1.Text)
        LoadStudentBaseOnSearch()
    End Sub

End Class