using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIStrategy: MonoBehaviour
{
    /// <summary>
    /// Рассчитывает применимость стратегии в данных обстоятельствах
    /// Возвращает значение от 0(абсолютно неприменима) до 100(идеально подходит)
    /// </summary>
    /// <param name="aiMind"></param>
    /// <returns></returns>
    public abstract float CalculateApplicability(AIMind mind);
    /// <summary>
    /// Изменяет приказ так, чтобы он способствовал выполнению данной стратегии
    /// </summary>
    /// <param name="order"></param>
    /// <param name="mind"></param>
    public abstract void ApplyStrategy(ShipOrder order, AIMind mind);
    /// <summary>
    /// Если установить этот флаг, то выполнение стратегии не будет прерываться до тех пор
    /// пока этот флаг не будет снят
    /// </summary>
    public bool IsLocked { get; }

}
