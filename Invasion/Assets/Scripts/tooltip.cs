using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI tooltipText;

    private bool isMouseOver = false; 

    private void Update()
    {
        if (isMouseOver)
        {
            // Mouse over, show the tooltip text
            ShowTooltip();
        }
        else
        {
            // Mouse is not over, hide the tooltip text
            HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    private void ShowTooltip()
    {
        // Enable the TMP Text component
        if (tooltipText != null)
        {
            tooltipText.enabled = true;
        }

        // Show the fixed tooltip area if it exists
        if (transform.childCount > 0)
        {
            Transform tooltipArea = transform.GetChild(0);
            if (tooltipArea != null)
            {
                tooltipArea.gameObject.SetActive(true);
            }
        }
    }

    private void HideTooltip()
    {
        // Disable the TMP Text component
        if (tooltipText != null)
        {
            tooltipText.enabled = false;
        }

        // Hide the fixed tooltip area if it exists
        if (transform.childCount > 0)
        {
            Transform tooltipArea = transform.GetChild(0);
            if (tooltipArea != null)
            {
                tooltipArea.gameObject.SetActive(false);
            }
        }
    }
}