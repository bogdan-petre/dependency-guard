using System;

namespace DependencyGuard
{
    /// <summary>
    /// Ignore classes with this attribute in dependency injection validation.
    /// Every class marked with the attribute will be ignored.
    /// </summary>
    public class IgnoreGuardAttribute : Attribute
    {
    }
}
