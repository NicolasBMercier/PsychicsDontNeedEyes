using UnityEngine;
using Verse;

namespace PsychicsDontNeedEyes
{
    public class PsychicsDontNeedEyesMod : Mod
    {
        public static BlindVisionSettings settings;
        private static bool _expectModifications = false;
        public static bool ExpectModifications;

        public PsychicsDontNeedEyesMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BlindVisionSettings>();
        }

        public override string SettingsCategory() => "Psychics Don't Need Eyes";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.initialize();

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 1: ", ref settings.SightImproveForPsylinkLevel1);
            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 2: ", ref settings.SightImproveForPsylinkLevel2);
            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 3: ", ref settings.SightImproveForPsylinkLevel3);
            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 4: ", ref settings.SightImproveForPsylinkLevel4);
            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 5: ", ref settings.SightImproveForPsylinkLevel5);
            DrawFloatSetting(listingStandard, "Sight Improve For Psylink Level 6: ", ref settings.SightImproveForPsylinkLevel6);

            listingStandard.GapLine();

            listingStandard.CheckboxLabeled("Only Affect Pawns with Blindsight meme", ref settings.OnlyAffectBlindsightPawns);

            listingStandard.CheckboxLabeled("Add Psychic sensitivity to Sight bonus equation", ref settings.AddPsychicSensitivityToSightOffset);

            if (listingStandard.ButtonText("Reset to Default"))
            {
                ResetSettings();
                settings.Write();
            }

            listingStandard.End();
            settings.Write();
        }

        private void DrawFloatSetting(Listing_Standard listingStandard, string label, ref float setting)
        {
            listingStandard.Label($"{label}: {setting:F2}");
            setting = listingStandard.Slider(setting, 0f, 4f);

            string buffer = setting.ToString("F2");
            listingStandard.TextFieldNumeric(ref setting, ref buffer, 0f, 4f);
        }

        private void ResetSettings()
        {
            settings.ResetToDefault();
        }
    }
}
