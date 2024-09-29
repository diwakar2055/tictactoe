using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this if you're using TextMeshPro
using System;  // Add this for System.Random
using System.Threading;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int whoseTurn; //0:x's turn , 1:y's turn
    public int turnCount; //counts the number of turn played
    public GameObject[] turnIcons; //displays whose turn this is
    public Sprite[] playIcons;  // 0 = x icon and 1= y icon
    public Button[] tictactoeSpaces;    //playable spaces for our game
    
    // This won't show in the inspector as 2D array, but we manage it in code
    public Button[,] ticTacToeButtons = new Button[3, 3]; // 3x3 grid for Tic-Tac-Toe

    private int[] markedSpaces = new int[9]; // Array to track which player marked each space.
    public GameObject[] winnerLine;
    // Change this to TextMeshProUGUI[] to work with TextMeshPro
    public TextMeshProUGUI winnerText;
    public int XPlayerScore;
    public int OPlayerScore;
    public TextMeshProUGUI XPlayerScoreText;
    public TextMeshProUGUI OPlayerScoreText;
    public Button rematch;
    public Button restart;
    //public Button xPlayerButton;
    //public Button yPlayerButton;
    public Image catImage;
    public int[] filledSpaces = new int[9];
    public int[] availableSpaces = new int[9];

    // Start is called before the first frame update
    void Start()
    {
        GameSetup();   
    }
     
    void GameSetup()
    {
        whoseTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
        Debug.Log("markedSpaces.Length" + markedSpaces.Length);
        for (int i = 0; i < markedSpaces.Length; i++) 
        {
            markedSpaces[i] = -100;
        }

        winnerText.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, winnerText.GetComponent<RectTransform>().anchoredPosition.y);
        winnerText.gameObject.SetActive(false);
        catImage.enabled = false;
    }

    public Button GetButton(int row, int col)
    {
        int index = row * 3 + col; // 3 is the number of columns in Tic-Tac-Toe
        return tictactoeSpaces[index];
    }

    public int GetMarkedSpaces(int row, int col)
    {
        int index = row * 3 + col;
        return markedSpaces[index];
    }

    // Setter method for setting values in markedSpaces array
    public void SetMarkedSpaces(int i, int j, int value)
    {
        int index = i * 3 + j;  // Convert 2D indices to 1D index
        markedSpaces[index] = value;
    }

    public void ticTacToeButton(int whichNumber)
    {
        Debug.Log("whichNumber:" + whichNumber);
        tictactoeSpaces[whichNumber].image.sprite = playIcons[whoseTurn];
        tictactoeSpaces[whichNumber].interactable = false;
        filledSpaces[turnCount] = whichNumber;        
        turnCount ++;

        markedSpaces[whichNumber] = whoseTurn + 1;

        if (turnCount > 4)
        {
            if (winnerCheckFunction())
            {
                return;
            }
        }


        if (whoseTurn == 0)
        {
            whoseTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        else
        {
            whoseTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }


        if (MainMenu.choosedLevel == 1)
        {
            // Create a Random instance
            System.Random random = new System.Random();
            int randomTime = random.Next(1,3);
            enableDisableButtons(false);
            Invoke("EasyCpuPlay", randomTime);

        }

        else if(MainMenu.choosedLevel == 2)
        {
            //@todo: code for Medium cpu level
            // Create a Random instance
            System.Random random = new System.Random();
            int randomTime = random.Next(1, 3);
            enableDisableButtons(false);
            Invoke("MediumCpuPlay", randomTime);
        }

        else if (MainMenu.choosedLevel == 3)
        {
            enableDisableButtons(false);
            //@todo: code for Medium cpu level
            // Create a Random instance
            System.Random random = new System.Random();
            int randomTime = random.Next(1, 3);
            enableDisableButtons(false);
            Invoke("HardCpuPlay", randomTime);
        }
        return;
    }

    public void EasyCpuPlay()
    {
        enableDisableButtons(true);
        int randomNumber = GetRandomNumberExcluding(filledSpaces);
        Debug.Log("randomNumber:" + randomNumber);
        if(randomNumber < 0 || randomNumber > 8)
        {
            return;
        }
        tictactoeSpaces[randomNumber].image.sprite = playIcons[whoseTurn];
        tictactoeSpaces[randomNumber].interactable = false;
        filledSpaces[turnCount] = randomNumber;
        turnCount++;

        markedSpaces[randomNumber] = whoseTurn + 1;

        if (turnCount > 4)
        {
            winnerCheckFunction();
        }


        if (whoseTurn == 0)
        {
            whoseTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        else
        {
            whoseTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }
    }

    public void MediumCpuPlay()
    {
        bool isMarked = false;
        enableDisableButtons(true);
        int randomNumber = GetRandomNumberExcluding(filledSpaces);

        // Array of arrays holding the indices for each row, column, and diagonal
        int[,] solutionIndices = new int[,]
        {
            {0, 1, 2}, // s0: Top row
            {3, 4, 5}, // s1: Middle row
            {6, 7, 8}, // s2: Bottom row
            {0, 3, 6}, // s3: Left column
            {1, 4, 7}, // s4: Middle column
            {2, 5, 8}, // s5: Right column
            {0, 4, 8}, // s6: Diagonal from top-left to bottom-right
            {2, 4, 6}  // s7: Diagonal from top-right to bottom-left
        };

        // Array to store the sum of each row, column, and diagonal
        int[] solution = new int[8];

        // Summing the values in `markedSpaces` based on the indices defined in `solutionIndices`
        for (int i = 0; i < solution.GetLength(0); i++) // Loop through rows of solutionIndices
        {
            solution[i] = 0; // Initialize the sum for each row/column/diagonal
            for (int j = 0; j < 3; j++) // Each set of indices has 3 values (3 for Tic-Tac-Toe)
            {
                solution[i] += markedSpaces[solutionIndices[i, j]]; // Add the values in markedSpaces based on the indices
            }
        }
        Debug.Log("Solution array: " + string.Join(", ", solution));

        // Now the array 'solution' contains the sums for each row, column, and diagonal.
        for (int i = 0; i < solutionIndices.GetLength(0); i++) // Loop through rows
        {
            if (solution[i] == -96 || solution[i] == -98)
            {
                Debug.Log("solution[" + i + "] =" + solution[i]);
                for (int j = 0; j < solutionIndices.GetLength(1); j++) // Loop through columns (0, 1, 2)
                {
                    int index = solutionIndices[i, j]; // Access the current inde
                    if (markedSpaces[index] == -100)
                    {
                        tictactoeSpaces[index].image.sprite = playIcons[whoseTurn];
                        tictactoeSpaces[index].interactable = false;
                        filledSpaces[turnCount] = 0;
                        turnCount++;
                        markedSpaces[index] = whoseTurn + 1;
                        isMarked = true;
                        break;
                    }
                }
            }
            if (isMarked)
            {
                break;
            }
        }

        if (!isMarked)
        {
            tictactoeSpaces[randomNumber].image.sprite = playIcons[whoseTurn];
            tictactoeSpaces[randomNumber].interactable = false;
            filledSpaces[turnCount] = randomNumber;
            turnCount++;
            markedSpaces[randomNumber] = whoseTurn + 1;
        }

        if (turnCount > 4)
        {
            winnerCheckFunction();
        }


        if (whoseTurn == 0)
        {
            whoseTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        else
        {
            whoseTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }
    }

    public void HardCpuPlay()
    {
        enableDisableButtons(true);
        int bestScore = -1000; // For maximizing player (CPU)
        int[] bestMove = { 0, 0 };

        // Loop through the board to find the best move
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // Check if the space is available
                if (GetMarkedSpaces(i, j) == -100)
                {
                    // Try the move
                    SetMarkedSpaces(i, j, whoseTurn + 1); // Mark the move for the current player

                    // Run the minimax algorithm to get a score for this move
                    int score = miniMax(0, false); // Passing `false` for the opponent's move

                    // Undo the move
                    SetMarkedSpaces(i, j, -100);
                    //Debug.Log("score after minimax is completed");
                    // Update the bestScore if the current move has a better score
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove[0] = i;
                        bestMove[1] = j;
                    }
                }
            }
        }

        Debug.Log("getting out of minimax loop");
        Debug.Log("bestMove is " +bestMove[0]+"," + bestMove[1]);
        // Perform the best move found
        GetButton(bestMove[0], bestMove[1]).image.sprite = playIcons[whoseTurn];
        GetButton(bestMove[0], bestMove[1]).interactable = false;
        filledSpaces[turnCount] = 0; // Track this move in filled spaces
        turnCount++;
        SetMarkedSpaces(bestMove[0], bestMove[1], whoseTurn + 1); // Mark the move on the board

        // Check if the game has been won
        if (turnCount > 4)
        {
            winnerCheckFunction();
        }

        // Switch turns
        whoseTurn = (whoseTurn == 0) ? 1 : 0; // Toggle between 0 and 1
        turnIcons[0].SetActive(whoseTurn == 0);
        turnIcons[1].SetActive(whoseTurn == 1);
    }

    public int miniMax(int depth, bool isMaximizing)
    {
        //Debug.Log("Inside miniMax, depth: " + depth + ", isMaximizing: " + isMaximizing);

        // Base case: evaluate the current board state
        int result = getScore(depth);
        if (result != -2) // Game is either won, lost, or drawn
        {
            //Debug.Log("Exiting miniMax loop, result: " + result);
            return result;
        }

        // Check for draw condition
        if (!HasAvailableMoves())
        {
            //Debug.Log("Draw detected in miniMax, returning 0");
            return 0;  // Draw
        }

        // Limit recursion depth
        if (depth > 8)
        {
            Debug.Log("Reached depth limit in miniMax, returning neutral score");
            return 0;  // Neutral score
        }

        if (isMaximizing)  // Maximizing player's (CPU) turn
        {
            int bestScore = -1000;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GetMarkedSpaces(i, j) == -100)  // If space is empty
                    {
                        SetMarkedSpaces(i, j, 2);  // Simulate CPU move (maximizing player)
                        int score = miniMax(depth + 1, false);  // Recur for opponent's turn
                        SetMarkedSpaces(i, j, -100);  // Undo the move
                        bestScore = Math.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else  // Minimizing player's (opponent) turn
        {
            int bestScore = 1000;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GetMarkedSpaces(i, j) == -100)  // If space is empty
                    {
                        SetMarkedSpaces(i, j, 1);  // Simulate opponent move (minimizing player)
                        int score = miniMax(depth + 1, true);  // Recur for CPU's turn
                        SetMarkedSpaces(i, j, -100);  // Undo the move
                        bestScore = Math.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }

    // Function to generate random number between 0 and 8, excluding specified numbers
    public int GetRandomNumberExcluding(int[] excludedNumbers)
    {
        System.Random random = new System.Random();

        // Create a list of available numbers (0 to 8)
        List<int> availableNumbers = new List<int>();

        for (int i = 0; i <= 8; i++)  // Add numbers 0 to 8 to the list
        {
            if (markedSpaces[i] <  0)  // Check if the number is not in excludedNumbers
            {
                availableNumbers.Add(i);  // Add to available numbers
            }
        }

        // If there are available numbers, return a random one
        if (availableNumbers.Count > 0)
        {
            int randomIndex = random.Next(0, availableNumbers.Count);  // Pick a random index
            return availableNumbers[randomIndex];  // Return the number at that index
        }
        else
        {
            return -1;  // Return -1 if no available numbers (error handling)
        }
    }

    public void enableDisableButtons(bool enable)
    {   

        if (!enable)
        {
            for (int i = 0; i < tictactoeSpaces.Length;i++)
            {

                if (markedSpaces[i] < 0)
                {
                    tictactoeSpaces[i].gameObject.SetActive(false); // This will hide the button entirely
                    //tictactoeSpaces[i].interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < tictactoeSpaces.Length; i++)
            {
                if (markedSpaces[i] < 0)
                {
                    tictactoeSpaces[i].gameObject.SetActive(true); // This will hide the button entirely
                    tictactoeSpaces[i].interactable = true;
                }
            }
        }
    }

    public void rematchButton()
    {
        GameSetup();
        for(int i = 0; i < winnerLine.Length; i++)
        {
            winnerLine[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].gameObject.SetActive(true); // Ensure the button GameObject is active
            tictactoeSpaces[i].interactable = true;        // Make the button interactable
        }
        return;

    }

    public void backBtn()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void restartButton()
    {
        GameSetup();
        for (int i = 0; i < winnerLine.Length; i++)
        {
            winnerLine[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].gameObject.SetActive(true); // Ensure the button GameObject is active
            tictactoeSpaces[i].interactable = true;        // Make the button interactable
        }
        XPlayerScore = 0;
        XPlayerScoreText.text = XPlayerScore.ToString();
        OPlayerScore = 0;
        OPlayerScoreText.text = OPlayerScore.ToString();

    }

    public int getScore(int depth)
    {
        // Possible win lines: Rows, Columns, and Diagonals
        int s0 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2]; // Top row
        int s1 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5]; // Middle row
        int s2 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8]; // Bottom row
        int s3 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6]; // Left column
        int s4 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7]; // Middle column
        int s5 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8]; // Right column
        int s6 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8]; // Diagonal top-left to bottom-right
        int s7 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6]; // Diagonal top-right to bottom-left

        // Store all line sums in an array
        int[] solution = { s0, s1, s2, s3, s4, s5, s6, s7 };

        // Check for a win (either X or O)
        for (int i = 0; i < solution.Length; i++)
        {
            // Check if Player X (minimizing player, represented by 1) wins
            if (solution[i] == 3)  // Player X controls the entire line (1 + 1 + 1)
            {
                Debug.Log("Player X wins");
                return -1;  // X wins (negative score because the opponent wins)
            }

            // Check if Player O (maximizing player, represented by 2) wins
            if (solution[i] == 6)  // Player O controls the entire line (2 + 2 + 2)
            {
                Debug.Log("Player O wins");
                return 1;  // O wins (positive score because the CPU wins)
            }
        }

        // Check if the board is full (draw)
        if (depth >= 9 || !HasAvailableMoves())  // If depth exceeds board size or no moves left
        {
            Debug.Log("It's a draw");
            return 0;  // Draw (neutral score)
        }

        // If no win and no draw, continue the game
        Debug.Log("Continue the game");
        return -2;  // Game is still ongoing
    }


    // Helper method to check if there are any available moves left
    public bool HasAvailableMoves()
    {
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            if (markedSpaces[i] == -100)  // Empty space
            {
                return true;
            }
        }
        return false;  // No moves left
    }

    public bool winnerCheckFunction()
    {
        int s0 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s1 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s2 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s3 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s4 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s5 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s6 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s7 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];

        int[] solution = {s0,s1,s2,s3,s4,s5,s6,s7};
        for (int i = 0; i < solution.Length; i++)
        {
            if (solution[i] == 3 * (whoseTurn + 1))
            {
                showWinner(i);
                return true;
            }
        }
            if (turnCount > 8)
            {
                winnerText.text = "It's Draw!!";
                // Access the RectTransform of the TextMeshProUGUI component and modify its position
                winnerText.GetComponent<RectTransform>().anchoredPosition = new Vector2(90, winnerText.GetComponent<RectTransform>().anchoredPosition.y);
                winnerText.gameObject.SetActive(true);
                Debug.Log("cat enabled........................................");
                catImage.enabled = true;
            return true;
            }
        return false;
    }

    public void showWinner(int whichline)
    {
        Debug.Log("player" + whoseTurn + "won");
        winnerLine[whichline].SetActive(true);
        if (whoseTurn == 0)
        {
            XPlayerScore++;
            XPlayerScoreText.text = XPlayerScore.ToString();
            winnerText.text = "PLAYER X WINS!!";
        }
        else
        {
            OPlayerScore++;
            OPlayerScoreText.text = OPlayerScore.ToString();
            winnerText.text = "PLAYER O WINS!!";
        }
        winnerText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, winnerText.GetComponent<RectTransform>().anchoredPosition.y);
        winnerText.gameObject.SetActive(true);
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = false;
            if (markedSpaces[i] < 0)
            {
                tictactoeSpaces[i].gameObject.SetActive(false); // This will hide the button entirely
            }
        }
        return;
    }

    public void switchPlayer(int whichPlayer)
    {
        if(turnCount == 0)
        {

            if(whichPlayer == 0)
            {
                whoseTurn = 0;
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
            }
            else
            {
                whoseTurn = 1;
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(true);
                if(MainMenu.choosedLevel == 3)
                {
                    GetButton(0,0).image.sprite = playIcons[whoseTurn];
                    GetButton(0, 0).interactable = false;
                    filledSpaces[turnCount] = 0;
                    turnCount++;
                    markedSpaces[0] = whoseTurn + 1;

                    whoseTurn = 0;
                    turnIcons[0].SetActive(true);
                    turnIcons[1].SetActive(false);
                }
            }
        }
        return ;
    }

    // Custom method to handle back button behavior
    public void OnBackButtonPressed()
    {
        // Example: Load the previous scene or exit the application
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex > 0)
        {
            // Load the previous scene
            SceneManager.LoadScene(currentSceneIndex - 1);
        }
        else
        {
            // Optionally exit the application
            Application.Quit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the back button (Escape key) is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Call your custom back button functionality here
            OnBackButtonPressed();
        }
   
    }
}
