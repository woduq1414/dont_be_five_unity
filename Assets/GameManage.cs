using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
// using Newtonsoft.Json;
using LitJson;
using UnityEngine.UI;

using System.Linq;

// [System.Serializable]
public class MapDataList
{
    public string version;
    public List<MapInfo> levels;
}


// [System.Serializable]
public class MapData
{


    public int seq;
    public int mapWidth;
    public int mapHeight;
    public int[,] map;

    // public int[,] confinedMap;

    public List<string> pStarCondition;
}

public class CoroutineExecuter : MonoBehaviour { }

public class GameManage : MonoBehaviour


{


    public static GameManage Instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    //Awake is always called before any Start functions

    public bool initIsEditMode = false;
    public static bool isEditMode = false;

    public static EditMode editMode = null;

    private static CoroutineExecuter instance;
    public GameObject blockPrefab;
    public GameObject personPrefab;

    public GameObject playerPrefab;

    public GameObject goalPrefab;


    public GameObject canvas;

    public GameObject itemButtonPrefab;

    public GameObject starConditionSquarePrefab;

    public static Goal goal;


    public static Block[,] blocks;
    public static Land[,] lands;

    public static List<Person>[,] persons;


    public static MapInfo mapInfo = new MapInfo();

    public static int moveCount = 0;


    public static bool isClickLocked;

    public static ItemData selectedItem;

    public static SelectMode selectMode;

    public static Dictionary<ItemData, int> usedItemDict;

    public static Dictionary<string, GameObject> modalDict;


    public void test()
    {
        selectItem(ItemData.isolate);
    }



