using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data.Common;

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
        // null check
        if (text == null)
        {
            Debug.Log("text is null");
            return;
        }

        string newText = currentProvider.GetTooltipText();

        // null check
        if (newText == null)
        {
            Debug.Log("new text is null");
            newText = "";
        }

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

        // Ensure layout/sizes are up to date
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Get root canvas and its rect
        Canvas rootCanvas = rectTransform.GetComponentInParent<Canvas>();
        if (rootCanvas == null)
        {
            Debug.LogError("TooltipUI: no parent Canvas found.");
            return;
        }

        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

        // For ScreenSpace-Overlay use null camera; otherwise use canvas camera
        Camera cam = (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera;

        // Convert screen point to local point in canvas space
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouse, cam, out localPoint))
        {
            // Should rarely fail, but bail gracefully
            return;
        }

        // Tooltip size in canvas local units
        Vector2 tooltipSize = rectTransform.rect.size;

        // Convert localPoint (-half..+half) -> (0..width/height) to do clipping checks
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 pointInCanvasSpace = localPoint + canvasSize * 0.5f;

        // Decide pivot flips (prevent off-screen)
        Vector2 newPivot = rectTransform.pivot;
        if (pointInCanvasSpace.x + tooltipSize.x + offset.x + 20f >= canvasSize.x)
            newPivot.x = 1f;
        else
            newPivot.x = 0f;

        if (pointInCanvasSpace.y + offset.y - (tooltipSize.y + 40f) <= 0f)
            newPivot.y = 0f;
        else
            newPivot.y = 1f;

        rectTransform.pivot = newPivot;

        // Compute anchored position (localPoint is centered around canvas center)
        Vector2 anchoredPos = localPoint + offset;

        // Convert to 0..canvas range to clamp
        Vector2 anchoredPosInCanvas = anchoredPos + canvasSize * 0.5f;
        anchoredPosInCanvas.x = Mathf.Clamp(anchoredPosInCanvas.x, 0f, canvasSize.x);
        anchoredPosInCanvas.y = Mathf.Clamp(anchoredPosInCanvas.y, 0f, canvasSize.y);

        // Convert back to local (-half..+half)
        anchoredPos = anchoredPosInCanvas - canvasSize * 0.5f;

        // Apply position using anchoredPosition (works correctly for UI)
        rectTransform.anchoredPosition = anchoredPos;

        // Debug: remove when fixed — logs canvas scale/scaleFactor if you still see large offsets
        // Debug.Log($"Canvas renderMode={rootCanvas.renderMode} scaleFactor={rootCanvas.scaleFactor} parentScale={rectTransform.parent?.lossyScale ?? Vector3.one}");


        // ==========================

        /*Vector2 mouse = Input.mousePosition;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        Vector2 tooltipSize = text.rectTransform.sizeDelta;
        //tooltipSize *= 1920 / 800;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);


        Vector2 pivot = rectTransform.pivot;


        *//*Debug.Log("Mouse pos: " + mouse);
        Debug.Log("tooltip size: " + tooltipSize);

        Debug.Log("Final pos: " + (mouse + tooltipSize + offset));*//*


        // Only flip pivot if necessary to prevent clipping
        if (mouse.x + tooltipSize.x + offset.x + 20 >= screenSize.x)
        {
            pivot.x = 1f; // flip right
            
            Debug.Log("tooltip X going offscreen; going left now.");
        }
        else
        {
            pivot.x = 0f;

            Debug.Log("tooltip X going right.");
        }

        if (mouse.y + offset.y - (tooltipSize.y + 40) <= 0)
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

        Debug.Log($"mouse: {mouse}");
        Debug.Log($"offset: {offset}");
        Debug.Log($"screenSize: {screenSize}");
        pos.x = Mathf.Clamp(pos.x, 0, screenSize.x);
        pos.y = Mathf.Clamp(pos.y, 0, screenSize.y);

        rectTransform.anchoredPosition = pos;
        Debug.Log($"position: {pos}");
        Debug.Log($"rectTransform pos: {rectTransform.position}");*/

    }


}