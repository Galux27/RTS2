using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacementDebugger : MonoBehaviour
{
    public BuildingTemplate template;
    public RoomTemplate room;
    public GeneratedBuilding building;
    public Vector2Int size;

    public ConstructableObjectInstance testObject;

    RoomObjectPlacement placement;
    public bool TestPositions = false;

    public bool GenerateBuidling = false;

    public List<RoomTemplateProp> Props;


    public EnvironmentObject ToPlace;
    public Vector2Int coords;
    public bool PlaceObject;

    private void Update()
    {
        if (GenerateBuidling)
        {
            GenerateBuidling = false;
            GenerateTestBuilding();
        }

        if (building != null)
        {
            if (TestPositions)
            {

                TestPositions = false;
                Props = new List<RoomTemplateProp>();

                foreach(KeyValuePair<string,ConstructableObject> kvp in ConstructableObjectManager.Instance.AllObjects)
                {
                    RoomTemplateProp prop = new RoomTemplateProp();
                    prop.PropName=kvp.Key;
                    prop.MaxQuantity = 1;
                    Props.Add(prop);
                }
                TestObjectPositions();
            }

            if (PlaceObject&&placement!=null)
            {
                PlaceObject = false;
                placement.OnObjectPlaced(coords, ToPlace.Name, room, building.MyRooms[0], building);
                TestPositions = true;
            }
        }
    }

    void TestObjectPositions()
    {
        placement = new RoomObjectPlacement(room, building.MyRooms[0], building,Props);
        RoomTileDebug.DebugDrawTileAvailablityForProps(placement, building);

    }

    public void GenerateTestBuilding()
    {
        Vector2Int camPos = new Vector2Int((int)CameraController.Instance.transform.position.x,
          (int)CameraController.Instance.transform.position.y);
        building = new GeneratedBuilding(size.x, size.y, camPos, template.BuildingName);
        GeneratedRoom r = new GeneratedRoom(size, Vector2Int.zero, room.RoomID, 0);


        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                r.RoomTiles[x, y].SetFloor(room.Floor);
                
                if(x==size.x/2 && y == size.y - 1)
                {
                    r.RoomTiles[x, y].SetDoor("Wood");
                }
            }
        }



        building.AddRoom(r);



        building.ShiftAllFloorTiles(template);

        building.UpdateEdgeTiles();
        BuildingWallMap exterior = BuildingFloorplan.PopulateExteriorWallTiles(building);
        BuildingWallMap interior = BuildingFloorplan.PopulateRoomInteriorWalls(building);
        building.GenerateWindows(exterior);
        building.GenerateExteriorDoors(exterior);
        building.GenerateInteriorDoors(interior);

        //have the above methods update room tiles to keep track of the walls/doors in generated rooms
        for (int x = 0; x < building.MyRooms.Count; x++)
        {
            building.MyRooms[x].RefreshTileWeights();
        }

        r.RefreshTileWeights();
        Dictionary<Vector2Int, GeneratedBuilding> splits = BuildingSplitUtil.SplitBuildingIntoChunks(building);
        foreach (KeyValuePair<Vector2Int, GeneratedBuilding> kvp in splits)
        {
            BuildingGenerator.Instance.ApplyBuidlingToWorld(kvp.Value);

        }

    }
}
