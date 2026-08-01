using Godot;

/*
 * EVENTBUS MANAGES ALL CUSTOM SIGNALS WITHIN THE GAME
*/


public partial class EventBus : Node
{
    [Signal]
    public delegate void ScoreUpdatedEventHandler(int scoreValue);

    
    // TODO --> MAKE IT SO I CAN USE THIS FOR ENEMIES AND PLAYER ALSO, ANOTHER ARGUMENT NODE2D?
    [Signal]
    public delegate void DamageTakenEventHandler(float damageValue);

    [Signal]
    public delegate void PlayerShootEventHandler();


    public static EventBus Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }
}
