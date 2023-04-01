using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.ProceduralGenerationSystem
{
    [DisallowMultipleComponent]
    public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
    {
        public Dictionary<string, Room> dungeonBuilderRoomDictionary = new();

        private readonly Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new();
        private List<RoomTemplateSO> roomTemplateList = null;
        private RoomNodeTypeListSO roomNodeTypeList;
        private bool dungeonBuildedSuccesfully;

        protected override void Awake()
        {
            base.Awake();

            //Load the room node type list
            LoadRoomNodeTypeList();
        }

        private void OnEnable()
        {
            //Set dimmed material to zero
            GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
        }

        private void OnDisable()
        {
            //Set dimmed material to fully visible
            GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
        }

        /// <summary>
        /// Load The Room Node Type List
        /// </summary>
        private void LoadRoomNodeTypeList()
        {
            roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        /// <summary>
        /// Generates the random dungeon.
        /// </summary>
        /// <returns>Returns true if succeded, false if failed to generate the dungeon</returns>
        public bool GenerateDungeon(GameLevelSO currentDungeonLevel)
        {
            roomTemplateList = currentDungeonLevel.roomTemplatesList;

            //Load the room template SO into the dictionary
            LoadRoomTemplateIntoDictionary();
            dungeonBuildedSuccesfully = false;
            int dungeonBuildAttemps = 0;

            while (!dungeonBuildedSuccesfully && dungeonBuildAttemps < Settings.maxDungeonBuildAttempts)
            {
                dungeonBuildAttemps++;

                //Select a random room node graph from the node graph list 
                RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphsList);

                int dungeonRebuildAttemptsForNodeGraph = 0;
                dungeonBuildedSuccesfully = false;

                //Loop until dungeon succesfully built or the max attemps for rebuilding the node graph had been reached
                while (!dungeonBuildedSuccesfully && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForNodeGraph)
                {
                    //Clears the dungeon room gameobjects and the room node dictionary
                    ClearDungeon();

                    dungeonRebuildAttemptsForNodeGraph++;

                    //Here is where we try to build a random dungeon from the selected node graph
                    dungeonBuildedSuccesfully = AttemptToBuildRandomDungeon(roomNodeGraph);

                    //Put Outside the while loop if it doesn't work
                    if (dungeonBuildedSuccesfully)
                    {
                        //Instantiate room gameobjects
                        InstantiateRoomGameobjects();
                    }
                }

            }

            return dungeonBuildedSuccesfully;
        }

        /// <summary>
        /// Load The Room Templates Into The Dictionary
        /// </summary>
        private void LoadRoomTemplateIntoDictionary()
        {
            //Clear room template dictionary
            roomTemplateDictionary.Clear();

            //Load room template list into dictionary
            foreach (RoomTemplateSO roomTemplate in roomTemplateList)
            {
                if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
                {
                    roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
                }
                else
                {
                    Debug.Log("Duplicated Room Template Key In " + roomTemplateList);
                }
            }
        }

        /// <summary>
        /// Attemps To Randomly Generate The Dungeon For The Specified Room Node Graph
        /// </summary>
        /// <param name="roomNodeGraph">The randomly selected dungeon node graph</param>
        /// <returns>Returns True If Succeded,
        /// Returns False If Failed And Another Attempt Is Required</returns>
        private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
        {
            //Creates the Queue for the open room node graphs
            Queue<RoomNodeSO> openRoomNodeQueue = new();

            //Add entrance node to the Queue, from the room node graph
            RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

            if (entranceNode != null)
            {
                openRoomNodeQueue.Enqueue(entranceNode);
            }
            else
            {
                Debug.Log("No Entrance Node, Cannot Procced");
                return false; // Dungeon Not Built
            }

            bool noRoomOverlaps = true;

            //Process the open room nodes in queue
            noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

            //If all the room nodes have been processed without overlaps, then return true
            if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Process The Room Nodes In The Open Room Node Queue
        /// </summary>
        /// <param name="roomNodeGraph">The randomly selected room node graph</param>
        /// <param name="openRoomNodeQueue">The queue for the room nodes</param>
        /// <param name="noRoomOverlaps">The bool to says if there are overlaps</param>
        /// <returns>Returns True If There Are No Overlaps</returns>
        private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
        {
            //While theres rooms in the queue and no rooms overlaps detected
            while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
            {
                // Get next room node from open room node queue.
                RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

                // Add child Nodes to queue from room node graph (with links to this parent Room)
                foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
                {
                    openRoomNodeQueue.Enqueue(childRoomNode);
                }

                // if the room is the entrance mark as positioned and add to room dictionary
                if (roomNode.roomNodeType.isEntrance)
                {
                    RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                    Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                    room.isPositioned = true;

                    // Add room to room dictionary
                    dungeonBuilderRoomDictionary.Add(room.id, room);
                }
                // else if the room type isn't an entrance
                else
                {
                    // Else get parent room for node
                    //Debug.Log(dungeonBuilderRoomDictionary);
                    Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                    // See if room can be placed without overlaps
                    noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
                }
            }

            return noRoomOverlaps;
        }

        /// <summary>
        /// Attempts to place the room node with no overlaps
        /// </summary>
        /// <param name="roomNode">The selected room node</param>
        /// <param name="parentRoom">The parent of the selected node</param>
        /// <returns>Returns the node if there were no overlaps, returns null otherwise</returns>
        private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
        {
            // initialise and assume overlap until proven otherwise.
            bool roomOverlaps = true;

            // Do While Room Overlaps - try to place against all available doorways of the parent until
            // the room is successfully placed without overlap.
            while (roomOverlaps)
            {
                // Select random unconnected available doorway for Parent
                List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

                if (unconnectedAvailableParentDoorways.Count == 0)
                {
                    // If no more doorways to try then overlap failure.
                    return false; // room overlaps
                }

                Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

                // Get a random room template for room node that is consistent with the parent door orientation
                RoomTemplateSO roomtemplate = GetRandomRoomTemplateConsistenWithParent(roomNode, doorwayParent);

                // Create a room
                Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

                // Place the room - returns true if the room doesn't overlap
                if (PlaceTheRoom(parentRoom, doorwayParent, room))
                {
                    // If room doesn't overlap then set to false to exit while loop
                    roomOverlaps = false;

                    // Mark room as positioned
                    room.isPositioned = true;

                    // Add room to dictionary
                    dungeonBuilderRoomDictionary.Add(room.id, room);

                }
                else
                {
                    roomOverlaps = true;
                }
            }

            return true;  // no room overlaps
        }

        /// <summary>
        /// Gets A Random Child Room Template That Matches The Parent Orientation
        /// </summary>
        private RoomTemplateSO GetRandomRoomTemplateConsistenWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
        {
            RoomTemplateSO roomTemplate = null;

            //If room node is a corridor, then select random corridor with the correct orientation,
            //based on the parent doorway orientation
            if (roomNode.roomNodeType.isCorridor)
            {
                switch (doorwayParent.doorOrientation)
                {
                    case Orientation.North:
                    case Orientation.South:
                        roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                        break;

                    case Orientation.East:
                    case Orientation.West:
                        roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                        break;

                    case Orientation.None:
                        break;

                    default:
                        break;
                }
            }
            //Else select random room template
            else
            {
                roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
            }

            return roomTemplate;
        }

        /// <summary>
        /// Place The Room.
        /// </summary>
        /// <returns>Returns True If The Room Doesn't Overlap, False If It Does.</returns>
        private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
        {
            // Get current room doorway position
            Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

            // Return if no doorway in room opposite to parent doorway
            if (doorway == null)
            {
                // Just mark the parent doorway as unavailable so we don't try and connect it again
                doorwayParent.isUnavailable = true;

                return false;
            }

            // Calculate 'world' grid parent doorway position
            Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.doorPosition - parentRoom.tilemapLowerBounds;

            Vector2Int adjustment = Vector2Int.zero;

            // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
            switch (doorway.doorOrientation)
            {
                case Orientation.North:
                    adjustment = new Vector2Int(0, -1);
                    break;

                case Orientation.East:
                    adjustment = new Vector2Int(-1, 0);
                    break;

                case Orientation.South:
                    adjustment = new Vector2Int(0, 1);
                    break;

                case Orientation.West:
                    adjustment = new Vector2Int(1, 0);
                    break;

                case Orientation.None:
                    break;

                default:
                    break;
            }

            // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
            room.lowerBounds = parentDoorwayPosition + adjustment + room.tilemapLowerBounds - doorway.doorPosition;
            room.upperBounds = room.lowerBounds + room.tilemapUpperBounds - room.tilemapLowerBounds;

            Room overlappingRoom = CheckForRoomOverlap(room);

            if (overlappingRoom == null)
            {
                // mark doorways as connected & unavailable
                doorwayParent.isConnected = true;
                doorwayParent.isUnavailable = true;

                doorway.isConnected = true;
                doorway.isUnavailable = true;

                // return true to show rooms have been connected with no overlap
                return true;
            }
            else
            {
                // Just mark the parent doorway as unavailable so we don't try and connect it again
                doorwayParent.isUnavailable = true;

                return false;
            }
        }

        /// <summary>
        /// Get The Doorway From The Doorway List That Has The Opposite Orientation To The Doorway
        /// </summary>
        private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
        {
            foreach (Doorway doorwayToCheck in doorwayList)
            {
                if (doorwayParent.doorOrientation == Orientation.East && doorwayToCheck.doorOrientation == Orientation.West)
                {
                    return doorwayToCheck;
                }
                else if (doorwayParent.doorOrientation == Orientation.West && doorwayToCheck.doorOrientation == Orientation.East)
                {
                    return doorwayToCheck;
                }
                else if (doorwayParent.doorOrientation == Orientation.North && doorwayToCheck.doorOrientation == Orientation.South)
                {
                    return doorwayToCheck;
                }
                else if (doorwayParent.doorOrientation == Orientation.South && doorwayToCheck.doorOrientation == Orientation.North)
                {
                    return doorwayToCheck;
                }
            }

            return null;
        }

        /// <summary>
        /// Check For The Rooms That Overlap The Upper And Lower Bounds Parameters
        /// </summary>
        /// <param name="roomToTest"></param>
        /// <returns>Returns the Room if there are no overlaps, Returns Null otherwise</returns>
        private Room CheckForRoomOverlap(Room roomToTest)
        {
            // Iterate through all rooms
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                // skip if same room as room to test or room hasn't been positioned
                if (room.id == roomToTest.id || !room.isPositioned)
                    continue;

                // If room overlaps
                if (IsOverlappingRoom(roomToTest, room))
                {
                    return room;
                }
            }

            // Return
            return null;
        }

        /// <summary>
        /// Check If 2 Rooms Overlap With Each Other.
        /// </summary>
        /// <returns>Return True If They Overlap, False If Not.</returns>
        private bool IsOverlappingRoom(Room room1, Room room2)
        {
            bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

            bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

            if (isOverlappingX && isOverlappingY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check If Interval 1 Overlaps With Interval 2 - This Method Is Used By The IsOverlappingRoom Method, Just Above
        /// </summary>
        private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
        {
            if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get A Random Room Template From The roomTemplateList That Matches The Room Type
        /// </summary>
        /// <param name="roomNodeType"></param>
        /// <returns>Returns the matching Room Template for the Matched Room, Returns Null if no matching template was found</returns>
        private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
        {
            List<RoomTemplateSO> matchingRoomTemplateList = new();

            //Loop through room template list
            foreach (RoomTemplateSO roomTemplate in roomTemplateList)
            {
                //Add matching room templates
                if (roomTemplate.roomNodeType == roomNodeType)
                {
                    matchingRoomTemplateList.Add(roomTemplate);
                }
            }

            //Return null if list is empty
            if (matchingRoomTemplateList.Count == 0)
                return null;

            //Select random room template from the list and return
            return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
        }

        /// <summary>
        /// Get Unconnected Doorways
        /// </summary>
        private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
        {
            //Loop through the doorway list
            foreach (Doorway doorway in roomDoorwayList)
            {
                if (!doorway.isConnected && !doorway.isUnavailable)
                {
                    yield return doorway;
                }
            }
        }

        /// <summary>
        /// Creates The Room Based On The RoomTemplate And LayoutNode.
        /// </summary>
        /// <returns>Returns The Created Room</returns>
        private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
        {
            // Initialize room from template
            Room room = new()
            {
                templateID = roomTemplate.guid,
                id = roomNode.id,
                roomTilemapPrefab = roomTemplate.prefab,
                roomNodeType = roomTemplate.roomNodeType,

                normalMusic = roomTemplate.normalMusic,
                battleMusic = roomTemplate.battleMusic,

                lowerBounds = roomTemplate.lowerBounds,
                upperBounds = roomTemplate.upperBounds,
                spawnPositionArray = roomTemplate.spawnPositionArray,

                // Populate the lists that will spawn the enemies on the current room
                EnemiesByLevelList = roomTemplate.enemiesByLevelList,
                RoomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParemetersList,

                // populate the lists that will spawn the different foods on the current room
                FoodsByLevelList = roomTemplate.foodByLevelList,
                RoomLevelFoodSpawnParametersList = roomTemplate.roomFoodSpawnParametersList,

                tilemapLowerBounds = roomTemplate.lowerBounds,
                tilemapUpperBounds = roomTemplate.upperBounds,

                childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList),
                doorwayList = CopyDoorwayList(roomTemplate.doorwayList)
            };

            //Set parent id for room
            if (roomNode.parentRoomNodeIDList.Count == 0) //This means is the Entrace
            {
                room.parentRoomID = "";
                room.isPreviouslyVisited = true;

                //Set the entrance as the current room in the game manager
                GameManager.Instance.SetCurrentRoom(room);
            }
            else
            {
                room.parentRoomID = roomNode.parentRoomNodeIDList[0];
            }

            // If there are no enemies to spawn in this room, then default it to be clear of enemies
            if (room.GetNumberOfItemsToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
            {
                room.isClearOfEnemies = true;
            }

            return room;
        }

        /// <summary>
        /// Select a random room node graph from the list of room node graphs
        /// </summary>
        /// <returns>Returns the selected <see cref="RoomNodeGraphSO"/>.</returns>
        private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphsList)
        {
            if (roomNodeGraphsList.Count > 0)
            {
                return roomNodeGraphsList[UnityEngine.Random.Range(0, roomNodeGraphsList.Count)];
            }
            else
            {
                Debug.Log("No Room Node Graphs In List");
                return null;
            }
        }

        /// <summary>
        /// Creates a deep copy of the old doorway list
        /// </summary>
        /// <param name="oldDoorwayList">The old list to make a copy from</param>
        /// <returns>A new doorway list with new entries</returns>
        private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
        {
            List<Doorway> newDoorwayList = new();

            foreach (Doorway doorway in oldDoorwayList)
            {
                Doorway newDoorway = new()
                {
                    doorPosition = doorway.doorPosition,
                    doorOrientation = doorway.doorOrientation,
                    doorPrefab = doorway.doorPrefab,
                    isConnected = doorway.isConnected,
                    isUnavailable = doorway.isUnavailable,
                    doorwayStartCopyPosition = doorway.doorwayStartCopyPosition,
                    doorwayCopyTileWidth = doorway.doorwayCopyTileWidth,
                    doorwayCopyTileHeight = doorway.doorwayCopyTileHeight
                };

                newDoorwayList.Add(newDoorway);
            }

            return newDoorwayList;
        }

        /// <summary>
        /// Creates A Deep Copy Of The String List
        /// </summary>
        /// <param name="oldStringList">The old string list to make a copy from</param>
        /// <returns>A new list with new entries</returns>
        private List<string> CopyStringList(List<string> oldStringList)
        {
            List<string> newStringList = new();

            foreach (string stringValue in oldStringList)
            {
                newStringList.Add(stringValue);
            }

            return newStringList;
        }

        /// <summary>
        /// Instantiate The Dungeon Room Gameobjects From The Prefabs
        /// </summary>
        private void InstantiateRoomGameobjects()
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                //Calculate room position (remember the room instantiation position needs to be adjusted by the room template (tilemap) lower bounds)
                Vector3 roomPosition = new(room.lowerBounds.x - room.tilemapLowerBounds.x,
                    room.lowerBounds.y - room.tilemapLowerBounds.y, 0f);

                //Instantiate room
                GameObject roomGameobject = Instantiate(room.roomTilemapPrefab, roomPosition, Quaternion.identity, transform);

                //Get instantiated room component from instantiated prefab
                InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

                instantiatedRoom.room = room;

                //Initialise the instantiated room
                instantiatedRoom.Initialise(roomGameobject);

                //Save gameobject reference
                room.instantiatedRoom = instantiatedRoom;
            }
        }

        /// <summary>
        /// Get A Room Template By Room TemplateID.
        /// </summary>
        /// <returns>Returns Null If ID Doesn't Exist</returns>
        public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
        {
            if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
            {
                return roomTemplate;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get A Room By RoomID.
        /// </summary>
        /// <returns>Returns Null If ID Doesn't Exist</returns>
        public Room GetRoomByRoomID(string roomID)
        {
            if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
            {
                return room;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear The Dungeon Room Gameobjects And The <see cref="dungeonBuilderRoomDictionary"/>.
        /// </summary>
        private void ClearDungeon()
        {
            //Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
            if (dungeonBuilderRoomDictionary.Count > 0)
            {
                foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
                {
                    Room room = keyValuePair.Value;

                    if (room.instantiatedRoom != null)
                    {
                        Destroy(room.instantiatedRoom.gameObject);
                    }
                }

                dungeonBuilderRoomDictionary.Clear();
            }
        }
    }
}