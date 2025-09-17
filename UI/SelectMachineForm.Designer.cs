// UI/SelectMachineForm.Designer.cs
namespace TekstilScada.UI
{
    partial class SelectMachineForm
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
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            label1 = new Label();
            lstMachines = new ListBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.2F);
            label1.Location = new Point(10, 7);
            label1.Name = "label1";
            label1.Size = new Size(268, 19);
            label1.TabIndex = 0;
            label1.Text = "Please select the machine to be processed:";
            // 
            // lstMachines
            // 
            lstMachines.FormattingEnabled = true;
            lstMachines.ItemHeight = 15;
            lstMachines.Location = new Point(10, 30);
            lstMachines.Margin = new Padding(3, 2, 3, 2);
            lstMachines.Name = "lstMachines";
            lstMachines.Size = new Size(316, 154);
            lstMachines.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(149, 195);
            btnOk.Margin = new Padding(3, 2, 3, 2);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(82, 22);
            btnOk.TabIndex = 2;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(243, 195);
            btnCancel.Margin = new Padding(3, 2, 3, 2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(82, 22);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // SelectMachineForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(336, 226);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(lstMachines);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SelectMachineForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Machine";
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstMachines;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}