using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GameCompletePage : MonoBehaviour
{
    public GameObject canvas;
    public GameObject starPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject nextLevelButton = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "NextLevelButton")?.gameObject;
        nextLevelButton.transform.GetChild(1).GetComponent<Text>().text = (GlobalVar.Instance.level + 1) + "레벨로 이동합니다.";
        nextLevelButton.GetComponent<Button>().onClick.AddListener(goNextLevel);

        GameObject replayButton = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "ReplayButton")?.gameObject;

        GameObject listButton = canvas.GetComponentsInChildren<Transform>()
                                        .FirstOrDefault(c => c.gameObject.name == "ListButton")?.gameObject;
        listButton.GetComponent<Button>().onClick.AddListener(goLevelList);


        GameObject levelResultWrapper = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "LevelResultWrapper")?.gameObject;

        levelResultWrapper.transform.GetChild(0).GetComponent<Text>().text = GlobalVar.Instance.gameData["levelType"];
        levelResultWrapper.transform.GetChild(1).GetComponent<Text>().text = GlobalVar.Instance.level.ToString() + " 단계";

        for (int i = 0; i < 3; i++)
        {
            bool isStarAchieved = GlobalVar.Instance.gameData["achievedStar"][i];
            GameObject star = Instantiate(starPrefab);
            star.transform.SetParent(levelResultWrapper.transform);
            star.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            star.GetComponent<RectTransform>().anchoredPosition = new Vector3((i - 1) * 150, -320, 0);
            star.GetComponent<Image>().color = isStarAchieved ? Colors.achievedStarColor : Colors.notAchievedStarColor;
        }


    }


    public void goLevelList()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void goNextLevel()
    {
        GlobalVar.Instance.setNextLevel();
        SceneManager.LoadScene("GameScene");

    }

    // Update is called once per frame
    void Update()
    {

    }
}
