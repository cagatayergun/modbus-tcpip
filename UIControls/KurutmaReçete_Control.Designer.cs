// UI/Controls/KurutmaReçete_Control.Designer.cs
namespace TekstilScada.UI.Controls
{
    partial class KurutmaReçete_Control
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
            label1 = new Label();
            numSicaklik = new NumericUpDown();
            numNem = new NumericUpDown();
            label2 = new Label();
            numZaman = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            numCalismaDevri = new NumericUpDown();
            label5 = new Label();
            numSogutmaZamani = new NumericUpDown();
            label6 = new Label();
            chkNemAktif = new CheckBox();
            chkZamanAktif = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numSicaklik).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numNem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numZaman).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSogutmaZamani).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F);
            label1.Location = new Point(18, 52);
            label1.Name = "label1";
            label1.Size = new Size(100, 15);
            label1.TabIndex = 0;
            label1.Text = "Temperature (°C):";
            // 
            // numSicaklik
            // 
            numSicaklik.Font = new Font("Segoe UI", 9F);
            numSicaklik.Location = new Point(149, 51);
            numSicaklik.Margin = new Padding(3, 2, 3, 2);
            numSicaklik.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numSicaklik.Name = "numSicaklik";
            numSicaklik.Size = new Size(131, 23);
            numSicaklik.TabIndex = 1;
            // 
            // numNem
            // 
            numNem.Font = new Font("Segoe UI", 9F);
            numNem.Location = new Point(149, 83);
            numNem.Margin = new Padding(3, 2, 3, 2);
            numNem.Name = "numNem";
            numNem.Size = new Size(131, 23);
            numNem.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F);
            label2.Location = new Point(18, 85);
            label2.Name = "label2";
            label2.Size = new Size(81, 15);
            label2.TabIndex = 2;
            label2.Text = "Humidity (%):";
            // 
            // numZaman
            // 
            numZaman.Font = new Font("Segoe UI", 9F);
            numZaman.Location = new Point(149, 116);
            numZaman.Margin = new Padding(3, 2, 3, 2);
            numZaman.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numZaman.Name = "numZaman";
            numZaman.Size = new Size(131, 23);
            numZaman.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F);
            label3.Location = new Point(18, 117);
            label3.Name = "label3";
            label3.Size = new Size(88, 15);
            label3.TabIndex = 4;
            label3.Text = "Duration (min):";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.Location = new Point(18, 15);
            label4.Name = "label4";
            label4.Size = new Size(208, 21);
            label4.TabIndex = 6;
            label4.Text = "Drying Recipe Parameters";
            // 
            // numCalismaDevri
            // 
            numCalismaDevri.Font = new Font("Segoe UI", 9F);
            numCalismaDevri.Location = new Point(149, 148);
            numCalismaDevri.Margin = new Padding(3, 2, 3, 2);
            numCalismaDevri.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numCalismaDevri.Name = "numCalismaDevri";
            numCalismaDevri.Size = new Size(131, 23);
            numCalismaDevri.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F);
            label5.Location = new Point(18, 149);
            label5.Name = "label5";
            label5.Size = new Size(131, 15);
            label5.TabIndex = 7;
            label5.Text = "Operating Speed ​​(rpm):";
            // 
            // numSogutmaZamani
            // 
            numSogutmaZamani.Font = new Font("Segoe UI", 9F);
            numSogutmaZamani.Location = new Point(149, 180);
            numSogutmaZamani.Margin = new Padding(3, 2, 3, 2);
            numSogutmaZamani.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSogutmaZamani.Name = "numSogutmaZamani";
            numSogutmaZamani.Size = new Size(131, 23);
            numSogutmaZamani.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F);
            label6.Location = new Point(18, 182);
            label6.Name = "label6";
            label6.Size = new Size(113, 15);
            label6.TabIndex = 9;
            label6.Text = "Cooling Time (min):";
            // 
            // chkNemAktif
            // 
            chkNemAktif.AutoSize = true;
            chkNemAktif.Location = new Point(285, 86);
            chkNemAktif.Margin = new Padding(3, 2, 3, 2);
            chkNemAktif.Name = "chkNemAktif";
            chkNemAktif.Size = new Size(59, 19);
            chkNemAktif.TabIndex = 11;
            chkNemAktif.Text = "Active";
            chkNemAktif.UseVisualStyleBackColor = true;
            // 
            // chkZamanAktif
            // 
            chkZamanAktif.AutoSize = true;
            chkZamanAktif.Location = new Point(285, 118);
            chkZamanAktif.Margin = new Padding(3, 2, 3, 2);
            chkZamanAktif.Name = "chkZamanAktif";
            chkZamanAktif.Size = new Size(59, 19);
            chkZamanAktif.TabIndex = 12;
            chkZamanAktif.Text = "Active";
            chkZamanAktif.UseVisualStyleBackColor = true;
            // 
            // KurutmaReçete_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkZamanAktif);
            Controls.Add(chkNemAktif);
            Controls.Add(numSogutmaZamani);
            Controls.Add(label6);
            Controls.Add(numCalismaDevri);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(numZaman);
            Controls.Add(label3);
            Controls.Add(numNem);
            Controls.Add(label2);
            Controls.Add(numSicaklik);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "KurutmaReçete_Control";
            Size = new Size(350, 338);
            ((System.ComponentModel.ISupportInitialize)numSicaklik).EndInit();
            ((System.ComponentModel.ISupportInitialize)numNem).EndInit();
            ((System.ComponentModel.ISupportInitialize)numZaman).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSogutmaZamani).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSicaklik;
        private System.Windows.Forms.NumericUpDown numNem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numZaman;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numCalismaDevri;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numSogutmaZamani;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkNemAktif;
        private System.Windows.Forms.CheckBox chkZamanAktif;
    }
}