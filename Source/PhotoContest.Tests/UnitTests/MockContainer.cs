namespace PhotoContest.Tests.UnitTests
{
    using Data.Contracts;
    using Models;
    using Moq;

    public class MockContainer
    {
        public Mock<IRepository<User>> UsersRepositoryMock { get; set; }
    }
}
