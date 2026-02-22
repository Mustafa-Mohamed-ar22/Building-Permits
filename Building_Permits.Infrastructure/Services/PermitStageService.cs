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
    public class PermitStageService : IPermitStageRepo
    {
        private AppDbContext _context;
        public PermitStageService(AppDbContext _context)
        {
            this._context = _context;
        }

        public List<PermitStage> GetAllPermitStages()
        {
            return _context.PermitStages.Include(x=>x.Permit).ToList();
        }

        public PermitStage GetPermitStageById(int id)
        {
            return _context.PermitStages.FirstOrDefault(x => x.Id == id);
        }

        public bool AddPermitStage(PermitStage permitStage)
        {
            if(permitStage is not null)
            {
                _context.PermitStages.Add(permitStage);
                return true;
            }
            return false;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
