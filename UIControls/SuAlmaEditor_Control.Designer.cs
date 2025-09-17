// UI/Controls/RecipeStepEditors/SuAlmaEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class SuAlmaEditor_Control
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
            numLitre = new NumericUpDown();
            chkSicakSu = new CheckBox();
            chkSogukSu = new CheckBox();
            chkYumusakSu = new CheckBox();
            chkTamburDur = new CheckBox();
            chkAlarm = new CheckBox();
            sualmabasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numLitre).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 47);
            label1.Name = "label1";
            label1.Size = new Size(95, 15);
            label1.TabIndex = 0;
            label1.Text = "Quantity (Liters):";
            // 
            // numLitre
            // 
            numLitre.Location = new Point(158, 43);
            numLitre.Margin = new Padding(3, 2, 3, 2);
            numLitre.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numLitre.Name = "numLitre";
            numLitre.Size = new Size(131, 23);
            numLitre.TabIndex = 1;
            // 
            // chkSicakSu
            // 
            chkSicakSu.AutoSize = true;
            chkSicakSu.Location = new Point(12, 81);
            chkSicakSu.Margin = new Padding(3, 2, 3, 2);
            chkSicakSu.Name = "chkSicakSu";
            chkSicakSu.Size = new Size(80, 19);
            chkSicakSu.TabIndex = 2;
            chkSicakSu.Text = "Hot Water";
            chkSicakSu.UseVisualStyleBackColor = true;
            // 
            // chkSogukSu
            // 
            chkSogukSu.AutoSize = true;
            chkSogukSu.Location = new Point(12, 105);
            chkSogukSu.Margin = new Padding(3, 2, 3, 2);
            chkSogukSu.Name = "chkSogukSu";
            chkSogukSu.Size = new Size(85, 19);
            chkSogukSu.TabIndex = 3;
            chkSogukSu.Text = "Cold Water";
            chkSogukSu.UseVisualStyleBackColor = true;
            // 
            // chkYumusakSu
            // 
            chkYumusakSu.AutoSize = true;
            chkYumusakSu.Location = new Point(12, 129);
            chkYumusakSu.Margin = new Padding(3, 2, 3, 2);
            chkYumusakSu.Name = "chkYumusakSu";
            chkYumusakSu.Size = new Size(81, 19);
            chkYumusakSu.TabIndex = 4;
            chkYumusakSu.Text = "Soft Water";
            chkYumusakSu.UseVisualStyleBackColor = true;
            // 
            // chkTamburDur
            // 
            chkTamburDur.AutoSize = true;
            chkTamburDur.Location = new Point(12, 153);
            chkTamburDur.Margin = new Padding(3, 2, 3, 2);
            chkTamburDur.Name = "chkTamburDur";
            chkTamburDur.Size = new Size(83, 19);
            chkTamburDur.TabIndex = 5;
            chkTamburDur.Text = "Drum Stop";
            chkTamburDur.UseVisualStyleBackColor = true;
            // 
            // chkAlarm
            // 
            chkAlarm.AutoSize = true;
            chkAlarm.Location = new Point(12, 177);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 6;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // sualmabasliktext
            // 
            sualmabasliktext.AutoSize = true;
            sualmabasliktext.Location = new Point(120, 11);
            sualmabasliktext.Name = "sualmabasliktext";
            sualmabasliktext.Size = new Size(73, 15);
            sualmabasliktext.TabIndex = 9;
            sualmabasliktext.Text = "TAKE WATER";
            // 
            // SuAlmaEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(sualmabasliktext);
            Controls.Add(chkAlarm);
            Controls.Add(chkTamburDur);
            Controls.Add(chkYumusakSu);
            Controls.Add(chkSogukSu);
            Controls.Add(chkSicakSu);
            Controls.Add(numLitre);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SuAlmaEditor_Control";
            Size = new Size(302, 205);
            ((System.ComponentModel.ISupportInitialize)numLitre).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numLitre;
        private System.Windows.Forms.CheckBox chkSicakSu;
        private System.Windows.Forms.CheckBox chkSogukSu;
        private System.Windows.Forms.CheckBox chkYumusakSu;
        private System.Windows.Forms.CheckBox chkTamburDur;
        private System.Windows.Forms.CheckBox chkAlarm;
        private Label sualmabasliktext;
    }
}
