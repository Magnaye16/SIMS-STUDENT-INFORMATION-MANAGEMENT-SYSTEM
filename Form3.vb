﻿Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports System.Drawing.Imaging
Imports System.Runtime.ConstrainedExecution
Imports System.Transactions
Imports ZXing

Public Class Form3
    Private ReadOnly time_start As Object

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Guna2ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox2.SelectedIndexChanged

    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        'generate qr
        Dim qrstring As String = Guna2TextBox2.Text

        ' Create a barcode writer
        Dim barcodeWriter As New BarcodeWriter
        barcodeWriter.Format = BarcodeFormat.CODE_128 ' < barcode

        ' Set encoding properties (optional)
        barcodeWriter.Options = New Common.EncodingOptions With {
            .Width = 300,
            .Height = 300
        }

        ' Generate the QR code bitmap
        Dim qrCodeBitmap = barcodeWriter.Write(qrstring)

        ' Display the QR code bitmap in a PictureBox or save it to a file
        Guna2PictureBox1.Image = qrCodeBitmap
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Dim filename = Guna2TextBox2.Text + "_" + Guna2TextBox1.Text ' <<< UID DAPAT (TEMPORARY)
        Dim filePath = "C:\Users\John Roi\source\repos\SMS(Student Management System)\Generated QR\" + filename + ".jpg"
        Dim qrCodeBitmap As Bitmap = Guna2PictureBox1.Image
        qrCodeBitmap.Save(filePath, ImageFormat.Png)
        MsgBox("Code has been generated and saved as " & filePath)
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Clear()

    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click

        'create 
        'validation  
        If Guna2TextBox1.Text = "" Or
           Guna2TextBox2.Text = "" Or
           Guna2TextBox4.Text = "" Or
           Guna2TextBox5.Text = "" Or
           Guna2TextBox6.Text = "" Or
           Guna2TextBox7.Text = "" Or
           Guna2TextBox8.Text = "" Or
           Guna2TextBox9.Text = "" Or
           Guna2ComboBox1.Text = "" Or
           Guna2ComboBox2.Text = "" Then
            MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Clear
            Return
        Else
            ADDUSER()
        End If

        Dim stud_ID = Guna2TextBox7.Text
        Dim query = "SELECT COUNT(*) FROM student_info WHERE student_number = @student_number"
        openCon() ' Open the connection

        Try
            Using command As New MySqlCommand(query, con)
                ' Add parameter to prevent SQL injection and ensure proper casting
                command.Parameters.AddWithValue("@student_number", stud_ID)

                ' ExecuteScalar will return the count, cast it to Integer
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                If count > 0 Then
                    ' If the count is greater than 0, the student already exists
                    MessageBox.Show("This user already exists.")
                    Return
                Else
                    ' If the count is 0, proceed with adding the student
                    con.Close()
                    ADDUSER() ' Call the method to insert user data
                    Dispose() ' Dispose of the current form if needed
                End If
            End Using
        Catch ex As Exception
            ' Handle any exceptions (e.g., log the error)
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            ' Ensure the connection is closed properly, even in case of an exception
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
            ' Optionally, clear fields or perform cleanup
        End Try
    End Sub

    Public Sub ADDUSER()

        Dim studentNumber = Guna2TextBox7.Text
        Dim studentId As Integer


        Try
            Dim insertStudentInfo As String = "INSERT INTO student_info (student_id, last_name, first_name, middle_name, email, contact_number, address, student_type, student_status)
                                               VALUES (@student_id, @last_name, @first_name, @middle_name, @email, @contact_number, @address, @student_type, @student_status);
                                               SELECT LAST_INSERT_ID();"

            Dim insertStudentClass As String = "INSERT INTO class_info (school_year, year, section, time_start, time_end, student_id, professor_id, course_id)
                                                VALUES (@school_year, @year, @section, @time_start, @time_end, @student_id, @professor_id, @course_id)"

            Using cmd1 As New MySqlCommand(insertStudentInfo, con)
                cmd1.Parameters.AddWithValue("@student_id", studentNumber)
                cmd1.Parameters.AddWithValue("@last_name", Guna2TextBox1.Text)
                cmd1.Parameters.AddWithValue("@first_name", Guna2TextBox4.Text)
                cmd1.Parameters.AddWithValue("@middle_name", Guna2TextBox2.Text)
                cmd1.Parameters.AddWithValue("@email", Guna2TextBox8.Text)
                cmd1.Parameters.AddWithValue("@contact_number", Guna2TextBox9.Text)
                cmd1.Parameters.AddWithValue("@address", Guna2TextBox3.Text)
                cmd1.Parameters.AddWithValue("@student_type", Guna2ComboBox2.Text)
                cmd1.Parameters.AddWithValue("@student_status", "E")
                cmd1.ExecuteNonQuery()

                studentId = Convert.ToInt32(cmd1.ExecuteScalar())
            End Using

            'add missing data
            Using cmd2 As New MySqlCommand(insertStudentClass, con)
                cmd2.Parameters.AddWithValue("@student_id", studentId)
                cmd2.ExecuteNonQuery()
            End Using

            MessageBox.Show($"Student recorded!")

        Catch ex As Exception
            MessageBox.Show($"Error inserting Student" & ex.Message)

        End Try
    End Sub

    Public Sub Clear()
        Guna2TextBox1.Text = ""
        Guna2TextBox2.Text = ""
        Guna2TextBox4.Text = ""
        Guna2TextBox5.Text = ""
        Guna2TextBox6.Text = ""
        Guna2TextBox7.Text = ""
        Guna2TextBox8.Text = ""
        Guna2TextBox9.Text = ""
        Guna2ComboBox1.Text = ""
        Guna2ComboBox2.Text = ""
    End Sub
End Class