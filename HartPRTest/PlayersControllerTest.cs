using System;
using System.Collections.Generic;
using System.Linq;
using HartPR.Controllers;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;
using HartPR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HartPRTest
{
    [TestClass]
    public class PlayersContollerTest
    {
        [TestInitialize]
        public void setup()
        {
            AutoMapper.Mapper.Reset();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                // players mapping
                cfg.CreateMap<HartPR.Entities.Player, HartPR.Models.PlayerDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"));
            });
        }

        [TestMethod]
        public void GetPlayers_Validation()
        {
            PlayersResourceParameters prp = new PlayersResourceParameters();
            Mock<IPropertyMappingService> mockPropertyMappingService = new Mock<IPropertyMappingService>(MockBehavior.Strict);
            mockPropertyMappingService.Setup(x => x.ValidMappingExistsFor<PlayerDto, Player>(It.IsAny<string>())).Returns(false);

            var controller = new PlayersController(null, null, mockPropertyMappingService.Object, null);
            var result = controller.GetPlayersFromTrueskillHistory(prp, "melee");

            var actual = result as BadRequestResult;

            Assert.AreEqual(400, actual.StatusCode);

            mockPropertyMappingService.VerifyAll();
        }

        [TestMethod]
        public void GetPlayers_()
        {
            PlayersResourceParameters prp = new PlayersResourceParameters();

            var players = new List<Player>()
            {
                new Player()
                {
                    Id = new Guid("76053df4-6687-4353-8937-b45556748abe"),
                    Tag = "Dz",
                    FirstName = "Mitch",
                    LastName = "Dzugan",
                    State = "CA",
                    Trueskill = 4200.69,
                    SggPlayerId = 98,
                    CreatedAt = new DateTimeOffset(new DateTime(2007, 6, 13)),
                    UpdatedAt = new DateTimeOffset(DateTime.Now)

                },
                new Player()
                {
                    Id = new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"),
                    Tag = "MILK$",
                    FirstName = "Gary",
                    LastName = "Mai",
                    State = "WA",
                    Trueskill = 2345.76,
                    SggPlayerId = 214,
                    CreatedAt = new DateTimeOffset(new DateTime(2007, 6, 14)),
                    UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
            };



            var queryablePlayers = players.AsQueryable();
            PagedList<Player> mockResult = PagedList<Player>.Create(queryablePlayers, 1, 2);


            Mock<IHartPRRepository> mockHartPRRepository = new Mock<IHartPRRepository>(MockBehavior.Strict);
            mockHartPRRepository.Setup(x => x.GetPlayersFromTrueskillHistory(prp, 0)).Returns(mockResult);

            Mock<IUrlHelper> mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<Object>())).Returns("someUrl");
            // mocking previous page
            //mockUrlHelper.Setup(x => x.Link("GetPlayers", new
            //{
            //    fields = prp.Fields,
            //    orderBy = prp.OrderBy,
            //    searchQuery = prp.SearchQuery,
            //    state = prp.State,
            //    pageNumber = prp.PageNumber - 1,
            //    pageSize = prp.PageSize
            //}));
            //mockUrlHelper.Setup(x => x.Link("GetPlayers", new
            //{
            //    fields = prp.Fields,
            //    orderBy = prp.OrderBy,
            //    searchQuery = prp.SearchQuery,
            //    state = prp.State,
            //    pageNumber = prp.PageNumber,
            //    pageSize = prp.PageSize
            //}));

            Mock<IPropertyMappingService> mockPropertyMappingService = new Mock<IPropertyMappingService>(MockBehavior.Strict);
            mockPropertyMappingService.Setup(x => x.ValidMappingExistsFor<PlayerDto, Player>(prp.OrderBy)).Returns(true);

            Mock<ITypeHelperService> mockTypeHelperService = new Mock<ITypeHelperService>(MockBehavior.Strict);
            mockTypeHelperService.Setup(x => x.TypeHasProperties<PlayerDto>(prp.Fields)).Returns(true);

            //Mock<HttpResponse> mockHttpResponse = new Mock<HttpResponse>(MockBehavior.Strict);
            //mockHttpResponse.Setup(r => r.Headers.Add(It.IsAny<string>(), It.IsAny<StringValues>()));//.Callback<string, string>((x, y) => mockHttpResponse.Object.Headers.Add(x, y));


            var controller = new PlayersController(mockHartPRRepository.Object, mockUrlHelper.Object, mockPropertyMappingService.Object, mockTypeHelperService.Object);
            var result = controller.GetPlayersFromTrueskillHistory(prp, "melee");

            //TODO: Cast and assert we're getting the response we expect.
            var actual = result as OkObjectResult;
            Assert.AreEqual(200, actual.StatusCode);

            mockHartPRRepository.VerifyAll();


        }
    }
}
