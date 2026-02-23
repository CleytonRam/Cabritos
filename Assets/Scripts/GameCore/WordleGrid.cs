using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System;
public class WordleGrid : MonoBehaviour
{
    [Header("Config")]
    public GameObject tilePrefab;
    public Transform gridParent;
    public int rows = 6;
    public int cols = 5;

    [Header("Words")]
    public string targetWord = "BODAO";
    public List<string> validWords = new List<string>();

    [Header("Colors")]
    public Color correctColor = Color.green;
    public Color presentColor = Color.yellow;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    private TileData[,] tiles;



    private bool _gameOver = false;
    private int _currentRow = 0;
    private int _currentCol = 0;
    private string _currentWord = "";


    public enum TileState 
    {
        Empty,
        Correct,
        Present,
        Wrong
    } 


    private void Start()
    {
        CreateGrid();
        InitValidWords();
    }

    public void Update()
    {
        if (_gameOver || _currentRow >= rows) return;

        if (_currentRow >= rows) return;

        if (Input.inputString.Length > 0)
        {
            char c = Input.inputString[0];
            if (char.IsLetter(c))
            {
                c = char.ToUpperInvariant(c);
                ProcessLetter(c);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ProcessBackspace();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ProcessEnter();
        }
    }

    public void CreateGrid()
    {
        tiles = new TileData[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject newTile = Instantiate(tilePrefab, gridParent);
                newTile.name = $"Tile_{row}_{col}";

                Image img = newTile.GetComponent<Image>();
                TextMeshProUGUI txt = newTile.GetComponentInChildren<TextMeshProUGUI>();

                TileData tileData = new TileData
                {
                    image = img,
                    text = txt,
                    row = row,
                    col = col
                };
                tiles[row, col] = tileData;

                txt.text = "";
            }
        }
    }

    public void InitValidWords()
    {
        validWords = new List<string>
        {
          "TESLA", "BODAO", "PIRUS", "CODAR", "MESAS", "PENIS", "PEITO", "BUNDA", "VIADO", "GAMES", "OADOB"
        };
    }

    public string GetCurrentWord()
    {
        string word = "";
        for (int col = 0; col < cols; col++)
        {
            word += tiles[_currentRow, col].text.text;
        }
        return word;
    }

    public void ProcessLetter(char letter)
    {
        if (_currentCol < cols)
        {
            tiles[_currentRow, _currentCol].text.text = letter.ToString();
            _currentCol++;
        }
    }
    public void ProcessBackspace()
    {
        if (_currentCol > 0)
        {
            _currentCol--;
            tiles[_currentRow, _currentCol].text.text = "";
        }
    }

    public void ProcessEnter()
    {
        if (_currentCol != cols) return;

        string guess = GetCurrentWord();
        if (!validWords.Contains(guess))
        {
            Debug.Log("Palavra inválida");
            return;
        }

        CheckGuess(guess);

        if (guess == targetWord)
        {
            return;
        }

        _currentRow++;
        _currentCol = 0;

        if (_currentRow >= rows)
        {
            Debug.Log("Suas tentativas acabaram. A palavra era: " + targetWord);
        }
    }

    public void CheckGuess(string guess) 
    {
        guess = guess.ToUpperInvariant();
        string target = targetWord.ToUpperInvariant();

        char[] targetChars = target.ToCharArray();
        char[] guessChars = guess.ToCharArray();

        bool[] targetUsed = new bool[cols];

        for (int i = 0; i < cols; i++) 
        {
            if (guessChars[i] == targetChars[i]) 
            {
                SetTileColor(_currentRow, i, correctColor);
                targetUsed[i] = true;
            }

            else 
            {
            }
        }

        for (int i = 0; i < cols; i++) 
        {
            if (guessChars[i] == targetChars[i])
                continue;

            bool found = false;

            for (int j = 0; j < cols; j++) 
            {
                if (!targetUsed[j] && guessChars[i] == targetChars[j]) 
                {
                    SetTileColor(_currentRow, i, presentColor);
                    targetUsed[j] = true;
                    found = true; 
                    break;
                }
            }
            if (!found) 
            {
                SetTileColor( _currentRow, i, wrongColor);
            }
        }
        if (guess == targetWord)
        {
            Debug.Log("Você acertou!");
            // Aqui você pode desabilitar a entrada, mostrar mensagem, etc.
            // Por exemplo, definir currentRow = rows para parar a digitação:
            _currentRow = rows; 
            EndGame();
        }
    }

    public void SetTileColor(int row, int col, Color color) 
    {
        if (tiles[row, col].image != null) 
        {
            tiles[row,col].image.color = color;
        }
    }

    public void EndGame() 
    {
        _gameOver = true;
    }


    [System.Serializable]
    public class TileData
    {
        public Image image;
        public TextMeshProUGUI text;
        public int row;
        public int col;
        public TileState state;
    }
}

