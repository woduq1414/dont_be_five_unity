using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GameCompletePage : MonoBehaviour
{
    public GameObject canvas;
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



    }


    public void goLevelList(){
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
