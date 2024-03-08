using RtelLibrary.Enums;

namespace MisterProtypoParser.Helpers
{
    public class ListBoxItem
    {
        public SysEventLevel EventLevel { get; set; } = SysEventLevel.Unknown;
        public string Text { get; set; }        
    }
}
