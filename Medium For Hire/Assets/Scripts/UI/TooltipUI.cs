using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI text;
    //public LayoutElement layoutElement;
    public Vector2 offset = new Vector2(15f, -15f);
    public int maxWidth = 300;

    private ITooltipProvider currentProvider;
    private string lastText;

    void Awake()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>(); // get the root's rectTransform
        if (text == null) Debug.LogError("Tooltip Text not assigned!");
        //if (layoutElement == null) Debug.LogError("LayoutElement not assigned!");
        Hide();
    }

    void Update()
    {
        if (!gameObject.activeSelf || currentProvider == null) return;

        FollowMouse();
        UpdateTextIfChanged();
    }

    public void Show(ITooltipProvider provider) // called by another UI Element's OnPointerEnter()
    {
        if (provider == null) return;

        currentProvider = provider;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        UpdateTextImmediately();
    }

    public void Hide()
    {
        currentProvider = null;
        gameObject.SetActive(false);
    }

    private void UpdateTextIfChanged()
    {
        string newText = currentProvider.GetTooltipText();
        if (newText != lastText)
        {
            lastText = newText;
            text.text = newText;
            //layoutElement.preferredWidth = Mathf.Min(maxWidth, text.preferredWidth);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    private void UpdateTextImmediately()
    {
        lastText = "";
        UpdateTextIfChanged();
    }

    private void FollowMouse()
    {
        Vector2 mouse = Input.mousePosition;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        //Vector2 tooltipSize = rectTransform.sizeDelta;
        Vector2 tooltipSize = text.rectTransform.sizeDelta;
        tooltipSize *= 1920 / 850;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        //Debug.Log("ScreenSize: " + screenSize);

        Vector2 pivot = rectTransform.pivot;
        //Vector2 pivot = text.rectTransform.pivot;


        Debug.Log("Mouse pos: " + mouse);
        Debug.Log("tooltip size: " + tooltipSize);

        Debug.Log("Final pos: " + (mouse + tooltipSize + offset));

        //Vector2 oldPos = text.rectTransform.position;

        // Only flip pivot if necessary to prevent clipping
        if (mouse.x + tooltipSize.x + offset.x >= screenSize.x)
        {
            pivot.x = 1f; // flip right
            
            Debug.Log("tooltip X going offscreen; going left now.");
        }
        else
        {
            pivot.x = 0f;

            Debug.Log("tooltip X going right.");
        }

        if (mouse.y + offset.y - (tooltipSize.y) <= 0)
        {
            pivot.y = 0f; // flip down

            Debug.Log("tooltip going up?");
        }
        else
        {
            pivot.y = 1f;

            Debug.Log("tooltip going down?");
        }
        //text.rectTransform.position = oldPos;

        rectTransform.pivot = pivot;


        // Clamp position to screen bounds
        Vector2 pos = mouse + offset;
        pos.x = Mathf.Clamp(pos.x, 0, screenSize.x);
        pos.y = Mathf.Clamp(pos.y, 0, screenSize.y);

        rectTransform.position = pos;
    }


}