Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class enlistment

    Private Sub LoadResidentInformation(searchTerm As String)

        Dim query As String = "SELECT * FROM student_info WHERE student_id LIKE @searchTerm"

        'If String.IsNullOrWhiteSpace(searchTerm) Then
        '    Guna2TextBox6.Clear()
        '    Guna2TextBox7.Clear()
        '    Guna2TextBox8.Clear()
        '    Guna2TextBox9.Clear()
        '    Return
        'End If

        Try
            openCon()

            Using command As New MySqlCommand(query, con)

                command.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()
                    DataGridView1.Rows.Clear()

                    '
                    If reader.Read() Then
                        TextBox3.Text = reader("last_name").ToString()
                        TextBox4.Text = reader("section").ToString()

                        'insert to table
                        While reader.Read()
                            ' Add a new row to the DataGridView
                            DataGridView1.Rows.Add(reader("student_id"), reader("last_Name"), reader("first_name"), reader("middle_Name"), reader("section"), reader("code"))
                        End While
                    Else
                        TextBox3.Clear()
                        TextBox4.Clear()
                        'loadform()
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Handle any errors that may have occurred
            'MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Public Sub ADDUSER()

        Dim studentId = TextBox3.Text


        Try
            openCon()

            Dim insertStudentInfo As String = "INSERT INTO class (studen)
                                               VALUES (@student_id"

            Dim insertStudentClass As String = "INSERT INTO class_info (school_year, year, section, time_start, time_end, student_id, professor_id, course_id)
                                                VALUES (@school_year, @year, @section, @time_start, @time_end, @student_id, @professor_id, @course_id)"

            Using cmd1 As New MySqlCommand(insertStudentInfo, con)
                'cmd1.Parameters.AddWithValue("@student_id", studentId)
                'cmd1.Parameters.AddWithValue("@last_name", Guna2TextBox1.Text)
                'cmd1.Parameters.AddWithValue("@first_name", Guna2TextBox4.Text)
                'cmd1.Parameters.AddWithValue("@middle_name", Guna2TextBox2.Text)
                'cmd1.Parameters.AddWithValue("@email", Guna2TextBox8.Text)
                'cmd1.Parameters.AddWithValue("@contact_number", Guna2TextBox9.Text)
                'cmd1.Parameters.AddWithValue("@address", Guna2TextBox3.Text)
                'cmd1.Parameters.AddWithValue("@student_type", Guna2ComboBox2.Text)
                'cmd1.Parameters.AddWithValue("@student_status", "E")
                'cmd1.ExecuteNonQuery()

                studentId = Convert.ToInt32(cmd1.ExecuteScalar())
            End Using

            'add missing data
            Using cmd2 As New MySqlCommand(insertStudentClass, con)
                cmd2.Parameters.AddWithValue("@student_id", studentId)
                cmd2.ExecuteNonQuery()
            End Using

            MessageBox.Show($"Student recorded!")

        Catch ex As Exception
            MessageBox.Show($"Error inserting Student: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        LoadResidentInformation(TextBox1.Text)
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub
    'Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
    '    Try
    '        con.Open()

    '        ' Query to check if the student number and code combination exists
    '        Dim checkQuery As String = "SELECT si.*, ci.* FROM sms_db si " &
    '                       "JOIN course_info ci ON si.course_id = ci.course_id " &
    '                       "WHERE si.student_id = @student_id AND ci.code = @code"

    '        Using command As New MySqlCommand(checkQuery, con)
    '            command.Parameters.AddWithValue("@last_name", TextBox3.Text)
    '            command.Parameters.AddWithValue("@section", TextBox4.Text)

    '            ' Execute the query and read the result
    '            Using reader As MySqlDataReader = command.ExecuteReader()
    '                ' Clear existing rows in DataGridView before adding new rows
    '                'DVG1.Rows.Clear()

    '                ' Check if any record exists and populate DataGridView
    '                'If reader.HasRows Then
    '                '    While reader.Read()
    '                '        ' Add a new row to the DataGridView
    '                '        DVG1.Rows.Add(reader("Student_Number"), reader("Last_Name"), reader("section_name"), reader("code"))
    '                '    End While
    '                'Else
    '                '    ' MessageBox.Show("No records found with the given student number and code.")
    '                'End If
    '            End Using
    '        End Using

    '    Catch ex As Exception
    '        MessageBox.Show("An error occurred: " & ex.Message)
    '    Finally
    '        If con.State = ConnectionState.Open Then
    '            con.Close()
    '        End If
    '    End Try
    'End Sub
End Class