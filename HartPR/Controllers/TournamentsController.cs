using AutoMapper;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;
using HartPR.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Controllers
{
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

        [HttpGet(Name = "GetTournaments")]
        public IActionResult GetTournaments(TournamentsResourceParameters tournamentsResourceParameters)
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

            var tournamentsFromRepo = _hartPRRepository.GetTournaments(tournamentsResourceParameters);

            var tournaments = Mapper.Map<IEnumerable<TournamentDto>>(tournamentsFromRepo);

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
                    (Guid)tournamentAsDictionary["Id"], tournamentsResourceParameters.Fields);

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

        private string CreateTournamentsResourceUri(
            TournamentsResourceParameters tournamentResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetTournaments",
                      new
                      {
                          fields = tournamentResourceParameters.Fields,
                          orderBy = tournamentResourceParameters.OrderBy,
                          searchQuery = tournamentResourceParameters.SearchQuery,
                          pageNumber = tournamentResourceParameters.PageNumber - 1,
                          pageSize = tournamentResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetTournaments",
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
                    return _urlHelper.Link("GetTournaments",
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

        private IEnumerable<LinkDto> CreateLinksForTournament(Guid id, string fields)
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

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteTournament", new { id = id }),
              "delete_player",
              "DELETE"));

            return links;
        }
    }
}
