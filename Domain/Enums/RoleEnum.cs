using System.ComponentModel;

namespace Domain.Enums
{
    public enum RoleEnum
    {
        [Description("SuperAdmin")]
        SuperAdmin = 1,
        [Description("Advance Seller")]
        AdvanceSeller = 2,
        [Description("User")]
        NoramlUser = 3,
    }
}
