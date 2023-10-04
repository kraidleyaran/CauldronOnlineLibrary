using CauldronOnlineCommon.Data.Math;
using CauldronOnlineServer.Services;

namespace CauldronOnlineServer
{
    public static class ServerStaticMethods
    {
        public static int Roll(this WorldIntRange intRange, bool inclusive = false)
        {
            return RNGService.Range(intRange.Min, inclusive ? intRange.Max + 1 : intRange.Max);
        }
    }
}