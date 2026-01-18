namespace TaskTrackerAPI.Test.Controllers
{

    [TestFixture]
    public class TasksControllerTests
    {
        private TasksController _testClass;
        private AppDbContext _context;
        private IMapper _mapper;
        private ILogger<TasksController> _logger;

        [SetUp]
        public void SetUp()
        {
            _context = new AppDbContext(new DbContextOptions<AppDbContext>());
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<TasksController>>();
            _testClass = new TasksController(_context, _mapper, _logger);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TasksController(_context, _mapper, _logger);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new TasksController(default(AppDbContext), _mapper, _logger));
        }

        [Test]
        public void CannotConstructWithNullMapper()
        {
            Assert.Throws<ArgumentNullException>(() => new TasksController(_context, default(IMapper), _logger));
        }

        [Test]
        public void CannotConstructWithNullLogger()
        {
            Assert.Throws<ArgumentNullException>(() => new TasksController(_context, _mapper, default(ILogger<TasksController>)));
        }

        [Test]
        public async Task CanCallGetTasks()
        {
            // Arrange
            var q = "TestValue1327106022";
            var sort = "TestValue762648607";

            // Act
            var result = await _testClass.GetTasks(q, sort);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetTasksWithInvalidQ(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetTasks(value, "TestValue1981637728"));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetTasksWithInvalidSort(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetTasks("TestValue921387268", value));
        }

        [Test]
        public async Task CanCallGetTask()
        {
            // Arrange
            var id = 248321210;

            // Act
            var result = await _testClass.GetTask(id);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallCreateTask()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                Title = "TestValue1297768040",
                Description = "TestValue1406080535",
                Status = TaskStatus.Done,
                Priority = Priority.High,
                DueDate = DateTime.UtcNow
            };

            // Act
            var result = await _testClass.CreateTask(createTaskDto);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateTaskWithNullCreateTaskDto()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateTask(default(CreateTaskDto)));
        }

        [Test]
        public async Task CanCallUpdateTask()
        {
            // Arrange
            var id = 2017155293;
            var updateTaskDto = new UpdateTaskDto
            {
                Title = "TestValue1438617696",
                Description = "TestValue1398931281",
                Status = new TaskStatus?(),
                Priority = new Priority?(),
                DueDate = DateTime.UtcNow
            };

            // Act
            var result = await _testClass.UpdateTask(id, updateTaskDto);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallUpdateTaskWithNullUpdateTaskDto()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.UpdateTask(755213639, default(UpdateTaskDto)));
        }

        [Test]
        public async Task CanCallDeleteTask()
        {
            // Arrange
            var id = 1071831454;

            // Act
            var result = await _testClass.DeleteTask(id);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallMarkAsComplete()
        {
            // Arrange
            var id = 1710823683;

            // Act
            var result = await _testClass.MarkAsComplete(id);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallMarkAsIncomplete()
        {
            // Arrange
            var id = 1124854060;

            // Act
            var result = await _testClass.MarkAsIncomplete(id);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallGetTasksByPriority()
        {
            // Arrange
            var priority = Priority.High;

            // Act
            var result = await _testClass.GetTasksByPriority(priority);

            // Assert
            Assert.Fail("Create or modify test");
        }
    }
}