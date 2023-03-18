using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class ThermalEnvironment : MMSingleton<ThermalEnvironment>
{
    [Tooltip("Этот параметр устанавливает, количество энергии, которое получают все объекты от ближайшей звезды." +
             "Благодаря тому, что количество излучаемой энергии зависит только от температуры объекта," +
             "то используя эти два параметра мы устанавливаем тепловое равновесие, которое гарантирует, что" +
             "все объекты изначально будут иметь заданную температуру, до тех пор пока на них ни" +
             "подействуют другие источники излучения, например лазер")]
    [SerializeField]
    protected float baseTemperature;
     [Tooltip("Количество энергии, излучаемой всеми объектами. Иными словами как быстро остывают объекты.")]
     [SerializeField]
    protected float baseCooling;
    [SerializeField]
    protected float heatTransferSpeed;
  
    //"Количество энергии, излучаемой всеми объектами. Иными словами как быстро остывают объекты."
   private float cooling;
   /// <summary>
   /// Количество энергии излучаемой в секунду ближайшей звездой
   /// Расчитывается исходя из базовой температуры и скорости охлаждения объектов
   /// </summary>
   private float emission;
    // Start is called before the first frame update

    public float Emission => emission;

    public float Cooling => cooling;

    public float HeatTransferSpeed => heatTransferSpeed;
    void Start()
    {
        cooling = baseCooling;
        emission = baseCooling * baseTemperature;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
