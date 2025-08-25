using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Models.API.Response;
using JobApplicationTracker.Common.Enums;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Services;
using Moq;

namespace JobApplicationTracker.Tests.Unit
{
    public class JobsServiceTests
    {
        private Mock<IJobRepository> _jobRepoMock = null!;
        private JobsService _service = null!;
        private readonly Guid _userId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _jobRepoMock = new Mock<IJobRepository>(MockBehavior.Strict);
            _service = new JobsService(_jobRepoMock.Object);
        }

        [Test]
        public async Task GetAllUserJobs_ReturnsPagedAndFiltered()
        {
            var data = new List<Job>
            {
                new Job { Id = Guid.NewGuid(), ApplicationUserId = _userId, Company = "A", Position = "Dev", Status = JobStatus.Applied, ApplicationDate = DateTime.UtcNow.AddDays(-2) },
                new Job { Id = Guid.NewGuid(), ApplicationUserId = _userId, Company = "B", Position = "QA", Status = JobStatus.Rejected, ApplicationDate = DateTime.UtcNow.AddDays(-1) },
                new Job { Id = Guid.NewGuid(), ApplicationUserId = _userId, Company = "C", Position = "DevOps", Status = JobStatus.Applied, ApplicationDate = DateTime.UtcNow }
            }.AsQueryable();

            _jobRepoMock.Setup(r => r.QueryByUser(_userId)).Returns(data);

            var query = new GetUserJobsQuery { Status = JobStatus.Applied, Page = 1, PageSize = 10 };
            var result = await _service.GetAllUserJobs(_userId, query);

            Assert.That(result.Total, Is.EqualTo(2));
            Assert.That(result.Jobs.Count, Is.EqualTo(2));
            Assert.That(result.Jobs.First().Company, Is.EqualTo("C"));
        }

        [Test]
        public async Task CreateJob_PersistsAndReturnsDto()
        {
            _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<Job>())).Returns(Task.CompletedTask).Verifiable();

            var req = new JobRequestModel
            {
                Company = "Test Co",
                Position = "Engineer",
                Status = JobStatus.Applied,
                ApplicationDate = DateTime.UtcNow,
                Notes = "N",
                Salary = 100000,
                Contact = "hr@test.co"
            };

            var dto = await _service.CreateJob(req, _userId);

            _jobRepoMock.Verify();
            Assert.That(dto.Company, Is.EqualTo("Test Co"));
            Assert.That(dto.Position, Is.EqualTo("Engineer"));
            Assert.That(dto.Salary, Is.EqualTo(100000));
        }

        [Test]
        public void UpdateJob_NotOwner_Throws()
        {
            var job = new Job { Id = Guid.NewGuid(), ApplicationUserId = Guid.NewGuid() };
            _jobRepoMock.Setup(r => r.GetByIdAsync(job.Id)).ReturnsAsync(job);

            var req = new UpdateJobRequestModel { JobId = job.Id, Company = "X", Position = "Y", Status = JobStatus.Applied, ApplicationDate = DateTime.UtcNow };

            Assert.ThrowsAsync<Exception>(async () => await _service.UpdateJob(req, _userId));
        }

        [Test]
        public void DeleteJob_NotFound_Throws()
        {
            var id = Guid.NewGuid();
            _jobRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Job)null);

            Assert.ThrowsAsync<Exception>(async () => await _service.DeleteJob(id, _userId));
        }
    }
}


