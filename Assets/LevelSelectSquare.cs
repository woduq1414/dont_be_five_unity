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
public class LevelSelectSquare : MonoBehaviour
{
    Button button;
    int level;

    public GameObject starPrefab;
    public GameObject lockPrefab;


    public void init(int _level, bool[] _levelProgressInfo)
    {
        level = _level;
        transform.GetChild(0).GetComponent<Text>().text = level.ToString("00");

        bool isUnlockeded = _levelProgressInfo[0];
        bool isCleared = _levelProgressInfo[1];

        if (isUnlockeded)
        {
            Debug.Log("level" + level + " is unlocked");
            for (int i = 0; i < 3; i++)
            {
                GameObject star = Instantiate(starPrefab);
                star.transform.SetParent(transform);
                star.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                star.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                   (i - 1) * 45, -125
                    , 0);

                star.GetComponent<Image>().color = _levelProgressInfo[i + 2] ? Colors.achievedStarColor : Colors.notAchievedStarColor;
            }
        }
        else
        {
            GameObject locks= Instantiate(lockPrefab);
            locks.transform.SetParent(transform);
            locks.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            locks.GetComponent<RectTransform>().anchoredPosition = new Vector3(
               0, -125
                , 0);
        }
        if(isCleared){
            transform.GetChild(0).GetComponent<Text>().color = Colors.achievedStarColor;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        button = this.transform.GetComponent<Button>();
        // fClick();
        button.onClick.AddListener(delegate { fClick(); });





    }


    public void test()
    {
        Debug.Log("test");
    }


    void fClick()
    {
        // Debug.Log(GlobalVar.Instance.jsonLevelDataList["levels"].Count);

        if(GlobalVar.Instance.jsonLevelDataList["levels"].Count < level){
            return;
        }


        GlobalVar.Instance.level = level;
        GlobalVar.Instance.gameMode = GameMode.levelMode;
        SceneManager.LoadScene("GameScene");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Clicked level " + level);
    }
}
