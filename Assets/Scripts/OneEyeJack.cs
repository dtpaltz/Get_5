using UnityEngine;

public class OneEyeJack : MonoBehaviour
{
	public static bool Enabled;

	private static SpriteRenderer rend;

	void Start()
	{
		rend = GetComponent<SpriteRenderer>();
	}

	public static void EnablePower()
	{
		Enabled = true;
		rend.color = new Color(1f, 1f, 1f, 1f);
	}

	public static void DisablePower()
	{
		Enabled = false;
		rend.color = new Color(1f, 1f, 1f, 0.5f);
	}

	void OnMouseDown()
	{
		if (GameControl.GameOver)
		{
			return;
		}

		if (Enabled)
		{
			DisablePower();
		}
		else
		{
			TwoEyeJack.DisablePower();
			EnablePower();
		}
	}
}
