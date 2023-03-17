using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[DataContract]
public class ObjectProperty
{
    protected string name;
    protected string shortName;
    protected int id;
    [JsonProperty]
    protected string imageName;
    [JsonProperty]
    protected float baseValue;
    /// <summary>
    /// Бонус от надетых предметов и модулей
    /// Прибавляется непосредственно к базовому значению,
    /// так что эффекты бафов будут скалироваться от этого значения тоже
    /// </summary>
    protected float equipmentBonus = 0;
    [JsonProperty]
    protected float additiveValue;
    [JsonProperty]
    protected float curValue;
    protected float lowerCap;
    protected float curValueLowerCap;
    [JsonProperty]
    protected float modifier=1;
    /// <summary>
    /// Если true, то curValue всегда будет равно Value
    /// </summary>
    protected bool useValueOnly;

    protected ObjectPropertyDescription description;

    public delegate void OnPropertyChanged(float oldCurValue, float newCurValue, float oldValue, float newValue);

    private OnPropertyChanged changeCallback;


    public float BaseValue => baseValue;

    public float AdditiveValue => additiveValue;

    public float Modifier => modifier;
    protected float CalcValue() => modifier * (baseValue+equipmentBonus) + additiveValue;
    public float Value
    {
        get
        {
            var v = CalcValue();
            return v>lowerCap?v:lowerCap;;
        }
    }
    
    public ObjectProperty(string name,string shortName,int id,string imageName,float cap,float curCap,float baseValue,bool useValueOnly = false)
    {
        this.name = name;
        this.shortName = shortName;
        this.id = id;
        this.imageName = imageName;
        this.baseValue = baseValue;
        lowerCap = cap;
        curValueLowerCap = curCap;
        curValue = Value;
        this.useValueOnly = useValueOnly;
    }

    public ObjectProperty(string name,float baseValue,float curValue = -1)
    {
        description = (ObjectPropertyDescription)Resources.Load(Consts.pathToObjectProperties + name);
        this.name = description.Name;
        this.shortName = description.ShortName;
        this.id = description.Id;
        this.imageName = description.ImageName;
        this.lowerCap = description.Cap;
        this.curValueLowerCap = description.CurCap;
        this.baseValue = baseValue;
        useValueOnly = description.UseValueOnly;
        if (curValue < 0)
        {
            this.curValue = Value;
        }
        else
        {
            this.curValue = Mathf.Min(curValue, Value);
        }
    }

    public ObjectProperty(ObjectPropertyDescription descr,float baseValue,float curValue=-1)
    {
        ExtractDescription(descr);
        
        this.baseValue = baseValue;
        if (curValue < 0)
        {
            this.curValue = Value;
        }
        else
        {
            this.curValue = Mathf.Min(curValue, Value);
        }
    }
    [JsonConstructor]
    public ObjectProperty(string imageName, float baseValue,float additiveValue,float curValue,float modifier)
    {
        ObjectPropertyDescription desc = Resources.Load("ObjectProperties/" + imageName) as ObjectPropertyDescription;
        ExtractDescription(desc);
        this.baseValue = baseValue;
        this.additiveValue = additiveValue;
        this.modifier = modifier;
        this.curValue = curValue;
    }
    /// <summary>
    /// Получает информацию из описния свойства
    /// </summary>
    private void ExtractDescription(ObjectPropertyDescription descr)
    {
        description = descr;
        this.name = description.Name;
        this.shortName = description.ShortName;
        this.id = description.Id;
        this.imageName = description.ImageName;
        this.lowerCap = description.Cap;
        this.curValueLowerCap = description.CurCap;
        useValueOnly = description.UseValueOnly;
    }

    /// <summary>
    /// Реинициализирует свойство, изменяя его базовое значение
    /// Используется при смене оружия
    /// </summary>
    /// <param name="value">новое базовое значение</param>
    public void Reinitialize(float value)
    {
        if (useValueOnly)
        {
            baseValue = value;
        }
        else
        {
            float p =(Value==0)? 0 : curValue / Value;
            baseValue = value;
            curValue = p*Value;
            //curValue = value;
        }
    }

    public void Reinitialize(float baseValue, float additiveValue, float modifier, float curValue)
    {
        this.baseValue = baseValue;
        this.additiveValue = additiveValue;
        this.modifier = modifier;
        this.curValue = curValue;
        changeCallback?.Invoke(GetCurValue(),GetCurValue(),Value,Value);
    }

    public void RegisterChangeCallback(OnPropertyChanged callback, bool allowDoubleRegistration = false)
    {
        //Запрещаем повторное добавление одного и того же метода в колбэк, если прямо не прописано обратное
        if (!allowDoubleRegistration && changeCallback!= null && changeCallback.GetInvocationList().Contains(callback))
        {
            return;
        }

        changeCallback += callback;
    }

    public void UnRegisterCallback(OnPropertyChanged callback)
    {
        changeCallback -= callback;
    }

    public float GetCurValue()
    {
        return (useValueOnly)?Value:curValue;
    }
    /// <summary>
    /// Устанавливает бонус от экипировки
    /// </summary>
    public void SetEquipmentBonus(float bonus)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        //float p = curValue / Value;
        float p =(Value==0)? 0 : curValue / Value;
        Debug.Log($"cur = {curValue}, v = {Value}");
        equipmentBonus = bonus;
        curValue = Value * p;
        CheckCurValueLimit();
        Debug.Log($"Установка бонуса {bonus}, новое значение свойства {Value}, текущее значение: {curValue}");
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void ChangeEquipmentBonus(float change)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        
        float p =(Value==0)? 0 : curValue / Value;

        equipmentBonus += change;
        curValue = Value * p;
        CheckCurValueLimit();
        Debug.Log($"Установка бонуса {equipmentBonus}, новое значение свойства {Value}, текущее значение: {curValue}");
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    /// <summary>
    /// Сбрасывает бонус от экипировки
    /// </summary>
    public void ResetEquipmentBonus()
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        equipmentBonus = 0;
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void ConstantBuff(float b)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        additiveValue += b;
        curValue += b;
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
        
    }

    public void ConstantDebuff(float d)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        additiveValue -= d;
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void PercentBuff(float b)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        float p =(Value==0)? 0 : curValue / Value;
        ChangeModifier(b);
        //curValue += b * baseValue;
        curValue = Value * p;
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void PercentDebuff(float d)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        ChangeModifier(-d);
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void ChangeCurValue(float update)
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        curValue += update;
        CheckCurValueLimit();
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public float getPercent()
    {
        return curValue / Value;
    }

    public string GetName()
    {
        return name;
    }
    
    public string GetShortName()
    {
        return shortName;
    }

    public int GetId()
    {
        return id;
    }
    public string GetImageName()
    {
        return imageName;
    }

    public void Reset()
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        additiveValue = 0;
        curValue = Value;
        modifier = 1;
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public void Recover()
    {
        float oldCurValue = GetCurValue();
        float oldValue = Value;
        curValue = Value;
        changeCallback?.Invoke(oldCurValue,GetCurValue(),oldValue,Value);
    }

    public float GetModifier()
    {
        return modifier;
    }
    
    private void ChangeModifier(float change)
    {
        modifier += change;
    }

    private void CheckCurValueLimit()
    {
        if (curValue < curValueLowerCap) curValue = curValueLowerCap;
        if (curValue > Value) curValue = Value;
    }
}
