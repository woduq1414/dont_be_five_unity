using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;


public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance { get; private set; }

    public int level { get; set; }


    public JsonData jsonLevelDataList;

    public static int canvasWidth = 1080;
    public static int canvasHeight = 2160;

    public  Dictionary<string, dynamic> gameData = new Dictionary<string, dynamic>();

    public MapInfo editModeMapInfo;
    public GameMode gameMode;


    public void setNextLevel()
    {
        level++;
        if (level > 5)
        {
            level = 1;
        }
    }

    GlobalVar()
    {
        level = 1;
    }


    private void Awake()
    {

        if(jsonLevelDataList == null){
            string json = Resources.Load("MapData").ToString();
            jsonLevelDataList = JsonMapper.ToObject(json);
        }
        if(Storage.get("levelModeProgress") == null){
            bool [][] levelModeProgress = new bool[1000][];
            for(int i = 0 ; i < 1000; i++){
                levelModeProgress[i] = new bool[5];
                for(int j  = 0; j < 5; j++){
                    levelModeProgress[i][j] = false; 
                    // 0번째 : 열렸는지 여부, 1번째 : 클리어했는 지 여부, 2~4번째 : 별 획득 여부 
                }
            }
            levelModeProgress[0][0] = true;
            Storage.set("levelModeProgress", levelModeProgress);
        }
        // Debug.Log("DATA:" + Storage.get("levelModeProgress")[0][0]);




        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Update()
    {
        bool isLevelChange = false;
        //when pressed number key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            level = 1;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            level = 2;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            level = 3;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            level = 4;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            level = 5;
            isLevelChange = true;
        }
        if (isLevelChange)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}