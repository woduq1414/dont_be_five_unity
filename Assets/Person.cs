using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{

    public int x;
    public int y;
    public int idx;

    public int cnt;

    public bool isPlayer;

    Rigidbody rb;
    BoxCollider bc;

    Vector3 prevMovePos = new Vector3();



    Vector3 movePos = new Vector3(-1, -1, -1);
    MeshRenderer meshRenderer;
    Material material;

    public bool explode;
    public Vector3 explodeVector;

    public void init(bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }



    public void move(int _x, int _y, int _idx, int _cnt, bool undo = false)
    {
        MapInfo mapInfo = GameManage.mapInfo;
        Land[,] lands = GameManage.lands;


        float objectX;
        float objectY;
        if (_cnt == 1)
        {
            objectX = _x;
            objectY = _y;
        }
        else
        {
            float radianDegree = (float)(Mathf.PI / 180) * (360.0f / _cnt * _idx + 180.0f / _cnt);
            objectX = _x + Mathf.Cos(radianDegree) * 0.25f;
            objectY = _y + Mathf.Sin(radianDegree) * 0.25f;
        }
        prevMovePos = transform.position;
        movePos = new Vector3(objectY, mapInfo.isIsolatedLand(_x, _y) ? 5.5f + 0.3f : 6.0f + 0.3f, mapInfo.mapHeight - objectX - 1);

        if (undo == false)
        {
            x = _x;
            y = _y;
            idx = _idx;
            cnt = _cnt;
        }
        else
        {

            GameManage.isClickLocked = true;


            StartCoroutine(WaitForUndo());
            IEnumerator WaitForUndo()
            {
                yield return new WaitForSeconds(0.3f);
                movePos = prevMovePos;
                GameManage.isClickLocked = false;
            }


        }


        // transform.position = movePos;

    }

    // public void undo()
    // {
    //     transform.position = prevMovePos;
    // }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;



        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        // opacity transparent  

        bc = GetComponent<BoxCollider>();

        material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);
        IEnumerator FadeTo(float aValue, float aTime)
        {

            float alpha = material.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                // Debug.Log(Mathf.Lerp(alpha, aValue, t));
                Color newColor = new Color(material.color.r, material.color.g, material.color.b, Mathf.Lerp(alpha, aValue, t));
                material.color = newColor;
                yield return null;
            }
        }

        StartCoroutine(FadeTo(1.0f, 1.0f));

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (explode)
        {
            // rb.useGravity = true;
            // apply random vector force
            if (explodeVector.x == 0 && explodeVector.y == 0 && explodeVector.z == 0)
            {
                bc.enabled = true;

                explodeVector = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(1.0f, 1.0f)) * 1.5f;
            }


            rb.AddForceAtPosition(explodeVector, transform.position);
            rb.AddTorque(Vector3.back * 0.1f);
            return;
        }




        if (movePos != new Vector3(-1, -1, -1))
        {
            transform.position = Vector3.Slerp(transform.position, movePos, Time.deltaTime * 10);

        }



    }



}
