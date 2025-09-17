namespace TekstilScada.Services
{
    /// <summary>
    /// Uygulama genelindeki tüm yetki kurallarını merkezi olarak yönetir.
    /// </summary>
    public static class PermissionService
    {
        public static bool HasAnyPermission(List<int> requiredRoleIds)
        {
            if (CurrentUser.User == null || CurrentUser.User.Roles == null)
            {
                return false;
            }

            var userRoleIds = CurrentUser.User.Roles.Select(r => r.Id).ToList();
            return userRoleIds.Any(roleId => requiredRoleIds.Contains(roleId));
        }
      

        // Diğer tüm yetki kuralları buraya eklenebilir.
        // Örneğin:
        // public static bool CanAcknowledgeAlarms => CurrentUser.HasRole("Admin") || CurrentUser.HasRole("Mühendis") || CurrentUser.HasRole("Operatör");
    }
}