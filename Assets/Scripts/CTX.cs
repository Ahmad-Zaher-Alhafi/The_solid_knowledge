using Controllers;
public static class Ctx {
    public static IControllers Deps { get; private set; }

    public static void SetContext(IControllers deps) {
        Deps = deps;
    }
}