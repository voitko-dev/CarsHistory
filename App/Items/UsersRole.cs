namespace CarsHistory.Items;

public enum UsersRole
{
    User,
    Admin,
    SuperAdmin,
    Pending,
    None
}

public static class UsersRoleExtensions
{
    public static UsersRole GetRole(this string usersRole)
    {
        if (string.Equals(usersRole, nameof(UsersRole.Admin), StringComparison.CurrentCultureIgnoreCase))
            return UsersRole.Admin;
        
        if (string.Equals(usersRole, nameof(UsersRole.User), StringComparison.CurrentCultureIgnoreCase))
            return UsersRole.User;
        
        if (string.Equals(usersRole, nameof(UsersRole.SuperAdmin), StringComparison.CurrentCultureIgnoreCase))
            return UsersRole.SuperAdmin;

        return string.Equals(usersRole, nameof(UsersRole.Pending), StringComparison.CurrentCultureIgnoreCase) ? UsersRole.Pending : UsersRole.None;
    }
    
    public static readonly string GG = "gkX5IOuIu6bIKDYSpFPSOigZIOO55IM5";
}