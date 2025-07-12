// Decompiled with JetBrains decompiler
// Type: RimWorld.CompRagnaidField
// Assembly: Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// MVID: F0AC5EB9-B52E-4CC3-96C7-0FC9A4EE15E5
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VC_Faction
{
    [StaticConstructorOnStartup]
    public class CompRagnaidField : ThingComp
    {
        private Sustainer sustainer;
        private Mote mote;
        public CompProperties_RagnaidField Props => (CompProperties_RagnaidField)this.props;

        public bool Active
        {
            get
            {
                CompCanBeDormant comp = this.parent.GetComp<CompCanBeDormant>();
                return comp == null || comp.Awake;
            }
        }



        public override void PostPostMake()
        {
            base.PostPostMake();
        }

        public override void PostDeSpawn(Map map) => this.sustainer?.End();


        public override void CompTick()
        {
            
            if (this.Props.activeSound.NullOrUndefined())
                return;
            if (this.Active)
            {
                if (this.sustainer == null || this.sustainer.Ended)
                    this.sustainer = this.Props.activeSound.TrySpawnSustainer(SoundInfo.InMap((TargetInfo)(Thing)this.parent));
                if (this.mote == null)
                {
                    this.mote = MoteMaker.MakeAttachedOverlay((Thing)this.parent, Props.mote, Vector3.zero);

                }
                else{
                this.mote.Maintain();  
                }
                foreach (Pawn targ in (IEnumerable<Pawn>)this.parent.Map.mapPawns.AllPawnsSpawned )
                {
                    if (targ.RaceProps.Humanlike && !targ.Dead && targ.health != null && (double)targ.Position.DistanceTo(this.parent.Position) <= (double)this.Props.radius)
                    {
                        Hediff hd = targ.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff);
                        if (hd == null)
                        {
                            hd = targ.health.AddHediff(this.Props.hediff, targ.health.hediffSet.GetBrain());
                        }
                        HediffComp_Disappears comp1 = hd.TryGetComp<HediffComp_Disappears>();
                        if (comp1 == null)
                            Log.Error("HediffComp_GiveHediffsInRange has a hediff in props which does not have a HediffComp_Disappears");
                        else
                            comp1.ticksToDisappear = 5;
                    }
                }
                this.sustainer.Maintain();
            }
            else
            {
                if (this.sustainer == null || this.sustainer.Ended)
                    return;
                this.sustainer.End();
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (this.Active)
                return;
        }

        public override void Notify_LordDestroyed()
        {
            base.Notify_LordDestroyed();
        }

        public override void PostDraw()
        {
            base.PostDraw();

        }


        public override void PostExposeData()
        {
            base.PostExposeData();
       
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
         
        }
    }
}
