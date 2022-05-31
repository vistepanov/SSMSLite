namespace SsmsLite.Core.Ui.Search
{
    public class TextFragment
    {
        public string Value { get; }

        public TextFragmentType FragmentType { get; }

        public TextFragment(string value, TextFragmentType fragmentType)
        {
            Value = value ?? ""; // ?? throw new ArgumentNullException("Cannot assign Null to TextFragment"); ;
            FragmentType = fragmentType;
        }

        public static TextFragment Primary(string value)
        {
            return new TextFragment(value, TextFragmentType.Primary);
        }

        public static TextFragment Highlight(string value)
        {
            return new TextFragment(value, TextFragmentType.Highlight);
        }

        public static TextFragment Secondary(string value)
        {
            return new TextFragment(value, TextFragmentType.Secondary);
        }
    }
}
