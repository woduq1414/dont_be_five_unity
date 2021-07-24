using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update
    public int x;
    public int y;

    public Vector3 movePos;
    // bool isMovePosAssigned = false;


    public float tDelta = 0.0f;
    Vector3 vel = Vector3.zero;
    public void changeHeight(float height)
    {
        moveTo(new Vector3(transform.position.x, height, transform.position.z));
    }


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

        MapInfo mapInfo = GameManage.mapInfo;

        x = _x;
        y = _y;
        // Debug.Log(x + " " + y);
        transform.GetChild(0).GetComponent<Land>().init(x, y);


        if (mapInfo.isIsolatedLand(x, y) == true)
        {
            moveTo(new Vector3(transform.position.x, (6 / 2) - 0.5f, transform.position.z));
        }
        else
        {
            moveTo(new Vector3(transform.position.x, (6 / 2), transform.position.z));
        }



        // get child Object
    }

    public Land getLand()
    {
        return transform.GetChild(0).GetComponent<Land>();
    }


    public void moveTo(Vector3 pos)
    {
        StartCoroutine(LerpPosition(pos, 1.2f));
    }
    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Slerp(transform.position, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
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



        // if (isMovePosAssigned)
        // {
        //     // transform.position = Vector3.SmoothDamp(transform.position, movePos, ref vel, 1f);
        //     if (tDelta != 0)
        //     {
        //         transform.position = Vector3.Slerp(transform.position, movePos, tDelta);
        //     }

        //     // movePos = new Vector3(movePos.x, movePos.y - 0.01f, movePos.z);
        //     // Debug.Log(Vector3.Slerp(transform.position, movePos, Time.deltaTime * 3));
        // }
        // else
        // {
        //     tDelta = Time.deltaTime * 10;
        //     movePos = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        //     isMovePosAssigned = true;
        // }


    }
}