    public static void useItem(int x, int y)
    {

        selectMode = SelectMode.normal;

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                if (lands[i, j] != null)
                {
                    lands[i, j].isItemTargetable = false;
                }


            }
        }
        if (selectedItem == ItemData.isolate)
        {
            mapInfo.isolatedMap[x, y] = 1;
            lands[x, y].isIsolated = true;
            blocks[x, y].changeHeight(2.3f);
            foreach (Person p in persons[x, y])
            {
                p.move(p.x, p.y, p.idx, p.cnt);
            }

            usedItemDict[selectedItem] += 1;

        }
        else if (selectedItem == ItemData.release)
        {
            mapInfo.isolatedMap[x, y] = 0;

            bool isViolate = GameManage.highlightAroundViolate(x, y);
            if (!isViolate)
            {
                lands[x, y].isIsolated = false;
                blocks[x, y].changeHeight(3.0f);
                foreach (Person p in persons[x, y])
                {
                    p.move(p.x, p.y, p.idx, p.cnt);
                }
            }
            else
            {
                mapInfo.isolatedMap[x, y] = 1;
            }

            usedItemDict[selectedItem] += 1;


        }
        else if (selectedItem == ItemData.vaccine)
        {
            for (int i = 0; i < persons[x, y].Count; i++)
            {
                Person p = persons[x, y][i];
                if (p.idx == mapInfo.getMapValue(x, y) - 1)
                {
                    p.gameObject.SetActive(false);
                    persons[x, y].Remove(p);
                    break;
                }
            }
            mapInfo.map[x, y] = mapInfo.map[x, y] - 1;
            foreach (Person p in persons[x, y])
            {
                p.move(p.x, p.y, p.idx, p.cnt - 1);
            }

            usedItemDict[selectedItem] += 1;
        }
        else if (selectedItem == ItemData.diagonal)
        {
            LandType landType = mapInfo.getLandType(x, y);
            foreach (Direction d in Direction.getDiagonal4DirectionList()) // 4방향 타일에 대해 Selectable 상태 적용
            {

                LandType targetLandType = mapInfo.getLandType(x + d.x, y + d.y);
                if ((targetLandType == LandType.block || targetLandType == landType) && !mapInfo.isIsolatedLand(x + d.x, y + d.y))
                {
                    if (!(mapInfo.isConfinedLand(x, y) == true && mapInfo.isConfinedLand(x + d.x, y + d.y) == false))
                    {

                        lands[x + d.x, y + d.y].isSelectable = true;
                    }

                }
            }

            lands[x, y].isSelected = true;

            selectMode = SelectMode.diagonalMove;
        }

        selectedItem = null;
    }


    public static bool highlightAroundViolate(int x, int y)
    {

        Land[,] lands = GameManage.lands;

        List<Vector2Int> aroundViolateLandList = mapInfo.getAroundViolateLandList(x, y);
        bool isViolate = aroundViolateLandList.Count > 0;

        if (isViolate)
        {
            // Debug.Log("Violate " + x + " " + y);
            // isVioloate = false;

            HashSet<Vector2Int> violateLandSet = new HashSet<Vector2Int>();

            foreach (Vector2Int v in aroundViolateLandList)
            {
                foreach (Direction d in Direction.get5DirectionList())
                {


                    if (mapInfo.getLandType((int)v.x + d.x, (int)v.y + d.y) != LandType.air)
                    {
                        lands[(int)v.x + d.x, (int)v.y + d.y].violateSignDegree += 0.2f;
                        violateLandSet.Add(new Vector2Int(v.x + d.x, v.y + d.y));
                    }


                }
            }

            unpaintViolateHighlight(violateLandSet);

        }
        return isViolate;
    }


    public static void highlightAllViolate()
    {

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {

                if (lands[i, j] != null)
                {
                    lands[i, j].violateSignDegree = 0;
                }


            }
        }

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {

                if (mapInfo.isTargetLandViolate(i, j))
                {
                    foreach (Direction d in Direction.get5DirectionList())
                    {
                        Vector2Int v = new Vector2Int(i, j);

                        if (mapInfo.getLandType((int)v.x + d.x, (int)v.y + d.y) != LandType.air)
                        {
                            lands[(int)v.x + d.x, (int)v.y + d.y].violateSignDegree += 0.2f;
                            // violateLandSet.Add(new Vector2Int(v.x + d.x, v.y + d.y));
                        }


                    }
                }


            }
        }
    }


    public static void unpaintViolateHighlight(HashSet<Vector2Int> violateLandSet)
    {
        if (!instance)
        {
            instance = FindObjectOfType<CoroutineExecuter>();

            if (!instance)
            {
                instance = new GameObject("CoroutineExecuter").AddComponent<CoroutineExecuter>();
            }
        }

        instance.StartCoroutine(WaitForUnpaint());
        IEnumerator WaitForUnpaint()
        {
            yield return new WaitForSeconds(0.5f);
            foreach (Vector2Int v in violateLandSet)
            {
                lands[(int)v.x, (int)v.y].violateSignDegree = 0;
            }
        }
    }

    public static bool selectItem(ItemData item)
    {

        // Debug.Log(mapInfo.items[item]);
        // Debug.Log(usedItemDict[item]);

        // if(mapInfo.items[item] <= usedItemDict[item]){
        //     Debug.Log("Item Limited");
        //     return false;
        // }


        int[,] map = mapInfo.map;
        int[,] isolatedMap = mapInfo.isolatedMap;


        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                LandType landType = mapInfo.getLandType(i, j);

                if (landType != LandType.air)
                {
                    lands[i, j].isItemTargetable = false;
                }
            }
        }

        if (item == selectedItem)
        {
            selectMode = SelectMode.normal;
            selectedItem = null;


            return false;
        }
        else
        {



            selectMode = SelectMode.itemTarget;
            selectedItem = item;

            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {

                    if (lands[i, j] != null)
                    {

                        lands[i, j].isSelected = false;
                        lands[i, j].isSelectable = false;

                    }
                }
            }




            if (item == ItemData.isolate)
            {
                for (int i = 0; i < mapInfo.mapHeight; i++)
                {
                    for (int j = 0; j < mapInfo.mapWidth; j++)
                    {
                        LandType landType = mapInfo.getLandType(i, j);
                        if ((landType == LandType.person || landType == LandType.player) && isolatedMap[i, j] == 0)
                        {
                            lands[i, j].isItemTargetable = true;
                        }

                    }
                }
            }
            else if (item == ItemData.release)
            {
                for (int i = 0; i < mapInfo.mapHeight; i++)
                {
                    for (int j = 0; j < mapInfo.mapWidth; j++)
                    {
                        LandType landType = mapInfo.getLandType(i, j);
                        if (isolatedMap[i, j] == 1)
                        {
                            lands[i, j].isItemTargetable = true;
                        }

                    }
                }
            }
            else if (item == ItemData.vaccine)
            {
                for (int i = 0; i < mapInfo.mapHeight; i++)
                {
                    for (int j = 0; j < mapInfo.mapWidth; j++)
                    {
                        LandType landType = mapInfo.getLandType(i, j);
                        if (landType == LandType.person)
                        {
                            lands[i, j].isItemTargetable = true;
                        }

                    }
                }
            }
            else if (item == ItemData.diagonal)
            {
                for (int i = 0; i < mapInfo.mapHeight; i++)
                {
                    for (int j = 0; j < mapInfo.mapWidth; j++)
                    {
                        LandType landType = mapInfo.getLandType(i, j);
                        if ((landType == LandType.person || landType == LandType.player) && isolatedMap[i, j] == 0)
                        {
                            lands[i, j].isItemTargetable = true;
                        }

                    }
                }
            }



            return true;
        }
    }

    public static void cancelItem()
    {

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                if (lands[i, j] != null)
                {
                    lands[i, j].isItemTargetable = false;
                }


            }
        }

        selectMode = SelectMode.normal;
        selectedItem = null;
    }

    public static void gameFinished()
    {
        GlobalVar.Instance.gameData["achievedStar"] = new bool[3];

        bool[] achievedStar = new bool[3];
        for (int i = 0; i < 3; i++)
        {
            string pStarCondition = GameManage.mapInfo.pStarCondition[i];
            string sc = pStarCondition.Trim();
            if (sc == "clear")
            {
                achievedStar[i] = true;
                continue;
            }
            string[] splitted = sc.Split(' ');
            if (splitted.Length != 2)
            {
                achievedStar[i] = false;
                continue;
            }
            if (splitted[0] == "item")
            {
                int itemCount = 0;
                foreach (int c in usedItemDict.Values)
                {
                    itemCount += c;
                }

                if (itemCount <= int.Parse(splitted[1]))
                {
                    achievedStar[i] = true;
                }
                else
                {
                    achievedStar[i] = false;
                }
                continue;

            }
            else if (splitted[0] == "move")
            {
                if (moveCount <= int.Parse(splitted[1]))
                {
                    achievedStar[i] = true;
                }
                else
                {
                    achievedStar[i] = false;
                }
                continue;
            }
            else
            {
                foreach (ItemData item in ItemData.getItemDataList())
                {
                    if (item.name == splitted[0])
                    {
                        if (usedItemDict[item] <= int.Parse(splitted[1]))
                        {
                            achievedStar[i] = true;
                        }
                        else
                        {
                            achievedStar[i] = false;
                        }
                        break;
                    }
                }
                continue;
            }

        }

        if (GlobalVar.Instance.gameMode == GameMode.levelMode)
        {
            JsonData levelModeProgress = Storage.get("levelModeProgress");
            for (int i = 0; i < 3; i++)
            {
                if ((bool)levelModeProgress[GlobalVar.Instance.level - 1][i + 2] == false)
                {
                    levelModeProgress[GlobalVar.Instance.level - 1][i + 2] = achievedStar[i];
                }
            }
            levelModeProgress[GlobalVar.Instance.level - 1][1] = true;

            Storage.set("levelModeProgress", levelModeProgress);
            for (int i = 0; i < 3; i++)
            {
                Debug.Log(
                    levelModeProgress[GlobalVar.Instance.level - 1][i + 2]
                );
            }

            unlockLevel(GlobalVar.Instance.level + 1);



            GlobalVar.Instance.gameData["achievedStar"] = achievedStar;

            GlobalVar.Instance.gameData["levelType"] = "단계모드";
            SceneManager.LoadScene("GameCompleteScene");

        }
        else if (GlobalVar.Instance.gameMode == GameMode.editTestMode)
        {
            GlobalVar.Instance.gameData["achievedStar"] = achievedStar;

            GlobalVar.Instance.gameData["levelType"] = "커스텀 맵 모드";
            SceneManager.LoadScene("EditCompleteScene");

        }





    }

    public static void unlockLevel(int level)
    {
        JsonData levelModeProgress = Storage.get("levelModeProgress");
        levelModeProgress[level - 1][0] = true;
        Storage.set("levelModeProgress", levelModeProgress);

    }



    public void init()
    {

        GameManage.isEditMode = initIsEditMode;


        moveCount = 0;
        selectedItem = null;
        selectMode = SelectMode.normal;


        if (!isEditMode)
        {

            if (GlobalVar.Instance.gameMode == GameMode.levelMode)
            {
                JsonData jsonData = GlobalVar.Instance.jsonLevelDataList;
                mapInfo = new MapInfo();
                if (GlobalVar.Instance == null)
                {
                    mapInfo.fromJson(jsonData["levels"][0]);
                }
                else
                {
                    mapInfo.fromJson(jsonData["levels"][GlobalVar.Instance.level - 1]);
                }


            }
            else if (GlobalVar.Instance.gameMode == GameMode.editTestMode)
            {
                mapInfo = GlobalVar.Instance.editModeMapInfo.getCopiedMapInfo();
            }
            usedItemDict = new Dictionary<ItemData, int>();

            foreach (ItemData item in ItemData.getItemDataList())
            {
                usedItemDict.Add(item, 0);
            }


        }
        else
        {

            if (GlobalVar.Instance == null || GlobalVar.Instance.editModeMapInfo == null)
            {
                mapInfo = new MapInfo();
                int initMapWidth = 5;
                int initMapHeight = 5;
                mapInfo.mapHeight = initMapHeight;
                mapInfo.mapWidth = initMapWidth;
                mapInfo.map = new int[initMapHeight, initMapWidth];
                mapInfo.isolatedMap = new int[initMapHeight, initMapWidth];
                mapInfo.confinedMap = new int[initMapHeight, initMapWidth];
                mapInfo.items = new Dictionary<ItemData, int>();
                foreach (ItemData item in ItemData.getItemDataList())
                {
                    mapInfo.items[item] = 0;

                }
                mapInfo.pStarCondition = new string[3] { "move 25", "move 20", "move 15" };

            }
            else
            {
                mapInfo = GlobalVar.Instance.editModeMapInfo;
            }


        }





        mapInfo.init();

        // });
        blocks = new Block[mapInfo.mapHeight, mapInfo.mapWidth];
        lands = new Land[mapInfo.mapHeight, mapInfo.mapWidth];
        persons = new List<Person>[mapInfo.mapHeight, mapInfo.mapWidth];
        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                persons[i, j] = new List<Person>();
                blocks[i, j] = null;
                lands[i, j] = null;
            }
        }





    }


    public static string getShortStarConditionText(string pStarCondition)
    {
        string result = "";
        string sc = pStarCondition.Trim();
        if (sc == "clear")
        {
            return "클리어";
        }
        string[] splitted = sc.Split(' ');
        // Debug.Log(splitted.Length + "hello");
        if (splitted.Length != 2)
        {
            return null;
        }
        if (splitted[0] == "item")
        {
            result += "아이템";
        }
        else if (splitted[0] == "move")
        {
            result += "이동";
        }
        else
        {
            foreach (ItemData item in ItemData.getItemDataList())
            {
                if (item.name == splitted[0])
                {
                    result += item.caption;
                }
            }
        }
        if (result == "")
        {
            return null;
        }
        result += " ";
        if (splitted[1] == "0")
        {
            result += "X";
        }
        else
        {
            result += splitted[1];
        }

        return result;



    }

    public static string getLongStarConditionText(string pStarCondition)
    {
        string result = "";
        string sc = pStarCondition.Trim();
        if (sc == "clear")
        {
            return "클리어";
        }
        string[] splitted = sc.Split(' ');
        // Debug.Log(splitted.Length + "hello");

        string limit = splitted[1];

        if (splitted.Length != 2)
        {
            return null;
        }
        if (splitted[0] == "item")
        {
            if (limit == "0")
            {
                result = "아이템 사용 X";
            }
            else
            {
                result = "아이템 " + limit + "개 이하로 사용";
            }

        }
        else if (splitted[0] == "move")
        {

            result = "이동 " + limit + "회 이하";
        }
        else
        {
            foreach (ItemData item in ItemData.getItemDataList())
            {
                if (item.name == splitted[0])
                {
                    if (limit == "0")
                    {
                        result += item.caption + " 사용 X";
                    }
                    result = item.caption + " " + limit + "개 이하로 사용";
                }
            }
        }
        if (result == "")
        {
            return null;
        }


        return result;



    }





    public static void goLevelTestMode()
    {
        if (GameManage.isEditMode)
        {

            if (checkIsValidMap())
            {
                GlobalVar.Instance.editModeMapInfo = mapInfo.getCopiedMapInfo();
                GlobalVar.Instance.gameMode = GameMode.editTestMode;
                SceneManager.LoadScene("GameScene");
            }


        }

    }

    public static void goEditLevelMode()
    {
        if (!GameManage.isEditMode)
        {
            SceneManager.LoadScene("MapEditScene");
        }

    }

    public static bool checkIsValidMap()
    {
        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {

                if (mapInfo.isTargetLandViolate(i, j))
                {
                    Utils.ShowToast("집합 금지에 어긋나는 타일이 있습니다!");
                    return false;
                }


            }
        }

        if (mapInfo.goalPos.x == -1 && mapInfo.goalPos.y == -1)
        {
            Utils.ShowToast("목적지가 설정되지 않았습니다!");
            return false;
        }

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                if (mapInfo.getLandType(i, j) == LandType.player)
                {
                    return true;
                }
            }
        }

        Utils.ShowToast("주인공이 배치되지 않았습니다!");
        return false;

    }



    public static void changeSelectedEditMode(EditMode editMode_)
    {
        if (editMode == editMode_)
        {
            editMode = EditMode.none;
        }
        else
        {
            editMode = editMode_;
        }
        refreshEditSelectable();
    }


    public static void refreshEditSelectable()
    {
        if (editMode == EditMode.deleteBlock)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType != LandType.air)
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.makeBlock)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType == LandType.air || mapInfo.isIsolatedLand(i, j) || mapInfo.isConfinedLand(i, j))
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.makeIsolate)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType != LandType.air)
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.makeConfine)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType != LandType.air)
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.eraser)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType == LandType.person || landType == LandType.goal || landType == LandType.player)
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.makePerson || editMode == EditMode.makePlayer)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType != LandType.air)
                    {
                        if ((editMode != EditMode.makePlayer && mapInfo.getLandType(i, j) == LandType.player)
                        || (editMode != EditMode.makePerson && mapInfo.getLandType(i, j) == LandType.person))
                        {

                        }
                        else
                        {
                            if (!(mapInfo.goalPos.x == i && mapInfo.goalPos.y == j))
                            {
                                if (mapInfo.getLandPeopleCount(i, j) < 9)
                                {
                                    lands[i, j].isEditSelectable = true;
                                }

                            }
                        }



                    }
                }
            }
        }
        else if (editMode == EditMode.makeGoal)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    lands[i, j].isEditSelectable = false;
                    LandType landType = mapInfo.getLandType(i, j);

                    if (landType == LandType.block)
                    {
                        lands[i, j].isEditSelectable = true;
                    }
                }
            }
        }
        else if (editMode == EditMode.none)
        {
            for (int i = 0; i < mapInfo.mapHeight; i++)
            {
                for (int j = 0; j < mapInfo.mapWidth; j++)
                {
                    if (lands[i, j] != null)
                    {
                        lands[i, j].isEditSelectable = false;
                    }
                }
            }
        }
    }


    public static void editExecute(int x, int y)
    {
        // bool prevIsEditSelectable = isEditSelectable;
        if (lands[x, y].isEditSelectable)
        {

            if (editMode == EditMode.deleteBlock)
            {


                Block block = blocks[x, y];
                Land land = lands[x, y];

                block.deactive();
                // lands[x, y] = null;
                mapInfo.map[x, y] = -1;
                mapInfo.isolatedMap[x, y] = 0;
                mapInfo.confinedMap[x, y] = 0;
                foreach (Person p in persons[x, y])
                {
                    p.gameObject.SetActive(false);

                }
                persons[x, y].Clear();

                if (mapInfo.goalPos.x == x && mapInfo.goalPos.y == y)
                {
                    mapInfo.goalPos = new Vector2Int(-1, -1);
                    goal.gameObject.SetActive(false);
                }

            }
            else if (editMode == EditMode.makeBlock)
            {


                Block block = blocks[x, y];
                Land land = lands[x, y];

                block.active();
                // lands[x, y] = null;
                mapInfo.map[x, y] = 0;
                mapInfo.isolatedMap[x, y] = 0;
                mapInfo.confinedMap[x, y] = 0;
                lands[x, y].isIsolated = false;
                lands[x, y].isConfined = false;

            }
            else if (editMode == EditMode.makeIsolate)
            {


                Block block = blocks[x, y];
                Land land = lands[x, y];

                if (mapInfo.isIsolatedLand(x, y))
                {
                    mapInfo.isolatedMap[x, y] = 0;
                    lands[x, y].isIsolated = false;
                }
                else
                {
                    mapInfo.isolatedMap[x, y] = 1;
                    lands[x, y].isIsolated = true;
                }




            }
            else if (editMode == EditMode.makeConfine)
            {


                Block block = blocks[x, y];
                Land land = lands[x, y];

                if (mapInfo.isConfinedLand(x, y))
                {
                    mapInfo.confinedMap[x, y] = 0;
                    lands[x, y].isConfined = false;

                }
                else
                {
                    mapInfo.confinedMap[x, y] = 1;
                    lands[x, y].isConfined = true;
                }



            }
            else if (editMode == EditMode.eraser)
            {
                LandType landType = mapInfo.getLandType(x, y);
                mapInfo.map[x, y] = 0;
                if (landType == LandType.person || landType == LandType.player)
                {

                    foreach (Person p in persons[x, y])
                    {
                        p.gameObject.SetActive(false);
                    }
                    persons[x, y].Clear();
                }
                else if (landType == LandType.goal)
                {
                    mapInfo.goalPos = new Vector2Int(-1, -1);
                    goal.gameObject.SetActive(false);
                }
            }
            else if (editMode == EditMode.makePerson || editMode == EditMode.makePlayer)
            {


                Block block = blocks[x, y];
                Land land = lands[x, y];

                if (editMode == EditMode.makePlayer && mapInfo.map[x, y] == 0)
                {
                    mapInfo.map[x, y] = 100;
                }
                mapInfo.map[x, y] += 1;
                LandType landType = editMode == EditMode.makePerson ? LandType.person : LandType.player;

                GameObject renderPrefab = landType == LandType.person ? GameManage.Instance.personPrefab : GameManage.Instance.playerPrefab;
                GameObject person = Instantiate(renderPrefab, new Vector3(y, 6.0f + 0.3f, mapInfo.mapWidth - x - 1), Quaternion.identity);




                foreach (Person p in persons[x, y])
                {
                    p.move(x, y, p.idx, p.cnt + 1);
                }



                persons[x, y].Add(person.GetComponent<Person>());
                int idx = persons[x, y].Count - 1;
                persons[x, y][idx].move(x, y, idx, persons[x, y].Count);
                persons[x, y][idx].init(landType == LandType.player);



            }
            else if (editMode == EditMode.makeGoal)
            {
                if (goal != null)
                {
                    goal.gameObject.SetActive(false);
                }
                if (mapInfo.goalPos.x != -1 && mapInfo.goalPos.y != -1)
                {
                    mapInfo.map[mapInfo.goalPos.x, mapInfo.goalPos.y] = 0;
                }


                mapInfo.goalPos = new Vector2Int(x, y);
                mapInfo.map[x, y] = 999999;
                goal = Instantiate(GameManage.Instance.goalPrefab, new Vector3(mapInfo.goalPos.y, 6.0f + 0.85f, mapInfo.mapWidth - mapInfo.goalPos.x - 1), Quaternion.identity).GetComponent<Goal>();


            }


        }
        else
        {
            GameManage.editMode = EditMode.none;
        }

        highlightAllViolate();

        GameManage.refreshEditSelectable();
    }


    public static void restartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public static GameObject getModalObject(string modalName)
    {
        return modalDict[modalName];
    }


    public static void changeMapSize(int width, int height)
    {

        MapInfo newMapInfo = mapInfo.getCopiedMapInfo();

        newMapInfo.mapHeight = height;
        newMapInfo.mapWidth = width;
        newMapInfo.map = new int[newMapInfo.mapHeight, newMapInfo.mapWidth];
        newMapInfo.isolatedMap = new int[newMapInfo.mapHeight, newMapInfo.mapWidth];
        newMapInfo.confinedMap = new int[newMapInfo.mapHeight, newMapInfo.mapWidth];
        newMapInfo.goalPos = new Vector2Int(-1, -1);

        GlobalVar.Instance.editModeMapInfo = newMapInfo.getCopiedMapInfo();

        SceneManager.LoadScene("MapEditScene");
    }


    // Start is called before the first frame update
    void Awake()
    {
        modalDict = new Dictionary<string, GameObject>();
        //Check if instance already exists
        if (Instance == null)
        {
            //if not, set instance to this
            Instance = this;
        }
        //If instance already exists and it's not this:
        else if (Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        // get all children of canvas

        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            Transform target = canvas.transform.GetChild(i);

            if (target.gameObject.tag == "Modal")
            {
                modalDict[target.gameObject.name] = target.gameObject;

                target.gameObject.SetActive(false);
            }
        }


        init();


        // camera 위치, 각도 설정



        Camera.main.transform.position = new Vector3(2, 15.2f, -3.6f);
        if (isEditMode)
        {
            Camera.main.transform.position = new Vector3(2, 15.2f, -5.8f);
        }
        Camera.main.transform.rotation = Quaternion.Euler(60, 0, 0);

        Debug.Log("GameManagerScript Start");
        // Debug.Log(mapData["mapHeight"]);


        int landHeight = 6;

        for (int i = 0; i < mapInfo.mapHeight; i++)
        {
            for (int j = 0; j < mapInfo.mapWidth; j++)
            {
                int mapValue = mapInfo.getMapValue(i, j);
                LandType landType = mapInfo.getLandType(i, j);
                int landPeopleCount = mapInfo.getLandPeopleCount(i, j);
                GameObject block;
                if (mapInfo.isIsolatedLand(i, j) && !GameManage.isEditMode)
                {
                    // isolated 공간이면 0.5만큼 아래에 위치.

                    block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2), mapInfo.mapWidth - i - 1), Quaternion.identity);
                    // block.moveTo(new Vector3(j, (landHeight / 2) - 0.5f, mapInfo.mapHeight - i - 1));
                }
                else
                {
                    block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2), mapInfo.mapWidth - i - 1), Quaternion.identity);
                    // block.moveTo(new Vector3(j, (landHeight / 2), mapInfo.mapHeight - i - 1));
                }


                blocks[i, j] = block.GetComponent<Block>();
                blocks[i, j].init(i, j);
                lands[i, j] = blocks[i, j].getLand();

                // person, player 렌더링
                if (landType == LandType.person || landType == LandType.player)

                {
                    GameObject renderPrefab = landType == LandType.person ? personPrefab : playerPrefab;
                    for (int k = 0; k < landPeopleCount; k++)
                    {


                        GameObject person = Instantiate(renderPrefab, new Vector3(j, landHeight + 0.3f, mapInfo.mapWidth - i - 1), Quaternion.identity);
                        persons[i, j].Add(person.GetComponent<Person>());
                        int idx = k;
                        persons[i, j][idx].move(i, j, idx, landPeopleCount);
                        persons[i, j][idx].init(landType == LandType.player);


                    }
                }
                if (landType == LandType.air)
                {

                    // 땅 렌더링
                    block.GetComponent<Block>().deactive();



                }

            }
        }

        // goal 렌더링
        if (!(mapInfo.goalPos.x == -1 && mapInfo.goalPos.y == -1))
        {

            goal = Instantiate(goalPrefab, new Vector3(mapInfo.goalPos.y, landHeight + 0.85f, mapInfo.mapWidth - mapInfo.goalPos.x - 1), Quaternion.identity).GetComponent<Goal>();

        }


        if (!isEditMode)
        {
            int k = 0;
            var availableItemList = new List<ItemData>(mapInfo.items.Keys).Where(x => mapInfo.items[x] != 0).ToList();

            foreach (ItemData item in availableItemList)
            {
                GameObject itemButton = Instantiate(itemButtonPrefab);
                itemButton.transform.SetParent(canvas.transform);
                itemButton.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector3 buttonPos = new Vector3(-(availableItemList.Count - 1) * (250 / 2) + k * 250,
                 -GlobalVar.canvasHeight / 2 + 200, 0f);

                itemButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;
                itemButton.GetComponent<ItemButton>().init(item);

                k++;
            }


            GameObject upperStatusContainer = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "UpperStatusContainer")?.gameObject;

            for (int i = 0; i < 3; i++)
            {
                GameObject starConditionSquare = Instantiate(starConditionSquarePrefab);
                starConditionSquare.transform.SetParent(upperStatusContainer.transform);
                starConditionSquare.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                starConditionSquare.GetComponent<RectTransform>().anchoredPosition = new Vector3(-(2 - i) * 180 - 50, -12, 0);
                starConditionSquare.transform.GetChild(1).GetComponent<Text>().text = getShortStarConditionText(mapInfo.pStarCondition[i]);
            }

        }





        // Instantiate(landPrefab, new Vector3(0, 0, 0), Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.tag == "Land") // land 클릭했을 때
                {

                    hit.collider.gameObject.GetComponent<Land>().onClick();

                }
            }
        }

        // on space down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("GameScene");
        }

        // on press j
        if (Input.GetKeyDown(KeyCode.H))
        {
            selectItem(ItemData.isolate);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            selectItem(ItemData.release);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            selectItem(ItemData.vaccine);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            selectItem(ItemData.diagonal);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            changeSelectedEditMode(EditMode.deleteBlock);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            changeSelectedEditMode(EditMode.makeBlock);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            changeSelectedEditMode(EditMode.makeIsolate);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            changeSelectedEditMode(EditMode.makeConfine);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            changeSelectedEditMode(EditMode.eraser);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            changeSelectedEditMode(EditMode.makePlayer);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            changeSelectedEditMode(EditMode.makePerson);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            changeSelectedEditMode(EditMode.makeGoal);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            goLevelTestMode();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            goEditLevelMode();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goEditLevelMode();
        }
    }


}
