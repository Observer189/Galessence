using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "New Expert System", menuName = "FuzzyLogic/System data", order = 3)]
public class ExpertSystemData : ScriptableObject
{

    public FuzzyVariableInfo[] inputVariables = {};

    public FuzzyVariableInfo[] outputVariables= {};

    public FuzzyRuleInfo[] rules = {};

   // public bool[] showInputTerms = {};
    //public bool[] showOutputTerms = {};
    [ContextMenu("Generation/GenerateRules")]
    public void GenerateRules()
    {
        List<FuzzyRuleInfo> newRules = new List<FuzzyRuleInfo>();
        
        var dirToTargetVar = Array.Find(inputVariables, (v) => v.name == "DirToTarget");
        var distToTargetVar = Array.Find(inputVariables, (v) => v.name == "DistanceToTarget");
        var leftFeelerDistVar = Array.Find(inputVariables, (v) => v.name == "LeftFeelerDistance");
        var rightFeelerDistVar = Array.Find(inputVariables, (v) => v.name == "RightFeelerDistance");
        var middleFeelerDistVar = Array.Find(inputVariables, (v) => v.name == "MiddleFeelerDistance");
        var sideLeftFeelerDistVar = Array.Find(inputVariables, (v) => v.name == "SideLeftFeelerDistance");
        var sideRightFeelerDistVar = Array.Find(inputVariables, (v) => v.name == "SideRightFeelerDistance");
        
        var targetSpeedVar = Array.Find(outputVariables, (v) => v.name == "Speed");
        var targetRotationVar = Array.Find(outputVariables, (v) => v.name == "Rotation");
        var sideEnginesVar = Array.Find(outputVariables, (v) => v.name == "SideEngines");

        foreach (var dirToTarget in dirToTargetVar.possibleTerms)
        {
            foreach (var leftFeelerDistance in leftFeelerDistVar.possibleTerms)
            {
                foreach (var rightFeelerDistance in rightFeelerDistVar.possibleTerms)
                {
                    foreach (var middleFeelerDistance in middleFeelerDistVar.possibleTerms)
                    {
                        foreach (var sideLeftFeelerDistance in sideLeftFeelerDistVar.possibleTerms)
                        {
                            foreach (var sideRightFeelerDistance in sideRightFeelerDistVar.possibleTerms)
                            {
                                FuzzyStatement[] conditions = {
                                    new() { variable = dirToTargetVar, term = dirToTarget.name },
                                    new()
                                        { variable = leftFeelerDistVar, term = leftFeelerDistance.name },
                                    new()
                                        { variable = rightFeelerDistVar, term = rightFeelerDistance.name },
                                    new()
                                        { variable = middleFeelerDistVar, term = middleFeelerDistance.name },
                                    new()
                                        { variable = sideLeftFeelerDistVar, term = sideLeftFeelerDistance.name },
                                    new()
                                        { variable = sideRightFeelerDistVar, term = sideRightFeelerDistance.name }
                                };
                                List<FuzzyStatement> conclusions = new List<FuzzyStatement>();
                                if (leftFeelerDistance.name is "Far" or "Medium" &&
                                      rightFeelerDistance.name is "Far" or "Medium" &&
                                      middleFeelerDistance.name is "Far" or "Medium" &&
                                    sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                    sideRightFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name == "Straight")
                                    {
                                        conclusions.Add(new FuzzyStatement(){variable = targetRotationVar, 
                                            term = targetRotationVar.possibleTerms[1].name});
                                        if (middleFeelerDistance.name == "Far")
                                        {
                                            conclusions.Add(new FuzzyStatement()
                                            {
                                                variable = targetSpeedVar,
                                                term = targetSpeedVar.possibleTerms[6].name
                                            });
                                        }
                                        else
                                        {
                                            conclusions.Add(new FuzzyStatement()
                                            {
                                                variable = targetSpeedVar,
                                                term = targetSpeedVar.possibleTerms[4].name
                                            });
                                        }
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[2].name});
                                    }
                                    else if (dirToTarget.name is "Left" or "FewLeft")
                                    {
                                        conclusions.Add(new FuzzyStatement(){variable = targetRotationVar, 
                                            term = targetRotationVar.possibleTerms[2].name});
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[2].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[2].name});
                                    }
                                    else if(dirToTarget.name is "Right" or "FewRight")
                                    {
                                        conclusions.Add(new FuzzyStatement(){variable = targetRotationVar, 
                                            term = targetRotationVar.possibleTerms[0].name});
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[2].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[2].name});
                                    }
                                }
                                else if (leftFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name is "Straight" or "FewLeft" or "FewRight" or "Left")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[2].name
                                        });
                                    }
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[3].name});
                                }
                                else if (rightFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name is "Straight" or "FewLeft" or "FewRight" or "Right")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[2].name
                                        });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                    }
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[1].name});
                                }
                                else if (middleFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" &&
                                         leftFeelerDistance.name is "Far" or "Medium" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name is "Left" or "FewLeft" or "Straight" )
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                    }
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" &&
                                         leftFeelerDistance.name is "Far" or "Medium" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name is "Left" or "FewLeft" or "Straight" or "FewRight")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[1].name
                                        });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                    }
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[3].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" &&
                                         leftFeelerDistance.name is "Far" or "Medium" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium")
                                {
                                    if (dirToTarget.name is "Right" or "FewRight" or "Straight" or "FewLeft")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[1].name
                                        });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[2].name
                                        });
                                    }
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[1].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         sideLeftFeelerDistance.name is "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" &&
                                         leftFeelerDistance.name is "Far" or "Medium" 
                                         )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[1].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[5].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         leftFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[2].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[5].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[1].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[0].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[5].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[3].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         leftFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    if (dirToTarget.name is "Left" or "FewLeft" or "Straight")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[2].name
                                        });
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[2].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[2].name});
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[4].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[3].name});
                                    }
                                    
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    if (dirToTarget.name is "Right" or "FewRight" or "Straight")
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[2].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[2].name});
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[2].name
                                        });
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                            term = targetSpeedVar.possibleTerms[4].name});
                                        conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                            term = sideEnginesVar.possibleTerms[1].name});
                                    }
                                   
                                }
                                else if (rightFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    // if (dirToTarget.name is "Right" or "FewRight" or "Straight")
                                    // {
                                    //     conclusions.Add(new FuzzyStatement()
                                    //     {
                                    //         variable = targetRotationVar,
                                    //         term = targetRotationVar.possibleTerms[2].name
                                    //     });
                                    // }
                                    // else
                                    // {
                                    //     conclusions.Add(new FuzzyStatement()
                                    //     {
                                    //         variable = targetRotationVar,
                                    //         term = targetRotationVar.possibleTerms[0].name
                                    //     });
                                    // }
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[0].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[1].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         sideLeftFeelerDistance.name is "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         sideLeftFeelerDistance.name is "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         leftFeelerDistance.name is "Far" or "Medium" 
                                        )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[2].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideRightFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         sideLeftFeelerDistance.name is "Far" or "Medium" )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[2].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[2].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[1].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         sideRightFeelerDistance.name is "Far" or "Medium" )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[0].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[2].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[3].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         sideRightFeelerDistance.name is "Close" &&
                                         rightFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         leftFeelerDistance.name is "Far" or "Medium" )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[0].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         sideRightFeelerDistance.name is "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         rightFeelerDistance.name is "Far" or "Medium" )
                                {
                                    conclusions.Add(new FuzzyStatement()
                                    {
                                        variable = targetRotationVar,
                                        term = targetRotationVar.possibleTerms[2].name
                                    });
                                    conclusions.Add(new FuzzyStatement(){variable = targetSpeedVar, 
                                        term = targetSpeedVar.possibleTerms[3].name});
                                    conclusions.Add(new FuzzyStatement(){variable = sideEnginesVar, 
                                        term = sideEnginesVar.possibleTerms[2].name});
                                }
                                else if (sideLeftFeelerDistance.name == "Close" &&
                                         sideRightFeelerDistance.name is "Close" &&
                                         leftFeelerDistance.name is "Close" &&
                                         middleFeelerDistance.name is "Far" or "Medium" or "Close" &&
                                         rightFeelerDistance.name is "Close")
                                {
                                    if (dirToTarget.name is "Straight" or "FewLeft" or "FewRight")
                                    {
                                        var condsCopy1 = new List<FuzzyStatement>();
                                        var condsCopy2 = new List<FuzzyStatement>();
                                        var condsCopy3 = new List<FuzzyStatement>();
                                        condsCopy1.AddRange(conditions);
                                        condsCopy2.AddRange(conditions);
                                        condsCopy3.AddRange(conditions);

                                        condsCopy1.Add(new FuzzyStatement()
                                            { variable = distToTargetVar, term = distToTargetVar.possibleTerms[0].name });
                                        condsCopy2.Add(new FuzzyStatement()
                                            { variable = distToTargetVar, term = distToTargetVar.possibleTerms[1].name });
                                        condsCopy3.Add(new FuzzyStatement()
                                            { variable = distToTargetVar, term = distToTargetVar.possibleTerms[2].name });

                                        List<FuzzyStatement> concls1 = new List<FuzzyStatement>();
                                        concls1.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[1].name
                                        });
                                        concls1.Add(new FuzzyStatement()
                                        {
                                            variable = targetSpeedVar,
                                            term = targetSpeedVar.possibleTerms[4].name
                                        });
                                        concls1.Add(new FuzzyStatement()
                                        {
                                            variable = sideEnginesVar,
                                            term = sideEnginesVar.possibleTerms[2].name
                                        });

                                        List<FuzzyStatement> concls2 = new List<FuzzyStatement>();
                                        concls2.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        concls2.Add(new FuzzyStatement()
                                        {
                                            variable = targetSpeedVar,
                                            term = targetSpeedVar.possibleTerms[2].name
                                        });
                                        concls2.Add(new FuzzyStatement()
                                        {
                                            variable = sideEnginesVar,
                                            term = sideEnginesVar.possibleTerms[2].name
                                        });

                                        List<FuzzyStatement> concls3 = new List<FuzzyStatement>();
                                        concls3.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        concls3.Add(new FuzzyStatement()
                                        {
                                            variable = targetSpeedVar,
                                            term = targetSpeedVar.possibleTerms[2].name
                                        });
                                        concls3.Add(new FuzzyStatement()
                                        {
                                            variable = sideEnginesVar,
                                            term = sideEnginesVar.possibleTerms[2].name
                                        });

                                        newRules.Add(new FuzzyRuleInfo()
                                            { conditions = condsCopy1.ToArray(), conclusions = concls1.ToArray() });
                                        newRules.Add(new FuzzyRuleInfo()
                                            { conditions = condsCopy2.ToArray(), conclusions = concls2.ToArray() });
                                        newRules.Add(new FuzzyRuleInfo()
                                            { conditions = condsCopy3.ToArray(), conclusions = concls3.ToArray() });
                                    }
                                    else
                                    {
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetRotationVar,
                                            term = targetRotationVar.possibleTerms[0].name
                                        });
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = targetSpeedVar,
                                            term = targetSpeedVar.possibleTerms[2].name
                                        });
                                        conclusions.Add(new FuzzyStatement()
                                        {
                                            variable = sideEnginesVar,
                                            term = sideEnginesVar.possibleTerms[2].name
                                        });
                                    }
                                }

                                if (conclusions.Count>0)
                                 newRules.Add(new FuzzyRuleInfo(){conditions = conditions, conclusions = conclusions.ToArray()});
                            }
                        }
                    }
                }
            }
        }

        rules = newRules.ToArray();
    }

    [ContextMenu("Generation/ClearRules")]
    public void ClearRules()
    {
        rules = Array.Empty<FuzzyRuleInfo>();
    }
}

