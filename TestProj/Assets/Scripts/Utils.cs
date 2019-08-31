using System;
using UnityEngine;

public class ItemLocation {
    public int floor;
    public Vector3 position;

    public ItemLocation(int floor, Vector3 position) {
        this.floor = floor;
        this.position = position;
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

