using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMode
{

    public string mode;

    public static GameMode levelMode = new GameMode("levelMode");
    public static GameMode storyMode = new GameMode("storyMode");
    public static GameMode editTestMode = new GameMode("editTestMode");


    public GameMode(string _mode)
    {
        mode = _mode;
    }
}
