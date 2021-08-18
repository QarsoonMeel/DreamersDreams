﻿using System.Collections.Generic;
using Verse;

namespace DreamersDream
{
    public class DD_Settings : ModSettings
    {
        public DD_Settings()
        {
        }

        public static bool isDreamingActive = true;

        public static bool isDefaultSettings = true;

        public static bool canNonSleepwalkerSleepwalk = true;

        public static bool isDebugMode = false;

        public static Dictionary<string, float> TagsCustomChances = new Dictionary<string, float>();

        //sleepwalk
        public static float sleepwalkerTraitModif = 1;

        public static float occasionalSleepwalkerTraitModif = 3f;
        public static float normalSleepwalkerTraitModif = 12.0f;
        public static float usualSleepwalkerTraitModif = 30.0f;

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref TagsCustomChances, "TagsCustomChances", LookMode.Value, LookMode.Value);

            Scribe_Values.Look(ref isDreamingActive, "isDreamingActive", true);

            Scribe_Values.Look(ref isDefaultSettings, "isDefaultSettings", true);

            //sleepwalking

            Scribe_Values.Look(ref sleepwalkerTraitModif, "sleepwalkerTraitModif", 1);

            //Scribe_Values.Look(ref occasionalSleepwalkerTraitModif, "occasionalSleepwalkerTraitModif", 1.5f);
            //Scribe_Values.Look(ref normalSleepwalkerTraitModif, "normalSleepwalkerTraitModif", 8.0f);
            //Scribe_Values.Look(ref usualSleepwalkerTraitModif, "usualSleepwalkerTraitModif", 30.0f);

            base.ExposeData();
        }

        public static void PurgeDict()
        {
            if (TagsCustomChances.EnumerableNullOrEmpty())
            {
                TagsCustomChances = new Dictionary<string, float>();
            }

            Dictionary<string, float> tempDict = new Dictionary<string, float>();

            foreach (var tag in DreamTracker.GetAllDreamTags)
            {
                if (TagsCustomChances.ContainsKey(tag.defName))
                {
                    tempDict.Add(tag.defName, TagsCustomChances[tag.defName]);
                }
            }

            TagsCustomChances.Clear();

            TagsCustomChances = tempDict;
        }
    }
}