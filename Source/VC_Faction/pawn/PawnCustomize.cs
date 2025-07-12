// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_Root_DistressCall
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class PawnCustomize
    {
        public static Pawn GenerateEdy()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("edy");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Militia"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("伊蒂", "伊蒂", "尼尔森");
                pawn.gender = Gender.Female;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.style.beardDef = null;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Edy");
                pawn.story.HairColor = new Color(0.635f, 0.624f, 0.557f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_Edy_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_Edy_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("NaturalMood"), 2));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Joyous")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Beauty"), 2));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(17 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(17 * 3600000);
                QuestGen.slate.Set<Pawn>("edy", pawn);

            }
            return pawn;
        }
        public static Pawn GenerateSelvaria()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("selvaria");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Empire_Fighter_Scout"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire));
                pawn.Name = new NameTriple("塞露贝利亚", "塞露贝利亚", "布蕾斯");
                pawn.gender = Gender.Female;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.style.beardDef = null;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Selvaria");
                pawn.story.HairColor = new Color(0.761f, 0.769f, 0.804f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_Selvaria_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_Selvaria_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("SpeedOffset"), 2));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Nimble")));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("Valkyria"));
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(22 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(22 * 3600000);
                QuestGen.slate.Set<Pawn>("selvaria", pawn);

            }
            return pawn;
        }
        public static Pawn GenerateClementiaForster()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("clementia");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Empire_Fighter_Scout"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Empire));
                pawn.Name = new NameTriple("克蕾曼缇亚", "克蕾曼缇亚", "佛斯特");
                pawn.gender = Gender.Female;
                pawn.guest.Recruitable = false;
                pawn.guest.resistance = -1f;
                pawn.style.beardDef = null;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Clementia");
                pawn.story.HairColor = new Color(0.314f, 0.337f, 0.302f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_ClementiaForster_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_ClementiaForster_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("TooSmart")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("FastLearner")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("GreatMemory")));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(37 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(37 * 3600000);
                QuestGen.slate.Set<Pawn>("clementia", pawn);

            }
            return pawn;
        }
        public static Pawn GenerateWelkin()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("welkin");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Farmer"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("维尔金", "维尔金", "巩特");
                pawn.gender = Gender.Male;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.style.beardDef = null;
                pawn.story.bodyType = BodyTypeDefOf.Male;
                pawn.story.skinColorOverride = Color.white;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Welkin");
                pawn.story.HairColor = new Color(0.549f, 0.514f, 0.502f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_WelkinGunther_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_WelkinGunther_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("NaturalMood"),1));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("FastLearner")));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(22 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(22 * 3600000);
                QuestGen.slate.Set<Pawn>("welkin", pawn);
            }
            return pawn;
        }
        public static Pawn GenerateAlicia()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("alicia");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Militia"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("艾莉西亚", "艾莉西亚", "梅露姬奥特");
                pawn.gender = Gender.Female;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.style.beardDef = null;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Alicia");
                pawn.story.HairColor = new Color(0.584f, 0.463f, 0.443f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_AliciaMelchiott_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_AliciaMelchiott_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("SpeedOffset"),2)); 
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Beauty"), 2));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("Valkyria"));
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(19 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(19 * 3600000);
                QuestGen.slate.Set<Pawn>("alicia", pawn);

            }
            return pawn;
        }
        public static Pawn GenerateRiela()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("riela");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Gallia_Fighter_Scout"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("莉艾拉", "莉艾拉", "玛鲁塞莉丝");
                pawn.gender = Gender.Female;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.style.beardDef = null;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Riela");
                pawn.story.HairColor = new Color(0.851f, 0.518f, 0.506f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_RielaMarcellis_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_RielaMarcellis_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(TraitDefOf.Kind));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Beauty"), 2));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("Valkyria"));
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(21 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(21 * 3600000);
                QuestGen.slate.Set<Pawn>("riela", pawn);
            }
            return pawn;
        }

        public static Pawn GenerateImca()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("imca");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Gallia_Fighter_Scout"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("伊", "伊姆卡", "姆卡");
                pawn.gender = Gender.Female;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.style.beardDef = null;
                pawn.story.bodyType = BodyTypeDefOf.Female;
                pawn.story.skinColorOverride = Color.white;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Imca");
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_Imca_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_Imca_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Recluse")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Nimble")));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("Darcsen"));
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(17 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(17 * 3600000);
                QuestGen.slate.Set<Pawn>("imca", pawn);
            }
            return pawn;
        }
        public static Pawn GenerateKurt()
        {
            Pawn pawn = QuestGen.slate.Get<Pawn>("kurt");
            if (pawn == null)
            {
                pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("VC_Gallia_Fighter_Scout"), Find.FactionManager.FirstFactionOfDef(FactionDefOf.VC_Gallia));
                pawn.Name = new NameTriple("克鲁特", "克鲁特", "欧文");
                pawn.gender = Gender.Male;
                pawn.guest.resistance = -1f;
                pawn.guest.Recruitable = false;
                pawn.story.bodyType = BodyTypeDefOf.Male;
                pawn.story.skinColorOverride = Color.white;
                pawn.style.beardDef = null;
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Kurt");
                pawn.story.HairColor = new Color(0.192f, 0.184f, 0.196f);
                pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("VC_KurtIrving_Childhood");
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("VC_KurtIrving_Adulthood");
                List<Trait> newTraits = new List<Trait>();
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Tough")));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("Nerves"), 2));
                newTraits.Add(new Trait(DefDatabase<TraitDef>.GetNamed("FastLearner")));
                pawn.story.traits.allTraits = newTraits;
                pawn.genes.ClearXenogenes();
                pawn.health.RemoveAllHediffs();
                pawn.ageTracker.AgeBiologicalTicks = (long)(20 * 3600000);
                pawn.ageTracker.AgeChronologicalTicks = (long)(20 * 3600000);
                QuestGen.slate.Set<Pawn>("kurt", pawn);
            }
            return pawn;
        }
    }
}
