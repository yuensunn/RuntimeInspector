
using UnityEngine;

namespace RI.UI
{
    //Draggable & Resizable Window
    public class DRWindow
    {


        public Rect windowRect { get; set; }
        public int windowID { get; private set; }
        public string windowName { get; private set; }
        public float dragOffset { get; private set; }
        public Rect localDraggableArea
        {
            get
            {
                return Rect.MinMaxRect(dragOffset, dragOffset,
                         windowRect.width - dragOffset,
                         windowRect.height - dragOffset);
            }
        }
        public Rect worldDraggableArea
        {
            get
            {
                return Rect.MinMaxRect(windowRect.xMin + dragOffset,
                            windowRect.yMin + dragOffset,
                            windowRect.xMax - dragOffset,
                            windowRect.yMax - dragOffset);
            }
        }
        public DRWindow(string windowName, Rect rect, int windowID)
        {
            this.windowID = windowID;
            this.windowName = windowName;
            this.windowRect = rect;
            this.dragOffset = 10;
        }

        public bool InDragArea(Vector2 mousePosition)
        {
            return worldDraggableArea.Contains(mousePosition);
        }
        public bool InResizeArea(Vector2 mousePosition)
        {
            return windowRect.Contains(mousePosition) && !InDragArea(mousePosition);
        }

        public Side GetClosestSide(Vector2 mousePosition)
        {
            Rect draggableArea = worldDraggableArea;
            if (mousePosition.x > draggableArea.xMax) return Side.Right;
            else if (mousePosition.x < draggableArea.xMin) return Side.Left;
            else if (mousePosition.y > draggableArea.yMax) return Side.Down;
            else if (mousePosition.y < draggableArea.yMin) return Side.Top;
            return Side.None;

        }

        public void Draw()
        {
            WindowResizer.currentResizer.Resize(this);
            windowRect = GUI.Window(windowID, windowRect, WindowFunction, windowName);
        }
        protected virtual void WindowFunction(int draw)
        {
            GUI.DragWindow(localDraggableArea);
        }
    }

    public enum Side
    {
        Top, Down, Left, Right, None
    }

    public class WindowResizer
    {
        public static WindowResizer currentResizer = new WindowResizer();
        private Texture2D horizontal;
        private Texture2D vertical;
        DRWindow selected = null;
        Side side = Side.None;
        bool isResizing = false;
        public WindowResizer()
        {
            horizontal = Resources.Load<Texture2D>("horizontal-resize-option");
            vertical = Resources.Load<Texture2D>("vertical-resizing-option");
        }
        public void Resize(DRWindow window)
        {
            if (selected != null && selected != window) return;

            Event current = Event.current;
            Rect windowRect = window.windowRect;

            if (!isResizing)
            {
                if (window.InResizeArea(current.mousePosition))
                {
                    selected = window;
                    side = window.GetClosestSide(current.mousePosition);
                    if (current.type == EventType.MouseDown) isResizing = true;
                    if (side == Side.Left || side == Side.Right)
                        Cursor.SetCursor(horizontal, new Vector2(horizontal.width / 2, horizontal.height / 2), CursorMode.ForceSoftware);
                    else if (side == Side.Top || side == Side.Down)
                        Cursor.SetCursor(vertical, new Vector2(horizontal.width / 2, horizontal.height / 2), CursorMode.ForceSoftware);
                }
                else
                {
                    Cursor.SetCursor(null, new Vector2(horizontal.width / 2, horizontal.height / 2), CursorMode.ForceSoftware);
                    selected = null;
                }
            }
            else
            {
                switch (side)
                {
                    case Side.Right:
                        windowRect.xMax = Mathf.Clamp(current.mousePosition.x + current.delta.x, windowRect.xMin + 20, Mathf.Infinity);
                        break;
                    case Side.Left:
                        windowRect.xMin = Mathf.Clamp(current.mousePosition.x + current.delta.x, Mathf.NegativeInfinity, windowRect.xMax - 20);
                        break;
                    case Side.Top:
                        windowRect.yMin = Mathf.Clamp(current.mousePosition.y + current.delta.y, Mathf.NegativeInfinity, windowRect.yMax - 20);
                        break;
                    case Side.Down:
                        windowRect.yMax = Mathf.Clamp(current.mousePosition.y + current.delta.y, windowRect.yMin + 20, Mathf.Infinity); ;
                        break;
                }
                if (current.type == EventType.MouseUp)
                {
                    selected = null;
                    isResizing = false;
                    side = Side.None;
                }


                window.windowRect = windowRect;
            }
        }

    }
}