namespace TekstilScada.UI.Views
{
    partial class ManualUsageReport_Control
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            pnlFilters = new Panel();
            btnGenerateReport = new Button();
            cmbMachines = new ComboBox();
            label3 = new Label();
            dtpEndTime = new DateTimePicker();
            label2 = new Label();
            dtpStartTime = new DateTimePicker();
            label1 = new Label();
            btnExportToExcel = new Button();
            dgvManualUsage = new DataGridView();
            pnlFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvManualUsage).BeginInit();
            SuspendLayout();
            // 
            // pnlFilters
            // 
            pnlFilters.Controls.Add(btnGenerateReport);
            pnlFilters.Controls.Add(cmbMachines);
            pnlFilters.Controls.Add(label3);
            pnlFilters.Controls.Add(dtpEndTime);
            pnlFilters.Controls.Add(label2);
            pnlFilters.Controls.Add(dtpStartTime);
            pnlFilters.Controls.Add(label1);
            pnlFilters.Controls.Add(btnExportToExcel);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Margin = new Padding(3, 2, 3, 2);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(788, 48);
            pnlFilters.TabIndex = 1;
            // 
            // btnGenerateReport
            // 
            btnGenerateReport.Location = new Point(570, 19);
            btnGenerateReport.Margin = new Padding(3, 2, 3, 2);
            btnGenerateReport.Name = "btnGenerateReport";
            btnGenerateReport.Size = new Size(105, 22);
            btnGenerateReport.TabIndex = 6;
            btnGenerateReport.Text = "Report";
            btnGenerateReport.UseVisualStyleBackColor = true;
            btnGenerateReport.Click += btnGenerateReport_Click;
            // 
            // cmbMachines
            // 
            cmbMachines.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMachines.FormattingEnabled = true;
            cmbMachines.Location = new Point(370, 19);
            cmbMachines.Margin = new Padding(3, 2, 3, 2);
            cmbMachines.Name = "cmbMachines";
            cmbMachines.Size = new Size(193, 23);
            cmbMachines.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(312, 23);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 4;
            label3.Text = "Machine:";
            // 
            // dtpEndTime
            // 
            dtpEndTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.Location = new Point(167, 19);
            dtpEndTime.Margin = new Padding(3, 2, 3, 2);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.Size = new Size(132, 23);
            dtpEndTime.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(149, 23);
            label2.Name = "label2";
            label2.Size = new Size(12, 15);
            label2.TabIndex = 2;
            label2.Text = "-";
            // 
            // dtpStartTime
            // 
            dtpStartTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.Location = new Point(12, 19);
            dtpStartTime.Margin = new Padding(3, 2, 3, 2);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.Size = new Size(132, 23);
            dtpStartTime.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 2);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 0;
            label1.Text = "Date Range:";
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new Point(678, 19);
            btnExportToExcel.Margin = new Padding(3, 2, 3, 2);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(106, 22);
            btnExportToExcel.TabIndex = 7;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            btnExportToExcel.Click += btnExportToExcel_Click;
            // 
            // dgvManualUsage
            // 
            dgvManualUsage.AllowUserToAddRows = false;
            dgvManualUsage.AllowUserToDeleteRows = false;
            dgvManualUsage.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvManualUsage.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvManualUsage.Dock = DockStyle.Fill;
            dgvManualUsage.Location = new Point(0, 48);
            dgvManualUsage.Margin = new Padding(3, 2, 3, 2);
            dgvManualUsage.Name = "dgvManualUsage";
            dgvManualUsage.ReadOnly = true;
            dgvManualUsage.RowHeadersWidth = 51;
            dgvManualUsage.RowTemplate.Height = 29;
            dgvManualUsage.Size = new Size(788, 402);
            dgvManualUsage.TabIndex = 2;
            // 
            // ManualUsageReport_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvManualUsage);
            Controls.Add(pnlFilters);
            Margin = new Padding(3, 2, 3, 2);
            Name = "ManualUsageReport_Control";
            Size = new Size(788, 450);
            Load += ManualUsageReport_Control_Load;
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvManualUsage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.ComboBox cmbMachines;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvManualUsage; // HATA GİDERİLDİ
        private System.Windows.Forms.Button btnExportToExcel;

    }
}