using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpace : MonoBehaviour
{
    public Player OccupiedBy;
    public bool Locked { get; private set; }

    public int Col;
    public int Row;

    public SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Locked && OccupiedBy != null && !(OccupiedBy is Joker))
        {
            var newSprite = Resources.Load<Sprite>(OccupiedBy.LockedSpriteName);
            rend.sprite = newSprite;
        }
    }

    public void MarkAsLocked(bool lockValue)
    {
        if (OccupiedBy != null && !(OccupiedBy is Joker))
        {
            Locked = lockValue;
        }
    }

    public bool IsOccupied => OccupiedBy != null;

    public static readonly string BlankSprite = "Sprites/Blank_Space";

    public void Clear()
    {
        if (!(OccupiedBy is Joker))
        {
            Locked = false;
            OccupiedBy = null;
            rend.sprite = Resources.Load<Sprite>(BlankSprite);
            GameControl.Board[Row, Col] = null;
        }
    }

    private void OnMouseDown()
    {
        if (GameControl.GameOver)
        {
            return;
        }

        new WaitForSeconds(1f);
        var currentP = GameControl.CurrentPlayer;

        if (OneEyeJack.Enabled)
        {
            if (!Locked && OccupiedBy != null && currentP.Team != OccupiedBy.Team)
            {
                Clear();
                GameControl.EndTurn();
            }
        }
        else if (OccupiedBy == null)
        {
            OccupiedBy = currentP;
            GameControl.Board[Row, Col] = this;
            rend.sprite = Resources.Load<Sprite>(OccupiedBy.SpriteName);
            GameControl.EndTurn();
        }
    }
}
