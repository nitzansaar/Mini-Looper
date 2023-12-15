using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool GenerateRandom;

    public Camera orthoCam;
    public GameObject WinCanvas;

    public GameObject PiecesCanvas;

    public GameObject[] piecePrefabs;

    public GameObject wordsPanel;
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject Word;
    public GameObject eventText;

    [System.Serializable]
    public class Puzzle
    {
        public int winValue;
        public int currValue;

        public int width;
        public int height;
        public piece[,] pieces;

    }

    public Puzzle puzzle;

    //ThoughtsCreation
    //---------------------------------------------------------------------
    // Event -> [0]     Negative thought -> [1]  Alternative thought -> [2]+
    string[] toughts1 = { "I was sick and 2 friends called me", "I have no friends Chloe is my friend she loves me and cares about me", "Chloe is my friend" };
    string[] toughts2 = { "I failed in the test", "I know nothing I wasn't exercising enough for the test" };
    string[] toughts3 = { "My bag fell when I entered the supermarket", "I am super Clumsy all the time Big deal" };
    string[] toughts4 = { "My date havn't shown up", "No one wants me I cannot know who is this person" };
    string[] toughts5 = { "No one replied to my instagram post", "Everyone hates me I don't need assurance of people I don't know" };
    string[] toughts6 = { "My neighbor didn't say hello", "People don't like me Sam is my neighbour and he's a friend" };
    string[] toughts7 = { "I got a resignation letter", "I will never find a job Can it be that I'm not the only one" };
    string[] toughts8 = { "I was sick and 2 friends called me", "I have no friends Chloe is my friend she loves me and cares about me", "Chloe is my friend" };
    string[] toughts9 = { "I was sick and 2 friends called me", "I have no friends Chloe is my friend she loves me and cares about me", "Chloe is my friend" };
    private string[][] levels;
    private static int thoughtsIndex = 0;
    // Use this for initialization
    void Start()
    {
        levels = new string[][] { toughts1, toughts2, toughts3, toughts4, toughts5, toughts6, toughts7, toughts8, toughts9 };

        WinCanvas.SetActive(false);

        if (GenerateRandom)
        {
            if (puzzle.width == 0 || puzzle.height == 0)
            {
                Debug.LogError("Please set the dimensions");
                Debug.Break();
            }

            GeneratePuzzle();
        }
        else
        {
            Vector2 dimensions = CheckDimensions();

            puzzle.width = (int)dimensions.x;
            puzzle.height = (int)dimensions.y;
            puzzle.pieces = new piece[puzzle.width, puzzle.height];

            string[] thoughts = levels[thoughtsIndex];
            eventText.GetComponent<TMP_Text>().text = thoughts[0];
            string[] wordsInSentence = thoughts[1].Split(' ');
            int wordindex = 0;
            // Manually assign pieces to the puzzle
            // Replace the following loop with your manual assignment
            foreach (var piece in GameObject.FindGameObjectsWithTag("Piece"))
            {

                Debug.Log("piece name:" + piece.name
                    + ", puzzle (width:"
                    + puzzle.width
                    + ", height:"
                    + puzzle.height
                    + ") try locating cell (x:"
                    + piece.transform.position.x
                    + ",y:"
                    + piece.transform.position.y
                    + ")");

                // Debug.Log(piece.name.StartsWith("null"));

                int x = (int)piece.transform.position.x;
                int y = (int)piece.transform.position.y;

                // instantiate words prefab
                if (wordindex < wordsInSentence.Length && !(piece.name.StartsWith("null")))
                {
                    GameObject newWord = (GameObject)Instantiate(Word, new Vector3(x, y, 0), Quaternion.identity);
                    if (wordindex == 0)
                    {
                        newWord.GetComponent<Image>().GetComponentInChildren<TMP_Text>().text = wordsInSentence[wordindex];
                    }
                    else
                    {
                        newWord.GetComponent<Image>().GetComponentInChildren<TMP_Text>().text = getNextWord(thoughts[1], wordsInSentence[wordindex - 1]);
                    }
                    wordindex++;

                    // Set word parent to be the wordCanvas Panel
                    newWord.transform.SetParent((Transform)wordsPanel.transform);
                    newWord.SetActive(true);
                }


                puzzle.pieces[x, y] = piece.GetComponent<piece>();
            }
        }

        foreach (var item in puzzle.pieces)
        {
            if (item != null)
            {
                Debug.Log(item.gameObject.name);
            }
        }

        puzzle.winValue = GetWinValue();

        Shuffle();

        puzzle.currValue = Sweep();
    }


    void GeneratePuzzle()
    {
        // Instantiate words at the same locations as the pieces
        string[] thoughts = levels[thoughtsIndex];
        eventText.GetComponent<TMP_Text>().text = thoughts[0];
        string[] wordsInSentence = thoughts[1].Split(' ');
        int wordindex = 0;

        puzzle.pieces = new piece[puzzle.width, puzzle.height];

        int[] auxValues = { 0, 0, 0, 0 };

        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                //width restrictions
                if (w == 0)
                    auxValues[3] = 0;
                else
                    auxValues[3] = puzzle.pieces[w - 1, h].values[1];

                if (w == puzzle.width - 1)
                    auxValues[1] = 0;
                else
                    auxValues[1] = Random.Range(0, 2);

                //height resctrictions

                if (h == 0)
                    auxValues[2] = 0;
                else
                    auxValues[2] = puzzle.pieces[w, h - 1].values[0];

                if (h == puzzle.height - 1)
                    auxValues[0] = 0;
                else
                    auxValues[0] = Random.Range(0, 2);

                //tells us piece type
                int valueSum = auxValues[0] + auxValues[1] + auxValues[2] + auxValues[3];

                if (valueSum == 2 && auxValues[0] != auxValues[2])
                    valueSum = 5;



                GameObject go = (GameObject)Instantiate(piecePrefabs[valueSum], new Vector3(w, h, 0), Quaternion.identity);
                go.transform.parent = PiecesCanvas.transform;

                // instantiate words prefab
                if (wordindex < wordsInSentence.Length)
                {
                    GameObject newWord = (GameObject)Instantiate(Word, new Vector3(w, h, 0), Quaternion.identity);
                    if (wordindex == 0)
                    {
                        newWord.GetComponent<Image>().GetComponentInChildren<TMP_Text>().text = wordsInSentence[wordindex];
                    }
                    else
                    {
                        newWord.GetComponent<Image>().GetComponentInChildren<TMP_Text>().text = getNextWord(thoughts[1], wordsInSentence[wordindex - 1]);
                    }
                    wordindex++;

                    // Set word parent to be the wordCanvas Panel
                    newWord.transform.SetParent((Transform)wordsPanel.transform);
                    newWord.SetActive(true);
                }

                while (go.GetComponent<piece>().values[0] != auxValues[0] ||
                          go.GetComponent<piece>().values[1] != auxValues[1] ||
                          go.GetComponent<piece>().values[2] != auxValues[2] ||
                          go.GetComponent<piece>().values[3] != auxValues[3])

                {
                    go.GetComponent<piece>().RotatePiece();
                }
                puzzle.pieces[w, h] = go.GetComponent<piece>();

            }
        }
    }
    public static string getNextWord(string sentence, string word)
    {
        string[] words = sentence.Split(new char[] { ' ' });
        List<string> wordsBesideNot = new List<string>();

        for (int i = 0; i < words.Length - 1; i++)
        {
            if (words[i].Equals(word))
                wordsBesideNot.Add(words[i + 1]);
        }
        return string.Join(" ", wordsBesideNot);
    }
    //    public void LoadWords()
    //    {
    //        // Thoughts creation
    //        // ---------------------------------------------------------------------
    //        //                         Event -> [0]                     Negative thought -> [1]  Alternative thought -> [2]+
    //        string[] toughts1 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts2 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts3 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts4 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts5 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts6 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts7 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts8 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };
    //        string[] toughts9 = { "I was sick, and 2 friends called me", "I have no friends", "Chloe is my friend" };

    //        //string[][] levels = { toughts1, toughts2, toughts3, toughts4, toughts5, toughts6, toughts7, toughts8, toughts9 };

    //        // Elements creation
    //        // ---------------------------------------------------------------------
    //        // 1. Read from Json file all elements locations for creation
    //        //  data: Level 0 -> Point1(x,y), Point2(x,y),...
    //        //        Level 1 -> ...
    //        //
    //        // levelLocations[0] contains the levels element locations (x,y)
    //        /// width - num of levels
    //        /// height - element (x,y) -> [0], etc.
    //        int numOfLevels = 3;
    //        Vector2[][] levelLocations = new Vector2[numOfLevels][];

    //        //locations(indices of the elements and also used for words locations
    //        // level.Add(Point.x, Point.y)
    //        levelLocations[0] = new Vector2[] { new Vector2(0, 0), new Vector2(25, 0), new Vector2(20, 40) };
    //        levelLocations[1] = new Vector2[] { new Vector2(0, 0), new Vector2(25, 0), new Vector2(20, 40) };
    //        levelLocations[2] = new Vector2[] { new Vector2(0, 0), new Vector2(25, 0), new Vector2(20, 40) };

    //        // Prefabs Thoughts sentences creation by piece array locations
    //        for (int levelindex = 0; levelindex < numOfLevels; levelindex++)
    //        {
    //            foreach (Vector2 location in levelLocations[levelindex])
    //            {
    //                //create a piece prefab in x,y location

    //                // Event  Write it as text in the above line
    //                DisplayWord(toughts1[0], location.x, location.y);

    //                // 1 = Negative
    //                DisplayWord(toughts1[1], location.x, location.y);
    //                // 2+ = Alternative

    //                for (int thoughtindex = 0; thoughtindex < toughts1.Length; thoughtindex++)
    //                {
    //                    DisplayWord(toughts1[thoughtindex], location.x, location.y);
    //                }
    //            }
    //        }
    //    }

    //public void DisplayWord(string sentence, float xlocation, float ylocation)
    //{
    //    // Instantiate Word at position (0, 0, 0)
    //    GameObject newWord = Instantiate(Word, new Vector2(xlocation,ylocation), Quaternion.identity);

    //    string[] wordsInSentence = sentence.Split(' ');

    //    // Set word text to be the next word in the alternative thoughts sentence
    //    newWord.GetComponent<TMP_Text>().text = wordsInSentence[0];

    //    // Set word parent to be the wordCanvas Panel
    //    //newWord.transform.SetParent((Transform)WordsCanvas.GetComponent("Panel"));
    //}

    public int Sweep()
    {
        int value = 0;

        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {
                //compares top
                if (h != puzzle.height - 1)
                    if (puzzle.pieces[w, h] != null && puzzle.pieces[w, h + 1] != null)
                    {
                        if (puzzle.pieces[w, h].values[0] == 1 && puzzle.pieces[w, h + 1].values[2] == 1)
                            value++;
                    }
                //compare right
                if (w != puzzle.width - 1)
                    if (puzzle.pieces[w, h] != null && puzzle.pieces[w + 1, h] != null)
                    {
                        if (puzzle.pieces[w, h].values[1] == 1 && puzzle.pieces[w + 1, h].values[3] == 1)
                            value++;
                    }
            }
        }
        return value;
    }
    public void Win()
    {
        WinCanvas.SetActive(true);
    }

    public int QuickSweep(int w, int h)
    {
        int value = 0;

        //compares top
        if (h != puzzle.height - 1)
            if (puzzle.pieces[w, h].values[0] == 1 && puzzle.pieces[w, h + 1].values[2] == 1)
                value++;

        //compare right
        if (w != puzzle.width - 1)
            if (puzzle.pieces[w, h].values[1] == 1 && puzzle.pieces[w + 1, h].values[3] == 1)
                value++;

        //compare left
        if (w != 0)
            if (puzzle.pieces[w, h].values[3] == 1 && puzzle.pieces[w - 1, h].values[1] == 1)
                value++;

        //compare bottom
        if (h != 0)
            if (puzzle.pieces[w, h].values[2] == 1 && puzzle.pieces[w, h - 1].values[0] == 1)
                value++;
        return value;
    }

    int GetWinValue()
    {
        int winValue = 0;
        foreach (var piece in puzzle.pieces)
        {
            if (piece != null)
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

    void Shuffle()
    {
        thoughtsIndex++;
        foreach (var piece in puzzle.pieces)
        {
            if (piece != null)
            {
                int k = Random.Range(0, 4);

                for (int i = 0; i < k; i++)
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
        SceneManager.LoadScene(nextLevel);
    }
}