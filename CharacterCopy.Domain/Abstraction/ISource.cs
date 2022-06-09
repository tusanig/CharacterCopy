namespace CharacterCopy.Domain.Abstraction
{
    public interface ISource
    {
        char ReadChar();

        char[] ReadChars(int maxCharCount);
    }
}
