namespace TekstilScada.UI
{
    partial class FtpSync_Form
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
            pnlTop = new Panel();
            clbMachines = new CheckedListBox();
            label1 = new Label();
            splitContainer1 = new SplitContainer();
            groupBox1 = new GroupBox();
            lstLocalRecipes = new ListBox();
            pnlMiddle = new Panel();
            btnReceive = new Button();
            btnSend = new Button();
            groupBox2 = new GroupBox();
            lstHmiRecipes = new ListBox();
            panel1 = new Panel();
            btnRefreshHmi = new Button();
            dgvTransfers = new DataGridView();
            tabControlMain = new TabControl();
            tabPageTransfers = new TabPage();
            tabPagePreview = new TabPage();
            pnlPreviewArea = new Panel();
            lblPreviewStatus = new Label();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            pnlMiddle.SuspendLayout();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTransfers).BeginInit();
            tabControlMain.SuspendLayout();
            tabPageTransfers.SuspendLayout();
            tabPagePreview.SuspendLayout();
            pnlPreviewArea.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(clbMachines);
            pnlTop.Controls.Add(label1);
            pnlTop.Dock = DockStyle.Right;
            pnlTop.Location = new Point(896, 8);
            pnlTop.Margin = new Padding(3, 2, 3, 2);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(257, 570);
            pnlTop.TabIndex = 0;
            // 
            // clbMachines
            // 
            clbMachines.FormattingEnabled = true;
            clbMachines.Location = new Point(4, 27);
            clbMachines.Margin = new Padding(3, 2, 3, 2);
            clbMachines.Name = "clbMachines";
            clbMachines.Size = new Size(248, 526);
            clbMachines.TabIndex = 1;
            clbMachines.ItemCheck += clbMachines_ItemCheck;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(84, 10);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 0;
            label1.Text = "Target Machines";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Top;
            splitContainer1.Location = new Point(9, 8);
            splitContainer1.Margin = new Padding(3, 2, 3, 2);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(pnlMiddle);
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Size = new Size(887, 280);
            splitContainer1.SplitterDistance = 387;
            splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lstLocalRecipes);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(387, 280);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "LOCAL RECIPE";
            // 
            // lstLocalRecipes
            // 
            lstLocalRecipes.Dock = DockStyle.Fill;
            lstLocalRecipes.FormattingEnabled = true;
            lstLocalRecipes.ItemHeight = 15;
            lstLocalRecipes.Location = new Point(3, 18);
            lstLocalRecipes.Margin = new Padding(3, 2, 3, 2);
            lstLocalRecipes.Name = "lstLocalRecipes";
            lstLocalRecipes.SelectionMode = SelectionMode.MultiExtended;
            lstLocalRecipes.Size = new Size(381, 260);
            lstLocalRecipes.TabIndex = 0;
            // 
            // pnlMiddle
            // 
            pnlMiddle.Controls.Add(btnReceive);
            pnlMiddle.Controls.Add(btnSend);
            pnlMiddle.Dock = DockStyle.Fill;
            pnlMiddle.Location = new Point(0, 0);
            pnlMiddle.Margin = new Padding(3, 2, 3, 2);
            pnlMiddle.Name = "pnlMiddle";
            pnlMiddle.Size = new Size(128, 280);
            pnlMiddle.TabIndex = 1;
            // 
            // btnReceive
            // 
            btnReceive.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnReceive.Location = new Point(14, 98);
            btnReceive.Margin = new Padding(3, 2, 3, 2);
            btnReceive.Name = "btnReceive";
            btnReceive.Size = new Size(102, 38);
            btnReceive.TabIndex = 1;
            btnReceive.Text = "<<Recive";
            btnReceive.UseVisualStyleBackColor = true;
            btnReceive.Click += btnReceive_Click;
            // 
            // btnSend
            // 
            btnSend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSend.Location = new Point(14, 45);
            btnSend.Margin = new Padding(3, 2, 3, 2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(102, 38);
            btnSend.TabIndex = 0;
            btnSend.Text = "Send >>";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lstHmiRecipes);
            groupBox2.Controls.Add(panel1);
            groupBox2.Dock = DockStyle.Right;
            groupBox2.Location = new Point(128, 0);
            groupBox2.Margin = new Padding(3, 2, 3, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 2, 3, 2);
            groupBox2.Size = new Size(368, 280);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "REMOTE RECIPES";
            // 
            // lstHmiRecipes
            // 
            lstHmiRecipes.Dock = DockStyle.Fill;
            lstHmiRecipes.FormattingEnabled = true;
            lstHmiRecipes.ItemHeight = 15;
            lstHmiRecipes.Location = new Point(3, 48);
            lstHmiRecipes.Margin = new Padding(3, 2, 3, 2);
            lstHmiRecipes.Name = "lstHmiRecipes";
            lstHmiRecipes.SelectionMode = SelectionMode.MultiExtended;
            lstHmiRecipes.Size = new Size(362, 230);
            lstHmiRecipes.TabIndex = 0;
            lstHmiRecipes.SelectedIndexChanged += lstHmiRecipes_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnRefreshHmi);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(3, 18);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(362, 30);
            panel1.TabIndex = 1;
            // 
            // btnRefreshHmi
            // 
            btnRefreshHmi.Location = new Point(3, 4);
            btnRefreshHmi.Margin = new Padding(3, 2, 3, 2);
            btnRefreshHmi.Name = "btnRefreshHmi";
            btnRefreshHmi.Size = new Size(105, 22);
            btnRefreshHmi.TabIndex = 0;
            btnRefreshHmi.Text = "Refresh List";
            btnRefreshHmi.UseVisualStyleBackColor = true;
            btnRefreshHmi.Click += btnRefreshHmi_Click;
            // 
            // dgvTransfers
            // 
            dgvTransfers.AllowUserToAddRows = false;
            dgvTransfers.AllowUserToDeleteRows = false;
            dgvTransfers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTransfers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTransfers.Dock = DockStyle.Fill;
            dgvTransfers.Location = new Point(3, 2);
            dgvTransfers.Margin = new Padding(3, 2, 3, 2);
            dgvTransfers.Name = "dgvTransfers";
            dgvTransfers.ReadOnly = true;
            dgvTransfers.RowHeadersWidth = 51;
            dgvTransfers.RowTemplate.Height = 29;
            dgvTransfers.Size = new Size(873, 258);
            dgvTransfers.TabIndex = 2;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageTransfers);
            tabControlMain.Controls.Add(tabPagePreview);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(9, 288);
            tabControlMain.Margin = new Padding(3, 2, 3, 2);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(887, 290);
            tabControlMain.TabIndex = 3;
            // 
            // tabPageTransfers
            // 
            tabPageTransfers.Controls.Add(dgvTransfers);
            tabPageTransfers.Location = new Point(4, 24);
            tabPageTransfers.Margin = new Padding(3, 2, 3, 2);
            tabPageTransfers.Name = "tabPageTransfers";
            tabPageTransfers.Padding = new Padding(3, 2, 3, 2);
            tabPageTransfers.Size = new Size(879, 262);
            tabPageTransfers.TabIndex = 0;
            tabPageTransfers.Text = "Transfer List";
            tabPageTransfers.UseVisualStyleBackColor = true;
            // 
            // tabPagePreview
            // 
            tabPagePreview.Controls.Add(pnlPreviewArea);
            tabPagePreview.Location = new Point(4, 24);
            tabPagePreview.Margin = new Padding(3, 2, 3, 2);
            tabPagePreview.Name = "tabPagePreview";
            tabPagePreview.Padding = new Padding(3, 2, 3, 2);
            tabPagePreview.Size = new Size(879, 262);
            tabPagePreview.TabIndex = 1;
            tabPagePreview.Text = "Recipe Preview";
            tabPagePreview.UseVisualStyleBackColor = true;
            // 
            // pnlPreviewArea
            // 
            pnlPreviewArea.Controls.Add(lblPreviewStatus);
            pnlPreviewArea.Dock = DockStyle.Fill;
            pnlPreviewArea.Location = new Point(3, 2);
            pnlPreviewArea.Margin = new Padding(3, 2, 3, 2);
            pnlPreviewArea.Name = "pnlPreviewArea";
            pnlPreviewArea.Size = new Size(873, 258);
            pnlPreviewArea.TabIndex = 0;
            // 
            // lblPreviewStatus
            // 
            lblPreviewStatus.Dock = DockStyle.Fill;
            lblPreviewStatus.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
            lblPreviewStatus.ForeColor = SystemColors.ControlDarkDark;
            lblPreviewStatus.Location = new Point(0, 0);
            lblPreviewStatus.Name = "lblPreviewStatus";
            lblPreviewStatus.Size = new Size(873, 258);
            lblPreviewStatus.TabIndex = 0;
            lblPreviewStatus.Text = "Ön izleme için HMI listesinden bir reçete seçin.";
            lblPreviewStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FtpSync_Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1162, 586);
            Controls.Add(tabControlMain);
            Controls.Add(splitContainer1);
            Controls.Add(pnlTop);
            Margin = new Padding(3, 2, 3, 2);
            Name = "FtpSync_Form";
            Padding = new Padding(9, 8, 9, 8);
            StartPosition = FormStartPosition.CenterParent;
            Text = "RECIPE SYNCHRONIZATION";
            Load += FtpSync_Form_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            pnlMiddle.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTransfers).EndInit();
            tabControlMain.ResumeLayout(false);
            tabPageTransfers.ResumeLayout(false);
            tabPagePreview.ResumeLayout(false);
            pnlPreviewArea.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.CheckedListBox clbMachines;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstLocalRecipes;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Button btnReceive;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstHmiRecipes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefreshHmi;
        private System.Windows.Forms.DataGridView dgvTransfers;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageTransfers;
        private System.Windows.Forms.TabPage tabPagePreview;
        private System.Windows.Forms.Panel pnlPreviewArea;
        private System.Windows.Forms.Label lblPreviewStatus;
    }
}
