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
                    UpdateBlindVisionStages();

                    var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(BlindVisionHediffDefOf.BlindVision);
                    if (hediff == null)
                    {
                        hediff = pawn.health.AddHediff(BlindVisionHediffDefOf.BlindVision);
                    }
                    UpdateHediffState(ref hediff, pawn);
                    

                    Log.Message($"[PsychicsDontNeedEyes] Applied BlindVision hediff with severity {hediff.Severity} to pawn {pawn.Name}");
                }
                else if (HasBlindVisionHediff(pawn))
                {
                    var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(BlindVisionHediffDefOf.BlindVision);
                    UpdateBlindVisionStages();
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
            hediff.CapMods.First(x => x.capacity == PawnCapacityDefOf.Sight).offset = CalculateTotalSightCapMods(hediff.CapMods.First(x => x.capacity == PawnCapacityDefOf.Sight).offset, pawn.psychicEntropy.PsychicSensitivity);
        }

        private static void UpdateBlindVisionStages() 
        {
            PsychicsDontNeedEyesMod.settings.initialize();

            HediffDef hediffDef = BlindVisionHediffDefOf.BlindVision;
            hediffDef.stages[0].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel1;
            hediffDef.stages[1].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel2;
            hediffDef.stages[2].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel3;
            hediffDef.stages[3].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel4;
            hediffDef.stages[4].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel5;
            hediffDef.stages[5].capMods[0].offset = PsychicsDontNeedEyesMod.settings.SightImproveForPsylinkLevel6;
        }

        public static bool AffectsSight(Hediff hediff) => hediff.Part is not null && hediff.Part.def.tags.Contains(BodyPartTagDefOf.SightSource);

        public static bool IsTechnicallyBlind(Pawn pawn) => !pawn.health.hediffSet.GetNotMissingParts().Any(part => part.def.tags.Contains(BodyPartTagDefOf.SightSource)) || pawn.health.hediffSet.HasHediff(HediffDefOf.Blindness);

        public static bool HasBlindVisionHediff(Pawn pawn) => pawn.health.hediffSet.HasHediff(BlindVisionHediffDefOf.BlindVision);

        public static bool IsIdeologyPreventingBlindVision(Pawn pawn) => PsychicsDontNeedEyesMod.settings.OnlyAffectBlindsightPawns && pawn.Ideo.memes.All(x => x.defName is not "Blindsight");

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

        public static float GetPsylinkLevel(Pawn pawn) => pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier)?.CurStageIndex + 1 ?? 0f;  // Adjust to match severity stages

        private static float CalculateSightCapModsForSeverity(float severity, float psychicSensitivity)
        {
            float baseCapMod = BlindVisionHediffDefOf.BlindVision.stages.Where(stage => stage.minSeverity >= severity).OrderBy(stage => stage.minSeverity).First().capMods.First(capmod => capmod.capacity == PawnCapacityDefOf.Sight).offset;
            return CalculateTotalSightCapMods(baseCapMod, psychicSensitivity);
        }

        private static float CalculateTotalSightCapMods(float baseCapMod, float psychicSensitivity)
        {
            return PsychicsDontNeedEyesMod.settings.AddPsychicSensitivityToSightOffset ? baseCapMod * psychicSensitivity : baseCapMod;
        }
    }
}
