using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Server1Event;
using Moq;
using NServiceBus.Logging;
using NServiceBus.Testing;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue.UnitTest.Server1Event
{
    public class ServerHandlerTest
    {
        private readonly Mock<IBaseRepository> _mockBaseRepository;
        private readonly ServerHandler _serverHandler;
        private readonly TestableMessageHandlerContext _context;

        static StringBuilder logStatements = new StringBuilder();

        public ServerHandlerTest()
        {
            InitializeLogManager();

            _mockBaseRepository = new Mock<IBaseRepository>();

            _serverHandler = new ServerHandler(_mockBaseRepository.Object);

            _context = new TestableMessageHandlerContext();
        }


        #region Setups 

        private Task HandleSetup()
        {
            _mockBaseRepository.Setup(s => s.Add(It.IsAny<SomeEntity>()))
                .Returns(Task.FromResult(1));

            return Task.CompletedTask;
        }

        private Task RepositoryGetObjectMethodSetup()
        {
            IQueryable<MyEntity> mocks = MockEntityDetail().AsQueryable();

            _mockBaseRepository.Setup(s
                    => s.GetObject(It.IsAny<Expression<Func<MyEntity, bool>>>(), It.IsAny<string>()))
                .Returns<Expression<Func<MyEntity, bool>>, string>((predicate, include)
                        => mocks.FirstOrDefault(predicate));

            return Task.CompletedTask;
        }

        private Task RepositoryGetWithIncludeMethodSetup()
        {
            IQueryable<MyEntity> mocks = MockEntityDetail().AsQueryable();

            _mockBaseRepository.Setup(s
                    => s.GetWithInclude(It.IsAny<Expression<Func<MyEntity, bool>>>(), It.IsAny<string>()))
                .Returns<Expression<Func<MyEntity, bool>>, string>((predicate, include)
                        => mocks.Where(predicate));

            return Task.CompletedTask;
        }

        private Task RepositoryGetWithIncludeMethodSetup()
        {
            var mocks = MockEntityDetail()
                .AsQueryable();

            _mockBaseRepository.Setup(s
                    => s.GetObjectAsync(It.IsAny<Expression<Func<MyEntity, bool>>>()))
                .Returns<Expression<Func<MyEntity, bool>>>(predicate
                        => Task.FromResult(mocks.FirstOrDefault(predicate)));

            return Task.CompletedTask;
        }

        #endregion End Setups 

        #region Mocks

        private PpsnAllocation MockMessage() => new MockMessage
        {
            Id = 1
        };

        private Task MockAppSettings()
        {
            // Properties need to be virtual because we are mocking a concrete class
            _appSettings.SetupGet(s => s.Folder).Returns("Test");

            return Task.CompletedTask;

            // IOptions interface
            //_appSettings.SetupGet(s => s.Value).Returns(new AppSettings { Id = 3 });
        }

        #endregion End Mocks

        #region LogManager

        private Task InitializeLogManager()
        {
            logStatements.Clear();

            LogManager.Use<TestingLoggerFactory>()
                .WriteTo(new StringWriter(logStatements));

            return Task.CompletedTask;
        }

        public static string LogStatements => logStatements.ToString();

        #endregion End LogManager
    }
}
