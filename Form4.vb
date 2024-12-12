Imports DocumentFormat.OpenXml.Bibliography
Imports DocumentFormat.OpenXml.ExtendedProperties
Imports MySql.Data.MySqlClient

Public Class Form4
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadAttendanceTable()
    End Sub
    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        Searchfromtable()
    End Sub



    Public Sub LoadAttendanceTable()
        Dim query As String = "
    SELECT 
        CONCAT(s.last_name, ', ', s.first_name, ' ', s.middle_name) AS `Full Name`,
        a.log_date AS `Date`, 
        a.time_in AS `Time In`, 
        CASE 
            WHEN a.status = 'P' THEN 'Present'
            WHEN a.status = 'A' THEN 'Absent'
            WHEN a.status = 'L' THEN 'Late'
            ELSE 'Unknown' 
        END AS `Status`,
        c.school_year AS `School Year`, 
        c.section AS `Section`
    FROM attendance_log AS a
    INNER JOIN student_info AS s ON a.student_id = s.student_id
    INNER JOIN class_info AS c ON a.class_id = c.class_id
    ORDER BY a.log_date DESC;
"

        Try
            openCon() ' Ensure this function properly opens the database connection.

            Using command As New MySqlCommand(query, con)
                ' Use parameterized query to prevent SQL injection
                'command.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()
                    Guna2DataGridView1.Rows.Clear() ' Clear existing rows in the DataGridView

                    ' Check if there are results
                    If reader.HasRows Then
                        While reader.Read()
                            ' Add data to the DataGridView
                            Guna2DataGridView1.Rows.Add(
                        reader("Full Name"),
                        reader("Date"),
                        reader("Time In"),
                        reader("Status"),
                        reader("School Year"),
                        reader("Section")
                    )
                        End While
                    Else
                        MessageBox.Show("No records found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Display the error message
            'MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            con.Close() ' Ensure the connection is closed even if an error occurs
        End Try

    End Sub



    Public Sub Searchfromtable()
        Dim searchTerm As String = Guna2TextBox1.Text.Trim()

        For Each row As DataGridViewRow In Guna2DataGridView1.Rows
            Dim matchFound As Boolean = False

            ' Search in each cell of the row
            For Each cell As DataGridViewCell In row.Cells
                If cell.Value IsNot Nothing AndAlso cell.Value.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) Then
                    matchFound = True
                    Exit For
                End If
            Next

            ' Show or hide the row based on the match
            row.Visible = matchFound
        Next

    End Sub
End Class