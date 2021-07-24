using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectMode{

    public string mode;

    public static SelectMode normal = new SelectMode("normal");
    public static SelectMode move = new SelectMode("move");

    public static SelectMode itemTarget = new SelectMode("itemTarget");

    public static SelectMode diagonalMove = new SelectMode("diagonalMove"); 

    public SelectMode(string _mode){
        mode = _mode;
    }
}
