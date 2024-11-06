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
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        attendanceDGV = New Guna.UI2.WinForms.Guna2DataGridView()
        CType(attendanceDGV, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' attendanceDGV
        ' 
        DataGridViewCellStyle4.BackColor = Color.White
        attendanceDGV.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle4
        DataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        DataGridViewCellStyle5.Font = New Font("Segoe UI", 9.0F)
        DataGridViewCellStyle5.ForeColor = Color.White
        DataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = DataGridViewTriState.True
        attendanceDGV.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        attendanceDGV.ColumnHeadersHeight = 4
        attendanceDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = Color.White
        DataGridViewCellStyle6.Font = New Font("Segoe UI", 9.0F)
        DataGridViewCellStyle6.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        DataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.False
        attendanceDGV.DefaultCellStyle = DataGridViewCellStyle6
        attendanceDGV.GridColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        attendanceDGV.Location = New Point(12, 12)
        attendanceDGV.Name = "attendanceDGV"
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
        attendanceDGV.ThemeStyle.HeaderStyle.Font = New Font("Segoe UI", 9.0F)
        attendanceDGV.ThemeStyle.HeaderStyle.ForeColor = Color.White
        attendanceDGV.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        attendanceDGV.ThemeStyle.HeaderStyle.Height = 4
        attendanceDGV.ThemeStyle.ReadOnly = False
        attendanceDGV.ThemeStyle.RowsStyle.BackColor = Color.White
        attendanceDGV.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        attendanceDGV.ThemeStyle.RowsStyle.Font = New Font("Segoe UI", 9.0F)
        attendanceDGV.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        attendanceDGV.ThemeStyle.RowsStyle.Height = 25
        attendanceDGV.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        attendanceDGV.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        ' 
        ' Form4
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
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
