using Verse;

namespace PsychicsDontNeedEyes
{
    public class BlindVisionSettings: ModSettings
    {
        public float SightImproveForPsylinkLevel1;
        public float SightImproveForPsylinkLevel2;
        public float SightImproveForPsylinkLevel3;
        public float SightImproveForPsylinkLevel4;
        public float SightImproveForPsylinkLevel5;
        public float SightImproveForPsylinkLevel6;
        public bool OnlyAffectBlindsightPawns = false;
        public bool AddPsychicSensitivityToSightOffset = false;

        public float DefaultSightImproveForPsylinkLevel1;
        public float DefaultSightImproveForPsylinkLevel2;
        public float DefaultSightImproveForPsylinkLevel3;
        public float DefaultSightImproveForPsylinkLevel4;
        public float DefaultSightImproveForPsylinkLevel5;
        public float DefaultSightImproveForPsylinkLevel6;
        public bool DefaultOnlyAffectBlindsightPawns = false;
        public bool DefaultPsychicSensitivityToSightOffset = false;

        public BlindVisionSettings()
        {
            ResetToDefault();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref SightImproveForPsylinkLevel1, nameof(SightImproveForPsylinkLevel1));
            Scribe_Values.Look(ref SightImproveForPsylinkLevel2, nameof(SightImproveForPsylinkLevel2));
            Scribe_Values.Look(ref SightImproveForPsylinkLevel3, nameof(SightImproveForPsylinkLevel3));
            Scribe_Values.Look(ref SightImproveForPsylinkLevel4, nameof(SightImproveForPsylinkLevel4));
            Scribe_Values.Look(ref SightImproveForPsylinkLevel5, nameof(SightImproveForPsylinkLevel5));
            Scribe_Values.Look(ref SightImproveForPsylinkLevel6, nameof(SightImproveForPsylinkLevel6));
            Scribe_Values.Look(ref OnlyAffectBlindsightPawns, nameof(OnlyAffectBlindsightPawns));
            Scribe_Values.Look(ref AddPsychicSensitivityToSightOffset, nameof(AddPsychicSensitivityToSightOffset));
        }

        private bool _isInitialized = false;
        public void initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;
            OverwriteDefaultSettings();
        }

        public void ResetToDefault()
        {
            SightImproveForPsylinkLevel1 = DefaultSightImproveForPsylinkLevel1;
            SightImproveForPsylinkLevel2 = DefaultSightImproveForPsylinkLevel2;
            SightImproveForPsylinkLevel3 = DefaultSightImproveForPsylinkLevel3;
            SightImproveForPsylinkLevel4 = DefaultSightImproveForPsylinkLevel4;
            SightImproveForPsylinkLevel5 = DefaultSightImproveForPsylinkLevel5;
            SightImproveForPsylinkLevel6 = DefaultSightImproveForPsylinkLevel6;
            OnlyAffectBlindsightPawns = DefaultOnlyAffectBlindsightPawns;
            AddPsychicSensitivityToSightOffset = DefaultPsychicSensitivityToSightOffset;
        }

        public void OverwriteDefaultSettings()
        {
            DefaultSightImproveForPsylinkLevel1 = BlindVisionHediffDefOf.BlindVision.stages[0].capMods[0].offset;
            DefaultSightImproveForPsylinkLevel2 = BlindVisionHediffDefOf.BlindVision.stages[1].capMods[0].offset;
            DefaultSightImproveForPsylinkLevel3 = BlindVisionHediffDefOf.BlindVision.stages[2].capMods[0].offset;
            DefaultSightImproveForPsylinkLevel4 = BlindVisionHediffDefOf.BlindVision.stages[3].capMods[0].offset;
            DefaultSightImproveForPsylinkLevel5 = BlindVisionHediffDefOf.BlindVision.stages[4].capMods[0].offset;
            DefaultSightImproveForPsylinkLevel6 = BlindVisionHediffDefOf.BlindVision.stages[5].capMods[0].offset;
        }
    }
}
