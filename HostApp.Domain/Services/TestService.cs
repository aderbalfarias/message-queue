using HostApp.Domain.Entities;
using HostApp.Domain.Interfaces.Repositories;
using HostApp.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HostApp.Domain.Services
{
    public class TestService : ITestService
    {
        private readonly IBaseRepository _baseRepository;

        public TestService(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public Task<IEnumerable<TestEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TestEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TestEntity> GetDetails(int id, int entityId)
        {
            throw new NotImplementedException();
        }

        public void Save(TestEntity model)
        {
            throw new NotImplementedException();
        }
    }
}
