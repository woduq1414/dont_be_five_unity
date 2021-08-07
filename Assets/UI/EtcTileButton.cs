using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EtcTileButton : MonoBehaviour
{
    // Start is called before the first frame update

    public Button button;

    public Text titleText;
    public Text hintText;
    public string type;
    public string title;
    public string hint;


    void Start()
    {
        titleText = transform.GetChild(1).GetComponent<Text>(); 
        hintText = transform.GetChild(2).GetComponent<Text>();

        titleText.text = title;
        hintText.text = hint;
        

        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);
    }

    void fClick(){
        if(type == "clearMap"){

        }else if(type == "changeMapSize"){
            GameObject m = GameManage.getModalObject("MapSizeModal");
            m.GetComponent<MapSizeModal>().open();
            // m.init()
        }
    }

    public void init(string title_, string hint_, string type_){

        title = title_;
        hint = hint_;
        type = type_;

    }


    // Update is called once per frame
    void Update()
    {
     
    }
}
