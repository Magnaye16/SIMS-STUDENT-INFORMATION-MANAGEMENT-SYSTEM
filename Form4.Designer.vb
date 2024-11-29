<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form4
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        attendanceDGV = New Guna.UI2.WinForms.Guna2DataGridView()
        CType(attendanceDGV, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' attendanceDGV
        ' 
        attendanceDGV.AllowUserToAddRows = False
        attendanceDGV.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = Color.White
        attendanceDGV.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.White
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        attendanceDGV.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        attendanceDGV.ColumnHeadersHeight = 4
        attendanceDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = Color.White
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        DataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.False
        attendanceDGV.DefaultCellStyle = DataGridViewCellStyle3
        attendanceDGV.GridColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        attendanceDGV.Location = New Point(12, 12)
        attendanceDGV.Name = "attendanceDGV"
        attendanceDGV.ReadOnly = True
        attendanceDGV.RowHeadersVisible = False
        attendanceDGV.Size = New Size(802, 552)
        attendanceDGV.TabIndex = 0
        attendanceDGV.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White
        attendanceDGV.ThemeStyle.AlternatingRowsStyle.Font = Nothing
        attendanceDGV.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty
        attendanceDGV.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty
        attendanceDGV.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty
        attendanceDGV.ThemeStyle.BackColor = Color.White
        attendanceDGV.ThemeStyle.GridColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        attendanceDGV.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        attendanceDGV.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None
        attendanceDGV.ThemeStyle.HeaderStyle.Font = New Font("Segoe UI", 9F)
        attendanceDGV.ThemeStyle.HeaderStyle.ForeColor = Color.White
        attendanceDGV.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        attendanceDGV.ThemeStyle.HeaderStyle.Height = 4
        attendanceDGV.ThemeStyle.ReadOnly = True
        attendanceDGV.ThemeStyle.RowsStyle.BackColor = Color.White
        attendanceDGV.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        attendanceDGV.ThemeStyle.RowsStyle.Font = New Font("Segoe UI", 9F)
        attendanceDGV.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        attendanceDGV.ThemeStyle.RowsStyle.Height = 25
        attendanceDGV.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        attendanceDGV.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        ' 
        ' Form4
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(826, 576)
        Controls.Add(attendanceDGV)
        FormBorderStyle = FormBorderStyle.None
        Name = "Form4"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Form4"
        CType(attendanceDGV, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents attendanceDGV As Guna.UI2.WinForms.Guna2DataGridView
End Class