[Serializable]
public class FuzzyVariableInfo
{
    public string name;

    public FuzzyTerm[] possibleTerms = {};

    public FuzzyVariableInfo GetCopy()
    {
        var n = new FuzzyVariableInfo();
        n.name = name;
        var terms = new FuzzyTerm[possibleTerms.Length];
        for (int i = 0; i < possibleTerms.Length; i++)
        {
            terms[i] = possibleTerms[i].GetCopy();
        }

        n.possibleTerms = terms;

        return n;
    }
}
[Serializable]
public class FuzzyTerm
{
    public string name;
    public float lowerBound;
    public float upperBound;

    public FuzzyTerm GetCopy()
    {
        var n = new FuzzyTerm();
        n.name = name;
        n.lowerBound = lowerBound;
        n.upperBound = upperBound;

        return n;
    }
}

[Serializable]
public class FuzzyRuleInfo
{
    public FuzzyStatement[] conditions = {};
    public FuzzyStatement[] conclusions = {};
}
[Serializable]
public class FuzzyStatement
{
    public FuzzyVariableInfo variable;
    public string term;
}
#if UNITY_EDITOR
[CustomEditor(typeof(ExpertSystemData))]
public class ExpertSystemDataEditor : Editor
{
    private ExpertSystemData data;

    private List<List<ReorderableList>> conditionLists;
    private List<List<ReorderableList>> conclusionLists;

