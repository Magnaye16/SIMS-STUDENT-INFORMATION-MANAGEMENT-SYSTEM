Imports System.CodeDom
Imports System.Net.Mail
Imports MySql.Data.MySqlClient

Public Class Form1
    Private Sub CapitalizeFirstLetter(sender As Object, e As EventArgs) Handles _
    Guna2TextBox1.TextChanged, Guna2TextBox2.TextChanged, Guna2TextBox3.TextChanged,
    Guna2TextBox4.TextChanged, Guna2TextBox5.TextChanged, Guna2TextBox8.TextChanged, Guna2TextBox9.TextChanged

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
    Private Sub Guna2TextBox8_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox8.KeyPress
        ' Allow only digits (0-9) and control keys (like Backspace)
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True ' Cancel the key press if it's not a number or control key
        End If
    End Sub

    Private Sub InsertProfessorData()
        Try
            ' Validate required fields
            If String.IsNullOrEmpty(Guna2TextBox1.Text) OrElse String.IsNullOrEmpty(Guna2TextBox2.Text) _
                OrElse String.IsNullOrEmpty(Guna2TextBox3.Text) _
                OrElse String.IsNullOrEmpty(Guna2TextBox5.Text) OrElse String.IsNullOrEmpty(Guna2TextBox8.Text) _
                OrElse String.IsNullOrEmpty(Guna2TextBox9.Text) Then
                MessageBox.Show("Please fill In all required fields.")
                Return
            End If

            ' Check if a similar professor already exists in the database
            Dim checkQuery As String = "SELECT COUNT(*) FROM professor_info WHERE last_name = @last_name " &
                                   "AND first_name = @first_name AND middle_name = @middle_name AND address = @address"
            openCon()
            Using cmd As New MySqlCommand(checkQuery, con)
                cmd.Parameters.AddWithValue("@last_name", Guna2TextBox1.Text)
                cmd.Parameters.AddWithValue("@first_name", Guna2TextBox2.Text)
                cmd.Parameters.AddWithValue("@middle_name", Guna2TextBox3.Text)
                cmd.Parameters.AddWithValue("@address", Guna2TextBox9.Text)


                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                If count > 0 Then
                    MessageBox.Show("A professor with the same name and address already exists in the database.")
                    Return
                End If
            End Using

            ' Prepare the SQL query for inserting professor data
            Dim query As String = "INSERT INTO professor_info (last_name, first_name, middle_name, suffix, email, mobile_no, address) " &
                              "VALUES (@last_name, @first_name, @middle_name, @suffix, @email, @mobile_no, @address)"

            ' Create and execute the insert command
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@last_name", Guna2TextBox1.Text)
                cmd.Parameters.AddWithValue("@first_name", Guna2TextBox2.Text)
                cmd.Parameters.AddWithValue("@middle_name", Guna2TextBox3.Text)
                cmd.Parameters.AddWithValue("@suffix", Guna2TextBox4.Text)
                cmd.Parameters.AddWithValue("@email", Guna2TextBox5.Text)
                cmd.Parameters.AddWithValue("@mobile_no", Guna2TextBox8.Text)
                cmd.Parameters.AddWithValue("@address", Guna2TextBox9.Text)

                cmd.ExecuteNonQuery()
            End Using

            ' Retrieve the professor_id of the newly inserted professor
            Dim getProfessorIdQuery As String = "SELECT LAST_INSERT_ID()"
            Using cmd As New MySqlCommand(getProfessorIdQuery, con)
                Dim professorId As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Guna2TextBox10.Text = professorId.ToString() ' Display the professor_id in Guna2TextBox10
            End Using

            MessageBox.Show("Professor data has been successfully inserted.")

        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub


    Private Sub SendProfessorEmail()
        InsertProfessorData()
        Try
            ' Validate email input and professor_id
            If String.IsNullOrEmpty(Guna2TextBox5.Text) OrElse String.IsNullOrEmpty(Guna2TextBox10.Text) Then
                MessageBox.Show("Professor's email and professor_id are required.")
                Return
            End If

            ' Generate a random 5-digit password
            Dim random As New Random()
            Dim password As String = random.Next(10000, 99999).ToString()

            ' Create the username (last_name + professor_id)
            Dim professor_id As String = Guna2TextBox10.Text.Trim()
            Dim username As String = Guna2TextBox1.Text.Trim()

            openCon()

            ' Check if the username already exists in the user_info table
            Dim checkUsernameQuery As String = "SELECT COUNT(*) FROM user_info WHERE username = @username"
            Using cmd As New MySqlCommand(checkUsernameQuery, con)
                cmd.Parameters.AddWithValue("@username", username)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                If count > 0 Then
                    MessageBox.Show("This professor already exists.")
                    Return
                End If
            End Using

            con.Close()

            ' Prepare the email message
            Dim mailMessage As New MailMessage()
            mailMessage.From = New MailAddress("qcuschool98@gmail.com") ' Your email address here
            mailMessage.To.Add(Guna2TextBox5.Text) ' Professor's email address from the text box
            mailMessage.Subject = "Your Username and Password"
            mailMessage.Body = $"Dear Professor {Guna2TextBox1.Text}, {Environment.NewLine}" &
                           $"Your ID number is: {Guna2TextBox10.Text}{Environment.NewLine}" &
                           $"Your username is: {username}{Environment.NewLine}" &
                           $"Your password is: {password}{Environment.NewLine}" &
                           "Please change your password after logging in."

            ' Configure SMTP client (using Gmail's SMTP server for this example)
            Dim smtpClient As New SmtpClient("smtp.gmail.com")
            smtpClient.Port = 587
            smtpClient.Credentials = New System.Net.NetworkCredential("qcuschool98@gmail.com", "fscr zkhn jzef etmd") ' Your email and password here
            smtpClient.EnableSsl = True

            ' Send the email
            smtpClient.Send(mailMessage)

            ' Now insert the username, password, and role into the user_info database
            Dim role As String = "professor" ' The role is predefined as 'professor'

            ' SQL query to insert into user_info table
            Dim query As String = "INSERT INTO user_info (username, password, role) VALUES (@username, @password, @role)"

            ' Open connection to the database
            openCon()

            ' Create and execute the command to insert data into user_info table
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@username", username)
                cmd.Parameters.AddWithValue("@password", password)
                cmd.Parameters.AddWithValue("@role", role)

                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("An email with the username and password has been sent to the professor and user info has been saved.")

        Catch ex As Exception
            MessageBox.Show("An error occurred while sending the email or inserting user info: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub


    'Dim query As String = "SELECT COUNT(*) FROM professor_info WHERE professor_id = @professor_id OR (last_name = @last_name AND first_name = @first_name)"

    Private Sub CreateBTN_Click(sender As Object, e As EventArgs) Handles CreateBTN.Click
        'If InsertProfessorData() Then
        SendProfessorEmail()

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2TextBox10.Enabled = False
    End Sub
End Class