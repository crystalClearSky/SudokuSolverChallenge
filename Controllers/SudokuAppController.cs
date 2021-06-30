using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SudokuChallengeMVC.Data;

namespace SudokuChallengeMVC.Controllers
{
    public class SudokuAppController : Controller
    {
        private int[,] finalBlock = new int[3, 3];
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<SudokuAppController> _logger;
        public SudokuAppController(ILogger<SudokuAppController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult Index()
        {
            var result = GetPuzzleApi().Result;
            ViewBag.Message = result;
            return View();
        }
        private async Task<List<int>> GetPuzzleApi()
        {
            // This retrieve random Sudoku Data from remote API @ Sudoku Web Service
            //var response = await client.GetStringAsync("http://cs.utep.edu/cheon/ws/sudoku/new/?size=9&level=1");
            var response = "{\"response\":true,\"size\":\"9\",\"squares\":[{\"x\":0,\"y\":0,\"value\":5},{\"x\":0,\"y\":2,\"value\":9},{\"x\":0,\"y\":3,\"value\":3},{\"x\":0,\"y\":6,\"value\":6},{\"x\":0,\"y\":8,\"value\":1},{\"x\":1,\"y\":1,\"value\":7},{\"x\":1,\"y\":3,\"value\":5},{\"x\":1,\"y\":5,\"value\":6},{\"x\":1,\"y\":7,\"value\":3},{\"x\":2,\"y\":2,\"value\":6},{\"x\":2,\"y\":4,\"value\":8},{\"x\":2,\"y\":5,\"value\":7},{\"x\":2,\"y\":7,\"value\":5},{\"x\":2,\"y\":8,\"value\":2},{\"x\":3,\"y\":1,\"value\":9},{\"x\":3,\"y\":2,\"value\":2},{\"x\":3,\"y\":5,\"value\":1},{\"x\":3,\"y\":6,\"value\":5},{\"x\":3,\"y\":7,\"value\":6},{\"x\":4,\"y\":1,\"value\":5},{\"x\":4,\"y\":3,\"value\":6},{\"x\":4,\"y\":4,\"value\":4},{\"x\":4,\"y\":5,\"value\":2},{\"x\":4,\"y\":7,\"value\":9},{\"x\":5,\"y\":2,\"value\":3},{\"x\":5,\"y\":3,\"value\":8},{\"x\":5,\"y\":4,\"value\":5},{\"x\":5,\"y\":6,\"value\":2},{\"x\":6,\"y\":0,\"value\":9},{\"x\":6,\"y\":2,\"value\":7},{\"x\":6,\"y\":3,\"value\":4},{\"x\":6,\"y\":4,\"value\":6},{\"x\":6,\"y\":7,\"value\":8},{\"x\":7,\"y\":0,\"value\":3},{\"x\":7,\"y\":1,\"value\":1},{\"x\":7,\"y\":3,\"value\":2},{\"x\":7,\"y\":5,\"value\":8},{\"x\":7,\"y\":7,\"value\":4},{\"x\":7,\"y\":8,\"value\":6},{\"x\":8,\"y\":0,\"value\":6},{\"x\":8,\"y\":8,\"value\":5}]}";
            //var response = "{\"response\":true,\"size\":\"9\",\"squares\":[{\"x\":0,\"y\":0,\"value\":5},{\"x\":0,\"y\":2,\"value\":9},{\"x\":0,\"y\":3,\"value\":3},{\"x\":0,\"y\":6,\"value\":6},{\"x\":0,\"y\":8,\"value\":1},{\"x\":1,\"y\":1,\"value\":7},{\"x\":1,\"y\":3,\"value\":5},{\"x\":1,\"y\":5,\"value\":6},{\"x\":1,\"y\":7,\"value\":3},{\"x\":2,\"y\":2,\"value\":6},{\"x\":2,\"y\":4,\"value\":8},{\"x\":2,\"y\":5,\"value\":7},{\"x\":2,\"y\":7,\"value\":5},{\"x\":2,\"y\":8,\"value\":2},{\"x\":3,\"y\":1,\"value\":9},{\"x\":3,\"y\":2,\"value\":2},{\"x\":3,\"y\":5,\"value\":1},{\"x\":3,\"y\":6,\"value\":5},{\"x\":3,\"y\":7,\"value\":6},{\"x\":4,\"y\":1,\"value\":5},{\"x\":4,\"y\":3,\"value\":6},{\"x\":4,\"y\":4,\"value\":4},{\"x\":4,\"y\":5,\"value\":2},{\"x\":4,\"y\":7,\"value\":9},{\"x\":5,\"y\":2,\"value\":3},{\"x\":5,\"y\":3,\"value\":8},{\"x\":5,\"y\":4,\"value\":5},{\"x\":5,\"y\":6,\"value\":2},{\"x\":6,\"y\":0,\"value\":9},{\"x\":6,\"y\":2,\"value\":7},{\"x\":6,\"y\":3,\"value\":4},{\"x\":6,\"y\":4,\"value\":6},{\"x\":6,\"y\":7,\"value\":8},{\"x\":7,\"y\":1,\"value\":1},{\"x\":7,\"y\":3,\"value\":2},{\"x\":7,\"y\":5,\"value\":8},{\"x\":7,\"y\":7,\"value\":4},{\"x\":7,\"y\":8,\"value\":6},{\"x\":8,\"y\":0,\"value\":6},{\"x\":8,\"y\":8,\"value\":5}]}";
            var result = JsonConvert.DeserializeObject<Puzzle>(response);
            _logger.LogInformation($"THAT -- {JsonConvert.SerializeObject(response)}");
            _logger.LogInformation($"this {response}");
            var final = SerializeNumberByBlock(result);

            return final;
        }

        /// <summary>
        /// SerializeNumberByBlock will serialize results from API by block 
        /// </summary>
        private List<int> SerializeNumberByBlock(Puzzle result)
        {

            _logger.LogInformation($"Result - {result.squares.Count}");
            // variable square values
            int values = 0;
            // adds zero where no value exist
            int blanks = 0;
            // increment through puzzle values
            int steps = 0;
            // Complete list of values
            var List = new List<int>();
            for (int i = 0; i < 82; i++)
            {
                // resets values to zero after a completeing row
                if (values == 9)
                    values = 0;


                if (result.squares[steps].y == values)
                {
                    // Adds values to list on Y row
                    List.Add(result.squares[steps].value);
                    values++;
                    steps++;
                }
                else
                {
                    List.Add(0);
                    blanks++;
                    values++;
                }

                if (steps == result.squares.Count)
                {
                    if (List.Count < 81)
                    {
                        var remaining = 81 - List.Count;
                        for (int s = 0; s < remaining; s++)
                        {
                            List.Add(0);
                        }
                    }
                    break;
                }

            }

            steps = 0;
            // Puzzle object to store - square x, y and value
            var Puzzle = new Puzzle()
            {
                response = true,
                size = 9
            };

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var squares = new Squares()
                    {
                        x = x,
                        y = y,
                        value = List[steps]
                    };
                    Puzzle.squares.Add(squares);
                    steps++;

                }
            }

