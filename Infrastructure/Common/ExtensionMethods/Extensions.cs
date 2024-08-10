using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace Infrastructure.Common.ExtensionMethods
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Method to get hash of the given string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="workFactor"></param>
        /// <returns></returns>

        /// <summary>
        /// This method will return the most inner exception message.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>

        public static string GetBaseExceptionMessage(this Exception ex)
        {
            return ex?.GetBaseException()?.Message ?? "Something went wrong";
        }

        /// <summary>
        /// This method is used to convert an amount into its smallest currency. e.g For USD it will convert it into Cents.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static long ToLowestCurrency(this decimal amount)
        {
            return Convert.ToInt64(decimal.Round(amount, 2) * 100);
        }

        /// <summary>
        /// This method will return the current logged in user id.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string GetUserClaim(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            return principal.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Get Enum Description
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>

        public static string GetEnumDescription(this Enum value)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


    }
}
