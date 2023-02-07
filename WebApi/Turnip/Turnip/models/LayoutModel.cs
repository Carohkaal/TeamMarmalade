using System.Windows.Media;

namespace Turnip.Models
{
    /// <summary>
    /// The layout model can be used to configure the default colors and other layout properties
    /// </summary>
    public class LayoutModel
    {
        public string FillColor { get; set; } = nameof(Colors.CadetBlue);
        public string ButtonBgColor { get; set; } = "#8F8F8F";
        public string ButtonBorderColor { get; set; } = nameof(Colors.Black);
    }

    public enum Alliance
    {
        Plants,
        Fungi,
        Animaux
    }
}