            // Checking for existance of a value at specific coordinace
            foreach (var square in Puzzle.squares)
            {
                if (square.x == 5 && square.y == 6)
                {
                    _logger.LogInformation($"value is -- {square.value}");
                }
            }
            // Rearrange sequence from y to x to x to y
            var correctSquence = new List<int>();
            for (int yy = 0; yy < 9; yy++)
            {
                for (int xx = 0; xx < 9; xx++)
                {
                    correctSquence.Add(Puzzle.squares.Where(x => (x.x == xx && x.y == yy)).Select(v => v.value).FirstOrDefault());
                }
            }
            _logger.LogInformation($"final: {Puzzle.squares.Where(x => (x.x == 8 && x.y == 7)).Select(v => v.value).FirstOrDefault()} ff: {correctSquence.Count}");
            foreach (var item in correctSquence)
            {
                _logger.LogInformation($"{item}");
            }
            GetAllNumbersOnXYRows(Puzzle, correctSquence);
            FindIndexOfValue(correctSquence);
            return correctSquence;

        }
        private void FindIndexOfValue(List<int> sequence)
        {
            var start = new Squares();
            var a = sequence.FindIndex(x => x == 7);
            var b = Math.Abs(a / 9);
            _logger.LogInformation($"b: {b}");
            start.x = a;
            start.y = a / 9;
            var c = a - (9 * (start.y));

            _logger.LogInformation($"Result -- x: {c} y: {start.y}");
        }

        private void GetCurrentPosition(int indexNumberInSequence)
        {

        }

        private void GetAllNumbersOnXYRows(Puzzle puzzle, List<int> sequence)
        {
            //x: 2, y: 3 v: 2
            // Puzzle puzzle, int x, int y
            var xRow = new List<int>();
            var yRow = new List<int>();
            foreach (var item in puzzle.squares)
            {
                if (item.x == 3)
                {
                    yRow.Add(item.value);
                }
                if (item.y == 3)
                {
                    xRow.Add(item.value);
                }
            }
            xRow.ForEach(i => _logger.LogInformation($"-- {i}"));

            // get rows and columns where value A exists (x: 3, y: 4)
            var xCol = new List<int>();
            var yCol = new List<int>();
            for (int x = 0; x < 81; x++)
            {
                if(puzzle.squares[x].x == 3)
                {
                    xCol.Add(x);
                }
                
                if (puzzle.squares[x].y == 4)
                {
                    yCol.Add(x);
                }
                
            }
            xCol.ForEach(i => _logger.LogInformation($"++ {i}"));
        }
    }

}