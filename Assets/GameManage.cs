using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
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

    private static CoroutineExecuter instance;
    public GameObject blockPrefab;
    public GameObject personPrefab;

    public GameObject playerPrefab;

    public GameObject goalPrefab;


    public GameObject canvas;

    public GameObject itemButtonPrefab;

    public GameObject starConditionSquarePrefab;


    public static Block[,] blocks;
    public static Land[,] lands;

    public static List<Person>[,] persons;


    public static MapInfo mapInfo = new MapInfo();

    public static int moveCount = 0;


    public static bool isClickLocked;

    public static ItemData selectedItem;

    public static SelectMode selectMode;

    public static Dictionary<ItemData, int> usedItemDict;



    public void test()
    {
        selectItem(ItemData.isolate);
    }



    public static void useItem(int x, int y)
    {

        selectMode = SelectMode.normal;

        for (int i = 0; i < mapInfo.mapWidth; i++)
        {
            for (int j = 0; j < mapInfo.mapHeight; j++)
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


        for (int i = 0; i < mapInfo.mapWidth; i++)
        {
            for (int j = 0; j < mapInfo.mapHeight; j++)
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

            for (int i = 0; i < mapInfo.mapWidth; i++)
            {
                for (int j = 0; j < mapInfo.mapHeight; j++)
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
                for (int i = 0; i < mapInfo.mapWidth; i++)
                {
                    for (int j = 0; j < mapInfo.mapHeight; j++)
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
                for (int i = 0; i < mapInfo.mapWidth; i++)
                {
                    for (int j = 0; j < mapInfo.mapHeight; j++)
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
                for (int i = 0; i < mapInfo.mapWidth; i++)
                {
                    for (int j = 0; j < mapInfo.mapHeight; j++)
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
                for (int i = 0; i < mapInfo.mapWidth; i++)
                {
                    for (int j = 0; j < mapInfo.mapHeight; j++)
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

        for (int i = 0; i < mapInfo.mapWidth; i++)
        {
            for (int j = 0; j < mapInfo.mapHeight; j++)
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



    public void init()
    {
        moveCount = 0;
        string json = Resources.Load("MapData").ToString();
        // Debug.Log(JsonUtility.FromJson<MapDataList>(json).levels[0].map);

        // JsonConvert.DeserializeObject<MapDataList>(json).levels[0].printMap();
        // Debug.Log("level" + GlobalVar.Instance.level);

        var jsonData = JsonMapper.ToObject(json);

        Debug.Log(jsonData["levels"][0].GetType());

        // var dict = Json.Deserialize(json) as Dictionary<string, object>;
        // List<object> levels = dict["levels"] as List<object>;
        mapInfo = new MapInfo();


        // return;

        if (GlobalVar.Instance == null)
        {
            mapInfo.fromJson(jsonData["levels"][0]);
        }
        else
        {
            mapInfo.fromJson(jsonData["levels"][GlobalVar.Instance.level - 1]);
        }

        usedItemDict = new Dictionary<ItemData, int>();

        foreach (ItemData item in ItemData.getItemDataList())
        {
            usedItemDict.Add(item, 0);
        }



        mapInfo.init();


        // mapInfo.init(new Dictionary<string, dynamic>{
        // {"map", new int[5, 5]{
        //         {1, 2, 1, -1, 2},
        //         {4, 0, 1, 1, 999999},
        //         {101, 1, 1, 0, 0},
        //         {0, 101, -1, 1, 1},
        //         {101, 1, 0, -1, 2},
        //         }},{"mapWidth" , 5}, {"mapHeight" , 5},
        //         {"isolatedMap", new int[5, 5]{
        //         {0, 0, 0, 0, 0},
        //         {1, 0, 0, 0, 0},
        //         {0, 0, 0, 1, 0},
        //         {1, 0, 0, 0, 0},
        //         {0, 0, 0, 0, 0},
        //         }}, {"confinedMap", new int[5, 5]{
        //         {1, 1, 0, 0, 0},
        //         {0, 0, 0, 0, 0},
        //         {0, 0, 0, 0, 0},
        //         {0, 0, 0, 1, 1},
        //         {0, 0, 0, 0, 1},
        //         }}

        // });
        // mapInfo.init(new Dictionary<string, dynamic>{
        // {"map", new int[4, 4]{
        //         {0, 0, 1, -1},
        //         {2, 0, 101, 999999},
        //         {0, 1, 1, -1},
        //         {-1, 1, 1, -1}
        //         }},{"mapWidth" , 4}, {"mapHeight" , 4}

        // });
        blocks = new Block[mapInfo.mapWidth, mapInfo.mapHeight];
        lands = new Land[mapInfo.mapWidth, mapInfo.mapHeight];
        persons = new List<Person>[mapInfo.mapWidth, mapInfo.mapHeight];
        for (int i = 0; i < mapInfo.mapWidth; i++)
        {
            for (int j = 0; j < mapInfo.mapHeight; j++)
            {
                persons[i, j] = new List<Person>();
                blocks[i, j] = null;
                lands[i, j] = null;
            }
        }

        selectedItem = null;
        selectMode = SelectMode.normal;


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


    public string getShortStarConditionText(string pStarCondition)
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
        }else if(splitted[0] == "move"){
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



    // Start is called before the first frame update
    void Awake()
    {
        init();


        // camera 위치, 각도 설정
        Camera.main.transform.position = new Vector3(2, 15.2f, -3.6f);
        Camera.main.transform.rotation = Quaternion.Euler(60, 0, 0);

        Debug.Log("GameManagerScript Start");
        // Debug.Log(mapData["mapHeight"]);


        int landHeight = 6;

        for (int i = 0; i < mapInfo.mapWidth; i++)
        {
            for (int j = 0; j < mapInfo.mapHeight; j++)
            {
                int mapValue = mapInfo.getMapValue(i, j);
                LandType landType = mapInfo.getLandType(i, j);
                int landPeopleCount = mapInfo.getLandPeopleCount(i, j);
                if (landType != LandType.air)
                {

                    // 땅 렌더링

                    GameObject block;
                    if (mapInfo.isIsolatedLand(i, j))
                    {
                        // isolated 공간이면 0.5만큼 아래에 위치.

                        block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2), mapInfo.mapHeight - i - 1), Quaternion.identity);
                        // block.moveTo(new Vector3(j, (landHeight / 2) - 0.5f, mapInfo.mapHeight - i - 1));
                    }
                    else
                    {
                        block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2), mapInfo.mapHeight - i - 1), Quaternion.identity);
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


                            GameObject person = Instantiate(renderPrefab, new Vector3(j, landHeight + 0.3f, mapInfo.mapHeight - i - 1), Quaternion.identity);
                            persons[i, j].Add(person.GetComponent<Person>());
                            int idx = k;
                            persons[i, j][idx].move(i, j, idx, landPeopleCount);
                            persons[i, j][idx].init(landType == LandType.player);


                        }
                    }


                }
                else
                {
                    blocks[i, j] = null;
                    lands[i, j] = null;
                }
            }
        }

        // goal 렌더링
        GameObject goal = Instantiate(goalPrefab, new Vector3(mapInfo.goalPos.y, landHeight + 0.85f, mapInfo.mapHeight - mapInfo.goalPos.x - 1), Quaternion.identity);







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
    }


    public static void restartGame(){
        SceneManager.LoadScene("GameScene");
    }
}
