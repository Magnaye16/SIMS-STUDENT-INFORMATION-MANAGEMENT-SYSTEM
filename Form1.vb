Imports System.Net.Mail
Imports MySql.Data.MySqlClient

Public Class Form1
    Private Sub InsertProfessorData()

        Try
            ' Validate required fields
            If String.IsNullOrEmpty(Guna2TextBox1.Text) OrElse String.IsNullOrEmpty(Guna2TextBox2.Text) _
            OrElse String.IsNullOrEmpty(Guna2TextBox3.Text) OrElse String.IsNullOrEmpty(Guna2TextBox4.Text) _
            OrElse String.IsNullOrEmpty(Guna2TextBox5.Text) OrElse String.IsNullOrEmpty(Guna2TextBox8.Text) _
            OrElse String.IsNullOrEmpty(Guna2TextBox9.Text) Then
                MessageBox.Show("Please fill In all required fields.")
                Return
            End If

            ' Prepare the SQL query for inserting professor data
            Dim query As String = "INSERT INTO professor_info (last_name, first_name, middle_name, suffix, email, mobile_no, address) " &
                              "VALUES (@last_name, @first_name, @middle_name, @suffix, @email, @mobile_no, @address)"

            ' Open database connection
            openCon()

            ' Create and execute the command
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
            Dim getProfessorIdQuery As String = "Select LAST_INSERT_ID()"
            Using cmd As New MySqlCommand(getProfessorIdQuery, con)
                Dim professorId As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Guna2TextBox10.Text = professorId.ToString() ' Display the professor_id in Guna2TextBox10
            End Using

            MessageBox.Show("Professor data has been successfully inserted.")

        Catch ex As Exception
            MessageBox.Show("An Error occurred: " & ex.Message)
        Finally
            ' Close connection if open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub SendProfessorEmail()
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
            Dim username As String = Guna2TextBox1.Text.Trim() & Guna2TextBox10.Text.Trim()

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

            MessageBox.Show("An email with your username and password has been sent to the professor and user info has been saved.")

        Catch ex As Exception
            MessageBox.Show("An error occurred while sending the email or inserting user info: " & ex.Message)
        End Try
    End Sub

    'Dim query As String = "SELECT COUNT(*) FROM professor_info WHERE professor_id = @professor_id OR (last_name = @last_name AND first_name = @first_name)"

    Private Sub CreateBTN_Click(sender As Object, e As EventArgs) Handles CreateBTN.Click
        InsertProfessorData()
        SendProfessorEmail()
    End Sub
End Class