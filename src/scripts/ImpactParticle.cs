using Godot;
using System;

// TODO --> WILL INHERIT PHYSICS PROPERTIES OF RIGID BODY IT IS A CHILD OF
public partial class ImpactParticle : GpuParticles2D
{
    public RigidBody2D parentNode;

    public override void _Ready()
    {
        parentNode = GetParent<RigidBody2D>();
        
        if (parentNode != null)
        {
            // CODE HERE
        }
        else
        {
            GD.Print("Impact particle has no parent");
        }
    }
}
