Public Class Form2
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        switchPanel(Form3)
        Guna2Button1.FillColor = Color.FromArgb(30, 71, 125)
        Guna2Button1.ForeColor = Color.White
        Guna2Button1.Font = New Font(Guna2Button1.Font, FontStyle.Bold)
        Guna2Button2.Font = New Font(Guna2Button2.Font, FontStyle.Regular)
        Guna2Button2.FillColor = Color.White
        Guna2Button2.ForeColor = Color.Black
    End Sub
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        switchPanel(Form4)
        Guna2Button1.FillColor = Color.White
        Guna2Button1.ForeColor = Color.Black
        Guna2Button1.Font = New Font(Guna2Button1.Font, FontStyle.Regular)
        Guna2Button2.Font = New Font(Guna2Button2.Font, FontStyle.Bold)
        Guna2Button2.FillColor = Color.FromArgb(30, 71, 125)
        Guna2Button2.ForeColor = Color.White
    End Sub
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        switchPanel(enlistment)
        Guna2Button1.FillColor = Color.White
        Guna2Button1.ForeColor = Color.Black
        Guna2Button1.Font = New Font(Guna2Button1.Font, FontStyle.Regular)
        Guna2Button2.Font = New Font(Guna2Button2.Font, FontStyle.Bold)
        Guna2Button2.FillColor = Color.FromArgb(30, 71, 125)
        Guna2Button2.ForeColor = Color.White
    End Sub

    'FUNTIONSS
    Sub switchPanel(ByVal panel As Form)
        Guna2Panel1.Controls.Clear()
        panel.TopLevel = False
        Guna2Panel1.Controls.Add(panel)
        panel.Show()
    End Sub


End Class