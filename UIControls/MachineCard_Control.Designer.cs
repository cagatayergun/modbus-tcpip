// UI/Controls/MachineCard_Control.Designer.cs
namespace TekstilScada.UI.Controls
{
    partial class MachineCard_Control
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
            pnlMain = new Panel();
            lblMachineIdValue = new Label();
            lblMachineNameValue = new Label();
            lblStepValue = new Label();
            lblOperatorValue = new Label();
            lblRecipeNameValue = new Label();
            lblMachineIdTitle = new Label();
            lblMachineNameTitle = new Label();
            lblStepTitle = new Label();
            lblOperatorTitle = new Label();
            lblRecipeNameTitle = new Label();
            lblPercentage = new Label();
            progressBar = new ProgressBar();
            lblProcessing = new Label();
            pnlIcons = new Panel();
            picConnection = new PictureBox();
            btnInfo = new Button();
            picAlarm = new PictureBox();
            picPause = new PictureBox();
            picPlay = new PictureBox();
            btnVnc = new Button();
            lblMachineNumber = new Label();
            pnlMain.SuspendLayout();
            pnlIcons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picConnection).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picAlarm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPause).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPlay).BeginInit();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(223, 230, 233);
            pnlMain.BorderStyle = BorderStyle.FixedSingle;
            pnlMain.Controls.Add(lblMachineIdValue);
            pnlMain.Controls.Add(lblMachineNameValue);
            pnlMain.Controls.Add(lblStepValue);
            pnlMain.Controls.Add(lblOperatorValue);
            pnlMain.Controls.Add(lblRecipeNameValue);
            pnlMain.Controls.Add(lblMachineIdTitle);
            pnlMain.Controls.Add(lblMachineNameTitle);
            pnlMain.Controls.Add(lblStepTitle);
            pnlMain.Controls.Add(lblOperatorTitle);
            pnlMain.Controls.Add(lblRecipeNameTitle);
            pnlMain.Controls.Add(lblPercentage);
            pnlMain.Controls.Add(progressBar);
            pnlMain.Controls.Add(lblProcessing);
            pnlMain.Controls.Add(pnlIcons);
            pnlMain.Controls.Add(lblMachineNumber);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Margin = new Padding(3, 2, 3, 2);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(280, 180);
            pnlMain.TabIndex = 0;
            // 
            // lblMachineIdValue
            // 
            lblMachineIdValue.BackColor = Color.White;
            lblMachineIdValue.BorderStyle = BorderStyle.FixedSingle;
            lblMachineIdValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblMachineIdValue.Location = new Point(105, 150);
            lblMachineIdValue.Name = "lblMachineIdValue";
            lblMachineIdValue.Size = new Size(158, 19);
            lblMachineIdValue.TabIndex = 14;
            lblMachineIdValue.Text = "---";
            lblMachineIdValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblMachineNameValue
            // 
            lblMachineNameValue.BackColor = Color.White;
            lblMachineNameValue.BorderStyle = BorderStyle.FixedSingle;
            lblMachineNameValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblMachineNameValue.Location = new Point(105, 128);
            lblMachineNameValue.Name = "lblMachineNameValue";
            lblMachineNameValue.Size = new Size(158, 19);
            lblMachineNameValue.TabIndex = 13;
            lblMachineNameValue.Text = "---";
            lblMachineNameValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStepValue
            // 
            lblStepValue.BackColor = Color.White;
            lblStepValue.BorderStyle = BorderStyle.FixedSingle;
            lblStepValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblStepValue.Location = new Point(105, 105);
            lblStepValue.Name = "lblStepValue";
            lblStepValue.Size = new Size(158, 19);
            lblStepValue.TabIndex = 12;
            lblStepValue.Text = "---";
            lblStepValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblOperatorValue
            // 
            lblOperatorValue.BackColor = Color.White;
            lblOperatorValue.BorderStyle = BorderStyle.FixedSingle;
            lblOperatorValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblOperatorValue.Location = new Point(105, 82);
            lblOperatorValue.Name = "lblOperatorValue";
            lblOperatorValue.Size = new Size(158, 19);
            lblOperatorValue.TabIndex = 11;
            lblOperatorValue.Text = "---";
            lblOperatorValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRecipeNameValue
            // 
            lblRecipeNameValue.BackColor = Color.White;
            lblRecipeNameValue.BorderStyle = BorderStyle.FixedSingle;
            lblRecipeNameValue.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblRecipeNameValue.Location = new Point(105, 60);
            lblRecipeNameValue.Name = "lblRecipeNameValue";
            lblRecipeNameValue.Size = new Size(158, 19);
            lblRecipeNameValue.TabIndex = 10;
            lblRecipeNameValue.Text = "---";
            lblRecipeNameValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblMachineIdTitle
            // 
            lblMachineIdTitle.AutoSize = true;
            lblMachineIdTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMachineIdTitle.Location = new Point(13, 152);
            lblMachineIdTitle.Name = "lblMachineIdTitle";
            lblMachineIdTitle.Size = new Size(80, 15);
            lblMachineIdTitle.TabIndex = 9;
            lblMachineIdTitle.Text = "MACHINE ID:";
            // 
            // lblMachineNameTitle
            // 
            lblMachineNameTitle.AutoSize = true;
            lblMachineNameTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMachineNameTitle.Location = new Point(13, 129);
            lblMachineNameTitle.Name = "lblMachineNameTitle";
            lblMachineNameTitle.Size = new Size(88, 15);
            lblMachineNameTitle.TabIndex = 8;
            lblMachineNameTitle.Text = "MACHINE ADI:";
            // 
            // lblStepTitle
            // 
            lblStepTitle.AutoSize = true;
            lblStepTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStepTitle.Location = new Point(13, 106);
            lblStepTitle.Name = "lblStepTitle";
            lblStepTitle.Size = new Size(37, 15);
            lblStepTitle.TabIndex = 7;
            lblStepTitle.Text = "STEP:";
            // 
            // lblOperatorTitle
            // 
            lblOperatorTitle.AutoSize = true;
            lblOperatorTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblOperatorTitle.Location = new Point(13, 84);
            lblOperatorTitle.Name = "lblOperatorTitle";
            lblOperatorTitle.Size = new Size(71, 15);
            lblOperatorTitle.TabIndex = 6;
            lblOperatorTitle.Text = "OPERATOR:";
            // 
            // lblRecipeNameTitle
            // 
            lblRecipeNameTitle.AutoSize = true;
            lblRecipeNameTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRecipeNameTitle.Location = new Point(13, 62);
            lblRecipeNameTitle.Name = "lblRecipeNameTitle";
            lblRecipeNameTitle.Size = new Size(85, 15);
            lblRecipeNameTitle.TabIndex = 5;
            lblRecipeNameTitle.Text = "RECIPE NAME:";
            // 
            // lblPercentage
            // 
            lblPercentage.AutoSize = true;
            lblPercentage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPercentage.Location = new Point(228, 38);
            lblPercentage.Name = "lblPercentage";
            lblPercentage.Size = new Size(27, 15);
            lblPercentage.TabIndex = 3;
            lblPercentage.Text = "0 %";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(105, 39);
            progressBar.Margin = new Padding(3, 2, 3, 2);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(114, 11);
            progressBar.TabIndex = 2;
            // 
            // lblProcessing
            // 
            lblProcessing.AutoSize = true;
            lblProcessing.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblProcessing.Location = new Point(13, 38);
            lblProcessing.Name = "lblProcessing";
            lblProcessing.Size = new Size(80, 15);
            lblProcessing.TabIndex = 1;
            lblProcessing.Text = "PROSESSING";
            // 
            // pnlIcons
            // 
            pnlIcons.Controls.Add(picConnection);
            pnlIcons.Controls.Add(btnInfo);
            pnlIcons.Controls.Add(picAlarm);
            pnlIcons.Controls.Add(picPause);
            pnlIcons.Controls.Add(picPlay);
            pnlIcons.Controls.Add(btnVnc);
            pnlIcons.Location = new Point(44, 4);
            pnlIcons.Margin = new Padding(3, 2, 3, 2);
            pnlIcons.Name = "pnlIcons";
            pnlIcons.Size = new Size(228, 30);
            pnlIcons.TabIndex = 15;
            // 
            // picConnection
            // 
            picConnection.BackColor = Color.Transparent;
            picConnection.Image = Properties.Resource1.yilmak_baglanti_2;
            picConnection.InitialImage = Properties.Resource1.yilmak_baglanti;
            picConnection.Location = new Point(149, 4);
            picConnection.Margin = new Padding(3, 2, 3, 2);
            picConnection.Name = "picConnection";
            picConnection.Size = new Size(26, 22);
            picConnection.SizeMode = PictureBoxSizeMode.StretchImage;
            picConnection.TabIndex = 6;
            picConnection.TabStop = false;
            // 
            // btnInfo
            // 
            btnInfo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnInfo.Location = new Point(192, 4);
            btnInfo.Margin = new Padding(3, 2, 3, 2);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(26, 22);
            btnInfo.TabIndex = 5;
            btnInfo.Text = "i";
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // picAlarm
            // 
            picAlarm.BackColor = Color.Transparent;
            picAlarm.Image = Properties.Resource1.alarm;
            picAlarm.Location = new Point(114, 4);
            picAlarm.Margin = new Padding(3, 2, 3, 2);
            picAlarm.Name = "picAlarm";
            picAlarm.Size = new Size(26, 22);
            picAlarm.SizeMode = PictureBoxSizeMode.StretchImage;
            picAlarm.TabIndex = 3;
            picAlarm.TabStop = false;
            // 
            // picPause
            // 
            picPause.BackColor = Color.Transparent;
            picPause.Image = Properties.Resource1.pause;
            picPause.Location = new Point(79, 4);
            picPause.Margin = new Padding(3, 2, 3, 2);
            picPause.Name = "picPause";
            picPause.Size = new Size(26, 22);
            picPause.SizeMode = PictureBoxSizeMode.StretchImage;
            picPause.TabIndex = 2;
            picPause.TabStop = false;
            // 
            // picPlay
            // 
            picPlay.BackColor = Color.Transparent;
            picPlay.Image = Properties.Resource1.play;
            picPlay.Location = new Point(44, 4);
            picPlay.Margin = new Padding(3, 2, 3, 2);
            picPlay.Name = "picPlay";
            picPlay.Size = new Size(26, 22);
            picPlay.SizeMode = PictureBoxSizeMode.StretchImage;
            picPlay.TabIndex = 1;
            picPlay.TabStop = false;
            // 
            // btnVnc
            // 
            btnVnc.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnVnc.Location = new Point(9, 4);
            btnVnc.Margin = new Padding(3, 2, 3, 2);
            btnVnc.Name = "btnVnc";
            btnVnc.Size = new Size(26, 22);
            btnVnc.TabIndex = 0;
            btnVnc.Text = "V";
            btnVnc.UseVisualStyleBackColor = true;
            btnVnc.Click += btnVnc_Click;
            // 
            // lblMachineNumber
            // 
            lblMachineNumber.AutoSize = true;
            lblMachineNumber.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold);
            lblMachineNumber.Location = new Point(9, 4);
            lblMachineNumber.Name = "lblMachineNumber";
            lblMachineNumber.Size = new Size(32, 30);
            lblMachineNumber.TabIndex = 0;
            lblMachineNumber.Text = "1.";
            // 
            // MachineCard_Control
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
            Margin = new Padding(9, 8, 9, 8);
            Name = "MachineCard_Control";
            Size = new Size(280, 180);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlIcons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picConnection).EndInit();
            ((System.ComponentModel.ISupportInitialize)picAlarm).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPause).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPlay).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblMachineNumber;
        private System.Windows.Forms.Panel pnlIcons;
        private System.Windows.Forms.Button btnVnc;
        private System.Windows.Forms.PictureBox picPlay;
        private System.Windows.Forms.PictureBox picPause;
        private System.Windows.Forms.PictureBox picAlarm;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.Label lblRecipeNameTitle;
        private System.Windows.Forms.Label lblOperatorTitle;
        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.Label lblMachineNameTitle;
        private System.Windows.Forms.Label lblMachineIdTitle;
        private System.Windows.Forms.Label lblRecipeNameValue;
        private System.Windows.Forms.Label lblOperatorValue;
        private System.Windows.Forms.Label lblStepValue;
        private System.Windows.Forms.Label lblMachineNameValue;
        private System.Windows.Forms.Label lblMachineIdValue;
        private System.Windows.Forms.PictureBox picConnection;
    }
}