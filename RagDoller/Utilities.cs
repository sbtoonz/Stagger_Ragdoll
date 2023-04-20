using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RagDoller
{
    public static class Utilities
    {
	    internal enum ConnectionState
	    {
		    Server,
		    Client,
		    Local,
		    Unknown
	    }
	    internal static ConnectionState GetConnectionState()
	    {
			if (ZNet.instance == null) return ConnectionState.Local;
		    if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated()) //server
		    {
			    return ConnectionState.Server;
		    }
		    
		    if (ZNet.m_isServer && ZNet.m_openServer) // Local server
		    {
			    return ConnectionState.Server;
		    }
		    if (!ZNet.instance.IsServer() && !ZNet.instance.IsDedicated()) //client
		    {
			    return ConnectionState.Client;
		    }

		    if (ZNet.IsSinglePlayer) 
		    {
			    return ConnectionState.Local;
		    }

		    return ConnectionState.Unknown;
	    }
        internal static AssetBundle? LoadAssetBundle(string bundleName)
        {
            var resource = typeof(RagDollerMod).Assembly.GetManifestResourceNames().Single
                (s => s.EndsWith(bundleName));
            using var stream = typeof(RagDollerMod).Assembly.GetManifestResourceStream(resource);
            return AssetBundle.LoadFromStream(stream);
        }

        internal static void LoadAssets(AssetBundle? bundle, ZNetScene zNetScene)
        {
            var tmp = bundle?.LoadAllAssets();
            if (zNetScene.m_prefabs.Count <= 0) return;
            if (tmp == null) return;
            foreach (var o in tmp)
            {
                var obj = (GameObject)o;
                zNetScene.m_prefabs.Add(obj);
                var hashcode = obj.GetHashCode();
                zNetScene.m_namedPrefabs.Add(hashcode, obj);
            }
        }
        
        internal static RuntimeAnimatorController MakeAoc(RuntimeAnimatorController ORIGINAL, Dictionary<string, string> replacements , Dictionary<string, AnimationClip> ExternalAnimations)
        {
	        AnimatorOverrideController aoc = new AnimatorOverrideController(ORIGINAL);
	        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
	        foreach (var animation in aoc.animationClips)
	        {
		        string name = animation.name;
		        if (replacements.TryGetValue(name, out var replacement))
		        {
			        AnimationClip newClip = MonoBehaviour.Instantiate<AnimationClip>(ExternalAnimations[replacement]);
			        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, newClip));
		        }
		        else
		        {
			        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, animation));
		        }
	        }
	        aoc.ApplyOverrides(anims);
	        return aoc;
        }

        internal static BoxCollider? BoxRagDollHelper(Vector3 center, Vector3 size,GameObject obj, bool shouldAddRB = false)
        {
	        if(!obj) return null;
	        var box = obj.AddComponent<BoxCollider>();
	        box.center = center;
	        box.size = size;
	        if (!shouldAddRB) return box;
	        var rb = obj.AddComponent<Rigidbody>();
	        rb.constraints = RigidbodyConstraints.FreezeRotation;
	        return box;
        }
        internal static CapsuleCollider? CylRagDollHelper(Vector3 center, float rad, float height, Direction direction,GameObject obj, bool shouldAddRB = false)
        {
	        if (!obj) return null;
	        var cap = obj.AddComponent<CapsuleCollider>();
	        cap.center = center;
	        cap.radius = rad;
	        cap.height = height;
	        switch (direction)
	        {
		        case Direction.Xaxis:
			        cap.direction = 0;
			        break;
		        case Direction.Yaxis:
			        cap.direction = 1;
			        break;
		        case Direction.Zaxis:
			        cap.direction = 2;
			        break;
	        }
	        if (!shouldAddRB) return cap;
	        var rb = obj.AddComponent<Rigidbody>();
	        rb.constraints = RigidbodyConstraints.FreezeRotation;
	        return cap;
        }

        internal static SphereCollider? SphereRagDollHelper(Vector3 center, float rad,GameObject obj, bool shouldAddRB = false)
        {
	        if (!obj) return null;
	        var sphere = obj.AddComponent<SphereCollider>();
	        sphere.center = center;
	        sphere.radius = rad;
	        if (!shouldAddRB) return sphere;
	        var rb = obj.AddComponent<Rigidbody>();
	        rb.constraints = RigidbodyConstraints.FreezeRotation;
	        return sphere;
        }

        internal static Rigidbody? RBAdder(GameObject obj)
        {
	        if (!obj) return null;
	        var r = obj.AddComponent<Rigidbody>();
	        r.collisionDetectionMode = CollisionDetectionMode.Continuous;
	        r.constraints = RigidbodyConstraints.FreezeRotation;
	        return r;
        }

        internal static void jointbuilder(JointInputStruct input, GameObject obj)
        {
	        if (!obj) return;
	        var j = obj.AddComponent<CharacterJoint>();
	        j.connectedBody = input.connectedBody;
	        j.anchor = input.anchor;
	        j.axis = input.axis;
	        j.autoConfigureConnectedAnchor = true;
	        j.connectedAnchor = input.connectedAnchor;
	        j.swingAxis = input.swingAxis;
	        
	        var twistLimitSpring = j.twistLimitSpring;
	        twistLimitSpring.spring = input.TwistLimitSpring;
	        twistLimitSpring.damper = input.TwistLimitDamper;
	        j.twistLimitSpring = twistLimitSpring;

	        var jLowTwistLimit = j.lowTwistLimit;
	        jLowTwistLimit.limit = input.LowTwistLimit;
	        jLowTwistLimit.bounciness = input.LowTwistBounciness;
	        jLowTwistLimit.contactDistance = input.LowTwistContactDistance;
	        j.lowTwistLimit = jLowTwistLimit;

	        var jHighTwistLimit = j.highTwistLimit;
	        jHighTwistLimit.limit = input.HighTwistLimit;
	        jHighTwistLimit.bounciness = input.HighTwistBounciness;
	        jHighTwistLimit.contactDistance = input.HighTwistContactDistance;
	        j.highTwistLimit = jHighTwistLimit;

	        var jSwingLimitSpring = j.swingLimitSpring;
	        jSwingLimitSpring.spring = input.SwingLimitSpring;
	        jSwingLimitSpring.damper = input.SwingLimitDamper;
	        j.swingLimitSpring = jSwingLimitSpring;

	        var jSwing1Limit = j.swing1Limit;
	        jSwing1Limit.limit = input.Swing1Limit;
	        jSwing1Limit.bounciness = input.Swing1Bounciness;
	        jSwing1Limit.contactDistance = input.Swing1ContactDistance;
	        j.swing1Limit = jSwing1Limit;

	        var jSwing2Limit = j.swing2Limit;
	        jSwing2Limit.limit = input.Swing2Limit;
	        jSwing2Limit.bounciness = input.Swing2Bounciness;
	        jSwing2Limit.contactDistance = input.Swing2ContactDistance;
	        j.swing2Limit = jSwing2Limit;

	        j.enableProjection = input.EnableProjection;
	        j.projectionDistance = input.projectionDistance;
	        j.projectionAngle = input.ProjectionAngle;
	        j.enableCollision = input.EnableCollision;
	        j.enablePreprocessing = input.EnablePreProcessing;
	        j.massScale = input.MassScale;
	        j.connectedMassScale = input.ConnectedMassScale;


        }

        internal struct JointInputStruct
        {
	        internal Rigidbody connectedBody = null;
	        internal Vector3 anchor= Vector3.zero;
	        internal Vector3 axis = Vector3.zero;
	        internal Vector3 connectedAnchor = Vector3.zero;
	        internal Vector3 swingAxis = Vector3.zero;
	        internal float TwistLimitSpring =0;
	        internal float TwistLimitDamper =0;
	        internal float LowTwistLimit =0;
	        internal float LowTwistBounciness =0;
	        internal float LowTwistContactDistance =0;
	        internal float HighTwistLimit =0;
	        internal float HighTwistBounciness =0;
	        internal float HighTwistContactDistance =0;
	        internal float SwingLimitSpring =0;
	        internal float SwingLimitDamper =0;
	        internal float Swing1Limit =0;
	        internal float Swing1Bounciness =0;
	        internal float Swing1ContactDistance =0;
	        internal float Swing2Limit =0;
	        internal float Swing2Bounciness =0;
	        internal float Swing2ContactDistance =0;
	        internal bool EnableProjection =false;
	        internal float projectionDistance =0;
	        internal int ProjectionAngle =0;
	        internal float BreakForce =0;
	        internal float BreakTorque =0;
	        internal bool EnableCollision =false;
	        internal bool EnablePreProcessing = false;
	        internal float MassScale =0;
	        internal float ConnectedMassScale =0;

	        public JointInputStruct()
	        {
	        }
        }

        internal enum Direction
        {
	        Xaxis,
	        Yaxis,
	        Zaxis,
        }
    }
}