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
            this.label1 = new System.Windows.Forms.Label();
            this.numSicaklik = new System.Windows.Forms.NumericUpDown();
            this.numNem = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numZaman = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numCalismaDevri = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numSogutmaZamani = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.chkNemAktif = new System.Windows.Forms.CheckBox();
            this.chkZamanAktif = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numSicaklik)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZaman)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCalismaDevri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSogutmaZamani)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(20, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sıcaklık (°C):";
            // 
            // numSicaklik
            // 
            this.numSicaklik.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numSicaklik.Location = new System.Drawing.Point(170, 68);
            this.numSicaklik.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numSicaklik.Name = "numSicaklik";
            this.numSicaklik.Size = new System.Drawing.Size(150, 27);
            this.numSicaklik.TabIndex = 1;
            // 
            // numNem
            // 
            this.numNem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numNem.Location = new System.Drawing.Point(170, 111);
            this.numNem.Name = "numNem";
            this.numNem.Size = new System.Drawing.Size(150, 27);
            this.numNem.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(20, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nem (%):";
            // 
            // numZaman
            // 
            this.numZaman.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numZaman.Location = new System.Drawing.Point(170, 154);
            this.numZaman.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numZaman.Name = "numZaman";
            this.numZaman.Size = new System.Drawing.Size(150, 27);
            this.numZaman.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(20, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Süre (dk):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(20, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(271, 28);
            this.label4.TabIndex = 6;
            this.label4.Text = "Kurutma Reçete Parametleri";
            // 
            // numCalismaDevri
            // 
            this.numCalismaDevri.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numCalismaDevri.Location = new System.Drawing.Point(170, 197);
            this.numCalismaDevri.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCalismaDevri.Name = "numCalismaDevri";
            this.numCalismaDevri.Size = new System.Drawing.Size(150, 27);
            this.numCalismaDevri.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(20, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Çalışma Devri (rpm):";
            // 
            // numSogutmaZamani
            // 
            this.numSogutmaZamani.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numSogutmaZamani.Location = new System.Drawing.Point(170, 240);
            this.numSogutmaZamani.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSogutmaZamani.Name = "numSogutmaZamani";
            this.numSogutmaZamani.Size = new System.Drawing.Size(150, 27);
            this.numSogutmaZamani.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(20, 242);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 20);
            this.label6.TabIndex = 9;
            this.label6.Text = "Soğutma Süresi (dk):";
            // 
            // chkNemAktif
            // 
            this.chkNemAktif.AutoSize = true;
            this.chkNemAktif.Location = new System.Drawing.Point(326, 115);
            this.chkNemAktif.Name = "chkNemAktif";
            this.chkNemAktif.Size = new System.Drawing.Size(61, 24);
            this.chkNemAktif.TabIndex = 11;
            this.chkNemAktif.Text = "Aktif";
            this.chkNemAktif.UseVisualStyleBackColor = true;
            // 
            // chkZamanAktif
            // 
            this.chkZamanAktif.AutoSize = true;
            this.chkZamanAktif.Location = new System.Drawing.Point(326, 158);
            this.chkZamanAktif.Name = "chkZamanAktif";
            this.chkZamanAktif.Size = new System.Drawing.Size(61, 24);
            this.chkZamanAktif.TabIndex = 12;
            this.chkZamanAktif.Text = "Aktif";
            this.chkZamanAktif.UseVisualStyleBackColor = true;
            // 
            // KurutmaReçete_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkZamanAktif);
            this.Controls.Add(this.chkNemAktif);
            this.Controls.Add(this.numSogutmaZamani);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numCalismaDevri);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numZaman);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numNem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSicaklik);
            this.Controls.Add(this.label1);
            this.Name = "KurutmaReçete_Control";
            this.Size = new System.Drawing.Size(400, 450);
            ((System.ComponentModel.ISupportInitialize)(this.numSicaklik)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZaman)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCalismaDevri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSogutmaZamani)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
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