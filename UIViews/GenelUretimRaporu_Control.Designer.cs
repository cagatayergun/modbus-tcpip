namespace TekstilScada.UI.Views
{
    partial class GenelUretimRaporu_Control
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
            groupBox1 = new GroupBox();
            radioBuhar = new RadioButton();
            radioSu = new RadioButton();
            radioElektrik = new RadioButton();
            btnRaporOlustur = new Button();
            dtpEndTime = new DateTimePicker();
            dtpStartTime = new DateTimePicker();
            pnlMain = new Panel();
            dgvReport = new DataGridView();
            pnlSelection = new Panel();
            flpMachineGroups = new FlowLayoutPanel();
            panel1 = new Panel();
            btnRemoveAll = new Button();
            btnAddAll = new Button();
            btnRemove = new Button();
            btnAdd = new Button();
            listBoxSeciliMakineler = new ListBox();
            pnlFilters.SuspendLayout();
            groupBox1.SuspendLayout();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            pnlSelection.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlFilters
            // 
            pnlFilters.Controls.Add(groupBox1);
            pnlFilters.Controls.Add(btnRaporOlustur);
            pnlFilters.Controls.Add(dtpEndTime);
            pnlFilters.Controls.Add(dtpStartTime);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Margin = new Padding(3, 2, 3, 2);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(1050, 60);
            pnlFilters.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioBuhar);
            groupBox1.Controls.Add(radioSu);
            groupBox1.Controls.Add(radioElektrik);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(194, 4);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(283, 49);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Consumption Type";
            // 
            // radioBuhar
            // 
            radioBuhar.AutoSize = true;
            radioBuhar.Location = new Point(205, 19);
            radioBuhar.Margin = new Padding(3, 2, 3, 2);
            radioBuhar.Name = "radioBuhar";
            radioBuhar.Size = new Size(61, 19);
            radioBuhar.TabIndex = 2;
            radioBuhar.Text = "Steam";
            radioBuhar.UseVisualStyleBackColor = true;
            radioBuhar.CheckedChanged += radioConsumption_CheckedChanged;
            // 
            // radioSu
            // 
            radioSu.AutoSize = true;
            radioSu.Location = new Point(127, 19);
            radioSu.Margin = new Padding(3, 2, 3, 2);
            radioSu.Name = "radioSu";
            radioSu.Size = new Size(67, 19);
            radioSu.TabIndex = 1;
            radioSu.Text = "Wather";
            radioSu.UseVisualStyleBackColor = true;
            radioSu.CheckedChanged += radioConsumption_CheckedChanged;
            // 
            // radioElektrik
            // 
            radioElektrik.AutoSize = true;
            radioElektrik.Checked = true;
            radioElektrik.Location = new Point(20, 19);
            radioElektrik.Margin = new Padding(3, 2, 3, 2);
            radioElektrik.Name = "radioElektrik";
            radioElektrik.Size = new Size(66, 19);
            radioElektrik.TabIndex = 0;
            radioElektrik.TabStop = true;
            radioElektrik.Text = "Electric";
            radioElektrik.UseVisualStyleBackColor = true;
            radioElektrik.CheckedChanged += radioConsumption_CheckedChanged;
            // 
            // btnRaporOlustur
            // 
            btnRaporOlustur.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnRaporOlustur.Location = new Point(525, 15);
            btnRaporOlustur.Margin = new Padding(3, 2, 3, 2);
            btnRaporOlustur.Name = "btnRaporOlustur";
            btnRaporOlustur.Size = new Size(131, 30);
            btnRaporOlustur.TabIndex = 2;
            btnRaporOlustur.Text = "Report";
            btnRaporOlustur.UseVisualStyleBackColor = true;
            btnRaporOlustur.Click += btnRaporOlustur_Click;
            // 
            // dtpEndTime
            // 
            dtpEndTime.Format = DateTimePickerFormat.Short;
            dtpEndTime.Location = new Point(13, 34);
            dtpEndTime.Margin = new Padding(3, 2, 3, 2);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.Size = new Size(120, 23);
            dtpEndTime.TabIndex = 1;
            // 
            // dtpStartTime
            // 
            dtpStartTime.Format = DateTimePickerFormat.Short;
            dtpStartTime.Location = new Point(13, 9);
            dtpStartTime.Margin = new Padding(3, 2, 3, 2);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.Size = new Size(120, 23);
            dtpStartTime.TabIndex = 0;
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(dgvReport);
            pnlMain.Controls.Add(pnlSelection);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 60);
            pnlMain.Margin = new Padding(3, 2, 3, 2);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1050, 465);
            pnlMain.TabIndex = 1;
            // 
            // dgvReport
            // 
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AllowUserToDeleteRows = false;
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReport.Dock = DockStyle.Fill;
            dgvReport.Location = new Point(0, 188);
            dgvReport.Margin = new Padding(3, 2, 3, 2);
            dgvReport.Name = "dgvReport";
            dgvReport.ReadOnly = true;
            dgvReport.RowHeadersWidth = 51;
            dgvReport.RowTemplate.Height = 29;
            dgvReport.Size = new Size(1050, 277);
            dgvReport.TabIndex = 1;
            // 
            // pnlSelection
            // 
            pnlSelection.Controls.Add(flpMachineGroups);
            pnlSelection.Controls.Add(panel1);
            pnlSelection.Controls.Add(listBoxSeciliMakineler);
            pnlSelection.Dock = DockStyle.Top;
            pnlSelection.Location = new Point(0, 0);
            pnlSelection.Margin = new Padding(3, 2, 3, 2);
            pnlSelection.Name = "pnlSelection";
            pnlSelection.Size = new Size(1050, 188);
            pnlSelection.TabIndex = 2;
            // 
            // flpMachineGroups
            // 
            flpMachineGroups.AutoScroll = true;
            flpMachineGroups.Dock = DockStyle.Fill;
            flpMachineGroups.Location = new Point(0, 0);
            flpMachineGroups.Margin = new Padding(3, 2, 3, 2);
            flpMachineGroups.Name = "flpMachineGroups";
            flpMachineGroups.Padding = new Padding(4);
            flpMachineGroups.Size = new Size(726, 188);
            flpMachineGroups.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnRemoveAll);
            panel1.Controls.Add(btnAddAll);
            panel1.Controls.Add(btnRemove);
            panel1.Controls.Add(btnAdd);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(726, 0);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(61, 188);
            panel1.TabIndex = 1;
            // 
            // btnRemoveAll
            // 
            btnRemoveAll.Location = new Point(9, 135);
            btnRemoveAll.Margin = new Padding(3, 2, 3, 2);
            btnRemoveAll.Name = "btnRemoveAll";
            btnRemoveAll.Size = new Size(44, 22);
            btnRemoveAll.TabIndex = 3;
            btnRemoveAll.Text = "<<";
            btnRemoveAll.UseVisualStyleBackColor = true;
            btnRemoveAll.Click += btnRemoveAll_Click;
            // 
            // btnAddAll
            // 
            btnAddAll.Location = new Point(9, 105);
            btnAddAll.Margin = new Padding(3, 2, 3, 2);
            btnAddAll.Name = "btnAddAll";
            btnAddAll.Size = new Size(44, 22);
            btnAddAll.TabIndex = 2;
            btnAddAll.Text = ">>";
            btnAddAll.UseVisualStyleBackColor = true;
            btnAddAll.Click += btnAddAll_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(9, 60);
            btnRemove.Margin = new Padding(3, 2, 3, 2);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(44, 22);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "<";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(9, 30);
            btnAdd.Margin = new Padding(3, 2, 3, 2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(44, 22);
            btnAdd.TabIndex = 0;
            btnAdd.Text = ">";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // listBoxSeciliMakineler
            // 
            listBoxSeciliMakineler.Dock = DockStyle.Right;
            listBoxSeciliMakineler.FormattingEnabled = true;
            listBoxSeciliMakineler.ItemHeight = 15;
            listBoxSeciliMakineler.Location = new Point(787, 0);
            listBoxSeciliMakineler.Margin = new Padding(3, 2, 3, 2);
            listBoxSeciliMakineler.Name = "listBoxSeciliMakineler";
            listBoxSeciliMakineler.SelectionMode = SelectionMode.MultiExtended;
            listBoxSeciliMakineler.Size = new Size(263, 188);
            listBoxSeciliMakineler.TabIndex = 0;
            // 
            // GenelUretimRaporu_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
            Controls.Add(pnlFilters);
            Margin = new Padding(3, 2, 3, 2);
            Name = "GenelUretimRaporu_Control";
            Size = new Size(1050, 525);
            Load += GenelUretimRaporu_Control_Load;
            pnlFilters.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
            pnlSelection.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Button btnRaporOlustur;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioBuhar;
        private System.Windows.Forms.RadioButton radioSu;
        private System.Windows.Forms.RadioButton radioElektrik;
        private System.Windows.Forms.DataGridView dgvReport;
        private System.Windows.Forms.Panel pnlSelection;
        private System.Windows.Forms.FlowLayoutPanel flpMachineGroups;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox listBoxSeciliMakineler;
    }
}