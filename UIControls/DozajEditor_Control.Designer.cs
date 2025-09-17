// UI/Controls/RecipeStepEditors/DozajEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class DozajEditor_Control
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
            dozajbasliktext = new Label();
            chkAlarm = new CheckBox();
            chkTamburDur = new CheckBox();
            chkTank1Dozaj = new CheckBox();
            chkTank1Su = new CheckBox();
            chkAnaTankTemizSu = new CheckBox();
            chkAnaTankMakSu = new CheckBox();
            numDozajLitre = new NumericUpDown();
            label5 = new Label();
            numDozajSure = new NumericUpDown();
            label4 = new Label();
            numCozmeSure = new NumericUpDown();
            label3 = new Label();
            numTankSu = new NumericUpDown();
            label2 = new Label();
            txtKimyasal = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)numDozajLitre).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDozajSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCozmeSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTankSu).BeginInit();
            SuspendLayout();
            // 
            // dozajbasliktext
            // 
            dozajbasliktext.AutoSize = true;
            dozajbasliktext.Location = new Point(121, 10);
            dozajbasliktext.Name = "dozajbasliktext";
            dozajbasliktext.Size = new Size(52, 15);
            dozajbasliktext.TabIndex = 33;
            dozajbasliktext.Text = "DOSAGE";
            // 
            // chkAlarm
            // 
            chkAlarm.AutoSize = true;
            chkAlarm.Location = new Point(4, 311);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 32;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // chkTamburDur
            // 
            chkTamburDur.AutoSize = true;
            chkTamburDur.Location = new Point(4, 288);
            chkTamburDur.Margin = new Padding(3, 2, 3, 2);
            chkTamburDur.Name = "chkTamburDur";
            chkTamburDur.Size = new Size(83, 19);
            chkTamburDur.TabIndex = 31;
            chkTamburDur.Text = "Drum Stop";
            chkTamburDur.UseVisualStyleBackColor = true;
            // 
            // chkTank1Dozaj
            // 
            chkTank1Dozaj.AutoSize = true;
            chkTank1Dozaj.Location = new Point(4, 265);
            chkTank1Dozaj.Margin = new Padding(3, 2, 3, 2);
            chkTank1Dozaj.Name = "chkTank1Dozaj";
            chkTank1Dozaj.Size = new Size(102, 19);
            chkTank1Dozaj.TabIndex = 30;
            chkTank1Dozaj.Text = "1. Tank Dosing";
            chkTank1Dozaj.UseVisualStyleBackColor = true;
            // 
            // chkTank1Su
            // 
            chkTank1Su.AutoSize = true;
            chkTank1Su.Location = new Point(4, 243);
            chkTank1Su.Margin = new Padding(3, 2, 3, 2);
            chkTank1Su.Name = "chkTank1Su";
            chkTank1Su.Size = new Size(122, 19);
            chkTank1Su.TabIndex = 29;
            chkTank1Su.Text = "1. Take Tank Water";
            chkTank1Su.UseVisualStyleBackColor = true;
            // 
            // chkAnaTankTemizSu
            // 
            chkAnaTankTemizSu.AutoSize = true;
            chkAnaTankTemizSu.Location = new Point(4, 221);
            chkAnaTankTemizSu.Margin = new Padding(3, 2, 3, 2);
            chkAnaTankTemizSu.Name = "chkAnaTankTemizSu";
            chkAnaTankTemizSu.Size = new Size(198, 19);
            chkAnaTankTemizSu.TabIndex = 28;
            chkAnaTankTemizSu.Text = "Main Tank Machine Water Intake";
            chkAnaTankTemizSu.UseVisualStyleBackColor = true;
            // 
            // chkAnaTankMakSu
            // 
            chkAnaTankMakSu.AutoSize = true;
            chkAnaTankMakSu.Location = new Point(4, 198);
            chkAnaTankMakSu.Margin = new Padding(3, 2, 3, 2);
            chkAnaTankMakSu.Name = "chkAnaTankMakSu";
            chkAnaTankMakSu.Size = new Size(198, 19);
            chkAnaTankMakSu.TabIndex = 27;
            chkAnaTankMakSu.Text = "Main Tank Machine Water Intake";
            chkAnaTankMakSu.UseVisualStyleBackColor = true;
            // 
            // numDozajLitre
            // 
            numDozajLitre.Location = new Point(166, 162);
            numDozajLitre.Margin = new Padding(3, 2, 3, 2);
            numDozajLitre.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDozajLitre.Name = "numDozajLitre";
            numDozajLitre.Size = new Size(131, 23);
            numDozajLitre.TabIndex = 26;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 166);
            label5.Name = "label5";
            label5.Size = new Size(93, 15);
            label5.TabIndex = 25;
            label5.Text = "Dosage in Liters:";
            // 
            // numDozajSure
            // 
            numDozajSure.Location = new Point(166, 132);
            numDozajSure.Margin = new Padding(3, 2, 3, 2);
            numDozajSure.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDozajSure.Name = "numDozajSure";
            numDozajSure.Size = new Size(131, 23);
            numDozajSure.TabIndex = 24;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 136);
            label4.Name = "label4";
            label4.Size = new Size(131, 15);
            label4.TabIndex = 23;
            label4.Text = "Chemical Dosage Time:";
            // 
            // numCozmeSure
            // 
            numCozmeSure.Location = new Point(166, 102);
            numCozmeSure.Margin = new Padding(3, 2, 3, 2);
            numCozmeSure.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numCozmeSure.Name = "numCozmeSure";
            numCozmeSure.Size = new Size(131, 23);
            numCozmeSure.TabIndex = 22;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 106);
            label3.Name = "label3";
            label3.Size = new Size(151, 15);
            label3.TabIndex = 21;
            label3.Text = "Chemical Dissolution Time:";
            // 
            // numTankSu
            // 
            numTankSu.Location = new Point(166, 72);
            numTankSu.Margin = new Padding(3, 2, 3, 2);
            numTankSu.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numTankSu.Name = "numTankSu";
            numTankSu.Size = new Size(131, 23);
            numTankSu.TabIndex = 20;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 76);
            label2.Name = "label2";
            label2.Size = new Size(122, 15);
            label2.TabIndex = 19;
            label2.Text = "Tank Water Receiving:";
            // 
            // txtKimyasal
            // 
            txtKimyasal.Location = new Point(166, 42);
            txtKimyasal.Margin = new Padding(3, 2, 3, 2);
            txtKimyasal.MaxLength = 6;
            txtKimyasal.Name = "txtKimyasal";
            txtKimyasal.Size = new Size(132, 23);
            txtKimyasal.TabIndex = 18;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 46);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 17;
            label1.Text = "Chemical:";
            // 
            // DozajEditor_Control
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
            Name = "DozajEditor_Control";
            Size = new Size(387, 341);
            ((System.ComponentModel.ISupportInitialize)numDozajLitre).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDozajSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCozmeSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTankSu).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label dozajbasliktext;
        private CheckBox chkAlarm;
        private CheckBox chkTamburDur;
        private CheckBox chkTank1Dozaj;
        private CheckBox chkTank1Su;
        private CheckBox chkAnaTankTemizSu;
        private CheckBox chkAnaTankMakSu;
        private NumericUpDown numDozajLitre;
        private Label label5;
        private NumericUpDown numDozajSure;
        private Label label4;
        private NumericUpDown numCozmeSure;
        private Label label3;
        private NumericUpDown numTankSu;
        private Label label2;
        private TextBox txtKimyasal;
        private Label label1;
    }
}
