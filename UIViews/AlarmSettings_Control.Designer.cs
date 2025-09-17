// UI/Views/AlarmSettings_Control.Designer.cs
namespace TekstilScada.UI.Views
{
    partial class AlarmSettings_Control
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
            dgvAlarms = new DataGridView();
            groupBox1 = new GroupBox();
            btnDelete = new Button();
            btnSave = new Button();
            btnNew = new Button();
            txtCategory = new TextBox();
            label4 = new Label();
            numSeverity = new NumericUpDown();
            label3 = new Label();
            txtAlarmText = new TextBox();
            label2 = new Label();
            numAlarmNo = new NumericUpDown();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvAlarms).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSeverity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAlarmNo).BeginInit();
            SuspendLayout();
            // 
            // dgvAlarms
            // 
            dgvAlarms.AllowUserToAddRows = false;
            dgvAlarms.AllowUserToDeleteRows = false;
            dgvAlarms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAlarms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAlarms.Dock = DockStyle.Fill;
            dgvAlarms.Location = new Point(0, 0);
            dgvAlarms.Margin = new Padding(3, 2, 3, 2);
            dgvAlarms.MultiSelect = false;
            dgvAlarms.Name = "dgvAlarms";
            dgvAlarms.ReadOnly = true;
            dgvAlarms.RowHeadersWidth = 51;
            dgvAlarms.RowTemplate.Height = 29;
            dgvAlarms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlarms.Size = new Size(700, 450);
            dgvAlarms.TabIndex = 0;
            dgvAlarms.SelectionChanged += dgvAlarms_SelectionChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnDelete);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(btnNew);
            groupBox1.Controls.Add(txtCategory);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(numSeverity);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtAlarmText);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(numAlarmNo);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new Point(0, 262);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(700, 188);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Alarm Details";
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(595, 142);
            btnDelete.Margin = new Padding(3, 2, 3, 2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(82, 22);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(508, 142);
            btnSave.Margin = new Padding(3, 2, 3, 2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(82, 22);
            btnSave.TabIndex = 9;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnNew
            // 
            btnNew.Location = new Point(420, 142);
            btnNew.Margin = new Padding(3, 2, 3, 2);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(82, 22);
            btnNew.TabIndex = 8;
            btnNew.Text = "New";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            // 
            // txtCategory
            // 
            txtCategory.Location = new Point(114, 112);
            txtCategory.Margin = new Padding(3, 2, 3, 2);
            txtCategory.Name = "txtCategory";
            txtCategory.Size = new Size(219, 23);
            txtCategory.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 115);
            label4.Name = "label4";
            label4.Size = new Size(58, 15);
            label4.TabIndex = 6;
            label4.Text = "Category:";
            // 
            // numSeverity
            // 
            numSeverity.Location = new Point(114, 82);
            numSeverity.Margin = new Padding(3, 2, 3, 2);
            numSeverity.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            numSeverity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSeverity.Name = "numSeverity";
            numSeverity.Size = new Size(131, 23);
            numSeverity.TabIndex = 5;
            numSeverity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 84);
            label3.Name = "label3";
            label3.Size = new Size(51, 15);
            label3.TabIndex = 4;
            label3.Text = "Severity:";
            // 
            // txtAlarmText
            // 
            txtAlarmText.Location = new Point(114, 52);
            txtAlarmText.Margin = new Padding(3, 2, 3, 2);
            txtAlarmText.Name = "txtAlarmText";
            txtAlarmText.Size = new Size(438, 23);
            txtAlarmText.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 55);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 2;
            label2.Text = "Alarm Text:";
            // 
            // numAlarmNo
            // 
            numAlarmNo.Location = new Point(114, 22);
            numAlarmNo.Margin = new Padding(3, 2, 3, 2);
            numAlarmNo.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numAlarmNo.Name = "numAlarmNo";
            numAlarmNo.Size = new Size(131, 23);
            numAlarmNo.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 24);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 0;
            label1.Text = "Alarm Number:";
            // 
            // AlarmSettings_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(dgvAlarms);
            Margin = new Padding(3, 2, 3, 2);
            Name = "AlarmSettings_Control";
            Size = new Size(700, 450);
            Load += AlarmSettings_Control_Load;
            ((System.ComponentModel.ISupportInitialize)dgvAlarms).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSeverity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAlarmNo).EndInit();
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.DataGridView dgvAlarms;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numAlarmNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAlarmText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSeverity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
    }
}