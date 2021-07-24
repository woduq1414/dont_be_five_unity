using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Land : MonoBehaviour
{
    public int x;
    public int y;

    public bool isSelectable;
    public bool isSelected;

    public bool isItemTargetable;


    public float violateSignDegree;

    public bool isIsolated;

    public bool isConfined;




    public bool isClickLocked;

    MeshRenderer meshRenderer;
    Material material;

    public void init(int _x, int _y)
    {
        MapInfo mapInfo = GameManage.mapInfo;

        x = _x;
        y = _y;

        isSelectable = false;
        isSelected = false;
        isItemTargetable = false;

        if (mapInfo.isIsolatedLand(x, y))
        {
            isIsolated = true;
        }
        else
        {
            isIsolated = false;
        }
        if (mapInfo.isConfinedLand(x, y))
        {
            isConfined = true;
        }
        else
        {
            isConfined = false;
        }

    }

    void Update()
    {
        // is object clicked on

        if (isItemTargetable)
        {
            material.color = Color.cyan;
        }
        else
        {
            if (isIsolated)
            {
                material.color = Color.yellow;
            }
            else
            {
                if (violateSignDegree > 0)
                {
                    material.color = new Color(1, 0, 0, Mathf.Clamp(violateSignDegree, 0, 1));
                    // Debug.Log(Mathf.Clamp(violateSignDegree, 0, 1));
                }
                else
                {

                    if (isSelected)
                    {
                        material.color = Color.gray;
                    }
                    else
                    {
                        if (isSelectable)
                        {
                            material.color = Color.green;
                        }
                        else
                        {
                            if (isConfined)
                            {
                                material.color = Color.blue;
                            }
                            else
                            {
                                material.color = Color.white;
                            }


                        }
                    }
                }
            }
        }









    }

    public void onClick(bool force = false)
    {

        if (GameManage.isClickLocked && force == false)
        { // force 일때는 clickLocked 상태여도 클릭 메소드 실행
            return;
        }

        Block[,] blocks = GameManage.blocks;
        Land[,] lands = GameManage.lands;
        List<Person>[,] persons = GameManage.persons;
        MapInfo mapInfo = GameManage.mapInfo;

        LandType landType = mapInfo.getLandType(x, y);

        Debug.Log(mapInfo.getLandPeopleCount(x, y));


        bool prevIsSelected = isSelected;
        bool prevIsSelectable = isSelectable;
        bool isIsolated = mapInfo.isIsolatedLand(x, y);
        bool isViolate = false;
        // Vector2Int prevSelectedPos = Vector2Int;

        bool isMoveFinished = false;
        Vector2Int moveStartPos = Vector2Int.zero;


        if (GameManage.selectMode == SelectMode.move || GameManage.selectMode == SelectMode.normal || GameManage.selectMode == SelectMode.diagonalMove)
        {
            for (int i = 0; i < mapInfo.mapWidth; i++)
            {
                for (int j = 0; j < mapInfo.mapHeight; j++)
                {
                    if (lands[i, j] != null)
                    {
                        // Debug.Log(lands[i, j].isSelected);

                        if (lands[i, j].isSelected == true) // 이전에 Select 된 타일을 찾은 경우
                        {
                            // prevSelectedPos = new Vector2Int(i, j);

                            if (prevIsSelectable) // 만약 방금 클릭한 타일이 Selectable 한 타일이었다면
                            {

                                // 이전에 Select 된 타일의 사람을 방금 클릭한 타일로 옮긴다.




                                isMoveFinished = true;
                                moveStartPos = new Vector2Int(i, j);
                                // x, y : 이동할 위치, i, j : 선택된 위치

                                int oldCount = persons[x, y].Count;
                                int newCount = persons[x, y].Count + persons[i, j].Count;

                                int[,] prevMap = mapInfo.getCopiedMap();

                                if (mapInfo.getLandType(x, y) == LandType.block)
                                {
                                    mapInfo.map[x, y] = mapInfo.map[i, j];
                                }
                                else
                                {
                                    mapInfo.map[x, y] += newCount - oldCount;
                                }
                                mapInfo.map[i, j] = 0;

                                isViolate = GameManage.highlightAroundViolate(x, y);
                                if (isViolate)
                                {
                                    mapInfo.map = prevMap;
                                }


                                // 이동 목적지에 있는 person들의 count를 변경해준다.
                                foreach (Person person in persons[x, y])
                                {
                                    // undo : true 를 주면 다시 되돌아간다.
                                    person.move(x, y, person.idx, newCount, undo: isViolate);
                                }

                                // 이동 출발지에 있는 person들의 idx, count를 변경해준다.
                                foreach (Person person in persons[i, j])
                                {
                                    person.move(x, y, person.idx + oldCount, newCount, undo: isViolate);

                                    if (!isViolate) // 위반하지 않았을 때만 persons를 갱신한다.
                                    {
                                        persons[x, y].Add(person);
                                    }




                                }


                                if (!isViolate)
                                {
                                    persons[i, j].Clear();

                                    if(GameManage.selectMode == SelectMode.diagonalMove){
                                        GameManage.usedItemDict[ItemData.diagonal] += 1;
                                    }

                                }


                            }

                        }

                        // 나머지의 isSlectable, isSelected는 모두 false로 설정한다.

                        lands[i, j].isSelectable = false;
                        lands[i, j].isSelected = false;
                    }



                }
            }


            if (isMoveFinished == true) // 만약 이동 명령을 내렸다면 (violate 했을 때도 포함)
            {
                
                




                bool isGameFinished = mapInfo.isAllPlayerGoal();
                if (isGameFinished && !isViolate)
                {



                    Debug.Log("Game Finished");


                    StartCoroutine(WaitForNextLevel());
                    IEnumerator WaitForNextLevel()
                    {
                        yield return new WaitForSeconds(1f);
                        GlobalVar.Instance.setNextLevel();
                        SceneManager.LoadScene("GameScene");
                    }




                }
                else
                {
                    if (!isViolate)
                    {
                        GameManage.moveCount += 1;
                        // 움직이고자 하는 목적지 타일 자동 클릭    
                        lands[x, y].onClick();

                    }
                    else
                    {
                        // lands[moveStartPos.x, moveStartPos.y].onClick(force: true);

                    }


                }



                return;
                // for ()
            }

            if (isIsolated)
            {
                Debug.Log("It's isolated");
                return;
            }




            if (prevIsSelected) // 만약 클릭한 타일이 Select 된 타일일 경우
            {
                isSelected = false; // Select 해제
            }
            else
            {

                if (landType != LandType.person && landType != LandType.player)
                {
                    return;
                }


                foreach (Direction d in Direction.get4DirectionList()) // 4방향 타일에 대해 Selectable 상태 적용
                {

                    LandType targetLandType = mapInfo.getLandType(x + d.x, y + d.y);
                    if ((targetLandType == LandType.block || targetLandType == landType) && !mapInfo.isIsolatedLand(x + d.x, y + d.y))
                    {
                        if (!(mapInfo.isConfinedLand(x, y) == true && mapInfo.isConfinedLand(x + d.x, y + d.y) == false))
                        {

                            lands[x + d.x, y + d.y].isSelectable = true;
                        }

                    }
                }
                GameManage.selectMode = SelectMode.move;



                isSelected = true;
            }
            return;
        }
        else if (GameManage.selectMode == SelectMode.itemTarget)
        {
            if (isItemTargetable == true)
            {
                GameManage.useItem(x, y);
            }
            else
            {
                GameManage.cancelItem();
            }
        }








    }




    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }
}
