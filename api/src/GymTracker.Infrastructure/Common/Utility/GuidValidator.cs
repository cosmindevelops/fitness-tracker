using GymTracker.Infrastructure.Common.Utility.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Common.Utility;

public class GuidValidator : IGuidValidator
{
    private readonly ILogger<GuidValidator> _logger;

    public GuidValidator(ILogger<GuidValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Validate(params Guid[] guids)
    {
        foreach (var guid in guids)
        {
            if (guid == Guid.Empty)
            {
                _logger.LogError("GUID cannot be empty: {Guid}", guid);
                throw new ArgumentNullException(nameof(guid), "GUID cannot be empty.");
            }
        }
    }
}