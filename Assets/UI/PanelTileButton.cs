using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTileButton : MonoBehaviour
{
    // Start is called before the first frame update


    Button button;

    EditMode editMode;


    void Start()
    {
        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);
    }

    public void init(string imgName, string text, EditMode editMode_)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/MapEdit/" + imgName);
        transform.GetChild(1).GetComponent<Text>().text = text;
        editMode = editMode_;

    }

    // Update is called once per frame

    void fClick(){
        GameManage.changeSelectedEditMode(editMode);
    }


    void Update()
    {

    }
}
