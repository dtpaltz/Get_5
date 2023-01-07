using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    //TODO: add lock editing
    //TODO: add draw card reminder
    //TODO: add thumbs up - Ryker's suggestion

    private static int Team1Score;
    private static int Team2Score;

    private static GameObject player1MoveText, player2MoveText, player3MoveText, player4MoveText, EndGameView, winningTeamText, team1ScoreText, team2ScoreText, NewGameButton;

    public static bool PickCardAudioReminderEnabled;

    public static GameSpace[,] Board;
    public static readonly int BoardDims = 10;
    public static readonly int BoardRows = 10;
    public static readonly int BoardCols = 10;

    public static int MoveNum;
    private static readonly int WinScore = 3;

    public static Player[] AllPlayers;

    public static Player CurrentPlayer;

    private static readonly int NumberOfPlayers = 4;

    public static bool GameOver = false;

    // Use this for initialization
    void Start ()
    {
        PickCardAudioReminderEnabled = true;

        NewGameButton = GameObject.Find("New_Game_Button");

        player1MoveText = GameObject.Find("Player1TurnText");
        player2MoveText = GameObject.Find("Player2TurnText");
        player3MoveText = GameObject.Find("Player3TurnText");
        player4MoveText = GameObject.Find("Player4TurnText");

        team1ScoreText = GameObject.Find("Team1ScoreText");
        team2ScoreText = GameObject.Find("Team2ScoreText");

        winningTeamText = GameObject.Find("WinnerText");
        EndGameView = GameObject.Find("EndGameView");

        InitPlayers();

        NewGame();
    }

    public static void NewGame()
    {
        InitGameBoard();

        MoveNum = 1;
        Team1Score = 0;
        Team2Score = 0;

        SetTeamScoreText(0, 0);
        EndGameView.SetActive(false);
        NewGameButton.SetActive(false);

        int num = UnityEngine.Random.Range(0, NumberOfPlayers);
        CurrentPlayer = AllPlayers[num];

        SetTurnIndicators(CurrentPlayer.PlayerIndex);

        GameOver = false;
    }

    private static void SetTurnIndicators(int playerIdx)
    {
        var pmt = new GameObject[] { player1MoveText, player2MoveText, player3MoveText, player4MoveText };

        foreach (var t in pmt)
        {
            t.SetActive(false);
        }

        if (playerIdx >= 0)
        {
            pmt[playerIdx].SetActive(true);
        }
    }

    private static void SetTeamScoreText(int ts1, int ts2)
    {
        team1ScoreText = GameObject.Find("Team1ScoreText");
        team1ScoreText.GetComponent<Text>().text = "Red = " + ts1;

        team2ScoreText = GameObject.Find("Team2ScoreText");
        team2ScoreText.GetComponent<Text>().text = "Blue = " + ts2;
    }

    public static void EndTurn()
    {
        OneEyeJack.DisablePower();
        TwoEyeJack.DisablePower();
        int idx = (CurrentPlayer.PlayerIndex + 1) % NumberOfPlayers;
        CurrentPlayer = AllPlayers[idx];
        MoveNum++;

        UpdateScores();

        if ((Team2Score < WinScore) && (Team2Score < WinScore) && PickCardAudioReminderEnabled)
        {
            PickCardAudio.Play();
        }
    }

    private static void InitPlayers()
    {
        AllPlayers = new Player[NumberOfPlayers];
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            // 0 = Red
            // 1 = Blue
            var chipSprite = i % 2 == 0 ? "Sprites/Red_Chip" : "Sprites/Blue_Chip";

            Player p = new Player(chipSprite, i % 2, i);
            AllPlayers[i] = p;
        }
    }

    private static void InitGameBoard()
    {
        if (Board != null)
        {
            for (int r = 0; r < GameControl.BoardRows; r++)
            {
                for (int c = 0; c < GameControl.BoardCols; c++)
                {
                    if (Board[r, c] != null)
                    {
                        Board[r, c].Clear();
                    }
                }
            }

            return;
        }

        Board = new GameSpace[BoardRows, BoardCols];

        int[,] jokerCoords = new int[4, 2] { { 0, 0 }, { 0, 9 }, { 9, 0 }, { 9, 9 } };

        for (int i = 0; i < jokerCoords.GetLength(0); i++)
        {
            GameObject obj = new GameObject();
            GameSpace gs = obj.AddComponent<GameSpace>();
            gs.name = "Joker_" + (i + 1);
            gs.OccupiedBy = new Joker();
            gs.Row = jokerCoords[i, 0];
            gs.Col = jokerCoords[i, 1];

            Board[gs.Row, gs.Col] = gs;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetTeamScoreText(Team1Score, Team2Score);

        if (Team1Score >= WinScore)
        {
            ShowEndGameView("Red");
        }
        else if (Team2Score >= WinScore)
        {
            ShowEndGameView("Blue");
        }
        else
        {
            SetTurnIndicators(CurrentPlayer.PlayerIndex);
        }
    }

    private void ShowEndGameView(string winningTeam)
    {
        NewGameButton.SetActive(true);
        EndGameView.SetActive(true);

        if (!GameOver)
        {
            GameOver = true;
            winningTeamText.GetComponent<Text>().text = winningTeam + " Team Wins";
            CongratsAudio.Play();
        }
    }

    private static void UpdateScores()
    {
        var boardRoutes = GetBoardRunRoutes();

        Team1Score = GetTeamScore(0, boardRoutes); // Red
        Team2Score = GetTeamScore(1, boardRoutes); // Blue
    }

    private static int GetTeamScore(int teamIdx, List<List<GameSpace>> boardRoutes)
    {
        int runningScore = 0;
        var teamRouteRuns = new List<List<GameSpace>>();

        foreach (var route in boardRoutes)
        {
            var currentRun = new List<GameSpace>();

            for (int i = 0; i < route.Count; i++)
            {
                var thisSpace = route[i];

                if (thisSpace != null && thisSpace.OccupiedBy != null && (thisSpace.OccupiedBy.Team == teamIdx || thisSpace.OccupiedBy is Joker))
                {
                    currentRun.Add(thisSpace);
                }

                if (thisSpace == null || !thisSpace.IsOccupied || i == route.Count - 1 || (thisSpace.OccupiedBy.Team != teamIdx && !(thisSpace.OccupiedBy is Joker)))
                {
                    if (currentRun.Count >= 5)
                    {
                        teamRouteRuns.Add(new List<GameSpace>(currentRun));
                    }

                    currentRun = new List<GameSpace>();
                }
            }
        }

        foreach (var routeRun in teamRouteRuns)
        {
            if (routeRun.Count % 5 == 0)
            {
                foreach (GameSpace s in routeRun)
                {
                    s.MarkAsLocked(true);
                }

                runningScore += routeRun.Count / 5;
            }

            if (routeRun.Count > 5 && routeRun.Count < 10)
            {
                // run was [6 - 9] spaces and included at least 1 locked space
                
                // greedy solution
                if (routeRun.All(s => !s.Locked))
                {
                    // all spaces in run are not locked
                    for (int i = 0; i < 5; i++)
                    {
                        routeRun[i].MarkAsLocked(true);
                    }
                }
                else
                {
                    // some spaces in the run are locked

                    int lockedSpacesInRun = routeRun.Where(s => s.Locked).ToList().Count;

                    if (lockedSpacesInRun < 4)
                    {
                        var groupsOfFive = new List<List<GameSpace>>();
                        for (int i = 0; i < routeRun.Count - 4; i++)
                        {
                            groupsOfFive.Add(routeRun.Skip(i).Take(5).ToList());
                        }

                        var minLocked = groupsOfFive.Min(g => g.Where(s => s.Locked).Count());
                        var idealRun = groupsOfFive.First(g => g.Where(s => s.Locked).Count() == minLocked);

                        for (int i = 0; i < 5; i++)
                        {
                            idealRun[i].MarkAsLocked(true);
                        }
                    }
                }

                runningScore += 1;
            }
        }

        return runningScore;
    }

    private static List<List<GameSpace>> GetBoardRunRoutes()
    {
        List<List<GameSpace>> boardRoutes = new List<List<GameSpace>>();

        for (int r = 0; r < GameControl.BoardRows; r++)
        {
            boardRoutes.Add(new List<GameSpace>());
            for (int c = 0; c < GameControl.BoardCols; c++)
            {
                boardRoutes[boardRoutes.Count - 1].Add(Board[r, c]);
            }
        }

        for (int c = 0; c < GameControl.BoardCols; c++)
        {
            boardRoutes.Add(new List<GameSpace>());
            for (int r = 0; r < GameControl.BoardRows; r++)
            {
                boardRoutes[boardRoutes.Count - 1].Add(Board[r, c]);
            }
        }

        for (int line = 1; line <= (GameControl.BoardRows + GameControl.BoardCols - 1); line++)
        {
            // Get column index of the first element in this line of output
            // The index is 0 for first ROW lines and line - ROW for remaining lines
            int start_col = Math.Max(0, line - GameControl.BoardRows);

            // Get count of elements in this line
            // The count of elements is equal to minimum of line number, COL-start_col and ROW
            int count = Math.Min(line, Math.Min((GameControl.BoardCols - start_col), GameControl.BoardRows));

            if (count >= 5)
            {
                boardRoutes.Add(new List<GameSpace>());

                for (int j = 0; j < count; j++)
                {
                    boardRoutes[boardRoutes.Count - 1].Add(Board[Math.Min(GameControl.BoardRows, line) - j - 1, start_col + j]);
                }
            }
        }

        var maxLength = Math.Max(BoardCols, BoardRows);
        for (var k = 0; k <= 2 * (maxLength - 1); ++k)
        {
            var temp = new List<GameSpace>();
            for (var y = BoardRows - 1; y >= 0; --y)
            {
                var x = k - (BoardRows - y);
                if (x >= 0 && x < BoardCols)
                {
                    temp.Add(Board[y, x]);
                }
            }

            if (temp.Count >= 5)
            {
                boardRoutes.Add(temp);
            }
        }

        return boardRoutes;
    }
}
