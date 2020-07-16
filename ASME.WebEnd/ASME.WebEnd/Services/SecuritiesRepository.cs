using System.Collections.Generic;

namespace ASME.WebEnd.Services
{
    public interface ISecuritiesRepository
    {
        IEnumerable<string> GetSecurityCodes();
    }

    public class SecuritiesRepository : ISecuritiesRepository
    {
        public IEnumerable<string> GetSecurityCodes()
        {
            yield return "AA01";
            yield return "AA02";
            yield return "AA03";
            yield return "AA04";
            yield return "AA05";
            yield return "AA06";
            yield return "AA07";
            yield return "AA08";
            yield return "AA09";
            yield return "AA10";
        }
    }
}