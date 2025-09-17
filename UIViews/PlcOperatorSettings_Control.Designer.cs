// UI/Views/PlcOperatorSettings_Control.Designer.cs
namespace TekstilScada.UI.Views
{
    partial class PlcOperatorSettings_Control
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
            label1 = new Label();
            cmbMachines = new ComboBox();
            label2 = new Label();
            cmbSlot = new ComboBox();
            btnSend = new Button();
            btnRead = new Button();
            dgvOperators = new DataGridView();
            btnDelete = new Button();
            ekle = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvOperators).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 15);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 0;
            label1.Text = "Target Machine:";
            // 
            // cmbMachines
            // 
            cmbMachines.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMachines.FormattingEnabled = true;
            cmbMachines.Location = new Point(105, 13);
            cmbMachines.Margin = new Padding(3, 2, 3, 2);
            cmbMachines.Name = "cmbMachines";
            cmbMachines.Size = new Size(219, 23);
            cmbMachines.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(341, 15);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 2;
            label2.Text = "User Order:";
            // 
            // cmbSlot
            // 
            cmbSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSlot.FormattingEnabled = true;
            cmbSlot.Location = new Point(415, 13);
            cmbSlot.Margin = new Padding(3, 2, 3, 2);
            cmbSlot.Name = "cmbSlot";
            cmbSlot.Size = new Size(53, 23);
            cmbSlot.TabIndex = 3;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(474, 13);
            btnSend.Margin = new Padding(3, 2, 3, 2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(209, 22);
            btnSend.TabIndex = 4;
            btnSend.Text = "Send Selected Template to PLC ->";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // btnRead
            // 
            btnRead.Location = new Point(474, 39);
            btnRead.Margin = new Padding(3, 2, 3, 2);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(209, 22);
            btnRead.TabIndex = 5;
            btnRead.Text = "<- Read Operator in PLC";
            btnRead.UseVisualStyleBackColor = true;
            btnRead.Click += btnRead_Click;
            // 
            // dgvOperators
            // 
            dgvOperators.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOperators.Location = new Point(18, 75);
            dgvOperators.Margin = new Padding(3, 2, 3, 2);
            dgvOperators.Name = "dgvOperators";
            dgvOperators.RowHeadersWidth = 51;
            dgvOperators.RowTemplate.Height = 29;
            dgvOperators.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOperators.Size = new Size(665, 262);
            dgvOperators.TabIndex = 6;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(569, 345);
            btnDelete.Margin = new Padding(3, 2, 3, 2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(114, 22);
            btnDelete.TabIndex = 7;
            btnDelete.Text = "Delete Selected Template";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // ekle
            // 
            ekle.Location = new Point(18, 345);
            ekle.Margin = new Padding(3, 2, 3, 2);
            ekle.Name = "ekle";
            ekle.Size = new Size(114, 22);
            ekle.TabIndex = 8;
            ekle.Text = "Add new User";
            ekle.UseVisualStyleBackColor = true;
            ekle.Click += ekle_Click;
            // 
            // PlcOperatorSettings_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ekle);
            Controls.Add(btnDelete);
            Controls.Add(dgvOperators);
            Controls.Add(btnRead);
            Controls.Add(btnSend);
            Controls.Add(cmbSlot);
            Controls.Add(label2);
            Controls.Add(cmbMachines);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "PlcOperatorSettings_Control";
            Size = new Size(700, 375);
            Load += PlcOperatorSettings_Control_Load;
            ((System.ComponentModel.ISupportInitialize)dgvOperators).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMachines;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSlot;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.DataGridView dgvOperators;
        private System.Windows.Forms.Button btnDelete;
        private Button ekle;
    }
}
