using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASME.WebEnd.Services
{
    public interface IIdGenerator
    {
        string GetIdPrefix(string security);
        string GenerateId(string security);
    }

    public class IdGenerator : IIdGenerator
    {
        private readonly Dictionary<string, string> _securityPrefixes;

        public IdGenerator(ISecuritiesRepository securitiesRepository)
        {
            var securities = securitiesRepository.GetSecurityCodes().ToArray();
            var idLength = (securities.Length - 1).ToString().Length;

            _securityPrefixes = securitiesRepository.GetSecurityCodes()
                .OrderBy(s => s)
                .Select((s, i) => new {Security = s, Prefix = i.ToString().PadLeft(idLength, '0')})
                .ToDictionary(s => s.Security, s => s.Prefix);
        }

        public string GetIdPrefix(string security)
        {
            return _securityPrefixes[security];
        }

        public string GenerateId(string security)
        {
            return $"{GetIdPrefix(security)}{Guid.NewGuid()}";
        }
    }
}