using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemButton : MonoBehaviour
{
    // Start is called before the first frame update

    Button button;

    ItemData item;

    Image itemImage;
    Image itemBottomImage;
    Text remainItemCountText;


    void Start()
    {
        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);

        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemBottomImage = transform.GetChild(1).GetComponent<Image>();
        remainItemCountText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    void Awake()
    {

    }

    public void init(ItemData _item)
    {
        item = _item;

        transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + item.name);
    }


    void fClick()
    {
        GameManage.selectItem(item);
        // Debug.Log("Clicked");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManage.selectedItem == item)
        {
            itemImage.color = new Color(1, 1, 1);
            itemBottomImage.color = new Color(1, 1, 1);
        }
        else
        {
            itemImage.color = new Color(0.8f, 0.8f, 0.8f);
            itemBottomImage.color = new Color(0.8f, 0.8f, 0.8f);
        }

        if (GameManage.mapInfo.items != null)
        {
            // remainItemCountText.text = (GameManage.mapInfo.items[item]).ToString();
            
            remainItemCountText.text = (GameManage.mapInfo.items[item] - GameManage.usedItemDict[item]).ToString();
        }


    }
}
