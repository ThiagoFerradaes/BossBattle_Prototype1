using UnityEngine;

public static class Extensions {

    /// <summary>
    /// Checks if a given layer is present in this LayerMask.
    /// </summary>
    /// <param name="layerMask">The given LayerMask.</param>
    /// <param name="layer">The given Layer.</param>
    /// <returns>True if layer is present.</returns>
    public static bool ContainsLayer(this LayerMask layerMask, int layer) {
        return (layerMask.value & (1 << layer)) > 0;
    }

}
