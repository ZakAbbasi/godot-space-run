using Godot;
using System;

/*
 * GAME MANAGER HANDLES GAME STATE AND VARIABLES
 */


public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    /* TODO --> APPLY SCORES TO PUBLIC ACCESSIBLE VARIABLES VIA GET AND SET */
    
    private int _playerScore;
    private float _playerHealth;

    public PlayerController player;
    public Main MainNode;
    
    public override void _Ready()
    {
        Instance = this;
    }
}
