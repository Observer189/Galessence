using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fuzzy;
using MoreMountains.Tools;
using UnityEngine;

public class NavigationTargetStrategy : AIStrategy
{
    public ExpertSystemData expertSystemData;

    protected List<Rule> expertRules;
    public override float CalculateApplicability(AIMind mind)
    {
        return 100;
    }

    public override void ApplyStrategy(ShipOrder order, AIMind mind)
    {
        //order.movementHasRotationDirection = true;
        order.movementOrderType = MovementOrderType.TargetSpeed;
        Debug.Log(mind.Perception.navigationTarget);
        if (mind.Perception.navigationTarget != null)
        {
            /*Vector3 moveVec = new Vector3(mind.Perception.navigationTarget.position.x, 0,
                mind.Perception.navigationTarget.position.y);
            if (Vector2.Angle(mind.Perception.navigationTarget.position-mind.Ship.transform.position, mind.Ship.transform.up) < 5)
            {
                moveVec = moveVec.MMSetY(1);
            }*/
            ///Угол между вектором напраления корабля и вектором до цели. Если он отрицательный, то цель левее, ели положительный
            /// То правее(или наоборот не помню:D)
            var angleToTarget =
                -Vector2.SignedAngle(mind.Perception.navigationTarget.position - mind.Ship.transform.position,
                    mind.Ship.transform.up);
            //Debug.Log(angleToTarget);
            var distanceToTarget = (mind.Perception.navigationTarget.position - mind.Ship.transform.position).magnitude;
            var sideLeftFeeler = mind.Ship.Feelers[0];
            var frontLeftFeeler = mind.Ship.Feelers[1];
            var middleFeeler = mind.Ship.Feelers[2];
            var frontRightFeeler = mind.Ship.Feelers[3];
            var sideRightFeeler = mind.Ship.Feelers[4];
            ///Расстояния до препятствий на соответствующих сенсорах
            var sideLeftFeelerDistance = (sideLeftFeeler.FeelTarget != null) ? sideLeftFeeler.DistanceToTarget : 1000;
            var leftFeelerDistance = (frontLeftFeeler.FeelTarget != null) ? frontLeftFeeler.DistanceToTarget : 1000;
            var middleFeelerDistance = (middleFeeler.FeelTarget != null) ? middleFeeler.DistanceToTarget : 1000;
            var rightFeelerDistance = (frontRightFeeler.FeelTarget != null) ? frontRightFeeler.DistanceToTarget : 1000;
            var sideRightFeelerDistance = (sideRightFeeler.FeelTarget != null) ? sideRightFeeler.DistanceToTarget : 1000;
            ///Данные из настроек экспертной системы
            var inputVariables = expertSystemData.inputVariables;
            
           // Debug.Log(middleFeeler.FeelTarget);
            //Debug.Log(middleFeelerDistance);
             
            ///Алгоритм экспертной системы должен вызываться здесь!
            double[] inputData = new double[inputVariables.Length];
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "DirToTarget")] = angleToTarget;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "DistanceToTarget")] = distanceToTarget;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "LeftFeelerDistance")] = leftFeelerDistance;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "RightFeelerDistance")] = rightFeelerDistance;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "MiddleFeelerDistance")] = middleFeelerDistance;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "SideLeftFeelerDistance")] = sideLeftFeelerDistance;
            inputData[Array.FindIndex(inputVariables, (v) => v.name == "SideRightFeelerDistance")] = sideRightFeelerDistance;

            var mamda = new Mamdani(expertRules, inputData);

            var output = mamda.execute();

            ///Дефаззифицированная скорость корабля
            float targetSpeed = (float)output[1]/*(mind.Perception.navigationTarget.position - mind.Ship.transform.position).magnitude/2*/;
            Debug.Log(output[2]);
            ///Дефазифицированная скорость поворота корабля
            float targetRotation = (float)output[0]/*-angleToTarget * 5*/;
            ///Дефазифицированная горизонтальная скорость корабля, получаемая за счет работы боковых ускорителей 
            float targetHorizontalSpeed = (float)output[2];
            
            ///Здесь мы используя рассчитанные параметры отдаем приказ на движение кораблю
            //var moveVec = new Vector3(Math.Sign(targetRotation),Math.Sign(targetSpeed),0);
            var moveVec = new Vector3( targetRotation,targetSpeed, targetHorizontalSpeed);
            
            order.movement = moveVec;

            if (targetHorizontalSpeed > 0)
            {
                order.leftAdditionalMovement = true;
            }

            if (targetHorizontalSpeed < 0)
            {
                order.rightAdditionalMovement = true;
            }
            
        }
    }

    private void Start()
    {
        MamdaniInitialization();
    }

    protected void MamdaniInitialization()
    {
        ///Данные из настроек экспертной системы
        var inputVariables = expertSystemData.inputVariables;
        var outputVariables = expertSystemData.outputVariables;
        var rulesInfo = expertSystemData.rules;
        
        Variable[] vars = new Variable[inputVariables.Length+outputVariables.Length];
            for (int i = 0; i < inputVariables.Length+outputVariables.Length; i++)
            {
                vars[i] = new Variable(i);
            }

            Rule[] rules = new Rule[rulesInfo.Length];

            for (int i = 0; i < rulesInfo.Length; i++)
            {
                var conditions = new List<Condition>();
                
                for (int j = 0; j < rulesInfo[i].conditions.Length; j++)
                {
                    var termIndex = Array.FindIndex(rulesInfo[i].conditions[j].variable.possibleTerms,
                        (t) => t.name == rulesInfo[i].conditions[j].term);
                    var term = rulesInfo[i].conditions[j].variable.possibleTerms[termIndex];
                    List<double> intervals = new List<double>();
                    //Считаем что термы задаются в порядке от меньших к большим
                    if (termIndex == 0)
                    {
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        try
                        {
                            intervals.Add(rulesInfo[i].conditions[j].variable.possibleTerms[termIndex+1].lowerBound);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.LogError("Переменная в экспертной системе должна имет минимум 2 терма");
                            throw;
                        }
                    }
                    else if (termIndex == rulesInfo[i].conditions[j].variable.possibleTerms.Length-1)
                    {
                        try
                        {
                            intervals.Add(rulesInfo[i].conditions[j].variable.possibleTerms[termIndex-1].upperBound);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.LogError("Переменная в экспертной системе должна имет минимум 2 терма");
                            throw;
                        }
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        intervals.Add(term.upperBound);
                    }
                    else
                    {
                        intervals.Add(rulesInfo[i].conditions[j].variable.possibleTerms[termIndex-1].upperBound);
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        intervals.Add(rulesInfo[i].conditions[j].variable.possibleTerms[termIndex+1].lowerBound);
                    }

                    conditions.Add(new Condition(vars[Array.FindIndex(inputVariables,
                            (v)=>v.name==rulesInfo[i].conditions[j].variable.name)],new FuzzySet(intervals)));
                }
                
                var conclusions = new List<Conclusion>();
                
                for (int j = 0; j < rulesInfo[i].conclusions.Length; j++)
                {
                    var termIndex = Array.FindIndex(rulesInfo[i].conclusions[j].variable.possibleTerms,
                        (t) => t.name == rulesInfo[i].conclusions[j].term);
                    var term = rulesInfo[i].conclusions[j].variable.possibleTerms[termIndex];
                    List<double> intervals = new List<double>();
                    //Считаем что термы задаются в порядке от меньших к большим
                    if (termIndex == 0)
                    {
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        try
                        {
                            intervals.Add(rulesInfo[i].conclusions[j].variable.possibleTerms[termIndex+1].lowerBound);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.LogError("Переменная в экспертной системе должна имет минимум 2 терма");
                            throw;
                        }
                    }
                    else if (termIndex == rulesInfo[i].conclusions[j].variable.possibleTerms.Length-1)
                    {
                        try
                        {
                            intervals.Add(rulesInfo[i].conclusions[j].variable.possibleTerms[termIndex-1].upperBound);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.LogError("Переменная в экспертной системе должна имет минимум 2 терма");
                            throw;
                        }
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        intervals.Add(term.upperBound);
                    }
                    else
                    {
                        intervals.Add(rulesInfo[i].conclusions[j].variable.possibleTerms[termIndex-1].upperBound);
                        intervals.Add(term.lowerBound);
                        intervals.Add(term.upperBound);
                        intervals.Add(rulesInfo[i].conclusions[j].variable.possibleTerms[termIndex+1].lowerBound);
                    }

                    conclusions.Add(new Conclusion(vars[Array.FindIndex(outputVariables,
                            (v)=>v.name==rulesInfo[i].conclusions[j].variable.name)+outputVariables.Length],new FuzzySet(intervals),1.0));
                }

                rules[i] = new Rule(conditions, conclusions);
            }

            expertRules = rules.ToList();
    }
}
