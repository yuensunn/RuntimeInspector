using UnityEngine;


namespace RI.UI
{
    public class ScrollView
    {
        public Vector2 scrollPos { get; private set; }

        public ScrollView()
        {
            scrollPos = Vector2.zero;
        }

        public void Begin(string style)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, style);
        }
        public void Begin()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
        }

        public void End()
        {
            GUILayout.EndScrollView();
        }
    }


    public class Foldout
    {

        string openArrow = "▼";
        string closeArrow = "►";
        public bool open { get; private set; }

        GUIStyle style = new GUIStyle();
        public Foldout(bool open)
        {
            style.onNormal.background = null;
            this.open = open;
            style.onNormal.textColor = Color.white;
            style.normal.textColor = Color.white;
            style.padding.left =
            style.padding.right =
            style.padding.top =
            style.padding.bottom = 2;
        }


        public void Draw()
        {
            if (open)
            {
                if (GUILayout.Button(openArrow, style, GUILayout.MaxWidth(15))) open = !open;
            }
            else
            {
                if (GUILayout.Button(closeArrow, style, GUILayout.MaxWidth(15))) open = !open;
            }
        }
    }
}