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
    [XmlType("VC_Faction.RoleScriptDef")]
    public class RoleScriptDef : Def
    {
        public Dictionary<string,string> contentDictionary;
        public Dictionary<string, string> choice1Dictionary;
        public Dictionary<string, string> choice2Dictionary;
        public Dictionary<string, string> outSignal1Dictionary;
        public Dictionary<string, string> outSignal2Dictionary;
        public Dictionary<string, string> soundPathDictionary;
        public Dictionary<string, string> portraitPathDictionary;



    }
}
