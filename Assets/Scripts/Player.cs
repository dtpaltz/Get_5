public class Player
{
	public string SpriteName;
	public int Team;
	public int PlayerIndex;

	public string LockedSpriteName
	{
		get
		{
			if (!string.IsNullOrEmpty(SpriteName))
			{
				return SpriteName.Replace("Sprites/", "Sprites/Locked_");
			}

			return null;
		}
	}

	public Player(string sprite, int team, int player)
	{
		SpriteName = sprite;
		Team = team;
		PlayerIndex = player;
	}
}

public class Joker : Player
{
	public Joker() : base(null, -1, -1)
	{ }
}
