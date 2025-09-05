// UI/Views/Ayarlar_Control.Designer.cs
namespace TekstilScada.UI.Views
{
    partial class Ayarlar_Control
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
            tabControlSettings = new TabControl();
            tabPageMachineSettings = new TabPage();
            tabPageUserSettings = new TabPage();
            tabPageAlarmSettings = new TabPage();
            tabPageCostSettings = new TabPage();
            tabPagePlcOperators = new TabPage();
            tabPageRecipeDesigner = new TabPage();
            tabControlSettings.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlSettings
            // 
            tabControlSettings.Controls.Add(tabPageMachineSettings);
            tabControlSettings.Controls.Add(tabPageUserSettings);
            tabControlSettings.Controls.Add(tabPageAlarmSettings);
            tabControlSettings.Controls.Add(tabPageCostSettings);
            tabControlSettings.Controls.Add(tabPagePlcOperators);
            tabControlSettings.Controls.Add(tabPageRecipeDesigner);
            tabControlSettings.Dock = DockStyle.Fill;
            tabControlSettings.Location = new Point(0, 0);
            tabControlSettings.Margin = new Padding(3, 2, 3, 2);
            tabControlSettings.Name = "tabControlSettings";
            tabControlSettings.SelectedIndex = 0;
            tabControlSettings.Size = new Size(700, 450);
            tabControlSettings.TabIndex = 0;
            // 
            // tabPageMachineSettings
            // 
            tabPageMachineSettings.Location = new Point(4, 24);
            tabPageMachineSettings.Margin = new Padding(3, 2, 3, 2);
            tabPageMachineSettings.Name = "tabPageMachineSettings";
            tabPageMachineSettings.Padding = new Padding(3, 2, 3, 2);
            tabPageMachineSettings.Size = new Size(692, 422);
            tabPageMachineSettings.TabIndex = 0;
            tabPageMachineSettings.Text = "Makine Yönetimi";
            tabPageMachineSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageUserSettings
            // 
            tabPageUserSettings.Location = new Point(4, 24);
            tabPageUserSettings.Margin = new Padding(3, 2, 3, 2);
            tabPageUserSettings.Name = "tabPageUserSettings";
            tabPageUserSettings.Padding = new Padding(3, 2, 3, 2);
            tabPageUserSettings.Size = new Size(692, 422);
            tabPageUserSettings.TabIndex = 1;
            tabPageUserSettings.Text = "Kullanıcı Yönetimi";
            tabPageUserSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageAlarmSettings
            // 
            tabPageAlarmSettings.Location = new Point(4, 24);
            tabPageAlarmSettings.Margin = new Padding(3, 2, 3, 2);
            tabPageAlarmSettings.Name = "tabPageAlarmSettings";
            tabPageAlarmSettings.Size = new Size(692, 422);
            tabPageAlarmSettings.TabIndex = 2;
            tabPageAlarmSettings.Text = "Alarm Tanımlama";
            tabPageAlarmSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageCostSettings
            // 
            tabPageCostSettings.Location = new Point(4, 24);
            tabPageCostSettings.Margin = new Padding(3, 2, 3, 2);
            tabPageCostSettings.Name = "tabPageCostSettings";
            tabPageCostSettings.Size = new Size(692, 422);
            tabPageCostSettings.TabIndex = 4;
            tabPageCostSettings.Text = "Maliyet Parametreleri";
            tabPageCostSettings.UseVisualStyleBackColor = true;
            // 
            // tabPagePlcOperators
            // 
            tabPagePlcOperators.Location = new Point(4, 24);
            tabPagePlcOperators.Margin = new Padding(3, 2, 3, 2);
            tabPagePlcOperators.Name = "tabPagePlcOperators";
            tabPagePlcOperators.Size = new Size(692, 422);
            tabPagePlcOperators.TabIndex = 3;
            tabPagePlcOperators.Text = "PLC Operatör Yönetimi";
            tabPagePlcOperators.UseVisualStyleBackColor = true;
            // 
            // tabPageRecipeDesigner
            // 
            tabPageRecipeDesigner.Location = new Point(4, 24);
            tabPageRecipeDesigner.Margin = new Padding(3, 2, 3, 2);
            tabPageRecipeDesigner.Name = "tabPageRecipeDesigner";
            tabPageRecipeDesigner.Padding = new Padding(3, 2, 3, 2);
            tabPageRecipeDesigner.Size = new Size(692, 422);
            tabPageRecipeDesigner.TabIndex = 5;
            tabPageRecipeDesigner.Text = "Reçete Adım Tasarımcısı";
            tabPageRecipeDesigner.UseVisualStyleBackColor = true;
            // 
            // Ayarlar_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControlSettings);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Ayarlar_Control";
            Size = new Size(700, 450);
            tabControlSettings.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageMachineSettings;
        private System.Windows.Forms.TabPage tabPageUserSettings;
        private System.Windows.Forms.TabPage tabPageAlarmSettings;
        private System.Windows.Forms.TabPage tabPagePlcOperators;
        private System.Windows.Forms.TabPage tabPageCostSettings; // YENİ
        private System.Windows.Forms.TabPage tabPageRecipeDesigner;// YENİ
    }
}
