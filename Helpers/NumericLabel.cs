using System.Windows.Forms;

namespace MisterTeamsUsersParserParser.Helpers
{
    public class NumericLabel : Label
    {
        int numericText = 0;
        public int NumericText { get { return numericText; } set { numericText = value; base.Text = value.ToString(); } }
    }
}
