using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    public int enemySpawnFactor = 1;
    public int enemiesRemaining;
    
    public override void _Ready()
    {
        GameManager.Instance.MainNode = this;
        EnemySpawn();
    }


    public override void _Process(double delta)
    {
        
        if (enemiesRemaining == 0)
        {
            enemySpawnFactor++;

            for (int i = 0; i <= enemySpawnFactor; i++)
            {
                EnemySpawn();
                SpawnAsteroids();
            }
        }
    }

    
    public void SpawnAsteroids()
    {
       Rect2 boundingRect = GetViewportRect();

       float randPosX = (float)GD.RandRange(boundingRect.Position.X, boundingRect.End.X);
       float randPosY = (float)GD.RandRange(boundingRect.Position.Y, boundingRect.End.Y);

       var asteroid = ResourceLoader.Load<PackedScene>("res://scenes/asteroid.tscn").Instantiate() as Asteroid;
       AddChild(asteroid);
       asteroid.GlobalPosition = new Vector2(randPosX, randPosY);
    }
    

    public void EnemySpawn()
    {
        float rand = GD.RandRange(0, 1);
        GameManager.Instance.player.spawnPathFollow.SetProgressRatio(rand);
        var enemyScene 
            = ResourceLoader.Load<PackedScene>("res://scenes/enemy_controller.tscn").Instantiate() as EnemyController;


        enemyScene.GlobalPosition = GameManager.Instance.player.spawnPathMarker.GlobalPosition;
        AddChild(enemyScene);
        enemiesRemaining++;
    }
}
