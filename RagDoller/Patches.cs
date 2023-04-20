using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
namespace RagDoller
{
    public class Patches
    {
        internal static List<Rigidbody> ragdollRBs = new List<Rigidbody>();
        internal static List<Collider> ragdollColliders = new List<Collider>();
        
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        public static class InitialRagdollPatch
        {
            public static void Prefix(Player __instance)
            {
               
            }
        }

        public static void ExecuteRBBuild(Player player)
        {
            var hips = player.gameObject.transform.Find("Visual/Armature/Hips");
            ragdollColliders.Add(Utilities.BoxRagDollHelper(new Vector3(0, 0.00285406f, -0.0002805286f),
                new Vector3(0.004570569f, 0.005708119f, 0.003304224f), hips!.gameObject)!);
            var hipRb = Utilities.RbAdder(hips.gameObject);
            hipRb!.mass = 3.125f;
            hipRb.angularDrag = 0.05f;
            ragdollRBs.Add(hipRb);
            
            
            var leftupleg = player.gameObject.transform.Find("Visual/Armature/Hips/LeftUpLeg");
            ragdollColliders.Add(Utilities.CylRagDollHelper(new Vector3(0,0.002369338f ,0), 0.001421603f, 0.004738676f, Utilities.Direction.Yaxis, leftupleg.gameObject)!);
            var leftuplegRb = Utilities.RbAdder(leftupleg.gameObject);
            leftuplegRb!.mass = 1.875f;
            leftuplegRb.angularDrag = 0.05f;
            ragdollRBs.Add(leftuplegRb);
            var leftUpLegJoint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(leftUpLegJoint, leftupleg.gameObject);
            
            
            var leftleg= player.gameObject.transform.Find("Visual/Armature/Hips/LeftUpLeg/LeftLeg");
            ragdollColliders.Add(Utilities.CylRagDollHelper(new Vector3(0, 0.00292089f, 0), 0.001460445f, 0.00584178f,
                Utilities.Direction.Yaxis, leftleg.gameObject)!);
            var leftlegRb = Utilities.RbAdder(leftleg.gameObject);
            leftlegRb!.mass = 1.875f;
            leftuplegRb.angularDrag = 0.05f;
            ragdollRBs.Add(leftlegRb);
            var leftLegJoint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(leftLegJoint, leftleg.gameObject);


            var rightupleg = player.gameObject.transform.Find("Visual/Armature/Hips/RightUpLeg");
            ragdollColliders.Add(Utilities.CylRagDollHelper(new Vector3(0,0.002369338f,0),0.001421603f,0.004738676f, Utilities.Direction.Yaxis, rightupleg.gameObject )!);
            var rightuplegRB = Utilities.RbAdder(rightupleg.gameObject);
            rightuplegRB!.mass = 1.875f;
            rightuplegRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightuplegRB);
            var rightupJoint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(rightupJoint, rightupleg.gameObject);

            var rightleg= player.gameObject.transform.Find("Visual/Armature/Hips/RightUpLeg/RightLeg");
            ragdollColliders.Add(Utilities.CylRagDollHelper(new Vector3(0,0.002369338f,0),0.001421603f,0.004738676f, Utilities.Direction.Yaxis, rightleg.gameObject )!);
            var rightlegRB = Utilities.RbAdder(rightleg.gameObject);
            rightlegRB!.mass = 1.875f;
            rightlegRB.angularDrag = 0.05f;
            ragdollRBs.Add(rightlegRB);
            var rightJoint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(rightJoint, rightleg.gameObject);
            
            
            var spine2= player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2");
            ragdollColliders.Add(Utilities.BoxRagDollHelper(new Vector3(0, 0.0004566551f, -5.985866e-05f), new Vector3(0.004570569f, 0.0009132955f,0.002743611f), spine2.gameObject)!);
            var spine2RB = Utilities.RbAdder(spine2.gameObject);
            spine2RB!.mass = 1.875f;
            spine2RB.angularDrag = 0.05f;
            ragdollRBs.Add(spine2RB);
            var spine2Joint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(spine2Joint, spine2.gameObject);
            
            var head = player.gameObject.transform.Find("Visual/Armature/Hips/Spine/Spine1/Spine2/Neck/Head");
            ragdollColliders.Add(Utilities.SphereRagDollHelper(new Vector3(-0.0002801421f, 0.001438531f, 2.874114e-05f), 0.001428619f, head.gameObject)!);
            var headRB = Utilities.RbAdder(head.gameObject);
            headRB!.mass = 1.875f;
            headRB.angularDrag = 0.05f;
            ragdollRBs.Add(headRB);
            var headJoint = new Utilities.JointInputStruct
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
            Utilities.Jointbuilder(headJoint, head.gameObject);

            /*var head;
            var leftarm;
            var leftforearm;
            var rightarm;
            var rightforearm;*/
        }
    }
}