using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using UnityEngine;

namespace Animotion {
    [Serializable]
    public class TreePropertyCondition {

        public int id;
        public static int idCounter;

        public static Func<int, int, bool> IsIntGreater = (x, y) => x > y;
        public static Func<int, int, bool> IsIntGreaterOrEqual = (x, y) => x >= y;
        public static Func<int, int, bool> IsIntLower = (x, y) => x < y;
        public static Func<int, int, bool> IsIntLowerOrEqual = (x, y) => x <= y;
        public static Func<int, int, bool> IsIntEqual = (x, y) => x == y;
        public static Func<int, int, bool> IsIntNotEqual = (x, y) => x == y;

        public static Func<float, float, bool> IsFloatGreater = (x, y) => x > y;
        public static Func<float, float, bool> IsFloatGreaterOrEqual = (x, y) => x >= y;
        public static Func<float, float, bool> IsFloatLower = (x, y) => x < y;
        public static Func<float, float, bool> IsFloatLowerOrEqual = (x, y) => x <= y;
        public static Func<float, float, bool> IsFloatEqual = (x, y) => x == y;
        public static Func<float, float, bool> IsFloatNotEqual = (x, y) => x == y;

        public static Func<bool, bool> IsBoolTrue = (b) => b is true;
        public static Func<bool, bool> IsBoolFalse = (b) => b is false;

        [SerializeReference] [SerializeField] [HideInInspector] public TreeProperty property;
        [SerializeReference] [SerializeField] [HideInInspector] public TreeData tree;

        public int conditionIndex;

        public Func<bool, bool> boolCondition;
        public Func<int, int, bool> intCondition;
        public int intValue;
        public Func<float, float, bool> floatCondition;
        public float floatValue;

        public TreePropertyCondition treePropertyCondition;

        public void SetValues(TreeProperty _property) {
            property = _property;
            if (id < 0) id = idCounter;
            idCounter++;
        }

        public void Process() {
            switch (property.type) {
                case TreePropertyType.Boolean:
                    boolCondition = GetBoolConditionMethods()[conditionIndex].oneParameterFunction;
                    break;
                case TreePropertyType.Trigger:
                    break;
                case TreePropertyType.Integer:
                    intCondition = GetIntConditionMethods()[conditionIndex].twoParametersFunction;
                    break;
                case TreePropertyType.Float:
                    floatCondition = GetFloatConditionMethods()[conditionIndex].twoParametersFunction;
                    break;
            }
        }


        public static List<ConditionAndName<int>> GetIntConditionMethods() {
            List<ConditionAndName<int>> results = new List<ConditionAndName<int>>() {
                new ConditionAndName<int>("is greater than",IsIntGreater), new ConditionAndName<int>("is greater or equal to",IsIntGreaterOrEqual),
                new ConditionAndName<int>("is equal to",IsIntEqual), new ConditionAndName<int>("is lower than",IsIntLower),
                new ConditionAndName<int>("is lower or equal to",IsIntLowerOrEqual), new ConditionAndName<int>("is not equal to",IsIntNotEqual)
            };
            return results;
        }
        public static ConditionAndName<int> GetIntConditionMethod(string name) {
            return GetIntConditionMethods().FirstOrDefault(can => can.name == name);
        }
        public static List<ConditionAndName<float>> GetFloatConditionMethods() {
            List<ConditionAndName<float>> results = new List<ConditionAndName<float>>() {
                new ConditionAndName<float>("is greater than",IsFloatGreater), new ConditionAndName<float>("is greater or equal to",IsFloatGreaterOrEqual),
                new ConditionAndName<float>("is equal to",IsFloatEqual), new ConditionAndName<float>("is lower than",IsFloatLower),
                new ConditionAndName<float>("is lower or equal to",IsFloatLowerOrEqual), new ConditionAndName<float>("is not equal to",IsFloatNotEqual)
            };
            return results;
        }
        public static ConditionAndName<float> GetFloatConditionMethod(string name) {
            return GetFloatConditionMethods().FirstOrDefault(can => can.name == name);
        }

        public static List<ConditionAndName<bool>> GetBoolConditionMethods() {
            List<ConditionAndName<bool>> results = new List<ConditionAndName<bool>>() {
                new ConditionAndName<bool>("is true", IsBoolTrue), new ConditionAndName<bool>("is false", IsBoolFalse)
            };
            return results;
        }
        public static ConditionAndName<bool> GetBoolConditionMethod(string name) {
            Debug.Log("funcName = " + name);
            return GetBoolConditionMethods().FirstOrDefault(can => can.name == name);
        }

    }

    public class ConditionAndName<T> {
        public string name;
        public Func<T, bool> oneParameterFunction { get; private set; }
        public Func<T, T, bool> twoParametersFunction { get; private set; }

        public ConditionAndName(string _name, Func<T, bool> _function) {
            name = _name;
            oneParameterFunction = _function;
        }

        public ConditionAndName(string _name, Func<T, T, bool> _function) {
            name = _name;
            twoParametersFunction = _function;
        }


        public bool Apply(T parameter) {
            return oneParameterFunction(parameter);
        }
        public bool Apply(T parameter1, T parameter2) {
            return twoParametersFunction(parameter1, parameter2);
        }

    }

}