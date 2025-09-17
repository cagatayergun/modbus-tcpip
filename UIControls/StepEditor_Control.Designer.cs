// StepEditor_Control.Designer.cs
// Bu kod bloğunu StepEditor_Control.Designer.cs dosyasına yapıştırın.
namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    partial class StepEditor_Control
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
            pnlStepTypes = new Panel();
            chkSikma = new CheckBox();
            chkBosaltma = new CheckBox();
            chkDozaj = new CheckBox();
            chkCalisma = new CheckBox();
            chkIsitma = new CheckBox();
            chkSuAlma = new CheckBox();
            flpParameters = new FlowLayoutPanel();
            pnlStepTypes.SuspendLayout();
            SuspendLayout();
            // 
            // pnlStepTypes
            // 
            pnlStepTypes.AutoScroll = true;
            pnlStepTypes.BorderStyle = BorderStyle.FixedSingle;
            pnlStepTypes.Controls.Add(chkSikma);
            pnlStepTypes.Controls.Add(chkBosaltma);
            pnlStepTypes.Controls.Add(chkDozaj);
            pnlStepTypes.Controls.Add(chkCalisma);
            pnlStepTypes.Controls.Add(chkIsitma);
            pnlStepTypes.Controls.Add(chkSuAlma);
            pnlStepTypes.Dock = DockStyle.Left;
            pnlStepTypes.Location = new Point(0, 0);
            pnlStepTypes.Margin = new Padding(3, 2, 3, 2);
            pnlStepTypes.Name = "pnlStepTypes";
            pnlStepTypes.Size = new Size(206, 338);
            pnlStepTypes.TabIndex = 0;
            // 
            // chkSikma
            // 
            chkSikma.AutoSize = true;
            chkSikma.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkSikma.Location = new Point(9, 120);
            chkSikma.Margin = new Padding(3, 2, 3, 2);
            chkSikma.Name = "chkSikma";
            chkSikma.Size = new Size(92, 19);
            chkSikma.TabIndex = 5;
            chkSikma.Text = "SQUEEZING";
            chkSikma.UseVisualStyleBackColor = true;
            // 
            // chkBosaltma
            // 
            chkBosaltma.AutoSize = true;
            chkBosaltma.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkBosaltma.Location = new Point(9, 98);
            chkBosaltma.Margin = new Padding(3, 2, 3, 2);
            chkBosaltma.Name = "chkBosaltma";
            chkBosaltma.Size = new Size(98, 19);
            chkBosaltma.TabIndex = 4;
            chkBosaltma.Text = "UNLOADING";
            chkBosaltma.UseVisualStyleBackColor = true;
            // 
            // chkDozaj
            // 
            chkDozaj.AutoSize = true;
            chkDozaj.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkDozaj.Location = new Point(9, 75);
            chkDozaj.Margin = new Padding(3, 2, 3, 2);
            chkDozaj.Name = "chkDozaj";
            chkDozaj.Size = new Size(74, 19);
            chkDozaj.TabIndex = 3;
            chkDozaj.Text = "DOSAGE";
            chkDozaj.UseVisualStyleBackColor = true;
            // 
            // chkCalisma
            // 
            chkCalisma.AutoSize = true;
            chkCalisma.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkCalisma.Location = new Point(9, 52);
            chkCalisma.Margin = new Padding(3, 2, 3, 2);
            chkCalisma.Name = "chkCalisma";
            chkCalisma.Size = new Size(85, 19);
            chkCalisma.TabIndex = 2;
            chkCalisma.Text = "WORKING";
            chkCalisma.UseVisualStyleBackColor = true;
            // 
            // chkIsitma
            // 
            chkIsitma.AutoSize = true;
            chkIsitma.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkIsitma.Location = new Point(9, 30);
            chkIsitma.Margin = new Padding(3, 2, 3, 2);
            chkIsitma.Name = "chkIsitma";
            chkIsitma.Size = new Size(77, 19);
            chkIsitma.TabIndex = 1;
            chkIsitma.Text = "HEATING";
            chkIsitma.UseVisualStyleBackColor = true;
            // 
            // chkSuAlma
            // 
            chkSuAlma.AutoSize = true;
            chkSuAlma.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkSuAlma.Location = new Point(9, 8);
            chkSuAlma.Margin = new Padding(3, 2, 3, 2);
            chkSuAlma.Name = "chkSuAlma";
            chkSuAlma.Size = new Size(97, 19);
            chkSuAlma.TabIndex = 0;
            chkSuAlma.Text = "TAKE WATER";
            chkSuAlma.UseVisualStyleBackColor = true;
            // 
            // flpParameters
            // 
            flpParameters.AutoScroll = true;
            flpParameters.Dock = DockStyle.Fill;
            flpParameters.FlowDirection = FlowDirection.TopDown;
            flpParameters.Location = new Point(206, 0);
            flpParameters.Margin = new Padding(3, 2, 3, 2);
            flpParameters.Name = "flpParameters";
            flpParameters.Size = new Size(144, 338);
            flpParameters.TabIndex = 0;
            flpParameters.WrapContents = false;
            // 
            // StepEditor_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flpParameters);
            Controls.Add(pnlStepTypes);
            Margin = new Padding(3, 2, 3, 2);
            Name = "StepEditor_Control";
            Size = new Size(350, 338);
            pnlStepTypes.ResumeLayout(false);
            pnlStepTypes.PerformLayout();
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.Panel pnlStepTypes;
        private System.Windows.Forms.CheckBox chkSuAlma;
        private System.Windows.Forms.CheckBox chkIsitma;
        private System.Windows.Forms.CheckBox chkCalisma;
        private System.Windows.Forms.CheckBox chkDozaj;
        private System.Windows.Forms.CheckBox chkBosaltma;
        private System.Windows.Forms.CheckBox chkSikma;
        private System.Windows.Forms.FlowLayoutPanel flpParameters;
    }
}
