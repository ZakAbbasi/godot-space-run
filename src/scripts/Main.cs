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
        SpawnEnemies();
        GetSpawnRect();
    }


    public override void _Process(double delta)
    {
        
        if (enemiesRemaining == 0)
        {
            enemySpawnFactor++;

            for (int i = 0; i <= enemySpawnFactor; i++)
            {
                GD.Print("Spawning enemies");
                SpawnEnemies();
                SpawnAsteroids();
            }
        }
    }

    
    public void SpawnEnemies()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemySpawned);
        
        var child 
            = GameManager.Instance.player._renderingArea.GetChild(0) as CollisionShape2D;
        
        
        
        if (child != null)
        {
            Rect2 boundingRect = child.Shape.GetRect();
        
            float rectLength = 20 * GameManager.Instance.player._renderingArea.Scale.X;
            float rectWidth = 20 * GameManager.Instance.player._renderingArea.Scale.Y;
            
            // Origin is typically top left of rectangle
            Vector2 rectOrigin = boundingRect.Position * GameManager.Instance.player._renderingArea.Scale; 
        
            Vector2 rectTopRight = rectOrigin + new Vector2(rectLength, 0);
            Vector2 rectBottomLeft = rectOrigin + new Vector2(0, rectWidth);
            Vector2 rectBottomRight = rectOrigin + new Vector2(rectLength, rectWidth);

            List<Vector2> spawnCoord = new List<Vector2>();
            
            spawnCoord.Add(rectTopRight);
            spawnCoord.Add(rectBottomRight);
            spawnCoord.Add(rectOrigin);
            spawnCoord.Add(rectBottomLeft);

            foreach (var coord in spawnCoord)
            {
                var enemyScene 
                    = ResourceLoader.Load<PackedScene>("res://scenes/enemy_controller.tscn").Instantiate() 
                        as EnemyController;
                AddChild(enemyScene);
                enemyScene.GlobalPosition = coord;

                enemiesRemaining++;
            }
        }

        else
        {
            GD.Print("Child doesn't exist");
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


    private void GetSpawnRect()
    {
        var child = GameManager.Instance.player._renderingArea.GetChild(0) as CollisionShape2D;
        var camera = GameManager.Instance.player._playerCamera; // MAYBE NOT NEEDED
        
        
        Rect2 boundingRect = child.Shape.GetRect();
        
        float rectLength = 20 * GameManager.Instance.player._renderingArea.Scale.X;
        float rectWidth = 20 * GameManager.Instance.player._renderingArea.Scale.Y;
            
        // Origin is typically top left of rectangle
        Vector2 rectOrigin = boundingRect.Position * GameManager.Instance.player._renderingArea.Scale; 
        Vector2 rectTopRight = rectOrigin + new Vector2(rectLength, 0);
        Vector2 rectBottomLeft = rectOrigin + new Vector2(0, rectWidth);
        Vector2 rectBottomRight = rectOrigin + new Vector2(rectLength, rectWidth);
        
        Rect2 innerRect = camera.GetViewportRect();
        
        GD.Print(innerRect.Position);
        GD.Print(innerRect.End);
        GD.Print(rectOrigin);
        GD.Print(rectBottomRight);


    }
}
