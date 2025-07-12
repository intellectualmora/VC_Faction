// Decompiled with JetBrains decompiler
// Type: RimWorld.CompProperties_ProjectileInterceptor
// Assembly: Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// MVID: F0AC5EB9-B52E-4CC3-96C7-0FC9A4EE15E5
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class CompProperties_RagnaidField : CompProperties
    {
        public float radius;
        public SoundDef activeSound;
        public ThingDef mote;
        public HediffDef hediff;

        public CompProperties_RagnaidField() => this.compClass = typeof(CompRagnaidField);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string configError in base.ConfigErrors(parentDef))
                yield return configError;
        }
    }
}