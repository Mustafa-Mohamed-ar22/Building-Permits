using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Infrastructure.Services
{
    public class StageService : IStageRepo
    {
        private AppDbContext _context;
        public StageService(AppDbContext _context)
        {
            this._context = _context;
        }

        public List<Stage> GetAllStages()
        {
            return _context.Stages.ToList();
        }

        public Stage GetStageById(int id)
        {
            return _context.Stages.FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<Stage>> GetFirst_6_StagesAsync()=> await _context.Stages.Where(x=>x.OrderNo<=6).ToListAsync();
        public List<Stage> GetFirst_6_Stages()=>  _context.Stages.Where(x=>x.OrderNo<=6).ToList();
        public List<Stage> Get_Second_Stages()=> _context.Stages.Where(x=>x.OrderNo>6&&x.OrderNo<=9).ToList();
        public List<Stage> Get_Third_Stages() => _context.Stages.Where(x => x.OrderNo == 10 || x.OrderNo == 11).ToList();
        public List<Stage> Get_Fourth_Stages()=> _context.Stages.Where(x=>x.OrderNo==12||x.OrderNo == 13).ToList();
    }
}
