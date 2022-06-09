using CharacterCopy.Domain.Abstraction;
using CharacterCopy.Domain.Implementation;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace CharacterCopy.Tests
{
    [TestFixture]
    public class CopierTests
    {
        [Test]
        public void Copy_WhenSourceReturnsNewLineCharacter_DoesNotWriteToDestination()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var character = '\n';
            source.ReadChar().Returns(character);

            //Act
            copier.Copy();

            //Assert
            source.Received(1).ReadChar();
            destinaion.DidNotReceive().WriteChar(Arg.Any<char>());
        }

        [Test]
        public void Copy_WhenSourceThrowsException_DoesNotWriteToDestinationButThrowsException()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var exceptionMessage = "An error occured";
            source.ReadChar().Returns(x => throw new Exception(exceptionMessage));

            //Act
            var exception = Assert.Throws<Exception>(() => copier.Copy());

            //Assert
            source.Received(1).ReadChar();
            destinaion.DidNotReceive().WriteChar(Arg.Any<char>());
            Assert.NotNull(exception);
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void Copy_WhenSourceReturnsCharacters_WritesToDestinationUntilNewLineCharacterIsReturned()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            source.ReadChar().Returns('a', 'b', 'c', '\n');

            //Act
            copier.Copy();

            //Assert
            source.Received(4).ReadChar();
            destinaion.Received(3).WriteChar(Arg.Any<char>());
            destinaion.Received(1).WriteChar(Arg.Is<char>('a'));
            destinaion.Received(1).WriteChar(Arg.Is<char>('b'));
            destinaion.Received(1).WriteChar(Arg.Is<char>('c'));
            destinaion.DidNotReceive().WriteChar(Arg.Is<char>('\n'));
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsEmptyArray_DoesNotWriteToDestination()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 10;
            var characters = new char[0];
            source.ReadChars(Arg.Any<int>()).Returns(characters);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(1).ReadChars(maxCharCount);
            destinaion.DidNotReceive().WriteChars(Arg.Any<char[]>());
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsArrayStartingWithNewLine_DoesNotWriteToDestination()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 5;
            var characters = new char[] { '\n', 'a', 'b', 'c', 'd' };
            source.ReadChars(Arg.Any<int>()).Returns(characters);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(1).ReadChars(maxCharCount);
            destinaion.DidNotReceive().WriteChars(Arg.Any<char[]>());
        }

        [Test]
        public void CopyMultiple_WhenMaxCharCountIsLessThanOne_ThrowsException()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
             var maxCharCount = 0;
            var exceptionMessage = "Maximum character count can not be less than 1.";

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() => copier.CopyMultiple(maxCharCount));

            //Assert
            Assert.IsNotNull(exception);
            Assert.IsInstanceOf<InvalidOperationException>(exception);
            Assert.AreEqual(exceptionMessage, exception.Message);
            source.DidNotReceive().ReadChars(Arg.Any<int>());
            destinaion.DidNotReceive().WriteChars(Arg.Any<char[]>());
        }

        [Test]
        public void CopyMultiple_WhenSourceThrowsException_DoesNotWriteToDestinationButThrowsException()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var exceptionMessage = "An error occured";
            var maxCharCount = 10;
            source.ReadChars(Arg.Any<int>()).Returns(x => throw new Exception(exceptionMessage));

            //Act
            var exception = Assert.Throws<Exception>(() => copier.CopyMultiple(maxCharCount));

            //Assert
            source.Received(1).ReadChars(maxCharCount);
            destinaion.DidNotReceive().WriteChars(Arg.Any<char[]>());
            Assert.NotNull(exception);
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsCharactersFewerThanMaxCharCount_WritesToDestination()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 4;
            var characters = new char[] { '1', '2', '3' };
            source.ReadChars(maxCharCount).Returns(characters);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(1).ReadChars(maxCharCount);
            destinaion.Received(1).WriteChars(characters);
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsCharactersContainingNewLine_WritesAllCharactersBeforeNewLineToDestination()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 5;
            var characters = new char[] { 'a', 'b', '\n', 'c', 'd' };
            var writableCharacters = new char[] { 'a', 'b' };
            source.ReadChars(maxCharCount).Returns(characters);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(1).ReadChars(maxCharCount);
            destinaion.Received(1).WriteChars(Arg.Is<char[]>(x=>x.SequenceEqual(writableCharacters)));
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsExpectedNumberOfCharactersWithoutNewLine_WritesAllCharactersToDestinationUntilUnexpectedNumberIsReturned()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 4; 
            var characters = new char[] { 'a', 'b', 'c', 'd' };
            var characters1 = new char[] { 'a', 'b' };
            source.ReadChars(maxCharCount).Returns(characters, characters1);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(2).ReadChars(maxCharCount);
            destinaion.Received(2).WriteChars(Arg.Any<char[]>());
            destinaion.Received(1).WriteChars(characters);
            destinaion.Received(1).WriteChars(characters1);
        }

        [Test]
        public void CopyMultiple_WhenSourceReturnsExpectedNumberOfCharactersWithoutNewLine_WritesAllCharactersToDestinationUntilEmptyArrayIsReturned()
        {
            //Arrange
            var source = Substitute.For<ISource>();
            var destinaion = Substitute.For<IDestination>();
            var copier = new Copier(source, destinaion);
            var maxCharCount = 4;
            var characters = new char[] { 'a', 'b', 'c', 'd' };
            var characters1 = new char[0];
            source.ReadChars(maxCharCount).Returns(characters, characters1);

            //Act
            copier.CopyMultiple(maxCharCount);

            //Assert
            source.Received(2).ReadChars(maxCharCount);
            destinaion.Received(1).WriteChars(Arg.Any<char[]>());
            destinaion.Received(1).WriteChars(characters);
            destinaion.DidNotReceive().WriteChars(characters1);
        }
    }
}
