namespace TekstilScada.UI.Views
{
    partial class RecipeOptimization_Control
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
            btnAnalyze = new Button();
            cmbRecipe2 = new ComboBox();
            label7 = new Label();
            cmbRecipes = new ComboBox();
            label1 = new Label();
            pnlContent = new Panel();
            dgvHistory = new DataGridView();
            label2 = new Label();
            pnlAverages = new Panel();
            pnlCompare = new Panel();
            lblAvgSteam2 = new Label();
            label8 = new Label();
            lblAvgElectricity2 = new Label();
            label10 = new Label();
            lblAvgCycleTime2 = new Label();
            label12 = new Label();
            lblAvgWater2 = new Label();
            label14 = new Label();
            pnlOriginal = new Panel();
            lblAvgSteam = new Label();
            label6 = new Label();
            lblAvgElectricity = new Label();
            label5 = new Label();
            lblAvgCycleTime = new Label();
            label4 = new Label();
            lblAvgWater = new Label();
            label3 = new Label();
            pnlFilters.SuspendLayout();
            pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            pnlAverages.SuspendLayout();
            pnlCompare.SuspendLayout();
            pnlOriginal.SuspendLayout();
            SuspendLayout();
            // 
            // pnlFilters
            // 
            pnlFilters.Controls.Add(btnAnalyze);
            pnlFilters.Controls.Add(cmbRecipe2);
            pnlFilters.Controls.Add(label7);
            pnlFilters.Controls.Add(cmbRecipes);
            pnlFilters.Controls.Add(label1);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Margin = new Padding(3, 2, 3, 2);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(788, 60);
            pnlFilters.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAnalyze.Location = new Point(668, 19);
            btnAnalyze.Margin = new Padding(3, 2, 3, 2);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(105, 22);
            btnAnalyze.TabIndex = 2;
            btnAnalyze.Text = "Analyze";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // cmbRecipe2
            // 
            cmbRecipe2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecipe2.FormattingEnabled = true;
            cmbRecipe2.Location = new Point(476, 20);
            cmbRecipe2.Margin = new Padding(3, 2, 3, 2);
            cmbRecipe2.Name = "cmbRecipe2";
            cmbRecipe2.Size = new Size(176, 23);
            cmbRecipe2.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label7.Location = new Point(371, 22);
            label7.Name = "label7";
            label7.Size = new Size(75, 15);
            label7.TabIndex = 4;
            label7.Text = "To compare:";
            // 
            // cmbRecipes
            // 
            cmbRecipes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecipes.FormattingEnabled = true;
            cmbRecipes.Location = new Point(100, 20);
            cmbRecipes.Margin = new Padding(3, 2, 3, 2);
            cmbRecipes.Name = "cmbRecipes";
            cmbRecipes.Size = new Size(263, 23);
            cmbRecipes.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 22);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 0;
            label1.Text = "Main Recipe:";
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(dgvHistory);
            pnlContent.Controls.Add(label2);
            pnlContent.Controls.Add(pnlAverages);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 60);
            pnlContent.Margin = new Padding(3, 2, 3, 2);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(788, 390);
            pnlContent.TabIndex = 1;
            // 
            // dgvHistory
            // 
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.Location = new Point(0, 128);
            dgvHistory.Margin = new Padding(3, 2, 3, 2);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.ReadOnly = true;
            dgvHistory.RowHeadersWidth = 51;
            dgvHistory.RowTemplate.Height = 29;
            dgvHistory.Size = new Size(788, 262);
            dgvHistory.TabIndex = 2;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Top;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label2.Location = new Point(0, 94);
            label2.Name = "label2";
            label2.Size = new Size(788, 34);
            label2.TabIndex = 1;
            label2.Text = "Geçmiş Üretimler";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlAverages
            // 
            pnlAverages.Controls.Add(pnlCompare);
            pnlAverages.Controls.Add(pnlOriginal);
            pnlAverages.Dock = DockStyle.Top;
            pnlAverages.Location = new Point(0, 0);
            pnlAverages.Margin = new Padding(3, 2, 3, 2);
            pnlAverages.Name = "pnlAverages";
            pnlAverages.Size = new Size(788, 94);
            pnlAverages.TabIndex = 0;
            // 
            // pnlCompare
            // 
            pnlCompare.Controls.Add(lblAvgSteam2);
            pnlCompare.Controls.Add(label8);
            pnlCompare.Controls.Add(lblAvgElectricity2);
            pnlCompare.Controls.Add(label10);
            pnlCompare.Controls.Add(lblAvgCycleTime2);
            pnlCompare.Controls.Add(label12);
            pnlCompare.Controls.Add(lblAvgWater2);
            pnlCompare.Controls.Add(label14);
            pnlCompare.Dock = DockStyle.Fill;
            pnlCompare.Location = new Point(394, 0);
            pnlCompare.Margin = new Padding(3, 2, 3, 2);
            pnlCompare.Name = "pnlCompare";
            pnlCompare.Size = new Size(394, 94);
            pnlCompare.TabIndex = 1;
            // 
            // lblAvgSteam2
            // 
            lblAvgSteam2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgSteam2.Location = new Point(197, 64);
            lblAvgSteam2.Name = "lblAvgSteam2";
            lblAvgSteam2.Size = new Size(175, 30);
            lblAvgSteam2.TabIndex = 15;
            lblAvgSteam2.Text = "0 kg";
            lblAvgSteam2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label8.Location = new Point(197, 45);
            label8.Name = "label8";
            label8.Size = new Size(175, 19);
            label8.TabIndex = 14;
            label8.Text = "Average Steam Consumption";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgElectricity2
            // 
            lblAvgElectricity2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgElectricity2.Location = new Point(13, 64);
            lblAvgElectricity2.Name = "lblAvgElectricity2";
            lblAvgElectricity2.Size = new Size(175, 30);
            lblAvgElectricity2.TabIndex = 13;
            lblAvgElectricity2.Text = "0 kW";
            lblAvgElectricity2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label10.Location = new Point(13, 45);
            label10.Name = "label10";
            label10.Size = new Size(175, 19);
            label10.TabIndex = 12;
            label10.Text = "Average Electricity Consumption";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgCycleTime2
            // 
            lblAvgCycleTime2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgCycleTime2.Location = new Point(197, 19);
            lblAvgCycleTime2.Name = "lblAvgCycleTime2";
            lblAvgCycleTime2.Size = new Size(175, 30);
            lblAvgCycleTime2.TabIndex = 11;
            lblAvgCycleTime2.Text = "00:00:00";
            lblAvgCycleTime2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label12.Location = new Point(197, 0);
            label12.Name = "label12";
            label12.Size = new Size(175, 19);
            label12.TabIndex = 10;
            label12.Text = "Average Cycle Time";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgWater2
            // 
            lblAvgWater2.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgWater2.Location = new Point(13, 19);
            lblAvgWater2.Name = "lblAvgWater2";
            lblAvgWater2.Size = new Size(175, 30);
            lblAvgWater2.TabIndex = 9;
            lblAvgWater2.Text = "0 L";
            lblAvgWater2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label14.Location = new Point(13, 0);
            label14.Name = "label14";
            label14.Size = new Size(175, 19);
            label14.TabIndex = 8;
            label14.Text = "Average Water Consumption";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlOriginal
            // 
            pnlOriginal.Controls.Add(lblAvgSteam);
            pnlOriginal.Controls.Add(label6);
            pnlOriginal.Controls.Add(lblAvgElectricity);
            pnlOriginal.Controls.Add(label5);
            pnlOriginal.Controls.Add(lblAvgCycleTime);
            pnlOriginal.Controls.Add(label4);
            pnlOriginal.Controls.Add(lblAvgWater);
            pnlOriginal.Controls.Add(label3);
            pnlOriginal.Dock = DockStyle.Left;
            pnlOriginal.Location = new Point(0, 0);
            pnlOriginal.Margin = new Padding(3, 2, 3, 2);
            pnlOriginal.Name = "pnlOriginal";
            pnlOriginal.Size = new Size(394, 94);
            pnlOriginal.TabIndex = 0;
            // 
            // lblAvgSteam
            // 
            lblAvgSteam.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgSteam.Location = new Point(197, 64);
            lblAvgSteam.Name = "lblAvgSteam";
            lblAvgSteam.Size = new Size(175, 30);
            lblAvgSteam.TabIndex = 7;
            lblAvgSteam.Text = "0 kg";
            lblAvgSteam.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.Location = new Point(197, 45);
            label6.Name = "label6";
            label6.Size = new Size(175, 19);
            label6.TabIndex = 6;
            label6.Text = "Average Steam Consumption";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgElectricity
            // 
            lblAvgElectricity.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgElectricity.Location = new Point(13, 64);
            lblAvgElectricity.Name = "lblAvgElectricity";
            lblAvgElectricity.Size = new Size(175, 30);
            lblAvgElectricity.TabIndex = 5;
            lblAvgElectricity.Text = "0 kW";
            lblAvgElectricity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(13, 45);
            label5.Name = "label5";
            label5.Size = new Size(175, 19);
            label5.TabIndex = 4;
            label5.Text = "Average Electricity Consumption";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgCycleTime
            // 
            lblAvgCycleTime.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgCycleTime.Location = new Point(197, 19);
            lblAvgCycleTime.Name = "lblAvgCycleTime";
            lblAvgCycleTime.Size = new Size(175, 30);
            lblAvgCycleTime.TabIndex = 3;
            lblAvgCycleTime.Text = "00:00:00";
            lblAvgCycleTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(197, 0);
            label4.Name = "label4";
            label4.Size = new Size(175, 19);
            label4.TabIndex = 2;
            label4.Text = "Average Cycle Time";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvgWater
            // 
            lblAvgWater.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblAvgWater.Location = new Point(13, 19);
            lblAvgWater.Name = "lblAvgWater";
            lblAvgWater.Size = new Size(175, 30);
            lblAvgWater.TabIndex = 1;
            lblAvgWater.Text = "0 L";
            lblAvgWater.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(13, 0);
            label3.Name = "label3";
            label3.Size = new Size(175, 19);
            label3.TabIndex = 0;
            label3.Text = "Average Water Consumption";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RecipeOptimization_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContent);
            Controls.Add(pnlFilters);
            Margin = new Padding(3, 2, 3, 2);
            Name = "RecipeOptimization_Control";
            Size = new Size(788, 450);
            Load += RecipeOptimization_Control_Load;
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            pnlAverages.ResumeLayout(false);
            pnlCompare.ResumeLayout(false);
            pnlOriginal.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.ComboBox cmbRecipes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlAverages;
        private System.Windows.Forms.Label lblAvgCycleTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblAvgWater;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblAvgSteam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAvgElectricity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbRecipe2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnlCompare;
        private System.Windows.Forms.Label lblAvgSteam2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblAvgElectricity2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblAvgCycleTime2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblAvgWater2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel pnlOriginal;
    }
}