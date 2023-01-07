using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
	{
        GameControl.NewGame();
	}
}
