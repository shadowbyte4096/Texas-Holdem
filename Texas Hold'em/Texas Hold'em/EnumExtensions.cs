using System.ComponentModel;

namespace Project;
public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        var type = value.GetType();
        var memInfo = type.GetMember(value.ToString());

        return memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() is DescriptionAttribute descriptionAttribute
            ? descriptionAttribute.Description
            : value.ToString();
    }
}
