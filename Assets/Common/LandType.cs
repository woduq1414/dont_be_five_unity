using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandType
{

    public string type;

    public static LandType air = new LandType("air");


    public static LandType block = new LandType("block");

    public static LandType person = new LandType("person");

    public static LandType player = new LandType("player");
    public static LandType goal = new LandType("goal");


    public LandType(string _type)
    {
        type = _type;
    }
}