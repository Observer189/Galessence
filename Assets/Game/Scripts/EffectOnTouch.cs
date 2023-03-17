using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class EffectOnTouch : MonoBehaviour
{
    public float DamageTakenEveryTime;

    public float DamageTakenDamageable;
    
    public float DamageTakenNonDamageable;
    [Tooltip("Список слоев с которым будет произведено взаимодействие")]
    [SerializeField] 
    protected LayerMask targetLayer;
    
    [Tooltip("Эффекты, которые будут заложены изначально, не зависят от характеристик")]
    [SerializeField]
    protected EffectDescription[] startEffects;

    [Tooltip("Эффекты, которые будут заложены изначально, не зависят от характеристик и при этом накладываются сквозь щит")] 
    [SerializeField]
    protected EffectDescription[] startThroughShieldEffects;
    [Tooltip("Толчок придаваемый объекту при соприкосновении")]
    [SerializeField]
    protected Vector2 knockBack;
    
    [Tooltip("Должен ли объект быть автоматически отключен через некоторое время после Awake")]
    [SerializeField] protected bool useAutoDisable = false;
    [SerializeField] protected float autoDisableTime;
    [Tooltip("Следует ли отключить столкновения с владельцем этого объекта?")]
    [SerializeField] 
    protected bool ignoreOwnerCollision;
    [Tooltip("Способ определения направления толчка" +
        "OwnDir - основано на собственном направлении - подходит для зон ударов" +
         "VelocityDir - основано на направлении собственной скорости - подходит для метательных объектов")]
    [SerializeField] 
    protected KnockbackDirDefinition knockbackDirDefinition;
    [Tooltip("Промежуток времени, через который на объект будет вновь наложен эффект в том случае " +
             "Если все это время объект находится в зоне эффекта")]
    [SerializeField]
    protected float repeatedEffectApplicationTime = 1f;
    [SerializeField]
    protected MMFeedbacks hitDamageableFeedback;
    [SerializeField]
    protected MMFeedbacks hitNonDamageableFeedback;
    [SerializeField]
    protected MMFeedbacks hitShieldFeedback;
    
    protected List<Effect> _effects;

    protected List<Effect> throughShieldEffects;
    
    protected Health objectHealth;
    protected Rigidbody2D rigidBody;
    /// <summary>
    /// Владелец объекта, наносящего урон
    /// </summary>
    protected ShipController owner;
    /// <summary>
    /// Список объектов, находящихся в зоне эффекта + время когда в последний раз на них производилось
    /// наложение эффекта
    /// </summary>
    protected Dictionary<Collider2D,float> collidingObjects;
    public void AddEffect(Effect ef)
    {
        _effects.Add(ef);
    }

    public void AddThroughShieldEffect(Effect ef)
    {
        throughShieldEffects.Add(ef);
    }

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        _effects=new List<Effect>();
        throughShieldEffects=new List<Effect>();
        collidingObjects = new Dictionary<Collider2D,float>();
       /* if (startEffects != null)
        {
            for (int i = 0; i < startEffects.Length; i++)
            {
                AddEffect(new Effect(startEffects[i]));
            }
            //Debug.Log(gameObject.name + "effects count: "+startEffects.Length);
        }*/
 
        if (useAutoDisable)
        {
            Invoke("AutoDisable",autoDisableTime);
        }
    }
    

    private void Start()
    {
        objectHealth = GetComponent<Health>();
        if (objectHealth != null)
        {
            objectHealth.OnDeath += OnDeath;
        }
    }

    private void Update()
    {
        HandleCollidedObjects();
    }

    protected void OnCollideWithDamageable(PropertyManager propertyManager)
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            propertyManager.AddEffect(_effects[i]);
        }
        var body = propertyManager.GetComponent<Rigidbody2D>();
       
        ApplyKnockback(body);
        
        hitDamageableFeedback?.PlayFeedbacks();
        
        objectHealth?.DoDamage(DamageTakenDamageable + DamageTakenEveryTime);
    }

    protected void OnCollideWithNonDamageable(Collider2D collider)
    {
        hitNonDamageableFeedback?.PlayFeedbacks();
        
        objectHealth?.DoDamage(DamageTakenNonDamageable + DamageTakenEveryTime);
    }

    /*protected void OnCollideWithShield(Shield shield)
    {
        hitShieldFeedback?.PlayFeedbacks();
        var propertyManager = shield.OwnerCollider.GetComponent<PropertyManager>();
        if (propertyManager != null)
        {
            for (int i = 0; i < throughShieldEffects.Count; i++)
            {
                propertyManager.AddEffect(throughShieldEffects[i]);
            }
        }

        ApplyKnockback(shield.OwnerCollider.attachedRigidbody,shield.KnockBackModifier);
        
        objectHealth?.DoDamage(900000);
    }*/

    protected void ApplyKnockback(Rigidbody2D body, float knockbackModifier = 1)
    {
        if (body != null)
        {
            Vector2 dir;
            if (knockbackDirDefinition == KnockbackDirDefinition.VelocityDir && rigidBody != null)
            {
                dir = rigidBody.velocity.normalized;
            }
            else if(knockbackDirDefinition == KnockbackDirDefinition.RightDir)
            {
                dir = ((Vector2) transform.right).normalized;
            }
            else
            {
                dir = ((Vector2)(body.transform.position - transform.position)).normalized;
            }

            var vec1 = dir * knockBack.x;
            Vector2 vec2;
            if (Vector2.SignedAngle(dir, Vector2.up) <= 0)
            {
                vec2 = dir.MMRotate(-90) * knockBack.y;
            }
            else
            {
                vec2 = dir.MMRotate(90) * knockBack.y;
            }
            //Debug.Log($"dir = {dir}, vec1 = {vec1}, vec2 = {vec2}, sum = {vec1+vec2}");
            body.AddForce((vec1 + vec2) * knockbackModifier, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!MMLayers.LayerInLayerMask(other.gameObject.layer, targetLayer))
        {

            return;
        } 
        Debug.Log(other.gameObject.name);
        collidingObjects.Add(other,0);
    }

    protected void HandleCollidedObjects()
    {
        ///Множество коллайдеров для тех персонажей, чей щит мы затронули
        HashSet<Collider2D> shieldsOwners = new HashSet<Collider2D>();

        var co = collidingObjects.ToArray();
        var updateTime = new List<Collider2D>();
        ///проходимся и записываем всех владельцев щитов, чтобы потом игнорировать их коллайдеры
        foreach (var col in co)
        {
            
            if(!collidingObjects.ContainsKey(col.Key)) continue;
            //Debug.Log($"{Time.time}: {col.Key.gameObject.name} - {col.Value}");
            var shield = col.Key.GetComponent<Shield>();
            if (shield != null)
            {
                //Debug.Log("Shield detected!");
                //shieldsOwners.Add(shield.OwnerCollider);
                if (Time.time - col.Value > repeatedEffectApplicationTime)
                {
                    updateTime.Add(col.Key);
                    //OnCollideWithShield(shield);
                }
            }
        }

        foreach (var col in co)
        {
            ///На любом шаге цикла может сработать OnDisable
            /// Тогда словарь соприкасаемых объектов полностью очистится
            /// И мы будем пытаться обратиться к кобъектам, к которым мы уже
            /// Не прикасаемся.
            /// Чтобы такого не происходило делаем эту проверку
            if(!collidingObjects.ContainsKey(col.Key)) continue;
            ///Проверяем, что мы давно не накладывали эффект
            if (Time.time - collidingObjects[col.Key] > repeatedEffectApplicationTime)
            {
                if (col.Key.GetComponent<Shield>() == null && !shieldsOwners.Contains(col.Key))
                {
                    updateTime.Add(col.Key);
                    var propertyManager = col.Key.GetComponent<PropertyManager>();
                    if (propertyManager != null)
                    {
                        OnCollideWithDamageable(propertyManager);
                    }
                    else
                    {
                        OnCollideWithNonDamageable(col.Key);
                    }
                    
                }
            }
        }
        

        foreach (var col in updateTime)
        {
            collidingObjects[col] = Time.time;
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!MMLayers.LayerInLayerMask(other.gameObject.layer, targetLayer))
        {

            return;
        }
        collidingObjects.Remove(other);
    }

    public void SetOwner(ShipController character)
    {
        owner = character;

        if (ignoreOwnerCollision)
        {
            Physics2D.IgnoreCollision(owner.GetComponent<Collider2D>(),GetComponentInChildren<Collider2D>());
        }
    }

    public void ClearEffects()
    {
        _effects.Clear();
        throughShieldEffects.Clear();
    }
    /// <summary>
    /// Очищаем эффекты, чтобы они не стакались
    /// </summary>
    private void OnDisable()
    {
        ClearEffects();
       // collidingObjects.Clear();
    }
    /// <summary>
    /// Если к объекту пожключено здоровье, то при смерти объекта будет вызвана эта функция
    /// </summary>
    public void OnDeath()
    {
        this.enabled = false;
    }

    protected void OnEnable()
    {
        //_effects=new List<Effect>();
        if (startEffects != null)
        {
            for (int i = 0; i < startEffects.Length; i++)
            {
                AddEffect(new Effect(startEffects[i]));
            }
            //Debug.Log(gameObject.name + "effects count: "+startEffects.Length);
        }

        if (startThroughShieldEffects != null)
        {
            for (int i = 0; i < startThroughShieldEffects.Length; i++)
            {
                AddThroughShieldEffect(new Effect(startThroughShieldEffects[i]));
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        
        if (collider != null)
        {
            if (collider.enabled)
            {
                MMDebug.DrawGizmoCube(this.transform, 
                    new Vector3(),
                    collider.size,
                    false);
            }
            else
            {
                MMDebug.DrawGizmoCube(this.transform,
                    new Vector3(), 
                    collider.size,
                    true);
            }                
        }
    }
}
/// <summary>
/// Способ определения направления толчка
/// OwnDir - основано на собственном направлении - подходит для зон ударов
/// VelocityDir - основано на направлении собственной скорости - подходит для метательных объектов
/// </summary>
public enum KnockbackDirDefinition
{
    RightDir, VelocityDir, DirToTarget
}
