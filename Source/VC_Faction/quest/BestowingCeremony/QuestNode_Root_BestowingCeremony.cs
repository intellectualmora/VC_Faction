// Decompiled with JetBrains decompiler
// Type: RimWorld.QuestGen.QuestNode_Root_BestowingCeremony
// Assembly: Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// MVID: F0AC5EB9-B52E-4CC3-96C7-0FC9A4EE15E5
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using Verse.Grammar;

namespace VC_Faction
{
    public class QuestNode_Root_BestowingCeremony : QuestNode
    {
        public const string QuestTag = "Bestowing";

        private bool TryGetCeremonyTarget(Slate slate, out Pawn pawn, out Faction bestowingFaction)
        {
            slate.TryGet<Faction>(nameof(bestowingFaction), out bestowingFaction);
            if (slate.TryGet<Pawn>("titleHolder", out pawn) && pawn.Faction != null && pawn.Faction.IsPlayer)
                return bestowingFaction != null ? RoyalTitleUtility.ShouldGetBestowingCeremonyQuest(pawn, bestowingFaction) : RoyalTitleUtility.ShouldGetBestowingCeremonyQuest(pawn, out bestowingFaction);
            pawn = (Pawn)null;
            foreach (Map map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    foreach (Pawn allPawn in map.mapPawns.AllPawns)
                    {
                        if (allPawn.Faction != null && allPawn.Faction.IsPlayer)
                            return bestowingFaction != null ? RoyalTitleUtility.ShouldGetBestowingCeremonyQuest(allPawn, bestowingFaction) : RoyalTitleUtility.ShouldGetBestowingCeremonyQuest(allPawn, out bestowingFaction);
                    }
                }
            }
            bestowingFaction = (Faction)null;
            return false;
        }

