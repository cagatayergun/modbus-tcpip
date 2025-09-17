// UIViews/ActionLogReport_Control.Designer.cs
namespace TekstilScada.UIViews
{
    partial class ActionLogReport_Control
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
            dtpStartDate = new DateTimePicker();
            dtpEndDate = new DateTimePicker();
            cmbUser = new ComboBox();
            txtDetails = new TextBox();
            btnFilter = new Button();
            dataGridView1 = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dtpStartDate
            // 
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Location = new Point(86, 15);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(106, 23);
            dtpStartDate.TabIndex = 0;
            // 
            // dtpEndDate
            // 
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(244, 15);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(106, 23);
            dtpEndDate.TabIndex = 1;
            // 
            // cmbUser
            // 
            cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUser.FormattingEnabled = true;
            cmbUser.Location = new Point(429, 15);
            cmbUser.Name = "cmbUser";
            cmbUser.Size = new Size(132, 23);
            cmbUser.TabIndex = 2;
            // 
            // txtDetails
            // 
            txtDetails.Location = new Point(642, 15);
            txtDetails.Name = "txtDetails";
            txtDetails.Size = new Size(176, 23);
            txtDetails.TabIndex = 3;
            // 
            // btnFilter
            // 
            btnFilter.Location = new Point(820, 15);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(66, 23);
            btnFilter.TabIndex = 4;
            btnFilter.Text = "Filter";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(18, 56);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 24;
            dataGridView1.Size = new Size(862, 356);
            dataGridView1.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 19);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 6;
            label1.Text = "Start";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(200, 19);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 7;
            label2.Text = "Finish:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(368, 19);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 8;
            label3.Text = "User:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(569, 19);
            label4.Name = "label4";
            label4.Size = new Size(72, 15);
            label4.TabIndex = 9;
            label4.Text = "Explanation:";
            // 
            // ActionLogReport_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Controls.Add(btnFilter);
            Controls.Add(txtDetails);
            Controls.Add(cmbUser);
            Controls.Add(dtpEndDate);
            Controls.Add(dtpStartDate);
            Name = "ActionLogReport_Control";
            Size = new Size(897, 422);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}