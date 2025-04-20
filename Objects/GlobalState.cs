using Alkitab.Models;

namespace Alkitab.Objects;

public static class GlobalState
{
    public static ToggleState ToggleState { get; } = new();
    public static BibleInstances BibleInstances { get; set; } = new();
}