using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserWeapon : PropertyObject, IShipActionController 
{
    [MMInspectorGroup("Beam Properties", true, 2)] 
    [SerializeField]
    protected GameObject beamPrefab;
    
    private List<LineRenderer> beamRenderers;
    [SerializeField]
    private LayerMask beamTargetMask;
    [SerializeField]
    private Transform beamStartPosition;
    [SerializeField] 
    protected float beamRange;
    [SerializeField] 
    protected float beamWidth;
    [Tooltip("Расстояния для компенсации неровности луча, добавляется к длине луча")]
    [SerializeField] protected float beamCompDist=0.2f;
    [Tooltip("Количество целей,пробиваемых насквозь")]
    [SerializeField] protected int pierceCount;
    [Tooltip("Количество рикошетов луча")]
    [SerializeField] protected int bounceCount;
    [Tooltip("Количество лучей лазера")]
    [SerializeField] protected int raysCount=1;
    
    [SerializeField]
    protected EffectDescription[] effects;
    [SerializeField]
    protected float energyDamage;
    [SerializeField]
    protected float energyConsumption;

    protected float lastAttackTime;

    protected ObjectProperty energyDamageProperty;
    
    /// <summary>
    /// Коллайдер цели по которой в данный момент бьет луч
    /// </summary>
    protected Collider2D[] targetColliders;
    // Start is called before the first frame update

    private ShipOrder currentOrder;

    public Transform BeamStartPosition => beamStartPosition;

    public float BeamRange => beamRange;

    public LayerMask BeamTargetMask => beamTargetMask;

    public override void Initialize()
    {
        base.Initialize();
        Debug.Log(_propertyManager);
        energyDamageProperty = _propertyManager.AddProperty("EnergyDamage", energyDamage);
    }
    
    void Start()
    {
        //_propertyManager = GetComponent<PropertyManager>();
        beamRenderers=new List<LineRenderer>();
        for (int i = 0; i < raysCount; i++)
        {
            var beamRenderer = Instantiate(beamPrefab, beamStartPosition.position, Quaternion.identity).GetComponent<LineRenderer>();
            beamRenderer.startWidth = beamWidth;
            beamRenderer.endWidth = beamWidth;
            beamRenderers.Add(beamRenderer);
            beamRenderer.gameObject.SetActive(false);
            beamRenderer.transform.parent = transform;
            Debug.Log("Add beam renderer!");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentOrder!=null && currentOrder.mainWeapon)
        {
            var energy = _propertyManager.GetPropertyById(8);
            if (energy.GetCurValue() > energyConsumption * Time.deltaTime)
            {
                LaserShooting();
                energy.ChangeCurValue(-energyConsumption*Time.deltaTime);
                return;
            }
        }
       
        HideBeams();
    }

    protected void LaserShooting()
    {
        /*for (int i = 0; i < beamRenderers.Count; i++)
        {
            DrawBeam(beamRenderers[i], beamStartPosition.up);
        }*/
        DrawNativeRenderers(true);
    }
    
     protected Collider2D[] DrawBeam(LineRenderer beam, Vector2 direction)
    {
        List<Collider2D> targets=new List<Collider2D>();
        List<Vector3> positions = new List<Vector3>();
        var col=Physics2D.OverlapPoint(beamStartPosition.position,beamTargetMask);
        ///Если вдруг точка из которой мы делаем рэйкаст оказывается внутри препятствия,то завершаем путь лазера
        if (col!=null && col.attachedRigidbody!=null && col.attachedRigidbody.GetComponent<Health>()==null)
        {
            //Debug.Log(col.name);
            HideBeams();
            return targets.ToArray();
        }
        positions.Add(beamStartPosition.position);
        DrawBeamRec(targets,positions,direction,beamRange+beamCompDist,pierceCount,bounceCount);
        beam.gameObject.SetActive(true);
        beam.positionCount = positions.Count;
        beam.SetPositions(positions.ToArray());
        return targets.ToArray();
    }

    private void OnDrawGizmos()
    {
        //var spread = AccuracyConverter.AccuracyToSpreadParabolical(accuracy);
        var spread = 0;
        var dir1 = ((Vector2) transform.right).MMRotate(-spread);
        var dir2 = ((Vector2) transform.right).MMRotate(spread);
        Gizmos.DrawRay(beamStartPosition.position,dir1*20);
        Gizmos.DrawRay(beamStartPosition.position,dir2*20);
    }

    /// <summary>
    /// Рекурсивно отрисовывает луч с заданными параметрами и сохраняет все задетые цели
    /// </summary>
    /// <param name="beam"></param>
    /// <param name="hitTargets">Массив, где будут сохранены все пораженные цели</param>
    /// <param name="positions">Массив, в котором сохраняются позиции луча</param>>
    /// <param name="direction">Направляющий вектор луча</param>
    /// <param name="distRemain">оставшаяся длина луча</param>
    /// <param name="pierceRemain">оставшееся количество пробитий цели</param>
    /// <param name="bounceRemain">оставшееся количество рикошетов луча</param>
    protected void DrawBeamRec(List<Collider2D> hitTargets,List<Vector3> positions,Vector2 direction,
        float distRemain,int pierceRemain,int bounceRemain)
    {
        var dist = distRemain;
        var startPosition = positions[positions.Count-1];
        var remP = pierceRemain;
        MMDebug.DrawPoint(startPosition,Color.red, 0.1f);
        ///Эта компенсация необходима, так как без нее луч не всегда точно определяет
        /// находится он в коллайдере препятствия
        var physicsCompensation=direction*0.05f;
        var col=Physics2D.OverlapPoint((Vector2)startPosition + physicsCompensation,beamTargetMask);
        //Debug.Log(col);
        ///Если вдруг точка из которой мы делаем рэйкаст оказывается внутри препятствия,то завершаем путь лазера

        if (col!=null && col.attachedRigidbody!=null && col.attachedRigidbody.GetComponent<Health>()==null)
        {
           // Debug.Log(col.name);
            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)startPosition+physicsCompensation,direction,distRemain,beamTargetMask);
        if (hits.Length > 0)
        {
            //Убираем все столкновения с собой чтобы не перекрывать дорогу лазеру
            hits = hits.Where((RaycastHit2D h) => h.collider != GetComponent<Collider2D>()).ToArray();
        }

        if (hits.Length > 0)
        {

            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                MMDebug.DrawPoint(hit.point,Color.magenta, 0.1f);
                //Debug.DrawRay(hit.point,direction*5,Color.magenta,10);
                Shield shield = hit.collider.GetComponent<Shield>();
                if (shield != null)
                {
                    /*
                    //Если попадаем по своему щиту, то ничего не делаем
                    if (shield.transform.root.gameObject == Owner.gameObject)
                    {
                        Debug.Log("passShield");
                    }
                    //Если попадаем по щиту противника, то воздействуем на него и завершаем цикл
                    else
                    {
                        positions.Add(hit.point);
                        AffectShield(shield);
                        break;
                    }
                    */
                }
                ///Если попадаем по уничтожимому объекту, то по возможности пробиваем его
                else if (hit.collider.attachedRigidbody!=null && hit.collider.attachedRigidbody.GetComponent<Health>() != null)
                {
                    hitTargets.Add(hit.collider);
                    pierceRemain--;
                    if (pierceRemain<0)
                    {
                        positions.Add(hit.point+direction*beamCompDist);
                        break;
                    }
                    else if (i==hits.Length-1)
                    {
                        positions.Add((Vector2)positions[^1] + direction*(beamCompDist+distRemain));
                    }
                }
                ///В противном случае инициируем рикошет
                else
                {
                    dist -= hit.distance;
                    //Debug.Log(hit.distance);
                    positions.Add(hit.point/*+direction*beamCompDist*/);
                    if(bounceRemain>0)
                    {
                        var newDirection = Vector2.Reflect(direction, hit.normal);
                    DrawBeamRec(hitTargets,positions,newDirection,dist,remP,bounceRemain-1);
                    }
                    break;
                }
            }
            
        }
        else
        {
            positions.Add((Vector2)startPosition+direction.normalized*distRemain);
        }
    }
    
    protected void HideBeams()
    {
        if (beamRenderers != null)
        {
            foreach (var beamRenderer in beamRenderers)
            {
                beamRenderer.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Отображает лучи в неимпульсном режиме и бьет по задетым объектам, если это указать
    /// </summary>
    protected void DrawNativeRenderers(bool hitTargets=false)
    {
        //var spread = AccuracyConverter.AccuracyToSpreadParabolical(accuracy);
        var spread = 0;
        var step = (beamRenderers.Count==1)?0:2 * spread / (beamRenderers.Count - 1);
        for (int i = 0; i < beamRenderers.Count; i++)
        {
            float angle = Mathf.Pow(-1, i) * step * ((i + 1) / 2);
            //Debug.Log(angle);
            var dir = ((Vector2) beamStartPosition.up).MMRotate(angle);
            targetColliders = DrawBeam(beamRenderers[i],dir);
            if (hitTargets)
            {
                foreach (var target in targetColliders)
                {
                    OnHitTarget(target);
                }
            }
        }
    }
    
    protected void OnHitTarget(Collider2D target)
    {
        if (target != null)
        {
            var propertyManager = target.attachedRigidbody.GetComponent<PropertyManager>();
            if (propertyManager != null)
            {
                AffectTarget(propertyManager);
            }
        }
    }

    protected void AffectTarget(PropertyManager target)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            var eff= new Effect(effects[i],_propertyManager);
            target.AddEffect(eff);
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    public ShipActionControllerType ControllerType => ShipActionControllerType.WeaponController;
}
