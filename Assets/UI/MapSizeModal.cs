using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSizeModal : MonoBehaviour
{




    public int widthValue;
    public int heightValue;


    public GameObject modalWrapper;
    public Text widthText;
    public Button widthUp;
    public Button widthDown;



    public Text heightText;
    public Button heightUp;
    public Button heightDown;

    public Button finishButton;

    public Button modalBackground;

    public void open()
    {
        transform.gameObject.SetActive(true);


        // refreshRenderText();
    }

    public void close()
    {
        transform.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {



        modalBackground = this.transform.GetComponent<Button>();
        modalBackground.onClick.AddListener(onBackgroundClicked);

        GameObject modalWrapper = Utils.FindChildren(this.transform.gameObject, "ModalWrapper");

        widthText = Utils.FindChildren(modalWrapper, "WidthText").GetComponent<Text>();
        widthUp = Utils.FindChildren(modalWrapper, "WidthUp").GetComponent<Button>();
        widthDown = Utils.FindChildren(modalWrapper, "WidthDown").GetComponent<Button>();
        widthUp.onClick.AddListener(onWidthUpClicked);
        widthDown.onClick.AddListener(onWidthDownClicked);

        heightText = Utils.FindChildren(modalWrapper, "HeightText").GetComponent<Text>();
        heightUp = Utils.FindChildren(modalWrapper, "HeightUp").GetComponent<Button>();
        heightDown = Utils.FindChildren(modalWrapper, "HeightDown").GetComponent<Button>();
        heightUp.onClick.AddListener(onHeightUpClicked);
        heightDown.onClick.AddListener(onHeightDownClicked);


        finishButton = Utils.FindChildren(modalWrapper, "FinishButton").GetComponent<Button>();

        finishButton.onClick.AddListener(onFinishButtonClicked);
        widthValue = GameManage.mapInfo.mapHeight;
        heightValue = GameManage.mapInfo.mapWidth;

        refreshRenderText();

    }

    void onWidthDownClicked()
    {
        if (widthValue > 4)
        {
            widthValue--;
        }
        refreshRenderText();
    }
    void onWidthUpClicked()
    {
        if (widthValue < 8)
        {
            widthValue++;
        }
        refreshRenderText();
    }

    void refreshRenderText()
    {

        widthText.text = widthValue.ToString();
        heightText.text = heightValue.ToString();
    }

    void onHeightUpClicked()
    {
        if (heightValue < 8)
        {
            heightValue++;
        }
        refreshRenderText();
    }
    void onHeightDownClicked()
    {
        if (heightValue > 4)
        {
            heightValue--;

        }
        refreshRenderText();
    }
    void onFinishButtonClicked()
    {
        close();
        GameManage.changeMapSize(widthValue, heightValue);
    }

    // void refreshStarConditionText(){
    // starConditionText.text = "";
    // }


    public void init(int targetIndex_)
    {
        // targetText = Utils.FindChildren(modalWrapper, "TargetText").GetComponent<Text>();

        // limitCountText = Utils.FindChildren(modalWrapper, "LimitCountText").GetComponent<Text>();
        // starConditionText = Utils.FindChildren(modalWrapper, "StarConditionText").GetComponent<Text>();




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
