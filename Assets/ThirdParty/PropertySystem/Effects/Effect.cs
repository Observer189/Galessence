using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
   protected List<Impact> _impacts;

   protected List<Impact> continuousImpacts;

   public List<Impact> ContinuousImpacts => continuousImpacts;
   protected int id;
   protected float chance;
   protected float _duration;
   protected int maxStackCount;
   protected EffectNature nature;

   public int Id => id;
   public float Duration => _duration;

   public float Chance => chance;

   public int MaxStackCount => maxStackCount;

   public EffectNature Nature => nature;
   
   public Effect(int id,float duration, float chance,int maxStackCount,EffectNature nature)
   {
      this.id = id;
      _impacts=new List<Impact>();
      continuousImpacts=new List<Impact>();
      _duration = duration;
      this.chance = Mathf.Clamp(chance, 0f, 1f);
      this.maxStackCount = maxStackCount;
      this.nature = nature;
   }
   /// <summary>
   /// Автоинициализируеует эффект по имени файла и свойствам субъекта
   /// Если не передавать PropertyManager, то инициализируется дефолтными значениями
   /// </summary>
   /// <param name="name">Имя файла в котором хранится информация о создаваемом эффекте</param>
   /// <param name="propertyManager">мэнэджер свойств объекта, который использует эффект</param>
   public Effect(string name,PropertyManager propertyManager=null)
   {
      EffectDescription description = (EffectDescription)Resources.Load("Effects/"+name);
     Initializiation(description,propertyManager);
   }
   
   public Effect(EffectDescription description,PropertyManager propertyManager=null)
   {
      Initializiation(description,propertyManager);
   }
   /// <summary>
   /// Конструктор копии
   /// </summary>
   public Effect(Effect eff)
   {
      id = eff.Id;
      _duration = eff.Duration;
      chance = eff.Chance;
      maxStackCount = eff.MaxStackCount;
      nature = eff.nature;
      _impacts=new List<Impact>();
      continuousImpacts=new List<Impact>();
      foreach (var imp in eff._impacts)
      {
        AddImpact(imp.GetCopy());
      }
   }

   protected void Initializiation(EffectDescription description,PropertyManager propertyManager=null)
   {
      id = description.Id;
      _duration = description.Duration;
      chance = description.Chance;
      maxStackCount = description.MaxStackCount;
      nature = description.Nature;
      _impacts=new List<Impact>();
      continuousImpacts=new List<Impact>();
      foreach (var impactDescription in description.Impacts)
      {
         var imp=new Impact(impactDescription,description.NeedDeltaNormalization,propertyManager);
         AddImpact(imp);
      }
   }

   public Effect GetCopy()
   {
      return new Effect(this);
   }

   public void AddImpact(Impact imp)
   {
      _impacts.Add(imp);
      if (imp.Type == ImpactType.ContinuousDamage || imp.Type==ImpactType.ContinuousPercentDamage)
      {
         continuousImpacts.Add(imp);
      }
   }

   public Impact[] getImpacts()
   {
      return _impacts.ToArray();
   }
   
   
   
}

public enum EffectNature
{
   Physical,Energy,Plasma,Other
}
