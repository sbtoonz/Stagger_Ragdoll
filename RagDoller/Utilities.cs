using System;
using System.Collections.Generic;
using System.Linq;
using RagDoller.MonoScripts;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        internal static RuntimeAnimatorController MakeAoc(RuntimeAnimatorController original, Dictionary<string, string> replacements , Dictionary<string, AnimationClip> externalAnimations)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(original);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var animation in aoc.animationClips)
            {
                string name = animation.name;
                if (replacements.TryGetValue(name, out var replacement))
                {
                    AnimationClip newClip = Object.Instantiate<AnimationClip>(externalAnimations[replacement]);
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

        internal static BoxCollider? BoxRagDollHelper(Vector3 center, Vector3 size,GameObject obj)
        {
            if(!obj) return null;
            var box = obj.AddComponent<BoxCollider>();
            box.center = center;
            box.size = size;
            return box;
        }
        internal static CapsuleCollider? CylRagDollHelper(Vector3 center, float rad, float height, Direction direction,GameObject obj)
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
            return cap;
        }

        internal static SphereCollider? SphereRagDollHelper(Vector3 center, float rad,GameObject obj)
        {
            if (!obj) return null;
            var sphere = obj.AddComponent<SphereCollider>();
            sphere.center = center;
            sphere.radius = rad;
            return sphere;
        }

        internal static Rigidbody? RbAdder(GameObject obj)
        {
            if (!obj) return null;
            var r = obj.AddComponent<Rigidbody>();
            r.collisionDetectionMode = CollisionDetectionMode.Continuous;
            r.constraints = RigidbodyConstraints.FreezeRotation;
            return r;
        }

        internal static void Jointbuilder(JointInputStruct input, GameObject obj)
        {
            if (!obj) return;
            var j = obj.AddComponent<CharacterJoint>();
            j.connectedBody = input.ConnectedBody;
            j.anchor = input.Anchor;
            j.axis = input.Axis;
            j.autoConfigureConnectedAnchor = true;
            j.connectedAnchor = input.ConnectedAnchor;
            j.swingAxis = input.SwingAxis;
	        
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
            j.projectionDistance = input.ProjectionDistance;
            j.projectionAngle = input.ProjectionAngle;
            j.enableCollision = input.EnableCollision;
            j.enablePreprocessing = input.EnablePreProcessing;
            j.massScale = input.MassScale;
            j.connectedMassScale = input.ConnectedMassScale;
        }

        internal struct JointInputStruct
        {
            internal Rigidbody? ConnectedBody = null;
            internal Vector3 Anchor= Vector3.zero;
            internal Vector3 Axis = Vector3.zero;
            internal Vector3 ConnectedAnchor = Vector3.zero;
            internal Vector3 SwingAxis = Vector3.zero;
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
            internal float ProjectionDistance =0;
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
        
        internal static void ExecuteRbBuild(Player player, List<Rigidbody> ragdollRBs, List<Collider> ragdollColliders)
        {
            var hips = player.gameObject.transform.Find("Visual/Armature/Hips");
            var t = hips.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(BoxRagDollHelper(new Vector3(0, 0.00285406f, -0.0002805286f),
                new Vector3(0.004570569f, 0.005708119f, 0.003304224f), hips!.gameObject)!);
            var hipRb = RbAdder(hips.gameObject);
            hipRb!.mass = 3.125f;
            hipRb.angularDrag = 0.05f;
            ragdollRBs.Add(hipRb);
            
            
            var leftupleg = player.gameObject.transform.Find("Visual/Armature/Hips/LeftUpLeg");
            leftupleg.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.002369338f ,0), 0.001421603f, 0.004738676f, Direction.Yaxis, leftupleg.gameObject)!);
            var leftuplegRb = RbAdder(leftupleg.gameObject);
            leftuplegRb!.mass = 1.875f;
            leftuplegRb.angularDrag = 0.05f;
            ragdollRBs.Add(leftuplegRb);
            var leftUpLegJoint = new JointInputStruct
            {
                ConnectedBody = hipRb,
                Axis =  new Vector3(-1,0,0),
                ConnectedAnchor = new Vector3(-0.001028195f, 0.001144695f, -5.299342e-05f),
                SwingAxis = new Vector3(0,0,1),
                LowTwistLimit = -20,
                HighTwistLimit = 70,
                Swing1Limit = 30,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(leftUpLegJoint, leftupleg.gameObject);
            
            
            var leftleg= player.gameObject.transform.Find("Visual/Armature/Hips/LeftUpLeg/LeftLeg");
            leftleg.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0, 0.00292089f, 0), 0.001460445f, 0.00584178f,
                Direction.Yaxis, leftleg.gameObject)!);
            var leftlegRb = RbAdder(leftleg.gameObject);
            leftlegRb!.mass = 1.875f;
            leftuplegRb.angularDrag = 0.05f;
            ragdollRBs.Add(leftlegRb);
            var leftLegJoint = new JointInputStruct
            {
                ConnectedBody =  leftuplegRb,
                Axis = new Vector3(-1,0,0),
                ConnectedAnchor = new Vector3(-1.455186e-11f, 0.004738676f, -4.947322e-10f),
                SwingAxis = new Vector3(0,0,1),
                LowTwistLimit = -80,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(leftLegJoint, leftleg.gameObject);


            var rightupleg = player.gameObject.transform.Find("Visual/Armature/Hips/RightUpLeg");
            rightupleg.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.002369338f,0),0.001421603f,0.004738676f, Direction.Yaxis, rightupleg.gameObject )!);
            var rightuplegRB = RbAdder(rightupleg.gameObject);
            rightuplegRB!.mass = 1.875f;
            rightuplegRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightuplegRB);
            var rightupJoint = new JointInputStruct
            {
                ConnectedBody = hipRb,
                Axis = new Vector3(-1,0,0),
                ConnectedAnchor = new Vector3(0.001028195f, 0.001144695f, -5.299342e-05f),
                SwingAxis =  new Vector3(0,0,1),
                LowTwistLimit = -20,
                HighTwistLimit = 70,
                Swing1Limit = 30,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(rightupJoint, rightupleg.gameObject);

            var rightleg= player.gameObject.transform.Find("Visual/Armature/Hips/RightUpLeg/RightLeg");
            rightleg.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.002369338f,0),0.001421603f,0.004738676f, Direction.Yaxis, rightleg.gameObject )!);
            var rightlegRB = RbAdder(rightleg.gameObject);
            rightlegRB!.mass = 1.875f;
            rightlegRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightlegRB);
            var rightJoint = new JointInputStruct
            {
                ConnectedBody = rightuplegRB,
                Axis = new Vector3(-1,0,0),
                ConnectedAnchor = new Vector3(1.455186e-11f, 0.004738676f, -4.947322e-10f),
                SwingAxis =  new Vector3(0,0,1),
                LowTwistLimit = -80,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(rightJoint, rightleg.gameObject);
            
            
            var spine2= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2");
            spine2.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(BoxRagDollHelper(new Vector3(0, 0.0004566551f, -5.985866e-05f), new Vector3(0.004570569f, 0.0009132955f,0.002743611f), spine2.gameObject)!);
            var spine2RB = RbAdder(spine2.gameObject);
            spine2RB!.mass = 1.875f;
            spine2RB.angularDrag = 0.05f;
            ragdollRBs.Add(spine2RB);
            var spine2Joint = new JointInputStruct
            {
                ConnectedBody = hipRb,
                Axis = new Vector3(1,0,0),
                ConnectedAnchor = new Vector3(0, 0.005708119f, -0.0003850954f),
                SwingAxis = new Vector3(0,0,1),
                LowTwistLimit = 20,
                Swing1Limit = 10,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(spine2Joint, spine2.gameObject);
            
            var head = player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/Neck/Head");
            head.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(SphereRagDollHelper(new Vector3(-0.0002801421f, 0.001438531f, 2.874114e-05f), 0.001428619f, head.gameObject)!);
            var headRB = RbAdder(head.gameObject);
            headRB!.mass = 1.875f;
            headRB.angularDrag = 0.05f;
            ragdollRBs.Add(headRB);
            var headJoint = new JointInputStruct
            {
                ConnectedBody = spine2RB,
                Axis = new Vector3(0,0,1),
                ConnectedAnchor = new Vector3(-4.301378e-05f,0.00224686f,0.0005117025f),
                SwingAxis = new Vector3(-1,0,0),
                LowTwistLimit = -40,
                HighTwistLimit = 25,
                Swing1Limit = 25,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(headJoint, head.gameObject);

            var leftarm= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm");
            leftarm.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.001673469f,0),0.0008367344f, 0.003346938f, Direction.Yaxis, leftarm.gameObject)!);
            var leftarmRB = RbAdder(leftarm.gameObject);
            leftarmRB!.mass = 1.875f;
            leftarmRB.angularDrag = 0.05f;
            ragdollRBs.Add(leftarmRB);
            var leftarmJoint = new JointInputStruct
            {
                ConnectedBody = spine2RB,
                Axis = new Vector3(0,-1,0),
                ConnectedAnchor = new Vector3(-0.002285284f,0.000913303f, -8.628116e-05f),
                SwingAxis = new Vector3(1,0,0),
                LowTwistLimit = -70,
                HighTwistLimit = 10,
                Swing1Limit = 50,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(leftarmJoint, leftarm.gameObject);
            
            
            var leftforearm= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm");
            leftforearm.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.002613377f,0), 0.001045351f,0.005226755f, Direction.Yaxis, leftforearm.gameObject)!);
            var leftforearmRB = RbAdder(leftforearm.gameObject);
            leftforearmRB!.mass = 1.875f;
            leftforearmRB.angularDrag = 0.05f;
            ragdollRBs.Add(leftforearmRB);
            var leftforearmJoint = new JointInputStruct
            {
                ConnectedBody = leftarmRB,
                Axis = new Vector3(1,0,0),
                ConnectedAnchor = new Vector3(-5.238689e-10f, 0.003346938f, -3.026798e-09f),
                SwingAxis = new Vector3(0,-1,0),
                LowTwistLimit = -90,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1  
            };
            Jointbuilder(leftforearmJoint, leftforearm.gameObject);

            var rightarm= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm");
            rightarm.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.001673469f,0), 0.0008367344f, 0.003346938f, Direction.Yaxis, rightarm.gameObject)!);
            var rightarmRB = RbAdder(rightarm.gameObject);
            rightarmRB!.mass = 1.875f;
            rightarmRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightarmRB);
            var rightarmjoint = new JointInputStruct
            {
                ConnectedBody = spine2RB,
                Axis = new Vector3(0,-1,0),
                ConnectedAnchor = new Vector3(0.002285284f, 0.000913303f, -8.628116e-05f),
                SwingAxis = new Vector3(-1,0,0),
                LowTwistLimit = -70,
                HighTwistLimit = 10,
                Swing1Limit = 50,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(rightarmjoint, rightarm.gameObject);
            
            
            var rightforearm= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm");
           rightforearm.gameObject.AddComponent<CollisionRedirector>();
            ragdollColliders.Add(CylRagDollHelper(new Vector3(0,0.002613377f,0), 0.001045351f, 0.005226755f, Direction.Yaxis, rightforearm.gameObject)!);
            var rightforearmRB = RbAdder(rightforearm.gameObject);
            rightforearmRB!.mass = 1.875f;
            rightforearmRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightforearmRB);
            var rightforearmJoint = new JointInputStruct
            {
                ConnectedBody = rightarmRB,
                Axis = new Vector3(-1,0,0),
                ConnectedAnchor = new Vector3(5.238689e-10f, 0.003346938f, -3.026798e-09f),
                SwingAxis = new Vector3(0,-1,0),
                LowTwistLimit = -90,
                EnableProjection = true,
                ProjectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Jointbuilder(rightforearmJoint, rightforearm.gameObject);
        }

        internal enum Direction
        {
            Xaxis,
            Yaxis,
            Zaxis,
        }
    }
}