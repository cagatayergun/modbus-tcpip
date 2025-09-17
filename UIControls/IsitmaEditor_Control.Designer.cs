// UI/Controls/RecipeStepEditors/IsitmaEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class IsitmaEditor_Control
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
            numIsi = new NumericUpDown();
            label2 = new Label();
            numSure = new NumericUpDown();
            chkDirekBuhar = new CheckBox();
            chkDolayliBuhar = new CheckBox();
            chkTamburDur = new CheckBox();
            chkAlarm = new CheckBox();
            isitmabasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numIsi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSure).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 62);
            label1.Name = "label1";
            label1.Size = new Size(100, 15);
            label1.TabIndex = 0;
            label1.Text = "Temperature (°C):";
            // 
            // numIsi
            // 
            numIsi.DecimalPlaces = 1;
            numIsi.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numIsi.Location = new Point(158, 58);
            numIsi.Margin = new Padding(3, 2, 3, 2);
            numIsi.Maximum = new decimal(new int[] { 1500, 0, 0, 0 });
            numIsi.Name = "numIsi";
            numIsi.Size = new Size(131, 23);
            numIsi.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 92);
            label2.Name = "label2";
            label2.Size = new Size(88, 15);
            label2.TabIndex = 2;
            label2.Text = "Duration (min):";
            // 
            // numSure
            // 
            numSure.Location = new Point(158, 88);
            numSure.Margin = new Padding(3, 2, 3, 2);
            numSure.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSure.Name = "numSure";
            numSure.Size = new Size(131, 23);
            numSure.TabIndex = 3;
            // 
            // chkDirekBuhar
            // 
            chkDirekBuhar.AutoSize = true;
            chkDirekBuhar.Location = new Point(12, 126);
            chkDirekBuhar.Margin = new Padding(3, 2, 3, 2);
            chkDirekBuhar.Name = "chkDirekBuhar";
            chkDirekBuhar.Size = new Size(93, 19);
            chkDirekBuhar.TabIndex = 4;
            chkDirekBuhar.Text = "Direct Steam";
            chkDirekBuhar.UseVisualStyleBackColor = true;
            // 
            // chkDolayliBuhar
            // 
            chkDolayliBuhar.AutoSize = true;
            chkDolayliBuhar.Location = new Point(12, 149);
            chkDolayliBuhar.Margin = new Padding(3, 2, 3, 2);
            chkDolayliBuhar.Name = "chkDolayliBuhar";
            chkDolayliBuhar.Size = new Size(102, 19);
            chkDolayliBuhar.TabIndex = 5;
            chkDolayliBuhar.Text = "Indirect Steam";
            chkDolayliBuhar.UseVisualStyleBackColor = true;
            // 
            // chkTamburDur
            // 
            chkTamburDur.AutoSize = true;
            chkTamburDur.Location = new Point(12, 171);
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
            chkAlarm.Location = new Point(12, 193);
            chkAlarm.Margin = new Padding(3, 2, 3, 2);
            chkAlarm.Name = "chkAlarm";
            chkAlarm.Size = new Size(58, 19);
            chkAlarm.TabIndex = 7;
            chkAlarm.Text = "Alarm";
            chkAlarm.UseVisualStyleBackColor = true;
            // 
            // isitmabasliktext
            // 
            isitmabasliktext.AutoSize = true;
            isitmabasliktext.Location = new Point(129, 11);
            isitmabasliktext.Name = "isitmabasliktext";
            isitmabasliktext.Size = new Size(55, 15);
            isitmabasliktext.TabIndex = 9;
            isitmabasliktext.Text = "HEATING";
            // 
            // IsitmaEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(isitmabasliktext);
            Controls.Add(chkAlarm);
            Controls.Add(chkTamburDur);
            Controls.Add(chkDolayliBuhar);
            Controls.Add(chkDirekBuhar);
            Controls.Add(numSure);
            Controls.Add(label2);
            Controls.Add(numIsi);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "IsitmaEditor_Control";
            Size = new Size(302, 221);
            ((System.ComponentModel.ISupportInitialize)numIsi).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSure).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numIsi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSure;
        private System.Windows.Forms.CheckBox chkDirekBuhar;
        private System.Windows.Forms.CheckBox chkDolayliBuhar;
        private System.Windows.Forms.CheckBox chkTamburDur;
        private System.Windows.Forms.CheckBox chkAlarm;
        private Label isitmabasliktext;
    }
}
