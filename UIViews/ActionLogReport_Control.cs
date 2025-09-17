// UIViews/ActionLogReport_Control.cs
using System;
using System.Linq;
using System.Windows.Forms;
//using TekstilScada.Core.Repositories;
using TekstilScada.Core.Services;
using TekstilScada.Repositories;

namespace TekstilScada.UIViews
{
    public partial class ActionLogReport_Control : UserControl
    {
        private readonly UserRepository _userRepository;

        public ActionLogReport_Control()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Kullanıcıları filtreleme için combobox'a yükle
            LoadUsers();

            // İlk açılışta verileri yükle
            btnFilter_Click(this, EventArgs.Empty);
        }

        private void LoadUsers()
        {
            var users = _userRepository.GetAllUsers();
            cmbUser.Items.Clear();
            cmbUser.Items.Add("All Users");
            foreach (var user in users)
            {
                cmbUser.Items.Add(user.Username);
            }
            cmbUser.SelectedIndex = 0;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime? startDate = dtpStartDate.Value;
            DateTime? endDate = dtpEndDate.Value.AddDays(1).AddSeconds(-1); // Tarih aralığını tam gün kapsayacak şekilde ayarla

            string username = cmbUser.SelectedIndex > 0 ? cmbUser.SelectedItem.ToString() : null;
            string details = string.IsNullOrEmpty(txtDetails.Text) ? null : txtDetails.Text;

            var logs = _userRepository.GetActionLogs(startDate, endDate, username, details);
            dataGridView1.DataSource = logs;
        }
    }
}