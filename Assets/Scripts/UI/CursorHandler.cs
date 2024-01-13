using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorHandler
{
    public static void Default()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public static void Hand()
    {
        Texture2D texture = Resources.Load("Images/Cursors/hand", typeof(Texture2D)) as Texture2D;
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
    public static void Cross()
    {
        Texture2D texture = Resources.Load("Images/Cursors/cross", typeof(Texture2D)) as Texture2D;
        Debug.Log(Resources.Load("Images/Cursors/cross"));
        Debug.Log(texture);
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}
