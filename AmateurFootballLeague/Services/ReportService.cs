using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public IQueryable<Report> GetList(params Expression<Func<Report, object>>[] includes)
        {
            return _reportRepository.GetList(includes);
        }
        public async Task<Report> GetByIdAsync(int id)
        {
            return await _reportRepository.GetByIdAsync(id);
        }
        public async Task<Report> AddAsync(Report entity)
        {
            return await _reportRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Report entity)
        {
            return await _reportRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Report entity)
        {
            return await _reportRepository.DeleteAsync(entity);
        }
    }
}
