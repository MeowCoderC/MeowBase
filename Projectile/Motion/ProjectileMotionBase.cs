using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ProjectileShooterBase is an abstract base class that encapsulates common functionality 
/// for calculating and visualizing the trajectory of a projectile.
/// 
/// This class includes:
///   - Calculation of trajectory parameters based on the launch origin, target point, launch angle, and force.
///   - Visualization of the trajectory using a LineRenderer (if enabled) and OnDrawGizmos for Scene view debugging.
///   - An abstract method LaunchProjectile() which must be overridden by derived classes to implement specific launching behaviors.
/// 
/// For example:
///   - A tank shooting a bullet can inherit from this class and override LaunchProjectile() to instantiate a bullet prefab,
///     then apply the calculated force to it.
///   - A throwable item that throws itself can override LaunchProjectile() to apply force to its own Rigidbody.
/// </summary>
public abstract class ProjectileMotionBase : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] protected Transform launchOrigin = null;   // Starting point of the projectile.
    [SerializeField] protected Transform targetPoint = null;    // Target point used for trajectory calculation.

    [Header("Launch Configuration")]
    [SerializeField] protected int launchAngleDegrees = 45;       // Launch angle (in degrees).
    [SerializeField] protected int trajectoryPointCount = 100;      // Number of points used to plot the trajectory.
    [SerializeField] protected float timeStep = 0.1f;               // Time interval between trajectory points.
    [SerializeField] protected float forceMultiplier = 50f;         // Force multiplier applied to the projectile.

    [Header("Trajectory Visualization")]
    [SerializeField] protected bool showTrajectoryLineRenderer = true;      // Toggle to display the trajectory using a LineRenderer.
    [SerializeField] protected LineRenderer trajectoryLineRenderer = null;  // Optional LineRenderer for trajectory visualization.

    protected const float Gravity = 10f;    // Gravity constant used for calculations.

    protected float initialVelocity;        // Calculated initial velocity.
    protected float launchAngleRad;         // Launch angle in radians.
    protected float horizontalAngleRad;     // Horizontal angle in radians.

    /// <summary>
    /// Calculates the trajectory parameters based on the launchOrigin and targetPoint.
    /// </summary>
    protected virtual void CalculateTrajectory()
    {
        if (launchOrigin == null || targetPoint == null)
            return;

        Vector3 direction = targetPoint.position - launchOrigin.position;
        float verticalDisplacement = direction.y;
        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        horizontalAngleRad = Mathf.Atan2(direction.z, direction.x);
        launchAngleRad = Mathf.Abs(launchAngleDegrees) * Mathf.Deg2Rad;
        timeStep = Mathf.Abs(timeStep);

        // Calculate initial velocity squared using the projectile motion formula:
        // v^2 = (g * x^2) / (2 * cos^2(theta) * (x * tan(theta) - y))
        float denominator = (Mathf.Tan(launchAngleRad) * horizontalDistance - verticalDisplacement) / (horizontalDistance * horizontalDistance);
        float vSquared = (Gravity / denominator) / (2f * Mathf.Pow(Mathf.Cos(launchAngleRad), 2f));
        initialVelocity = Mathf.Sqrt(Mathf.Abs(vSquared));
    }

    /// <summary>
    /// Renders the projectile trajectory using the LineRenderer, if enabled.
    /// </summary>
    protected virtual void RenderTrajectory()
    {
        if (!showTrajectoryLineRenderer || trajectoryLineRenderer == null)
            return;

        List<Vector3> trajectoryPoints = new List<Vector3>();

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * timeStep;
            float x = initialVelocity * Mathf.Cos(launchAngleRad) * Mathf.Cos(horizontalAngleRad) * t;
            float z = initialVelocity * Mathf.Cos(launchAngleRad) * Mathf.Sin(horizontalAngleRad) * t;
            float y = initialVelocity * Mathf.Sin(launchAngleRad) * t - 0.5f * Gravity * t * t;
            trajectoryPoints.Add(launchOrigin.position + new Vector3(x, y, z));
        }

        trajectoryLineRenderer.positionCount = trajectoryPoints.Count;
        trajectoryLineRenderer.SetPositions(trajectoryPoints.ToArray());
    }

    /// <summary>
    /// Updates the trajectory calculations and visualization in LateUpdate.
    /// Use LateUpdate if the object is being moved by the player to ensure the trajectory remains up-to-date.
    /// </summary>
    protected virtual void LateUpdate()
    {
        CalculateTrajectory();
        RenderTrajectory();
    }

    /// <summary>
    /// Draws the projectile trajectory as Gizmos in the Scene view for debugging and visualization.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (launchOrigin == null || targetPoint == null)
            return;

        List<Vector3> gizmoPoints = new List<Vector3>();
        Vector3 direction = targetPoint.position - launchOrigin.position;
        float verticalDisplacement = direction.y;
        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        horizontalAngleRad = Mathf.Atan2(direction.z, direction.x);
        launchAngleRad = Mathf.Abs(launchAngleDegrees) * Mathf.Deg2Rad;

        float denominator = (Mathf.Tan(launchAngleRad) * horizontalDistance - verticalDisplacement) / (horizontalDistance * horizontalDistance);
        float vSquared = (Gravity / denominator) / (2f * Mathf.Pow(Mathf.Cos(launchAngleRad), 2f));
        float calcInitialVelocity = Mathf.Sqrt(Mathf.Abs(vSquared));

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * timeStep;
            float x = calcInitialVelocity * Mathf.Cos(launchAngleRad) * Mathf.Cos(horizontalAngleRad) * t;
            float z = calcInitialVelocity * Mathf.Cos(launchAngleRad) * Mathf.Sin(horizontalAngleRad) * t;
            float y = calcInitialVelocity * Mathf.Sin(launchAngleRad) * t - 0.5f * Gravity * t * t;
            gizmoPoints.Add(launchOrigin.position + new Vector3(x, y, z));
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < gizmoPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(gizmoPoints[i], gizmoPoints[i + 1]);
        }
    }

    /// <summary>
    /// Abstract method to launch the projectile.
    /// Derived classes must override this method to define their specific launching behavior.
    /// For example:
    ///   - A tank shooting a bullet should instantiate a bullet prefab, set its initial position, 
    ///     and apply the calculated force based on the trajectory.
    ///   - A throwable item that throws itself can simply apply force to its own Rigidbody.
    /// </summary>
    public abstract void LaunchProjectile();
    // LaunchProjectile by ridibody
    // Vector3 forceVector = new Vector3(
    //     initialVelocity * forceMultiplier * Mathf.Cos(launchAngleRad) * Mathf.Cos(horizontalAngleRad),
    //     initialVelocity * forceMultiplier * Mathf.Sin(launchAngleRad),
    //     initialVelocity * forceMultiplier * Mathf.Cos(launchAngleRad) * Mathf.Sin(horizontalAngleRad)
    // );
    //
    // rb.AddForce(forceVector);
}
