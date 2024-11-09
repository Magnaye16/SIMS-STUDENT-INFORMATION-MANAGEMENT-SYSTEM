Imports DocumentFormat.OpenXml.Bibliography
Imports DocumentFormat.OpenXml.ExtendedProperties
Imports MySql.Data.MySqlClient

Public Class Form4
    Public Sub LoadAttendanceTable()
        Try

            openCon()

            Dim query As String = "
                    SELECT CONCAT(s.last_name, ', ', s.first_name, ' ', s.middle_name) AS 'Full Name',
                            a.log_date AS Date, a.time_in AS 'Time In', a.time_out AS 'Time Out', a.status,
                            c.year AS Year, c.section AS Section, c.class_day
                    FROM attendance_log AS a
                    INNER JOIN student_info AS s ON a.student_id = s.student_id
                    INNER JOIN class_info AS c ON a.class_id = c.class_id
                    ORDER BY a.log_date DESC;
                "

            Using adapter As New MySqlDataAdapter(query, con)

                Dim dataTable As New DataTable()
                adapter.Fill(dataTable)

                dataTable.Columns.Add("Day", GetType(String))
                dataTable.Columns.Add("Remark", GetType(String))

                For Each row As DataRow In dataTable.Rows
                    Dim dayNumber As Integer = Convert.ToInt32(row("class_day"))
                    row("Day") = GetDayName(dayNumber)

                    Dim statusChar As Char = Convert.ToChar(row("status"))
                    row("Remark") = CharToWord(statusChar)
                Next

                attendanceDGV.DataSource = dataTable
                attendanceDGV.Columns("status").Visible = False
                attendanceDGV.Columns("class_day").Visible = False
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Function GetDayName(dayNumber As Integer) As String
        Select Case dayNumber
            Case 1 : Return "Sunday"
            Case 2 : Return "Monday"
            Case 3 : Return "Tuesday"
            Case 4 : Return "Wednesday"
            Case 5 : Return "Thursday"
            Case 6 : Return "Friday"
            Case 7 : Return "Saturday"
            Case Else : Return "Unknown"
        End Select
    End Function

    Function CharToWord(ByVal character As Char) As String
        Select Case character
            Case "A"c : Return "Absent"
            Case "P"c : Return "Present"
            Case "L"c : Return "Late"
            Case Else
                Return "Unknown"
        End Select
    End Function
    Private Sub LoadResidentInformation(searchTerm As String)

        Dim query As String = "
                    SELECT CONCAT(s.last_name, ', ', s.first_name, ' ', s.middle_name) AS 'Full Name',
                            a.log_date AS Date, a.time_in AS 'Time In', a.time_out AS 'Time Out', a.status,
                            c.year AS Year, c.section AS Section, c.class_day
                    FROM attendance_log AS a
                    INNER JOIN student_info AS s ON a.student_id = s.student_id
                    INNER JOIN class_info AS c ON a.class_id = c.class_id
                    ORDER BY a.log_date DESC;
                "

        Try
            openCon()

            Using command As New MySqlCommand(query, con)

                command.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                Using reader As MySqlDataReader = command.ExecuteReader()
                    attendanceDGV.Rows.Clear()
                    '
                    If reader.Read() Then
                        Dim fullname As String
                        fullname = reader("last_name") + "," + reader("first_name") + " " + reader("middle_name")

                        'insert to table
                        While reader.Read()
                            ' Add a new row to the DataGridView
                            attendanceDGV.Rows.Add(reader(fullname), reader(""), reader("given_Name"), reader("middle_Name"), reader("address"))
                        End While
                    Else

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


End Class