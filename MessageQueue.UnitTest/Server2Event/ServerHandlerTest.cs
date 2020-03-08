using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using Moq;
using NServiceBus.Logging;
using NServiceBus.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessageQueue.UnitTest.Server2Event
{
    public class ServerHandlerTest
    {
        #region Fields 

        private readonly Mock<IBaseRepository> _mockBaseRepository;
        private readonly ServerHandler _serverHandler;
        private readonly TestableMessageHandlerContext _context;

        static StringBuilder logStatements = new StringBuilder();

        #endregion End Fields 

        #region Constructor

        public ServerHandlerTest()
        {
            InitializeLogManager();

            _mockBaseRepository = new Mock<IBaseRepository>();

            _serverHandler = new ServerHandler(_mockBaseRepository.Object);

            _context = new TestableMessageHandlerContext();
        }

        #endregion End Constructor

        #region Setups 

        private Task RepositorySetup()
        {
            _mockBaseRepository.Setup(s => s.Add(It.IsAny<TrackerEntity>()))
                .Returns(Task.FromResult(1));

            IQueryable<TrackerEntity> mocks = MockTrackerEntity.AsQueryable();

            _mockBaseRepository.Setup(s
                    => s.GetObjectWithInclude(It.IsAny<Expression<Func<TrackerEntity, bool>>>(), It.IsAny<string>()))
                .Returns<Expression<Func<TrackerEntity, bool>>, string>((predicate, include)
                        => Task.FromResult(mocks.FirstOrDefault(predicate)));

            _mockBaseRepository.Setup(s
                    => s.Get(It.IsAny<Expression<Func<TrackerEntity, bool>>>(), It.IsAny<string>()))
                .Returns<Expression<Func<TrackerEntity, bool>>, string>((predicate, include)
                        => mocks.Where(predicate));

            _mockBaseRepository.Setup(s
                    => s.GetObjectAsync(It.IsAny<Expression<Func<TrackerEntity, bool>>>()))
                .Returns<Expression<Func<TrackerEntity, bool>>>(predicate
                        => Task.FromResult(mocks.FirstOrDefault(predicate)));

            return Task.CompletedTask;
        }

        #endregion End Setups 

        #region Mocks

        private MessageEventEntity MockMessage => new MessageEventEntity
        {
            Id = 1,
            Description = "123"
        };

        private IEnumerable<TrackerEntity> MockTrackerEntity
            => new List<TrackerEntity>
            {
                new TrackerEntity
                {
                    Id = 1,
                    ProjectName = "Server1Handler"
                },
                new TrackerEntity
                {
                    Id = 2,
                    ProjectName = "Server2Handler"
                },
            };

        private Task MockAppSettings()
        {
            // Properties need to be virtual because we are mocking a concrete class
            //_appSettings.SetupGet(s => s.Folder).Returns("Test");

            // IOptions interface
            //_appSettings.SetupGet(s => s.Value).Returns(new AppSettings { Id = 3 });

            return Task.CompletedTask;
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

        #region Tests

        [Fact]
        public async Task When_Handle_Receive_Message_Should_Log_Correctly()
        {
            await RepositorySetup();

            await _serverHandler.Handle(MockMessage, _context).ConfigureAwait(false);

            var expectedLog = $"Message {MockMessage.Id} received " +
                $"at MessageQueue.Server2Event.ServerHandler";

            Assert.Contains(expectedLog, LogStatements);
        }

        [Fact]
        public async Task When_Handle_Should_Execute_Without_Exception()
        {
            await RepositorySetup();

            var exception = await Record.ExceptionAsync(()
                => _serverHandler.Handle(MockMessage, _context));

            Assert.Null(exception);
        }

        #endregion End Tests
    }
}
