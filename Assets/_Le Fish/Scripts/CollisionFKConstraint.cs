using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class CollisionFKConstraint : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float weight = 1.0f;
    [SerializeField] Transform root, tip;
    [SerializeField] LayerMask mask;
    [SerializeField] float angleStepSize = 1f;

    [SerializeField] private BoneData[] _bones;

    [Serializable]
    private class BoneData
    {
        [SerializeField] public Transform Bone, MaxRotation;
        [SerializeField] public Quaternion StartRotation;
        [SerializeField] public Vector3 ColliderDirection, ColliderOffset;
        [SerializeField] public float MaxAngle;
        [SerializeField] public CapsuleCollider CapsuleCollider;
        [SerializeField] public float CapsuleRadius, CapsuleHeight;
    }

    private void OnValidate()
    {
        if (Application.isPlaying || root == null || tip == null) 
            return;

        Transform bone = root;
        Transform maxRotation = transform;
        SetNextMaxRotation();

        void SetNextMaxRotation()
        {
            try
            {
                maxRotation = maxRotation.GetChild(0);
            }
            catch
            {
                Transform parent = maxRotation;
                maxRotation = new GameObject($"{bone.name} Max Rotation").transform;
                maxRotation.SetPositionAndRotation(bone.position, bone.rotation);
                maxRotation.SetParent(parent);
            }
        }

        List<BoneData> bones = new();
        while (bone != tip)
        {
            CapsuleCollider capsuleCollider = bone.GetComponentsInChildren<CapsuleCollider>().Last();
            if (capsuleCollider == null)
            {
                Debug.LogWarning($"Couldn't find a collider on {bone.name}");
                bone = bone.GetChild(0);
                SetNextMaxRotation();
                continue;
            }

            bones.Add(new BoneData
            {
                Bone = bone,
                MaxRotation = maxRotation,
                CapsuleCollider = capsuleCollider,
                CapsuleRadius = capsuleCollider.radius,
                CapsuleHeight = capsuleCollider.height - capsuleCollider.radius * 2,
            });
            bone = bone.GetChild(0);
            SetNextMaxRotation();
        }
        _bones = bones.ToArray();

        if (TryGetComponent<BoneRenderer>(out var renderer))
        {
            renderer.transforms = _bones
                .Select(data => data.MaxRotation)
                .Append(_bones.Last().MaxRotation.GetChild(0))
                .ToArray();
        }
    }

    private void Start()
    {
        foreach (BoneData data in _bones)
        {
            data.StartRotation = data.Bone.rotation;
            data.MaxAngle = Quaternion.Angle(data.StartRotation, data.MaxRotation.rotation);
            data.ColliderDirection = data.Bone.InverseTransformDirection(data.CapsuleCollider.direction switch
            {
                0 => data.CapsuleCollider.transform.right,
                1 => data.CapsuleCollider.transform.up,
                2 => data.CapsuleCollider.transform.forward,
                _ => throw new NotImplementedException(),
            });
            data.ColliderOffset = data.Bone.InverseTransformDirection(data.CapsuleCollider.transform.position - data.Bone.position);
        }
    }

    private void Update()
    {
        foreach(BoneData data in _bones)
        {
            MoveBone(data);
        }
    }

    private void MoveBone(BoneData boneData)
    {
        float angle = 0;

        while (angle < boneData.MaxAngle)
        {
            if (TryRotation(angle))
                return;

            angle += angleStepSize;
        }

        TryRotation(boneData.MaxAngle);

        bool TryRotation(float angle)
        {
            Quaternion rotation = Quaternion.RotateTowards(boneData.StartRotation, boneData.MaxRotation.rotation, angle);
            Vector3 direction = rotation * boneData.ColliderDirection;
            Vector3 center = boneData.MaxRotation.position + rotation * boneData.ColliderOffset;

            Debug.DrawRay(center, boneData.CapsuleHeight * direction, Color.red);
            Debug.DrawLine(boneData.Bone.position, center, Color.cyan);

            if (Physics.CheckCapsule(center + boneData.CapsuleHeight * direction, center + boneData.CapsuleHeight * -direction, boneData.CapsuleRadius, mask))
            {
                boneData.Bone.rotation = Quaternion.Lerp(boneData.StartRotation, rotation, weight);
                return true;
            }
            return false;
        }
    }
}
