using RimWorld;
using Verse;

namespace PsychicsDontNeedEyes
{
    [DefOf]
    public static class BlindVisionHediffDefOf
    {
        [MayRequireRoyalty]
        public static HediffDef BlindVision = new();

        static BlindVisionHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BlindVisionHediffDefOf));
        }
    }
}
