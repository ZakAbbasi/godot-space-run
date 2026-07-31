using Godot;
using System;

public partial class Bullet : RigidBody2D
{
    /* VARIABLE DECLARATION */
    
    /* METHOD DECLARATIONS */
    public override void _Ready()
    {
        AddToGroup("bullet");
    }

    
    // TODO --> WHEN BULLET GOES OUT OF VIEW -> DELETED
    private void DestroyBullet()
    {
        QueueFree();
    }
}
