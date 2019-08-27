using UnityEngine;

public class Wall : MonoBehaviour
{
    public enum Type {
        Dirt,
        Stone,
        //todo: add more
    }

    public Type type;
}
