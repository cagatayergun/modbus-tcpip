namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class BosaltmaEditor_Control
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
            numBeklemeZamani = new NumericUpDown();
            label3 = new Label();
            numCalismaDevri = new NumericUpDown();
            chkTamburDur = new CheckBox();
            chkAlarm = new CheckBox();
            bosaltmabasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numSagSolSure).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBeklemeZamani).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 51);
            label1.Name = "label1";
            label1.Size = new Size(189, 15);
            label1.TabIndex = 0;
            label1.Text = "Right-Left Direction Duration (SN):";
            // 
            // numSagSolSure
            // 
            numSagSolSure.Location = new Point(196, 47);
            numSagSolSure.Margin = new Padding(3, 2, 3, 2);
            numSagSolSure.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSagSolSure.Name = "numSagSolSure";
            numSagSolSure.Size = new Size(131, 23);
            numSagSolSure.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(5, 81);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 2;
            label2.Text = "Waiting Time (SN):";
            // 
            // numBeklemeZamani
            // 
            numBeklemeZamani.Location = new Point(196, 77);
            numBeklemeZamani.Margin = new Padding(3, 2, 3, 2);
            numBeklemeZamani.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numBeklemeZamani.Name = "numBeklemeZamani";
            numBeklemeZamani.Size = new Size(131, 23);
            numBeklemeZamani.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(5, 111);
            label3.Name = "label3";
            label3.Size = new Size(83, 15);
            label3.TabIndex = 4;
            label3.Text = "Working RPM:";
            // 
            // numCalismaDevri
            // 
            numCalismaDevri.Location = new Point(196, 107);
            numCalismaDevri.Margin = new Padding(3, 2, 3, 2);
            numCalismaDevri.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numCalismaDevri.Name = "numCalismaDevri";
            numCalismaDevri.Size = new Size(131, 23);
            numCalismaDevri.TabIndex = 5;
            // 
            // chkTamburDur
            // 
            chkTamburDur.AutoSize = true;
            chkTamburDur.Location = new Point(10, 145);
            chkTamburDur.Margin = new Padding(3, 2, 3, 2);
            chkTamburDur.Name = "chkTamburDur";
            chkTamburDur.Size = new Size(83, 19);
            chkTamburDur.TabIndex = 6;
            chkTamburDur.Text = "Drum Stop";
            chkTamburDur.UseVisualStyleBackColor = true;
            // 
            // chkAlarm
            // 
            chkAlarm.AutoSize = true;
            chkAlarm.Location = new Point(10, 168);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 7;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // bosaltmabasliktext
            // 
            bosaltmabasliktext.AutoSize = true;
            bosaltmabasliktext.Location = new Point(118, 11);
            bosaltmabasliktext.Name = "bosaltmabasliktext";
            bosaltmabasliktext.Size = new Size(75, 15);
            bosaltmabasliktext.TabIndex = 8;
            bosaltmabasliktext.Text = "UNLOADING";
            // 
            // BosaltmaEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.None;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(bosaltmabasliktext);
            Controls.Add(chkAlarm);
            Controls.Add(chkTamburDur);
            Controls.Add(numCalismaDevri);
            Controls.Add(label3);
            Controls.Add(numBeklemeZamani);
            Controls.Add(label2);
            Controls.Add(numSagSolSure);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "BosaltmaEditor_Control";
            Size = new Size(339, 221);
            ((System.ComponentModel.ISupportInitialize)numSagSolSure).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBeklemeZamani).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCalismaDevri).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSagSolSure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numBeklemeZamani;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCalismaDevri;
        private System.Windows.Forms.CheckBox chkTamburDur;
        private System.Windows.Forms.CheckBox chkAlarm;
        private Label bosaltmabasliktext;
    }
}