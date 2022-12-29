using System;
using UnityEditor;
using UnityEngine;

public class MyGUIStyles
{
    public GUIStyle entranceNodeStyle = new();
    public GUIStyle entranceNodeSelectedStyle = new();

    public GUIStyle exitNodeStyle = new();
    public GUIStyle exitNodeSelectedStyle = new();

    public GUIStyle roomNodeStyle = new();
    public GUIStyle roomNodeSelectedStyle = new();

    public GUIStyle bossRoomNodeStyle = new();
    public GUIStyle bossRoomNodeSelectedStyle = new();

    public GUIStyle corridorNodeStyle = new();
    public GUIStyle corridorNodeSelectedStyle = new();

    public GUIStyle chestRoomNodeStyle = new();
    public GUIStyle chestRoomNodeSelectedStyle = new();

    public GUIStyle shopRoomNodeStyle = new();
    public GUIStyle shopRoomNodeSelectedStyle = new();

    // Node layout values
    private const int NodePadding = 20; // Spacing inside the GUI element
    private const int NodeBorder = 12; // Spacing outside the GUI element
    #region Unity Editor
#if UNITY_EDITOR
    /// <summary>
    /// Initialises the GUI styles for the nodes
    /// </summary>
    public void Initialize()
    {
        SetupEntranceNodeStyle();
        SetupExitNodeStyle();
        SetupRoomNodeStyle();
        SetupBossRoomNodeStyle();
        SetupCorridorNodeStyle();
        SetupChestRoomNodeStyle();
        SetupShopRoomNodeStyle();

        void SetupEntranceNodeStyle()
        {
            entranceNodeStyle = new GUIStyle();
            entranceNodeStyle.normal.background = EditorGUIUtility.Load("node3") as Texture2D;
            entranceNodeStyle.normal.textColor = Color.white;
            entranceNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            entranceNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            entranceNodeSelectedStyle = new GUIStyle();
            entranceNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node3 on") as Texture2D;
            entranceNodeSelectedStyle.normal.textColor = Color.white;
            entranceNodeSelectedStyle.padding = entranceNodeStyle.padding;
            entranceNodeSelectedStyle.border = entranceNodeStyle.border;
        }

        void SetupExitNodeStyle()
        {
            exitNodeStyle = new GUIStyle();
            exitNodeStyle.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
            exitNodeStyle.normal.textColor = Color.white;
            exitNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            exitNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            exitNodeSelectedStyle = new GUIStyle();
            exitNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node5 on") as Texture2D;
            exitNodeSelectedStyle.normal.textColor = Color.white;
            exitNodeSelectedStyle.padding = exitNodeStyle.padding;
            exitNodeSelectedStyle.border = exitNodeStyle.border;
        }

        void SetupRoomNodeStyle()
        {
            roomNodeStyle = new GUIStyle();
            roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            roomNodeStyle.normal.textColor = Color.white;
            roomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            roomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            roomNodeSelectedStyle = new GUIStyle();
            roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
            roomNodeSelectedStyle.normal.textColor = Color.white;
            roomNodeSelectedStyle.padding = roomNodeStyle.padding;
            roomNodeSelectedStyle.border = roomNodeStyle.border;
        }

        void SetupBossRoomNodeStyle()
        {
            bossRoomNodeStyle = new GUIStyle();
            bossRoomNodeStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
            bossRoomNodeStyle.normal.textColor = Color.black;
            bossRoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            bossRoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            bossRoomNodeSelectedStyle = new GUIStyle();
            bossRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node6 on") as Texture2D;
            bossRoomNodeSelectedStyle.normal.textColor = Color.black;
            bossRoomNodeSelectedStyle.padding = bossRoomNodeStyle.padding;
            bossRoomNodeSelectedStyle.border = bossRoomNodeStyle.border;
        }

        void SetupCorridorNodeStyle()
        {
            corridorNodeStyle = new GUIStyle();
            corridorNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            corridorNodeStyle.normal.textColor = Color.white;
            corridorNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            corridorNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            corridorNodeSelectedStyle = new GUIStyle();
            corridorNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;
            corridorNodeSelectedStyle.normal.textColor = Color.white;
            corridorNodeSelectedStyle.padding = corridorNodeStyle.padding;
            corridorNodeSelectedStyle.border = corridorNodeStyle.border;
        }

        void SetupChestRoomNodeStyle()
        {
            chestRoomNodeStyle = new GUIStyle();
            chestRoomNodeStyle.normal.background = EditorGUIUtility.Load("node4") as Texture2D;
            chestRoomNodeStyle.normal.textColor = Color.black;
            chestRoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            chestRoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            chestRoomNodeSelectedStyle = new GUIStyle();
            chestRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node4 on") as Texture2D;
            chestRoomNodeSelectedStyle.normal.textColor = Color.black;
            chestRoomNodeSelectedStyle.padding = chestRoomNodeStyle.padding;
            chestRoomNodeSelectedStyle.border = chestRoomNodeStyle.border;
        }

        void SetupShopRoomNodeStyle()
        {
            shopRoomNodeStyle = new GUIStyle();
            shopRoomNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            shopRoomNodeStyle.normal.textColor = Color.red;
            shopRoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
            shopRoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

            shopRoomNodeSelectedStyle = new GUIStyle();
            shopRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node2 on") as Texture2D;
            shopRoomNodeSelectedStyle.normal.textColor = Color.red;
            shopRoomNodeSelectedStyle.padding = shopRoomNodeStyle.padding;
            shopRoomNodeSelectedStyle.border = shopRoomNodeStyle.border;
        }
    }
#endif
    #endregion Unity Editor
}
