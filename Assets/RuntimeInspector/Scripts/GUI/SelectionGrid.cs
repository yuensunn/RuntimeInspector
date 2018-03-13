using UnityEngine;

namespace RI
{
    public class SelectionGrid
    {
        GUIStyle listStyle;
        public int selected { get; set; }
        public string[] options { get; set; }

        public System.Action<int, string> OnOptionChange;

        public SelectionGrid()
        {
            listStyle = new GUIStyle();
            listStyle.normal.textColor = Color.white;
            listStyle.hover.background = new Texture2D(2, 2);
            listStyle.onNormal.textColor = Color.white;
            listStyle.onNormal.background = new Texture2D(2, 2).SetColor(new Color(0, 0, 0.2f, 0.5f));
            listStyle.padding.left =
            listStyle.padding.right =
            listStyle.padding.top =
            listStyle.padding.bottom = 4;
            listStyle.onFocused.textColor = Color.blue;
        }


        public void SetSelection(int newSelected)
        {
            selected = newSelected;
            if (OnOptionChange != null) OnOptionChange(selected, options[selected]);
        }



        public void Draw()
        {
            if (options != null)
            {
                int newSelected = GUILayout.SelectionGrid(selected, options, 1, listStyle);
                if (newSelected != selected)
                {
                    SetSelection(newSelected);
                }
            }
        }
    }
}