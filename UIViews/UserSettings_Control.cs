// UI/Views/UserSettings_Control.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TekstilScada.Models;
using TekstilScada.Properties;
using TekstilScada.Repositories;
using TekstilScada.Services;

namespace TekstilScada.UI.Views
{
    public partial class UserSettings_Control : UserControl
    {
        private readonly UserRepository _repository;
        private List<User> _users;
        private List<Role> _allRoles;
        private User _selectedUser;
      
        public UserSettings_Control()
        {
            InitializeComponent();
            ApplyLocalization();
            _repository = new UserRepository();
        }

        private void UserSettings_Control_Load(object sender, EventArgs e)
        {
            LoadAllRoles();
            RefreshUserList();
        }
        private void ApplyLocalization()
        {


            groupBox1.Text = Resources.Userdetail;
            label1.Text = Resources.Username;
            label3.Text = Resources.namesurname;
            label4.Text = Resources.Password;
            chkIsActive.Text = Resources.Useractive;
            label5.Text= Resources.roller;
            btnNew.Text = Resources.New;
            btnSave.Text = Resources.Save;
            btnDelete.Text = Resources.Delete;
        }
        public void LoadAllRoles()
        {
           
            _allRoles = _repository.GetAllRoles();
            
            var filteredRoles = _allRoles.Where(r => r.Id != 1000).ToList();
            // ComboBox'ın veri kaynağını sıfırla
            
            if (PermissionService.HasAnyPermission(new List<int> { 1000 }))
            {
                clbRoles.DataSource = null;
                clbRoles.Items.Clear();
                clbRoles.DataSource = _allRoles;
            }
            else
            {
                clbRoles.DataSource = null;
                clbRoles.Items.Clear();
                clbRoles.DataSource = filteredRoles;
            }



            clbRoles.DisplayMember = "RoleName";
            clbRoles.ValueMember = "Id";
            RefreshUserList();
        }

        private void RefreshUserList()
        {
            _users = _repository.GetAllUsers();
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = _users;
            if (dgvUsers.Columns["Id"] != null) dgvUsers.Columns["Id"].Visible = false;
            if (dgvUsers.Columns["Roles"] != null) dgvUsers.Columns["Roles"].Visible = false;
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                _selectedUser = dgvUsers.SelectedRows[0].DataBoundItem as User;
                if (_selectedUser != null)
                {
                    PopulateFields(_selectedUser);
                }
            }
        }

        private void PopulateFields(User user)
        {
            txtUsername.Text = user.Username;
            txtFullName.Text = user.FullName;
            chkIsActive.Checked = user.IsActive;
            txtPassword.Text = ""; // Şifre alanını güvenlik için boş bırak

            // Kullanıcının rollerini işaretle
            var userRoles = _repository.GetUserRoles(user.Id);
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                var role = clbRoles.Items[i] as Role;
                if (userRoles.Any(ur => ur.Id == role.Id))
                {
                    clbRoles.SetItemChecked(i, true);
                }
                else
                {
                    clbRoles.SetItemChecked(i, false);
                }
            }
        }

        private void ClearFields()
        {
            _selectedUser = null;
            dgvUsers.ClearSelection();
            txtUsername.Text = "";
            txtFullName.Text = "";
            txtPassword.Text = "";
            chkIsActive.Checked = true;
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                clbRoles.SetItemChecked(i, false);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
               
                MessageBox.Show($"{Resources.usernamehard}", $"{Resources.EksikBilgi}");
                return;
            }

            var selectedRoleIds = clbRoles.CheckedItems.OfType<Role>().Select(r => r.Id).ToList();

            try
            {
                if (_selectedUser == null) // Yeni Kayıt
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show($"{Resources.Yenikullanıcıiçinsifrezorunludur}", $"{Resources.EksikBilgi}");
                        return;
                    }
                   
                    var newUser = new User { Username = txtUsername.Text, FullName = txtFullName.Text, IsActive = chkIsActive.Checked };
                    _repository.AddUser(newUser, txtPassword.Text, selectedRoleIds);
                }
                else // Güncelleme
                {
                    _selectedUser.Username = txtUsername.Text;
                    _selectedUser.FullName = txtFullName.Text;
                    _selectedUser.IsActive = chkIsActive.Checked;
                    _repository.UpdateUser(_selectedUser, selectedRoleIds, txtPassword.Text);
                }
                RefreshUserList();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.Kayıt_sırasında_hata_}{ ex.Message}", $"{Resources.Error}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;
            var result = MessageBox.Show($"'{_selectedUser.Username}' {Resources.kullanıcısını_silmek_istediğinizden_emin_misiniz_}", $"{Resources.Confirim}", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteUser(_selectedUser.Id);
                    RefreshUserList();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Resources.Silmesırasındahata} { ex.Message}", "Hata");
                }
            }
        }
    }
}