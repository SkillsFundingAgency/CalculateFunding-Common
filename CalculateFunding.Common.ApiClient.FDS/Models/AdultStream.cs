using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public static class AdultStream
    {

        private enum Type
        {
            ADF,
            ASF,
            ALL,
            ASFC,
            ASFG,
            LOANS,
            ALLPMP,
            ASFPMP
        }

        private static string ParentType = nameof(Type.ADF);

        private static List<string> ChildTypes = new List<string>() { nameof(Type.ALL), nameof(Type.ASF) , nameof(Type.ASFC) , nameof(Type.ASFG) , nameof(Type.LOANS), nameof(Type.ALLPMP) , nameof(Type.ASFPMP) };

        public static string GetParent()
        {
            return ParentType;
        }

        public static List<string> GetChilds()
        {
            return ChildTypes;
        }

        public static bool IsExists(string adultStream)
        {
            return ChildTypes.Contains(adultStream);
        }
    }
}
