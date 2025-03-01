namespace Application.Services
{
    public class AuthorizationService
    {
        public bool IsAuthorized(string userRole, string requiredRole)
        {
            var roleHierarchy = new[] { "User", "Tester", "Auditor", "Deployer", "Admin" };

            return Array.IndexOf(roleHierarchy, userRole) >= Array.IndexOf(roleHierarchy, requiredRole);
        }
    }
}
