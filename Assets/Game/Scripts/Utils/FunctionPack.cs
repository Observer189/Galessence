using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FunctionPack 
{
   public static float AngleToPoint(Vector2 yourPosition, Vector2 point)
   {
      return Mathf.Atan2(point.y - yourPosition.y,
         point.x - yourPosition.x) * Mathf.Rad2Deg - 90;
   }
   //возвращает направление в котором нужно разворачиваться, чтобы повернуться в сторону точки point
   public static int FindRotationDirection(Vector2 yourPosition ,float yourRotation,Vector2 point,float epsilon)
   {
      
      var angle = AngleToPoint(yourPosition, point);
      return FindRotationDirection(yourRotation, angle, epsilon);
   }

   public static int FindRotationDirection(float yourRotation,float desiredAngle,float epsilon)
   {
      var angle = NormalizeAngle(desiredAngle);
      var yourRot = NormalizeAngle(yourRotation);
      var dist = Mathf.Abs(yourRot - angle);
      if (dist < epsilon) return 0;
      var internalArc = dist;
      var externalArc = 360 - internalArc;
      if (angle > yourRot)
      {
         if (internalArc > externalArc)
            return -1;
       
         return 1;
         
      }
    
      if (internalArc > externalArc) return 1;
      return -1;
   }

   public static float NormalizeAngle(float angle) //приводит угол в градусах к эквивалентному >0 и <360
   {
      var res = angle % 360;
      res = res < 0 ? 360 + res : res;
      return res;
   }
   /// <summary>
   /// Парсить стандартное стринговое представление разрешения в объект класса Resolution
   /// </summary>
   /// <returns></returns>
   public static Resolution ParseResolution(string str)
   {
      var match=Regex.Match(str,@"(\d+)x(\d+) @ (\d+)Hz");

      if (!match.Success)
      {
         Debug.LogError("Некорректный формат строки разрешения");
      }

      Resolution res = new Resolution
      {
         width = int.Parse(match.Groups[1].Value),
         height = int.Parse(match.Groups[2].Value),
         refreshRate = int.Parse(match.Groups[3].Value)
      };

      return res;
   }

}
