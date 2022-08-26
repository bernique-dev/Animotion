using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreePropertyType {
    Boolean, Trigger, Integer, Float
}

public static class TreePropertyTypeExtensions {
    
    public static int GetConditionFieldsNumber(this TreePropertyType type) {
        int number = 1;
        switch (type) {
            case TreePropertyType.Boolean:
                number = 2;
                break;
            case TreePropertyType.Trigger:
                number = 1;
                break;
            case TreePropertyType.Integer:
                number = 3;
                break;
            case TreePropertyType.Float:
                number = 3;
                break;
        }
        return number;
    }


}