using System;
using System.Collections.Generic;
using System.Text;

namespace Scrabble
{
    class LetterNumber
    {
        public char Letter { get; set; }

        public int Number { get; set; }

        public LetterNumber(char letter, int number)
        {
            Letter = letter;
            Number = number;
        }
        public LetterNumber Clone()
        {
            return (LetterNumber)this.MemberwiseClone();
        }
    }
}
