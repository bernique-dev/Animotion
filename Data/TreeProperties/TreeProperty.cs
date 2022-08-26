using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TreeProperty {

    public string name;
    public TreePropertyType type;

    public object value {
        get {
            object result = null;
            switch (type) {
                case TreePropertyType.Boolean:
                    result = m_booleanValue;
                    break;
                case TreePropertyType.Trigger:
                    result = m_booleanValue;
                    break;
                case TreePropertyType.Integer:
                    break;
                case TreePropertyType.Float:
                    break;
            }
            return result;
        }
        set {
            switch (type) {
                case TreePropertyType.Boolean:
                    m_booleanValue = (bool)value;
                    break;
                case TreePropertyType.Trigger:
                    m_booleanValue = (bool)value;
                    break;
                case TreePropertyType.Integer:
                    break;
                case TreePropertyType.Float:
                    break;
            }
        }
    }

    [SerializeField] /*[HideInInspector]*/ private bool m_booleanValue;

    public TreeProperty(string _name, TreePropertyType _type) {
        name = _name;
        type = _type;
    }

}