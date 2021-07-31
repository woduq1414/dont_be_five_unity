using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RestartButton : MonoBehaviour
{
    // Start is called before the first frame update

    Button button;



    void Start()
    {
        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);

      
    }

    void Awake()
    {

    }



    void fClick()
    {
        GameManage.restartGame();
    }

    // Update is called once per frame
    void Update()
    {
      


    }
}
