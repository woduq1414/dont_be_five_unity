using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using SSTools;

public class GoLevelTestButton : MonoBehaviour
{
    // Start is called before the first frame update
    Button button;
    void Start()
    {
        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);

    }

    void fClick(){
        GameManage.goLevelTestMode();
        
    }


    // Update is called once per frame
    void Update()
    {

    }
}
