using Microsoft.EntityFrameworkCore;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Client;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class ClientService : BaseService, IClientService
    {
        public ClientService(BaseTLTravelDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<ClientVM> GetAll()
        {
            return Db.Clients.Where(x => x.IsActive)
                .Select(c => new ClientVM { Id = c.Id, Name = c.Name, Email = c.Email, Phone = c.Phone });
        }

        public ClientVM GetById(int id)
        {
            var client = Db.Clients.Find(id);
            if (client == null)
            {
                throw new ArgumentException("No such client");
            }
            return new ClientVM { Name = client.Name, Email = client.Email, Phone = client.Phone };
        }

        public ClientVM AddEdit(ClientUM client, int id = 0)
        {
            if (id > 0)
            {
                var existingClient = Db.Clients.Find(id);
                if (existingClient == null) throw new Exception("Client not found.");

                existingClient.Id = id;
                existingClient.Name = client.Name;
                existingClient.Email = client.Email;
                existingClient.Phone = client.Phone;

                Db.Clients.Update(existingClient);
            }
            else
            {
                var newClient = new Client
                {
                    Id = id,
                    Name = client.Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    IsActive = true
                };

                Db.Clients.Add(newClient);
            }

            Db.SaveChanges();

            return new ClientVM { Name = client.Name, Email = client.Email, Phone = client.Phone };
        }

        public void Delete(int id)
        {
            var client = this.Db.Clients.Find(id);
            client.IsActive = false;
            Db.SaveChanges();

        }
    }
}
