using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact
{
   private int _targetId;

   public int TargetId => _targetId;

   private ImpactType _type;

   public ImpactType Type => _type;

   private float value;

   public float Value => value;

  

   public Impact(int id,float value,ImpactType type)
   {
      _targetId = id;
      this.value = value;
      _type = type;
   }

   public Impact(ImpactDescription description,bool needDeltaNormalization ,PropertyManager propertyManager=null)
   {
      _targetId = description.TargetId;
      _type = description.Type;
      var property = propertyManager?.GetPropertyById(description.PropertyId);
      if (property != null)
      {
         value = property.GetCurValue();
      }
      else
      {
         value = description.DefaultValue;
      }

      if (needDeltaNormalization) value *= Time.deltaTime;
   }
   /// <summary>
   /// Конструктор копии
   /// </summary>
   /// <param name="imp"></param>
   public Impact(Impact imp)
   {
      _targetId = imp.TargetId;
      _type = imp.Type;
      value = imp.value;
   }

   public Impact GetCopy()
   {
      return new Impact(this);
   }

}
/// <summary>
/// Бафы увеличивают как максимальное значение свойства, так и текущие, чего нельзя сказать о дебафах
/// Бафы можно использовать для уменьшения парамаетров, а дебафы для их увеличения
/// </summary>
public enum ImpactType
{
   InstantDamage, InstantPercentDamage,ContinuousDamage,ContinuousPercentDamage, ConstantBuff, PercentBuff,ConstantDebuff,PercentDebuff
}
