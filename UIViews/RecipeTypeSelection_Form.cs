using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TekstilScada.UI.Views;

namespace TekstilScada.UIViews
{
    public partial class RecipeTypeSelection_Form : Form
    {
       // makinetip.text = "Makine Tipini Seçiniz"
            
            
        public string SelectedType { get; private set; }
        public RecipeTypeSelection_Form(List<string> machineTypes)
        {
           
            InitializeComponent(); // Designer.cs'in oluşturduğu metot

            // ComboBox'ı doldur
            cmbRecipeType.DataSource = machineTypes;
            if (cmbRecipeType.Items.Count > 0)
            {
                cmbRecipeType.SelectedIndex = 0;
            }

            makinetip.Text = "Select Machine Type";
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (cmbRecipeType.SelectedItem != null)
            {
              SelectedType = cmbRecipeType.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a prescription type.", "Warning");
            }
        }
    }
}
