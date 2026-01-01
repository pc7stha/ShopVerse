using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Common;

public static class DeveloperCertificateHelper
{
    public static void EnsureDeveloperCertificate(IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        if (string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "dev-certs https --trust",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string stdOut = process.StandardOutput.ReadToEnd();
        string stdErr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Failed to ensure developer certificate. ExitCode: {process.ExitCode}. StdOut: {stdOut} StdErr: {stdErr}");
        }
    }
}
