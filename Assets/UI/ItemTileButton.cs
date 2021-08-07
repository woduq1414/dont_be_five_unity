using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemTileButton : MonoBehaviour
{
    // Start is called before the first frame update

    ItemData item;

    Button downButton;
    Button upButton;

    Text countText;


    void Start()
    {
        downButton = transform.GetChild(3).GetComponent<Button>();
        downButton.onClick.AddListener(downCount);
        upButton = transform.GetChild(4).GetComponent<Button>();
        upButton.onClick.AddListener(upCount);

        countText = transform.GetChild(5).GetComponent<Text>();
    }

    public void init(string imgName, string name, string description, ItemData item_)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/MapEdit/" + imgName);
        transform.GetChild(1).GetComponent<Text>().text = name;
        transform.GetChild(2).GetComponent<Text>().text = description;
        // transform.GetChild(5).GetComponent<Text>().text = description;
        item = item_;

    }


    void downCount(){
        if( GameManage.mapInfo.items[item] > 0 ){
            GameManage.mapInfo.items[item]--;
        }
    }

    void upCount(){
        Debug.Log("SFDFD");
        if( GameManage.mapInfo.items[item] <= 8 ){
            GameManage.mapInfo.items[item]++;
        }
    }




    // Update is called once per frame
    void Update()
    {
        if(item != null){
            countText.text = GameManage.mapInfo.items[item].ToString();
        }
    }
}
