// Decompiled with JetBrains decompiler
// Type: RimWorld.TraderKindDef
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using RimWorld;
using Verse;

namespace VC_Faction
{
    [XmlType("VC_Faction.MissionDef")]
    public class MissionKindDef : Def
    {
        public float commonality = 1f;
        public string category;
        public FactionDef faction;
        public string graphicsPath;
        public string text;
        public string choice1;
        public string choice2;
        public string title;
        public float point;
        public QuestScriptDef questScriptDef;
        public SimpleCurve commonalityMultFromPopulationIntent;
        public float CalculatedCommonality
        {
            get
            {
                float commonality = this.commonality;
                if (this.commonalityMultFromPopulationIntent != null)
                    commonality *= this.commonalityMultFromPopulationIntent.Evaluate(StorytellerUtilityPopulation.PopulationIntent);
                return commonality;
            }
        }

    }
}
