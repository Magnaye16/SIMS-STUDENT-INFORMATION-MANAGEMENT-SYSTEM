Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports System.Drawing.Imaging
Imports System.Runtime.ConstrainedExecution
Imports ZXing

Public Class Form3
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

        'check if user already already exist
        'Dim stud_ID = Guna2TextBox7.Text
        'Dim query = "SELECT COUNT(*) FROM crudsstud WHERE stud_ID = @stud_ID "
        'openCon()

        'Try

        '    Using command As New MySqlCommand(query, con)
        '        command.Parameters.AddWithValue("@stud_ID", stud_ID)

        '        Dim count As Integer = command.ExecuteScalar

        '        If count > 0 Then
        '            MessageBox.Show("This user already already exists.")
        '            con.Close()
        '            'Clear
        '            Return
        '        ElseIf count = 0 Then

        '            'insert all the info
        '            ADDUSER()
        '            Dispose()
        '            'Form1.Show()
        '        End If
        '    End Using
        'Catch ex As Exception
        'Finally
        '    'Clear()

        'End Try

        Dim stud_ID = Guna2TextBox7.Text
        Dim query = "SELECT COUNT(*) FROM crudstud WHERE stud_ID = @stud_ID"
        openCon() ' Open the connection

        Try
            Using command As New MySqlCommand(query, con)
                ' Add parameter to prevent SQL injection and ensure proper casting
                command.Parameters.AddWithValue("@stud_ID", stud_ID)

                ' ExecuteScalar will return the count, cast it to Integer
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                If count > 0 Then
                    ' If the count is greater than 0, the student already exists
                    'MessageBox.Show("This user already exists.")
                    Return
                Else
                    ' If the count is 0, proceed with adding the student
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
        ' Dim lastName, givenName As Guna2TextBox1.Text Guna2TextBox4.Text
        ' , givenName, middleNAme, year, section, course, studentStatus, email, number, address, studentID, barCode
        Try
            openCon()
            Dim query As String = "INSERT INTO crudstud (stud_ID, lname, gname, mname, year, section, course, studstat, email, contact, address, bcode) VALUES (@stud_ID, @lname, @gname, @mname, @year, @section, @course, @studstat, @email, @contact, @address, @bcode)"
            Using command1 As New MySqlCommand(query, con)
                command1.Parameters.AddWithValue("@stud_ID", Guna2TextBox7.Text)
                command1.Parameters.AddWithValue("@lname", Guna2TextBox1.Text)
                command1.Parameters.AddWithValue("@gname", Guna2TextBox4.Text)
                command1.Parameters.AddWithValue("@mname", Guna2TextBox2.Text)
                command1.Parameters.AddWithValue("@year", Guna2TextBox6.Text)
                command1.Parameters.AddWithValue("@section", Guna2TextBox5.Text)
                command1.Parameters.AddWithValue("@course", Guna2ComboBox1.Text)
                command1.Parameters.AddWithValue("@studstat", Guna2ComboBox2.Text)
                command1.Parameters.AddWithValue("@email", Guna2TextBox8.Text)
                command1.Parameters.AddWithValue("@contact", Guna2TextBox9.Text)
                command1.Parameters.AddWithValue("@address", Guna2TextBox3.Text)
                command1.Parameters.AddWithValue("@bcode", Guna2PictureBox1.Text)

                command1.ExecuteNonQuery()
                MessageBox.Show($"Student recorded!")

            End Using
        Catch ex As Exception
            MessageBox.Show($"Error inserting Student")

        Finally
            con.Close()
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