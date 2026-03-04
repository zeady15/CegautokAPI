using Microsoft.VisualStudio.TestTools.UnitTesting;
using CegautokAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CegautokAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CegautokAPI.DTOs;
using Microsoft.EntityFrameworkCore.InMemory;

namespace CegautokAPI.Controllers.Tests
{
    [TestClass()]
    public class GepjarmuControllerTests
    {


        private FlottaContext _context;
        private JarmuController _controller;

        //Arrange
        [TestInitialize()]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<FlottaContext>().
                UseInMemoryDatabase(databaseName: "TestDB").Options;

            _context = new FlottaContext(options);

            //Tesztadatok
            var gepjarmu = new Gepjarmu()
            {
                Id = 1,
                Rendszam = "ABC-123",
                Marka = "Toyota",
                Tipus = "Corolla",
                Ulesek = 5
            };

            var gepjarmu2 = new Gepjarmu()
            {
                Id = 2,
                Rendszam = "BCD-234",
                Marka = "BMW",
                Tipus = "M2",
                Ulesek = 2
            };

            var sofor1 = new User
            {
                Id = 1,
                Name = "Kiss Pista",
                LoginName = "kisspista",
                Active = true,
                Address = "Budapest, Fő utca 1.",
                Email = "kp@email.com",
                Hash = "hash",
                Salt = "salt",
                Phone = "123456789",
                Image = "image.jpg",
                Permission = 1
            };

            var sofor2 = new User
            {
                Id = 2,
                Name = "Nagy Pista",
                LoginName = "nagypista",
                Active = true,
                Address = "Miskolc, Fő utca 1.",
                Email = "np@email.com",
                Hash = "hash1",
                Salt = "salt1",
                Phone = "123456789",
                Image = "nagyP.jpg",
                Permission = 2
            };

            _context.AddRange(
                new Kikuldottjarmu() { Id = 1, Gepjarmu = gepjarmu, Sofor = 1, SoforNavigation = sofor1, GepjarmuId = 1 },
                new Kikuldottjarmu() { Id = 2, Gepjarmu = gepjarmu2, Sofor = 2, SoforNavigation = sofor2, GepjarmuId = 2 });

            _context.SaveChanges();
            _controller = new JarmuController(_context);

        }

        [TestMethod()]
        public void GetSoforTest()
        {
            var result = _controller.GetSofor();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            //AZ adatok ellenőrzése
            var data = okResult.Value as List<SoforDTO>;
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count);

            // adattartalom ellenőrzése
            var first = data[0];
            Assert.AreEqual("Kiss Pista", first.SoforNev);
            Assert.AreEqual("ABC-123", first.Rendszam);
            Assert.AreEqual(1, first.Darab);

            var second = data[1];
            Assert.AreEqual("Nagy Pista", second.SoforNev);
            Assert.AreEqual("BCD-234", second.Rendszam);
            Assert.AreEqual(1, second.Darab);
        }


    }
}