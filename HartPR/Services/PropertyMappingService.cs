﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Entities;
using HartPR.Models;

namespace HartPR.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _playerPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Tag", new PropertyMappingValue(new List<string>() { "Tag" } )},
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
               { "State", new PropertyMappingValue(new List<string>() { "State" } ) },
               { "Trueskill", new PropertyMappingValue(new List<string>() { "Trueskill" } ) },
            };

        //private Dictionary<string, PropertyMappingValue> _meleePlayerPropertyMapping =
        //    new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        //    {
        //       { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
        //       { "Tag", new PropertyMappingValue(new List<string>() { "Tag" } )},
        //       { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
        //       { "State", new PropertyMappingValue(new List<string>() { "State" } ) },
        //       { "Trueskill", new PropertyMappingValue(new List<string>() { "MeleeTrueskill" } ) },
        //    };

        //private Dictionary<string, PropertyMappingValue> _smash4PlayerPropertyMapping =
        //    new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        //    {
        //       { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
        //       { "Tag", new PropertyMappingValue(new List<string>() { "Tag" } )},
        //       { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) },
        //       { "State", new PropertyMappingValue(new List<string>() { "State" } ) },
        //       { "Trueskill", new PropertyMappingValue(new List<string>() { "Smash4Trueskill" } ) },
        //    };

        private Dictionary<string, PropertyMappingValue> _tournamentPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } ) },
               { "Website", new PropertyMappingValue(new List<string>() { "Website" } ) },
               { "Subdomain", new PropertyMappingValue(new List<string>() { "Subdomain" } ) },
               { "URL", new PropertyMappingValue(new List<string>() { "URL" } ) },
               { "Date", new PropertyMappingValue(new List<string>() { "Date" } ) },
            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            //propertyMappings.Add(new PropertyMapping<PlayerMeleeDto, Player>(_meleePlayerPropertyMapping));
            //propertyMappings.Add(new PropertyMapping<PlayerSmash4Dto, Player>(_smash4PlayerPropertyMapping));
            propertyMappings.Add(new PropertyMapping<PlayerDto, Player>(_playerPropertyMapping));

            propertyMappings.Add(new PropertyMapping<TournamentDto, Tournament>(_tournamentPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
