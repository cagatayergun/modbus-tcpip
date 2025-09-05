// MainForm.Designer.cs
namespace TekstilScada
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlNavigation = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            btnAyarlar = new Button();
            btnRaporlar = new Button();
            btnProsesKontrol = new Button();
            btnProsesIzleme = new Button();
            btnGenelBakis = new Button();
            pnlContent = new Panel();
            menuStrip1 = new MenuStrip();
            dilToolStripMenuItem = new ToolStripMenuItem();
            türkçeToolStripMenuItem = new ToolStripMenuItem();
            englishToolStripMenuItem = new ToolStripMenuItem();
            oturumToolStripMenuItem = new ToolStripMenuItem();
            çıkışYapToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblStatusCurrentUser = new ToolStripStatusLabel();
            springLabel = new ToolStripStatusLabel();
            lblStatusLiveEvents = new ToolStripStatusLabel();
            pnlNavigation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlNavigation
            // 
            pnlNavigation.BackColor = Color.FromArgb(0, 0, 64);
            pnlNavigation.Controls.Add(pictureBox2);
            pnlNavigation.Controls.Add(pictureBox1);
            pnlNavigation.Controls.Add(btnAyarlar);
            pnlNavigation.Controls.Add(btnRaporlar);
            pnlNavigation.Controls.Add(btnProsesKontrol);
            pnlNavigation.Controls.Add(btnProsesIzleme);
            pnlNavigation.Controls.Add(btnGenelBakis);
            pnlNavigation.Dock = DockStyle.Left;
            pnlNavigation.ForeColor = Color.Navy;
            pnlNavigation.Location = new Point(0, 24);
            pnlNavigation.Margin = new Padding(3, 2, 3, 2);
            pnlNavigation.Name = "pnlNavigation";
            pnlNavigation.Size = new Size(175, 369);
            pnlNavigation.TabIndex = 0;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Bottom;
            pictureBox2.Image = Properties.Resource1.yilmak;
            pictureBox2.Location = new Point(0, 329);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(175, 40);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 6;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom;
            pictureBox1.Image = Properties.Resource1.yilmak2___Kopya;
            pictureBox1.Location = new Point(56, 274);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(63, 55);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // btnAyarlar
            // 
            btnAyarlar.Dock = DockStyle.Top;
            btnAyarlar.FlatAppearance.BorderSize = 0;
            btnAyarlar.FlatStyle = FlatStyle.Flat;
            btnAyarlar.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnAyarlar.ForeColor = Color.White;
            btnAyarlar.Location = new Point(0, 136);
            btnAyarlar.Margin = new Padding(3, 2, 3, 2);
            btnAyarlar.Name = "btnAyarlar";
            btnAyarlar.Padding = new Padding(9, 0, 0, 0);
            btnAyarlar.Size = new Size(175, 34);
            btnAyarlar.TabIndex = 3;
            btnAyarlar.Text = "Ayarlar";
            btnAyarlar.TextAlign = ContentAlignment.MiddleLeft;
            btnAyarlar.UseVisualStyleBackColor = true;
            btnAyarlar.Click += btnAyarlar_Click;
            // 
            // btnRaporlar
            // 
            btnRaporlar.Dock = DockStyle.Top;
            btnRaporlar.FlatAppearance.BorderSize = 0;
            btnRaporlar.FlatStyle = FlatStyle.Flat;
            btnRaporlar.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnRaporlar.ForeColor = Color.White;
            btnRaporlar.Location = new Point(0, 102);
            btnRaporlar.Margin = new Padding(3, 2, 3, 2);
            btnRaporlar.Name = "btnRaporlar";
            btnRaporlar.Padding = new Padding(9, 0, 0, 0);
            btnRaporlar.Size = new Size(175, 34);
            btnRaporlar.TabIndex = 2;
            btnRaporlar.Text = "Raporlar";
            btnRaporlar.TextAlign = ContentAlignment.MiddleLeft;
            btnRaporlar.UseVisualStyleBackColor = true;
            btnRaporlar.Click += btnRaporlar_Click;
            // 
            // btnProsesKontrol
            // 
            btnProsesKontrol.Dock = DockStyle.Top;
            btnProsesKontrol.FlatAppearance.BorderSize = 0;
            btnProsesKontrol.FlatStyle = FlatStyle.Flat;
            btnProsesKontrol.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnProsesKontrol.ForeColor = Color.White;
            btnProsesKontrol.Location = new Point(0, 68);
            btnProsesKontrol.Margin = new Padding(3, 2, 3, 2);
            btnProsesKontrol.Name = "btnProsesKontrol";
            btnProsesKontrol.Padding = new Padding(9, 0, 0, 0);
            btnProsesKontrol.Size = new Size(175, 34);
            btnProsesKontrol.TabIndex = 1;
            btnProsesKontrol.Text = "Proses Kontrol";
            btnProsesKontrol.TextAlign = ContentAlignment.MiddleLeft;
            btnProsesKontrol.UseVisualStyleBackColor = true;
            btnProsesKontrol.Click += btnProsesKontrol_Click;
            // 
            // btnProsesIzleme
            // 
            btnProsesIzleme.Dock = DockStyle.Top;
            btnProsesIzleme.FlatAppearance.BorderSize = 0;
            btnProsesIzleme.FlatStyle = FlatStyle.Flat;
            btnProsesIzleme.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnProsesIzleme.ForeColor = Color.White;
            btnProsesIzleme.Location = new Point(0, 34);
            btnProsesIzleme.Margin = new Padding(3, 2, 3, 2);
            btnProsesIzleme.Name = "btnProsesIzleme";
            btnProsesIzleme.Padding = new Padding(9, 0, 0, 0);
            btnProsesIzleme.Size = new Size(175, 34);
            btnProsesIzleme.TabIndex = 0;
            btnProsesIzleme.Text = "Proses İzleme";
            btnProsesIzleme.TextAlign = ContentAlignment.MiddleLeft;
            btnProsesIzleme.UseVisualStyleBackColor = true;
            btnProsesIzleme.Click += btnProsesIzleme_Click;
            // 
            // btnGenelBakis
            // 
            btnGenelBakis.Dock = DockStyle.Top;
            btnGenelBakis.FlatAppearance.BorderSize = 0;
            btnGenelBakis.FlatStyle = FlatStyle.Flat;
            btnGenelBakis.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnGenelBakis.ForeColor = Color.White;
            btnGenelBakis.Location = new Point(0, 0);
            btnGenelBakis.Margin = new Padding(3, 2, 3, 2);
            btnGenelBakis.Name = "btnGenelBakis";
            btnGenelBakis.Padding = new Padding(9, 0, 0, 0);
            btnGenelBakis.Size = new Size(175, 34);
            btnGenelBakis.TabIndex = 4;
            btnGenelBakis.Text = "Genel Bakış";
            btnGenelBakis.TextAlign = ContentAlignment.MiddleLeft;
            btnGenelBakis.UseVisualStyleBackColor = true;
            btnGenelBakis.Click += btnGenelBakis_Click;
            // 
            // pnlContent
            // 
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(175, 24);
            pnlContent.Margin = new Padding(3, 2, 3, 2);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(719, 369);
            pnlContent.TabIndex = 1;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { dilToolStripMenuItem, oturumToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(894, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // dilToolStripMenuItem
            // 
            dilToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { türkçeToolStripMenuItem, englishToolStripMenuItem });
            dilToolStripMenuItem.Name = "dilToolStripMenuItem";
            dilToolStripMenuItem.Size = new Size(33, 20);
            dilToolStripMenuItem.Text = "Dil";
            // 
            // türkçeToolStripMenuItem
            // 
            türkçeToolStripMenuItem.Name = "türkçeToolStripMenuItem";
            türkçeToolStripMenuItem.Size = new Size(112, 22);
            türkçeToolStripMenuItem.Text = "Türkçe";
            türkçeToolStripMenuItem.Click += türkçeToolStripMenuItem_Click;
            // 
            // englishToolStripMenuItem
            // 
            englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            englishToolStripMenuItem.Size = new Size(112, 22);
            englishToolStripMenuItem.Text = "English";
            englishToolStripMenuItem.Click += englishToolStripMenuItem_Click;
            // 
            // oturumToolStripMenuItem
            // 
            oturumToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { çıkışYapToolStripMenuItem });
            oturumToolStripMenuItem.Name = "oturumToolStripMenuItem";
            oturumToolStripMenuItem.Size = new Size(61, 20);
            oturumToolStripMenuItem.Text = "Oturum";
            // 
            // çıkışYapToolStripMenuItem
            // 
            çıkışYapToolStripMenuItem.Name = "çıkışYapToolStripMenuItem";
            çıkışYapToolStripMenuItem.Size = new Size(121, 22);
            çıkışYapToolStripMenuItem.Text = "Oturum";
            çıkışYapToolStripMenuItem.Click += çıkışYapToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatusCurrentUser, springLabel, lblStatusLiveEvents });
            statusStrip1.Location = new Point(0, 393);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 12, 0);
            statusStrip1.Size = new Size(894, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatusCurrentUser
            // 
            lblStatusCurrentUser.Name = "lblStatusCurrentUser";
            lblStatusCurrentUser.Size = new Size(76, 17);
            lblStatusCurrentUser.Text = "Giriş Yapan: -";
            // 
            // springLabel
            // 
            springLabel.Name = "springLabel";
            springLabel.Size = new Size(679, 17);
            springLabel.Spring = true;
            // 
            // lblStatusLiveEvents
            // 
            lblStatusLiveEvents.IsLink = true;
            lblStatusLiveEvents.Name = "lblStatusLiveEvents";
            lblStatusLiveEvents.Size = new Size(126, 17);
            lblStatusLiveEvents.Text = "Canlı Olay Akışı Göster";
            lblStatusLiveEvents.Click += lblStatusLiveEvents_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(894, 415);
            Controls.Add(pnlContent);
            Controls.Add(pnlNavigation);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 2, 3, 2);
            Name = "MainForm";
            Text = "Tekstil SCADA Sistemi";
            WindowState = FormWindowState.Maximized;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            pnlNavigation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlNavigation;
        private System.Windows.Forms.Button btnAyarlar;
        private System.Windows.Forms.Button btnRaporlar;
        private System.Windows.Forms.Button btnProsesKontrol;
        private System.Windows.Forms.Button btnProsesIzleme;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Button btnGenelBakis;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem türkçeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusCurrentUser;
        private System.Windows.Forms.ToolStripStatusLabel springLabel;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusLiveEvents;
        private System.Windows.Forms.ToolStripMenuItem oturumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem çıkışYapToolStripMenuItem;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}
