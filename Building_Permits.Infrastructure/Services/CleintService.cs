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
    public class CleintService : IClientRepo
    {
        private AppDbContext _context;
       public CleintService(AppDbContext _context)
        {
            this._context = _context;
        }
        public bool AddClient(Client client)
        {
            if (client is not null)
            {
                _context.Clients.Add(client);
                return true;
            }
            return false;
        }

        public bool DeleteClient(int id)
        {
            Client client = _context.Clients.FirstOrDefault(x => x.Id == id);
            if(client is not null)
            {
                _context.Clients.Remove(client);
                return true;
            }
            return false;
        }

        public List<Client> GetAllClients()=>_context.Clients.Include(x=>x.Permit).ToList();
        public Client GetById(int id) => _context.Clients.FirstOrDefault(x => x.Id == id);
        public bool UpdateClient(Client client)
        {
            Client clientDB = _context.Clients.FirstOrDefault(x => x.Id == client.Id);
            if( client is not null)
            {
                clientDB.FullName = client.FullName;
                clientDB.Address=client.Address;
                clientDB.Phone = client.Phone;
                client.NationalId= client.NationalId;
                return true;
            }
            return false;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Client GetByPermitId(int id)
        {
            return _context.Clients.FirstOrDefault(x => x.PermitId == id);
        }
        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
