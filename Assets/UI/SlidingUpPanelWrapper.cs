using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class SlidingUpPanelWrapper : MonoBehaviour
{

    public static SlidingUpPanelWrapper Instance = null;


    public GameObject canvas;
    public string[] categoryNameList;

    public GameObject categoryPrefab;



    public GameObject listViewPrefab;

    public GameObject[] listViewArray;

    public GameObject panelTileButtonPrefab;

    public GameObject itemTileButtonPrefab;
    public GameObject starTileButtonPrefab;

    public GameObject etcTileButtonPrefab;


    public int selectedCategoryIndex = 0;
    GameObject slidingUpSwitchWrapper;

    public GameObject panelContainer;
    public RectTransform panelContentRectTransform;

    // Start is called before the first frame update


    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)
        {
            //if not, set instance to this
            Instance = this;
        }
        //If instance already exists and it's not this:
        else if (Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

    }

    void Start()
    {
        GameObject slidingUpPanelWrapper = canvas.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "SlidingUpPanelWrapper")?.gameObject;
        GameObject slidingUpPanel = slidingUpPanelWrapper.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == "SlidingUpPanel")?.gameObject;
        GameObject categoryContainer = slidingUpPanel.GetComponentsInChildren<Transform>()
        .FirstOrDefault(c => c.gameObject.name == "CategoryContainer")?.gameObject;

        panelContainer = slidingUpPanel.GetComponentsInChildren<Transform>()
         .FirstOrDefault(c => c.gameObject.name == "PanelContainer")?.gameObject;

        panelContentRectTransform = panelContainer.GetComponent<RectTransform>();
        slidingUpSwitchWrapper = slidingUpPanelWrapper.GetComponentsInChildren<Transform>()
        .FirstOrDefault(c => c.gameObject.name == "SlidingUpSwitchWrapper")?.gameObject;

        slidingUpSwitchWrapper.GetComponent<SlidingUpSwitchWrapper>().init(
            slidingUpPanelWrapper
        );

        categoryNameList = new string[6]{
            "타일", "오브젝트", "아이템", "별 조건", "배경", "기타"
        };
        float categoryContainerWidth = categoryContainer.GetComponent<RectTransform>().rect.width;
        float categoryWidth = 1080 / 7;

        listViewArray = new GameObject[6];

        for (int i = 0; i < 6; i++)
        {
            GameObject category = Instantiate(categoryPrefab);
            category.transform.SetParent(categoryContainer.transform);
            category.transform.localScale = new Vector3(1f, 1f, 1f);
            // category.GetComponent<RectTransform>().rect.width = categoryWidth;



            category.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * (categoryContainerWidth - categoryWidth) / 5, 0, 0);
            category.transform.GetChild(0).GetComponent<Text>().text = categoryNameList[i];
            category.GetComponent<SlidingUpPanelCategory>().init(i);

            GameObject listView = Instantiate(listViewPrefab);
            listView.transform.SetParent(panelContainer.transform);
            listView.transform.localScale = new Vector3(1f, 1f, 1f);
            listView.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * 1080 + 54, 0, 0);
            listViewArray[i] = listView;
        }


        GameObject panelTileButton;
        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 0, new Vector2(0, 0));
        float panelTileButtonWidth = panelTileButton.GetComponent<RectTransform>().rect.width;
        float panelTileButtonHeight = panelTileButton.GetComponent<RectTransform>().rect.height;
        panelTileButton.GetComponent<PanelTileButton>().init("tile_empty", "미존재", EditMode.deleteBlock);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 0, new Vector2(1 * (972 - panelTileButtonWidth) / 2, 0));
        panelTileButton.GetComponent<PanelTileButton>().init("tile_normal", "기본", EditMode.makeBlock);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 0, new Vector2(2 * (972 - panelTileButtonWidth) / 2, 0));
        panelTileButton.GetComponent<PanelTileButton>().init("tile_isolated", "자가격리", EditMode.makeIsolate);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 0, new Vector2(0, panelTileButtonHeight + 15));
        panelTileButton.GetComponent<PanelTileButton>().init("tile_confined", "외출금지", EditMode.makeConfine);


        ///////////////////////

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 1, new Vector2(0 * (972 - panelTileButtonWidth) / 2, 0));
        panelTileButton.GetComponent<PanelTileButton>().init("object_delete", "삭제", EditMode.eraser);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 1, new Vector2(1 * (972 - panelTileButtonWidth) / 2, 0));
        panelTileButton.GetComponent<PanelTileButton>().init("object_player", "주인공", EditMode.makePlayer);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 1, new Vector2(2 * (972 - panelTileButtonWidth) / 2, 0));
        panelTileButton.GetComponent<PanelTileButton>().init("object_person", "사람", EditMode.makePerson);

        panelTileButton = Instantiate(panelTileButtonPrefab);
        renderAt(panelTileButton, 1, new Vector2(0 * (972 - panelTileButtonWidth) / 2, panelTileButtonHeight + 15));
        panelTileButton.GetComponent<PanelTileButton>().init("object_goal", "목적지", EditMode.makeGoal);

        ///////////////////////

        GameObject itemTileButton;
        itemTileButton = Instantiate(itemTileButtonPrefab);
        renderAt(itemTileButton, 2, new Vector2(0, 0));
        float itemTileButtonWidth = itemTileButton.GetComponent<RectTransform>().rect.width;
        float itemTileButtonHeight = itemTileButton.GetComponent<RectTransform>().rect.height;
        itemTileButton.GetComponent<ItemTileButton>().init("isolate_gray", "자가격리", "대상을 현재 위치에 격리", ItemData.isolate);


        itemTileButton = Instantiate(itemTileButtonPrefab);
        renderAt(itemTileButton, 2, new Vector2(0, 1 * itemTileButtonHeight + 25));
        itemTileButton.GetComponent<ItemTileButton>().init("liberate_gray", "격리해제", "대상을 자가격리로부터 해제", ItemData.release);

        itemTileButton = Instantiate(itemTileButtonPrefab);
        renderAt(itemTileButton, 2, new Vector2(0, 2 * itemTileButtonHeight + 25));
        itemTileButton.GetComponent<ItemTileButton>().init("vaccine_gray", "백신", "대상이 2 이동 후 집합금지 규칙 무시 가능", ItemData.vaccine);

        itemTileButton = Instantiate(itemTileButtonPrefab);
        renderAt(itemTileButton, 2, new Vector2(0, 3 * itemTileButtonHeight + 25));
        itemTileButton.GetComponent<ItemTileButton>().init("vaccine_gray", "대각선이동", "대상이 대각선으로 1회 이동 가능", ItemData.diagonal);


        ///////////////////////

        GameObject starTileButton;
        starTileButton = Instantiate(starTileButtonPrefab);
        renderAt(starTileButton, 3, new Vector2(0, 0));
        float starTileButtonWidth = starTileButton.GetComponent<RectTransform>().rect.width;
        float starTileButtonHeight = starTileButton.GetComponent<RectTransform>().rect.height;
        starTileButton.GetComponent<StarTileButton>().init(0);

        starTileButton = Instantiate(starTileButtonPrefab);
        renderAt(starTileButton, 3, new Vector2(0, 1 * starTileButtonHeight + 25));
        starTileButton.GetComponent<StarTileButton>().init(1);

        starTileButton = Instantiate(starTileButtonPrefab);
        renderAt(starTileButton, 3, new Vector2(0, 2 * starTileButtonHeight + 25));
        starTileButton.GetComponent<StarTileButton>().init(2);



        ////////////////////////////////////////////////////


        GameObject etcTileButton;
        etcTileButton = Instantiate(etcTileButtonPrefab);
        renderAt(etcTileButton, 5, new Vector2(0, 0));
        float etcTileButtonWidth = etcTileButton.GetComponent<RectTransform>().rect.width;
        float etcTileButtonHeight = etcTileButton.GetComponent<RectTransform>().rect.height;
        etcTileButton.GetComponent<EtcTileButton>().init("맵 초기화", "초기 설정 상태로 맵을 초기화", "clearMap");

        etcTileButton = Instantiate(etcTileButtonPrefab);
        renderAt(etcTileButton, 5, new Vector2(0, 1 * starTileButtonHeight + 25));
        etcTileButton.GetComponent<EtcTileButton>().init("맵 크기 변경", "맵의 크기를 변경하고 초기화", "changeMapSize");
    

    }

    void renderAt(GameObject ob, int renderCategoryIndex, Vector2 dest)
    {
        ob.transform.SetParent(listViewArray[renderCategoryIndex].transform.GetChild(0).GetChild(0).transform);
        ob.transform.localScale = new Vector3(1f, 1f, 1f);

        ob.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(dest.x, -dest.y, 0);
    }


    public void categoryChanged()
    {
        slidingUpSwitchWrapper.GetComponent<SlidingUpSwitchWrapper>().panelUp();
    }


    // Update is called once per frame
    void Update()
    {
        // panelContentRectTransform.anchoredPosition = new Vector3(selectedCategoryIndex * 1080, -120.83f, 0);
        panelContentRectTransform.anchoredPosition = Vector3.Lerp(panelContentRectTransform.anchoredPosition, new Vector3(-selectedCategoryIndex * 1080, -120.83f, 0), Time.deltaTime * 10);
    }
}
