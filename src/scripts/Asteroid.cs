using Godot;
using System;

public partial class Asteroid : RigidBody2D
{
    public float healingAmount = 5f;
    public float _scaleAmount;

    public override void _Ready()
    {
        AddToGroup("asteroid");
        
        _scaleAmount = (int)GD.RandRange(1, 3.0);
        Scale *= _scaleAmount;
        
        GlobalRotation = GD.Randf() % Mathf.Pi - Mathf.Pi;
    }

    
    // If player or enemy crashes into it, damage
    // If player shoots it, heals
    public void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("bullet") || body.IsInGroup("enemy_bullet"))
        {
            QueueFree();
        }

        if (body.IsInGroup("player"))
        {
            
        }
    }
}
