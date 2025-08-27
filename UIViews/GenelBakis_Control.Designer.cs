namespace TekstilScada.UI.Views
{
    partial class GenelBakis_Control
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }
        #region Component Designer generated code
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.flpTopKpis = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tlpMainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.flpMachineGroups = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.tlpSidebarLayout = new System.Windows.Forms.TableLayoutPanel();
            this.gbTopAlarms = new System.Windows.Forms.GroupBox();
            this.formsPlotTopAlarms = new ScottPlot.WinForms.FormsPlot();
            this.gbHourlyConsumptionSteam = new System.Windows.Forms.GroupBox();
            this.formsPlotHourlySteam = new ScottPlot.WinForms.FormsPlot();
            this.gbHourlyConsumptionWater = new System.Windows.Forms.GroupBox();
            this.formsPlotHourlyWater = new ScottPlot.WinForms.FormsPlot();
            this.gbHourlyConsumption = new System.Windows.Forms.GroupBox();
            this.formsPlotHourly = new ScottPlot.WinForms.FormsPlot();
            // YENİ KOD BAŞLANGICI
            this.gbHourlyOee = new System.Windows.Forms.GroupBox();
            this.formsPlotHourlyOee = new ScottPlot.WinForms.FormsPlot();
            // YENİ KOD BİTİŞİ
            this.pnlHeader.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.tlpMainLayout.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.tlpSidebarLayout.SuspendLayout();
            this.gbTopAlarms.SuspendLayout();
            this.gbHourlyConsumptionSteam.SuspendLayout();
            this.gbHourlyConsumptionWater.SuspendLayout();
            this.gbHourlyConsumption.SuspendLayout();
            // YENİ KOD BAŞLANGICI
            this.gbHourlyOee.SuspendLayout();
            // YENİ KOD BİTİŞİ
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.flpTopKpis);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(9, 8);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(4);
            this.pnlHeader.Size = new System.Drawing.Size(1032, 75);
            this.pnlHeader.TabIndex = 0;
            // 
            // flpTopKpis
            // 
            this.flpTopKpis.BackColor = System.Drawing.Color.White;
            this.flpTopKpis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTopKpis.Location = new System.Drawing.Point(4, 4);
            this.flpTopKpis.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpTopKpis.Name = "flpTopKpis";
            this.flpTopKpis.Size = new System.Drawing.Size(1024, 67);
            this.flpTopKpis.TabIndex = 0;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tlpMainLayout);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(9, 83);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1032, 509);
            this.pnlMain.TabIndex = 2;
            // 
            // tlpMainLayout
            // 
            this.tlpMainLayout.ColumnCount = 2;
            this.tlpMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 306F));
            this.tlpMainLayout.Controls.Add(this.flpMachineGroups, 0, 0);
            this.tlpMainLayout.Controls.Add(this.pnlSidebar, 1, 0);
            this.tlpMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainLayout.Location = new System.Drawing.Point(0, 0);
            this.tlpMainLayout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMainLayout.Name = "tlpMainLayout";
            this.tlpMainLayout.RowCount = 1;
            this.tlpMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainLayout.Size = new System.Drawing.Size(1032, 509);
            this.tlpMainLayout.TabIndex = 0;
            // 
            // flpMachineGroups
            // 
            this.flpMachineGroups.AutoScroll = true;
            this.flpMachineGroups.BackColor = System.Drawing.SystemColors.Control;
            this.flpMachineGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMachineGroups.Location = new System.Drawing.Point(3, 2);
            this.flpMachineGroups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpMachineGroups.Name = "flpMachineGroups";
            this.flpMachineGroups.Padding = new System.Windows.Forms.Padding(4);
            this.flpMachineGroups.Size = new System.Drawing.Size(720, 505);
            this.flpMachineGroups.TabIndex = 1;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.Controls.Add(this.tlpSidebarLayout);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSidebar.Location = new System.Drawing.Point(729, 2);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(300, 505);
            this.pnlSidebar.TabIndex = 2;
            // 
            // tlpSidebarLayout
            // 
            this.tlpSidebarLayout.ColumnCount = 1;
            // GÜNCELLENDİ: Satır sayısı 4'ten 5'e çıkarıldı
            this.tlpSidebarLayout.RowCount = 5;
            this.tlpSidebarLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // GÜNCELLENDİ: Tüm satırların boyutu %20 olarak ayarlandı
            this.tlpSidebarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpSidebarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpSidebarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpSidebarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpSidebarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F)); // YENİ SATIR
            this.tlpSidebarLayout.Controls.Add(this.gbTopAlarms, 0, 3);
            this.tlpSidebarLayout.Controls.Add(this.gbHourlyConsumptionSteam, 0, 2);
            this.tlpSidebarLayout.Controls.Add(this.gbHourlyConsumptionWater, 0, 1);
            this.tlpSidebarLayout.Controls.Add(this.gbHourlyConsumption, 0, 0);
            this.tlpSidebarLayout.Controls.Add(this.gbHourlyOee, 0, 4); // YENİ: Grup kutusu 5. satıra eklendi
            this.tlpSidebarLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSidebarLayout.Location = new System.Drawing.Point(0, 0);
            this.tlpSidebarLayout.Name = "tlpSidebarLayout";
            this.tlpSidebarLayout.Size = new System.Drawing.Size(300, 505);
            this.tlpSidebarLayout.TabIndex = 0;
            // 
            // gbTopAlarms
            // 
            this.gbTopAlarms.Controls.Add(this.formsPlotTopAlarms);
            this.gbTopAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTopAlarms.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbTopAlarms.Location = new System.Drawing.Point(3, 303);
            this.gbTopAlarms.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbTopAlarms.Name = "gbTopAlarms";
            this.gbTopAlarms.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbTopAlarms.Size = new System.Drawing.Size(294, 98);
            this.gbTopAlarms.TabIndex = 3;
            this.gbTopAlarms.TabStop = false;
            this.gbTopAlarms.Text = "Son 24 Saatin Popüler Alarmları";
            // 
            // formsPlotTopAlarms
            // 
            this.formsPlotTopAlarms.DisplayScale = 1F;
            this.formsPlotTopAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlotTopAlarms.Location = new System.Drawing.Point(3, 20);
            this.formsPlotTopAlarms.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.formsPlotTopAlarms.Name = "formsPlotTopAlarms";
            this.formsPlotTopAlarms.Size = new System.Drawing.Size(288, 76);
            this.formsPlotTopAlarms.TabIndex = 0;
            // 
            // gbHourlyConsumptionSteam
            // 
            this.gbHourlyConsumptionSteam.Controls.Add(this.formsPlotHourlySteam);
            this.gbHourlyConsumptionSteam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbHourlyConsumptionSteam.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbHourlyConsumptionSteam.Location = new System.Drawing.Point(3, 203);
            this.gbHourlyConsumptionSteam.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbHourlyConsumptionSteam.Name = "gbHourlyConsumptionSteam";
            this.gbHourlyConsumptionSteam.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbHourlyConsumptionSteam.Size = new System.Drawing.Size(294, 98);
            this.gbHourlyConsumptionSteam.TabIndex = 2;
            this.gbHourlyConsumptionSteam.TabStop = false;
            this.gbHourlyConsumptionSteam.Text = "Saatlik Buhar Tüketimi";
            // 
            // formsPlotHourlySteam
            // 
            this.formsPlotHourlySteam.DisplayScale = 1F;
            this.formsPlotHourlySteam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlotHourlySteam.Location = new Point(3, 20);
            this.formsPlotHourlySteam.Margin = new Padding(3, 2, 3, 2);
            this.formsPlotHourlySteam.Name = "formsPlotHourlySteam";
            this.formsPlotHourlySteam.Size = new Size(288, 76);
            this.formsPlotHourlySteam.TabIndex = 0;
            // 
            // gbHourlyConsumptionWater
            // 
            this.gbHourlyConsumptionWater.Controls.Add(this.formsPlotHourlyWater);
            this.gbHourlyConsumptionWater.Dock = DockStyle.Fill;
            this.gbHourlyConsumptionWater.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.gbHourlyConsumptionWater.Location = new Point(3, 103);
            this.gbHourlyConsumptionWater.Margin = new Padding(3, 2, 3, 2);
            this.gbHourlyConsumptionWater.Name = "gbHourlyConsumptionWater";
            this.gbHourlyConsumptionWater.Padding = new Padding(3, 2, 3, 2);
            this.gbHourlyConsumptionWater.Size = new Size(294, 98);
            this.gbHourlyConsumptionWater.TabIndex = 1;
            this.gbHourlyConsumptionWater.TabStop = false;
            this.gbHourlyConsumptionWater.Text = "Saatlik Su Tüketimi";
            // 
            // formsPlotHourlyWater
            // 
            this.formsPlotHourlyWater.DisplayScale = 1F;
            this.formsPlotHourlyWater.Dock = DockStyle.Fill;
            this.formsPlotHourlyWater.Location = new Point(3, 20);
            this.formsPlotHourlyWater.Margin = new Padding(3, 2, 3, 2);
            this.formsPlotHourlyWater.Name = "formsPlotHourlyWater";
            this.formsPlotHourlyWater.Size = new Size(288, 76);
            this.formsPlotHourlyWater.TabIndex = 0;
            // 
            // gbHourlyConsumption
            // 
            this.gbHourlyConsumption.Controls.Add(this.formsPlotHourly);
            this.gbHourlyConsumption.Dock = DockStyle.Fill;
            this.gbHourlyConsumption.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.gbHourlyConsumption.Location = new Point(3, 3);
            this.gbHourlyConsumption.Margin = new Padding(3, 2, 3, 2);
            this.gbHourlyConsumption.Name = "gbHourlyConsumption";
            this.gbHourlyConsumption.Padding = new Padding(3, 2, 3, 2);
            this.gbHourlyConsumption.Size = new Size(294, 98);
            this.gbHourlyConsumption.TabIndex = 0;
            this.gbHourlyConsumption.TabStop = false;
            this.gbHourlyConsumption.Text = "Saatlik Elektrik Tüketimi";
            // 
            // formsPlotHourly
            // 
            this.formsPlotHourly.DisplayScale = 1F;
            this.formsPlotHourly.Dock = DockStyle.Fill;
            this.formsPlotHourly.Location = new Point(3, 20);
            this.formsPlotHourly.Margin = new Padding(3, 2, 3, 2);
            this.formsPlotHourly.Name = "formsPlotHourly";
            this.formsPlotHourly.Size = new Size(288, 76);
            this.formsPlotHourly.TabIndex = 0;
            // 
            // YENİ KOD BAŞLANGICI
            // gbHourlyOee
            // 
            this.gbHourlyOee.Controls.Add(this.formsPlotHourlyOee);
            this.gbHourlyOee.Dock = DockStyle.Fill;
            this.gbHourlyOee.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.gbHourlyOee.Location = new Point(3, 403);
            this.gbHourlyOee.Margin = new Padding(3, 2, 3, 2);
            this.gbHourlyOee.Name = "gbHourlyOee";
            this.gbHourlyOee.Padding = new Padding(3, 2, 3, 2);
            this.gbHourlyOee.Size = new Size(294, 100);
            this.gbHourlyOee.TabIndex = 4;
            this.gbHourlyOee.TabStop = false;
            this.gbHourlyOee.Text = "Son 24 Saatin OEE Ortalaması";
            // 
            // formsPlotHourlyOee
            // 
            this.formsPlotHourlyOee.DisplayScale = 1F;
            this.formsPlotHourlyOee.Dock = DockStyle.Fill;
            this.formsPlotHourlyOee.Location = new Point(3, 20);
            this.formsPlotHourlyOee.Margin = new Padding(3, 2, 3, 2);
            this.formsPlotHourlyOee.Name = "formsPlotHourlyOee";
            this.formsPlotHourlyOee.Size = new Size(288, 78);
            this.formsPlotHourlyOee.TabIndex = 0;
            // YENİ KOD BİTİŞİ
            // 
            // GenelBakis_Control
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new Padding(9, 8, 9, 8);
            this.Name = "GenelBakis_Control";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.Size = new Size(1050, 600);
            this.Load += new System.EventHandler(this.GenelBakis_Control_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.tlpMainLayout.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.tlpSidebarLayout.ResumeLayout(false);
            this.gbTopAlarms.ResumeLayout(false);
            this.gbHourlyConsumptionSteam.ResumeLayout(false);
            this.gbHourlyConsumptionWater.ResumeLayout(false);
            this.gbHourlyConsumption.ResumeLayout(false);
            // YENİ KOD BAŞLANGICI
            this.gbHourlyOee.ResumeLayout(false);
            // YENİ KOD BİTİŞİ
            this.ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.FlowLayoutPanel flpTopKpis;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel tlpMainLayout;
        private System.Windows.Forms.FlowLayoutPanel flpMachineGroups;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.GroupBox gbTopAlarms;
        private ScottPlot.WinForms.FormsPlot formsPlotTopAlarms;
        private System.Windows.Forms.GroupBox gbHourlyConsumption;
        private ScottPlot.WinForms.FormsPlot formsPlotHourly;
        private System.Windows.Forms.GroupBox gbHourlyConsumptionWater;
        private ScottPlot.WinForms.FormsPlot formsPlotHourlyWater;
        private System.Windows.Forms.GroupBox gbHourlyConsumptionSteam;
        private ScottPlot.WinForms.FormsPlot formsPlotHourlySteam;
        private System.Windows.Forms.TableLayoutPanel tlpSidebarLayout;
        // YENİ KOD BAŞLANGICI
        private System.Windows.Forms.GroupBox gbHourlyOee;
        private ScottPlot.WinForms.FormsPlot formsPlotHourlyOee;
        // YENİ KOD BİTİŞİ
    }
}