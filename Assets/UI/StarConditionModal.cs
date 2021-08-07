using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarConditionModal : MonoBehaviour
{

    public int targetStarConditionIndex;


    public int targetIndex;

    public List<Dictionary<string, string>> targetList;

    public int limitCountValue;


    public GameObject modalWrapper;
    public Text targetText;
    public Button targetTextLeft;
    public Button targetTextRight;

    public Text limitCountText;
    public Button limitCountUp;
    public Button limitCountDown;

    public Text starConditionText;

    public Button finishButton;

    public Button modalBackground;

    public void open()
    {
        transform.gameObject.SetActive(true);
    }

    public void close()
    {
        transform.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {

        targetList = new List<Dictionary<string, string>>();
        targetList.Add(new Dictionary<string, string>() { { "caption", "이동" }, { "name", "move" } });
        targetList.Add(new Dictionary<string, string>() { { "caption", "아이템" }, { "name", "item" } });
        foreach (ItemData item in ItemData.getItemDataList())
        {
            targetList.Add(new Dictionary<string, string>() { { "caption", item.caption }, { "name", item.name } });
        }

        modalBackground = this.transform.GetComponent<Button>();
        modalBackground.onClick.AddListener(onBackgroundClicked);

        GameObject modalWrapper = Utils.FindChildren(this.transform.gameObject, "ModalWrapper");
        targetText = Utils.FindChildren(modalWrapper, "TargetText").GetComponent<Text>();
        targetTextLeft = Utils.FindChildren(modalWrapper, "TargetTextLeft").GetComponent<Button>();
        targetTextRight = Utils.FindChildren(modalWrapper, "TargetTextRight").GetComponent<Button>();
        limitCountText = Utils.FindChildren(modalWrapper, "LimitCountText").GetComponent<Text>();
        limitCountUp = Utils.FindChildren(modalWrapper, "LimitCountUp").GetComponent<Button>();
        limitCountDown = Utils.FindChildren(modalWrapper, "LimitCountDown").GetComponent<Button>();
        starConditionText = Utils.FindChildren(modalWrapper, "StarConditionText").GetComponent<Text>();
        finishButton = Utils.FindChildren(modalWrapper, "FinishButton").GetComponent<Button>();


        targetTextLeft.onClick.AddListener(onTargetTextLeftClicked);
        targetTextRight.onClick.AddListener(onTargetTextRightClicked);
        limitCountUp.onClick.AddListener(onLimitCountUpClicked);
        limitCountDown.onClick.AddListener(onLimitCountDownClicked);
        finishButton.onClick.AddListener(onFinishButtonClicked);

        string originStarCondition = GameManage.mapInfo.pStarCondition[targetStarConditionIndex];
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i]["name"] == originStarCondition.Split(' ')[0])
            {
                targetIndex = i;
            }
        }
        limitCountValue = int.Parse(originStarCondition.Split(' ')[1]);
        refreshRenderText();

    }

    void onTargetTextLeftClicked()
    {
        if (targetIndex > 0)
        {
            targetIndex--;
        }
        refreshRenderText();
    }
    void onTargetTextRightClicked()
    {
        if (targetIndex < targetList.Count - 1)
        {
            targetIndex++;
        }
        refreshRenderText();
    }

    void refreshRenderText()
    {

        targetText.text = targetList[targetIndex]["caption"];
        limitCountText.text = limitCountValue.ToString();

        starConditionText.text = GameManage.getLongStarConditionText(targetList[targetIndex]["name"] + " " + limitCountValue);
    }

    void onLimitCountUpClicked()
    {
        if (limitCountValue < 99)
        {
            limitCountValue++;
        }
        refreshRenderText();
    }
    void onLimitCountDownClicked()
    {
        if (limitCountValue > 0)
        {
            limitCountValue--;

        }
        refreshRenderText();
    }
    void onFinishButtonClicked()
    {
        GameManage.mapInfo.pStarCondition[targetStarConditionIndex] = targetList[targetIndex]["name"] + " " + limitCountValue;
        close();
    }

    // void refreshStarConditionText(){
    // starConditionText.text = "";
    // }


    public void init(int targetIndex_)
    {
        // targetText = Utils.FindChildren(modalWrapper, "TargetText").GetComponent<Text>();

        // limitCountText = Utils.FindChildren(modalWrapper, "LimitCountText").GetComponent<Text>();
        // starConditionText = Utils.FindChildren(modalWrapper, "StarConditionText").GetComponent<Text>();

        targetStarConditionIndex = targetIndex_;
        try
        {
            string originStarCondition = GameManage.mapInfo.pStarCondition[targetStarConditionIndex];
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i]["name"] == originStarCondition.Split(' ')[0])
                {
                    targetIndex = i;
                }
            }
            limitCountValue = int.Parse(originStarCondition.Split(' ')[1]);
            refreshRenderText();
        }
        catch
        {

        }


    }

    void onBackgroundClicked()
    {
        close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
