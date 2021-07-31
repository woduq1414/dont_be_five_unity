using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using LitJson;
using UnityEngine.UI;

using System.Linq;
public class LevelSelectSquare : MonoBehaviour
{
    Button button;
    int level;

    public void init(int _level)
    {
        level = _level;
        transform.GetChild(0).GetComponent<Text>().text = level.ToString("00");
    }

    // Start is called before the first frame update
    void Start()
    {
        button = this.transform.GetComponent<Button>();
        // fClick();
        button.onClick.AddListener(delegate{fClick();});
    }


    public void test(){
        Debug.Log("test");
    }


    void fClick()
    {
        // Debug.Log("Clicked level");
        GlobalVar.Instance.level = level;
        SceneManager.LoadScene("GameScene");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Clicked level " + level);
    }
}
