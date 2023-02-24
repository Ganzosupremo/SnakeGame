using System.Collections;
using SnakeGame;
using SnakeGame.Debuging;
using UnityEngine;
using UnityEngine.InputSystem;

public static class HelperUtilities
{
    public static Camera mainCamera;

    /// <summary>
    /// Get The World Position Of The Mouse
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        //Clamp mouse position to the screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        //Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        //worldPosition.z = 0f;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public static Vector3 GetAimPositionGamepad()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Get the position of the right joystick and mouse
        Vector2 joystickScreenPosition = Gamepad.current.rightStick.ReadValue();
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 newJoystickPosition = joystickScreenPosition + mouseScreenPosition;

        //Clamp the joystick position to the screen size
        newJoystickPosition.x = Mathf.Clamp(newJoystickPosition.x, 0f, Screen.width);
        newJoystickPosition.y = Mathf.Clamp(newJoystickPosition.y, 0f, Screen.height);

        // Return the world position of the joystick
        return mainCamera.ScreenToWorldPoint(newJoystickPosition);
    }

    /// <summary>
    /// Gets The Angle In Degrees From A Vector
    /// </summary>
    /// <returns>The Angle in Degrees</returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    /// <summary>
    /// Converts An Angle To A Direction Vector
    /// </summary>
    /// <param name="angle">The angle to calculate the directional vector from</param>
    /// <returns>The Direction Vector</returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }
    
    /// <summary>
    /// Get the nearest spawn point position
    /// </summary>
    /// <param name="position">The position to get the nearest spawn point from</param>
    /// <returns></returns>
    public static Vector3 GetNearestSpawnPointPosition(Vector3 position)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new(10000f, 10000f, 0);

        // Loop theough all the room spawn points
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            //Convert the local spawn grid positions to world positions values
            Vector3 worldSpawnPosition = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            //If the distance between worldSpawnPosition and the position is less than the nearesSpawnPosition and position, then we found
            // the nearest spawn point to the passed position
            if (Vector3.Distance(worldSpawnPosition, position) < Vector3.Distance(nearestSpawnPosition, position))
            {
                //This is now the nearest spawn position
                nearestSpawnPosition = worldSpawnPosition;
            }
        }

        return nearestSpawnPosition;
    }

    /// <summary>
    /// Gets The AimDirection Enum Value From The Passed angleDegrees Variable
    /// </summary>
    /// <param name="angleDegrees">The angle in degrees to get the AimDirection from</param>
    /// <returns>The AimDirection Enum</returns>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        //Set the player direction
        //Up Right
        if (angleDegrees >= 22f && angleDegrees <= 67f)
            aimDirection = AimDirection.UpRight;
        //Up
        else if (angleDegrees > 67f && angleDegrees <= 112f)
            aimDirection = AimDirection.Up;
        //Up Left
        else if (angleDegrees > 112f && angleDegrees <= 158f)
            aimDirection = AimDirection.UpLeft;
        //Left
        else if (angleDegrees <= 180f && angleDegrees > 158f || (angleDegrees > -180f && angleDegrees <= -135f))
            aimDirection = AimDirection.Left;
        //Down
        else if (angleDegrees > -135f && angleDegrees <= -45f)
            aimDirection = AimDirection.Down;
        //Right
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees >= 0 && angleDegrees < 22f))
            aimDirection = AimDirection.Right;
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    /// <summary>
    /// Get the camera viewport lower and upper bounds
    /// </summary>
    /// <param name="worldPositionLowerBounds">The lower world lower bounds of the camera</param>
    /// <param name="worldPositionUpperBounds">The upper world bounds of the camera</param>
    /// <param name="camera">The camera to calculate the position bounds</param>
    public static void CameraWorldPositionBounds(out Vector2Int worldPositionLowerBounds, out Vector2Int worldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        worldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        worldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    /// <summary>
    /// Convert The Linear Volume Scale To Decibels
    /// </summary>
    public static float LinearToDecibels(int linearValue)
    {
        float linearScaleRange = 20f;

        //formula to convert from the linear scale to the logarithmic decibel scale
        return Mathf.Log10((float)linearValue / linearScaleRange) * 20f;
    }

    ///<summary>
    ///Empty String Debug Check
    ///</summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debuger.Log(fieldName + " Is Empty and Must Contain a Value in Object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// This Checks If There Are Null Values In Some Object
    /// </summary>
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debuger.Log(fieldName + " Is Null and Must Contain a Value in Object " + thisObject.name.ToString());
            return true;
        }

        return false;
    }

    ///<summary>
    ///List Empty Or Contains Null Value Check
    ///</summary>
    ///<returns>Returns true if the object is null, has null values or has no values at all.</returns>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debuger.Log(fieldName + " Is Null in Object " + thisObject.name.ToString());
            return true;
        }


        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debuger.Log(fieldName + " Has Null Values In Object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debuger.Log(fieldName + " Has No Values In Object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Positive Value Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True.
    /// </summary>
    /// <returns>Returns false if the value to check is less than zero when zero is not allowed.
    ///  Or returns false if zero is allowed but the value to check is less than zero</returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debuger.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debuger.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Value Float Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debuger.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debuger.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Value Double Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, double valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debuger.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debuger.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Range Debug Check - Set The bool isZeroAllowed To True If Both The Min And Max Range Can Be Zero. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMin, float valueToCheckMin,
        string fieldNameMax, float valueToCheckMax, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMin > valueToCheckMax)
        {
            Debuger.Log(fieldNameMin + "must be less or equal than " + fieldNameMax + " in Object " + thisObject.name.ToString());
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMin, valueToCheckMin, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMax, valueToCheckMax, isZeroAllowed)) error = true;

        return error;
    }

    /// <summary>
    /// Positive Range Debug Check - Set The bool isZeroAllowed To True If Both The Min And Max Range Can Be Zero. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMin, int valueToCheckMin,
        string fieldNameMax, int valueToCheckMax, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMin > valueToCheckMax)
        {
            Debuger.Log(fieldNameMin + "must be less or equal than " + fieldNameMax + " in Object " + thisObject.name.ToString());
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMin, (float)valueToCheckMin, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMax, (float)valueToCheckMax, isZeroAllowed)) error = true;

        return error;
    }
}
