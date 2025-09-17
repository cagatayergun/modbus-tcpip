// UI/Controls/RecipeStepEditors/StepEditor_Control.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.UI.Controls.RecipeStepEditors
{
    public partial class StepEditor_Control : UserControl
    {
        private ScadaRecipeStep _step;
        private Machine _machine;
        private bool _isUpdating = false;
        private readonly RecipeConfigurationRepository _configRepo = new RecipeConfigurationRepository();
        public event EventHandler StepDataChanged;

        public StepEditor_Control()
        {
            InitializeComponent();

            chkSuAlma.CheckedChanged += OnStepTypeChanged;
            chkIsitma.CheckedChanged += OnStepTypeChanged;
            chkCalisma.CheckedChanged += OnStepTypeChanged;
            chkDozaj.CheckedChanged += OnStepTypeChanged;
            chkBosaltma.CheckedChanged += OnStepTypeChanged;
            chkSikma.CheckedChanged += OnStepTypeChanged;

            flpParameters.Resize += new EventHandler(flpParameters_Resize);
        }

        public void LoadStep(ScadaRecipeStep step, Machine machine)
        {
            _step = step;
            _machine = machine;
            _isUpdating = true;
            UpdateCheckboxesFromStepData();
            _isUpdating = false;
            UpdateEditorPanels();
        }

        private void flpParameters_Resize(object sender, EventArgs e)
        {
            flpParameters.SuspendLayout();
            foreach (Control control in flpParameters.Controls)
            {
                if (control is Panel)
                {
                    control.Width = flpParameters.ClientSize.Width - 25;
                }
            }
            flpParameters.ResumeLayout();
        }

        public void SetReadOnly(bool isReadOnly)
        {
            SetControlsState(this.Controls, !isReadOnly);
        }

        private void SetControlsState(Control.ControlCollection controls, bool enabled)
        {
            foreach (Control control in controls)
            {
                if (control is NumericUpDown || control is TextBox || control is CheckBox || control is ComboBox)
                {
                    control.Enabled = enabled;
                }
                if (control.HasChildren)
                {
                    SetControlsState(control.Controls, enabled);
                }
            }
        }

        private void UpdateCheckboxesFromStepData()
        {
            if (_step == null) return;
            short controlWord = _step.StepDataWords[24];
            chkSuAlma.Checked = (controlWord & 1) != 0;
            chkIsitma.Checked = (controlWord & 2) != 0;
            chkCalisma.Checked = (controlWord & 4) != 0;
            chkDozaj.Checked = (controlWord & 8) != 0;
            chkBosaltma.Checked = (controlWord & 16) != 0;
            chkSikma.Checked = (controlWord & 32) != 0;
        }

        private void OnStepTypeChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            var changedCheckbox = sender as CheckBox;
            if (changedCheckbox == null) return;

            // Kural kontrol metodunu çağırıyoruz.
            if (!IsSelectionValid(changedCheckbox))
            {
                // Eğer seçim geçerli değilse, yapılan son değişikliği geri al.
                _isUpdating = true;
                changedCheckbox.Checked = false;
                _isUpdating = false;
                return; // Metottan çık, başka işlem yapma.
            }

            // Eğer seçim geçerliyse, her zamanki gibi devam et.
            UpdateStepDataFromCheckboxes();
            UpdateEditorPanels();
            StepDataChanged?.Invoke(this, EventArgs.Empty);
        }

        // --- HATA DÜZELTMESİ: KURAL KONTROLÜ ARTIK DOĞRU YERDE ÇALIŞIYOR ---
        private bool IsSelectionValid(CheckBox justChanged)
        {
            // Kontrol içindeki tüm CheckBox'ları, ait oldukları panelden (`pnlStepTypes`) alıyoruz.
            // Eğer panelin adı farklıysa, bu satırdaki "pnlStepTypes" ismini doğru olanla değiştirin.
            var checkedBoxes = pnlStepTypes.Controls.OfType<CheckBox>().Where(c => c.Checked).ToList();

            // Kural 1: Toplamda 2'den fazla seçim yapılamaz.
            if (checkedBoxes.Count > 2)
            {
                MessageBox.Show("You can select up to 2 different transaction types in one step.", "Rule Violation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Adım gruplarını tanımla
            var specialSteps = new List<CheckBox> { chkSikma, chkBosaltma };
            var standardSteps = new List<CheckBox> { chkSuAlma, chkIsitma, chkDozaj, chkCalisma };

            // Seçili olanlar arasında özel veya standart adım var mı?
            bool isAnySpecialChecked = checkedBoxes.Any(c => specialSteps.Contains(c));
            bool isAnyStandardChecked = checkedBoxes.Any(c => standardSteps.Contains(c));

            // Kural 2: Özel Grup ve Standart Grup bir arada seçilemez.
            if (isAnySpecialChecked && isAnyStandardChecked)
            {
                MessageBox.Show("Spinning or Draining steps cannot be selected together with other steps such as Water Intake, Heating.", "Rule Violation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Eğer buraya kadar geldiyse, seçim geçerlidir.
            return true;
        }

        private void UpdateStepDataFromCheckboxes()
        {
            if (_step == null) return;
            short controlWord = 0;
            if (chkSuAlma.Checked) controlWord |= 1;
            if (chkIsitma.Checked) controlWord |= 2;
            if (chkCalisma.Checked) controlWord |= 4;
            if (chkDozaj.Checked) controlWord |= 8;
            if (chkBosaltma.Checked) controlWord |= 16;
            if (chkSikma.Checked) controlWord |= 32;
            _step.StepDataWords[24] = controlWord;
        }

        private void UpdateEditorPanels()
        {
            flpParameters.SuspendLayout();
            flpParameters.Controls.Clear();

            if (_step == null || _machine == null)
            {
                flpParameters.ResumeLayout();
                return;
            }

            try
            {
                var stepColorMap = new Dictionary<int, Color>
                {
                    { 1, Color.FromArgb(204, 229, 255) }, { 2, Color.FromArgb(255, 204, 204) },
                    { 3, Color.FromArgb(204, 255, 204) }, { 4, Color.FromArgb(255, 211, 106) },
                    { 5, Color.FromArgb(173, 216, 230) }, { 6, Color.FromArgb(213, 213, 211) }
                };
                var stepIdMap = new Dictionary<CheckBox, int>
                {
                    { chkSuAlma, 1 }, { chkIsitma, 2 }, { chkCalisma, 3 },
                    { chkDozaj, 4 }, { chkBosaltma, 5 }, { chkSikma, 6 }
                };

                foreach (var kvp in stepIdMap)
                {
                    if (kvp.Key.Checked)
                    {
                        int stepId = kvp.Value;
                        string stepName = kvp.Key.Text.ToUpper();
                        var containerPanel = new Panel
                        {
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(10, 5, 10, 5)
                        };
                        containerPanel.BackColor = stepColorMap.TryGetValue(stepId, out Color color) ? color : Color.WhiteSmoke;
                        int currentY = 5;
                        var header = new Label
                        {
                            Text = stepName,
                            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                            ForeColor = Color.Black,
                            AutoSize = true,
                            Location = new Point(5, currentY)
                        };
                        containerPanel.Controls.Add(header);
                        currentY = header.Bottom + 10;
                        string layoutJson = _configRepo.GetLayoutJson(_machine.MachineSubType, stepId) ?? _configRepo.GetLayoutJson("DEFAULT", stepId);
                        if (!string.IsNullOrEmpty(layoutJson))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var controlsData = JsonSerializer.Deserialize<List<ControlMetadata>>(layoutJson, options);
                            foreach (var data in controlsData)
                            {
                                var control = CreateControlFromMetadata(data);
                                if (control != null)
                                {
                                    control.Location = new Point(15, currentY);
                                    containerPanel.Controls.Add(control);
                                    currentY = control.Bottom + 5;
                                }
                            }
                        }
                        containerPanel.Height = currentY + 5;
                        flpParameters.Controls.Add(containerPanel);
                    }
                }
            }
            finally
            {
                flpParameters.ResumeLayout(true);
                flpParameters_Resize(this, EventArgs.Empty);
            }
        }

        private Control CreateControlFromMetadata(ControlMetadata data)
        {
            Type controlType = Type.GetType(data.ControlType);
            if (controlType == null) return null;
            Control control = (Control)Activator.CreateInstance(controlType);
            control.Name = data.Name;
            control.Text = data.Text;
            var locParts = data.Location.Split(',');
            control.Location = new Point(int.Parse(locParts[0].Trim()), int.Parse(locParts[1].Trim()));
            var sizeParts = data.Size.Split(',');
            control.Size = new Size(int.Parse(sizeParts[0].Trim()), int.Parse(sizeParts[1].Trim()));
            control.Tag = new PlcMapping { WordIndex = data.PLC_WordIndex, BitIndex = data.PLC_BitIndex };
            if (control is NumericUpDown num)
            {
                num.Maximum = data.Maximum;
                num.DecimalPlaces = data.DecimalPlaces;
                if (data.PLC_WordIndex < _step.StepDataWords.Length)
                {
                    if (num.DecimalPlaces > 0)
                        num.Value = _step.StepDataWords[data.PLC_WordIndex] / (decimal)Math.Pow(10, num.DecimalPlaces);
                    else
                        num.Value = _step.StepDataWords[data.PLC_WordIndex];
                }
                num.ValueChanged += OnDynamicControlValueChanged;
            }
            else if (control is CheckBox chk)
            {
                if (data.PLC_WordIndex < _step.StepDataWords.Length)
                {
                    short word = _step.StepDataWords[data.PLC_WordIndex];
                    int bitMask = 1 << data.PLC_BitIndex;
                    chk.Checked = (word & bitMask) != 0;
                }
                chk.CheckedChanged += OnDynamicControlValueChanged;
            }
            return control;
        }

        private void OnDynamicControlValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            Control control = sender as Control;
            if (control?.Tag is not PlcMapping mapping) return;
            if (mapping.WordIndex < _step.StepDataWords.Length)
            {
                if (control is NumericUpDown num)
                {
                    if (num.DecimalPlaces > 0)
                        _step.StepDataWords[mapping.WordIndex] = (short)(num.Value * (decimal)Math.Pow(10, num.DecimalPlaces));
                    else
                        _step.StepDataWords[mapping.WordIndex] = (short)num.Value;
                }
                else if (control is CheckBox chk)
                {
                    SetBit(_step.StepDataWords, mapping.WordIndex, mapping.BitIndex, chk.Checked);
                }
            }
            StepDataChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetBit(short[] data, int wordIndex, int bitIndex, bool value)
        {
            if (value) data[wordIndex] = (short)(data[wordIndex] | (1 << bitIndex));
            else data[wordIndex] = (short)(data[wordIndex] & ~(1 << bitIndex));
        }
    }
}