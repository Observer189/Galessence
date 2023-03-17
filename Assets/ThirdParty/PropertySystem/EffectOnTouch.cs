/*using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EffectOnTouch : DamageOnTouch
{
    [Tooltip("Эффекты, которые будут заложены изначально, не зависят от характеристик")]
    [SerializeField]
    protected EffectDescription[] startEffects;
    [Tooltip("Будет ли использоваться тот алгоритм отдачи, который я написал")]
    [SerializeField]
    protected bool UseCustomKnockback;
    [Tooltip("Должен ли объект быть автоматически отключен через некоторое время после Awake")]
    [SerializeField] protected bool useAutoDisable = false;
    [SerializeField] protected float autoDisableTime;
    [SerializeField] protected bool ignoreOtherOwnerObjects = true;

    [SerializeField] protected MMFeedbacks onHitShield;
    [Tooltip("Урон получаемы при поражении пули")]
    [SerializeField] protected int DamageTakenProjectile;
   protected List<Effect> _effects;

   [Header("Shield things")]
   ///Урон который получает пуля при столкновении с щитом
   public int DamageTakenShield=100000;
   /// <summary>
   /// Урон который наносится щитам при попадании
   /// </summary>
   public int DamageToShield=1;

   public void AddEffect(Effect ef)
   {
      _effects.Add(ef);
   }

   protected override void Awake()
   {
       base.Awake();
       _effects=new List<Effect>();
       if (startEffects != null)
       {
           for (int i = 0; i < startEffects.Length; i++)
           {
               AddEffect(new Effect(startEffects[i]));
           }
           Debug.Log(gameObject.name + "effects count: "+startEffects.Length);
       }
 
       if (useAutoDisable)
       {
           Invoke("AutoDisable",autoDisableTime);
       }
   }

   void Start()
   {
      
   }

   protected override void OnCollideWithDamageable(Health health)
   {
       Debug.Log("Collide "+health.gameObject.name+"go "+gameObject.name);
       // if what we're colliding with is a TopDownController, we apply a knockback force
      // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
            _colliderRigidBody = health.gameObject.MMGetComponentNoAlloc<Rigidbody>();

            if ((_colliderTopDownController != null) && (DamageCausedKnockbackForce != Vector3.zero) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                _knockbackForce = DamageCausedKnockbackForce;

                if (_twoD) // if we're in 2D
                {
                    if (UseCustomKnockback)
                    {
                        _knockbackForce = new Vector2(DamageCausedKnockbackForce.x,DamageCausedKnockbackForce.y).MMRotate(transform.rotation.eulerAngles.z);
                    }
                    else if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
                    {
                        Vector3 totalVelocity = _colliderTopDownController.Speed + _velocity;
                        _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, totalVelocity.normalized, 10f, 0f);
                    }
                    else if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
                    {
                        if (Owner == null) { Owner = this.gameObject; }
                        Vector3 relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                        _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, relativePosition.normalized, 10f, 0f);
                    }    
                }
                else // if we're in 3D
                {
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
                    {
                        Vector3 totalVelocity = _colliderTopDownController.Speed + _velocity;
                        _knockbackForce = DamageCausedKnockbackForce * totalVelocity.magnitude;
                    }
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
                    {
                        if (Owner == null) { Owner = this.gameObject; }
                        Vector3 relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                        _knockbackForce.x = relativePosition.normalized.x * DamageCausedKnockbackForce.x;
                        _knockbackForce.y = DamageCausedKnockbackForce.y;
                        _knockbackForce.z = relativePosition.normalized.x * DamageCausedKnockbackForce.z;
                    } 
                }

                if (DamageCausedKnockbackType == KnockbackStyles.AddForce)
                {
                    _colliderTopDownController.Impact(_knockbackForce.normalized, _knockbackForce.magnitude);
                }
            }

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            Debug.Log("effects: "+_effects.Count);
            // we apply the damage to the thing we've collided with
            bool hitProjectile = false;
            ///Если сталкиваемся с объектом со свойствами, то накладываем эффекты
            if (health is ExtendedHealth)
            {
                for (int i = 0; i < _effects.Count; i++)
                {
                    Debug.Log(_effects[i].getImpacts()[0].Value);
                    var pm = health.GetComponent<PropertyManager>();
                    if (pm != null)
                    {
                        pm.AddEffect(_effects[i]);
                    }
                }
            }
            ///В противном случае(например ракета или пуля) наносим урон обычный
            else
            {
                hitProjectile = true;
                health.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection);
            }

            int additionalDamage = (hitProjectile) ? DamageTakenProjectile : DamageTakenDamageable;
            if (DamageTakenEveryTime + additionalDamage > 0)
            {
                SelfDamage(DamageTakenEveryTime + additionalDamage);
            }
            //_effects.Clear();
   }

   protected void OnHitShield(Shield shield)
   {
       if (shield.transform.root.gameObject == Owner.gameObject)
       {
           Debug.Log("passShield");
           return;
       }

       Debug.Log("HitShield");
       onHitShield?.PlayFeedbacks();
       shield.HitShield(DamageToShield);
       SelfDamage(DamageTakenShield);
   }

   public override void OnTriggerEnter2D(Collider2D collider)
   {
       Shield s = collider.GetComponent<Shield>();
       if (s != null)
       {
           OnHitShield(s);
       }
       else if (collider.attachedRigidbody != null)
       {
           if (ignoreOtherOwnerObjects)
           {
               ///Игнорируем пули того же владельца, если надо
               var eff = collider.attachedRigidbody.GetComponent<DamageOnTouch>();
               if (eff != null && eff.Owner == Owner)
               {
                 return;
               }
           }

           Colliding(collider.attachedRigidbody.gameObject);
       }
       else
       {
           Colliding(collider.gameObject);
       }
   }

   protected override void OnCollideWithNonDamageable()
   {
       base.OnCollideWithNonDamageable();
       //_effects.Clear();
   }

   protected void AutoDisable()
   {
       //this.enabled = false;
       gameObject.SetActive(false);
   }

   public override void OnTriggerStay2D(Collider2D collider)
   {
       //base.OnTriggerStay2D(collider);
   }

   public void ClearEffects()
   {
       _effects.Clear();
   }
   /// <summary>
   /// Очищаем эффекты, чтобы они не стакались
   /// </summary>
   private void OnDisable()
   {
       ClearEffects();
   }

   protected override void OnEnable()
   {
       _effects=new List<Effect>();
       if (startEffects != null)
       {
           for (int i = 0; i < startEffects.Length; i++)
           {
               AddEffect(new Effect(startEffects[i]));
           }
           //Debug.Log(gameObject.name + "effects count: "+startEffects.Length);
       }
   }
}*/
