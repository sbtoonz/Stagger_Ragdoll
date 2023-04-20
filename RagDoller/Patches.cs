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
            var hipRb = Utilities.RBAdder(hips.gameObject);
            hipRb!.mass = 3.125f;
            hipRb.angularDrag = 0.05f;
            ragdollRBs.Add(hipRb);
            
            
            var leftupleg = player.gameObject.transform.Find("Visual/Armature/Hips/LeftUpLeg");;
            ragdollColliders.Add(Utilities.CylRagDollHelper(new Vector3(0,0.002369338f ,0), 0.001421603f, 0.004738676f, Utilities.Direction.Yaxis, leftupleg.gameObject)!);
            var leftuplegRb = Utilities.RBAdder(leftupleg.gameObject);
            leftuplegRb!.mass = 1.875f;
            leftuplegRb.angularDrag = 0.05f;
            ragdollRBs.Add(leftuplegRb);
            Utilities.JointInputStruct leftUpLegJoint = new Utilities.JointInputStruct
            {
                connectedBody = hipRb,
                axis =  new Vector3(-1,0,0),
                connectedAnchor = new Vector3(-0.001028195f, 0.001144695f, -5.299342e-05f),
                swingAxis = new Vector3(0,0,1),
                LowTwistLimit = -20,
                HighTwistLimit = 70,
                Swing1Limit = 30,
                projectionDistance = 0.1f,
                ProjectionAngle = 180,
                BreakForce = float.MaxValue,
                BreakTorque = float.MaxValue,
                MassScale = 1,
                ConnectedMassScale = 1
            };
            Utilities.jointbuilder(leftUpLegJoint, leftupleg.gameObject);
            /*var leftleg;
            var rigghtupleg;
            var rightleg;
            var spine2;
            var head;
            var leftarm;
            var leftforearm;
            var rightarm;
            var rightforearm;*/
        }
    }
}