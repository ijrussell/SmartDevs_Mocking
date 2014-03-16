using System;
using NSubstitute;
using NUnit.Framework;

namespace MockingDemo.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        [Test]
        public void get_returns_an_existing_user()
        {
            var dataService = Substitute.For<IUserDataService>();

            var email = "ian@test.com";
            var user = new User(Guid.NewGuid(), "fred", email);

            dataService.GetByEmail(email).Returns(user);

            var userService = new UserService(dataService);

            var result = userService.Get(email);

            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public void get_returns_an_non_existing_user()
        {
            IUserDataService dataService = new StubNullUserDataService();

            var userService = new UserService(dataService);

            var email = "ian@test.com";

            var result = userService.Get(email);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void user_who_doesnt_exist_is_created()
        {
            IUserDataService dataService = new StubNullCreateUserDataService();

            var userService = new UserService(dataService);

            var guid = Guid.NewGuid();

            GuidProvider.Current = new FakeGuidProvider(guid);

            var name = "ian";
            var email = "ian@test.com";

            var result = userService.Create(name, email);

            Assert.That(result.Id, Is.EqualTo(guid));
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.Email, Is.EqualTo(email));

            GuidProvider.ResetToDefault();
        }

        [Test]
        public void user_who_does_exist_is_not_created()
        {
            IUserDataService dataService = new StubUserExistsDataService();

            var userService = new UserService(dataService);

            var name = "ian";
            var email = "ian@test.com";

            Assert.Throws<Exception>(() => userService.Create(name, email));
        }

        [Test]
        public void user_is_deleted()
        {
            var dataService = Substitute.For<IUserDataService>();

            var email = "ian@test.com";

            var userService = new UserService(dataService);
            
            userService.Delete(email);
            
            dataService.Received().Delete(email);
        }
    }

    public class StubUserDataService : IUserDataService
    {
        public User GetByEmail(string email)
        {
            return new User(Guid.NewGuid(), "fred", email);
        }

        public User Create(Guid id, string name, string email)
        {
            throw new NotImplementedException();
        }

        public void Delete(string email)
        {
            throw new NotImplementedException();
        }
    }
    
    public class StubNullUserDataService : IUserDataService
    {
        public User GetByEmail(string email)
        {
            return null;
        }

        public User Create(Guid id, string name, string email)
        {
            throw new NotImplementedException();
        }

        public void Delete(string email)
        {
            throw new NotImplementedException();
        }
    }

    public class StubNullCreateUserDataService : IUserDataService
    {
        public User GetByEmail(string email)
        {
            return null;
        }

        public User Create(Guid id, string name, string email)
        {
            return new User(id, name, email);
        }

        public void Delete(string email)
        {
            throw new NotImplementedException();
        }
    }
    
    public class StubUserExistsDataService : IUserDataService
    {
        public User GetByEmail(string email)
        {
            return new User(Guid.NewGuid(), "fred", email);
        }

        public User Create(Guid id, string name, string email)
        {
            throw new NotImplementedException();
        }

        public void Delete(string email)
        {
            throw new NotImplementedException();
        }
    }

    public class MockUserDataService : IUserDataService
    {
        public User GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public User Create(Guid id, string name, string email)
        {
            throw new NotImplementedException();
        }

        public void Delete(string email)
        {
            DeletedHasbeenCalled = true;
        }

        public bool DeletedHasbeenCalled { get; set; }
    }
}