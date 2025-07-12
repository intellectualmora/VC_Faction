// Decompiled with JetBrains decompiler
// Type: RimWorld.CompAbilityEffect_WithDest
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace VC_Faction
{
    public abstract class CompAbilityEffect_WithFixedDest : CompAbilityEffect, ITargetingSource
    {
        protected LocalTargetInfo selectedTarget = LocalTargetInfo.Invalid;
        private List<IntVec3> cells = new List<IntVec3>();

        public CompProperties_EffectWithDest Props => (CompProperties_EffectWithDest)this.props;

        public virtual TargetingParameters targetParams => new TargetingParameters()
        {
            canTargetLocations = true
        };

        public bool MultiSelect => false;

        public Thing Caster => (Thing)this.parent.pawn;

        public Pawn CasterPawn => this.parent.pawn;

        public Verb GetVerb => (Verb)null;

        public bool CasterIsPawn => true;

        public bool IsMeleeAttack => false;

        public bool Targetable => true;

        public Texture2D UIIcon => BaseContent.BadTex;

        public ITargetingSource DestinationSelector => (ITargetingSource)null;

        public bool HidePawnTooltips => true;

        public LocalTargetInfo GetDestination(LocalTargetInfo target)
        {
            Map map = this.parent.pawn.Map;
            List<CompGatherSpot> list = map.gatherSpotLister.activeSpots;
            if (list.Count > 0)
            {
                foreach(var comp in list)
                {
                    if(comp.parent.def.defName.Contains("Flag"))
                    {
                        return new LocalTargetInfo(comp.parent.InteractionCell);
                    }
                }
            }
            return target;
    
        }

        public bool CanPlaceSelectedTargetAt(LocalTargetInfo target)
        {
            Pawn pawn = this.selectedTarget.Pawn;
            if (pawn == null)
                return CompAbilityEffect_WithFixedDest.CanTeleportThingTo(target, this.parent.pawn.Map);
            Building_Door door = target.Cell.GetDoor(pawn.Map);
            return (door == null || door.CanPhysicallyPass(pawn)) && pawn.Spawned && !target.Cell.Impassable(this.parent.pawn.Map) && target.Cell.WalkableBy(this.parent.pawn.Map, pawn);
        }

        public static bool CanTeleportThingTo(LocalTargetInfo target, Map map)
        {
            Building edifice = target.Cell.GetEdifice(map);
            if (edifice != null && edifice.def.surfaceType != SurfaceType.Item && edifice.def.surfaceType != SurfaceType.Eat && (!(edifice is Building_Door buildingDoor) || !buildingDoor.Open))
                return false;
            List<Thing> thingList = target.Cell.GetThingList(map);
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (thingList[index].def.category == ThingCategory.Item)
                    return false;
            }
            return true;
        }

        public virtual bool CanHitTarget(LocalTargetInfo target) => target.IsValid;

        public virtual bool ValidateTarget(LocalTargetInfo target, bool showMessages = true) => this.CanHitTarget(target);

        public void DrawHighlight(LocalTargetInfo target)
        {
            if (!target.IsValid)
                return;
            GenDraw.DrawTargetHighlight(target);
        }

        public void OnGUI(LocalTargetInfo target)
        {
            GenUI.DrawMouseAttachment(!target.IsValid ? TexCommand.CannotShoot : this.parent.def.uiIcon);
            string str = this.ExtraLabelMouseAttachment(target);
            if (str.NullOrEmpty())
                return;
            Widgets.MouseAttachedLabel(str);
        }

        public void OrderForceTarget(LocalTargetInfo target)
        {
            this.parent.QueueCastingJob(this.selectedTarget, target);
            this.selectedTarget = LocalTargetInfo.Invalid;
        }

        public void SetTarget(LocalTargetInfo target) => this.selectedTarget = target;

        public virtual void SelectDestination() => Find.Targeter.BeginTargeting((ITargetingSource)this);

        public bool SelectedTargetInvalidated() => this.selectedTarget.HasThing && !this.selectedTarget.Thing.Spawned;
    }
}
