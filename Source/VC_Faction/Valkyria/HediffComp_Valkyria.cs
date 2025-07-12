using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Valkyria
{
    public class HediffComp_Valkyria : HediffComp
    {
        public static readonly Color forcedHairColor = new Color(0.9019607f, 0.9450980f, 0.94117647f);
        public Color cacheHairColor;
        public CompGlower_Valkyria cacheCompGlower;
        public HediffCompProperties_Valkyria Props => (HediffCompProperties_Valkyria)this.props;
        public override void CompPostMake()
        {
            base.CompPostMake();
            cacheHairColor = this.Pawn.story.HairColor;
            this.Pawn.story.HairColor = forcedHairColor;
            this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
            MusicManagerPlay musicManagerPlay = Find.MusicManagerPlay;
            musicManagerPlay.ForcePlaySong(Props.songDef,true);
            ThingComp instance = (ThingComp)Activator.CreateInstance(Props.compGlowerClass);
            instance.parent = (ThingWithComps)this.Pawn;
            this.Pawn.AllComps.Add(instance);
            CompProperties_Glower glowerprop = new CompProperties_Glower();
            glowerprop.compClass = Props.compGlowerClass;
            glowerprop.glowColor = Props.glowColor;
            glowerprop.glowRadius = Props.glowRadius;
            glowerprop.overlightRadius = Props.overlightRadius;
            glowerprop.colorPickerEnabled = Props.colorPickerEnabled;
            glowerprop.darklightToggle = Props.darklightToggle;
            cacheCompGlower = (CompGlower_Valkyria)instance;
            instance.Initialize((CompProperties)glowerprop);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            foreach (ThingComp allComp in this.Pawn.AllComps)
            {
                if (allComp.GetType() == Props.compGlowerClass)
                {
                    CompGlower_Valkyria temp = (CompGlower_Valkyria)allComp;
                    temp.isBeLit = false;
                    temp.UpdateLit(this.Pawn.Map);
                    this.Pawn.AllComps.Remove(allComp);

                    ThingComp instance = (ThingComp)Activator.CreateInstance(Props.compGlowerClass);
                    instance.parent = (ThingWithComps)this.Pawn;
                    this.Pawn.AllComps.Add(instance);
                    CompProperties_Glower glowerprop = new CompProperties_Glower();
                    glowerprop.compClass = Props.compGlowerClass;
                    glowerprop.glowColor = Props.glowColor;
                    glowerprop.glowRadius = Props.glowRadius;
                    glowerprop.overlightRadius = Props.overlightRadius;
                    glowerprop.colorPickerEnabled = Props.colorPickerEnabled;
                    glowerprop.darklightToggle = Props.darklightToggle;
                    cacheCompGlower = (CompGlower_Valkyria)instance;
                    instance.Initialize((CompProperties)glowerprop);
                    CompGlower_Valkyria temp2 = (CompGlower_Valkyria)instance;
                    temp2.UpdateLit(this.Pawn.Map);
                    break;
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (ThingComp allComp in this.Pawn.AllComps)
            {
                if (allComp.GetType() == Props.compGlowerClass)
                {
                    CompGlower_Valkyria temp = (CompGlower_Valkyria) allComp;
                    temp.isBeLit = false;
                    temp.UpdateLit(this.Pawn.Map);
                    this.Pawn.AllComps.Remove(allComp);
                    break;
                }
            }
            this.Pawn.story.HairColor = cacheHairColor;
            this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
            MusicManagerPlay musicManagerPlay = Find.MusicManagerPlay;
            musicManagerPlay.Stop();
        }
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            cacheCompGlower.isBeLit = false;
            cacheCompGlower.UpdateLit(this.Pawn.Corpse.Map);
            this.Pawn.story.HairColor = cacheHairColor;
            this.Pawn.Drawer.renderer.SetAllGraphicsDirty();
            MusicManagerPlay musicManagerPlay = Find.MusicManagerPlay;
            musicManagerPlay.Stop();
        }
    }
}
