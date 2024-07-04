using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface ITemplateWeekRepository
{
    Task<IEnumerable<TemplateWeek>> GetAllAsync();

    Task<TemplateWeek> GetByIdAsync(Guid id);

    Task<IEnumerable<TemplateWeek>> GetAllTemplateWeeksAsync();

    Task<TemplateWeek> GetTemplateWeekByIdAsync(Guid id);

    Task<TemplateWeek> CreateTemplateWeekAsync(TemplateWeek templateWeek);

    Task UpdateTemplateWeekAsync(TemplateWeek templateWeek);

    Task DeleteTemplateWeekAsync(Guid id);
}