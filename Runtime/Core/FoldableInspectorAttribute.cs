using System;

namespace GameKit.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FoldableInspectorAttribute : Attribute
    {
        public bool HideFieldHeaders { get; }
        public string DisplayName { get; }

        public FoldableInspectorAttribute(bool hideFieldHeaders = false, string displayName = null)
        {
            HideFieldHeaders = hideFieldHeaders;
            DisplayName = displayName;
        }
    }
}
