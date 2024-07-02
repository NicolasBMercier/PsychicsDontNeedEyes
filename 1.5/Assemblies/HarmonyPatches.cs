using HarmonyLib;
using RimWorld;
using Verse;

namespace PsychicsDontNeedEyes
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new("BurritoJambon.PsychicsDontNeedEyes");
            harmony.PatchAll();
        }

        //[HarmonyPatch(typeof(Pawn_HealthTracker))]
        //[HarmonyPatch("AddHediff", typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult))]
        //public static class AddHediff_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
        //    {
        //        if (hediff.def == HediffDefOf.Blindness || IsPsylinkHediff(hediff) || AffectsSight(hediff))
        //        {
        //            CheckAndApplyBlindVisionHediff(__instance.hediffSet.pawn);
        //        }
        //    }
        //}

        //[HarmonyPatch(typeof(Pawn_HealthTracker))]
        //[HarmonyPatch("RemoveHediff", typeof(Hediff))]
        //public static class RemoveHediff_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
        //    {
        //        if (hediff.def == HediffDefOf.Blindness || IsPsylinkHediff(hediff) || AffectsSight(hediff))
        //        {
        //            CheckAndApplyBlindVisionHediff(__instance.hediffSet.pawn);
        //        }
        //    }
        //}

        //[HarmonyPatch(typeof(Pawn_HealthTracker))]
        //[HarmonyPatch("Notify_HediffChanged")]
        //public static class Notify_HediffChanged_Patch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
        //    {
        //        if ((hediff.Part != null && hediff.Part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || IsPsylinkHediff(hediff))
        //        {
        //            CheckAndApplyBlindVisionHediff(__instance.hediffSet.pawn);
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(PawnCapacityWorker_Sight))]
        [HarmonyPatch("CanHaveCapacity")]
        public static class CanHaveCapacity_SightPatch
        {
            public static void Postfix(ref bool __result)
            {
                try
                {
                    if (CalculateCapacityLevel_SightPatch.CurrentCapacityPawn == null)
                    {
                        return;
                    }

                    if (CalculateCapacityLevel_SightPatch.CurrentCapacityPawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                    {
                        // Modify the result based on your conditions
                        __result = true;
                    }
                }
                catch (Exception e)
                {

                    Log.Error($"[PsychicsDontNeedEyes] Error checking if pawn can have sight capacity: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(PawnCapacityWorker_Sight))]
        [HarmonyPatch("CalculateCapacityLevel")]
        public static class CalculateCapacityLevel_SightPatch
        {
            internal static Pawn CurrentCapacityPawn;
            public static void Postfix(ref float __result, HediffSet diffSet)
            {
                CurrentCapacityPawn = diffSet.pawn;

                if (CurrentCapacityPawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                    __result = 0.001f;

                BlindUtils.CheckAndApplyBlindVisionHediff(CurrentCapacityPawn, ref __result);
            }
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("IsBiologicallyBlind")]
        public static class IsBiologicallyBlind_Patch
        {
            public static void Postfix(ref bool __result, Pawn pawn)
            {
                if (pawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                {
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(ThoughtWorker_Precept_HalfBlind))]
        [HarmonyPatch("IsHalfBlind")]
        public static class IsHalfBlind_Patch
        {
            public static void Postfix(ref bool __result, Pawn p)
            {
                if (p.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("IsArtificiallyBlind")]
        public static class IsArtificiallyBlind_Patch
        {
            public static void Postfix(ref bool __result, Pawn p)
            {
                if (p.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(StatPart_BlindPsychicSensitivityOffset))]
        [HarmonyPatch("ConsideredBlind")]
        public static class ConsideredBlind_Patch
        {
            public static void Postfix(ref bool __result, Pawn pawn)
            {
                if (pawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision))
                {
                    __result = false;
                }
            }
        }

    }
}