    private ReorderableList rulesList;
    private void OnEnable()
    {
        data = target as ExpertSystemData;
        conditionLists = new List<List<ReorderableList>>();
        conclusionLists = new List<List<ReorderableList>>();

        for (int i = 0; i < data.rules.Length; i++)
        {
            conditionLists.Add(new List<ReorderableList>());
            conclusionLists.Add(new List<ReorderableList>());
            ///Conditions
            SerializedProperty conditions = serializedObject.FindProperty("rules").GetArrayElementAtIndex(i)
                .FindPropertyRelative("conditions");
            
            var conditionList = new ReorderableList(serializedObject, conditions, true, true, true, true);

            conditionList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>

            {
                SerializedProperty conditionElement = conditions.GetArrayElementAtIndex(j);

                string[] variableNames = data.inputVariables.Select(x => x.name).ToArray();
                int variableIndex = Array.IndexOf(variableNames,
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue);

                rect.y += 1;
                variableIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    $"Condition {j + 1} Variable",
                    variableIndex,
                    variableNames);
                
                if (variableIndex >= 0)
                {
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue =
                        data.inputVariables[variableIndex].name;
                }
                else
                {
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue = null;
                }

                if (variableIndex >= 0)
                {
                    FuzzyVariableInfo variable = data.inputVariables[variableIndex];
                    string[] terms = variable.possibleTerms.Select(x=>x.name).ToArray();
                    int termIndex = Array.IndexOf(terms, conditionElement.FindPropertyRelative("term").stringValue);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    termIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        $"Condition {j + 1} Term",
                        termIndex,
                        terms);
                    if (termIndex >= 0 && termIndex < terms.Length)
                    {
                        conditionElement.FindPropertyRelative("term").stringValue = terms[termIndex];
                    }
                }
            };
            conditionList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Conditions"); };
            conditionList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                //element.stringValue = "New Term";
            };
            conditionList.elementHeightCallback = index =>
            {
                var element = conditions.GetArrayElementAtIndex(index);

                var height = EditorGUIUtility.singleLineHeight * 2;

                return height;
            };
            
            conditionLists[i].Add(conditionList);
            
            
            ///Conclusions
            SerializedProperty conclusions = serializedObject.FindProperty("rules").GetArrayElementAtIndex(i)
                .FindPropertyRelative("conclusions");
            
            var conclusionList = new ReorderableList(serializedObject, conclusions, true, true, true, true);

            conclusionList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>

            {
                SerializedProperty conclusionElement = conclusions.GetArrayElementAtIndex(j);

                string[] variableNames = data.outputVariables.Select(x => x.name).ToArray();
                int variableIndex = Array.IndexOf(variableNames,
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue);

                rect.y += 1;
                variableIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    $"conclusion {j + 1} Variable",
                    variableIndex,
                    variableNames);
                
                if (variableIndex >= 0)
                {
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue =
                        data.outputVariables[variableIndex].name;
                }
                else
                {
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue = null;
                }

                if (variableIndex >= 0)
                {
                    FuzzyVariableInfo variable = data.outputVariables[variableIndex];
                    string[] terms = variable.possibleTerms.Select(x=>x.name).ToArray();
                    int termIndex = Array.IndexOf(terms, conclusionElement.FindPropertyRelative("term").stringValue);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    termIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        $"conclusion {j + 1} Term",
                        termIndex,
                        terms);
                    if (termIndex >= 0 && termIndex < terms.Length)
                    {
                        conclusionElement.FindPropertyRelative("term").stringValue = terms[termIndex];
                    }
                }
            };
            conclusionList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "conclusions"); };
            conclusionList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                //element.stringValue = "New Term";
            };
            conclusionList.elementHeightCallback = index =>
            {
                var height = EditorGUIUtility.singleLineHeight * 2;

                return height;
            };
            
            conclusionLists[i].Add(conclusionList);
        }
        
        SerializedProperty rules = serializedObject.FindProperty("rules");

        rulesList = new ReorderableList(serializedObject, rules, true, true, true, true);

        rulesList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>
        {
            EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width,EditorGUIUtility.singleLineHeight),$"Rule: {j}");
            rect.y += EditorGUIUtility.singleLineHeight;
           
                for (int i = 0; i < conditionLists[j].Count; i++)
                {
                    conditionLists[j][i].DoList(new Rect(rect.x, rect.y, rect.width,
                        rect.height - EditorGUIUtility.singleLineHeight));
                    rect.y += conditionLists[j][i].GetHeight();
                    conclusionLists[j][i].DoList(new Rect(rect.x, rect.y, rect.width,
                        rect.height - EditorGUIUtility.singleLineHeight));
                }
            

        };
        
        rulesList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Rules"); };
        
        rulesList.elementHeightCallback = index =>
        {
            float height = EditorGUIUtility.singleLineHeight;
            
                for (int i = 0; i < conditionLists[index].Count; i++)
                {
                    height += conditionLists[index][i].GetHeight();
                    height += conclusionLists[index][i].GetHeight();
                }
            

            return height;
        };
    }
    
    
    
    

    public override void OnInspectorGUI()
    {
        DrawScriptField();
        
        serializedObject.Update();

        // Input variables
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputVariables"), true);
        EditorGUILayout.Space();
        //Output variables
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputVariables"), true);

        // Rules
        EditorGUILayout.Space();
        
        EditorGUI.BeginChangeCheck();
        //rulesList.DoLayoutList();
         Debug.Log(rulesList.count);
        // Add buttons to add new input/output variables and rules
        if (GUILayout.Button("Add Input Variable"))
        {
            Array.Resize(ref data.inputVariables, data.inputVariables.Length + 1);
            data.inputVariables[data.inputVariables.Length - 1] = new FuzzyVariableInfo();
        }

        if (GUILayout.Button("Add Output Variable"))
        {
            Array.Resize(ref data.outputVariables, data.outputVariables.Length + 1);
            data.outputVariables[data.outputVariables.Length - 1] = new FuzzyVariableInfo();
        }

        if (GUILayout.Button("Add Rule"))
        {
            Array.Resize(ref data.rules, data.rules.Length + 1);
            data.rules[data.rules.Length - 1] = new FuzzyRuleInfo();
            OnEnable();
        }

        serializedObject.ApplyModifiedProperties();

        foreach (var rule in data.rules)
        {
            foreach (var cond in rule.conditions)
            {
                cond.variable =
                    data.inputVariables[
                        Array.FindIndex(data.inputVariables,(v)=>v.name==cond.variable.name)
                    ].GetCopy();
            }

            foreach (var conclusion in rule.conclusions)
            {
                conclusion.variable =
                    data.outputVariables[
                        Array.FindIndex(data.outputVariables,(v)=>v.name==conclusion.variable.name)
                    ].GetCopy();
            }
        }
    }
    
    private void DrawScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ExpertSystemData)target), typeof(ExpertSystemData), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
    }
}
#endif
