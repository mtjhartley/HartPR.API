using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;
using HartPR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HartPR.Controllers
{
    [Route("api/players")]
    public class PlayersController : Controller
    {
        private IHartPRRepository _hartPRRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public PlayersController(IHartPRRepository hartPRRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _hartPRRepository = hartPRRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetPlayers")]
        public IActionResult GetAuthors(PlayersResourceParameters playersResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PlayerDto, Player>
               (playersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<PlayerDto>
                (playersResourceParameters.Fields))
            {
                return BadRequest();
            }

            var playersFromRepo = _hartPRRepository.GetPlayers(playersResourceParameters);

            var players = Mapper.Map<IEnumerable<PlayerDto>>(playersFromRepo);

            var paginationMetadata = new
            {
                totalCount = playersFromRepo.TotalCount,
                pageSize = playersFromRepo.PageSize,
                currentPage = playersFromRepo.CurrentPage,
                totalPages = playersFromRepo.TotalPages,
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForPlayers(playersResourceParameters,
                playersFromRepo.HasNext, playersFromRepo.HasPrevious);

            var shapedPlayers = players.ShapeData(playersResourceParameters.Fields);

            var shapedPlayersWithLinks = shapedPlayers.Select(player =>
            {
                var playerAsDictionary = player as IDictionary<string, object>;
                var playerLinks = CreateLinksForPlayer(
                    (Guid)playerAsDictionary["Id"], playersResourceParameters.Fields);

                playerAsDictionary.Add("links", playerLinks);

                return playerAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedPlayersWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        private string CreatePlayersResourceUri(
            PlayersResourceParameters playersResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetPlayers",
                      new
                      {
                          fields = playersResourceParameters.Fields,
                          orderBy = playersResourceParameters.OrderBy,
                          searchQuery = playersResourceParameters.SearchQuery,
                          state = playersResourceParameters.State,
                          pageNumber = playersResourceParameters.PageNumber - 1,
                          pageSize = playersResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetPlayers",
                      new
                      {
                          fields = playersResourceParameters.Fields,
                          orderBy = playersResourceParameters.OrderBy,
                          searchQuery = playersResourceParameters.SearchQuery,
                          state = playersResourceParameters.State,
                          pageNumber = playersResourceParameters.PageNumber + 1,
                          pageSize = playersResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetPlayers",
                    new
                    {
                        fields = playersResourceParameters.Fields,
                        orderBy = playersResourceParameters.OrderBy,
                        searchQuery = playersResourceParameters.SearchQuery,
                        state = playersResourceParameters.State,
                        pageNumber = playersResourceParameters.PageNumber,
                        pageSize = playersResourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForPlayer(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetPlayer", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetPlayer", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeletePlayer", new { id = id }),
              "delete_player",
              "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForPlayers(
            PlayersResourceParameters playersResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreatePlayersResourceUri(playersResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreatePlayersResourceUri(playersResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreatePlayersResourceUri(playersResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }

        [HttpGet("{id}", Name = "GetPlayer")]
        public IActionResult GetPlayer(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<PlayerDto>
              (fields))
            {
                return BadRequest();
            }

            var playerFromRepo = _hartPRRepository.GetPlayer(id);

            if (playerFromRepo == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<PlayerDto>(playerFromRepo);

            var links = CreateLinksForPlayer(id, fields);

            var linkedResourceToReturn = author.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreatePlayer")]
        public IActionResult CreatePlayer([FromBody] PlayerForCreationDto player)
        {
            if (player == null)
            {
                return BadRequest();
            }

            var playerEntity = Mapper.Map<Player>(player);

            _hartPRRepository.AddPlayer(playerEntity);

            if (!_hartPRRepository.Save())
            {
                throw new Exception("Creating an player failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            var playerToReturn = Mapper.Map<PlayerDto>(playerEntity);

            return CreatedAtRoute("GetPlayer",
                new { id = playerToReturn.Id },
                playerToReturn);
        }
    }
}
