using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Explosion.Components;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Projectiles.Reflect;
public sealed partial class RMCReflectSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RMCReflectiveComponent, ProjectileReflectAttemptEvent>(OnReflectAttempt, before: [typeof(SharedTriggerSystem)]);
    }

    private void OnReflectAttempt(Entity<RMCReflectiveComponent> reflector, ref ProjectileReflectAttemptEvent args)
    {
        if (!_random.Prob(reflector.Comp.ReflectChance.Float()))
            return;

        if ((TryComp<ProjectileIFFComponent>(args.ProjUid, out var iff) && iff.Enabled) || !TryComp<PhysicsComponent>(args.ProjUid, out var physics))
            return;

        var projectile = args.ProjUid;

        var rotation = _random.NextAngle(-reflector.Comp.Spread / 2, reflector.Comp.Spread / 2).Opposite();
        var existingVelocity = _physics.GetMapLinearVelocity(projectile, component: physics);
        var relativeVelocity = existingVelocity - _physics.GetMapLinearVelocity(reflector);
        var newVelocity = rotation.RotateVec(relativeVelocity);

        // Have the velocity in world terms above so need to convert it back to local.
        var difference = newVelocity - existingVelocity;

        _physics.SetLinearVelocity(projectile, physics.LinearVelocity + difference, body: physics);

        var locRot = Transform(projectile).LocalRotation;
        var newRot = rotation.RotateVec(locRot.ToVec());
        _transform.SetLocalRotation(projectile, newRot.ToAngle());

        if (TryComp<ProjectileComponent>(projectile, out var comp))
        {
            _adminLogger.Add(LogType.BulletHit, LogImpact.Medium, $"{ToPrettyString(reflector)} reflected {ToPrettyString(projectile)} from {ToPrettyString(comp.Weapon)} shot by {comp.Shooter}");

            comp.Shooter = reflector;
            comp.Weapon = reflector;
            comp.Damage *= reflector.Comp.DamageMult;
            comp.IgnoreShooter = true;
            Dirty(projectile, comp);

            if (TryComp<ProjectileFixedDistanceComponent>(projectile, out var fixedDist) && comp.Shooter != null &&
                comp.Weapon != null && TryComp<GunComponent>(comp.Weapon, out var gun) &&
    Transform(reflector).Coordinates.TryDistance(EntityManager, comp.Shooter.Value.ToCoordinates(), out var distance))
            {
                fixedDist.FlyEndTime = _timing.CurTime + TimeSpan.FromSeconds(distance / gun.ProjectileSpeedModified);
                Dirty(projectile, fixedDist);
        }
        }
        else
        {
            _adminLogger.Add(LogType.BulletHit, LogImpact.Medium, $"{ToPrettyString(reflector)} reflected {ToPrettyString(projectile)}");
        }

        if (TryComp<RMCProjectileAccuracyComponent>(projectile, out var accuracy))
        {
            accuracy.Accuracy = reflector.Comp.Accuracy;
            Dirty(projectile, accuracy);
        }

        args.Cancelled = true;

    }
}