        protected override void RunInt()
        {
            if (!ModLister.CheckRoyalty("Bestowing ceremony"))
                return;
            Quest quest = RimWorld.QuestGen.QuestGen.quest;
            Slate slate = RimWorld.QuestGen.QuestGen.slate;
            Pawn pawn1;
            Faction bestowingFaction;
            if (!this.TryGetCeremonyTarget(RimWorld.QuestGen.QuestGen.slate, out pawn1, out bestowingFaction))
                return;
            RoyalTitleDef awardedWhenUpdating = pawn1.royalty.GetTitleAwardedWhenUpdating(bestowingFaction, pawn1.royalty.GetFavor(bestowingFaction));
            string str1 = QuestGenUtility.HardcodedTargetQuestTagWithQuestID("Bestowing");
            string inSignal1 = QuestGenUtility.QuestTagSignal(str1, "CeremonyExpired");
            string inSignal2 = QuestGenUtility.QuestTagSignal(str1, "CeremonyFailed");
            string inSignal3 = QuestGenUtility.QuestTagSignal(str1, "CeremonyDone");
            string inSignal4 = QuestGenUtility.QuestTagSignal(str1, "BeingAttacked");
            string inSignal5 = QuestGenUtility.QuestTagSignal(str1, "Fleeing");
            string inSignal6 = QuestGenUtility.QuestTagSignal(str1, "TitleAwardedWhenUpdatingChanged");
            Thing shuttle = QuestGen_Shuttle.GenerateShuttle(bestowingFaction);
            PawnKindDef pawn2def = RimWorld.PawnKindDefOf.Empire_Royal_Bestower;
            if (bestowingFaction.def.defName == FactionDefOf.VC_Empire.defName)
            {
                pawn2def = PawnKindDefOf.VC_Empire_Fighter_Elite_Melee;
            }
            else if (bestowingFaction.def.defName == FactionDefOf.VC_Federation.defName)
            {
                pawn2def = PawnKindDefOf.VC_Federation_Fighter_Elite_Melee;
            }
            else if (bestowingFaction.def.defName == FactionDefOf.VC_Gallia.defName)
            {
                pawn2def = PawnKindDefOf.VC_Gallia_Fighter_Elite_Melee;
            }
            Pawn pawn2 = quest.GetPawn(new QuestGen_Pawns.GetPawnParms()
            {
                mustBeOfKind = pawn2def,
                canGeneratePawn = true,
                mustBeOfFaction = bestowingFaction,
                mustBeWorldPawn = true,
                ifWorldPawnThenMustBeFree = true,
                redressPawn = true
            });
            QuestUtility.AddQuestTag(ref shuttle.questTags, str1);
            QuestUtility.AddQuestTag(ref pawn1.questTags, str1);
            ThingOwner<Thing> innerContainer = pawn2.inventory.innerContainer;
            for (int index = innerContainer.Count - 1; index >= 0; --index)
            {
                if (innerContainer[index].def == RimWorld.ThingDefOf.PsychicAmplifier)
                {
                    Thing thing = innerContainer[index];
                    innerContainer.RemoveAt(index);
                    thing.Destroy();
                }
            }


            for (int index = 0; index < 2; ++index)
                innerContainer.TryAdd(ThingMaker.MakeThing(RimWorld.ThingDefOf.PsychicAmplifier), 1, true);

            List<Pawn> pawnList1 = new List<Pawn>();
            pawnList1.Add(pawn2);
            slate.Set<List<Pawn>>("shuttleContents", pawnList1);
            slate.Set<Thing>("shuttle", shuttle);
            slate.Set<Pawn>("target", pawn1);
            slate.Set<Pawn>("bestower", pawn2);
            slate.Set<Faction>("bestowingFaction", bestowingFaction);
            List<Pawn> pawnList2 = new List<Pawn>();
            for (int index = 0; index < 6; ++index)
            {
                Pawn pawn3 = quest.GeneratePawn(RimWorld.PawnKindDefOf.Empire_Fighter_Janissary, bestowingFaction);
                if (bestowingFaction.def.defName == FactionDefOf.VC_Empire.defName)
                {
                    pawn3 = quest.GeneratePawn(PawnKindDefOf.VC_Empire_Fighter_Gunner, bestowingFaction);
                }
                else if (bestowingFaction.def.defName == FactionDefOf.VC_Federation.defName)
                {
                    pawn3 = quest.GeneratePawn(PawnKindDefOf.VC_Federation_Fighter_Gunner, bestowingFaction);
                }
                else if (bestowingFaction.def.defName == FactionDefOf.VC_Gallia.defName)
                {
                    pawn3 = quest.GeneratePawn(PawnKindDefOf.VC_Gallia_Fighter_Gunner, bestowingFaction);
                }
                pawnList1.Add(pawn3);
                pawnList2.Add(pawn3);
            }
            quest.EnsureNotDowned((IEnumerable<Pawn>)pawnList1);
            slate.Set<List<Pawn>>("defenders", pawnList2);
            shuttle.TryGetComp<CompShuttle>().requiredPawns = pawnList1;
            TransportShip transportShip = quest.GenerateTransportShip(TransportShipDefOf.Ship_Shuttle, (IEnumerable<Thing>)pawnList1, shuttle).transportShip;
            quest.AddShipJob_Arrive(transportShip, (MapParent)null, pawn1, startMode: ShipJobStartMode.Instant, factionForArrival: Faction.OfEmpire);
            quest.AddShipJob(transportShip, ShipJobDefOf.Unload);
            quest.AddShipJob_WaitForever(transportShip, true, false, pawnList1.Cast<Thing>().ToList<Thing>()).sendAwayIfAnyDespawnedDownedOrDead = new List<Thing>()
      {
        (Thing) pawn2
      };
            QuestUtility.AddQuestTag(ref transportShip.questTags, str1);
            quest.FactionGoodwillChange(bestowingFaction, -5, QuestGenUtility.HardcodedSignalWithQuestID("defenders.Killed"), historyEvent: HistoryEventDefOf.QuestPawnLost);
            QuestPart_BestowingCeremony part1 = new QuestPart_BestowingCeremony();
            part1.inSignal = RimWorld.QuestGen.QuestGen.slate.Get<string>("inSignal");
            part1.pawns.Add(pawn2);
            part1.mapOfPawn = pawn1;
            part1.faction = pawn2.Faction;
            part1.bestower = pawn2;
            part1.target = pawn1;
            part1.shuttle = shuttle;
            part1.questTag = str1;
            quest.AddPart((QuestPart)part1);
            QuestPart_EscortPawn part2 = new QuestPart_EscortPawn();
            part2.inSignal = RimWorld.QuestGen.QuestGen.slate.Get<string>("inSignal");
            part2.escortee = pawn2;
            part2.pawns.AddRange((IEnumerable<Pawn>)pawnList2);
            part2.mapOfPawn = pawn1;
            part2.faction = pawn2.Faction;
            part2.shuttle = shuttle;
            part2.questTag = str1;
            part2.leavingDangerMessage = (string)"MessageBestowingDanger".Translate();
            quest.AddPart((QuestPart)part2);
            string inSignal7 = QuestGenUtility.HardcodedSignalWithQuestID("shuttle.Killed");
            quest.FactionGoodwillChange(bestowingFaction, 0, inSignal7, historyEvent: HistoryEventDefOf.ShuttleDestroyed, ensureMakesHostile: true);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal7, sendStandardLetter: true);
            quest.AddPart((QuestPart)new QuestPart_RequirementsToAcceptThroneRoom()
            {
                faction = bestowingFaction,
                forPawn = pawn1,
                forTitle = awardedWhenUpdating
            });
            quest.AddPart((QuestPart)new QuestPart_RequirementsToAcceptPawnOnColonyMap()
            {
                pawn = pawn1
            });
            quest.AddPart((QuestPart)new QuestPart_RequirementsToAcceptNoDanger()
            {
                mapPawn = pawn1,
                dangerTo = bestowingFaction
            });
            quest.AddPart((QuestPart)new QuestPart_RequirementsToAcceptNoOngoingBestowingCeremony());
            string inSignal8 = QuestGenUtility.HardcodedSignalWithQuestID("shuttleContents.Recruited");
            string inSignal9 = QuestGenUtility.HardcodedSignalWithQuestID("bestowingFaction.BecameHostileToPlayer");
            quest.Signal(inSignal8, (Action)(() => QuestGen_End.End(quest, QuestEndOutcome.Fail, sendStandardLetter: true)));
            quest.Bestowing_TargetChangedTitle(pawn1, pawn2, awardedWhenUpdating, inSignal6);
            Quest quest1 = quest;
            LetterDef negativeEvent = LetterDefOf.NegativeEvent;
            string inSignal10 = inSignal1;
            string str2 = (string)"LetterLabelBestowingCeremonyExpired".Translate();
            string text = (string)"LetterTextBestowingCeremonyExpired".Translate(pawn1.Named("TARGET"));
            string label = str2;
            quest1.Letter(negativeEvent, inSignal10, text: text, label: label);
            quest.End(QuestEndOutcome.Fail, inSignal: QuestGenUtility.HardcodedSignalWithQuestID("target.Killed"), signalListenMode: QuestPart.SignalListenMode.OngoingOrNotYetAccepted, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Fail, inSignal: QuestGenUtility.HardcodedSignalWithQuestID("bestower.Killed"), signalListenMode: QuestPart.SignalListenMode.OngoingOrNotYetAccepted, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal1);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal9, signalListenMode: QuestPart.SignalListenMode.OngoingOrNotYetAccepted, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal2, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal4, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Fail, inSignal: inSignal5, sendStandardLetter: true);
            quest.End(QuestEndOutcome.Success, inSignal: inSignal3);
            quest.RewardChoice().choices.Add(new QuestPart_Choice.Choice()
            {
                rewards = {
          (Reward) new Reward_BestowingCeremony()
          {
            targetPawnName = pawn1.NameShortColored.Resolve(),
            titleName = awardedWhenUpdating.GetLabelCapFor(pawn1),
            awardingFaction = bestowingFaction,
            givePsylink = (awardedWhenUpdating.maxPsylinkLevel > pawn1.GetPsylinkLevel()),
            royalTitle = awardedWhenUpdating
          }
        }
            });
            List<Rule> rules1 = new List<Rule>();
            rules1.AddRange(GrammarUtility.RulesForPawn("pawn", pawn1));
            rules1.Add((Rule)new Rule_String("newTitle", awardedWhenUpdating.GetLabelCapFor(pawn1)));
            RimWorld.QuestGen.QuestGen.AddQuestNameRules(rules1);
            List<Rule> rules2 = new List<Rule>();
            rules2.AddRange(GrammarUtility.RulesForFaction("faction", bestowingFaction));
            rules2.AddRange(GrammarUtility.RulesForPawn("pawn", pawn1));
            rules2.Add((Rule)new Rule_String("newTitle", pawn1.royalty.GetTitleAwardedWhenUpdating(bestowingFaction, pawn1.royalty.GetFavor(bestowingFaction)).GetLabelFor(pawn1)));
            rules2.Add((Rule)new Rule_String("psylinkLevel", awardedWhenUpdating.maxPsylinkLevel.ToString()));
            RimWorld.QuestGen.QuestGen.AddQuestDescriptionRules(rules2);
        }

        protected override bool TestRunInt(Slate slate)
        {
            Pawn pawn;
            Faction bestowingFaction;
            if (!this.TryGetCeremonyTarget(slate, out pawn, out bestowingFaction) || bestowingFaction.HostileTo(Faction.OfPlayer))
                return false;
            return QuestGen_Pawns.GetPawnTest(new QuestGen_Pawns.GetPawnParms()
            {
                mustBeOfKind = PawnKindDefOf.VC_Gallia_Fighter_Elite_Melee,
                canGeneratePawn = true,
                mustBeOfFaction = bestowingFaction
            }, out pawn);
        }
    }
}
