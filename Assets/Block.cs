using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update
    public int x;
    public int y;

    public Vector3 movePos;
    bool isMovePosAssigned = false;

    Transform GetChildWithName(GameObject go, string childName)
    {
        Transform child = null;
        foreach (Transform t in go.GetComponentsInChildren<Transform>())
        {
            if (t.name == childName)
            {
                child = t;
                break;
            }
        }
        return child;
    }

    public void init(int _x, int _y)
    {
        x = _x;
        y = _y;
        // Debug.Log(x + " " + y);
        transform.GetChild(0).GetComponent<Land>().init(x, y);



        // get child Object
    }

    public Land getLand()
    {
        return transform.GetChild(0).GetComponent<Land>();
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (isMovePosAssigned)
        {
            transform.position = Vector3.Slerp(transform.position, movePos, Time.deltaTime * 3);
        }else{
            movePos = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
            isMovePosAssigned = true;
        }


    }
}
