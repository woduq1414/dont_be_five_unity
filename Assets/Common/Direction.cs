using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Direction
{

    public int x;
    public int y;

    public static Direction up = new Direction(0, 1);

    public static Direction right = new Direction(1, 0);

    public static Direction down = new Direction(0, -1);

    public static Direction left = new Direction(-1, 0);

    public static Direction stay = new Direction(0, 0);

    public static Direction rightUp = new Direction(1, 1);
    public static Direction rightDown = new Direction(1, -1);
    public static Direction leftUp = new Direction(-1, 1);
    public static Direction leftDown = new Direction(-1, -1);


    public Direction(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public static List<Direction> get4DirectionList()
    {
        return new List<Direction> { Direction.up, Direction.right, Direction.down, Direction.left };
    }

    public static List<Direction> get5DirectionList()
    {
        return new List<Direction> { Direction.stay, Direction.up, Direction.right, Direction.down, Direction.left };
    }

    public static List<Direction> getDiagonal4DirectionList()
    {
        return new List<Direction> { Direction.rightUp, Direction.rightDown, Direction.leftUp, Direction.leftDown };
    }
}