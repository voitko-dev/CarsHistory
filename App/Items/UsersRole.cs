namespace CarsHistory.Items;

public enum UsersRole
{
    User,
    Admin,
    Pending,
    None
}

public static class UsersRoleExtensions
{
    public static UsersRole GetRole(this string usersRole)
    {
        if (string.Equals(usersRole, UsersRole.Admin.ToString(), StringComparison.CurrentCultureIgnoreCase))
            return UsersRole.Admin;
        
        if (string.Equals(usersRole, UsersRole.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
            return UsersRole.User;

        return string.Equals(usersRole, UsersRole.Pending.ToString(), StringComparison.CurrentCultureIgnoreCase) ? UsersRole.Pending : UsersRole.None;
    }
    
    public static readonly string GG = "gkX5IOuIu6bIKDYSpFPSOigZIOO55IM5";
}