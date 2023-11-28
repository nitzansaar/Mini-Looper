using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {
   public GameObject canvas;

   public GameObject[] piecePrefabs;
   public bool generateRandom;


   [System.Serializable]
   public class Puzzle
   {
       public int winValue;
       public int currValue;


       public int width;
       public int height;
       public piece[,] pieces;


   }




   public Puzzle puzzle = new Puzzle();




   // Use this for initialization
   void Start ()
   {
  
       Debug.Log("GameManager Start method is being executed.");


       canvas.SetActive(false);
        if (generateRandom)
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
            Vector2 dimensions = CheckDimensions ();


            puzzle.width = (int)dimensions.x;
            puzzle.height = (int)dimensions.y;




            puzzle.pieces = new piece[puzzle.width, puzzle.height];


            GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
            foreach (var piece in pieces)
            {
                puzzle.pieces [(int)piece.transform.position.x, (int)piece.transform.position.y] = piece.GetComponent<piece>();
                //    Debug.Log("test");
            }
        }

    //    foreach(var piece in puzzle.pieces)
    //    {
    //        Debug.Log("hello");
    //    }


       puzzle.winValue = getWinValue();


       Shuffle();


       puzzle.currValue = Sweep();


   }

   void GeneratePuzzle()
   {
        puzzle.pieces = new piece[puzzle.width, puzzle.height];


        int[] auxValues = {0, 0, 0, 0};
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


   int getWinValue()
   {
       int winValue = 0;
       foreach(var piece in puzzle.pieces)
       {
           if (piece != null && piece.CompareTag("Piece"))
           {
               foreach(var j in piece.values)
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
       Debug.Log("You win");
   }


   void Shuffle()
   {
       foreach(var piece in puzzle.pieces)
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


       GameObject[] pieces = GameObject.FindGameObjectsWithTag ("Piece");


       foreach (var p in pieces)
       {
           if (p.transform.position.x > aux.x)
               aux.x = p.transform.position.x;


           if (p.transform.position.y > aux.y)
               aux.y= p.transform.position.y;
       }


       aux.x++;
       aux.y++;


       return aux;
   }
  
   // Update is called once per frame
   void Update ()
   {
  
   }


   public void NextLevel(string nextLevel)
   {
       SceneManager.LoadScene(nextLevel);
   }


}
