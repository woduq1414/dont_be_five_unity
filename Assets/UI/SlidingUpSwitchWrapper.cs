
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SlidingUpSwitchWrapper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Button button;
    RectTransform slidingUpPanelWrapperRectTransform;
    // Start is called before the first frame update

    GameObject slidingUpPanelWrapper;

    Vector2 beginDragPosition;
    Vector2 beginDragRectPosition;

    bool isDragable = true;


    public void init(GameObject slidingUpPanelWrapper_)
    {
        this.slidingUpPanelWrapper = slidingUpPanelWrapper_;
        slidingUpPanelWrapperRectTransform = slidingUpPanelWrapper.GetComponent<RectTransform>();
    }


    void Start()
    {
        button = this.transform.GetComponent<Button>();
        button.onClick.AddListener(fClick);
        // rectTransform = this.transform.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void fClick()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable)
        {
            return;
        }
        if (beginDragPosition != null)
        {
            Vector2 delta = eventData.position - beginDragPosition;
            slidingUpPanelWrapperRectTransform.anchoredPosition = new Vector2(slidingUpPanelWrapperRectTransform.anchoredPosition.x, Mathf.Clamp(beginDragRectPosition.y + delta.y, -700, 0));
        }
        // slidingUpPanelWrapperRectTransform.anchoredPosition = eventData.position;
        // Debug.Log(eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDragable)
        {
            return;
        }
        beginDragPosition = eventData.position;
        beginDragRectPosition = new Vector2(slidingUpPanelWrapperRectTransform.anchoredPosition.x, slidingUpPanelWrapperRectTransform.anchoredPosition.y);
        // Debug.Log("begin~");
        // Debug.Log(eventData.position);
    }

    public void panelUp()
    {
        StartCoroutine(MoveTo(-50f, 0.5f));
    }


    IEnumerator MoveTo(float aValue, float aTime)
    {
        isDragable = false;
        float alpha = slidingUpPanelWrapperRectTransform.anchoredPosition.y;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            // Debug.Log(Mathf.Lerp(alpha, aValue, t));
            slidingUpPanelWrapperRectTransform.anchoredPosition = new Vector2(slidingUpPanelWrapperRectTransform.anchoredPosition.x, Mathf.Lerp(slidingUpPanelWrapperRectTransform.anchoredPosition.y, aValue, t));
            yield return null;
        }
        isDragable = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - beginDragPosition;

        float movedY = slidingUpPanelWrapperRectTransform.anchoredPosition.y;
        if (movedY > -50 || delta.y >= 80)
        {
            StartCoroutine(MoveTo(-50f, 0.5f));
        }
        else if (movedY < -700 || delta.y <= -80)
        {
            StartCoroutine(MoveTo(-700f, 0.5f));
        }



    }


}
