Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net.Mail
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
        Dim filename = Guna2TextBox1.Text + "_" + Guna2TextBox7.Text ' <<< UID DAPAT (TEMPORARY)
        Dim filePath = "C:\Users\John Roi\source\repos\SMS(Student Management System)\Generated QR\" + filename + ".jpg"
        Dim qrCodeBitmap As Bitmap = Guna2PictureBox1.Image
        qrCodeBitmap.Save(filePath, ImageFormat.Png)
        MsgBox("Code has been generated and saved as " & filePath)
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        'Clear()
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
        checkerforadduser()



    End Sub





    'funtionss

    Private Sub insertdatatodocu()
        ' Path to the template document
        Dim templatePath As String = "C:\Users\John Roi\source\repos\tezt\docu\template.docx"

        ' Input values from the textboxes
        Dim studID As String = Guna2TextBox7.Text
        Dim lastname As String = Guna2TextBox1.Text
        Dim givenname As String = Guna2TextBox4.Text
        Dim middlename As String = Guna2TextBox2.Text
        Dim year As String = Guna2TextBox6.Text
        Dim section As String = Guna2TextBox5.Text
        Dim course As String = Guna2ComboBox1.Text
        Dim studstat As String = Guna2ComboBox2.Text
        Dim email As String = Guna2TextBox8.Text
        Dim contact As String = Guna2TextBox9.Text
        Dim address As String = Guna2TextBox3.Text


        Dim imagePath As String = "C:\Users\John Roi\source\repos\tezt\docu\" + lastname + "_" + studID' Path to the image file

        ' Generate a customized file name based on student's name and current date/time
        Dim sanitizedStudentName As String = lastname.Replace(" ", "_") ' Remove spaces in the file name
        Dim dateTimeStamp As String = DateTime.Now.ToString("yyyyMMdd") ' Add a timestamp to the file name

        Dim newFileName As String = sanitizedStudentName & "_" & dateTimeStamp & ".docx"
        Dim newFilePath As String = Path.Combine("C:\Users\John Roi\source\repos\tezt\docu\generated docu\", newFileName)

        ' Copy the template file to a new file with the customized name
        File.Copy(templatePath, newFilePath, True) ' The True flag will overwrite if the file already exists

        ' Replace placeholders in the new document
        ReplaceTextInWordDocument(newFilePath, "{studID}", studID)
        ReplaceTextInWordDocument(newFilePath, "{lastName}", lastname)
        ReplaceTextInWordDocument(newFilePath, "{givenName}", givenname)
        ReplaceTextInWordDocument(newFilePath, "{middlename}", middlename)
        ReplaceTextInWordDocument(newFilePath, "{Year}", year)
        ReplaceTextInWordDocument(newFilePath, "{Section}", section)
        ReplaceTextInWordDocument(newFilePath, "{Course}", course)
        ReplaceTextInWordDocument(newFilePath, "{Status}", studstat)
        ReplaceTextInWordDocument(newFilePath, "{Email}", email)
        ReplaceTextInWordDocument(newFilePath, "{contact}", contact)
        ReplaceTextInWordDocument(newFilePath, "{Address}", address)
        ' Insert image at the placeholder
        InsertImageInWordDocument(newFilePath, "{Image}", imagePath)

        ' Inform the user that the document has been saved
        MessageBox.Show("Document created and saved successfully as " & newFilePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ReplaceTextInWordDocument(filePath As String, placeholder As String, replacementText As String)
        ' Open the existing Word document as read/write
        Using wordDoc As WordprocessingDocument = WordprocessingDocument.Open(filePath, True)
            ' Get the main document part
            Dim mainPart As MainDocumentPart = wordDoc.MainDocumentPart
            Dim documentBody As Body = mainPart.Document.Body

            ' Loop through all text elements in the document
            For Each textElement As Text In documentBody.Descendants(Of Text)()
                ' Check if the text contains the placeholder
                If textElement.Text.Contains(placeholder) Then
                    ' Replace the placeholder with the actual value
                    textElement.Text = textElement.Text.Replace(placeholder, replacementText)
                End If
            Next

            ' Save changes to the document
            mainPart.Document.Save()
        End Using
    End Sub

    ' Method to insert an image into the Word document
    Private Sub InsertImageInWordDocument(filePath As String, imagePlaceholder As String, imagePath As String)
        Using wordDoc As WordprocessingDocument = WordprocessingDocument.Open(filePath, True)
            Dim mainPart As MainDocumentPart = wordDoc.MainDocumentPart
            Dim documentBody As Body = mainPart.Document.Body

            ' Find the paragraph containing the image placeholder
            For Each paragraph As Paragraph In documentBody.Descendants(Of Paragraph)()
                For Each run As Run In paragraph.Descendants(Of Run)()
                    For Each textElement As Text In run.Descendants(Of Text)()
                        If textElement.Text.Contains(imagePlaceholder) Then
                            ' Replace the placeholder text with an empty string (removing the placeholder)
                            textElement.Text = textElement.Text.Replace(imagePlaceholder, "")

                            ' Add an image part to the document
                            Dim imagePart As ImagePart = mainPart.AddImagePart(ImagePartType.Jpeg)

                            ' Feed the image data
                            Using stream As FileStream = New FileStream(imagePath, FileMode.Open)
                                imagePart.FeedData(stream)
                            End Using

                            ' Get the unique relationship ID for the image part
                            Dim relationshipId As String = mainPart.GetIdOfPart(imagePart)

                            ' Insert the image at the location of the placeholder
                            Dim element As New Drawing(
                                New DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                                    New DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent() With {.Cx = 990000L, .Cy = 792000L}, ' Image size in EMU
                                    New DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent() With {.LeftEdge = 0L, .TopEdge = 0L, .RightEdge = 0L, .BottomEdge = 0L},
                                    New DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties() With {.Id = 1UI, .Name = "Image 1"},
                                    New DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                                        New DocumentFormat.OpenXml.Drawing.GraphicFrameLocks() With {.NoChangeAspect = True}),
                                    New DocumentFormat.OpenXml.Drawing.Graphic(
                                        New DocumentFormat.OpenXml.Drawing.GraphicData(
                                            New DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                                New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                                    New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties() With {.Id = 0UI, .Name = "Image"},
                                                    New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
                                                ),
                                                New DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                                    New DocumentFormat.OpenXml.Drawing.Blip() With {.Embed = relationshipId, .CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print},
                                                    New DocumentFormat.OpenXml.Drawing.Stretch(New DocumentFormat.OpenXml.Drawing.FillRectangle())
                                                ),
                                                New DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                                    New DocumentFormat.OpenXml.Drawing.Transform2D(
                                                        New DocumentFormat.OpenXml.Drawing.Offset() With {.X = 0L, .Y = 0L},
                                                        New DocumentFormat.OpenXml.Drawing.Extents() With {.Cx = 990000L, .Cy = 792000L}
                                                    ),
                                                    New DocumentFormat.OpenXml.Drawing.PresetGeometry(New DocumentFormat.OpenXml.Drawing.AdjustValueList()) With {.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle}
                                                )
                                            )
                                        ) With {.Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"}
                                    )
                                )
                            )

                            ' Append the drawing to the Run
                            run.AppendChild(New Run(element))
                        End If
                    Next
                Next
            Next

            ' Save changes to the document
            mainPart.Document.Save()
        End Using
    End Sub


    Private Sub checkerforadduser()
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

    Public Sub Sendcreatedemail()
        Dim name As String = Guna2TextBox1.Text + ", " + Guna2TextBox4.Text + " " + Guna2TextBox2.Text
        Dim currentDate As DateTime = DateTime.Now
        Dim datenow As String = currentDate.ToString("yyyy-MM-dd")

        Try
            ' Set up the email configuration
            Dim smtpClient As New SmtpClient
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New System.Net.NetworkCredential("magnayejohnroi@gmail.com", "hynyggnexxswbcjm")
            smtpClient.Port = 587 ' Use the appropriate port for your SMTP server
            smtpClient.EnableSsl = True ' Enable SSL if required by your SMTP server
            smtpClient.Host = "smtp.gmail.com"

            ' Create the MailMessage object
            Dim mail As New MailMessage()
            mail.From = New MailAddress("magnayejohnroi@gmail.com")
            If Not String.IsNullOrEmpty(Guna2TextBox8.Text) Then
                mail.To.Add(Guna2TextBox8.Text)
            Else
                MessageBox.Show("Recipient's email address is empty.")
                Exit Sub ' Exit the subroutine if the recipient's email address is empty
            End If
            mail.Subject = "Inforamtion for " + name
            mail.Body = "Here is your payslip for this month" + Guna2TextBox1.Text

            ' Attach the image file
            Dim filename As String = name
            Dim attachment As New Attachment("C:\Users\John Roi\Documents\Visual Studio 2015\Projects\100+_Payroll\80+_Payroll\Payroll\Payroll\PAYSLIP PDF\" + name + "_" + datenow + ".pdf")
            mail.Attachments.Add(attachment)


            ' Send the email
            smtpClient.Send(mail)

            MessageBox.Show("Email sent successfully.")
        Catch ex As Exception
            MessageBox.Show("Error sending email: " & ex.Message)
        End Try
    End Sub


End Class