using Microsoft.VisualStudio.TestTools.UnitTesting;
using CegautokAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        private FlottaContext _context;
        private UserController _controller;
        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<FlottaContext>()
                    .UseInMemoryDatabase(databaseName: "TestDB")
                    .Options;

            _context = new FlottaContext(options);

            User ujUser = new User()
            {
                Id = 3,
                Name = "Teszt Imre",
                LoginName = "testimre",
                Active = true,
                Address = "1234 Monor, Kalapács utca 4",
                Email = "ti@gmail.com",
                Hash = "hash",
                Salt = "salt",
                Image = "missing.jpg",
                Phone = "9516284",
                Permission = 1,
                PermissionNavigation = new Privilege()
                {
                    Id = 1,
                    Name = "User",
                    Level = 1,
                    Users = null
                }
            };

            if (!_context.Users.Any(u => u.Id == 3))
            {
                _context.Users.Add(ujUser);
                _context.SaveChanges();
            }
            //Tesztelendő controller példányának előkészítése --> eredménye a _controller
            _controller = new UserController(_context);
        }
        [TestMethod()]
        public void GetUsersTest()
        {
            var result = _controller.GetUsers();
            Assert.IsInstanceOfType<OkObjectResult>(result);
        }

        [TestMethod()]
        public void GetUserByIdTest_Missing()
        {
            //Arrange
            int id = 3;

            //Act
            var result = _controller.GetUserById(id);


            //Assert
            Assert.IsNotNull(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            //Assert.IsInstanceOfType(result, typeof(User));
            var data = okResult.Value as User;
            Assert.IsNotNull(data);
            //tényleges adattartalom ellenőrzése
            Assert.AreEqual(id, data.Id);
            Assert.AreEqual("Teszt Imre", data.Name);
        }
        [TestMethod()]
        public void GetUserByIdTest_Success()
        {
            //Arrange
            int id = -1;

            //Act
            var result = _controller.GetUserById(id);


            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
        }

        [TestMethod()]
        public void PostUserTest_Success()
        {
            //Arrange
            User ujUser = new User()
            {
                Id = 3,
                Name = "Teszt Elek",
                LoginName = "testelek",
                Active = true,
                Address = "1234 Monor, Kalapács utca 4",
                Email = "tE@gmail.com",
                Hash = "hash",
                Salt = "salt",
                Image = "missing.jpg",
                Phone = "9516284",
                Permission = 1,
                PermissionNavigation = null
            };
            //Act
            var result = _controller.PostUser(ujUser);
            //Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

        }

        [TestMethod()]
        public void PostUserTest_Failed()
        {
            //Arrange
            User ujUser = new User()
            {
                Id = 3,
                Name = "Teszt Elek",
                LoginName = "testelek",
                Active = true,
                Address = "1234 Monor, Kalapács utca 4",
                Email = "tE@gmail.com",
                Hash = "hash",
                Salt = "salt",
                Image = "missing.jpg",
                Phone = "9516284",
                Permission = 1,
                PermissionNavigation = null
            };
            //Act
            var result = _controller.PostUser(ujUser);
            //Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            var okResult = result as BadRequestObjectResult;
            Assert.IsNotNull(okResult);

        }


        [TestMethod()]
        public void PutUserTest()
        {
            //Arrange
            User celszemely = new User()
            {
                Id = 3,
                Name = "Teszt Imre",
                LoginName = "testimre",
                Active = true,
                Address = "1234 Monor, Kalapács utca 4",
                Email = "tE@gmail.com",
                Hash = "hash",
                Salt = "salt",
                Image = "missing.jpg",
                Phone = "9516284",
                Permission = 1,
                PermissionNavigation = null
            };
            //Act
            var result = _controller.PutUser(celszemely);
            //Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
        }
        [TestMethod()]
        public void PutUserTest_Failed()
        {
            //Arrange
            User celszemely = new User()
            {
                Id = 3,
                Name = "Teszt Imre",
                LoginName = "testimre",
                Active = true,
                Address = "1234 Monor, Kalapács utca 4",
                Email = "tE@gmail.com",
                Hash = "hash",
                Salt = "salt",
                Image = "missing.jpg",
                Phone = "9516284",
                Permission = 1,
                PermissionNavigation = null
            };
            //Act
            var result = _controller.PutUser(celszemely);
            //Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
        }
    }
}