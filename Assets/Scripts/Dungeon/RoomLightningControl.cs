using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using SnakeGame.Decorations;
using SnakeGame;

[RequireComponent(typeof(InstantiatedRoom))]
[DisallowMultipleComponent]
public class RoomLightningControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        //Suscribe to the on room change event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChange;
    }

    private void OnDisable()
    {
        //Unsuscribe to the on room change event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChange;
    }

    /// <summary>
    /// Handle the room change event
    /// </summary>
    private void StaticEventHandler_OnRoomChange(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            //Fade in the lightning on the rooms
            FadeInRoomLightning();

            instantiatedRoom.ActivateEnvironmentObjects();

            //Fade in the environment objects alongside the rooms
            FadeInEnvironmentLights();

            //Fade in the lightning also on the doors
            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }

    /// <summary>
    /// Fade In The Room Lightning
    /// </summary>
    private void FadeInRoomLightning()
    {
        //Fade in the lightning on the room tilemaps
        StartCoroutine(FadeInRoomLightningCoroutine(instantiatedRoom));
    }

    private IEnumerator FadeInRoomLightningCoroutine(InstantiatedRoom instantiatedRoom)
    {
        Material material = new(GameResources.Instance.variableLitShader);

        //instantiatedRoom.BackgroundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.Groundtilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.Decorations1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.Decorations2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.FrontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.MinimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);

            yield return null;
        }

        //Set the material back to the lit material
        //instantiatedRoom.BackgroundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.Groundtilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.Decorations1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.Decorations2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.FrontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.MinimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    ///// <summary>
    ///// Fades in the environment decoration objects 
    ///// </summary>
    private void FadeInEnvironmentLights()
    {
        Material material = new(GameResources.Instance.variableLitShader);

        Decoration[] environmentComponents = GetComponentsInChildren<Decoration>();

        foreach (Decoration environment in environmentComponents)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = material;
            }
        }

        StartCoroutine(FadeInEnvironmentRoutine(material, environmentComponents));
    }

    private IEnumerator FadeInEnvironmentRoutine(Material material, Decoration[] environmentComponents)
    {
        //Gradually fade in the lighting
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        //Set the material to the lit default one
        foreach (Decoration environment in environmentComponents)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = GameResources.Instance.litMaterial;
            }
        }
    }

    /// <summary>
    /// Fade In The Doors
    /// </summary>
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightningControl doorLightningControl = door.GetComponentInChildren<DoorLightningControl>();

            doorLightningControl.FadeInDoor(door);
        }
    }
}
