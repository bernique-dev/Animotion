using System.Collections;
using UnityEngine;
using Animotion;
using System.Collections.Generic;

public class LinkedAnimotor : Animotor {

    [SerializeField] private Animotor _animotor;

    public void SetAnimotor(Animotor animotor) {
        if (animotor != _animotor) {
            if (_animotor != null) {
                _animotor.UnlinkAnimotor(this);
            }
            if (animotor != null) {
                animotor.LinkAnimotor(this);
            }
            _animotor = animotor;
        }
    }

    public override Animotor GetAnimotor() {
        return _animotor;
    }

    public override void UpdatePropertyValue(string propertyName, object value, bool callChildren = true, bool callParent = true) {
        base.UpdatePropertyValue(propertyName, value, callChildren, callParent);
        if (callParent) {
            if (_animotor.HasProperty(propertyName)) {
                _animotor.UpdatePropertyValue(propertyName, value, false, true);
            }
        }
    }
}