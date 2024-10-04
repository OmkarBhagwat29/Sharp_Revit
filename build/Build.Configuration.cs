sealed partial class Build
{
    const string Version = "0.0.0";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
            "Installer*"
        ];

        Bundles =
        [
            Solution.Sharp_Revit
        ];

        InstallersMap = new()
        {
            { Solution.Installer, Solution.Sharp_Revit }
        };
    }
}