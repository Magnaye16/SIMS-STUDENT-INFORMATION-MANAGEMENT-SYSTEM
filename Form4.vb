Imports DocumentFormat.OpenXml.Bibliography
Imports MySql.Data.MySqlClient

Public Class Form4
    Public Sub LoadAttendanceTable()
        Try
            openCon()

            Dim query As String = "
                    SELECT (s.last_name + ', ' + s.first_name + ' ' + a.middle_name) AS Name,
                            a.log_date AS Date, a.time_in AS Time In, a.time_out AS Time Out, a.status AS Status,
                            c.year AS Year, c.section AS Section, c.class_day
                    FROM attendance_log AS a
                    INNER JOIN student_info AS s ON a.student_id = s.student_id
                    INNER JOIN class_info AS c ON a.class_id = c.class_id
                    ORDER BY t1.DateAdded DESC;
                "

            Using adapter As New MySqlDataAdapter(query, con)

                Dim dataTable As New DataTable()
                adapter.Fill(dataTable)

                dataTable.Columns.Add("Day", GetType(String))

                For Each row As DataRow In dataTable.Rows
                    Dim dayNumber As Integer = Convert.ToInt32(row("class_day"))
                    row("Day") = GetDayName(dayNumber)
                Next

                attendanceDGV.DataSource = dataTable
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


End Class