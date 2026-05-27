using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace PsychicsDontNeedEyes
{
    public class PsychicsDontNeedEyesMod : Mod
    {
        public static BlindVisionSettings settings;
        private Vector2 scrollPosition = Vector2.zero;

        public PsychicsDontNeedEyesMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BlindVisionSettings>();
        }

        public override string SettingsCategory() => "Psychics Don't Need Eyes";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            Rect viewRect = new Rect(0, 0, inRect.width - 16, settings.SightImproveForPsylinkLevel.Count * 80 + 200); // Adjust height as needed

            if (settings.SightImproveForPsylinkLevel is null)
                settings.SightImproveForPsylinkLevel = [];

            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
            listingStandard.Begin(viewRect);

            foreach (var level in settings.SightImproveForPsylinkLevel.Keys.ToList())
            {
                float value = settings.SightImproveForPsylinkLevel[level];
                DrawFloatSetting(listingStandard, $"Sight Improve For Psylink Level {level} ", ref value);
                settings.SightImproveForPsylinkLevel[level] = value;
            }

            if (listingStandard.ButtonText("Add Psylink Level"))
            {
                int newLevel = 1;
                if (settings.SightImproveForPsylinkLevel.Any())
                    newLevel = settings.SightImproveForPsylinkLevel.Keys.Max() + 1;

                settings.SightImproveForPsylinkLevel[newLevel] = newLevel * 0.25f; // Example default value
            }

            listingStandard.GapLine();
            listingStandard.CheckboxLabeled("Only Affect Pawns with Blindsight meme", ref settings.OnlyAffectBlindsightPawns);
            listingStandard.CheckboxLabeled("Add Psychic sensitivity to Sight bonus equation", ref settings.AddPsychicSensitivityToSightOffset);

            listingStandard.GapLine();

            listingStandard.CheckboxLabeled("Automatically Generate Sight bonus for missing Psylink levels", ref settings.GenerateSightOffsetFromPsylinkLevel);
            DrawFloatSetting(listingStandard, "Sight Improve by Psylink Level for automatic generation ", ref settings.SightImproveForPsylinkLevelDefault);

            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.ResetToDefault();
                settings.Write();
            }

            listingStandard.End();
            Widgets.EndScrollView();
            settings.Write();
        }

        private void DrawFloatSetting(Listing_Standard listingStandard, string label, ref float setting)
        {
            listingStandard.Label($"{label}: {setting:F2}");
            setting = listingStandard.Slider(setting, 0f, 4f);
            string buffer = setting.ToString("F2");
            listingStandard.TextFieldNumeric(ref setting, ref buffer, 0f, 4f);
        }
    }
}
