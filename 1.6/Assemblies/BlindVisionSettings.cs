using System.Collections.Generic;
using Verse;

namespace PsychicsDontNeedEyes
{
    public class BlindVisionSettings : ModSettings
    {
        public Dictionary<int, float> SightImproveForPsylinkLevel = new();
        public float SightImproveForPsylinkLevelDefault;
        public bool OnlyAffectBlindsightPawns = false;
        public bool AddPsychicSensitivityToSightOffset = false;
        public bool GenerateSightOffsetFromPsylinkLevel = true;

        public BlindVisionSettings()
        {
            ResetToDefault();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref SightImproveForPsylinkLevel, nameof(SightImproveForPsylinkLevel), LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref SightImproveForPsylinkLevelDefault, nameof(SightImproveForPsylinkLevelDefault));
            Scribe_Values.Look(ref OnlyAffectBlindsightPawns, nameof(OnlyAffectBlindsightPawns));
            Scribe_Values.Look(ref AddPsychicSensitivityToSightOffset, nameof(AddPsychicSensitivityToSightOffset));
            Scribe_Values.Look(ref GenerateSightOffsetFromPsylinkLevel, nameof(GenerateSightOffsetFromPsylinkLevel));
        }

        public void ResetToDefault()
        {
            SightImproveForPsylinkLevelDefault = 0.05f;
            SightImproveForPsylinkLevel.Clear();
            for (int i = 1; i <= 6; i++) // Default to 10 levels, can be extended
            {
                SightImproveForPsylinkLevel[i] = i * 0.25f; // Example default values
            }
            OnlyAffectBlindsightPawns = false;
            AddPsychicSensitivityToSightOffset = false;
            GenerateSightOffsetFromPsylinkLevel = true;
        }
    }
}
