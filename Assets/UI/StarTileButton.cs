using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarTileButton : MonoBehaviour
{
    // Start is called before the first frame update

    public Button button;

    public Text starConditionText;
    public Text hintText;

    int starConditionIndex;



    void Start()
    {
        starConditionText = transform.GetChild(1).GetComponent<Text>(); 
        hintText = transform.GetChild(2).GetComponent<Text>();

        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);
    }

    void fClick(){
        StarConditionModal m = GameManage.getModalObject("StarConditionModal").GetComponent<StarConditionModal>();
        m.open();
        m.init(starConditionIndex);
    }

    public void init(int starConditionIndex_)
    {
        starConditionIndex = starConditionIndex_;
        transform.GetChild(2).GetComponent<Text>().text = "탭하여 별 조건 " + (starConditionIndex + 1) + " 설정";
    }

    // Update is called once per frame
    void Update()
    {
        if(starConditionIndex != null){
            starConditionText.text = GameManage.getLongStarConditionText(
                GameManage.mapInfo.pStarCondition[starConditionIndex]
                );
        }
    }
}
