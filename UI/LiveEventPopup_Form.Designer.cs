// UI/LiveEventPopup_Form.Designer.cs
namespace TekstilScada.UI
{
    partial class LiveEventPopup_Form
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lstEvents = new System.Windows.Forms.ListView();
            this.colTime = new System.Windows.Forms.ColumnHeader();
            this.colSource = new System.Windows.Forms.ColumnHeader();
            this.colMessage = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lstEvents
            // 
            this.lstEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTime,
            this.colSource,
            this.colMessage});
            this.lstEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEvents.FullRowSelect = true;
            this.lstEvents.HideSelection = false;
            this.lstEvents.Location = new System.Drawing.Point(0, 0);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(600, 300);
            this.lstEvents.TabIndex = 0;
            this.lstEvents.UseCompatibleStateImageBehavior = false;
            this.lstEvents.View = System.Windows.Forms.View.Details;
            // 
            // colTime
            // 
            this.colTime.Text = "Time";
            this.colTime.Width = 80;
            // 
            // colSource
            // 
            this.colSource.Text = "Source";
            this.colSource.Width = 120;
            // 
            // colMessage
            // 
            this.colMessage.Text = "Message";
            this.colMessage.Width = 380;
            // 
            // LiveEventPopup_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 300);
            this.Controls.Add(this.lstEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LiveEventPopup_Form";
            this.Text = "Live Event Stream";
            this.TopMost = true; // Her zaman üstte kal
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LiveEventPopup_Form_FormClosing);
            this.ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.ListView lstEvents;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colSource;
        private System.Windows.Forms.ColumnHeader colMessage;
    }
}
