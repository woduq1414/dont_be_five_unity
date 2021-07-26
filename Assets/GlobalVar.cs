using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance { get; private set; }

    public int level { get; set; }

    public static int canvasWidth = 1080;
    public static int canvasHeight = 2160;


    public void setNextLevel()
    {
        level++;
        if (level >= 5)
        {
            level = 0;
        }
    }

    GlobalVar()
    {
        level = 0;
    }


    private void Awake()
    {



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
            level = 0;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            level = 1;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            level = 2;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            level = 3;
            isLevelChange = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            level = 4;
            isLevelChange = true;
        }
        if (isLevelChange)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}