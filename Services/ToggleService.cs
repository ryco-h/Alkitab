using Alkitab.Objects;

namespace Alkitab.Services;

public class ToggleService
{
    public ToggleState ToggleState { get; } = new ToggleState();

    private static ToggleService? _instance;
    public static ToggleService Instance => _instance ??= new ToggleService();

    private ToggleService() {}
}