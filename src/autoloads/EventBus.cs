using Godot;

/*
 * EVENTBUS MANAGES ALL CUSTOM SIGNALS WITHIN THE GAME
*/


public partial class EventBus : Node
{
    [Signal]
    public delegate void ScoreUpdatedEventHandler(int scoreValue);

    [Signal]
    public delegate void DamageTakenEventHandler(Node2D body, float damageValue);

    [Signal]
    public delegate void PlayerShootEventHandler();
    
    public static EventBus Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }
}
