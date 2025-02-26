﻿Imports DocumentFormat.OpenXml.Drawing.Diagrams
Imports DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net.Mail
Imports System.Runtime.ConstrainedExecution
Imports System.Text.RegularExpressions
Imports System.Transactions
Imports ZXing

Public Class studentCRUD

    Private Sub CapitalizeFirstLetter(sender As Object, e As EventArgs) Handles _
    Guna2TextBox1.TextChanged, Guna2TextBox2.TextChanged, Guna2TextBox3.TextChanged,
    Guna2TextBox4.TextChanged, Guna2TextBox5.TextChanged, Guna2TextBox6.TextChanged,
    Guna2TextBox10.TextChanged, Guna2TextBox8.TextChanged, Guna2TextBox9.TextChanged

        Dim textBox As Guna.UI2.WinForms.Guna2TextBox = CType(sender, Guna.UI2.WinForms.Guna2TextBox)

        ' Capitalize the first letter of each word in the text
        textBox.Text = CapitalizeWords(textBox.Text)

        ' Move the cursor to the end of the text after capitalization
        textBox.SelectionStart = textBox.Text.Length
    End Sub

    ' Function to capitalize the first letter of each word in a string
    Private Function CapitalizeWords(input As String) As String
        ' Split the input string into words
        Dim words As String() = input.Split(" "c)

        ' Capitalize each word
        For i As Integer = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1).ToLower()
            End If
        Next

        ' Join the words back into a single string and return it
        Return String.Join(" ", words)
    End Function
    Private Sub Guna2TextBox7_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox7.KeyPress
        ' Allow digits (0-9), hyphen (-), and control keys (like Backspace)
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "-"c AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True ' Cancel the key press if it's not a number, hyphen, or control key
        End If

        ' Ensure that only one hyphen can be typed (if required)
        ' Optionally, ensure the hyphen can only be typed at the beginning of the input
        If e.KeyChar = "-"c Then
            ' If there is already a hyphen or the hyphen is not at the start, cancel the input
            If Guna2TextBox7.Text.Contains("-") OrElse Guna2TextBox7.SelectionStart > 0 Then
                e.Handled = True
            End If
        End If
    End Sub


    Private ReadOnly time_start As Object

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Autogencode()
        Dim filename = Guna2TextBox7.Text ' <<< UID DAPAT (TEMPORARY)
        Dim filePath = "C:\Users\John Roi\source\repos\SMS(Student Management System)\Generated QR\" + filename + ".jpg"
        Dim qrCodeBitmap As Bitmap = Guna2PictureBox1.Image
        qrCodeBitmap.Save(filePath, ImageFormat.Png)
        MsgBox("Code has been generated and saved as " & filePath)
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Clear()
    End Sub

    Private Sub CreateBTN_Click(sender As Object, e As EventArgs) Handles CreateBTN.Click
        Dim studentYear As Integer
        Dim userInput As String = Guna2TextBox9.Text.Trim()
        Dim student_number = Guna2TextBox7.Text

        If Guna2TextBox1.Text = "" Or
           Guna2TextBox4.Text = "" Or
           Guna2TextBox6.Text = "" Or
           Guna2TextBox5.Text = "" Or
           Guna2TextBox3.Text = "" Or
           Guna2TextBox7.Text = "" Or
           Guna2TextBox8.Text = "" Or
           Guna2TextBox9.Text = "" Or
           Guna2ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not Integer.TryParse(Guna2TextBox6.Text, studentYear) OrElse studentYear < 1 OrElse studentYear > 4 Then
            MessageBox.Show("Please enter valid year", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not IsValidPhoneNumber(userInput) Then
            MessageBox.Show("Invalid phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If


        Dim query = "SELECT COUNT(*) FROM student_info WHERE student_id = @student_number"
        Try
            openCon()
            Using command As New MySqlCommand(query, con)
                command.Parameters.AddWithValue("@student_number", student_number)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())

                If count > 0 Then
                    MessageBox.Show("This student already exists.")
                    con.Close()
                    Return
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Dim email As String = Guna2TextBox8.Text
        If Not IsValidEmail(email) Then
            MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else
            SendEmail(email)
            Clear()
        End If


        'create 
        'validation  
        'If Guna2TextBox1.Text = "" Or
        '   Guna2TextBox2.Text = "" Or
        '   Guna2TextBox4.Text = "" Or
        '   Guna2TextBox5.Text = "" Or
        '   Guna2TextBox6.Text = "" Or
        '   Guna2TextBox7.Text = "" Or
        '   Guna2TextBox8.Text = "" Or
        '   Guna2TextBox9.Text = "" Or
        '   Guna2ComboBox1.Text = "" Or
        '   Guna2ComboBox2.Text = "" Then
        '    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    'Clear
        '    Return
        'Else
        '    ADDUSER()
        '    Dim email As String = Guna2TextBox8.Text

        '    If Not IsValidEmail(email) Then
        '        MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        Return
        '    Else
        '        SendEmail(email)
        '    End If
        'End If
        'checkerforadduser()

    End Sub





    'funtionss

    Private Sub Autogencode()
        'generate qr
        Dim qrstring As String = Guna2TextBox7.Text

        ' Create a barcode writer
        Dim barcodeWriter As New BarcodeWriter
        barcodeWriter.Format = BarcodeFormat.CODE_128 ' < barcode

        ' Set encoding properties (optional)
        barcodeWriter.Options = New Common.EncodingOptions With {
            .Width = 328,
            .Height = 126
        }

        'generate the qr code bitmap
        Dim qrCodeBitmap = barcodeWriter.Write(qrstring)

        'Display the QR code bitmap in a PictureBox Or save it to a file
        Guna2PictureBox1.Image = qrCodeBitmap
    End Sub

    'Private Sub insertdatatodocu()
    '    ' Path to the template document
    '    Dim templatePath As String = "C:\Users\John Roi\source\repos\tezt\docu\template.docx"

    '    ' Input values from the textboxes
    '    Dim studID As String = Guna2TextBox7.Text
    '    Dim lastname As String = Guna2TextBox1.Text
    '    Dim givenname As String = Guna2TextBox4.Text
    '    Dim middlename As String = Guna2TextBox2.Text
    '    Dim year As String = Guna2TextBox6.Text
    '    Dim section As String = Guna2TextBox5.Text
    '    Dim course As String = Guna2ComboBox1.Text
    '    Dim studstat As String = Guna2ComboBox2.Text
    '    Dim email As String = Guna2TextBox8.Text
    '    Dim contact As String = Guna2TextBox9.Text
    '    Dim address As String = Guna2TextBox3.Text


    '    Dim imagePath As String = "C:\Users\John Roi\source\repos\tezt\docu\" + lastname + "_" + studID ' Path to the image file

    '    ' Generate a customized file name based on student's name and current date/time
    '    Dim sanitizedStudentName As String = lastname.Replace(" ", "_") ' Remove spaces in the file name
    '    Dim dateTimeStamp As String = DateTime.Now.ToString("yyyyMMdd") ' Add a timestamp to the file name

    '    Dim newFileName As String = sanitizedStudentName & "_" & dateTimeStamp & ".docx"
    '    Dim newFilePath As String = Path.Combine("C:\Users\John Roi\source\repos\tezt\docu\generated docu\", newFileName)

    '    ' Copy the template file to a new file with the customized name
    '    File.Copy(templatePath, newFilePath, True) ' The True flag will overwrite if the file already exists

    '    ' Replace placeholders in the new document
    '    ReplaceTextInWordDocument(newFilePath, "{studID}", studID)
    '    ReplaceTextInWordDocument(newFilePath, "{lastName}", lastname)
    '    ReplaceTextInWordDocument(newFilePath, "{givenName}", givenname)
    '    ReplaceTextInWordDocument(newFilePath, "{middlename}", middlename)
    '    ReplaceTextInWordDocument(newFilePath, "{Year}", year)
    '    ReplaceTextInWordDocument(newFilePath, "{Section}", section)
    '    ReplaceTextInWordDocument(newFilePath, "{Course}", course)
    '    ReplaceTextInWordDocument(newFilePath, "{Status}", studstat)
    '    ReplaceTextInWordDocument(newFilePath, "{Email}", email)
    '    ReplaceTextInWordDocument(newFilePath, "{contact}", contact)
    '    ReplaceTextInWordDocument(newFilePath, "{Address}", address)
    '    ' Insert image at the placeholder
    '    InsertImageInWordDocument(newFilePath, "{Image}", imagePath)

    '    ' Inform the user that the document has been saved
    '    MessageBox.Show("Document created and saved successfully as " & newFilePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    'End Sub

    'Private Sub ReplaceTextInWordDocument(filePath As String, placeholder As String, replacementText As String)
    '    ' Open the existing Word document as read/write
    '    Using wordDoc As WordprocessingDocument = WordprocessingDocument.Open(filePath, True)
    '        ' Get the main document part
    '        Dim mainPart As MainDocumentPart = wordDoc.MainDocumentPart
    '        Dim documentBody As Body = mainPart.Document.Body

    '        ' Loop through all text elements in the document
    '        For Each textElement As Text In documentBody.Descendants(Of Text)()
    '            ' Check if the text contains the placeholder
    '            If textElement.Text.Contains(placeholder) Then
    '                ' Replace the placeholder with the actual value
    '                textElement.Text = textElement.Text.Replace(placeholder, replacementText)
    '            End If
    '        Next

    '        ' Save changes to the document
    '        mainPart.Document.Save()
    '    End Using
    'End Sub

    ' Method to insert an image into the Word document
    'Private Sub InsertImageInWordDocument(filePath As String, imagePlaceholder As String, imagePath As String)
    '    Using wordDoc As WordprocessingDocument = WordprocessingDocument.Open(filePath, True)
    '        Dim mainPart As MainDocumentPart = wordDoc.MainDocumentPart
    '        Dim documentBody As Body = mainPart.Document.Body

    '        ' Find the paragraph containing the image placeholder
    '        For Each paragraph As Paragraph In documentBody.Descendants(Of Paragraph)()
    '            For Each run As Run In paragraph.Descendants(Of Run)()
    '                For Each textElement As Text In run.Descendants(Of Text)()
    '                    If textElement.Text.Contains(imagePlaceholder) Then
    '                        ' Replace the placeholder text with an empty string (removing the placeholder)
    '                        textElement.Text = textElement.Text.Replace(imagePlaceholder, "")

    '                        ' Add an image part to the document
    '                        Dim imagePart As ImagePart = mainPart.AddImagePart(ImagePartType.Jpeg)

    '                        ' Feed the image data
    '                        Using stream As FileStream = New FileStream(imagePath, FileMode.Open)
    '                            imagePart.FeedData(stream)
    '                        End Using

    '                        ' Get the unique relationship ID for the image part
    '                        Dim relationshipId As String = mainPart.GetIdOfPart(imagePart)

    '                        ' Insert the image at the location of the placeholder
    '                        Dim element As New Drawing(
    '                            New DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
    '                                New DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent() With {.Cx = 990000L, .Cy = 792000L}, ' Image size in EMU
    '                                New DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent() With {.LeftEdge = 0L, .TopEdge = 0L, .RightEdge = 0L, .BottomEdge = 0L},
    '                                New DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties() With {.Id = 1UI, .Name = "Image 1"},
    '                                New DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
    '                                    New DocumentFormat.OpenXml.Drawing.GraphicFrameLocks() With {.NoChangeAspect = True}),
    '                                New DocumentFormat.OpenXml.Drawing.Graphic(
    '                                    New DocumentFormat.OpenXml.Drawing.GraphicData(
    '                                        New DocumentFormat.OpenXml.Drawing.Pictures.Picture(
    '                                            New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
    '                                                New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties() With {.Id = 0UI, .Name = "Image"},
    '                                                New DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
    '                                            ),
    '                                            New DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
    '                                                New DocumentFormat.OpenXml.Drawing.Blip() With {.Embed = relationshipId, .CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print},
    '                                                New DocumentFormat.OpenXml.Drawing.Stretch(New DocumentFormat.OpenXml.Drawing.FillRectangle())
    '                                            ),
    '                                            New DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
    '                                                New DocumentFormat.OpenXml.Drawing.Transform2D(
    '                                                    New DocumentFormat.OpenXml.Drawing.Offset() With {.X = 0L, .Y = 0L},
    '                                                    New DocumentFormat.OpenXml.Drawing.Extents() With {.Cx = 990000L, .Cy = 792000L}
    '                                                ),
    '                                                New DocumentFormat.OpenXml.Drawing.PresetGeometry(New DocumentFormat.OpenXml.Drawing.AdjustValueList()) With {.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle}
    '                                            )
    '                                        )
    '                                    ) With {.Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"}
    '                                )
    '                            )
    '                        )

    '                        ' Append the drawing to the Run
    '                        run.AppendChild(New Run(element))
    '                    End If
    '                Next
    '            Next
    '        Next

    '        ' Save changes to the document
    '        mainPart.Document.Save()
    '    End Using
    'End Sub


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

    Private Sub AddStudentAccount(username As String, password As String)
        Dim studentNumber = Guna2TextBox7.Text
        Dim userId As Integer

        Dim studentType As Integer
        Select Case Guna2ComboBox2.SelectedItem.ToString()
            Case "Regular"
                studentType = 1
            Case "Irregular"
                studentType = 2
        End Select

        Dim insertUserInfo As String = "INSERT INTO user_info (username, password, role) VALUES (@username, @password, @role)"
        Dim insertStudentInfo As String = "INSERT INTO student_info (student_id, last_name, first_name, middle_name, suffix, email, contact_number, address, student_type, student_status, user_id)
                                           VALUES (@student_id, @last_name, @first_name, @middle_name, @suffix, @email, @contact_number, @address, @student_type, @student_status, @user_id)"

        openCon()

        Using transaction As MySqlTransaction = con.BeginTransaction()
            Try

                Using command1 As New MySqlCommand(insertUserInfo, con, transaction)
                    command1.Parameters.AddWithValue("@username", username)
                    command1.Parameters.AddWithValue("@password", password)
                    command1.Parameters.AddWithValue("@role", "Student")
                    command1.ExecuteNonQuery()
                    userId = command1.LastInsertedId
                End Using

                Using command2 As New MySqlCommand(insertStudentInfo, con, transaction)
                    command2.Parameters.AddWithValue("@student_id", Guna2TextBox7.Text)
                    command2.Parameters.AddWithValue("@last_name", Guna2TextBox1.Text)
                    command2.Parameters.AddWithValue("@first_name", Guna2TextBox4.Text)
                    command2.Parameters.AddWithValue("@middle_name", Guna2TextBox2.Text)
                    command2.Parameters.AddWithValue("@suffix", Guna2TextBox10.Text)
                    command2.Parameters.AddWithValue("@email", Guna2TextBox8.Text)
                    command2.Parameters.AddWithValue("@contact_number", Guna2TextBox9.Text)
                    command2.Parameters.AddWithValue("@address", Guna2TextBox3.Text)
                    command2.Parameters.AddWithValue("@student_type", studentType)
                    command2.Parameters.AddWithValue("@student_status", "E")
                    command2.Parameters.AddWithValue("@user_id", userId)
                    command2.ExecuteNonQuery()
                End Using

                transaction.Commit()
                MessageBox.Show("Success.")

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Error occurred: " & ex.Message)
            End Try
        End Using

        con.Close()
    End Sub


    Public Sub ADDUSER()

        Dim studentNumber = Guna2TextBox7.Text
        Dim studentId As Integer


        Try
            openCon()

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
            MessageBox.Show($"Error inserting Student: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub Clear()
        Guna2TextBox1.Clear()
        Guna2TextBox2.Clear()
        Guna2TextBox3.Clear()
        Guna2TextBox4.Clear()
        Guna2TextBox5.Clear()
        Guna2TextBox6.Clear()
        Guna2TextBox7.Clear()
        Guna2TextBox8.Clear()
        Guna2TextBox9.Clear()
        Guna2ComboBox1.Text = Nothing
        Guna2ComboBox2.Text = Nothing
        Guna2PictureBox1.Image = Nothing

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


    Private Function IsValidEmail(email As String) As Boolean
        Dim emailPattern As String = "^[^@\s]+@[^@\s]+\.[^@\s]+$"
        Return Regex.IsMatch(email, emailPattern)
    End Function

    Private Sub SendEmail(email As String)
        Try
            Dim smtpClient As New SmtpClient("smtp.gmail.com")
            smtpClient.Port = 587
            smtpClient.Credentials = New Net.NetworkCredential("qcuschool98@gmail.com", "fscr zkhn jzef etmd")
            smtpClient.EnableSsl = True

            Dim usernameText As String = Guna2TextBox1.Text + "_" + Guna2TextBox4.Text
            Dim passwordText As String = GenerateRandomString(8)

            Dim mail As New MailMessage()
            mail.From = New MailAddress("qcuschool98@gmail.com")
            mail.To.Add(email)
            mail.Subject = "QCU Account"
            mail.Body = "You can now log in." & vbCrLf & "Username: " & usernameText & vbCrLf & "Password: " & passwordText

            smtpClient.Send(mail)
            AddStudentAccount(usernameText, passwordText)

            MessageBox.Show("Email sent successfully to " & email, "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Failed to send email: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GenerateRandomString(length As Integer) As String
        Dim characters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim random As New Random()
        Dim result As New System.Text.StringBuilder()

        For i As Integer = 1 To length
            Dim index As Integer = random.Next(0, characters.Length)
            result.Append(characters(index))
        Next

        Return result.ToString()
    End Function

    Private Function IsValidPhoneNumber(input As String) As Boolean
        Dim pattern As String = "^09\d{9}$"
        Return System.Text.RegularExpressions.Regex.IsMatch(input, pattern)
    End Function


End Class