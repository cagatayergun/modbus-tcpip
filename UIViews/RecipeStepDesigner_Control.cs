using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Windows.Forms;
using TekstilScada.Core;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.UI.Views
{
    public partial class RecipeStepDesigner_Control : UserControl
    {
        private Control _activeControl;
        private Point _previousLocation;
        private readonly RecipeConfigurationRepository _configRepo = new RecipeConfigurationRepository();
        // Diğer repository'ler ve veriler...

        public RecipeStepDesigner_Control()
        {
            InitializeComponent();
            // Olayları bağla
            pnlDesignSurface.DragEnter += PnlDesignSurface_DragEnter;
            pnlDesignSurface.DragDrop += PnlDesignSurface_DragDrop;
            pnlDesignSurface.Paint += PnlDesignSurface_Paint; // YENİ: Seçim çerçevesini çizmek için
            pnlDesignSurface.Click += (s, e) => SelectControl(null); // Boş alana tıklayınca seçimi kaldır
            btnLabel.MouseDown += Toolbox_MouseDown;
            btnNumeric.MouseDown += Toolbox_MouseDown;
            btnCheckbox.MouseDown += Toolbox_MouseDown;
            btnTextbox.MouseDown += Toolbox_MouseDown; // HATA GİDERİLDİ: Olay eklendi
            btnSaveLayout.Click += BtnSaveLayout_Click;
            btnNewLayout.Click += BtnNewLayout_Click; // YENİ
            // YENİ: ComboBox olaylarını bağla
            cmbMachineSubType.SelectedIndexChanged += LoadLayoutForSelection;
            cmbStepType.SelectedIndexChanged += LoadLayoutForSelection;
            // YENİ: Toolbox'a TextBox butonu için olay ekle
            btnTextbox.MouseDown += Toolbox_MouseDown;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.DesignMode)
            {
                LoadComboBoxes();
            }
        }

        private void LoadComboBoxes()
        {
            // Makine Tiplerini Yükle
            cmbMachineSubType.DataSource = _configRepo.GetMachineSubTypes();

            // Adım Tiplerini Yükle
            cmbStepType.DataSource = _configRepo.GetStepTypes();
            cmbStepType.DisplayMember = "StepName";
            cmbStepType.ValueMember = "Id";
        }
        // YENİ: Seçime göre mevcut tasarımı yükleyen metot
        private void LoadLayoutForSelection(object sender, EventArgs e)
        {
            if (cmbMachineSubType.SelectedItem == null || cmbStepType.SelectedItem == null) return;

            string machineSubType = cmbMachineSubType.SelectedItem.ToString();

            // HATA DÜZELTMESİ: SelectedValue doğrudan dönüştürülemez.
            // Önce seçili öğeyi (DataRowView) alıp, sonra içinden 'Id' kolonunu okumalıyız.
            DataRowView selectedStepTypeRow = cmbStepType.SelectedItem as DataRowView;
            if (selectedStepTypeRow == null) return;
            int stepTypeId = Convert.ToInt32(selectedStepTypeRow["Id"]);

            string layoutJson = _configRepo.GetLayoutJson(machineSubType, stepTypeId);

            pnlDesignSurface.Controls.Clear();
            SelectControl(null);

            if (!string.IsNullOrEmpty(layoutJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var controlsData = JsonSerializer.Deserialize<List<ControlMetadata>>(layoutJson, options);
                    foreach (var data in controlsData)
                    {
                        CreateControlFromJson(data);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading design: {ex.Message}");
                }
            }
        }

        // YENİ: Kaydetme butonu artık dinamik
        private async void BtnSaveLayout_Click(object sender, EventArgs e)
        {
            if (cmbMachineSubType.SelectedItem == null || cmbStepType.SelectedValue == null)
            {
                MessageBox.Show("Please select a machine subtype and step type.", "Missing Information");
                return;
            }

            string machineSubType = cmbMachineSubType.SelectedItem.ToString();
            int stepTypeId = Convert.ToInt32(cmbStepType.SelectedValue);
            string layoutName = $"{machineSubType} - {cmbStepType.Text}";

            var controlsMetadata = new List<ControlMetadata>();
            foreach (Control control in pnlDesignSurface.Controls)
            {
                var wrapper = new ControlPropertyWrapper(control);

                // Önce her kontrol için ortak olan meta veriyi oluşturuyoruz.
                var metadata = new ControlMetadata
                {
                    ControlType = control.GetType().AssemblyQualifiedName,
                    Name = wrapper.Name,
                    Text = wrapper.Text,
                    Location = $"{wrapper.Location.X}, {wrapper.Location.Y}",
                    Size = $"{wrapper.Size.Width}, {wrapper.Size.Height}",
                    PLC_WordIndex = wrapper.PLC_WordIndex,
                    PLC_BitIndex = wrapper.PLC_BitIndex
                    // Not: Maximum ve DecimalPlaces'ı burada hemen atamıyoruz.
                };

                // --- YENİ EKLENECEK KISIM BURASI ---
                // Şimdi, eğer kontrol NumericUpDown ise, ona özel özellikleri de ekliyoruz.
                if (control is NumericUpDown num)
                {
                    metadata.Maximum = num.Maximum;
                    metadata.DecimalPlaces = num.DecimalPlaces; // İşte bu satırı ekliyoruz!
                }
                // --- EKLEME BİTTİ ---

                controlsMetadata.Add(metadata);
            }

            string jsonLayout = JsonSerializer.Serialize(controlsMetadata, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                await Task.Run(() => _configRepo.SaveLayout(layoutName, machineSubType, stepTypeId, jsonLayout));
                MessageBox.Show("Interface design saved successfully!", "Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving design: {ex.Message}", "Error");
            }
        }
        private void BtnNewLayout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("The current design will be cleared. Are you sure?", "New Design", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                pnlDesignSurface.Controls.Clear();
                SelectControl(null);
            }
        }
        // Refactor: JSON'dan kontrol oluşturma işlemini ayrı bir metoda taşıdık
        private void CreateControlFromJson(ControlMetadata data)
        {
            Type controlType = Type.GetType(data.ControlType);
            if (controlType == null) return;

            Control newControl = (Control)Activator.CreateInstance(controlType);
            var wrapper = new ControlPropertyWrapper(newControl);

            wrapper.Name = data.Name;
            wrapper.Text = data.Text;
            wrapper.Location = new Point(int.Parse(data.Location.Split(',')[0].Trim()), int.Parse(data.Location.Split(',')[1].Trim()));
            wrapper.Size = new Size(int.Parse(data.Size.Split(',')[0].Trim()), int.Parse(data.Size.Split(',')[1].Trim()));
            wrapper.PLC_WordIndex = data.PLC_WordIndex;
            wrapper.PLC_BitIndex = data.PLC_BitIndex;
            if (newControl is NumericUpDown num)
            {
                num.Maximum = data.Maximum;
                num.DecimalPlaces = data.DecimalPlaces;

            }
            

            newControl.MouseDown += Control_MouseDown;
            newControl.MouseMove += Control_MouseMove;
            newControl.MouseUp += Control_MouseUp;
            newControl.KeyDown += Control_KeyDown; // YENİ: Silme işlemi için
            pnlDesignSurface.Controls.Add(newControl);
        }

        // Seviye 2'den gelen diğer metotlar (Toolbox_MouseDown, PnlDesignSurface_DragDrop, Control_MouseDown vb.)
        // herhangi bir değişiklik olmadan aynı kalıyor.
    
        private void Toolbox_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control control && control.Tag is Type type)
            {
                control.DoDragDrop(type.AssemblyQualifiedName, DragDropEffects.Copy);
            }
        }

        private void PnlDesignSurface_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text)) e.Effect = DragDropEffects.Copy;
        }

        private void PnlDesignSurface_DragDrop(object sender, DragEventArgs e)
        {
            string typeName = (string)e.Data.GetData(DataFormats.Text);
            Type controlType = Type.GetType(typeName);
            if (controlType != null)
            {
                Control newControl = (Control)Activator.CreateInstance(controlType);
                newControl.Location = pnlDesignSurface.PointToClient(new Point(e.X, e.Y));
                newControl.Tag = new PlcMapping(); // Boş PLC eşlemesi ile başlat

                // Yeni kontrollere hareket ve seçme yeteneği ekle
                newControl.MouseDown += Control_MouseDown;
                newControl.MouseMove += Control_MouseMove;
                newControl.MouseUp += Control_MouseUp;
                newControl.KeyDown += Control_KeyDown;
                pnlDesignSurface.Controls.Add(newControl);
                SelectControl(newControl);
            }
        }

        // --- DÜZELTME: Silme işleminin çalışması için odaklama ekleniyor ---
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _activeControl = sender as Control;
                _activeControl.Focus(); // KONTROLE ODAKLAN! Bu, KeyDown olayının tetiklenmesini sağlar.
                _previousLocation = e.Location;
                SelectControl(_activeControl);
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _activeControl != null)
            {
                _activeControl.Left += e.X - _previousLocation.X;
                _activeControl.Top += e.Y - _previousLocation.Y;
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            _activeControl = null;
            propertyGrid.Refresh();
        }
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _activeControl != null)
            {
                pnlDesignSurface.Controls.Remove(_activeControl);
                SelectControl(null);
            }
        }
        private void SelectControl(Control control)
        {
            _activeControl = control;
            // PropertyGrid'e doğrudan kontrolü değil, özel sarmalayıcı sınıfımızı atıyoruz.
            propertyGrid.SelectedObject = new ControlPropertyWrapper(control);
        }

        private void PnlDesignSurface_Paint(object sender, PaintEventArgs e)
        {
            if (_activeControl != null)
            {
                // Seçili kontrolün etrafına kesikli bir dikdörtgen çiz
                Rectangle rect = _activeControl.Bounds;
                rect.Inflate(2, 2); // Çerçevenin kontrolün biraz dışında olması için
                ControlPaint.DrawFocusRectangle(e.Graphics, rect);
            }
        }


        // --- YENİ YARDIMCI SINIF: PropertyGrid için özel özellikler ekler ---
        // --- HATA DÜZELTMESİ BU SINIFTA YAPILDI ---
        public class ControlPropertyWrapper
        {
            private readonly Control _control;
            private readonly PlcMapping _mapping;

            public ControlPropertyWrapper(Control control)
            {
                // Null kontrolü constructor'da kalmaya devam ediyor, bu iyi bir pratik.
                if (control == null)
                {
                    _control = null;
                    _mapping = null;
                    return;
                }

                _control = control;
                _mapping = _control.Tag as PlcMapping ?? new PlcMapping();
                _control.Tag = _mapping;
            }

            // HATA DÜZELTMESİ: Tüm property'lerin get ve set blokları,
            // _control veya _mapping nesnelerinin null olma ihtimaline karşı
            // null-conditional operator (?.) ile güvenli hale getirildi.

            [Category("Tasarım")]
            [DisplayName("Ondalık Basamak Sayısı")]
            public int DecimalPlaces
            {
                get => (_control as NumericUpDown)?.DecimalPlaces ?? 0;
                set { if (_control is NumericUpDown num) num.DecimalPlaces = value; }
            }
            [Category("Tasarım")]
            public string Name
            {
                get => _control?.Name ?? string.Empty;
                set { if (_control != null) _control.Name = value; }
            }
            [Category("Tasarım")]
            public string Text
            {
                get => _control?.Text ?? string.Empty;
                set { if (_control != null) _control.Text = value; }
            }
            [Category("Tasarım")]
            public Point Location
            {
                get => _control?.Location ?? Point.Empty;
                set { if (_control != null) _control.Location = value; }
            }
            [Category("Tasarım")]
            public Size Size
            {
                get => _control?.Size ?? Size.Empty;
                set { if (_control != null) _control.Size = value; }
            }
            [Category("PLC Eşleme")]
            [DisplayName("PLC Word Index")]
            public int PLC_WordIndex
            {
                get => _mapping?.WordIndex ?? 0;
                set { if (_mapping != null) _mapping.WordIndex = value; }
            }
            [Category("PLC Eşleme")]
            [DisplayName("PLC Bit Index")]
            public int PLC_BitIndex
            {
                get => _mapping?.BitIndex ?? 0;
                set { if (_control is CheckBox && _mapping != null) _mapping.BitIndex = value; }
            }
            [Category("PLC Eşleme")]
            [DisplayName("String Word Uzunluğu")]
            public int PLC_StringWordLength
            {
                get => _mapping?.StringWordLength ?? 0;
                set { if (_control is TextBox && _mapping != null) _mapping.StringWordLength = value; }
            }
        }
    }
}
