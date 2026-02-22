using Building_Permits.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Core.Repo
{
    public interface IClientRepo
    {
        public Client GetById(int id);
        public Client GetByPermitId(int id);
        public bool AddClient(Client client);
        public bool DeleteClient(int id);
        public bool UpdateClient(Client client);    
        public List<Client> GetAllClients();
        public void Save();
        Task SaveAsync();
    }
}
