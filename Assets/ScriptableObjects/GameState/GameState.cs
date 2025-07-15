using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameState")]
public class GameState : ScriptableObject
{
    public bool hasBlueprint;
    public bool reachedGoal;
    public string lastVisitedRoom;

    public void ResetState()
    {
        hasBlueprint = false;
        reachedGoal = false;
        lastVisitedRoom = "";
    }
}
