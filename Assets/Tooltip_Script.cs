using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip_Script : MonoBehaviour
{
    public static Tooltip_Script instance;

    private Image tooltipBackground;
    private Text tooltipText;
    public float maxWidth;

    public float loadTime = 3;
    private float timeSpentLoading;
    private bool isLoading;
    private string textToLoad;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        tooltipBackground = this.gameObject.GetComponent<Image>();
        tooltipText = this.gameObject.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        Vector2 outPoint;
        RectTransform canvasRect = tooltipBackground.transform.parent.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, Camera.main, out outPoint);
        tooltipBackground.rectTransform.localPosition = outPoint;
        if(!isFullyOnScreen(tooltipBackground.rectTransform, canvasRect))
        {
            tooltipBackground.rectTransform.anchorMin = new Vector2(1, 0);
            tooltipBackground.rectTransform.anchorMax = new Vector2(1, 0);
            tooltipBackground.rectTransform.pivot = new Vector2(1, 0);
        }
        else
        {
            tooltipBackground.rectTransform.anchorMin = new Vector2(0, 0);
            tooltipBackground.rectTransform.anchorMax = new Vector2(0, 0);
            tooltipBackground.rectTransform.pivot = new Vector2(0, 0);
        }

        //Display after a timer
        if(isLoading)
        {
            if(timeSpentLoading >= loadTime)
            {
                showTooltip(textToLoad);
            }
            else
            {
                timeSpentLoading += Time.deltaTime;
            }
        }

    }

    public void showTooltip(string textIn)
    {
        tooltipBackground.enabled = true;
        tooltipText.enabled = true;
        tooltipText.text = textIn;

        float textPadding = 4.0f;
        Vector2 preferedSize = new Vector2(tooltipText.preferredWidth + (textPadding * 2), tooltipText.preferredHeight + (textPadding * 2));

        //Limit width to a inspector defined size
        if (preferedSize.x > maxWidth)
        {
            preferedSize.x = maxWidth;
        }

        tooltipBackground.rectTransform.sizeDelta = preferedSize;
    }

    public void unShowTooltip()
    {
        tooltipBackground.enabled = false;
        tooltipText.enabled = false;
    }

    public static void displayTooltip(string textIn)
    {
        instance.textToLoad = textIn;
        instance.setTooltipIsLoading(true);
    }

    public static void hideTooltip()
    {
        instance.setTooltipIsLoading(false);
        instance.unShowTooltip();
    }

    private bool isFullyOnScreen(RectTransform tooltipRect, RectTransform canvasRect)
    {
        float rightEdgeXOfToolTip = tooltipRect.localPosition.x + tooltipRect.sizeDelta.x;
        float rightEdgeXOfCanvas = canvasRect.sizeDelta.x / 2;
        if(rightEdgeXOfToolTip > rightEdgeXOfCanvas)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void setTooltipIsLoading(bool isLoadingIn)
    {
        this.isLoading = isLoadingIn;
        this.timeSpentLoading = 0;
    }
}
