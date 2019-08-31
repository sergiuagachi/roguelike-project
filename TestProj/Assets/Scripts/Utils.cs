using System;
using UnityEngine;

public class ItemLocation {
    public readonly int Floor;
    public readonly Vector3 Position;

    public ItemLocation(int floor, Vector3 position) {
        Floor = floor;
        Position = position;
    }
}

[Serializable]
public class Item {
    public string name;
    public bool isActive;

    public Item(string name, bool isActive) {
        this.name = name;
        this.isActive = isActive;
    }
}

public class DefaultParameters {
    public int Health;
    public int AttackPoints;

    protected DefaultParameters(int health, int attackPoints) {
        Health = health;
        AttackPoints = attackPoints;
    }
}

public class Statistics {
    public int DeathCounter;
    public int EnemiesKilled;
    public int StepsTaken;
    public int DamageDealt;
    public int DamageReceived;
    public int FloorsChanged;
    public int TotalHeal;
}

