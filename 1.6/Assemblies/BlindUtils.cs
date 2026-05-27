using RimWorld;
using Verse;

namespace PsychicsDontNeedEyes
{
    public static class BlindUtils
    {
        public static void CheckAndApplyBlindVisionHediff(Pawn pawn, ref float __result)
        {
            try
            {
                if (IsIdeologyPreventingBlindVision(pawn) is false
                    && IsTechnicallyBlind(pawn)
                    && HasPsylinkHediff(pawn)
                    && HasBlindVisionHediff(pawn) is false)
                {
                    var hediff = pawn.health.AddHediff(BlindVisionHediffDefOf.BlindVision);
                    UpdateHediffState(ref hediff, pawn);
                    Log.Message($"[PsychicsDontNeedEyes] Applied BlindVision hediff with severity {hediff.Severity} to pawn {pawn.Name}");
                }
                else if (HasBlindVisionHediff(pawn))
                {
                    var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(BlindVisionHediffDefOf.BlindVision);
                    if (hediff.Severity != GetPsylinkLevel(pawn) || hediff.CapMods.First(x => x.capacity == PawnCapacityDefOf.Sight).offset != CalculateSightCapModsForSeverity(hediff.Severity, pawn.psychicEntropy.PsychicSensitivity))
                    {
                        UpdateHediffState(ref hediff, pawn);
                        Log.Message($"[PsychicsDontNeedEyes] Updated BlindVision hediff severity to {hediff.Severity} for pawn {pawn.Name}");
                    }
                }

                if (HasBlindVisionHediff(pawn)
                    && (IsTechnicallyBlind(pawn) is false || IsIdeologyPreventingBlindVision(pawn) || HasPsylinkHediff(pawn) is false))
                {
                    RemoveBlindVisionHediff(pawn);
                }

            }
            catch (Exception e)
            {
                Log.Error($"[PsychicsDontNeedEyes] Error applying BlindVision hediff to pawn {pawn.Name}: {e.Message}");
            }
        }

        private static void UpdateHediffState(ref Hediff hediff, Pawn pawn)
        {
            hediff.Severity = GetPsylinkLevel(pawn);
            hediff.CapMods.First(x => x.capacity == PawnCapacityDefOf.Sight).offset = CalculateSightCapModsForSeverity(hediff.Severity, pawn.psychicEntropy.PsychicSensitivity);
        }

        public static float CalculateSightCapModsForSeverity(float severity, float psychicSensitivity)
        {
            if (PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel.TryGetValue((int)severity, out float baseCapMod))
                return CalculateTotalSightCapMods(baseCapMod, psychicSensitivity);

            if (PsychicsDontNeedEyesMod.settings.GenerateSightOffsetFromPsylinkLevel && PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel.Any())
            {
                var closestSeverity = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel.OrderBy(x => Math.Abs(x.Key - severity)).First();
                //Calculate the sight cap mods relatives to the closest severity level
                return CalculateTotalSightCapMods(closestSeverity.Value + (severity - closestSeverity.Key) * PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevelDefault, psychicSensitivity);
            }

            if (PsychicsDontNeedEyesMod.settings.GenerateSightOffsetFromPsylinkLevel)
                return CalculateTotalSightCapMods(severity * PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevelDefault, psychicSensitivity);

            if (PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel.Any() is false)
                return CalculateTotalSightCapMods(0, psychicSensitivity);

            return CalculateTotalSightCapMods(PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel.OrderBy(x => Math.Abs(x.Key - severity)).First().Value, psychicSensitivity);
        }

        public static bool AffectsSight(Hediff hediff) => hediff.Part is not null && hediff.Part.def.tags.Contains(BodyPartTagDefOf.SightSource);

        public static bool IsTechnicallyBlind(Pawn pawn) => !pawn.health.hediffSet.GetNotMissingParts().Any(part => part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || pawn.health.hediffSet.HasHediff(HediffDefOf.Blindness);

        public static bool HasBlindVisionHediff(Pawn pawn) => pawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision);

        public static bool IsIdeologyPreventingBlindVision(Pawn pawn)
        {
            if (pawn.Ideo is null)
                return PsychicsDontNeedEyesMod.settings.OnlyAffectBlindsightPawns;

            return PsychicsDontNeedEyesMod.settings.OnlyAffectBlindsightPawns && pawn.Ideo.memes.All(x => x.defName is not "Blindsight");
        }

        public static void RemoveBlindVisionHediff(Pawn pawn)
        {
            var hediffDef = BlindVisionHediffDefOf.BlindVision;
            if (hediffDef is null)
            {
                Log.Error($"[PsychicsDontNeedEyes] Could not find hediff definition for BlindVision.");
                return;
            }

            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            if (hediff is not null)
            {
                pawn.health.RemoveHediff(hediff);
                pawn.health.capacities.Notify_CapacityLevelsDirty(); // Ensure capacities are recalculated
                pawn.health.Notify_HediffChanged(hediff); // Notify the pawn's health tracker to recalculate capacities
            }
        }

        public static bool IsPsylinkHediff(Hediff hediff) => hediff.def.hediffClass == typeof(Hediff_Psylink);

        public static bool HasPsylinkHediff(Pawn pawn) => pawn.health.hediffSet.hediffs.Any(IsPsylinkHediff);

        public static float GetPsylinkLevel(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier) is not Hediff_Level hediff)
                return 0f;

            return hediff.level;  // Adjust to match severity stages
        }

        private static float CalculateTotalSightCapMods(float baseCapMod, float psychicSensitivity)
        {
            return PsychicsDontNeedEyesMod.settings.AddPsychicSensitivityToSightOffset ? baseCapMod * psychicSensitivity : baseCapMod;
        }
    }
}
