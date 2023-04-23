using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIMind : MonoBehaviour
{
   public bool IsActive=false;
   
   public ShipController Ship => ship;

   public AIPerception Perception => perception;
   
   protected ShipController ship;
   protected AIPerception perception;
   /// <summary>
   /// Основные стратегии, которые могут быть применены данным кораблем
   /// Из всех возможных основных стратегий выбирается одна самая подходящая
   /// </summary>
   [SerializeField]
   protected AIStrategy[] mainStrategies;
   /// <summary>
   /// Если этот флаг поднят, то применимость стратегии рассматривается как вес вероятности ее выбора из остальных
   /// И таким образом выбирается не самая лучшая стратегия, но случайная, вероятность выбора которой тем выше, чем выше
   /// ее применимость
   /// </summary>
   [SerializeField]
   protected bool strategyChoiceIsStochastic = true;
   /// <summary>
   /// Время между сменой основной стратегии, небходимо, чтобы стратегия не менялась каждый кадр)
   /// </summary>
   [SerializeField]
   protected float timeBetweenChangeMainStrategy;
   /// <summary>
   /// Дополнительные стратегии корабля отвечающие обычно за стрельбу и способности корабля
   /// Несколько таких стратегий может быть активировано одновременно
   /// </summary>
   [SerializeField]
   protected AIStrategy[] additionalStrategies;
   
   protected AIStrategy currentMainStrategy;
   protected float timeAfterLastChangeMainStrategy;
   public void SetShip(ShipController s)
   {
      ship = s;
   }

   private void Awake()
   {
      perception = new AIPerception();
   }

   public void Update()
   {
      if (!IsActive || ship == null) return;
      
      perception.UpdateInfo(ship);
      
      ShipOrder order = new ShipOrder();
      ///Если пришло время менять стратегию и текущая стратегия не заблокирована, то выбираем новую стратегию
      if (mainStrategies.Length>0 && (currentMainStrategy == null ||
          (Time.time - timeAfterLastChangeMainStrategy > timeBetweenChangeMainStrategy && !currentMainStrategy.IsLocked)))
      {
         float maxApp = float.MinValue;
         float sum = 0;
         List<float> apps = new List<float>();
         AIStrategy bestStrategy = null;
         ///Считаем применимость каждой стратегии
         foreach (var strategy in mainStrategies)
         {
            float app = strategy.CalculateApplicability(this);
            apps.Add(app);
            sum += app;
            if (app > maxApp)
            {
               maxApp = app;
               bestStrategy = strategy;
            }
         }
         //Стохастически выбираем новую стратегию в соответствии с распределением их применимости 
         if (strategyChoiceIsStochastic)
         {
            float rand = Random.Range(0, sum);

            for (int i = 0; i < apps.Count; i++)
            {
               if (rand < apps[i])
               {
                  currentMainStrategy = mainStrategies[i];
                  break;
               }

               rand -= apps[i];
            }
            //На всякий случай
            if (currentMainStrategy == null)
            {
               currentMainStrategy = mainStrategies[0];
            }
         }
         ///Или же просто выбираем лучшую
         else
         {
            currentMainStrategy = bestStrategy;
         }
      }
      
      currentMainStrategy?.ApplyStrategy(order,this);

      foreach (var strategy in additionalStrategies)
      {
         var app = strategy.CalculateApplicability(this);
         var rand = Random.Range(0, 100);
         if (rand < app)
         {
            strategy.ApplyStrategy(order, this);
         }
      }
      
      ship.Owner?.UpdateOrder(order);
   }
}
