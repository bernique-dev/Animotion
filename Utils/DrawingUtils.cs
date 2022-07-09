using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawingUtils {
    
    public static Vector3[] GetArrowSummitPoints(Vector2 start, Vector2 end, float arrowHeight, float arrowWidth) {
        Vector2 link = end - start;
        Vector2 arrowBaseCenter = end - link.normalized * arrowHeight;
        Vector2 arrowBisector = arrowBaseCenter - end;

        Vector3[] arrowPoints = new Vector3[] {
                    end,
                    end + arrowBisector + Vector2.Perpendicular(arrowBisector).normalized * arrowWidth,
                    end + arrowBisector - Vector2.Perpendicular(arrowBisector).normalized * arrowWidth,
                    end
                };
        return arrowPoints;
    }

    public static void FindClosestSegment(Vector2[] startPoints, Vector2[] endPoints, ref Vector2 start, ref Vector2 end) {

        foreach (Vector2 startSideCenter in startPoints) {
            foreach (Vector2 endSideCenter in endPoints) {
                if (Vector2.Distance(startSideCenter, endSideCenter) < Vector2.Distance(start, end)) {
                    start = startSideCenter;
                    end = endSideCenter;
                }
            }
        }
    }

}
