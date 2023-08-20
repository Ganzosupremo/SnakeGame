using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour
{
    private static event Action<Sprite> OnCrosshairChanged;

    public Sprite defaultCrosshair;
    private Image cursorImage;

    private void OnEnable()
    {
        OnCrosshairChanged += CrosshairChanged;
    }

    private void OnDisable()
    {
        OnCrosshairChanged -= CrosshairChanged;
    }

    private void Awake()
    {
        Cursor.visible = false;
        cursorImage = GetComponent<Image>();
        cursorImage.sprite = defaultCrosshair;
    }

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
    }


    private void CrosshairChanged(Sprite crosshair)
    {
        ChangeCrosshair(crosshair);
    }

    private void ChangeCrosshair(Sprite crosshair)
    {
        if (crosshair != null)
            cursorImage.sprite = crosshair;
        else
            cursorImage.sprite = defaultCrosshair;
    }

    /// <summary>
    /// Changes the cursor image to the specified image.
    /// </summary>
    /// <param name="crosshair">The sprite to change the cursor image to.</param>
    public static void SetCrosshairImage(Sprite crosshair)
    {
        OnCrosshairChanged?.Invoke(crosshair);
    }
}
