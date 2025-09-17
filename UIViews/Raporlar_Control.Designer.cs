namespace TekstilScada.UI.Views
{
    partial class Raporlar_Control
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
            this.tabControlReports = new System.Windows.Forms.TabControl();
            this.tabPageAlarmReport = new System.Windows.Forms.TabPage();
            this.tabPageProductionReport = new System.Windows.Forms.TabPage();
            // YENİ: Yeni sekmeler eklendi
            this.tabPageManualReport = new System.Windows.Forms.TabPage(); // YENİ
            this.tabPageOeeReport = new System.Windows.Forms.TabPage();
            this.tabPageTrendAnalysis = new System.Windows.Forms.TabPage();
            this.tabPageRecipeOptimization = new System.Windows.Forms.TabPage();
            this.tabPageGenelUretim = new System.Windows.Forms.TabPage();
            this.tabPageActionLog = new System.Windows.Forms.TabPage();
            this.tabControlReports.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlReports
            // 
            this.tabControlReports.Controls.Add(this.tabPageProductionReport);
            this.tabControlReports.Controls.Add(this.tabPageAlarmReport);
            this.tabControlReports.Controls.Add(this.tabPageOeeReport);
            this.tabControlReports.Controls.Add(this.tabPageTrendAnalysis);
            this.tabControlReports.Controls.Add(this.tabPageRecipeOptimization);
            this.tabControlReports.Controls.Add(this.tabPageManualReport); // YENİ
            this.tabControlReports.Controls.Add(this.tabPageGenelUretim); // YENİ
            this.tabControlReports.Controls.Add(this.tabPageActionLog);
            this.tabControlReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlReports.Location = new System.Drawing.Point(0, 0);
            this.tabControlReports.Name = "tabControlReports";
            this.tabControlReports.SelectedIndex = 0;
            this.tabControlReports.Size = new System.Drawing.Size(800, 600);
            this.tabControlReports.TabIndex = 0;
            // 
            // tabPageAlarmReport
            // 
            this.tabPageAlarmReport.Location = new System.Drawing.Point(4, 29);
            this.tabPageAlarmReport.Name = "tabPageAlarmReport";
            this.tabPageAlarmReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAlarmReport.Size = new System.Drawing.Size(792, 567);
            this.tabPageAlarmReport.TabIndex = 0;
            this.tabPageAlarmReport.Text = "Past Alarms";
            this.tabPageAlarmReport.UseVisualStyleBackColor = true;
            // 
            // tabPageProductionReport
            // 
            this.tabPageProductionReport.Location = new System.Drawing.Point(4, 29);
            this.tabPageProductionReport.Name = "tabPageProductionReport";
            this.tabPageProductionReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProductionReport.Size = new System.Drawing.Size(792, 567);
            this.tabPageProductionReport.TabIndex = 1;
            this.tabPageProductionReport.Text = "Production Report";
            this.tabPageProductionReport.UseVisualStyleBackColor = true;
            //
            // tabPageOeeReport
            //
            this.tabPageOeeReport.Name = "tabPageOeeReport";
            this.tabPageOeeReport.TabIndex = 2;
            this.tabPageOeeReport.Text = "OEE Report";
            //
            // tabPageTrendAnalysis
            //
            this.tabPageTrendAnalysis.Name = "tabPageTrendAnalysis";
            this.tabPageTrendAnalysis.TabIndex = 3;
            this.tabPageTrendAnalysis.Text = "Trend Analysis";
            // 
            // tabPageRecipeOptimization
            // 
            this.tabPageRecipeOptimization.Location = new System.Drawing.Point(4, 29);
            this.tabPageRecipeOptimization.Name = "tabPageRecipeOptimization";
            this.tabPageRecipeOptimization.Size = new System.Drawing.Size(792, 567);
            this.tabPageRecipeOptimization.TabIndex = 4;
            this.tabPageRecipeOptimization.Text = "Prescription Consumption Analysis"; // GÜNCELLENDİ
            this.tabPageRecipeOptimization.UseVisualStyleBackColor = true;
            // 
            // tabPageManualReport
            // 
            this.tabPageManualReport.Location = new System.Drawing.Point(4, 29);
            this.tabPageManualReport.Name = "tabPageManualReport";
            this.tabPageManualReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageManualReport.Size = new System.Drawing.Size(792, 567);
            this.tabPageManualReport.TabIndex = 5; // Sıradaki index
            this.tabPageManualReport.Text = "Manual Consumption";
            this.tabPageManualReport.UseVisualStyleBackColor = true;
            // 
            // tabPageProductionReport
            // 
            this.tabPageGenelUretim.Location = new System.Drawing.Point(4, 29);
            this.tabPageGenelUretim.Name = "tabPageGenelUretim";
            this.tabPageGenelUretim.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGenelUretim.Size = new System.Drawing.Size(792, 567);
            this.tabPageGenelUretim.TabIndex = 6;
            this.tabPageGenelUretim.Text = "General Consumption Report";
            this.tabPageGenelUretim.UseVisualStyleBackColor = true;
            // Yeni tabPage'in ayarlarını yapın
           // this.tabPageActionLog.Controls.Add(this.actionLogReport_Control1);
            this.tabPageActionLog.Location = new System.Drawing.Point(4, 29);
            this.tabPageActionLog.Name = "tabPageActionLog";
            this.tabPageActionLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageActionLog.Size = new System.Drawing.Size(992, 608);
            this.tabPageActionLog.TabIndex = 4;
            this.tabPageActionLog.Text = "Action Records";
            this.tabPageActionLog.UseVisualStyleBackColor = true;
            // 
            // Raporlar_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlReports);
            this.Name = "Raporlar_Control";
            this.Size = new System.Drawing.Size(800, 600);
            this.tabControlReports.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.TabPage tabPageActionLog;
        private System.Windows.Forms.TabControl tabControlReports;
        private System.Windows.Forms.TabPage tabPageAlarmReport;
        private System.Windows.Forms.TabPage tabPageProductionReport;
        private System.Windows.Forms.TabPage tabPageOeeReport;
        private System.Windows.Forms.TabPage tabPageTrendAnalysis;
        private System.Windows.Forms.TabPage tabPageRecipeOptimization;
        private System.Windows.Forms.TabPage tabPageManualReport; // YENİ
        private System.Windows.Forms.TabPage tabPageGenelUretim; // YENİ
    }
}