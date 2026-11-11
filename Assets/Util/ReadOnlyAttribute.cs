using System;
using UnityEngine;

/// <summary>
/// Displays a serialized field as read-only in the Inspector.
/// Private fields also require SerializeField. Runtime code can still change the value.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ReadOnlyAttribute : PropertyAttribute
{
}
