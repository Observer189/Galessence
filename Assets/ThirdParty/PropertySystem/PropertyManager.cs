using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropertyManager : MonoBehaviour
{
    /// <summary>
    /// список всех свойств объекта
    /// </summary>
    
    private Dictionary<int,ObjectProperty> _properties;
    /// <summary>
    /// список эффектов наложенных на объект в данный момент
    /// </summary>
    private List<Effect> _effects;
    
    /// <summary>
    /// Словарь сопоставляющий наложенные эффекты со временем, которое прошло с момента наложения
    /// </summary>
    private Dictionary<Effect, float> effectsOverlayDuration;

    private Dictionary<EffectNature, ObjectProperty> resistances;
    /// <summary>
    /// Словарь различных бонусов от экипировки
    /// Ключ хранит название экипировки, от которой получен бонус (weapon, helmet etc)
    /// В значении же хранятся все бонусы получаемые от конкретного элемента экипировки
    /// </summary>
    private Dictionary<string, Dictionary<int, float>> equipmentBonuses;

    public PropertyManager()
    {
        _properties=new Dictionary<int, ObjectProperty>();
        _effects = new List<Effect>();
       resistances=new Dictionary<EffectNature, ObjectProperty>();
       equipmentBonuses = new Dictionary<string, Dictionary<int, float>>();
       effectsOverlayDuration = new Dictionary<Effect, float>();
    }

    private void Awake()
    {
       
    }
    
    /// <summary>
    /// Добавляет заданное свойство если его еще нет в списке и возвращает соответствующее в качестве результата
    /// </summary>
    public ObjectProperty AddProperty(string name, float baseValue, float curValue = -1)
    {
        var property = new ObjectProperty(name,baseValue,curValue);
        if (!_properties.ContainsKey(property.GetId()))
        {
            _properties.Add(property.GetId(), property);
        }
        else
        {
            ///Реинициализация свойства новым значением если оно уже есть в списке
            _properties[property.GetId()].Reinitialize(baseValue);
        }
        //Debug.Log("Properties count ="+_properties.Count);
        return _properties[property.GetId()];
    }
    
    /// <summary>
    /// Добавляет заданное свойство если его еще нет в списке и возвращает соответствующее в качестве результата
    /// </summary>
    public ObjectProperty AddProperty(ObjectPropertyDescription description, float baseValue, float curValue = -1)
    {
        var property = new ObjectProperty(description,baseValue,curValue);
        if (!_properties.ContainsKey(property.GetId()))
        {
            _properties.Add(property.GetId(), property);
        }
        else
        {
            ///Реинициализация свойства новым значением если оно уже есть в списке
            _properties[property.GetId()].Reinitialize(baseValue);
        }
        //Debug.Log("Properties count ="+_properties.Count);
        return _properties[property.GetId()];
    }

    public ObjectProperty AddProperty(ObjectProperty property)
    {
        if (!_properties.ContainsKey(property.GetId()))
        {
            _properties.Add(property.GetId(), property);
        }
        else
        {
            _properties[property.GetId()].Reinitialize(property.BaseValue,property.AdditiveValue,property.Modifier,property.GetCurValue());
        }
        return _properties[property.GetId()];
    }

   /* /// <summary>
    /// Метод для загрузки свойств и эффектов из сохранения
    /// </summary>
    public void Load(PropertyManagerSave save)
    {
        for (int i = 0; i < save.properties.Length; i++)
        {
            AddProperty(save.properties[i]);
        }
    }*/
    

    public void AddResistance(EffectNature nature, ObjectProperty resist)
    {
        resistances[nature] = resist;
    }

    public ObjectProperty GetPropertyByName(string name)
    {
        foreach (var prop in _properties)
        {
            if (prop.Value.GetName().Equals(name))
            {
                return prop.Value;
            }
        }
        Debug.LogError($"Нет свойства с заданным именем \"{name}\"");
        return null;
    }
    public ObjectProperty GetPropertyById(int id)
    {
        //Debug.LogError("Нет свойства с заданным id: "+id);
        if(_properties.TryGetValue(id, out ObjectProperty property))
        {
            return property;
        }
        return null;
    }

    private void Update()
    {
        for (int i=0;i<_effects.Count;i++)
        {
            var eff = _effects[i];
            foreach (var imp in eff.ContinuousImpacts)
            {
                //Debug.Log("change");
                var prop = GetPropertyById(imp.TargetId);
                var resist = GetResistance(eff.Nature);
                float dmg=0;
                if (imp.Type == ImpactType.ContinuousDamage) dmg = imp.Value;
                else if(imp.Type==ImpactType.ContinuousPercentDamage)
                {
                    dmg = imp.Value * prop.Value;
                }
                dmg =dmg - Mathf.Sign(imp.Value)*Mathf.Abs(dmg) * resist; 
                //Debug.Log("finally dmg"+dmg);
                prop.ChangeCurValue(dmg*(Time.deltaTime));
            }
            
            effectsOverlayDuration[eff] += Time.deltaTime;
            if (eff.Duration > 0 && effectsOverlayDuration[eff] > eff.Duration)
            {
                effectsOverlayDuration.Remove(eff);
                RemoveEffect(eff);
                i--;
            }
        }
    }

    public int GetPropertyCount()
    {
        return _properties.Count;
    }

    public void RecoverAll()
    {
        foreach (var prop in _properties)
        {
            prop.Value.Recover();
        }
    }

    public void AddEffect(Effect effect)
    {
        var rand = Random.Range(0f, 1f);
        if (effect.Chance > rand)
        {
            var c = _effects.Count(x => x.Id == effect.Id);
            if ((effect.MaxStackCount==0)||(c < effect.MaxStackCount))
            {
                var resist = GetResistance(effect.Nature);
                if (effect.Duration != 0)
                {
                    var eff = (c > 0) ? effect.GetCopy() : effect;
                    _effects.Add(eff);
                    effectsOverlayDuration.Add(eff,0f);
                    //Debug.Log("Property count = "+_properties.Count);
                    //Debug.Log("add");
                    //StartCoroutine(RemoveEffect(effect));
                }

                Impact[] impacts = effect.getImpacts();
                foreach (var imp in impacts)
                {
                    if (imp.Type == ImpactType.InstantDamage)
                    {
                        var dmg = imp.Value;
                        dmg = dmg - dmg * resist;
                        GetPropertyById(imp.TargetId)?.ChangeCurValue(-dmg);
                    }
                    else if (imp.Type==ImpactType.InstantPercentDamage)
                    {
                        var prop = GetPropertyById(imp.TargetId);
                        var dmg = imp.Value * prop.Value;
                        dmg = dmg - dmg * resist;
                        prop.ChangeCurValue(-dmg);
                    }
                    else if (imp.Type == ImpactType.ContinuousDamage)
                    {

                    }
                    else if(imp.Type == ImpactType.ContinuousPercentDamage)
                    {
                        
                    }
                    else if (imp.Type == ImpactType.ConstantBuff)
                    {
                        GetPropertyById(imp.TargetId).ConstantBuff(imp.Value);
                    }
                    else if (imp.Type == ImpactType.PercentBuff)
                    {
                        GetPropertyById(imp.TargetId).PercentBuff(imp.Value);
                    }
                    else if (imp.Type == ImpactType.ConstantDebuff)
                    {
                        GetPropertyById(imp.TargetId).ConstantDebuff(imp.Value);
                    }
                    else if (imp.Type == ImpactType.PercentDebuff)
                    {
                        GetPropertyById(imp.TargetId).PercentDebuff(imp.Value);
                    }

                }
            }
        }


    }

    private void RemoveEffect(Effect effect)
    {
        if(effect == null) return;
        
        Impact[] impacts = effect.getImpacts();
        foreach (var imp in impacts)
        {
            if (imp.Type == ImpactType.ConstantBuff)
            {
                GetPropertyById(imp.TargetId).ConstantDebuff(imp.Value);
            }
            else if (imp.Type == ImpactType.ConstantDebuff)
            {
                GetPropertyById(imp.TargetId).ConstantDebuff(-imp.Value);
            }
            else if (imp.Type == ImpactType.PercentBuff)
            {
                GetPropertyById(imp.TargetId).PercentDebuff(imp.Value);
            }
            else if (imp.Type == ImpactType.PercentDebuff)
            {
                GetPropertyById(imp.TargetId).PercentDebuff(-imp.Value);
            }
        }
        //Debug.Log($"EffectCount={_effects.Count} mod = {GetPropertyById(1).GetModifier()}");
        _effects.Remove(effect);
    }
    /// <summary>
    /// Снимает с персонажа эффект с указанным id
    /// </summary>
    /// <param name="id">id снимаемого эффекта</param>
    /// <param name="removeAll">должны ли быть сняты все эффекты с данным id или только один</param>
    public void RemoveEffect(int id,bool removeAll=false)
    {
        if (removeAll)
        {
            var effects=_effects.FindAll(e => e.Id == id);
            for (int i = 0; i < effects.Count; i++)
            {
                RemoveEffect(effects[i]);
            }
        }
        else
        {
            RemoveEffect(_effects.Find(e=>e.Id==id));
        }
    }

    /*private IEnumerator RemoveEffect(Effect effect)
    { 
        yield return new WaitForSeconds(effect.Duration);
        Impact[] impacts = effect.getImpacts();
        foreach (var imp in impacts)
        {
            if (imp.Type == ImpactType.ConstantBuff)
            {
                GetPropertyById(imp.TargetId).ConstantDebuff(imp.Value);
            }
            else if (imp.Type == ImpactType.ConstantDebuff)
            {
                GetPropertyById(imp.TargetId).ConstantDebuff(-imp.Value);
            }
            else if (imp.Type == ImpactType.PercentBuff)
            {
                GetPropertyById(imp.TargetId).PercentDebuff(imp.Value);
            }
            else if (imp.Type == ImpactType.PercentDebuff)
            {
                GetPropertyById(imp.TargetId).PercentDebuff(-imp.Value);
            }
        }
         //Debug.Log($"EffectCount={_effects.Count} mod = {GetPropertyById(1).GetModifier()}");
        _effects.Remove(effect);
    }*/

    public float GetResistance(EffectNature nature)
    {
        var resist = 0f;
        if (resistances.ContainsKey(nature))
        {
            resist = resistances[nature].GetCurValue();
        }

        return resist;
    }
    
    public void SetEquipmentBonuses(string eqType, Dictionary<int,(float,float)> bonuses)
    {
       RemoveEquipmentBonuses(eqType);
       var dict=new Dictionary<int,float>();
       foreach (var b in bonuses)
       {
           var prop = GetPropertyById(b.Key);
           if (prop != null)
           {
               var bonus = b.Value.Item1 + b.Value.Item2 * prop.BaseValue;
               prop.ChangeEquipmentBonus(bonus);
               dict[b.Key]=bonus;
           }
       }

       equipmentBonuses[eqType] = dict;
    }
    /// <summary>
    /// Снимает все бонусы от указанного типа экипировки
    /// </summary>
    /// <param name="eqType"></param>
    public void RemoveEquipmentBonuses(string eqType)
    {
        if (equipmentBonuses.ContainsKey(eqType))
        {
            foreach (var b in equipmentBonuses[eqType])
            {
                var prop = GetPropertyById(b.Key);

                prop?.ChangeEquipmentBonus(-b.Value);
            }

            equipmentBonuses.Remove(eqType);
        }
    }

    /// <summary>
    /// Убирает все бонусы от экипировки
    /// </summary>
    public void ResetEquipmentBonuses()
    {
        foreach (var prop in _properties)
        {
            prop.Value.ResetEquipmentBonus();
        }
    }

    public ObjectProperty[] GetProperties()
    {
        return _properties.Values.ToArray();
    }
}