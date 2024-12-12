Imports DocumentFormat.OpenXml.VariantTypes
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
        If Guna2RadioButton1.Checked Then
            AddProgramInfo()
        End If
    End Sub


End Class