using System.Collections.Generic;

namespace SudokuChallengeMVC.Data
{
    public class Puzzle
    {
        public bool response { get; set; }
        public int size { get; set; }
        public List<Squares> squares { get; set; } = new List<Squares>();
    }
}