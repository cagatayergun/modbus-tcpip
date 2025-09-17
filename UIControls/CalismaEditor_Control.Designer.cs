// UI/Controls/RecipeStepEditors/CalismaEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class CalismaEditor_Control
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            numSagSolSure = new NumericUpDown();
            label2 = new Label();
            numBeklemeSuresi = new NumericUpDown();
            label3 = new Label();
            numCalismaDevri = new NumericUpDown();
            label4 = new Label();
            numCalismaSuresi = new NumericUpDown();
            chkIsiKontrol = new CheckBox();
            chkAlarm = new CheckBox();
            calismabasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numSagSolSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBeklemeSuresi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaSuresi).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 57);
            label1.Name = "label1";
            label1.Size = new Size(189, 15);
            label1.TabIndex = 0;
            label1.Text = "Right-Left Direction Duration (SN):";
            // 
            // numSagSolSure
            // 
            numSagSolSure.Location = new Point(206, 53);
            numSagSolSure.Margin = new Padding(3, 2, 3, 2);
            numSagSolSure.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSagSolSure.Name = "numSagSolSure";
            numSagSolSure.Size = new Size(131, 23);
            numSagSolSure.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 83);
            label2.Name = "label2";
            label2.Size = new Size(111, 15);
            label2.TabIndex = 2;
            label2.Text = "Waiting Time (SEC):";
            // 
            // numBeklemeSuresi
            // 
            numBeklemeSuresi.Location = new Point(206, 79);
            numBeklemeSuresi.Margin = new Padding(3, 2, 3, 2);
            numBeklemeSuresi.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numBeklemeSuresi.Name = "numBeklemeSuresi";
            numBeklemeSuresi.Size = new Size(131, 23);
            numBeklemeSuresi.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 109);
            label3.Name = "label3";
            label3.Size = new Size(83, 15);
            label3.TabIndex = 4;
            label3.Text = "Working RPM:";
            // 
            // numCalismaDevri
            // 
            numCalismaDevri.Location = new Point(206, 105);
            numCalismaDevri.Margin = new Padding(3, 2, 3, 2);
            numCalismaDevri.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numCalismaDevri.Name = "numCalismaDevri";
            numCalismaDevri.Size = new Size(131, 23);
            numCalismaDevri.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 135);
            label4.Name = "label4";
            label4.Size = new Size(118, 15);
            label4.TabIndex = 6;
            label4.Text = "Working Time (MIN):";
            // 
            // numCalismaSuresi
            // 
            numCalismaSuresi.Location = new Point(206, 131);
            numCalismaSuresi.Margin = new Padding(3, 2, 3, 2);
            numCalismaSuresi.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numCalismaSuresi.Name = "numCalismaSuresi";
            numCalismaSuresi.Size = new Size(131, 23);
            numCalismaSuresi.TabIndex = 7;
            // 
            // chkIsiKontrol
            // 
            chkIsiKontrol.AutoSize = true;
            chkIsiKontrol.Location = new Point(12, 161);
            chkIsiKontrol.Margin = new Padding(3, 2, 3, 2);
            chkIsiKontrol.Name = "chkIsiKontrol";
            chkIsiKontrol.Size = new Size(94, 19);
            chkIsiKontrol.TabIndex = 8;
            chkIsiKontrol.Text = "Heat Control";
            chkIsiKontrol.UseVisualStyleBackColor = true;
            // 
            // chkAlarm
            // 
            chkAlarm.AutoSize = true;
            chkAlarm.Location = new Point(12, 182);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 9;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // calismabasliktext
            // 
            calismabasliktext.AutoSize = true;
            calismabasliktext.Location = new Point(123, 11);
            calismabasliktext.Name = "calismabasliktext";
            calismabasliktext.Size = new Size(61, 15);
            calismabasliktext.TabIndex = 10;
            calismabasliktext.Text = "WORKING";
            // 
            // CalismaEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(calismabasliktext);
            Controls.Add(chkAlarm);
            Controls.Add(chkIsiKontrol);
            Controls.Add(numCalismaSuresi);
            Controls.Add(label4);
            Controls.Add(numCalismaDevri);
            Controls.Add(label3);
            Controls.Add(numBeklemeSuresi);
            Controls.Add(label2);
            Controls.Add(numSagSolSure);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "CalismaEditor_Control";
            Size = new Size(355, 221);
            ((System.ComponentModel.ISupportInitialize)numSagSolSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBeklemeSuresi).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaSuresi).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSagSolSure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numBeklemeSuresi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCalismaDevri;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numCalismaSuresi;
        private System.Windows.Forms.CheckBox chkIsiKontrol;
        private System.Windows.Forms.CheckBox chkAlarm;
        private Label calismabasliktext;
    }
}
