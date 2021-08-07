using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class EditCompletePage : MonoBehaviour
{
    public GameObject canvas;
    public GameObject starPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject goEditModeButton = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "GoEditButton")?.gameObject;
        // goEditModeButton.transform.GetChild(1).GetComponent<Text>().text = (GlobalVar.Instance.level + 1) + "레벨로 이동합니다.";
        goEditModeButton.GetComponent<Button>().onClick.AddListener(goEditMode);

        GameObject replayButton = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "ReplayButton")?.gameObject;

        GameObject uploadButton = canvas.GetComponentsInChildren<Transform>()
                                        .FirstOrDefault(c => c.gameObject.name == "UploadButton")?.gameObject;
        uploadButton.GetComponent<Button>().onClick.AddListener(goUpload);


        GameObject levelResultWrapper = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "LevelResultWrapper")?.gameObject;

        levelResultWrapper.transform.GetChild(0).GetComponent<Text>().text = "커스텀 맵";
        levelResultWrapper.transform.GetChild(1).GetComponent<Text>().text = "";

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


    public void goEditMode()
    {
        GameManage.goEditLevelMode();
        // SceneManager.LoadScene("LevelSelectScene");
    }

    public void goUpload()
    {
        // GlobalVar.Instance.setNextLevel();
        // SceneManager.LoadScene("GameScene");
        bool[] starAchievedArr = GlobalVar.Instance.gameData["achievedStar"];
        if(starAchievedArr.All(c => c == true)){
            Utils.ShowToast("업로드 구현 안 됨 ㅋ");
        } else{
            Utils.ShowToast("3별 클리어 하고 오세요 ㅋ");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
