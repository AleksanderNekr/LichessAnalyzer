using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi;

public static class Extensions
{
    public static PlayCategory ToCategoryEnum(this string source)
    {
        Enum.TryParse(source, ignoreCase: true, out PlayCategory category);

        return category;
    }
}
