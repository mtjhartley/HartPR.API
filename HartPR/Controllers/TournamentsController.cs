﻿using AutoMapper;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;
using HartPR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Controllers
{
    [Authorize]
    [Route("api/tournaments")]
    public class TournamentsController : Controller
    {
        private IHartPRRepository _hartPRRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public TournamentsController(IHartPRRepository hartPRRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _hartPRRepository = hartPRRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        //[AllowAnonymous]
        //[HttpGet(Name = "GetTournaments")]
        //[HttpHead]
        //public IActionResult GetTournaments(TournamentsResourceParameters tournamentsResourceParameters)
        //{
        //    if (!_propertyMappingService.ValidMappingExistsFor<TournamentDto, Tournament>
        //        (tournamentsResourceParameters.OrderBy))
        //    {
        //        return BadRequest();
        //    }

        //    if (!_typeHelperService.TypeHasProperties<TournamentDto>
        //        (tournamentsResourceParameters.Fields))
        //    {
        //        return BadRequest();
        //    }

        //    var tournamentsFromRepo = _hartPRRepository.GetTournaments(tournamentsResourceParameters);

        //    var tournaments = Mapper.Map<IEnumerable<TournamentDto>>(tournamentsFromRepo);

        //    var paginationMetadata = new
        //    {
        //        totalCount = tournamentsFromRepo.TotalCount,
        //        pageSize = tournamentsFromRepo.PageSize,
        //        currentPage = tournamentsFromRepo.CurrentPage,
        //        totalPages = tournamentsFromRepo.TotalPages,
        //    };

        //    Response.Headers.Add("X-Pagination",
        //        Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

        //    var links = CreateLinksForTournaments(tournamentsResourceParameters,
        //        tournamentsFromRepo.HasNext, tournamentsFromRepo.HasPrevious);

        //    var shapedTournaments = tournaments.ShapeData(tournamentsResourceParameters.Fields);

        //    var shapedTournamentsWithLinks = shapedTournaments.Select(tournament =>
        //    {
        //        var tournamentAsDictionary = tournament as IDictionary<string, object>;
        //        var tournamentLinks = CreateLinksForTournament(
        //            (Guid)tournamentAsDictionary["Id"], tournamentsResourceParameters.Fields);

        //        tournamentAsDictionary.Add("links", tournamentLinks);

        //        return tournamentAsDictionary;
        //    });

        //    var linkedCollectionResource = new
        //    {
        //        value = shapedTournamentsWithLinks,
        //        links = links
        //    };

        //    return Ok(linkedCollectionResource);
        //}

        [AllowAnonymous]
        [HttpGet("{game}", Name = "GetTournamentsForGame")]
        public IActionResult GetTournamentsForGame(TournamentsResourceParameters tournamentsResourceParameters, string game)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<TournamentDto, Tournament>
                (tournamentsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<TournamentDto>
                (tournamentsResourceParameters.Fields))
            {
                return BadRequest();
            }

            int gameNum = GetGameFromUrl(game);
            if (gameNum == -1)
            {
                return BadRequest();
            }

            var tournamentsFromRepo = _hartPRRepository.GetTournamentsForGame(tournamentsResourceParameters, gameNum);

            var tournaments = Mapper.Map<IEnumerable<TournamentDto>>(tournamentsFromRepo);

            foreach (TournamentDto tourney in tournaments)
            {
                int attendees = _hartPRRepository.GetEntrantsForTournament(tourney.Id).Count();
                tourney.Attendees = attendees;
            }

            var paginationMetadata = new
            {
                totalCount = tournamentsFromRepo.TotalCount,
                pageSize = tournamentsFromRepo.PageSize,
                currentPage = tournamentsFromRepo.CurrentPage,
                totalPages = tournamentsFromRepo.TotalPages,
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForTournaments(tournamentsResourceParameters,
                tournamentsFromRepo.HasNext, tournamentsFromRepo.HasPrevious);

            var shapedTournaments = tournaments.ShapeData(tournamentsResourceParameters.Fields);

            var shapedTournamentsWithLinks = shapedTournaments.Select(tournament =>
            {
                var tournamentAsDictionary = tournament as IDictionary<string, object>;
                var tournamentLinks = CreateLinksForTournament(
                    (Guid)tournamentAsDictionary["Id"], gameNum, tournamentsResourceParameters.Fields);

                tournamentAsDictionary.Add("links", tournamentLinks);

                return tournamentAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedTournamentsWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        private int GetGameFromUrl(string game)
        {
            int gameNum;
            if (Enum.TryParse(game, out Games gameValue))
            {
                if (Enum.IsDefined(typeof(Games), gameValue))
                {
                    gameNum = (int)gameValue;
                    return gameNum;
                }
                else
                {
                    return gameNum = -1;
                }
            }
            else
            {
                return gameNum = -1;
            }
        }

        private string CreateTournamentsResourceUri(
            TournamentsResourceParameters tournamentResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetTournamentsForGame",
                      new
                      {
                          fields = tournamentResourceParameters.Fields,
                          orderBy = tournamentResourceParameters.OrderBy,
                          searchQuery = tournamentResourceParameters.SearchQuery,
                          pageNumber = tournamentResourceParameters.PageNumber - 1,
                          pageSize = tournamentResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetTournamentsForGame",
                      new
                      {
                          fields = tournamentResourceParameters.Fields,
                          orderBy = tournamentResourceParameters.OrderBy,
                          searchQuery = tournamentResourceParameters.SearchQuery,
                          pageNumber = tournamentResourceParameters.PageNumber + 1,
                          pageSize = tournamentResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetTournamentsForGame",
                    new
                    {
                        fields = tournamentResourceParameters.Fields,
                        orderBy = tournamentResourceParameters.OrderBy,
                        searchQuery = tournamentResourceParameters.SearchQuery,
                        pageNumber = tournamentResourceParameters.PageNumber,
                        pageSize = tournamentResourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForTournaments(
            TournamentsResourceParameters tournamentResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateTournamentsResourceUri(tournamentResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateTournamentsResourceUri(tournamentResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateTournamentsResourceUri(tournamentResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
        private IEnumerable<LinkDto> CreateLinksForTournament(Guid id, int gameNum, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetTournament", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetTournament", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            //links.Add(
            //  new LinkDto(_urlHelper.Link("DeleteTournament", new { id = id }),
            //  "delete_tournament",
            //  "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTournament(Guid id, string game, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetTournament", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetTournament", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            //links.Add(
            //  new LinkDto(_urlHelper.Link("DeleteTournament", new { id = id }),
            //  "delete_tournament",
            //  "DELETE"));

            return links;
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}", Name = "GetTournament")]
        public IActionResult GetTournament(Guid id, [FromQuery] string fields, string game)
        {
            int gameNum = GetGameFromUrl(game);
            if (gameNum == -1)
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<TournamentDto>
              (fields))
            {
                return BadRequest();
            }

            var tournamentFromRepo = _hartPRRepository.GetTournament(id);

            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            var tournament = Mapper.Map<TournamentDto>(tournamentFromRepo);

            var links = CreateLinksForTournament(id, gameNum, fields);

            var linkedResourceToReturn = tournament.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateTournament")]
        public IActionResult CreateTournament([FromBody] TournamentForCreationDto tournament)
        {
            if (tournament == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var tournamentEntity = Mapper.Map<Tournament>(tournament);
            tournamentEntity.CreatedAt = new DateTimeOffset(DateTime.Now);
            tournamentEntity.UpdatedAt = new DateTimeOffset(DateTime.Now);

            _hartPRRepository.AddTournament(tournamentEntity);

            if (!_hartPRRepository.Save())
            {
                throw new Exception("Creating a tournament failed on save.");
            }

            var tournamentToReturn = Mapper.Map<TournamentDto>(tournamentEntity);

            return CreatedAtRoute("GetTournament",
                new { id = tournamentToReturn.Id },
                tournamentToReturn);
        }

        [HttpDelete("{id}", Name = "DeleteTournament")]
        public IActionResult DeleteTournament(Guid id)
        {
            var tournamentFromRepo = _hartPRRepository.GetTournament(id);
            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            _hartPRRepository.DeleteTournament(tournamentFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Deleting tournament {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTournament(Guid id,
            [FromBody] TournamentForUpdateDto tournament)
        {
            if (tournament == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var tournamentFromRepo = _hartPRRepository.GetTournament(id);

            if (tournamentFromRepo == null)
            {
                //not seeing a reason to add upsert implementation to tournament currently.
                return NotFound();
            }

            //map

            //apply update

            //map update dto back to entity

            Mapper.Map(tournament, tournamentFromRepo); //TODO: Check if this needs to be configured
            tournamentFromRepo.UpdatedAt = new DateTimeOffset(DateTime.Now);

            _hartPRRepository.UpdateTournament(tournamentFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Updating tournament {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateTournament(Guid id,
            [FromBody] JsonPatchDocument<TournamentForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var tournamentFromRepo = _hartPRRepository.GetTournament(id);
            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            var tournamentToPatch = Mapper.Map<TournamentForUpdateDto>(tournamentFromRepo);

            patchDoc.ApplyTo(tournamentToPatch, ModelState);

            TryValidateModel(tournamentToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(tournamentToPatch, tournamentFromRepo);
            tournamentFromRepo.UpdatedAt = new DateTimeOffset(DateTime.Now);

            _hartPRRepository.UpdateTournament(tournamentFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Patching tournament {id} failed on save.");
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/sets", Name = "GetSetsForTournament")]
        public IActionResult GetSetsForTournament(Guid id)
        {
            var tournamentFromRepo = _hartPRRepository.GetTournament(id);

            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            var setsForTournamentFromRepo = _hartPRRepository.GetSetsForTournament(id);

            return Ok(setsForTournamentFromRepo);
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/players", Name = "GetPlayersForTournament")]
        public IActionResult GetPlayersForTournament(Guid id)
        {
            var tournamentFromRepo = _hartPRRepository.GetTournament(id);

            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            var playersForTournamentFromRepo = _hartPRRepository.GetPlayersForTournament(id);

            var players = Mapper.Map<IEnumerable<PlayerDto>>(playersForTournamentFromRepo);

            return Ok(players);
        }

        [AllowAnonymous]
        [HttpGet("{game}/{id}/entrants", Name = "GetEntrantsForTournament")]
        public IActionResult GetEntrantsForTournament(Guid id)
        {
            var tournamentFromRepo = _hartPRRepository.GetTournament(id);

            if (tournamentFromRepo == null)
            {
                return NotFound();
            }

            var entrants = _hartPRRepository.GetEntrantsForTournament(id);

            //var players = Mapper.Map<IEnumerable<PlayerDto>>(playersForTournamentFromRepo);

            return Ok(entrants);
        }

        [AllowAnonymous]
        [HttpOptions]
        public IActionResult GetTournamentsOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS");
            return Ok();
        }
    }
}
