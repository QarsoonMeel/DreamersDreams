﻿using Verse;

namespace DreamersDream
{
    [StaticConstructorOnStartup]
    internal static class DreamersDreamsInitialisation
    {
        static DreamersDreamsInitialisation()
        {
            LoadDreams();
            LoadDreamTags();
        }

        private static void LoadDreams()
        {
            var totalDreams = 0;

            foreach (DreamDef dream in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(DreamDef)))
            {
                if (!dream.tags.NullOrEmpty())
                {
                    totalDreams++;
                    DreamTracker.GetAllDreams.Add(dream);
                }
                else
                {
                    Log.Warning("Dream " + dream.defName + " does not have category, so it will not be loaded.");
                }
            }
            Log.Message("Dreamer's Dreams: successfully loaded " + totalDreams + " dreams.");
        }

        private static void LoadDreamTags()
        {
            var totalTags = 0;

            foreach (DreamTagDef tag in GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(DreamTagDef)))
            {
                totalTags++;
                DreamTracker.GetAllDreamTags.Add(tag);
            }
            Log.Message("Dreamer's Dreams: successfully loaded " + totalTags + " dream tags.");
        }
    }
}