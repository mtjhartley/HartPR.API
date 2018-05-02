using AutoMapper;
using HartPR.Models;
using HartPR.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Controllers
{
    //[Route("api/tournaments/{tournamentId}/sets")]
    public class SetsController : Controller
    {
        private IHartPRRepository _hartPRRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public SetsController(IHartPRRepository hartPRRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _hartPRRepository = hartPRRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        //[HttpGet(Name = "GetSetsForTournament")]
        //public IActionResult GetSetsForTournament(Guid tournamentId)
        //{
        //    if (!_hartPRRepository.TournamentExists(tournamentId))
        //    {
        //        return NotFound();
        //    }

        //    var setsForTournamentFromRepo = _hartPRRepository.GetSetsForTournament(tournamentId);

        //    var setsForTournament = Mapper.Map<IEnumerable<SetDto>>(setsForTournamentFromRepo);

        //    return Ok(setsForTournament);
        //}
    }
}
