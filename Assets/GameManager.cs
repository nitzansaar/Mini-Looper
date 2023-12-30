using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject[] piecePrefabs;
    public GameObject eventText;
    public GameObject winningText;
    public string winnersText;

    public string[] thoughtArray;

    public int index;


    [System.Serializable]
    public class Puzzle
    {
        public int winValue;
        public int currValue;


        public int width;
        public int height;
        public piece[,] pieces;


    }

    private string[][] levels;
    private static int thoughtsIndex = 0;

    public Puzzle puzzle = new Puzzle();




    // Use this for initialization
    void Start()
    {

        Debug.Log("GameManager Start method is being executed.");

        InitializeLevels();



        canvas.SetActive(false);
        // shuffleButton.SetActive(true);
        // LoadWords();

        if (puzzle.width == 0 || puzzle.height == 0)
        {
            Debug.LogError("Please set the dimensions");
            Debug.Break();
        }
        GeneratePuzzle();


        puzzle.winValue = getWinValue();


        Shuffle();


        puzzle.currValue = Sweep();

        thoughtArray = GenerateThoughtArray(winnersText, puzzle.winValue, puzzle.currValue);



    }
    public void InitializeLevels()
    {
        string[] toughts1 = { "I was sick and 2 friends called me", "I have no friends", "Chloe is my friend she loves me and cares about me" };
        string[] toughts2 = { "I failed in the test", "I know nothing", "I wasn't exercising enough for the test" };
        string[] toughts3 = { "My bag fell when I entered the supermarket", "I am super Clumsy all the time Big deal" };
        string[] toughts4 = { "My date hasn't shown up", "No one wants me", "I cannot know who is this person" };
        string[] toughts5 = { "No one replied to my instagram post", "Everyone hates me", "I don't need assurance of people I don't know" };
        string[] toughts6 = { "My neighbor didn't say hello", "People don't like me", "Sam is my neighbour and he's a friend" };
        string[] toughts7 = { "I got a resignation letter", "I will never find a job", "Can it be that I'm not the only one" };
        string[] toughts8 = { "I was sick and 2 friends called me", "I have no friends", "Chloe is my friend she loves me and cares about me" };

        levels = new string[][] { toughts1, toughts2, toughts3, toughts4, toughts5, toughts6, toughts7, toughts8 };
    }
    public void GeneratePuzzle()
    {
        // Instantiate words at the same locations as the pieces
        // Split the string at each comma to get an array of thoughts

        string[] thoughts = levels[thoughtsIndex];
        if (thoughtsIndex >= thoughts.Length)
        {
            thoughtsIndex = 0;
        }
        if (thoughtsIndex >= thoughts.Length)
        {
            thoughtsIndex = 0;
        }
        string[] wordsInSentence = thoughts[thoughtsIndex].Split(' ');

        if (thoughts.Length > 2)
        {
            winnersText = thoughts[2];
        }
        else
        {
            winnersText = thoughts[1];
        }

        int wordindex = 0;

        puzzle.pieces = new piece[puzzle.width, puzzle.height];


        int[] auxValues = { 0, 0, 0, 0 };
        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                if (w == 0)
                {
                    auxValues[3] = 0;
                }
                else
                {
                    auxValues[3] = puzzle.pieces[w - 1, h].values[1];
                }
                if (w == puzzle.width - 1)
                {
                    auxValues[1] = 0;
                }
                else
                {
                    auxValues[1] = Random.Range(0, 2);
                }

                if (h == 0)
                {
                    auxValues[2] = 0;
                }
                else
                {
                    auxValues[2] = puzzle.pieces[w, h - 1].values[0];
                }
                if (h == puzzle.height - 1)
                {
                    auxValues[0] = 0;
                }
                else
                {
                    auxValues[0] = Random.Range(0, 2);
                }
                int valueSum = auxValues[0] + auxValues[1] + auxValues[2] + auxValues[3];

                if (valueSum == 2 && auxValues[0] != auxValues[2])
                {
                    valueSum = 5;
                }
                if (valueSum == 2)
                {
                    if (auxValues[0] != auxValues[2] && Random.Range(0, 5) == 0)
                    {
                        valueSum = 5;
                    }
                }


                GameObject go = Instantiate(piecePrefabs[valueSum], new Vector3(w, h, 0), Quaternion.identity);

                while (go.GetComponent<piece>().values[0] != auxValues[0] || go.GetComponent<piece>().values[1] != auxValues[1]
                || go.GetComponent<piece>().values[2] != auxValues[2] || go.GetComponent<piece>().values[3] != auxValues[3])
                {
                    go.GetComponent<piece>().RotatePiece();
                }
                puzzle.pieces[w, h] = go.GetComponent<piece>();
            }
        }
    }
    private string[] GenerateThoughtArray(string currentThought, int winValue, int currValue)
    {
        if (currValue == 0)
        {
            Debug.LogError("Error: currValue is zero in GenerateThoughtArray method.");
            return new string[] { currentThought }; // Return the whole thought as a single part
        }

        int numParts = Mathf.Max(1, winValue - currValue); // Ensure numParts is at least 1

        string[] thoughtParts = new string[numParts];

        string[] words = currentThought.Split(' ');

        int wordsPerPart = words.Length / numParts;

        for (int i = 0; i < numParts; i++)
        {
            int startIndex = i * wordsPerPart;
            int endIndex = (i == numParts - 1) ? words.Length : startIndex + wordsPerPart;
            thoughtParts[i] = string.Join(" ", words, startIndex, endIndex - startIndex);
        }

        return thoughtParts;
    }



    int getWinValue()
    {
        int winValue = 0;
        foreach (var piece in puzzle.pieces)
        {
            if (piece != null && piece.CompareTag("Piece"))
            {
                foreach (var j in piece.values)
                {
                    winValue += j;
                }
            }
        }
        winValue /= 2;
        return winValue;
    }


    public int Sweep()
    {
        int value = 0;
        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                if (h != puzzle.height - 1)
                {
                    if (puzzle.pieces[w, h].values[0] == 1 && puzzle.pieces[w, h + 1].values[2] == 1)
                    {
                        value++;
                    }
                }
                if (w != puzzle.width - 1)
                {
                    if (puzzle.pieces[w, h].values[1] == 1 && puzzle.pieces[w + 1, h].values[3] == 1)
                    {
                        value++;
                    }
                }
            }


        }
        return value;


    }


    public int QuickSweep(int w, int h)
    {
        int value = 0;
        if (h != puzzle.height - 1)
        {
            if (puzzle.pieces[w, h].values[0] == 1 && puzzle.pieces[w, h + 1].values[2] == 1)
            {
                value++;
            }
        }
        if (w != puzzle.width - 1)
        {
            if (puzzle.pieces[w, h].values[1] == 1 && puzzle.pieces[w + 1, h].values[3] == 1)
            {
                value++;
            }
        }
        if (w != 0)
        {
            if (puzzle.pieces[w, h].values[3] == 1 && puzzle.pieces[w - 1, h].values[1] == 1)
            {
                value++;
            }
        }
        if (h != 0)
        {
            if (puzzle.pieces[w, h].values[2] == 1 && puzzle.pieces[w, h - 1].values[0] == 1)
            {
                value++;
            }
        }
        return value;


    }


    public void Win()
    {
        canvas.SetActive(true);
        eventText.SetActive(false);
        winningText.GetComponent<TMP_Text>().text = winnersText;
        Debug.Log("You win");
    }


    public void Shuffle()
    {
        foreach (var piece in puzzle.pieces)
        {
            int k = Random.Range(0, 4);
            for (int i = 0; i < k; i++)
            {
                if (piece != null && piece.CompareTag("Piece"))
                {
                    piece.RotatePiece();
                }
            }
        }
    }


    Vector2 CheckDimensions()
    {
        Vector2 aux = Vector2.zero;


        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");


        foreach (var p in pieces)
        {
            if (p.transform.position.x > aux.x)
                aux.x = p.transform.position.x;


            if (p.transform.position.y > aux.y)
                aux.y = p.transform.position.y;
        }


        aux.x++;
        aux.y++;


        return aux;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void NextLevel(string nextLevel)
    {
        thoughtsIndex++;
        SceneManager.LoadScene(nextLevel);
    }


}