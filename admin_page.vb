Imports System.Drawing.Imaging
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports ZXing

Public Class admin_page
    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        'validation  
        If Guna2TextBox1.Text = "" Or
           Guna2TextBox2.Text = "" Or
           Guna2TextBox4.Text = "" Or
           Guna2TextBox5.Text = "" Or
           Guna2TextBox6.Text = "" Then
            MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Clear
            Return

        End If
        'check if user already already exist
        Dim user = Guna2TextBox5.Text
        Dim query = "SELECT COUNT(*) FROM user WHERE uname = @uname "
        openCon

        Try

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@uname", user)

                Dim count As Integer = command.ExecuteScalar

                If count > 0 Then
                    MessageBox.Show("This user already already exist.")
                    con.Close
                    Clear
                    Return
                ElseIf count = 0 Then
                    con.Close
                    'insert all the info
                    'ADDUSER()
                    Dispose
                    'Form1.Show()
                End If
            End Using
        Catch ex As Exception
        Finally
            'Clear()

        End Try
    End Sub
    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
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
        PictureBox1.Image = qrCodeBitmap

    End Sub
    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        Dim filename = Guna2TextBox2.Text + "_" + Guna2TextBox1.Text ' <<< UID DAPAT (TEMPORARY)
        Dim filePath = "C:\Users\John Roi\source\repos\SMS(Student Management System)\Generated QR\" + filename + ".jpg"
        Dim qrCodeBitmap As Bitmap = PictureBox1.Image
        qrCodeBitmap.Save(filePath, ImageFormat.Png)
        MsgBox("Code has been generated and saved as " & filePath)

        'close
        'Dim result = MessageBox.Show("Do you want to send Qr Code", "Send Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        '' Check the user's response
        'If result = DialogResult.No Then
        '    Close()
        'Else
        '    'Sendcreatedemail()
        '    Close()
        'End If
    End Sub


    'Functions to call out
    Private Sub Clear()
        Guna2TextBox1.Clear()
        Guna2TextBox2.Clear()
        Guna2TextBox3.Clear()
        Guna2TextBox4.Clear()
        Guna2TextBox5.Clear()
        Guna2TextBox6.Clear()
    End Sub
    Private Sub LoadUSERnfo()
        Student_data.Rows.Clear()

        openCon()

        Try
            ' Create a SqlConnection using the connection string
            ' Open the connection

            ' Create a SqlCommand to select data from the addemp table
            Dim command As New MySqlCommand("SELECT * FROM user", con)

            ' Execute the command and obtain a reader
            Dim reader As MySqlDataReader = command.ExecuteReader()

            ' Loop through the rows in the SqlDataReader
            While reader.Read()
                ' Add a new row to the DataGridView
                Student_data.Rows.Add(reader("gencode"), reader("lname"), reader("fname"), reader("mname"), reader("age"), reader("uname"), reader("pword"), reader("role"))
            End While

            ' Close the SqlDataReader
            reader.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try
    End Sub


End Class


