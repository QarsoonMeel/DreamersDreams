﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DreamersDream
{
    public class DD_Mod : Mod
    {
        private DD_Settings settings;

        public DD_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<DD_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (DD_Settings.TagsCustomChances.NullOrEmpty())
            {
                DD_Settings.TagsCustomChances = new Dictionary<string, float>();
            }

            Widgets.DrawTextureFitted(new Rect(inRect.x, inRect.y, inRect.width, inRect.height), Textures.SettingsBackGround, 1);

            Rect masterRect = new Rect(inRect.x + (0.1f * inRect.width), inRect.y + 40, 0.8f * inRect.width, 936);

            Listing_Standard listingTop = new Listing_Standard();
            Rect TopSettings = new Rect(masterRect.x, masterRect.y, masterRect.width, 45);

            listingTop.Begin(TopSettings); //column 1
            listingTop.ColumnWidth = TopSettings.width / 3.2f;

            if (listingTop.ButtonText("Mod active: " + DD_Settings.isDreamingActive.ToString()))
            {
                DD_Settings.isDreamingActive = !DD_Settings.isDreamingActive;
            }

            Rect MidSettings = new Rect(TopSettings.x, TopSettings.y + listingTop.CurHeight, masterRect.width, 60);

            listingTop.NewColumn(); // column 2
            if (DD_Settings.isDreamingActive)
            {
                if (listingTop.ButtonText("Default settings: " + DD_Settings.isDefaultSettings.ToString()))
                {
                    DD_Settings.isDefaultSettings = !DD_Settings.isDefaultSettings;
                }
            }

            listingTop.NewColumn(); //column 3
            if (listingTop.ButtonText("Reset settings", "Set now"))
            {
                ResetValues();
            }

            listingTop.End();

            Listing_Standard listingMid = new Listing_Standard();
            if (DD_Settings.isDreamingActive && !DD_Settings.isDefaultSettings)
            {
                listingMid.Begin(MidSettings);

                listingMid.GapLine();

                if (DD_Settings.sleepwalkerTraitModif == 0)
                {
                    listingMid.Label("How much sleepwalker traits affect chance for sleepwalking: " + "off");
                }
                else
                {
                    listingMid.Label("How much sleepwalker traits affect chance for sleepwalking: " + DD_Settings.sleepwalkerTraitModif * 100 + "%");
                }

                DD_Settings.sleepwalkerTraitModif = (float)Math.Round(listingMid.Slider(DD_Settings.sleepwalkerTraitModif, 0, 2), 2);
                listingMid.GapLine();

                Rect TableSettings = new Rect(masterRect.x, MidSettings.y + listingMid.CurHeight, 270f, inRect.height);
                TableSettings.height = inRect.height - TableSettings.y;

                listingMid.End();

                Rect TagTable = new Rect(TableSettings.x, TableSettings.y, TableSettings.width, columnHeight * CountDisplayableTags());
                Widgets.BeginScrollView(TableSettings, ref scrollPos, TagTable, true);

                DrawTagsRows(TagTable);

                Widgets.EndScrollView();
            }
        }

        private void DrawTagsRows(Rect TableRect)
        {
            float count = 0;

            float ScrollYPos = TableRect.y;

            Rect columnTags = new Rect(TableRect.x, ScrollYPos, 100f, columnHeight);
            ModSettingsUtility.DrawTableFirstRow(ref columnTags, "Tags");

            Rect columnChance = new Rect(columnTags.x + columnTags.width, ScrollYPos, 146f, columnHeight);
            ModSettingsUtility.DrawTableFirstRow(ref columnChance, "Chance");

            foreach (var dreamTag in DreamTracker.GetAllDreamTags)
            {
                if (dreamTag.isSideTag)
                {
                    continue;
                }

                ResolveAlternatingBG(count, columnTags, Textures.TableEntryBGCat1, Textures.TableEntryBGCat2);
                DrawColumnCategory(ref columnTags, dreamTag);

                ResolveAlternatingBG(count, columnChance, Textures.TableEntryBGChance1, Textures.TableEntryBGChance2);
                DrawColumnChance(ref columnChance, dreamTag);

                count++;

                columnTags.y += columnHeight;
                columnChance.y += columnHeight;
            }
        }

        private void DrawColumnCategory(ref Rect column, DreamTagDef tag)
        {
            Widgets.Label(ModSettingsUtility.GetMiddleOfRectForString(column, tag.label), tag.defName);
        }

        private void DrawColumnChance(ref Rect column, DreamTagDef tag)
        {
            float chance = tag.chance;
            ModSettingsUtility.CheckIfMasterListContainsAddIfNot(tag, ref chance);

            ModSettingsUtility.DrawChanceButtons(column, ref chance);

            string label = Math.Round(PawnDreamTagsOddsTracker.ChanceInPercentages(chance, ModSettingsUtility.AddUpChancesForQualities()), 2) + "%";
            Widgets.Label(ModSettingsUtility.GetMiddleOfRectForString(column, label), label);

            DD_Settings.TagsCustomChances[tag.defName] = chance;
        }

        private void ResetValues()
        {
            foreach (var dreamTag in DreamTracker.GetAllDreamTags)
            {
                DD_Settings.TagsCustomChances[dreamTag.defName] = dreamTag.chance;
            }

            DD_Settings.sleepwalkerTraitModif = 1;
        }

        private void ResolveAlternatingBG(float alternatingBGCount, Rect column, Texture texture1, Texture texture2)
        {
            switch (alternatingBGCount % 2)
            {
                case 0:
                    Widgets.DrawTextureFitted(column, texture1, 1);
                    break;

                case 1:
                    Widgets.DrawTextureFitted(column, texture2, 1);
                    break;
            }
        }

        private float CountDisplayableTags()
        {
            float numberOfRows = 1;

            foreach (var tag in DreamTracker.GetAllDreamTags)
            {
                if (!tag.isSideTag)
                {
                    numberOfRows++;
                }
            }

            return numberOfRows;
        }

        public override string SettingsCategory()
        {
            return "Dreamer's Dreams";
        }

        private static Vector2 scrollPos = Vector2.zero;

        private static float columnHeight = 25f;
    }
}