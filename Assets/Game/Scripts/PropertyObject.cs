using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyObject : MonoBehaviour
{
    protected PropertyManager _propertyManager;
    protected ShipController owner;
    public virtual void Initialize() //Иногда может выполниться раньше собственного Awake. Это нужно, чтобы к моменту инициализации способностей, все свойства были уже заданы
    {
        owner = gameObject.GetComponentInParent<ShipController>();
        _propertyManager = GetComponent<PropertyManager>();
        if (_propertyManager == null)//гарантия того что объекты с компонентом health имеют PropertyManager
        {
            _propertyManager = gameObject.AddComponent<PropertyManager>();
        }
    }
    protected virtual void Awake()
    {
        if (_propertyManager == null)
        {
            Initialize();
        }
    }
}
