namespace CharacterCopy.Domain.Abstraction
{
    public interface IDestination
    {
        void WriteChar(char character);
        void WriteChars(char[] characters);
    }
}
