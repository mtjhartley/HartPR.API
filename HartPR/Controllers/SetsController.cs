using AutoMapper;
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
    [Route("api/sets")]
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

        [AllowAnonymous]
        [HttpGet("{id}", Name="GetSet")]
        public IActionResult GetSet(Guid id)
        {
            var setFromRepo = _hartPRRepository.GetSet(id);
            if (setFromRepo == null)
            {
                return NotFound();
            }

            var set = Mapper.Map<SetDto>(setFromRepo);

            return Ok(set);
        }

        [AllowAnonymous]
        [HttpGet("{id}/display", Name = "GetSetForDisplay")]
        public IActionResult GetSetForDisplay(Guid id)
        {
            var setFromRepo = _hartPRRepository.GetSetForDisplay(id);

            if (setFromRepo == null)
            {
                return NotFound();
            }

            return Ok(setFromRepo);
        }

        [HttpPost(Name = "CreateSet")]
        public IActionResult CreateSet([FromBody] SetForCreationDto set)
        {
            if (set == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var setEntity = Mapper.Map<Set>(set);

            _hartPRRepository.AddSet(setEntity);

            if (!_hartPRRepository.Save())
            {
                throw new Exception("Creating a set failed on save.");
            }

            var setToReturn = Mapper.Map<SetDto>(setEntity);

            return CreatedAtRoute("GetSet",
                new { id = setToReturn.Id },
                setToReturn);
        }

        [HttpDelete("{id}", Name = "DeleteSet")]
        public IActionResult DeleteSet(Guid id)
        {
            var setFromRepo = _hartPRRepository.GetSet(id);
            if (setFromRepo == null)
            {
                return NotFound();
            }

            _hartPRRepository.DeleteSet(setFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Deleting set {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSet(Guid id,
            [FromBody] SetForUpdateDto set)
        {
            if (set == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var setFromRepo = _hartPRRepository.GetSet(id);
            if (setFromRepo == null)
            {
                //not seeing a reason to add upsert implementation to set currently.
                return NotFound();
            }

            //map

            //apply update

            //map update dto back to entity

            Mapper.Map(set, setFromRepo); //TODO: Check if this needs to be configured

            _hartPRRepository.UpdateSet(setFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Updating set {id} failed on save.");
            }

            return NoContent();
        }

        //If a set is patched for the players, then both the Entrant and the Player need to be updated..
        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateSet(Guid id,
            [FromBody] JsonPatchDocument<SetForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var setFromRepo = _hartPRRepository.GetSet(id);
            if (setFromRepo == null)
            {
                return NotFound();
            }

            var setToPatch = Mapper.Map<SetForUpdateDto>(setFromRepo);

            patchDoc.ApplyTo(setToPatch, ModelState);

            TryValidateModel(setToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(setToPatch, setFromRepo);

            _hartPRRepository.UpdateSet(setFromRepo);

            if (!_hartPRRepository.Save())
            {
                throw new Exception($"Patching set {id} failed on save.");
            }

            return NoContent();
        }
    }
}
