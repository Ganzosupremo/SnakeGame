using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour
{
    public static GameCursor Instance { get { return instance; } }
    private static GameCursor instance;

    public Sprite defaultCrosshair;
    private Image cursorImage;

    private void Awake()
    {
        instance = this;
        Cursor.visible = false;
        cursorImage = GetComponent<Image>();
        cursorImage.sprite = defaultCrosshair;

    }

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    /// <summary>
    /// Changes the screen crosshair
    /// </summary>
    /// <param name="crosshair">The sprite to change the crosshair to</param>
    public void ChangeCrosshair(Sprite crosshair)
    {
        if (crosshair != null)
            cursorImage.sprite = crosshair;
        else
            cursorImage.sprite = defaultCrosshair;
    }
}
