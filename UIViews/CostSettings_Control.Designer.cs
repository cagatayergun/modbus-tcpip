namespace TekstilScada.UI.Views
{
    partial class CostSettings_Control
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }
        #region Component Designer generated code
        private void InitializeComponent()
        {
            dgvCostParameters = new DataGridView();
            pnlBottom = new Panel();
            btnSave = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvCostParameters).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // dgvCostParameters
            // 
            dgvCostParameters.AllowUserToAddRows = false;
            dgvCostParameters.AllowUserToDeleteRows = false;
            dgvCostParameters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCostParameters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCostParameters.Dock = DockStyle.Fill;
            dgvCostParameters.Location = new Point(0, 0);
            dgvCostParameters.Margin = new Padding(3, 2, 3, 2);
            dgvCostParameters.Name = "dgvCostParameters";
            dgvCostParameters.RowHeadersWidth = 51;
            dgvCostParameters.RowTemplate.Height = 29;
            dgvCostParameters.Size = new Size(700, 412);
            dgvCostParameters.TabIndex = 0;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnSave);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 412);
            pnlBottom.Margin = new Padding(3, 2, 3, 2);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(700, 38);
            pnlBottom.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.Location = new Point(602, 8);
            btnSave.Margin = new Padding(3, 2, 3, 2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 22);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // CostSettings_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvCostParameters);
            Controls.Add(pnlBottom);
            Margin = new Padding(3, 2, 3, 2);
            Name = "CostSettings_Control";
            Size = new Size(700, 450);
            Load += CostSettings_Control_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCostParameters).EndInit();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.DataGridView dgvCostParameters;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnSave;
    }
}