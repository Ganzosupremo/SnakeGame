using Cysharp.Threading.Tasks;
using SnakeGame.Debuging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SnakeGame.ProceduralGenerationSystem
{
    public class DungeonBuilderAsync : SingletonMonoBehaviour<DungeonBuilderAsync>
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

        /// <summary>
        /// Load The Room Node Type List
        /// </summary>
        private void LoadRoomNodeTypeList()
        {
            roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        public async UniTask<bool> BuildDungeon(GameLevelSO currentDungeonLevel)
        {
            roomTemplateList = currentDungeonLevel.roomTemplatesList;

            //Load the room template SO into the dictionary
            await LoadRoomTemplateIntoDictionary();
            dungeonBuildedSuccesfully = false;
            int dungeonBuildAttemps = 0;

            while (!dungeonBuildedSuccesfully && dungeonBuildAttemps < Settings.maxDungeonBuildAttempts)
            {
                dungeonBuildAttemps++;

                //Select a random room node graph from the node graph list 
                RoomNodeGraphSO roomNodeGraph = await SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphsList);

                int dungeonRebuildAttemptsForNodeGraph = 0;
                dungeonBuildedSuccesfully = false;

                //Loop until dungeon succesfully built or the max attemps for rebuilding the node graph had been reached
                while (!dungeonBuildedSuccesfully && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForNodeGraph)
                {
                    //Clears the dungeon room gameobjects and the room node dictionary
                    ClearDungeon();

                    dungeonRebuildAttemptsForNodeGraph++;

                    //Here is where we try to build a random dungeon from the selected node graph
                    dungeonBuildedSuccesfully = await AttemptToBuildRandomDungeon(roomNodeGraph);

                    //Put Outside the while loop if it doesn't work
                    if (dungeonBuildedSuccesfully)
                    {
                        //Instantiate room gameobjects
                        await InstantiateRoomGameobjects();
                    }
                }

            }

            return dungeonBuildedSuccesfully;
        }

        private async UniTask InstantiateRoomGameobjects()
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
                await UniTask.NextFrame();

                //Save gameobject reference
                room.InstantiatedRoom = instantiatedRoom;
            }
        }

        private async UniTask LoadRoomTemplateIntoDictionary()
        {
            //Clear room template dictionary
            roomTemplateDictionary.Clear();

            //Load room template list into dictionary
            foreach (RoomTemplateSO roomTemplate in roomTemplateList)
            {
                if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
                {
                    roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
                    await UniTask.NextFrame();
                }
                else
                    this.Log("Duplicated Room Template Key In " + roomTemplateList);
            }
        }

        /// <summary>
        /// Attemps To Randomly Generate The Dungeon For The Specified Room Node Graph
        /// </summary>
        /// <param name="roomNodeGraph">The randomly selected dungeon node graph</param>
        /// <returns>Returns True If Succeded,
        /// Returns False If Failed And Another Attempt Is Required</returns>
        private async UniTask<bool> AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
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
                this.Log("No Entrance Node, Cannot Procced");
                return false; // Dungeon Not Built
            }

            bool noRoomOverlaps = true;

            //Process the open room nodes in queue
            noRoomOverlaps = await ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

            //If all the room nodes have been processed without overlaps, then return true
            if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Process The Room Nodes In The Open Room Node Queue
        /// </summary>
        /// <param name="roomNodeGraph">The randomly selected room node graph</param>
        /// <param name="openRoomNodeQueue">The queue for the room nodes</param>
        /// <param name="noRoomOverlaps">The bool to says if there are overlaps</param>
        /// <returns>Returns True If There Are No Overlaps</returns>
        private async UniTask<bool> ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
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
                    await UniTask.NextFrame();
                }

                // if the room is the entrance mark as positioned and add to room dictionary
                if (roomNode.roomNodeType.isEntrance)
                {
                    RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                    Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                    room.IsPositioned = true;

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

        private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
        {
            throw new NotImplementedException();
        }

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

                ChildRoomIDList = CopyStringList(roomNode.childRoomNodeIDList),
                doorwayList = CopyDoorwayList(roomTemplate.doorwayList)
            };

            //Set parent id for room
            if (roomNode.parentRoomNodeIDList.Count == 0) //This means is the Entrace
            {
                room.ParentRoomID = "";
                room.IsPreviouslyVisited = true;

                //Set the entrance as the current room in the game manager
                GameManager.Instance.SetCurrentRoom(room);
            }
            else
            {
                room.ParentRoomID = roomNode.parentRoomNodeIDList[0];
            }

            // If there are no enemies to spawn in this room, then default it to be clear of enemies
            if (room.GetNumberOfItemsToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
            {
                room.IsClearOfEnemies = true;
            }

            return room;
        }

        private List<Doorway> CopyDoorwayList(List<Doorway> doorwayList)
        {
            throw new NotImplementedException();
        }

        private List<string> CopyStringList(List<string> childRoomNodeIDList)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Select a random room node graph from the list of room node graphs
        /// </summary>
        /// <returns>Returns the selected <see cref="RoomNodeGraphSO"/>.</returns>
        private async UniTask<RoomNodeGraphSO> SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphsList)
        {
            if (roomNodeGraphsList.Count > 0)
            {
                await UniTask.Delay(1000);
                return roomNodeGraphsList[UnityEngine.Random.Range(0, roomNodeGraphsList.Count)];
            }
            else
            {
                this.Log("No Room Node Graphs In List");
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

                    if (room.InstantiatedRoom != null)
                    {
                        Destroy(room.InstantiatedRoom.gameObject);
                    }
                }

                dungeonBuilderRoomDictionary.Clear();
            }
        }

        #region Getters
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
        /// Get The Doorway From The Doorway List That Has The Opposite Orientation To The Doorway
        /// </summary>
        private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
        {
            foreach (Doorway doorwayToCheck in doorwayList)
            {
                if (doorwayParent.doorOrientation == Orientation.East && doorwayToCheck.doorOrientation == Orientation.West)
                    return doorwayToCheck;
                else if (doorwayParent.doorOrientation == Orientation.West && doorwayToCheck.doorOrientation == Orientation.East)
                    return doorwayToCheck;
                else if (doorwayParent.doorOrientation == Orientation.North && doorwayToCheck.doorOrientation == Orientation.South)
                    return doorwayToCheck;
                else if (doorwayParent.doorOrientation == Orientation.South && doorwayToCheck.doorOrientation == Orientation.North)
                    return doorwayToCheck;
            }
            return null;
        }
        #endregion
    }
}
