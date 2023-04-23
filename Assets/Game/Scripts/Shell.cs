using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEditor.Rendering;
using UnityEngine;


public class Shell : PropertyObject
{
    [Tooltip("Теплоемкость объекта")]
    [SerializeField]
    protected float thermalCapacity;
    [Tooltip("Стартовая температура тела")]
    [SerializeField]
    protected float baseTemperature;

    [Tooltip("Система температурных ограничений. При достижении определенной температуры объект будет" +
             "получать указанное количество урона в секунду. Также урон будет линейно интерполироваться между указанными" +
             "ограничесниями")]
    [SerializeField]
    protected TemperatureThreshold[] temperatureThresholds;
    [SerializeField]
    protected Gradient gradient;
    [SerializeField]
    protected float minImpulseToCollisionDamage;
    [SerializeField]
    protected float collisionDamageScale;
    [SerializeField]
    protected SpriteRenderer model;
    [Tooltip("Фидбэк, вызываемый при столкновении нанесшем урон")]
    protected MMFeedbacks collisionFeedback;

    public Rigidbody2D Body => body;

    private ObjectProperty thermalEnergy;
    private Health health;
    
    //список всех сил воздействующих на объект на этом кадре
    private List<Vector2> affectedForces;
    private HashSet<Shell> collidedShells;
    private Rigidbody2D body;
    private IMovementSystem movementSystem;

    public float Temperature => thermalEnergy.GetCurValue() / (thermalCapacity * body.mass);
    
    public override void Initialize()
    {
        base.Initialize();
        body = GetComponent<Rigidbody2D>();
        thermalEnergy =
            _propertyManager.AddProperty("ThermalEnergy", 
                float.MaxValue, baseTemperature * thermalCapacity * body.mass);

        affectedForces = new List<Vector2>();
        collidedShells = new HashSet<Shell>();
        health = GetComponent<Health>();
        movementSystem = GetComponent<IMovementSystem>();
    }

    private void Update()
    {
        #region TemperatureStuff
        var cooling = ThermalEnvironment.Instance.Cooling * Temperature;
        var heating = ThermalEnvironment.Instance.Emission;

        heating -= cooling;
         //Обмениваемся теплом с телами с которыми взаимодействуем
         List<Shell> destroyedShells = new List<Shell>();
        foreach (var otherShell in collidedShells)
        {
            if (otherShell != null)
            {
                //Debug.Log(Mathf.Min((otherShell.Temperature - Temperature)*thermalCapacity*body.mass,
                 //   (otherShell.Temperature - Temperature) * ThermalEnvironment.Instance.HeatTransferSpeed*Time.deltaTime));
                var change = (otherShell.Temperature - Temperature) * ThermalEnvironment.Instance.HeatTransferSpeed*Time.deltaTime;
                var balanceEnergyNeeded =
                    Mathf.Abs((otherShell.Temperature - Temperature) * thermalCapacity * body.mass);
                if (balanceEnergyNeeded < Mathf.Abs(change))
                {
                    change = Mathf.Sign(change) * balanceEnergyNeeded;
                }
                thermalEnergy.ChangeCurValue(change);
                
            }
            else
            {
                destroyedShells.Add(otherShell);
            }
        }
        //Удаляем тела, которые видимо были разрушены
        for (int i = 0; i < destroyedShells.Count; i++)
        {
            collidedShells.Remove(destroyedShells[i]);
        }
        thermalEnergy.ChangeCurValue(heating);
        var sprite = model;
        if (Input.GetKey(KeyCode.Space))
        {
            //sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
                sprite.color = gradient.Evaluate(Temperature / 2000);
        }
        else
        {
            if (sprite != null)
                sprite.color = Color.white;
        }
        

        #endregion

        #region TemperatureDamage

        if (temperatureThresholds!=null && temperatureThresholds.Length > 0)
        {
            ///Находим тот диапазон в котором лежит наша температура
            int diapNum = -1;
            for (int i = 0; i < temperatureThresholds.Length; i++)
            {
                if(Temperature < temperatureThresholds[i].Threshold) break;
                diapNum = i;
            }

            if (diapNum != -1)
            {
                ///Если вышли за крайний диапазон, то мгновенно убиваем объект
                if (diapNum == temperatureThresholds.Length - 1)
                {
                    health.DoDamage(health.HP);
                }
                else
                {
                    ///Интерполируем урон в дипазоне
                    float dmg = (temperatureThresholds[diapNum].Damage + temperatureThresholds[diapNum + 1].Damage) / 2;
                    dmg *= Time.deltaTime;
                    health.DoDamage(dmg);
                    //Debug.Log(dmg);
                }
            }
            
        }

        #endregion

        #region Movement

        
        if (affectedForces.Count > 0)
        {
            Vector2 resultantForce = affectedForces.Aggregate((Vector2 v1, Vector2 v2)=>v1+v2);
            body.AddForce(resultantForce,ForceMode2D.Impulse);
            if (movementSystem != null)
            {
                if (body.velocity.sqrMagnitude > movementSystem.MaxSpeed * movementSystem.MaxSpeed)
                {
                    body.velocity = body.velocity.normalized * movementSystem.MaxSpeed;
                }
            }

            affectedForces.Clear();
        }
        #endregion
    }

    public void AddImpulse(Vector2 impulse)
    {
        affectedForces.Add(impulse);
    }

    Vector2 ComputeTotalImpulse(Collision2D collision) {
        Vector2 impulse = Vector2.zero;

        int contactCount = collision.contactCount;
        for(int i = 0; i < contactCount; i++) {
            var contact = collision.GetContact(i);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }

        return impulse;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        #region collisionDamage

        Vector2 normal = col.GetContact(0).normal;
        Vector2 impulse = ComputeTotalImpulse(col);

        // Both bodies see the same impulse. Flip it for one of the bodies.
        if (Vector3.Dot(normal, impulse) < 0f)
            impulse *= -1f;

        Vector2 myIncidentVelocity = body.velocity - impulse / body.mass;

        Vector2 otherIncidentVelocity = Vector2.zero;
        var otherBody = col.rigidbody;
        if(otherBody != null)
        {
            otherIncidentVelocity = otherBody.velocity;
            if(!otherBody.isKinematic)
                otherIncidentVelocity += impulse / otherBody.mass;
        }

        // Compute how fast each one was moving along the collision normal,
        // Or zero if we were moving against the normal.
        float myApproach = Mathf.Max(0f, Vector3.Dot(myIncidentVelocity, normal));
        float otherApproach = Mathf.Max(0f, Vector3.Dot(otherIncidentVelocity, normal));

        float damage = Mathf.Max(0f, otherApproach - myApproach - minImpulseToCollisionDamage);
        if(damage>0)
            collisionFeedback?.PlayFeedbacks();
        
        health.DoDamage(damage * collisionDamageScale);

        #endregion

        var shell = col.rigidbody.GetComponent<Shell>();
        if (shell != null)
        {
            if (!collidedShells.Contains(shell))
            {
                collidedShells.Add(shell);
            }
        }
    }

    private void OnGUI()
    {
        //var point = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        var point = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(point.x,Screen.height-point.y,70,50),$"{Temperature} K");
        
        
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        var shell = col.rigidbody.GetComponent<Shell>();
        if (shell != null)
        {
            if (collidedShells.Contains(shell))
            {
                collidedShells.Remove(shell);
            }
        }
    }
}
[Serializable]
public class TemperatureThreshold
{
    public float Threshold;
    public float Damage;
}
