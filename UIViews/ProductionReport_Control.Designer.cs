// UI/Views/ProductionReport_Control.Designer.cs
namespace TekstilScada.UI.Views
{
    partial class ProductionReport_Control
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
            btnExportToExcel = new Button();
            txtOperator = new TextBox();
            label9 = new Label();
            txtCustomerNo = new TextBox();
            label8 = new Label();
            txtOrderNo = new TextBox();
            label7 = new Label();
            txtRecipeName = new TextBox();
            label5 = new Label();
            txtBatchNo = new TextBox();
            label4 = new Label();
            btnGenerateReport = new Button();
            cmbMachines = new ComboBox();
            label3 = new Label();
            dtpEndTime = new DateTimePicker();
            label2 = new Label();
            dtpStartTime = new DateTimePicker();
            label1 = new Label();
            dgvProductionReport = new DataGridView();
            pnlFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductionReport).BeginInit();
            SuspendLayout();
            // 
            // pnlFilters
            // 
            pnlFilters.Controls.Add(btnExportToExcel);
            pnlFilters.Controls.Add(txtOperator);
            pnlFilters.Controls.Add(label9);
            pnlFilters.Controls.Add(txtCustomerNo);
            pnlFilters.Controls.Add(label8);
            pnlFilters.Controls.Add(txtOrderNo);
            pnlFilters.Controls.Add(label7);
            pnlFilters.Controls.Add(txtRecipeName);
            pnlFilters.Controls.Add(label5);
            pnlFilters.Controls.Add(txtBatchNo);
            pnlFilters.Controls.Add(label4);
            pnlFilters.Controls.Add(btnGenerateReport);
            pnlFilters.Controls.Add(cmbMachines);
            pnlFilters.Controls.Add(label3);
            pnlFilters.Controls.Add(dtpEndTime);
            pnlFilters.Controls.Add(label2);
            pnlFilters.Controls.Add(dtpStartTime);
            pnlFilters.Controls.Add(label1);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Margin = new Padding(3, 2, 3, 2);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(970, 90);
            pnlFilters.TabIndex = 0;
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new Point(848, 63);
            btnExportToExcel.Margin = new Padding(3, 2, 3, 2);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(105, 22);
            btnExportToExcel.TabIndex = 17;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            btnExportToExcel.Click += btnExportToExcel_Click;
            // 
            // txtOperator
            // 
            txtOperator.Location = new Point(764, 36);
            txtOperator.Margin = new Padding(3, 2, 3, 2);
            txtOperator.Name = "txtOperator";
            txtOperator.Size = new Size(189, 23);
            txtOperator.TabIndex = 16;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(697, 39);
            label9.Name = "label9";
            label9.Size = new Size(57, 15);
            label9.TabIndex = 15;
            label9.Text = "Operator:";
            // 
            // txtCustomerNo
            // 
            txtCustomerNo.Location = new Point(480, 36);
            txtCustomerNo.Margin = new Padding(3, 2, 3, 2);
            txtCustomerNo.Name = "txtCustomerNo";
            txtCustomerNo.Size = new Size(178, 23);
            txtCustomerNo.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(359, 39);
            label8.Name = "label8";
            label8.Size = new Size(109, 15);
            label8.TabIndex = 13;
            label8.Text = "Customer Number:";
            // 
            // txtOrderNo
            // 
            txtOrderNo.Location = new Point(231, 36);
            txtOrderNo.Margin = new Padding(3, 2, 3, 2);
            txtOrderNo.Name = "txtOrderNo";
            txtOrderNo.Size = new Size(114, 23);
            txtOrderNo.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(158, 39);
            label7.Name = "label7";
            label7.Size = new Size(59, 15);
            label7.TabIndex = 11;
            label7.Text = "Order No:";
            // 
            // txtRecipeName
            // 
            txtRecipeName.Location = new Point(764, 9);
            txtRecipeName.Margin = new Padding(3, 2, 3, 2);
            txtRecipeName.Name = "txtRecipeName";
            txtRecipeName.Size = new Size(189, 23);
            txtRecipeName.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(686, 12);
            label5.Name = "label5";
            label5.Size = new Size(80, 15);
            label5.TabIndex = 9;
            label5.Text = "Recipe Name:";
            // 
            // txtBatchNo
            // 
            txtBatchNo.Location = new Point(74, 36);
            txtBatchNo.Margin = new Padding(3, 2, 3, 2);
            txtBatchNo.Name = "txtBatchNo";
            txtBatchNo.Size = new Size(79, 23);
            txtBatchNo.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 39);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 7;
            label4.Text = "Batch No:";
            // 
            // btnGenerateReport
            // 
            btnGenerateReport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGenerateReport.Location = new Point(751, 63);
            btnGenerateReport.Margin = new Padding(3, 2, 3, 2);
            btnGenerateReport.Name = "btnGenerateReport";
            btnGenerateReport.Size = new Size(91, 22);
            btnGenerateReport.TabIndex = 6;
            btnGenerateReport.Text = "Report";
            btnGenerateReport.UseVisualStyleBackColor = true;
            btnGenerateReport.Click += btnGenerateReport_Click;
            // 
            // cmbMachines
            // 
            cmbMachines.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMachines.FormattingEnabled = true;
            cmbMachines.Location = new Point(480, 9);
            cmbMachines.Margin = new Padding(3, 2, 3, 2);
            cmbMachines.Name = "cmbMachines";
            cmbMachines.Size = new Size(178, 23);
            cmbMachines.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(412, 12);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 4;
            label3.Text = "Machine:";
            // 
            // dtpEndTime
            // 
            dtpEndTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.Location = new Point(216, 9);
            dtpEndTime.Margin = new Padding(3, 2, 3, 2);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.Size = new Size(132, 23);
            dtpEndTime.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label2.Location = new Point(198, 10);
            label2.Name = "label2";
            label2.Size = new Size(15, 19);
            label2.TabIndex = 2;
            label2.Text = "-";
            // 
            // dtpStartTime
            // 
            dtpStartTime.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.Location = new Point(80, 9);
            dtpStartTime.Margin = new Padding(3, 2, 3, 2);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.Size = new Size(115, 23);
            dtpStartTime.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 12);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 0;
            label1.Text = "Date Range:";
            // 
            // dgvProductionReport
            // 
            dgvProductionReport.AllowUserToAddRows = false;
            dgvProductionReport.AllowUserToDeleteRows = false;
            dgvProductionReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductionReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductionReport.Dock = DockStyle.Fill;
            dgvProductionReport.Location = new Point(0, 90);
            dgvProductionReport.Margin = new Padding(3, 2, 3, 2);
            dgvProductionReport.Name = "dgvProductionReport";
            dgvProductionReport.ReadOnly = true;
            dgvProductionReport.RowHeadersWidth = 51;
            dgvProductionReport.RowTemplate.Height = 29;
            dgvProductionReport.Size = new Size(970, 360);
            dgvProductionReport.TabIndex = 1;
            dgvProductionReport.CellDoubleClick += dgvProductionReport_CellDoubleClick;
            // 
            // ProductionReport_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvProductionReport);
            Controls.Add(pnlFilters);
            Margin = new Padding(3, 2, 3, 2);
            Name = "ProductionReport_Control";
            Size = new Size(970, 450);
            Load += ProductionReport_Control_Load;
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductionReport).EndInit();
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
        private System.Windows.Forms.DataGridView dgvProductionReport;
        private System.Windows.Forms.TextBox txtBatchNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRecipeName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOrderNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCustomerNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnExportToExcel;
    }
}