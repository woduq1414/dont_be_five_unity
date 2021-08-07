using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditMode
{

    public string mode;

    public static EditMode none = new EditMode("none");
    public static EditMode deleteBlock = new EditMode("deleteBlock");

    public static EditMode makeBlock = new EditMode("makeBlock");
    public static EditMode makeIsolate = new EditMode("makeIsolate");

    public static EditMode makeConfine = new EditMode("makeConfine");


    public static EditMode eraser = new EditMode("eraser");
    public static EditMode makePlayer = new EditMode("makePlayer");
    public static EditMode makePerson = new EditMode("makePerson");

    public static EditMode makeGoal = new EditMode("makeGoal");

    public EditMode(string _mode)
    {
        mode = _mode;
    }
}
