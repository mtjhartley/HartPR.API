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
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace HartPR.Controllers
{
    [Authorize]
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

        // DEPRECATED WAY OF DOING IT AFTER MOVING TRUESKILL OUT OF THE PLAYER TABLE COMPLETELY
        //[AllowAnonymous]
        ////[HttpGet(Name = "GetPlayers")]
        //[HttpHead]
        //public IActionResult GetPlayers(PlayersResourceParameters playersResourceParameters)
        //{
        //    if (!_propertyMappingService.ValidMappingExistsFor<PlayerDto, Player>(playersResourceParameters.OrderBy))
        //    {
        //        return BadRequest();
        //    }

        //    if (!_typeHelperService.TypeHasProperties<PlayerDto>(playersResourceParameters.Fields))
        //    {
        //        return BadRequest();
        //    }

        //    var playersFromRepo = _hartPRRepository.GetPlayers(playersResourceParameters);

        //    var players = Mapper.Map<IEnumerable<PlayerDto>>(playersFromRepo);

        //    var paginationMetadata = new
        //    {
        //        totalCount = playersFromRepo.TotalCount,
        //        pageSize = playersFromRepo.PageSize,
        //        currentPage = playersFromRepo.CurrentPage,
        //        totalPages = playersFromRepo.TotalPages,
        //    };

        //    //TODO: Figure out exactly waht this is doing, check the pluralsight course
        //    //Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

        //    var links = CreateLinksForPlayers(playersResourceParameters, playersFromRepo.HasNext, playersFromRepo.HasPrevious);


        //    var shapedPlayers = players.ShapeData(playersResourceParameters.Fields);

        //    var shapedPlayersWithLinks = shapedPlayers.Select(player =>
        //    {
        //        var playerAsDictionary = player as IDictionary<string, object>;
        //        var playerLinks = CreateLinksForPlayer((Guid)playerAsDictionary["Id"], playersResourceParameters.Fields);

        //        playerAsDictionary.Add("links", playerLinks);

        //        return playerAsDictionary;
        //    });

        //    var linkedCollectionResource = new
        //    {
        //        value = shapedPlayersWithLinks,
        //        links = links
        //    };

        //    return Ok(linkedCollectionResource);
        //}

        [AllowAnonymous]
        [HttpGet("{game}", Name = "GetPlayers")]
        public IActionResult GetPlayersFromTrueskillHistory(PlayersResourceParameters playersResourceParameters, string game)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PlayerDto, Player>(playersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<PlayerDto>(playersResourceParameters.Fields))
            {
                return BadRequest();
            }

            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            //TODO: Add gameNum to this request! 
            var playersFromRepo = _hartPRRepository.GetPlayersFromTrueskillHistory(playersResourceParameters, gameNum);

            if (playersFromRepo == null)
            {
                return NotFound();
            }

            var players = Mapper.Map<IEnumerable<PlayerDto>>(playersFromRepo);

            //return Ok(players);

            var paginationMetadata = new
            {
                totalCount = playersFromRepo.TotalCount,
                pageSize = playersFromRepo.PageSize,
                currentPage = playersFromRepo.CurrentPage,
                totalPages = playersFromRepo.TotalPages,
            };

            //TODO: Figure out exactly waht this is doing, check the pluralsight course
            //Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForPlayers(playersResourceParameters, playersFromRepo.HasNext, playersFromRepo.HasPrevious);


            var shapedPlayers = players.ShapeData(playersResourceParameters.Fields);

            var shapedPlayersWithLinks = shapedPlayers.Select(player =>
            {
                var playerAsDictionary = player as IDictionary<string, object>;
                var playerLinks = CreateLinksForPlayer((Guid)playerAsDictionary["Id"], gameNum, playersResourceParameters.Fields);

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

        private string CreatePlayersResourceUri(PlayersResourceParameters playersResourceParameters, ResourceUriType type)
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

        private IEnumerable<LinkDto> CreateLinksForPlayer(Guid id, int gameNum, string fields)
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

            //links.Add(
            //  new LinkDto(_urlHelper.Link("DeletePlayer", new { id = id }),
            //  "delete_player",
            //  "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForPlayers(PlayersResourceParameters playersResourceParameters, bool hasNext, bool hasPrevious)
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

        // DEPRECATED WAY OF DOING IT AFTER MOVING TRUESKILL OUT OF THE PLAYER TABLE COMPLETELY
        //[AllowAnonymous]
        //[HttpGet("{id}", Name = "GetPlayer")]
        //public IActionResult GetPlayer(Guid id, [FromQuery] string fields)
        //{
        //    if (!_typeHelperService.TypeHasProperties<PlayerDto>
        //      (fields))
        //    {
        //        return BadRequest();
        //    }

        //    var playerFromRepo = _hartPRRepository.GetPlayer(id);

        //    if (playerFromRepo == null)
        //    {
        //        return NotFound();
        //    }

        //    var player = Mapper.Map<PlayerDto>(playerFromRepo);

        //    var links = CreateLinksForPlayer(id, fields);

        //    var linkedResourceToReturn = player.ShapeData(fields)
        //        as IDictionary<string, object>;

        //    linkedResourceToReturn.Add("links", links);

        //    return Ok(linkedResourceToReturn);
        //}

        [AllowAnonymous]
        [HttpGet("{game}/{id}", Name = "GetPlayer")]
        public IActionResult GetPlayerFromTrueskillHistory(Guid id, string game, [FromQuery] string fields)
        {
            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var playerFromRepo = _hartPRRepository.GetPlayerFromTrueskillHistory(id, gameNum);

            if (playerFromRepo == null)
            {
                return NotFound();
            }

            var player = Mapper.Map<PlayerDto>(playerFromRepo);

            //return Ok(player);

            var links = CreateLinksForPlayer(id, gameNum, fields);

            var linkedResourceToReturn = player.ShapeData(fields)
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

            if (!ModelState.IsValid)
            {
                // return 422 unprocessable entity
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var playerEntity = Mapper.Map<Player>(player);
            playerEntity.CreatedAt = new DateTimeOffset(DateTime.Now);
            playerEntity.UpdatedAt = new DateTimeOffset(DateTime.Now);

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

        [HttpDelete("{id}", Name = "DeletePlayer")]
        public IActionResult DeletePlayer(Guid id)
        {
            var playerFromRepo = _hartPRRepository.GetPlayer(id);
            if (playerFromRepo == null)
            {
                return NotFound();
            }

            _hartPRRepository.DeletePlayer(playerFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Deleting player {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePlayer(Guid id,
            [FromBody] PlayerForUpdateDto player)
        {
            if (player == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var playerFromRepo = _hartPRRepository.GetPlayer(id);

            if (playerFromRepo == null)
            {
                //Player upserting implementation
                var playerToAdd = Mapper.Map<Player>(player);
                playerToAdd.Id = id;

                _hartPRRepository.AddPlayer(playerToAdd, id);

                if (!_hartPRRepository.Save())
                {
                    throw new Exception($"Upserting player {id} failed on save.");
                }

                var playerToReturn = Mapper.Map<PlayerDto>(playerToAdd);

                return CreatedAtRoute("GetPlayer", new { id = playerToReturn.Id } ,playerToReturn);
            }

            //map 

            //apply update

            //map update dto back to entity
            Mapper.Map(player, playerFromRepo);
            playerFromRepo.UpdatedAt = new DateTimeOffset(DateTime.Now);

            _hartPRRepository.UpdatePlayer(playerFromRepo); //calling into this empty method 

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Updating player {id} failed on save.");
            }

            return NoContent();
        }
        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePlayer(Guid id,
            [FromBody] JsonPatchDocument<PlayerForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var playerFromRepo = _hartPRRepository.GetPlayer(id);
            if (playerFromRepo == null)
            {
                return NotFound();
            }

            var playerToPatch = Mapper.Map<PlayerForUpdateDto>(playerFromRepo);

            patchDoc.ApplyTo(playerToPatch, ModelState);

            TryValidateModel(playerToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(playerToPatch, playerFromRepo);
            playerFromRepo.UpdatedAt = new DateTimeOffset(DateTime.Now);

            _hartPRRepository.UpdatePlayer(playerFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Patching player {id} failed on save.");
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/sets", Name = "GetSetsForPlayer")]
        public IActionResult GetSetsForPlayer(Guid id, string game)
        {
            var playerFromRepo = _hartPRRepository.GetPlayer(id);

            if (playerFromRepo == null)
            {
                return NotFound();
            }

            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var setsForPlayerFromRepo = _hartPRRepository.GetSetsForPlayer(id, gameNum);

            return Ok(setsForPlayerFromRepo);
        }

        [AllowAnonymous]
        [HttpGet("{game}/head2head/{player1Id}/{player2Id}", Name = "GetHead2HeadBetweenPlayers")]
        public IActionResult GetHead2HeadBetweenPlayers(Guid player1Id, Guid player2Id, string game)
        {
            if (!_hartPRRepository.PlayerExists(player1Id) || !_hartPRRepository.PlayerExists(player2Id))
            {
                return NotFound();
            }

            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var setsBetweenPlayers = _hartPRRepository.GetSetsBetweenPlayers(player1Id, player2Id, gameNum);

            return Ok(setsBetweenPlayers);
        }

        [AllowAnonymous]
        [HttpOptions]
        public IActionResult GetPlayersOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS");
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/trueskillhistory", Name = "GetTrueskillHistoryForPlayer")]
        public IActionResult GetTrueskillHistoryForPlayer(Guid id, string game)
        {
            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var trueskillHistoryFromRepo = _hartPRRepository.GetTrueskillHistoryForPlayer(id, gameNum);

            if (trueskillHistoryFromRepo == null)
            {
                return NotFound();
            }

            return Ok(trueskillHistoryFromRepo);
        }


        [AllowAnonymous]
        [HttpGet("{game}/{id}/trueskillhistoryrecent", Name = "GetTrueskillHistoryRecentForPlayer")]
        public IActionResult GetMostRecentTrueskillForPlayer(Guid id, string game)
        {
            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var trueskillHistoryFromRepo = _hartPRRepository.GetMostRecentTrueskillForPlayer(id, gameNum);

            if (trueskillHistoryFromRepo == null)
            {
                return NotFound();
            }

            return Ok(trueskillHistoryFromRepo);
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/tournaments", Name = "GetTournamentsForPlayer")]
        public IActionResult GetTournamentsForPlayer(Guid id, string game)
        {
            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            var tournamentsForPlayerFromRepo = _hartPRRepository.GetTournamentsForPlayer(id, gameNum);

            if (tournamentsForPlayerFromRepo == null)
            {
                return NotFound();
            }

            var tournaments = Mapper.Map<IEnumerable<TournamentDto>>(tournamentsForPlayerFromRepo);

            return Ok(tournaments);
        }


    }
}
