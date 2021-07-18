using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameManage : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject personPrefab;

    public GameObject playerPrefab;

    public GameObject goalPrefab;

    public static Block[,] blocks;
    public static Land[,] lands;

    public static List<Person>[,] persons;


    public static MapInfo mapInfo = new MapInfo();

    public static int moveCount = 0;


    public static bool isClickLocked;


    public void init()
    {
        moveCount = 0;

        mapInfo.init(new Dictionary<string, dynamic>{
        {"map", new int[5, 5]{
                {1, 2, 1, -1, 2},
                {4, 0, 1, 1, 999999},
                {101, 1, 1, 0, 0},
                {0, 101, -1, 1, 1},
                {101, 1, 0, -1, 2},
                }},{"mapWidth" , 5}, {"mapHeight" , 5},
                {"isolatedMap", new int[5, 5]{
                {0, 0, 0, 0, 0},
                {1, 0, 0, 0, 0},
                {0, 0, 0, 1, 0},
                {1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                }}, {"confinedMap", new int[5, 5]{
                {1, 1, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 1, 1},
                {0, 0, 0, 0, 1},
                }}

        });
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

    }


    // Start is called before the first frame update
    void Awake()
    {
        init();


        // camera 위치, 각도 설정
        Camera.main.transform.position = new Vector3(2, 14.5f, -1.5f);
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

                        block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2) + 0.5f, mapInfo.mapHeight - i - 1), Quaternion.identity);
                    }
                    else
                    {
                        block = Instantiate(blockPrefab, new Vector3(j, -(landHeight / 2), mapInfo.mapHeight - i - 1), Quaternion.identity);
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
    }
}
