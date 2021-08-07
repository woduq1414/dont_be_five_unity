using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SlidingUpPanelCategory : MonoBehaviour
{
    // Start is called before the first frame update

    Text categoryText;
    Image categoryImage;
    int categoryIndex = -1;
    Button button;
    // MeshRenderer meshRenderer;
    // Material material;



    void Start()
    {
        // meshRenderer = GetComponent<MeshRenderer>();
        // material = meshRenderer.material;
        categoryImage = transform.GetComponent<Image>();
        categoryText = transform.GetChild(0).GetComponent<Text>();
          button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);

    }

    public void init(int categoryIndex_)
    {
        categoryIndex = categoryIndex_;
    }

    // Update is called once per frame

    void fClick(){
        SlidingUpPanelWrapper.Instance.selectedCategoryIndex = categoryIndex;
        SlidingUpPanelWrapper.Instance.categoryChanged();
    }


    void Update()
    {
        if (SlidingUpPanelWrapper.Instance != null)
        {
            if (categoryIndex == SlidingUpPanelWrapper.Instance.selectedCategoryIndex)
            {
                categoryText.color = Colors.selectedCategoryTextColor;
                categoryImage.color = Colors.selectedCategoryColor;
            }else{
                categoryText.color = Color.black;
                categoryImage.color = Color.white;
            }
        }
    }
}
