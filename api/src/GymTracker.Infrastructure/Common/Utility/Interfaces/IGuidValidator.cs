namespace GymTracker.Infrastructure.Common.Utility.Interfaces;

public interface IGuidValidator
{
    void Validate(params Guid[] guids);
}