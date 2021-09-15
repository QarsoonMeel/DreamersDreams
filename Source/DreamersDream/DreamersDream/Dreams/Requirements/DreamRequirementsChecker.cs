﻿using RimWorld;
using Verse;

namespace DreamersDream
{
    internal static class DreamRequirementsChecker
    {
        public static bool CheckBackstory(this BackstoryCategory backstoryCat, Pawn pawn, bool invert)
        {
            bool flag = false;
            foreach (var cats in pawn.story.GetBackstory(BackstorySlot.Adulthood)?.spawnCategories)
            {
                if (cats == backstoryCat.ToString())
                {
                    flag = true;
                }
            }

            foreach (var cats in pawn.story.GetBackstory(BackstorySlot.Childhood)?.spawnCategories)
            {
                if (cats == backstoryCat.ToString())
                {
                    flag = true;
                }
            }
            if (invert)
            {
                flag = !flag;
            }
            return flag;
        }

        public static bool CheckStandingStatus(this StandingStatus standing, Pawn pawn, bool invert)
        {
            bool flag = true;
            switch (standing)
            {
                case StandingStatus.prisoner:
                    if (!pawn.IsPrisoner)
                    {
                        flag = false;
                    }
                    break;

                case StandingStatus.colonist:
                    if (!pawn.IsColonist)
                    {
                        flag = false;
                    }
                    break;

                case StandingStatus.guest:

                    break;

                case StandingStatus.slave:
                    if (!pawn.IsSlave)
                    {
                        flag = false;
                    }
                    break;

                default:
                    break;
            }
            if (invert)
            {
                flag = !flag;
            }
            return flag;
        }

        public static bool CheckHealthStatus(this HealthStatus health, Pawn pawn, bool invert)
        {
            bool flag = true;
            switch (health)
            {
                case HealthStatus.wounded:
                    if (!pawn.health.hediffSet.HasNaturallyHealingInjury())
                    {
                        flag = false;
                    }
                    break;

                case HealthStatus.healthy:
                    if (pawn.health.hediffSet.HasNaturallyHealingInjury() || pawn.health.hediffSet.AnyHediffMakesSickThought)
                    {
                        flag = false;
                    }
                    break;

                case HealthStatus.ill:
                    if (!pawn.health.hediffSet.AnyHediffMakesSickThought)
                    {
                        flag = false;
                    }
                    break;

                case HealthStatus.hungry:
                    if ((float)pawn.needs.food.CurCategory == 0)
                    {
                        flag = false;
                    }
                    break;

                case HealthStatus.fed:
                    if ((float)pawn.needs.food.CurCategory != 0)
                    {
                        flag = false;
                    }
                    break;

                case HealthStatus.starving:
                    if (!pawn.needs.food.Starving)
                    {
                        flag = false;
                    }
                    break;

                default:
                    break;
            }
            if (invert)
            {
                flag = !flag;
            }
            return flag;
        }

        public static bool CheckSocialStatus(this SocialStatus social, Pawn pawn, bool invert)
        {
            bool flag = true;
            switch (social)
            {
                case SocialStatus.married:

                    break;

                case SocialStatus.bonded:

                    break;

                case SocialStatus.befriended:
                    break;

                case SocialStatus.killer:
                    if (pawn.records.GetValue(RecordDefOf.KillsHumanlikes) <= 0)
                    {
                        flag = false;
                    }
                    break;

                case SocialStatus.guilty:
                    if (!pawn.guilt.IsGuilty)
                    {
                        flag = false;
                    }
                    break;

                case SocialStatus.hasEx:
                    break;

                case SocialStatus.lonely:
                    break;

                default:
                    break;
            }
            if (invert)
            {
                flag = !flag;
            }
            return flag;
        }

        private static void AssignMaxMoodValues(ref float moodValue, MoodStatus mood, Pawn pawn)
        {
            switch (mood)
            {
                case MoodStatus.happy:
                    moodValue = 1f;
                    break;

                case MoodStatus.content:
                    moodValue = 0.9f;
                    break;

                case MoodStatus.neutral:
                    moodValue = 0.65f;
                    break;

                case MoodStatus.stressed:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdMinor;
                    break;

                case MoodStatus.onEdge:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdExtreme + 0.05f;
                    break;

                case MoodStatus.aboutToBreak:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdExtreme;
                    break;

                default:
                    break;
            }
        }

        private static void AssignMinMoodValues(ref float moodValue, MoodStatus mood, Pawn pawn)
        {
            switch (mood)
            {
                case MoodStatus.happy:
                    moodValue = 0.9f;
                    break;

                case MoodStatus.content:
                    moodValue = 0.65f;
                    break;

                case MoodStatus.neutral:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdMinor;
                    break;

                case MoodStatus.stressed:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdExtreme + 0.05f;
                    break;

                case MoodStatus.onEdge:
                    moodValue = pawn.mindState.mentalBreaker.BreakThresholdExtreme;
                    break;

                case MoodStatus.aboutToBreak:
                    moodValue = 0;
                    break;

                default:
                    break;
            }
        }

        public static bool CheckMoodStatus(MoodStatus moodMin, MoodStatus moodMax, Pawn pawn)
        {
            float currentMood = pawn.needs.mood.CurLevel;

            float minMood = 0;
            float maxMood = 1;

            AssignMinMoodValues(ref minMood, moodMin, pawn);

            AssignMaxMoodValues(ref maxMood, moodMax, pawn);

            bool flag = true;

            if (minMood > currentMood || maxMood < currentMood)
            {
                flag = false;
            }

            return flag;
        }

        internal static bool CheckRequirementForPawn(this DreamDef dream, Pawn pawn)
        {
            bool flag = true;
            //backstory
            foreach (var req in dream.conflictingBackstory)
            {
                if (!req.CheckBackstory(pawn, true))
                {
                    flag = false;
                }
            }
            foreach (var req in dream.requiredBackstory)
            {
                if (!req.CheckBackstory(pawn, false))
                {
                    flag = false;
                }
            }
            //standing
            foreach (var req in dream.conflictingStandingStatus)
            {
                if (!req.CheckStandingStatus(pawn, true))
                {
                    flag = false;
                }
            }
            foreach (var req in dream.requiredStandingStatus)
            {
                if (!req.CheckStandingStatus(pawn, false))
                {
                    flag = false;
                }
            }
            //health
            foreach (var req in dream.conflictingHealthStatus)
            {
                if (!req.CheckHealthStatus(pawn, true))
                {
                    flag = false;
                }
            }
            foreach (var req in dream.requiredHealthStatus)
            {
                if (!req.CheckHealthStatus(pawn, false))
                {
                    flag = false;
                }
            }
            //social
            foreach (var req in dream.conflictingSocialStatus)
            {
                if (!req.CheckSocialStatus(pawn, true))
                {
                    flag = false;
                }
            }
            foreach (var req in dream.conflictingSocialStatus)
            {
                if (!req.CheckSocialStatus(pawn, false))
                {
                    flag = false;
                }
            }

            if (!CheckMoodStatus(dream.minMood, dream.maxMood, pawn))
            {
                flag = false;
            }
            return flag;
        }
    }
}