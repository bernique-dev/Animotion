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

    public override void UpdatePropertyValue(string propertyName, object value, bool originalCall = true) {
        base.UpdatePropertyValue(propertyName, value, originalCall);
        if (originalCall) {
            _animotor.UpdatePropertyValue(propertyName, value, false);
        }
    }

}