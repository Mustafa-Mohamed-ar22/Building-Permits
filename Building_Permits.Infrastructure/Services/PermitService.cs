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
    public class PermitService : IPermitRepo
    {
        private AppDbContext _context;
        public PermitService(AppDbContext _context)
        {
            this._context = _context;
        }
        public bool AddPermit(Permit permit)
        {
            if (permit is not null)
            {
                _context.Permits.Add(permit);
                return true;
            }
            return false;
        }

        public bool DeletePermit(int id)
        {
            Permit permit = _context.Permits.FirstOrDefault(x => x.Id == id);
            if (permit is not null)
            {
                _context.Permits.Remove(permit);
                return true;
            }
            return false;
        }

        public List<Permit> GetAllPermits() => _context.Permits.Include(x=>x.Client).ToList();
        public Permit GetById(int id)
        {
            var permits = _context.Permits.ToList();
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var permit in permits)
            {
                if(permit.Id==id)
                    Console.WriteLine("I FOUND IT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine(permit.Id);
            }
			Console.ForegroundColor = ConsoleColor.White;

			return _context.Permits.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
