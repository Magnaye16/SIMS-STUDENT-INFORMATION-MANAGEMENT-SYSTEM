Imports DocumentFormat.OpenXml.VariantTypes
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class Form2

    Private Sub AddProgramInfo()
        Dim name As String = Guna2TextBox1.Text
        Dim code As String = Guna2TextBox2.Text

        Try
            Dim checkCode As String = "SELECT COUNT(*) FROM program_info WHERE code = @code"
            Using cmd As New MySqlCommand(checkCode, con)
                cmd.Parameters.AddWithValue("@code", code)
                openCon()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This program code already exists.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try

        Try
            Dim checkName As String = "SELECT COUNT(*) FROM program_info WHERE name = @name"
            Using cmd As New MySqlCommand(checkName, con)
                cmd.Parameters.AddWithValue("@name", name)
                openCon()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This program name already exists.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try


        Dim query As String = "INSERT INTO program_info (name, code) VALUES (@name, @code)"

        Using command As New MySqlCommand(query, con)
            command.Parameters.AddWithValue("@name", name)
            command.Parameters.AddWithValue("@code", code)

            Try
                openCon()

                command.ExecuteNonQuery()

                MessageBox.Show("Program info added successfully!")
            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try

        End Using
    End Sub
    Private Sub AddStudentInfo()
        Dim student_id As String = ComboBox5.SelectedItem?.ToString()
        Dim section As String = ComboBox6.SelectedItem?.ToString()


        Try
            Dim checkName As String = "SELECT COUNT(*) FROM class_members WHERE student_id = @student_id AND section = @section"
            Using cmd As New MySqlCommand(checkName, con)
                cmd.Parameters.AddWithValue("@student_id", student_id)
                cmd.Parameters.AddWithValue("@section", section)

                openCon()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This program name already exists.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try


        Dim query As String = "INSERT INTO class_members (student_id, section) VALUES (@student_id, @section)"

        Using command As New MySqlCommand(query, con)
            command.Parameters.AddWithValue("@student_id", student_id)
            command.Parameters.AddWithValue("@section", section)

            Try
                openCon()

                command.ExecuteNonQuery()

                MessageBox.Show("Student added successfully!")
            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try

        End Using
    End Sub

    Private Sub AddCourseInfo()
        Dim code As String = Guna2TextBox4.Text
        Dim name As String = Guna2TextBox5.Text
        Dim programId As Integer = 0

        If String.IsNullOrEmpty(name) Then
            MessageBox.Show("Please select a program name.")
            Return
        End If

        Try
            Dim getProgramIdQuery As String = "SELECT program_id FROM program_info WHERE name = @name"
            Using cmd As New MySqlCommand(getProgramIdQuery, con)
                cmd.Parameters.AddWithValue("@name", name)
                openCon()

                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    programId = Convert.ToInt32(result)
                Else
                    MessageBox.Show("No matching program found.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
            Return
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
            Return
        Finally
            con.Close()
        End Try

        Dim checkExistingQuery As String = "SELECT COUNT(*) FROM course_info WHERE program_id = @programId AND code = @code"
        Try
            Using cmd As New MySqlCommand(checkExistingQuery, con)
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@code", code)
                openCon()

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This program and section combination already exists.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
            Return
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
            Return
        Finally
            con.Close()
        End Try

        Dim insertQuery As String = "INSERT INTO section_info (program_id, section) VALUES (@programId, @section)"

        Try
            Using cmd As New MySqlCommand(insertQuery, con)
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@name", name)

                openCon()
                cmd.ExecuteNonQuery()

                MessageBox.Show("Section info added successfully!")
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub AddSectionInfo()
        Dim name As String = ComboBox1.SelectedItem?.ToString()
        Dim section As String = Guna2TextBox3.Text
        Dim programId As Integer = 0

        If String.IsNullOrEmpty(name) Then
            MessageBox.Show("Please select a program name.")
            Return
        End If

        Try
            Dim getProgramIdQuery As String = "SELECT program_id FROM program_info WHERE name = @name"
            Using cmd As New MySqlCommand(getProgramIdQuery, con)
                cmd.Parameters.AddWithValue("@name", name)
                openCon()

                ' Execute the query to get the program_id
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    programId = Convert.ToInt32(result)
                Else
                    MessageBox.Show("No matching program found.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
            Return
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
            Return
        Finally
            con.Close()
        End Try

        Dim checkExistingQuery As String = "SELECT COUNT(*) FROM section_info WHERE program_id = @programId AND section = @section"
        Try
            Using cmd As New MySqlCommand(checkExistingQuery, con)
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@section", section)
                openCon()

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This program and section combination already exists.")
                    Return
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database error occurred: " & ex.Message)
            Return
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
            Return
        Finally
            con.Close()
        End Try

        Dim insertQuery As String = "INSERT INTO section_info (program_id, section) VALUES (@programId, @section)"

        Try
            Using cmd As New MySqlCommand(insertQuery, con)
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@section", section)

                openCon()
                cmd.ExecuteNonQuery()

                ' Notify user of 
                MessageBox.Show("Section info added successfully!")
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub


    Private Sub PopulateComboBox1()

        Dim query As String = "SELECT name FROM program_info"

        ComboBox1.Items.Clear()
        ComboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox1.AutoCompleteSource = AutoCompleteSource.ListItems

        ComboBox2.Items.Clear()
        ComboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox2.AutoCompleteSource = AutoCompleteSource.ListItems

        Using command As New MySqlCommand(query, con)
            Try
                openCon()
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ComboBox1.Items.Add(reader("name").ToString())
                        ComboBox2.Items.Add(reader("name").ToString())
                    End While
                End Using

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End Using
    End Sub

    Private Sub PopulateComboBox4()

        Dim query As String = "SELECT name FROM course_info"

        ComboBox2.Items.Clear()
        ComboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox2.AutoCompleteSource = AutoCompleteSource.ListItems

        Using command As New MySqlCommand(query, con)
            Try
                openCon()
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ComboBox2.Items.Add(reader("name").ToString())
                    End While
                End Using

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End Using
    End Sub

    Private Sub PopulateComboBox5()

        Dim query As String = "SELECT student_id FROM student_info"

        ComboBox5.Items.Clear()
        ComboBox5.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox5.AutoCompleteSource = AutoCompleteSource.ListItems

        Using command As New MySqlCommand(query, con)
            Try
                openCon()
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ComboBox5.Items.Add(reader("student_id").ToString())
                    End While
                End Using

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End Using
    End Sub

    Private Sub PopulateComboBox6()

        Dim query As String = "SELECT section FROM section_info"

        ComboBox6.Items.Clear()
        ComboBox6.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox6.AutoCompleteSource = AutoCompleteSource.ListItems

        ComboBox3.Items.Clear()
        ComboBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox3.AutoCompleteSource = AutoCompleteSource.ListItems

        Using command As New MySqlCommand(query, con)
            Try
                openCon()
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ComboBox6.Items.Add(reader("section").ToString())
                        ComboBox3.Items.Add(reader("section").ToString())

                    End While
                End Using

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End Using
    End Sub


    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateComboBox1()
        PopulateComboBox5()
        PopulateComboBox6()
        DisableGroupBoxes()
    End Sub
    Private Sub DisableGroupBoxes()
        ' Disable all Guna2GroupBox controls
        Guna2GroupBox1.Enabled = False
        Guna2GroupBox2.Enabled = False
        Guna2GroupBox3.Enabled = False
        Guna2GroupBox4.Enabled = False
        Guna2GroupBox5.Enabled = False
    End Sub

    Private Sub Guna2RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2RadioButton1.CheckedChanged
        ' Enable the corresponding GroupBox when RadioButton1 is checked
        If Guna2RadioButton1.Checked Then
            EnableGroupBox(Guna2GroupBox1)
        End If
    End Sub

    Private Sub Guna2RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2RadioButton2.CheckedChanged
        ' Enable the corresponding GroupBox when RadioButton2 is checked
        If Guna2RadioButton2.Checked Then
            EnableGroupBox(Guna2GroupBox3)
        End If
    End Sub

    Private Sub Guna2RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2RadioButton3.CheckedChanged
        ' Enable the corresponding GroupBox when RadioButton3 is checked
        If Guna2RadioButton3.Checked Then
            EnableGroupBox(Guna2GroupBox2)
        End If
    End Sub

    Private Sub Guna2RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2RadioButton4.CheckedChanged
        ' Enable the corresponding GroupBox when RadioButton4 is checked
        If Guna2RadioButton4.Checked Then
            EnableGroupBox(Guna2GroupBox4)
        End If
    End Sub

    Private Sub Guna2RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2RadioButton5.CheckedChanged
        ' Enable the corresponding GroupBox when RadioButton5 is checked
        If Guna2RadioButton5.Checked Then
            EnableGroupBox(Guna2GroupBox5)
        End If
    End Sub

    Private Sub EnableGroupBox(groupBox As Guna.UI2.WinForms.Guna2GroupBox)
        ' Disable all GroupBoxes first
        DisableGroupBoxes()

        ' Enable the specified GroupBox
        groupBox.Enabled = True
    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Select Case True
            Case Guna2RadioButton1.Checked
                AddProgramInfo()

            Case Guna2RadioButton2.Checked
                AddSectionInfo()

            Case Guna2RadioButton3.Checked
                AddProgramInfo()

            Case Guna2RadioButton4.Checked
                AddProgramInfo()

            Case Guna2RadioButton5.Checked
                AddStudentInfo()

            Case Else
                MessageBox.Show("Please select a program option.")
        End Select
    End Sub


    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click

    End Sub
End Class