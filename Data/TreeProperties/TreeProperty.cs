using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TreeProperty : ScriptableObject {

    public int id;
    public static int idCounter;
    public new string name;
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
                    result = m_intValue;
                    break;
                case TreePropertyType.Float:
                    result = m_floatValue;
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
                    m_intValue = (int)value;
                    break;
                case TreePropertyType.Float:
                    m_floatValue = (float)value;
                    break;
            }
        }
    }

    [SerializeField] /*[HideInInspector]*/ private bool m_booleanValue;
    [SerializeField] /*[HideInInspector]*/ private int m_intValue;
    [SerializeField] /*[HideInInspector]*/ private float m_floatValue;

    public void SetValues(string _name, TreePropertyType _type) {
        id = idCounter;
        idCounter++;
        name = _name;
        type = _type;
    }

    public void SetValues(TreeProperty treeProperty) {
        SetValues(treeProperty.name, treeProperty.type);
        value = treeProperty.value;
    }

}