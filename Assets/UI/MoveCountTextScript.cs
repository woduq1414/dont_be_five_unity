using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoveCountTextScript : MonoBehaviour
{
    public Text MoveCountText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCountText.text = GameManage.moveCount.ToString();
    }
}
