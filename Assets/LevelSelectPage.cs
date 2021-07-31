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


public class LevelSelectPage : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject levelSelectSquarePrefab;
    public GameObject canvas;

    public GameObject levelSelectContainer;

    static int currentPage = 0;
    static int hCount = 4;
    static int vCount = 5;
    static int pageCount =1;

    // public float canvasWidth = 0;

    static Vector3 levelSelectContainerPos = new Vector3(0, 0, 0);


    void Start()
    {

        // floatcanvasWidth = 1080;

        levelSelectContainer = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "LevelSelectContainer")?.gameObject;

        float containerWidth = levelSelectContainer.GetComponent<RectTransform>().rect.width;
        float containerHeight = levelSelectContainer.GetComponent<RectTransform>().rect.height;

        float squareWidth = 220f;
        float squareHeight = 220f;

        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;


        //  0 => 0, 3 => 730

        for (int k = 0; k < pageCount; k++)
        {
            for (int i = 0; i < vCount; i++)
            {
                for (int j = 0; j < hCount; j++)
                {
                    GameObject levelSelectSquare = Instantiate(levelSelectSquarePrefab);
                    levelSelectSquare.transform.SetParent(levelSelectContainer.transform);
                    levelSelectSquare.transform.localScale = new Vector3(1f, 1f, 1f);
                    levelSelectSquare.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                        k * canvasWidth + j * (containerWidth - squareWidth) / (hCount - 1),
                        -i * (containerHeight - squareHeight) / (vCount - 1)
                        , 0);

                    int level = k * hCount * vCount + i * hCount + j + 1;
                    levelSelectSquare.GetComponent<LevelSelectSquare>().init(_level: level);

                }
            }
        }
    }


    public static void moveNextLevelPage()
    {
        if (currentPage + 1 >= pageCount)
        {
            return;
        }
        moveLevelPage(++currentPage);
    }

    public static void movePrevLevelPage()
    {
        if (currentPage <= 0)
        {
            return;
        }
        moveLevelPage(--currentPage);
    }

    public static void moveLevelPage(int page)
    {
        levelSelectContainerPos = new Vector3(-1080 * page, 0, 0);

        // test(new Vector3(-canvas.GetComponent<RectTransform>().rect.width * page, 0, 0));

    }

    // public void test(Vector3 pos)
    // {
    //     StartCoroutine(LerpPosition(pos, 1.2f));
    // }

    // IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    // {
    //     float time = 0;
    //     Vector3 startPosition = levelSelectContainer.transform.position;

    //     while (time < duration)
    //     {
    //         levelSelectContainer.transform.position = Vector3.Slerp(levelSelectContainer.transform.position, targetPosition, time / duration);
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     levelSelectContainer.transform.position = targetPosition;
    // }
    // Update is called once per frame
    void Update()
    {
        levelSelectContainer.GetComponent<RectTransform>().anchoredPosition = 
        Vector3.Lerp(levelSelectContainer.GetComponent<RectTransform>().anchoredPosition, levelSelectContainerPos, 
        Time.deltaTime * 6);

    }
}
