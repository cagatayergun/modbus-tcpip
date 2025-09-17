// UI/Controls/RecipeStepEditors/DozajEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class DozajEditor_Boyama_Control
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }
        #region Component Designer generated code
        private void InitializeComponent()
        {
            label1 = new Label();
            txtKimyasal = new TextBox();
            label2 = new Label();
            numTankSu = new NumericUpDown();
            label3 = new Label();
            numCozmeSure = new NumericUpDown();
            label4 = new Label();
            numDozajSure = new NumericUpDown();
            label5 = new Label();
            numDozajLitre = new NumericUpDown();
            chkAnaTankMakSu = new CheckBox();
            chkAnaTankTemizSu = new CheckBox();
            chkTank1Su = new CheckBox();
            chkTank1Dozaj = new CheckBox();
            chkTamburDur = new CheckBox();
            chkAlarm = new CheckBox();
            dozajbasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numTankSu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCozmeSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDozajSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDozajLitre).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 47);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 0;
            label1.Text = "Chemical:";
            // 
            // txtKimyasal
            // 
            txtKimyasal.Location = new Point(174, 43);
            txtKimyasal.Margin = new Padding(3, 2, 3, 2);
            txtKimyasal.MaxLength = 6;
            txtKimyasal.Name = "txtKimyasal";
            txtKimyasal.Size = new Size(132, 23);
            txtKimyasal.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 77);
            label2.Name = "label2";
            label2.Size = new Size(122, 15);
            label2.TabIndex = 2;
            label2.Text = "Tank Water Receiving:";
            // 
            // numTankSu
            // 
            numTankSu.Location = new Point(174, 73);
            numTankSu.Margin = new Padding(3, 2, 3, 2);
            numTankSu.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numTankSu.Name = "numTankSu";
            numTankSu.Size = new Size(131, 23);
            numTankSu.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 107);
            label3.Name = "label3";
            label3.Size = new Size(151, 15);
            label3.TabIndex = 4;
            label3.Text = "Chemical Dissolution Time:";
            // 
            // numCozmeSure
            // 
            numCozmeSure.Location = new Point(174, 103);
            numCozmeSure.Margin = new Padding(3, 2, 3, 2);
            numCozmeSure.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numCozmeSure.Name = "numCozmeSure";
            numCozmeSure.Size = new Size(131, 23);
            numCozmeSure.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 137);
            label4.Name = "label4";
            label4.Size = new Size(131, 15);
            label4.TabIndex = 6;
            label4.Text = "Chemical Dosage Time:";
            // 
            // numDozajSure
            // 
            numDozajSure.Location = new Point(174, 133);
            numDozajSure.Margin = new Padding(3, 2, 3, 2);
            numDozajSure.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDozajSure.Name = "numDozajSure";
            numDozajSure.Size = new Size(131, 23);
            numDozajSure.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 167);
            label5.Name = "label5";
            label5.Size = new Size(93, 15);
            label5.TabIndex = 8;
            label5.Text = "Dosage in Liters:";
            // 
            // numDozajLitre
            // 
            numDozajLitre.Location = new Point(174, 163);
            numDozajLitre.Margin = new Padding(3, 2, 3, 2);
            numDozajLitre.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDozajLitre.Name = "numDozajLitre";
            numDozajLitre.Size = new Size(131, 23);
            numDozajLitre.TabIndex = 9;
            // 
            // chkAnaTankMakSu
            // 
            chkAnaTankMakSu.AutoSize = true;
            chkAnaTankMakSu.Location = new Point(12, 199);
            chkAnaTankMakSu.Margin = new Padding(3, 2, 3, 2);
            chkAnaTankMakSu.Name = "chkAnaTankMakSu";
            chkAnaTankMakSu.Size = new Size(198, 19);
            chkAnaTankMakSu.TabIndex = 10;
            chkAnaTankMakSu.Text = "Main Tank Machine Water Intake";
            chkAnaTankMakSu.UseVisualStyleBackColor = true;
            // 
            // chkAnaTankTemizSu
            // 
            chkAnaTankTemizSu.AutoSize = true;
            chkAnaTankTemizSu.Location = new Point(12, 222);
            chkAnaTankTemizSu.Margin = new Padding(3, 2, 3, 2);
            chkAnaTankTemizSu.Name = "chkAnaTankTemizSu";
            chkAnaTankTemizSu.Size = new Size(198, 19);
            chkAnaTankTemizSu.TabIndex = 11;
            chkAnaTankTemizSu.Text = "Main Tank Machine Water Intake";
            chkAnaTankTemizSu.UseVisualStyleBackColor = true;
            // 
            // chkTank1Su
            // 
            chkTank1Su.AutoSize = true;
            chkTank1Su.Location = new Point(12, 244);
            chkTank1Su.Margin = new Padding(3, 2, 3, 2);
            chkTank1Su.Name = "chkTank1Su";
            chkTank1Su.Size = new Size(122, 19);
            chkTank1Su.TabIndex = 12;
            chkTank1Su.Text = "1. Take Tank Water";
            chkTank1Su.UseVisualStyleBackColor = true;
            // 
            // chkTank1Dozaj
            // 
            chkTank1Dozaj.AutoSize = true;
            chkTank1Dozaj.Location = new Point(12, 266);
            chkTank1Dozaj.Margin = new Padding(3, 2, 3, 2);
            chkTank1Dozaj.Name = "chkTank1Dozaj";
            chkTank1Dozaj.Size = new Size(102, 19);
            chkTank1Dozaj.TabIndex = 13;
            chkTank1Dozaj.Text = "1. Tank Dosing";
            chkTank1Dozaj.UseVisualStyleBackColor = true;
            // 
            // chkTamburDur
            // 
            chkTamburDur.AutoSize = true;
            chkTamburDur.Location = new Point(12, 289);
            chkTamburDur.Margin = new Padding(3, 2, 3, 2);
            chkTamburDur.Name = "chkTamburDur";
            chkTamburDur.Size = new Size(83, 19);
            chkTamburDur.TabIndex = 14;
            chkTamburDur.Text = "Drum Stop";
            chkTamburDur.UseVisualStyleBackColor = true;
            // 
            // chkAlarm
            // 
            chkAlarm.AutoSize = true;
            chkAlarm.Location = new Point(12, 312);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 15;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // dozajbasliktext
            // 
            dozajbasliktext.AutoSize = true;
            dozajbasliktext.Location = new Point(129, 11);
            dozajbasliktext.Name = "dozajbasliktext";
            dozajbasliktext.Size = new Size(52, 15);
            dozajbasliktext.TabIndex = 16;
            dozajbasliktext.Text = "DOSAGE";
            // 
            // DozajEditor_Boyama_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(dozajbasliktext);
            Controls.Add(chkAlarm);
            Controls.Add(chkTamburDur);
            Controls.Add(chkTank1Dozaj);
            Controls.Add(chkTank1Su);
            Controls.Add(chkAnaTankTemizSu);
            Controls.Add(chkAnaTankMakSu);
            Controls.Add(numDozajLitre);
            Controls.Add(label5);
            Controls.Add(numDozajSure);
            Controls.Add(label4);
            Controls.Add(numCozmeSure);
            Controls.Add(label3);
            Controls.Add(numTankSu);
            Controls.Add(label2);
            Controls.Add(txtKimyasal);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "DozajEditor_Boyama_Control";
            Size = new Size(393, 341);
            ((System.ComponentModel.ISupportInitialize)numTankSu).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCozmeSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDozajSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDozajLitre).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKimyasal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numTankSu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCozmeSure;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numDozajSure;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numDozajLitre;
        private System.Windows.Forms.CheckBox chkAnaTankMakSu;
        private System.Windows.Forms.CheckBox chkAnaTankTemizSu;
        private System.Windows.Forms.CheckBox chkTank1Su;
        private System.Windows.Forms.CheckBox chkTank1Dozaj;
        private System.Windows.Forms.CheckBox chkTamburDur;
        private System.Windows.Forms.CheckBox chkAlarm;
        private Label dozajbasliktext;
    }
}
