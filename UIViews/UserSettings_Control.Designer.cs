// UI/Views/UserSettings_Control.Designer.cs
namespace TekstilScada.UI.Views
{
    partial class UserSettings_Control
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvUsers = new DataGridView();
            groupBox1 = new GroupBox();
            btnNew = new Button();
            btnSave = new Button();
            btnDelete = new Button();
            clbRoles = new CheckedListBox();
            label5 = new Label();
            chkIsActive = new CheckBox();
            txtPassword = new TextBox();
            label4 = new Label();
            txtFullName = new TextBox();
            label3 = new Label();
            txtUsername = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvUsers
            // 
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.Location = new Point(0, 0);
            dgvUsers.Margin = new Padding(3, 2, 3, 2);
            dgvUsers.MultiSelect = false;
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.RowTemplate.Height = 29;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(700, 450);
            dgvUsers.TabIndex = 0;
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnNew);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(btnDelete);
            groupBox1.Controls.Add(clbRoles);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(chkIsActive);
            groupBox1.Controls.Add(txtPassword);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtFullName);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtUsername);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new Point(0, 262);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(700, 188);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "User Details";
            // 
            // btnNew
            // 
            btnNew.Location = new Point(420, 142);
            btnNew.Margin = new Padding(3, 2, 3, 2);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(82, 22);
            btnNew.TabIndex = 10;
            btnNew.Text = "New";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(508, 142);
            btnSave.Margin = new Padding(3, 2, 3, 2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(82, 22);
            btnSave.TabIndex = 11;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(595, 142);
            btnDelete.Margin = new Padding(3, 2, 3, 2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(82, 22);
            btnDelete.TabIndex = 12;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // clbRoles
            // 
            clbRoles.FormattingEnabled = true;
            clbRoles.Location = new Point(394, 38);
            clbRoles.Margin = new Padding(3, 2, 3, 2);
            clbRoles.Name = "clbRoles";
            clbRoles.Size = new Size(176, 76);
            clbRoles.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(394, 19);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 8;
            label5.Text = "Roles:";
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Location = new Point(114, 142);
            chkIsActive.Margin = new Padding(3, 2, 3, 2);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(85, 19);
            chkIsActive.TabIndex = 7;
            chkIsActive.Text = "User Active";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(114, 98);
            txtPassword.Margin = new Padding(3, 2, 3, 2);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(219, 23);
            txtPassword.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 100);
            label4.Name = "label4";
            label4.Size = new Size(95, 15);
            label4.TabIndex = 5;
            label4.Text = "Password (New):";
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(114, 68);
            txtFullName.Margin = new Padding(3, 2, 3, 2);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(219, 23);
            txtFullName.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 70);
            label3.Name = "label3";
            label3.Size = new Size(92, 15);
            label3.TabIndex = 3;
            label3.Text = "Name Surname:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(114, 38);
            txtUsername.Margin = new Padding(3, 2, 3, 2);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(219, 23);
            txtUsername.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 40);
            label1.Name = "label1";
            label1.Size = new Size(66, 15);
            label1.TabIndex = 0;
            label1.Text = "User name:";
            // 
            // UserSettings_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(dgvUsers);
            Margin = new Padding(3, 2, 3, 2);
            Name = "UserSettings_Control";
            Size = new Size(700, 450);
            Load += UserSettings_Control_Load;
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.CheckedListBox clbRoles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
    }
}
