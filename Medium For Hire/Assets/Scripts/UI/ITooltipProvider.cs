public interface ITooltipProvider
{
    string GetTooltipText();

    //new
    string GetName();
    string GetDescription();
}