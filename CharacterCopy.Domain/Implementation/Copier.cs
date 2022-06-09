using CharacterCopy.Domain.Abstraction;
using System;
using System.Linq;

namespace CharacterCopy.Domain.Implementation
{
    public class Copier
    {
        private readonly ISource source;
        private readonly IDestination destination;

        public Copier(ISource source, IDestination destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void Copy()
        {
            var character = source.ReadChar();
            if (character != '\n')
            {
                destination.WriteChar(character);
                Copy();
            }
        }

        public void CopyMultiple(int maxCharCount)
        {
            if (maxCharCount < 1)
            {
                throw new InvalidOperationException("Maximum character count can not be less than 1.");
            }
            var characters = source.ReadChars(maxCharCount);

            if (characters.Any() && !StartsWithNewLine(characters))
            {

                if (ContainsNewLine(characters))
                {
                    characters = GetWritableCharacters(characters);
                }
                destination.WriteChars(characters);

                if (HasMaximumLength(characters, maxCharCount))
                {
                    CopyMultiple(maxCharCount);
                }
            }
        }

        private static char[] GetWritableCharacters(char[] characters)
        {
            return string.Join("", characters).Split('\n').First().ToArray();
        }

        private static bool StartsWithNewLine(char[] characters)
        {
            return characters.First() == '\n';
        }

        private static bool HasMaximumLength(char[] characters, int maxCharCount)
        {
            return characters.Length == maxCharCount;
        }

        private static bool ContainsNewLine(char[] characters)
        {
            return characters.Any(x => x == '\n');
        }
    }
}
