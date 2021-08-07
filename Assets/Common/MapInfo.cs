using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[System.Serializable]
public class MapInfo
{
    public int mapHeight;
    public int mapWidth;
    public int[,] map;

    public int[,] isolatedMap;
    public int[,] confinedMap;

    public Dictionary<ItemData, int> items;

    public string[] pStarCondition;



    public Vector2Int goalPos = new Vector2Int(-1, -1);


    public void fromJson(JsonData json)
    {
        mapHeight = (int)json["mapWidth"];
        mapWidth = (int)json["mapHeight"];

        bool isIsolatedMapExist = json.ContainsKey("isolatedMap");
        bool isConfinedMapExist = json.ContainsKey("confinedMap");

        map = new int[mapHeight, mapWidth];
        isolatedMap = new int[mapHeight, mapWidth];
        confinedMap = new int[mapHeight, mapWidth];

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                map[i, j] = (int)json["map"][i][j];
                if (isIsolatedMapExist)
                {
                    isolatedMap[i, j] = (int)json["isolatedMap"][i][j];
                }
                if (isConfinedMapExist)
                {
                    confinedMap[i, j] = (int)json["confinedMap"][i][j];
                }
            }
        }

        items = new Dictionary<ItemData, int>();


        foreach (ItemData item in ItemData.getItemDataList())
        {
            if (json.ContainsKey("items"))
            {
                if (json["items"].ContainsKey(item.name))
                {
                    items[item] = (int)json["items"][item.name];
                }
                else
                {
                    items[item] = 0;
                }
            }
            else
            {
                items[item] = 0;
            }

        }


        pStarCondition = new string[3];

        for (int i = 0; i < 3; i++)
        {
            pStarCondition[i] = (string)json["pStarCondition"][i];
        }




        // items = new Dictionary<string, object>();
        // printMap();
    }


    public JsonData toJson()
    {
        Debug.Log(mapHeight + "/" + mapWidth);
        JsonData json = new JsonData();
        json["mapWidth"] = mapHeight;
        json["mapHeight"] = mapWidth;
        json["map"] = new JsonData();
        json["map"].SetJsonType(JsonType.Array);
        for (int i = 0; i < mapHeight; i++)
        {
            json["map"].Add(new JsonData());
            json["map"][i].SetJsonType(JsonType.Array);
            for (int j = 0; j < mapWidth; j++)
            {
                json["map"][i].Add(map[i, j]);
            }

        }

        if (isolatedMap != null)
        {
            json["isolatedMap"] = new JsonData();
            json["isolatedMap"].SetJsonType(JsonType.Array);
            for (int i = 0; i < mapHeight; i++)
            {
                json["isolatedMap"].Add(new JsonData());
                json["isolatedMap"][i].SetJsonType(JsonType.Array);
                for (int j = 0; j < mapWidth; j++)
                {
                    json["isolatedMap"][i].Add(isolatedMap[i, j]);
                }
            }

        }

        if (confinedMap != null)
        {
            json["confinedMap"] = new JsonData();
            json["confinedMap"].SetJsonType(JsonType.Array);
            for (int i = 0; i < mapHeight; i++)
            {
                json["confinedMap"].Add(new JsonData());
                json["confinedMap"][i].SetJsonType(JsonType.Array);
                for (int j = 0; j < mapWidth; j++)
                {
                    json["confinedMap"][i].Add(confinedMap[i, j]);
                }
            }
        }
        if (items != null)
        {
            json["items"] = new JsonData();
            foreach (ItemData item in ItemData.getItemDataList())
            {
                if (items.ContainsKey(item))
                {
                    json["items"][item.name] = items[item];
                }
                else
                {
                    json["items"][item.name] = 0;
                }
            }
        }
        if (pStarCondition != null)
        {
            json["pStarCondition"] = new JsonData();
            json["pStarCondition"].SetJsonType(JsonType.Array);
            for (int i = 0; i < 3; i++)
            {
                json["pStarCondition"].Add(pStarCondition[i]);
            }
        }
        Debug.Log(json.ToJson());
        return json;
    }


    public MapInfo getCopiedMapInfo(){
        MapInfo newMapInfo = new MapInfo();
        newMapInfo.fromJson(toJson());
        return newMapInfo;

    }


    public bool isIsolatedLand(int x, int y)
    {

        if (x < 0 || y < 0 || x >= mapHeight || y >= mapWidth)
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

        if (x < 0 || y < 0 || x >= mapHeight || y >= mapWidth)
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

            mapHeight = mapDataDict["mapWidth"];
            mapWidth = mapDataDict["mapHeight"];
            map = mapDataDict["map"];
            isolatedMap = mapDataDict["isolatedMap"];
            confinedMap = mapDataDict["confinedMap"];

        }

        // Debug.Log(items["vaccine"]);
        // foreach(string k in items.Keys){
        // Debug.Log(items[k]);
        // }

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
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
            isolatedMap = new int[mapHeight, mapWidth];
        }

        if (confinedMap == null)
        {
            confinedMap = new int[mapHeight, mapWidth];
        }


        printMap();
    }

    public int[,] getCopiedMap()
    {
        int[,] copiedMap = new int[mapHeight, mapWidth];
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                copiedMap[i, j] = map[i, j];
            }
        }
        return copiedMap;
    }



    public int getMapValue(int x, int y)
    {

        if (x < 0 || y < 0 || x >= mapHeight || y >= mapWidth)
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
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
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