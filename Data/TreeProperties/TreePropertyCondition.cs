using System;
using System.Collections.Generic;
using UnityEngine;

public class TreePropertyCondition {

    public static Func<int, int, bool> IsIntGreater = (x, y) => x > y;
    public static Func<int, int, bool> IsIntGreaterOrEqual = (x, y) => x >= y;

    public TreeProperty property;

    public TreePropertyCondition(TreeProperty _property) {
        property = _property;
    }

}
