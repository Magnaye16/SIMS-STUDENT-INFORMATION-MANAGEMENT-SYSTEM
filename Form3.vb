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
        'clear 

    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        'create 
        'validation  
        If Guna2TextBox1.Text = "" Or
           Guna2TextBox2.Text = "" Or
           Guna2TextBox4.Text = "" Or
           Guna2TextBox5.Text = "" Or
           Guna2TextBox6.Text = "" Then
            MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Clear
            Return

        End If
        'check if user already already exist
        Dim user = Guna2TextBox5.Text
        Dim query = "SELECT COUNT(*) FROM user WHERE uname = @uname "
        openCon()

        Try

            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@uname", user)

                Dim count As Integer = command.ExecuteScalar

                If count > 0 Then
                    MessageBox.Show("This user already already exist.")
                    con.Close()
                    'Clear
                    Return
                ElseIf count = 0 Then
                    con.Close()
                    'insert all the info
                    'ADDUSER()
                    Dispose()
                    'Form1.Show()
                End If
            End Using
        Catch ex As Exception
        Finally
            'Clear()

        End Try
    End Sub
End Class