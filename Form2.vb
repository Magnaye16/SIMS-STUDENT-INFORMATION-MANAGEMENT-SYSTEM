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

    Private Sub AddSectionInfo()
        ' Gather the values from the controls
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

        ' Check if the combination of program_id and section already exists in the section_info table
        Dim checkExistingQuery As String = "SELECT COUNT(*) FROM section_info WHERE program_id = @programId AND section = @section"
        Try
            Using cmd As New MySqlCommand(checkExistingQuery, con)
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@section", section)
                openCon()

                ' Execute the query to check if the combination already exists
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

        ' Now insert the program_id and section into the section_info table
        Dim insertQuery As String = "INSERT INTO section_info (program_id, section) VALUES (@programId, @section)"

        Try
            Using cmd As New MySqlCommand(insertQuery, con)
                ' Add parameters to the insert command
                cmd.Parameters.AddWithValue("@programId", programId)
                cmd.Parameters.AddWithValue("@section", section)

                ' Open connection and execute the insert command
                openCon()
                cmd.ExecuteNonQuery()

                ' Notify user of success
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

        Using command As New MySqlCommand(query, con)
            Try
                openCon()
                Using reader As MySqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        ComboBox1.Items.Add(reader("name").ToString())
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

            Case Else
                MessageBox.Show("Please select a program option.")
        End Select
    End Sub


    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click

    End Sub
End Class