
using System;
using System.Linq;
using Verse;
using RimWorld;
namespace VC_Faction
{
    public class IncidentWorker_MissionSignalReceived : IncidentWorker
    {
        private const int MaxShips = 2;

        protected override bool CanFireNowSub(IncidentParms parms) => base.CanFireNowSub(parms) && ((Map)parms.target).passingShipManager.passingShips.Count < 5;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            MissionKindDef mission = null;
            Map map = (Map)parms.target;
            if (map.GetComponent<RadioSignalMapComponent>().radioSignalManager.radioSignals.Count >= MaxShips)
            {
                return false;
            }

            if (!DefDatabase<MissionKindDef>.AllDefs
                    .Where<MissionKindDef>((Func<MissionKindDef, bool>)(x => this.CanSpawn(map, x)))
                    .TryRandomElementByWeight<MissionKindDef>(
                        (Func<MissionKindDef, float>)(missionKindDef => missionKindDef.CalculatedCommonality),
                        out mission))
            {
                throw new InvalidOperationException();
            }

            MissionSignal vis = new MissionSignal(mission,this.GetFaction(mission));

            vis.WasAnnounced = false;
            if (map.listerBuildings.allBuildingsColonist.Any<Building>((Predicate<Building>)(b =>
            {
                if (!(b.def == VC_Faction.ThingDefOf.VC_Radio))
                {
                    return false;
                }

                return b.GetComp<CompPowerTrader>() == null || b.GetComp<CompPowerTrader>().PowerOn;
            })))
            {
                this.SendStandardLetter(vis.MissionName, "收到无线电信号", LetterDefOf.PositiveEvent, parms, LookTargets.Invalid);
                vis.WasAnnounced = true;
            }
            map.GetComponent<RadioSignalMapComponent>().radioSignalManager.AddRadioSignal((RadioSignal)vis);
            return true;
        }

        private Faction GetFaction(MissionKindDef mission)
        {
            if (mission.faction == null)
                return (Faction)null;
            Faction result;
            return !Find.FactionManager.AllFactions.Where<Faction>((Func<Faction, bool>)(f => f.def == mission.faction)).TryRandomElement<Faction>(out result) ? (Faction)null : result;
        }

        private bool CanSpawn(Map map, MissionKindDef mission)
        {

            if (mission.faction == null)
                return true;
            Faction faction = this.GetFaction(mission);
            if (faction == null)
                return false;
            if (!faction.HostileTo(Faction.OfPlayer))
                    return true;
            return false;
        }
    }
}
