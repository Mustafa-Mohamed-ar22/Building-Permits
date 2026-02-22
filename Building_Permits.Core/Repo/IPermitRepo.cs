using Building_Permits.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Core.Repo
{
    public interface IPermitRepo
    {
        public Permit GetById(int id);
        public bool AddPermit(Permit permit);
        public bool DeletePermit(int id);
        public List<Permit> GetAllPermits();
        public void Save();
    }
}
