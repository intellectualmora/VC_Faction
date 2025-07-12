// Decompiled with JetBrains decompiler
// Type: RimWorld.ScenPart_GameStartDialog
// Assembly: Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// MVID: F0AC5EB9-B52E-4CC3-96C7-0FC9A4EE15E5
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class VC_ScenPart_GameStartDialog : ScenPart_GameStartDialog
    {
        private string text = "";
        private string textKey;
        private SoundDef closeSound;

        public override void PostGameStart()
        {
            if (!Find.GameInitData.startedFromEntry)
                return;
            Find.MusicManagerPlay.disabled = true;
            Find.WindowStack.Notify_GameStartDialogOpened();
            DiaNode nodeRoot = new DiaNode((TaggedString)(this.text.NullOrEmpty() ? this.textKey.TranslateSimple() : this.text));
            nodeRoot.options.Add(new DiaOption()
            {
                resolveTree = true,
                clickSound = (SoundDef)null
            });
            Dialog_NodeTree dialogNodeTree = new Dialog_NodeTree(nodeRoot);
            dialogNodeTree.soundClose = this.closeSound != null ? this.closeSound : SoundDefOf.GameStartSting;
            dialogNodeTree.closeAction = (Action)(() =>
            {
                Find.MusicManagerPlay.ForceSilenceFor(60f);
                Find.MusicManagerPlay.disabled = false;
                Find.WindowStack.Notify_GameStartDialogClosed();
                Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
                TutorSystem.Notify_Event((EventPack)"GameStartDialogClosed");
            });
            Find.WindowStack.Add((Window)dialogNodeTree);
            Find.Archive.Add((IArchivable)new ArchivedDialog((string)nodeRoot.text));
        }
        public override int GetHashCode() => base.GetHashCode() ^ this.text.GetHashCodeSafe() ^ this.textKey.GetHashCodeSafe() ^ (this.closeSound != null ? this.closeSound.GetHashCode() : 0);
    }
}
