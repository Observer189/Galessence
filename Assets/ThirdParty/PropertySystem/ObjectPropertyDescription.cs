using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Property", menuName = "ObjectProperty", order = 1)]
public class ObjectPropertyDescription : ScriptableObject
{
    [Tooltip("Имя свойства")]
    [SerializeField] private string name;
    [Tooltip("Краткое имя свойства")]
    [SerializeField] private string shortName;
    [Tooltip("Униальное ID свойства (обязательно сверяйтесь с таблицей свойств)")]
    [SerializeField] private int id;
    [Tooltip("Путь к иконке свойства")]
    [SerializeField] private string imageName;
    [Tooltip("Граница минимального значения свойства")]
    [SerializeField] private float cap;
    [Tooltip("Граница минимального текущего значения свойства")]
    [SerializeField] private float curCap;
    [Tooltip(" Если true, то curValue всегда будет равно Value")]
    [SerializeField] private bool useValueOnly;
    [TextArea]
    [Tooltip("Описание свойства, выводимое в компонентах UI")]
    [SerializeField] private string description;

    public string Name => name;

    public string ShortName => shortName;

    public int Id => id;

    public string ImageName => imageName;

    public float Cap => cap;

    public float CurCap => curCap;

    public bool UseValueOnly => useValueOnly;

    public string Description => description;
}
