using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Health : PropertyObject
{
    [Tooltip("Максимальное здоровье персонажа")]
    [SerializeField]
    protected float maxHealth;
    [Tooltip("Количество здоровья с которым персонаж появляется")]
    [SerializeField]
    protected float initialHealth;
    [Tooltip("Должен ли объект быть уничтожен при смерти")]
    [SerializeField]
    protected bool destroyOnDeath;
    [Tooltip("Время после смерти перед уничтожением объекта")]
    [SerializeField]
    protected float timeBeforeDestruction;
    [SerializeField]
    protected MMFeedbacks damageFeedback;
    [SerializeField]
    protected MMFeedbacks OnKillFeedback;
    public delegate void OnDamageDelegate(float damage);
    public OnDamageDelegate OnDamage;
    // death delegate
    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;
    
    protected ObjectProperty hProperty;

    protected bool isAlive = true;

    public float HP => hProperty.GetCurValue();
    public bool IsAlive => isAlive;

    public override void Initialize()
    {
        base.Initialize();
        hProperty = _propertyManager.AddProperty("Health", maxHealth, initialHealth);
        hProperty.RegisterChangeCallback(OnHealthChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoDamage(float damage)
    {
        hProperty.ChangeCurValue(-damage);
    }

    protected void CheckDeath()
    {
        if (hProperty.GetCurValue() <= 0)
        {
            Kill();
        }
    }

    protected void Kill()
    {
        isAlive = false;
        if (owner != null)
        {
            owner.Kill();
        }
        OnDeath?.Invoke();
        OnKillFeedback?.PlayFeedbacks();
        if (destroyOnDeath)
        {
            Invoke(nameof(DestroySelf), timeBeforeDestruction);
        }
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected void OnHealthChanged(float oldCurValue, float newCurValue, float oldValue, float newValue)
    {
        if(newCurValue<oldCurValue)
        damageFeedback?.PlayFeedbacks();
        //Debug.Log($"Damage: {oldCurValue - newCurValue}");
        OnDamage?.Invoke(oldCurValue - newCurValue);
        CheckDeath();
    }
}
