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
    public MessageManager messageManager;

    [Header("Words")]
    public string targetWord = "BODAO";
    public List<string> validWords = new List<string>();
    public List<string> possibleTargets = new List<string>();



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
        PickNewTarget();
    }

    public void Update()
    {
        if (_gameOver || _currentRow >= rows) return;


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


    private List<string> LoadWordsFromFile(string filename) 
    {
        List<string> words = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filename);
        if (file != null) 
        {
            string[] lines = file.text.Split(new[] {'\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines) 
            {
                string word = line.Trim().ToUpperInvariant();
                if(word.Length == cols) 
                {
                    words.Add(word);
                }
                else 
                {
                    Debug.LogWarning($"Palavra ignorada por ter tamanho diferente de {cols}: {word}");
                }
             
            }
        }
        else { Debug.LogError($"Arquivo {filename} não encontrado na pasta Resources!"); }
        return words;
    }

    public void InitValidWords()
    {
        validWords = LoadWordsFromFile("valid_words");
        if (validWords.Count == 0) 
        {
            Debug.LogError("Nenhuma palavra válida carregada! Usando lista de emergência.");
            validWords = new List<string> { "TESLA", "BODAO", "GAMES" }; // fallback
        }

        possibleTargets = LoadWordsFromFile("target_words");

        if(possibleTargets.Count == 0) 
        {
            possibleTargets = new List<string>(validWords);
        }
        
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


    #region PROCESSING
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
            if(messageManager != null) 
            {
                messageManager.ShowMessage("Palavra Invalida meu parça");
            }
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
            if (messageManager != null)
                messageManager.ShowMessage("Suas tentativas acabaram. A palavra era: " + targetWord);
            _gameOver = true;
        }
    }
    #endregion
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
            if(messageManager != null) 
            {
                messageManager.ShowMessage("Voce acertou!");
            }
            EndGame();
        }
    }

    public void SetTileColor(int row, int col, Color color) 
    {
        if (tiles[row, col].image != null) 
        {
            tiles[row,col].image.color = color;

            TextMeshProUGUI txt = tiles[row, col].text;
            if (txt != null)
            {
                float luminance = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
                txt.color = luminance > 0.5f ? Color.black : Color.white;
            }
        }
    }

    public void EndGame() 
    {
        _gameOver = true;
    }

    private void PickNewTarget() 
    {
        if(possibleTargets != null && possibleTargets.Count > 0) 
        {
            int index = UnityEngine.Random.Range(0,possibleTargets.Count);
            targetWord = possibleTargets[index];
        }
        else if(validWords.Count > 0) 
        {
            int index = UnityEngine.Random.Range(0, validWords.Count);
            targetWord = validWords[index];
        }
        else 
        {
            targetWord = "BODAO";
        }
        Debug.Log("Nova palavra alvo: " +  targetWord);
    }


    public void RestartGame() 
    {
        for (int row =0 ; row < rows; row++) 
        {
            for(int col =0 ; col < cols; col++) 
            {
                tiles[row, col].text.text = "";
                tiles[row, col].image.color = defaultColor;
                tiles[row, col].text.color = Color.black;
            }
        }

        _currentRow = 0;
        _currentCol = 0;
        _gameOver = false;

        if(messageManager!= null)
            messageManager.ClearMessage();

         PickNewTarget(); 
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

