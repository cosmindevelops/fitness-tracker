namespace GymTracker.Infrastructure.Common.Utility;

public class GuidValidator
{
    public static void Validate(params Guid[] guids)
    {
        foreach (var guid in guids)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(guid), "GUID cannot be empty.");
            }
        }
    }
}