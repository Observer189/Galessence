using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Ship", menuName = "ShipInfo", order = 3)]
public class ShipInfo : ScriptableObject
{
    [Tooltip("Название корабля")]
    [SerializeField]
    protected string name;
    [Tooltip("Длина корабля(размер по Y)")]
    [SerializeField]
    protected float length;
    [Tooltip("Ширина корабля(размер по X)")]
    [SerializeField]
    protected float width;
    [SerializeField]
    protected GameObject prefab;
    [SerializeField]
    protected GameObject aiMindPrefab;

    public string Name => name;

    public float Length => length;

    public float Width => width;

    public GameObject Prefab => prefab;

    public GameObject AIMindPrefab => aiMindPrefab;
}
