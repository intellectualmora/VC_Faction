using UnityEngine;
using Verse;
using RimWorld;
namespace VC_Faction
{
    public class TalkWindowDrawing : IWindowDrawing
    {
        public GUIStyle EmptyStyle => new GUIStyle();

        public void DoWindowBackground(Rect rect)
        {
            GUI.DrawTexture(rect, ContentFinder<Texture2D>.Get("UI/Windows/Dialog_Talk/background"), ScaleMode.StretchToFill, alphaBlend: true);
        }

        public bool DoClostButtonSmall(Rect windowRect)
        {
            Vector2 buttonSize = new Vector2(24f, 24f);
            Rect buttonRect = new Rect(windowRect.width - buttonSize.x - 10f, 10f, buttonSize.x, buttonSize.y);
            return Widgets.ButtonImage(buttonRect, TexButton.CloseXSmall);
        }

        public bool DoCloseButton(Rect rect, string label)
        {
            return false;
        }

        public void BeginGroup(Rect rect)
        {
            GUI.BeginGroup(rect);
        }

        public void EndGroup()
        {
            GUI.EndGroup();
        }

        public void DoGrayOut(Rect rect)
        {
            Widgets.DrawWindowBackgroundTutor(rect);
        }
    }
}