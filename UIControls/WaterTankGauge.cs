// WaterTankGauge.cs
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// Namespace'i kendi projenize göre düzenleyebilirsiniz.
namespace TekstilScada.UI.Controls
{
    public partial class WaterTankGauge : UserControl
    {
        private int _value = 0;
        private int _maximum = 5000;
        private string _title = "AMOUNT OF WATER";
        private string _unit = "L";

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(0, Math.Min(_maximum, value));
                this.Invalidate(); // Değer değiştiğinde kontrolü yeniden çizdir.
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                this.Invalidate();
            }
        }

        public WaterTankGauge()
        {
            InitializeComponent();
            // Daha akıcı çizimler için DoubleBuffering'i etkinleştir.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int panelWidth = this.Width;
            int panelHeight = this.Height;
            int padding = 10;
            // Alttaki metinler için 40px boşluk bırak
            RectangleF tankBounds = new RectangleF(padding, padding, panelWidth - 2 * padding, panelHeight - 2 * padding - 40);

            // 1. Tankın dış çerçevesini çiz
            using (Pen tankOutlinePen = new Pen(Color.FromArgb(100, 100, 100), 3))
            {
                g.DrawEllipse(tankOutlinePen, tankBounds);
            }

            if (_value > 0)
            {
                // 2. Suyun yüksekliğini hesapla
                float doluYukseklik = tankBounds.Height * ((float)_value / _maximum);

                // 3. Su için bir GraphicsPath oluştur
                using (GraphicsPath tankPath = new GraphicsPath())
                {
                    tankPath.AddEllipse(tankBounds);

                    // 4. Suyu temsil eden dikdörtgeni oluştur
                    RectangleF suDikdortgeni = new RectangleF(
                        tankBounds.X,
                        tankBounds.Y + tankBounds.Height - doluYukseklik,
                        tankBounds.Width,
                        doluYukseklik
                    );

                    // 5. Elips ile su dikdörtgeninin kesişimini al
                    Region suBolgesi = new Region(suDikdortgeni);
                    suBolgesi.Intersect(tankPath);

                    // 6. Kesişim bölgesini mavi renkle doldur
                    using (SolidBrush suBrush = new SolidBrush(Color.FromArgb(52, 152, 219)))
                    {
                        g.FillRegion(suBrush, suBolgesi);
                    }
                    suBolgesi.Dispose();
                }
            }

            // 7. Metinleri çiz
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                // Değer Metni (Örn: "1234 L")
                RectangleF valueRect = new RectangleF(0, tankBounds.Bottom, panelWidth, 25);
                using (Font valueFont = new Font("Segoe UI", 12F, FontStyle.Bold))
                {
                    g.DrawString($"{_value} {_unit}", valueFont, Brushes.Black, valueRect, sf);
                }

                // Başlık Metni (Örn: "SU MİKTARI")
                RectangleF titleRect = new RectangleF(0, tankBounds.Bottom + 20, panelWidth, 20);
                using (Font titleFont = new Font("Segoe UI", 9F))
                {
                    g.DrawString(_title, titleFont, Brushes.Gray, titleRect, sf);
                }
            }
        }
    }
}