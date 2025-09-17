// UI/Controls/RecipeStepEditors/SikmaEditor_Control.Designer.cs
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class SikmaEditor_Control
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
            numSikmaDevri = new NumericUpDown();
            label2 = new Label();
            numSikmaSure = new NumericUpDown();
            sikmabasliktext = new Label();
            ((System.ComponentModel.ISupportInitialize)numSikmaDevri).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSikmaSure).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 87);
            label1.Name = "label1";
            label1.Size = new Size(99, 15);
            label1.TabIndex = 0;
            label1.Text = "Squeezing Speed:";
            // 
            // numSikmaDevri
            // 
            numSikmaDevri.Location = new Point(158, 83);
            numSikmaDevri.Margin = new Padding(3, 2, 3, 2);
            numSikmaDevri.Maximum = new decimal(new int[] { 1200, 0, 0, 0 });
            numSikmaDevri.Name = "numSikmaDevri";
            numSikmaDevri.Size = new Size(131, 23);
            numSikmaDevri.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 117);
            label2.Name = "label2";
            label2.Size = new Size(127, 15);
            label2.TabIndex = 2;
            label2.Text = "Squeezing Time (MIN):";
            // 
            // numSikmaSure
            // 
            numSikmaSure.Location = new Point(158, 113);
            numSikmaSure.Margin = new Padding(3, 2, 3, 2);
            numSikmaSure.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSikmaSure.Name = "numSikmaSure";
            numSikmaSure.Size = new Size(131, 23);
            numSikmaSure.TabIndex = 3;
            // 
            // sikmabasliktext
            // 
            sikmabasliktext.AutoSize = true;
            sikmabasliktext.Location = new Point(132, 11);
            sikmabasliktext.Name = "sikmabasliktext";
            sikmabasliktext.Size = new Size(69, 15);
            sikmabasliktext.TabIndex = 9;
            sikmabasliktext.Text = "SQUEEZING";
            // 
            // SikmaEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(sikmabasliktext);
            Controls.Add(numSikmaSure);
            Controls.Add(label2);
            Controls.Add(numSikmaDevri);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SikmaEditor_Control";
            Size = new Size(302, 221);
            ((System.ComponentModel.ISupportInitialize)numSikmaDevri).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSikmaSure).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSikmaDevri;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSikmaSure;
        private Label sikmabasliktext;
    }
}