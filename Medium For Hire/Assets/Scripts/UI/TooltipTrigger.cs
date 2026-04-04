using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ITooltipProvider provider;
    private TooltipUI tooltip;

    void Awake()
    {
        /*tooltip = UIManager.Instance.tooltip;
        //tooltip = FindAnyObjectByType<TooltipUI>(FindObjectsInactive.Include);
        if (tooltip == null) Debug.LogError("TooltipUI not found in scene!");*/
    }

    private void Start()
    {
        tooltip = UIManager.Instance.tooltip;
        //tooltip = FindAnyObjectByType<TooltipUI>(FindObjectsInactive.Include);
        if (tooltip == null) Debug.LogError("TooltipUI not found in scene!");
    }

    public void SetProvider(ITooltipProvider newProvider)
    {
        provider = newProvider;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (provider != null) tooltip.Show(provider);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}