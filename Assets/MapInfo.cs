using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[System.Serializable]
public class MapInfo
{
    public int mapWidth;
    public int mapHeight;
    public int[,] map;

    public int[,] isolatedMap;
    public int[,] confinedMap;

    public Dictionary<ItemData, int> items;



    public Vector2Int goalPos = new Vector2Int(0, 0);


    public void fromJson(JsonData json){
        mapWidth = (int)json["mapWidth"];
        mapHeight = (int)json["mapHeight"];

        bool isIsolatedMapExist = json.ContainsKey("isolatedMap");
        bool isConfinedMapExist = json.ContainsKey("confinedMap");

        map = new int[mapWidth, mapHeight];
        isolatedMap = new int[mapWidth, mapHeight];
        confinedMap = new int[mapWidth, mapHeight];

        for(int i = 0 ; i < mapWidth ; i++){
            for(int j = 0 ; j < mapHeight ; j++){
                map[i, j] = (int)json["map"][i][j];
                if(isIsolatedMapExist){
                    isolatedMap[i, j] = (int)json["isolatedMap"][i][j];
                }
                if(isConfinedMapExist){
                    confinedMap[i, j] = (int)json["confinedMap"][i][j];
                }
            }
        }

        items = new Dictionary<ItemData, int>();

        if(json.ContainsKey("items")){
            foreach(ItemData item in ItemData.getItemDataList()){
                if(json["items"].ContainsKey(item.name)){
                    items[item] = (int)json["items"][item.name];
                }else{
                    items[item] = 0;
                }
            }
        }




        // items = new Dictionary<string, object>();
        // printMap();
    }


    public bool isIsolatedLand(int x, int y)
    {

        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
        {
            return false;
        }

        if (isolatedMap[x, y] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isConfinedLand(int x, int y)
    {

        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
        {
            return false;
        }

        if (confinedMap[x, y] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public void init(Dictionary<string, dynamic> mapDataDict = null)
    {

        if (mapDataDict != null)
        {

            mapWidth = mapDataDict["mapWidth"];
            mapHeight = mapDataDict["mapHeight"];
            map = mapDataDict["map"];
            isolatedMap = mapDataDict["isolatedMap"];
            confinedMap = mapDataDict["confinedMap"];

        }

        // Debug.Log(items["vaccine"]);
        // foreach(string k in items.Keys){
            // Debug.Log(items[k]);
        // }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (map[i, j] == 999999)
                {
                    goalPos = new Vector2Int(i, j);
                    map[i, j] = 0;
                    break;
                }
            }
        }

        if (isolatedMap == null)
        {
            isolatedMap = new int[mapWidth, mapHeight];
        }

        if (confinedMap == null)
        {
            confinedMap = new int[mapWidth, mapHeight];
        }


        printMap();
    }

    public int[,] getCopiedMap()
    {
        int[,] copiedMap = new int[mapWidth, mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                copiedMap[i, j] = map[i, j];
            }
        }
        return copiedMap;
    }



    public int getMapValue(int x, int y)
    {

        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
        {
            return -1;
        }

        return map[x, y];
    }

    public LandType getLandType(int x, int y)
    {
        int mapValue = getMapValue(x, y);
        if (1 <= mapValue && mapValue <= 10)
        {
            return LandType.person;
        }
        else if (101 <= mapValue && mapValue <= 110)
        {
            return LandType.player;
        }
        else if (mapValue == 0)
        {
            return LandType.block;
        }
        else if (mapValue == 999999)
        {
            return LandType.goal;
        }
        else
        {
            return LandType.air;
        }
    }

    public int getLandPeopleCount(int x, int y)
    {
        LandType landType = getLandType(x, y);
        if (landType == LandType.person)
        {
            return getMapValue(x, y);
        }
        else if (landType == LandType.player)
        {
            return getMapValue(x, y) - 100;
        }
        return 0;
    }

    public bool isTargetLandViolate(int x, int y)
    {
        int peopleCountSum = 0;
        if (!isIsolatedLand(x, y))
        {
            peopleCountSum += getLandPeopleCount(x, y);
        }

        if (peopleCountSum == 0)
        {
            return false;
        }

        foreach (Direction d in Direction.get4DirectionList()) // 4방향 타일에 대해 Selectable 상태 적용
        {
            if (!isIsolatedLand(x + d.x, y + d.y))
            {
                peopleCountSum += getLandPeopleCount(x + d.x, y + d.y);
            }

        }

        return peopleCountSum >= 5;


    }

    public List<Vector2Int> getAroundViolateLandList(int x, int y)
    {
        List<Vector2Int> aroundViolateLandList = new List<Vector2Int>();
        foreach (Direction d in Direction.get5DirectionList()) // 4방향 타일에 대해 Selectable 상태 적용
        {
            if (!isIsolatedLand(x + d.x, y + d.y))
            {
                if (isTargetLandViolate(x + d.x, y + d.y))
                {
                    aroundViolateLandList.Add(new Vector2Int(x + d.x, y + d.y));
                };
            }

        }

        return aroundViolateLandList;
    }






    public bool isAllPlayerGoal()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (getLandType(i, j) == LandType.player)
                {
                    if (goalPos != new Vector2Int(i, j))
                    {
                        return false;
                    }

                }
            }
        }
        return true;
    }


    public void printMap()
    {
        string s = "";
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                s += " " + map[i, j];

            }
            s += "\n";
        }
        Debug.Log(s);
    }
}