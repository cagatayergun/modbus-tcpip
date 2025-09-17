namespace TekstilScada.UI.Views
{
    partial class MakineDetay_Control
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
            pnlTop = new Panel();
            btnGeri = new Button();
            lblMakineAdi = new Label();
            pnlMainContent = new Panel();
            tableLayoutPanelMain = new TableLayoutPanel();
            pnlTopDashboard = new Panel();
            tableLayoutPanelTop = new TableLayoutPanel();
            pnlGaugesAndInfo = new Panel();
            pnlGauges = new Panel();
            waterTankGauge1 = new TekstilScada.UI.Controls.WaterTankGauge();
            panelTemp = new Panel();
            lblTempValue = new Label();
            lblTempTitle = new Label();
            progressTemp = new ProgressBar();
            gaugeRpm = new CircularProgressBar.CircularProgressBar();
            pnlInfo = new Panel();
            lblSiparisNo = new Label();
            label6 = new Label();
            lblBatchNo = new Label();
            label5 = new Label();
            lblMusteriNo = new Label();
            label4 = new Label();
            lblOperator = new Label();
            label3 = new Label();
            lblReceteAdi = new Label();
            label2 = new Label();
            label1 = new Label();
            pnlTimeline = new Panel();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            pnlAlarmsAndSteps = new Panel();
            dgvAdimlar = new DataGridView();
            label10 = new Label();
            lblCalisanAdim = new Label();
            label8 = new Label();
            lstAlarmlar = new ListBox();
            label7 = new Label();
            pnlTop.SuspendLayout();
            pnlMainContent.SuspendLayout();
            tableLayoutPanelMain.SuspendLayout();
            pnlTopDashboard.SuspendLayout();
            tableLayoutPanelTop.SuspendLayout();
            pnlGaugesAndInfo.SuspendLayout();
            pnlGauges.SuspendLayout();
            panelTemp.SuspendLayout();
            pnlInfo.SuspendLayout();
            pnlTimeline.SuspendLayout();
            pnlAlarmsAndSteps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAdimlar).BeginInit();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(btnGeri);
            pnlTop.Controls.Add(lblMakineAdi);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(3, 2, 3, 2);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1358, 38);
            pnlTop.TabIndex = 1;
            // 
            // btnGeri
            // 
            btnGeri.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGeri.Location = new Point(9, 8);
            btnGeri.Margin = new Padding(3, 2, 3, 2);
            btnGeri.Name = "btnGeri";
            btnGeri.Size = new Size(82, 22);
            btnGeri.TabIndex = 0;
            btnGeri.Text = "< GERİ";
            btnGeri.UseVisualStyleBackColor = true;
            // 
            // lblMakineAdi
            // 
            lblMakineAdi.Dock = DockStyle.Fill;
            lblMakineAdi.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblMakineAdi.ForeColor = Color.FromArgb(45, 52, 54);
            lblMakineAdi.Location = new Point(0, 0);
            lblMakineAdi.Name = "lblMakineAdi";
            lblMakineAdi.Size = new Size(1358, 38);
            lblMakineAdi.TabIndex = 1;
            lblMakineAdi.Text = "MAKİNE ADI";
            lblMakineAdi.TextAlign = ContentAlignment.MiddleCenter;
            lblMakineAdi.Click += lblMakineAdi_Click;
            // 
            // pnlMainContent
            // 
            pnlMainContent.Controls.Add(tableLayoutPanelMain);
            pnlMainContent.Controls.Add(pnlAlarmsAndSteps);
            pnlMainContent.Dock = DockStyle.Fill;
            pnlMainContent.Location = new Point(0, 38);
            pnlMainContent.Margin = new Padding(3, 2, 3, 2);
            pnlMainContent.Name = "pnlMainContent";
            pnlMainContent.Size = new Size(1358, 761);
            pnlMainContent.TabIndex = 2;
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Controls.Add(pnlTopDashboard, 0, 0);
            tableLayoutPanelMain.Controls.Add(pnlTimeline, 0, 1);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 2;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 239F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelMain.Size = new Size(969, 761);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // pnlTopDashboard
            // 
            pnlTopDashboard.Controls.Add(tableLayoutPanelTop);
            pnlTopDashboard.Dock = DockStyle.Fill;
            pnlTopDashboard.Location = new Point(3, 2);
            pnlTopDashboard.Margin = new Padding(3, 2, 3, 2);
            pnlTopDashboard.Name = "pnlTopDashboard";
            pnlTopDashboard.Size = new Size(963, 235);
            pnlTopDashboard.TabIndex = 0;
            // 
            // tableLayoutPanelTop
            // 
            tableLayoutPanelTop.ColumnCount = 1;
            tableLayoutPanelTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 93.04734F));
            tableLayoutPanelTop.Controls.Add(pnlGaugesAndInfo, 0, 0);
            tableLayoutPanelTop.Dock = DockStyle.Fill;
            tableLayoutPanelTop.Location = new Point(0, 0);
            tableLayoutPanelTop.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            tableLayoutPanelTop.RowCount = 1;
            tableLayoutPanelTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelTop.Size = new Size(963, 235);
            tableLayoutPanelTop.TabIndex = 0;
            // 
            // pnlGaugesAndInfo
            // 
            pnlGaugesAndInfo.BackColor = Color.WhiteSmoke;
            pnlGaugesAndInfo.Controls.Add(pnlGauges);
            pnlGaugesAndInfo.Controls.Add(pnlInfo);
            pnlGaugesAndInfo.Dock = DockStyle.Fill;
            pnlGaugesAndInfo.Location = new Point(3, 2);
            pnlGaugesAndInfo.Margin = new Padding(3, 2, 3, 2);
            pnlGaugesAndInfo.Name = "pnlGaugesAndInfo";
            pnlGaugesAndInfo.Size = new Size(957, 231);
            pnlGaugesAndInfo.TabIndex = 2;
            // 
            // pnlGauges
            // 
            pnlGauges.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            pnlGauges.Controls.Add(waterTankGauge1);
            pnlGauges.Controls.Add(panelTemp);
            pnlGauges.Controls.Add(gaugeRpm);
            pnlGauges.Location = new Point(401, -1);
            pnlGauges.Margin = new Padding(3, 2, 3, 2);
            pnlGauges.Name = "pnlGauges";
            pnlGauges.Size = new Size(556, 231);
            pnlGauges.TabIndex = 1;
            // 
            // waterTankGauge1
            // 
            waterTankGauge1.Location = new Point(362, 18);
            waterTankGauge1.Margin = new Padding(3, 2, 3, 2);
            waterTankGauge1.Maximum = 5000;
            waterTankGauge1.Name = "waterTankGauge1";
            waterTankGauge1.Size = new Size(170, 197);
            waterTankGauge1.TabIndex = 4;
            waterTankGauge1.Value = 0;
            // 
            // panelTemp
            // 
            panelTemp.Controls.Add(lblTempValue);
            panelTemp.Controls.Add(lblTempTitle);
            panelTemp.Controls.Add(progressTemp);
            panelTemp.Location = new Point(220, 28);
            panelTemp.Margin = new Padding(3, 2, 3, 2);
            panelTemp.Name = "panelTemp";
            panelTemp.Size = new Size(105, 176);
            panelTemp.TabIndex = 3;
            // 
            // lblTempValue
            // 
            lblTempValue.Dock = DockStyle.Bottom;
            lblTempValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTempValue.Location = new Point(0, 132);
            lblTempValue.Name = "lblTempValue";
            lblTempValue.Size = new Size(105, 22);
            lblTempValue.TabIndex = 2;
            lblTempValue.Text = "0 °C";
            lblTempValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTempTitle
            // 
            lblTempTitle.Dock = DockStyle.Bottom;
            lblTempTitle.Font = new Font("Segoe UI", 10F);
            lblTempTitle.ForeColor = Color.FromArgb(100, 100, 100);
            lblTempTitle.Location = new Point(0, 154);
            lblTempTitle.Name = "lblTempTitle";
            lblTempTitle.Size = new Size(105, 22);
            lblTempTitle.TabIndex = 1;
            lblTempTitle.Text = "Sıcaklık";
            lblTempTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressTemp
            // 
            progressTemp.Location = new Point(39, 17);
            progressTemp.Margin = new Padding(3, 2, 3, 2);
            progressTemp.Maximum = 1500;
            progressTemp.Name = "progressTemp";
            progressTemp.Size = new Size(26, 113);
            progressTemp.Step = 1;
            progressTemp.Style = ProgressBarStyle.Continuous;
            progressTemp.TabIndex = 0;
            // 
            // gaugeRpm
            // 
            gaugeRpm.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            gaugeRpm.AnimationSpeed = 500;
            gaugeRpm.BackColor = Color.Transparent;
            gaugeRpm.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            gaugeRpm.ForeColor = Color.FromArgb(64, 64, 64);
            gaugeRpm.InnerColor = Color.White;
            gaugeRpm.InnerMargin = 2;
            gaugeRpm.InnerWidth = -1;
            gaugeRpm.Location = new Point(24, 42);
            gaugeRpm.Margin = new Padding(3, 2, 3, 2);
            gaugeRpm.MarqueeAnimationSpeed = 2000;
            gaugeRpm.Maximum = 500;
            gaugeRpm.Name = "gaugeRpm";
            gaugeRpm.OuterColor = Color.FromArgb(224, 224, 224);
            gaugeRpm.OuterMargin = -25;
            gaugeRpm.OuterWidth = 26;
            gaugeRpm.ProgressColor = Color.FromArgb(46, 204, 113);
            gaugeRpm.ProgressWidth = 18;
            gaugeRpm.SecondaryFont = new Font("Segoe UI", 10F);
            gaugeRpm.Size = new Size(159, 148);
            gaugeRpm.StartAngle = 135;
            gaugeRpm.SubscriptColor = Color.FromArgb(100, 100, 100);
            gaugeRpm.SubscriptMargin = new Padding(-6, 0, 0, 0);
            gaugeRpm.SubscriptText = "RPM";
            gaugeRpm.SuperscriptColor = Color.FromArgb(166, 166, 166);
            gaugeRpm.SuperscriptMargin = new Padding(0, -35, 50, 0);
            gaugeRpm.SuperscriptText = "";
            gaugeRpm.TabIndex = 0;
            gaugeRpm.Text = "0";
            gaugeRpm.TextMargin = new Padding(7, 20, 0, 0);
            gaugeRpm.Value = 68;
            // 
            // pnlInfo
            // 
            pnlInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            pnlInfo.Controls.Add(lblSiparisNo);
            pnlInfo.Controls.Add(label6);
            pnlInfo.Controls.Add(lblBatchNo);
            pnlInfo.Controls.Add(label5);
            pnlInfo.Controls.Add(lblMusteriNo);
            pnlInfo.Controls.Add(label4);
            pnlInfo.Controls.Add(lblOperator);
            pnlInfo.Controls.Add(label3);
            pnlInfo.Controls.Add(lblReceteAdi);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Location = new Point(0, 0);
            pnlInfo.Margin = new Padding(3, 2, 3, 2);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(360, 231);
            pnlInfo.TabIndex = 0;
            // 
            // lblSiparisNo
            // 
            lblSiparisNo.BackColor = Color.White;
            lblSiparisNo.BorderStyle = BorderStyle.FixedSingle;
            lblSiparisNo.Font = new Font("Segoe UI", 9F);
            lblSiparisNo.Location = new Point(128, 134);
            lblSiparisNo.Name = "lblSiparisNo";
            lblSiparisNo.Size = new Size(175, 19);
            lblSiparisNo.TabIndex = 21;
            lblSiparisNo.Text = "---";
            lblSiparisNo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.Location = new Point(10, 135);
            label6.Name = "label6";
            label6.Size = new Size(62, 15);
            label6.TabIndex = 20;
            label6.Text = "Order No:";
            // 
            // lblBatchNo
            // 
            lblBatchNo.BackColor = Color.White;
            lblBatchNo.BorderStyle = BorderStyle.FixedSingle;
            lblBatchNo.Font = new Font("Segoe UI", 9F);
            lblBatchNo.Location = new Point(128, 110);
            lblBatchNo.Name = "lblBatchNo";
            lblBatchNo.Size = new Size(175, 19);
            lblBatchNo.TabIndex = 19;
            lblBatchNo.Text = "---";
            lblBatchNo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(10, 112);
            label5.Name = "label5";
            label5.Size = new Size(61, 15);
            label5.TabIndex = 18;
            label5.Text = "Batch No:";
            // 
            // lblMusteriNo
            // 
            lblMusteriNo.BackColor = Color.White;
            lblMusteriNo.BorderStyle = BorderStyle.FixedSingle;
            lblMusteriNo.Font = new Font("Segoe UI", 9F);
            lblMusteriNo.Location = new Point(128, 87);
            lblMusteriNo.Name = "lblMusteriNo";
            lblMusteriNo.Size = new Size(175, 19);
            lblMusteriNo.TabIndex = 17;
            lblMusteriNo.Text = "---";
            lblMusteriNo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(10, 88);
            label4.Name = "label4";
            label4.Size = new Size(113, 15);
            label4.TabIndex = 16;
            label4.Text = "Customer Number:";
            // 
            // lblOperator
            // 
            lblOperator.BackColor = Color.White;
            lblOperator.BorderStyle = BorderStyle.FixedSingle;
            lblOperator.Font = new Font("Segoe UI", 9F);
            lblOperator.Location = new Point(128, 64);
            lblOperator.Name = "lblOperator";
            lblOperator.Size = new Size(175, 19);
            lblOperator.TabIndex = 15;
            lblOperator.Text = "---";
            lblOperator.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(10, 65);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 14;
            label3.Text = "Operator:";
            // 
            // lblReceteAdi
            // 
            lblReceteAdi.BackColor = Color.White;
            lblReceteAdi.BorderStyle = BorderStyle.FixedSingle;
            lblReceteAdi.Font = new Font("Segoe UI", 9F);
            lblReceteAdi.Location = new Point(128, 40);
            lblReceteAdi.Name = "lblReceteAdi";
            lblReceteAdi.Size = new Size(175, 19);
            lblReceteAdi.TabIndex = 13;
            lblReceteAdi.Text = "---";
            lblReceteAdi.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(10, 42);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 12;
            label2.Text = "Recipe Name:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.Location = new Point(7, 5);
            label1.Name = "label1";
            label1.Size = new Size(172, 21);
            label1.TabIndex = 0;
            label1.Text = "Machine Information";
            // 
            // pnlTimeline
            // 
            pnlTimeline.Controls.Add(formsPlot1);
            pnlTimeline.Dock = DockStyle.Fill;
            pnlTimeline.Location = new Point(3, 241);
            pnlTimeline.Margin = new Padding(3, 2, 3, 2);
            pnlTimeline.Name = "pnlTimeline";
            pnlTimeline.Size = new Size(963, 518);
            pnlTimeline.TabIndex = 1;
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Dock = DockStyle.Fill;
            formsPlot1.Location = new Point(0, 0);
            formsPlot1.Margin = new Padding(3, 2, 3, 2);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(963, 518);
            formsPlot1.TabIndex = 0;
            // 
            // pnlAlarmsAndSteps
            // 
            pnlAlarmsAndSteps.BackColor = Color.WhiteSmoke;
            pnlAlarmsAndSteps.Controls.Add(dgvAdimlar);
            pnlAlarmsAndSteps.Controls.Add(label10);
            pnlAlarmsAndSteps.Controls.Add(lblCalisanAdim);
            pnlAlarmsAndSteps.Controls.Add(label8);
            pnlAlarmsAndSteps.Controls.Add(lstAlarmlar);
            pnlAlarmsAndSteps.Controls.Add(label7);
            pnlAlarmsAndSteps.Dock = DockStyle.Right;
            pnlAlarmsAndSteps.Location = new Point(969, 0);
            pnlAlarmsAndSteps.Margin = new Padding(3, 2, 3, 2);
            pnlAlarmsAndSteps.Name = "pnlAlarmsAndSteps";
            pnlAlarmsAndSteps.Size = new Size(389, 761);
            pnlAlarmsAndSteps.TabIndex = 5;
            pnlAlarmsAndSteps.TabStop = true;
            pnlAlarmsAndSteps.Paint += pnlAlarmsAndSteps_Paint;
            // 
            // dgvAdimlar
            // 
            dgvAdimlar.AllowUserToAddRows = false;
            dgvAdimlar.AllowUserToDeleteRows = false;
            dgvAdimlar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dgvAdimlar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAdimlar.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAdimlar.Location = new Point(49, 89);
            dgvAdimlar.Margin = new Padding(3, 2, 3, 2);
            dgvAdimlar.Name = "dgvAdimlar";
            dgvAdimlar.ReadOnly = true;
            dgvAdimlar.RowHeadersVisible = false;
            dgvAdimlar.RowHeadersWidth = 51;
            dgvAdimlar.RowTemplate.Height = 24;
            dgvAdimlar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAdimlar.Size = new Size(291, 305);
            dgvAdimlar.TabIndex = 5;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label10.Location = new Point(136, 69);
            label10.Name = "label10";
            label10.Size = new Size(94, 19);
            label10.TabIndex = 4;
            label10.Text = "Recipe Steps";
            // 
            // lblCalisanAdim
            // 
            lblCalisanAdim.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblCalisanAdim.BackColor = Color.White;
            lblCalisanAdim.BorderStyle = BorderStyle.FixedSingle;
            lblCalisanAdim.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCalisanAdim.ForeColor = Color.ForestGreen;
            lblCalisanAdim.Location = new Point(49, 33);
            lblCalisanAdim.Name = "lblCalisanAdim";
            lblCalisanAdim.Size = new Size(291, 24);
            lblCalisanAdim.TabIndex = 3;
            lblCalisanAdim.Text = "---";
            lblCalisanAdim.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label8.Location = new Point(146, 13);
            label8.Name = "label8";
            label8.Size = new Size(97, 19);
            label8.TabIndex = 2;
            label8.Text = "Running Step";
            // 
            // lstAlarmlar
            // 
            lstAlarmlar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lstAlarmlar.BackColor = Color.FromArgb(45, 52, 54);
            lstAlarmlar.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lstAlarmlar.ForeColor = Color.White;
            lstAlarmlar.FormattingEnabled = true;
            lstAlarmlar.ItemHeight = 15;
            lstAlarmlar.Location = new Point(49, 427);
            lstAlarmlar.Margin = new Padding(3, 2, 3, 2);
            lstAlarmlar.Name = "lstAlarmlar";
            lstAlarmlar.Size = new Size(291, 304);
            lstAlarmlar.TabIndex = 1;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label7.Location = new Point(133, 407);
            label7.Name = "label7";
            label7.Size = new Size(133, 19);
            label7.TabIndex = 0;
            label7.Text = "Intra-Party Alarms";
            // 
            // MakineDetay_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(pnlMainContent);
            Controls.Add(pnlTop);
            Margin = new Padding(3, 2, 3, 2);
            Name = "MakineDetay_Control";
            Size = new Size(1358, 799);
            pnlTop.ResumeLayout(false);
            pnlMainContent.ResumeLayout(false);
            tableLayoutPanelMain.ResumeLayout(false);
            pnlTopDashboard.ResumeLayout(false);
            tableLayoutPanelTop.ResumeLayout(false);
            pnlGaugesAndInfo.ResumeLayout(false);
            pnlGauges.ResumeLayout(false);
            panelTemp.ResumeLayout(false);
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            pnlTimeline.ResumeLayout(false);
            pnlAlarmsAndSteps.ResumeLayout(false);
            pnlAlarmsAndSteps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAdimlar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnGeri;
        private System.Windows.Forms.Label lblMakineAdi;
        private System.Windows.Forms.Panel pnlMainContent;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel pnlTopDashboard;
        private System.Windows.Forms.Panel pnlTimeline;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.Panel pnlGaugesAndInfo;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlGauges;
        private System.Windows.Forms.Label lblSiparisNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBatchNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMusteriNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblReceteAdi;
        private System.Windows.Forms.Label label2;
        private CircularProgressBar.CircularProgressBar gaugeRpm;
        private System.Windows.Forms.Panel panelTemp;
        private System.Windows.Forms.ProgressBar progressTemp;
        private System.Windows.Forms.Label lblTempValue;
        private System.Windows.Forms.Label lblTempTitle;
        private UI.Controls.WaterTankGauge waterTankGauge1;
        private Panel pnlAlarmsAndSteps;
        private DataGridView dgvAdimlar;
        private Label label10;
        private Label lblCalisanAdim;
        private Label label8;
        private ListBox lstAlarmlar;
        private Label label7;
    }
}


