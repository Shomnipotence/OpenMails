using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace OpenMails
{

    [Generator]
    public class AppSecretsGenerator : ISourceGenerator
    {
        const string ClassName = "AppSecrets";

        Dictionary<string, string>? _secrets = null;

        private string GetSecretValue(string secretName)
        {
            return string.Empty;
        }

        private string EscapeString(string originString)
        {
            StringBuilder sb = new StringBuilder(originString);
            sb.Replace("'", "\'");
            sb.Replace("\"", "\\\"");
            sb.Replace("\\", "\\\\");
            sb.Replace("\r", "\\r");
            sb.Replace("\n", "\\n");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(GeneratorExecutionContext context)
        {
            var path = $"{ClassName}.ini";
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir))
                path = System.IO.Path.Combine(projectDir, path);

            if (!File.Exists(path))
                throw new Exception($"'{path}' is not exist, current directory: {Environment.CurrentDirectory}");

            var lines = File.ReadAllLines(path);
            var separator = new char[] { '=' };

            _secrets = new();
            foreach (var line in lines)
            {
                var segments = line.Split(separator, 2);

                if (segments.Length != 2)
                    continue;

                var name = segments[0];
                var value = segments[1];

                if (value.Length >= 2)
                {
                    if (value[0] is '"' &&
                        value[value.Length - 1] is '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                }

                _secrets[name] = value;
            }

            StringBuilder sb = new();

            sb.AppendLine($"namespace {nameof(OpenMails)};");

            sb.AppendLine($"public partial class {ClassName}");
            sb.AppendLine(@"{");

            if (_secrets != null)
            {
                foreach (var secret in _secrets)
                {
                    string secretName = secret.Key;
                    string? secretValue = secret.Value;

                    sb.AppendLine($"    /// <summary>");
                    sb.AppendLine($"    /// {secretName}: {secretValue}");
                    sb.AppendLine($"    /// </summary>");
                    sb.AppendLine($"    public static string {secretName} {{ get; }} = \"{EscapeString(secretValue)}\";");
                }
            }

            sb.AppendLine(@"}");

            context.AddSource($"{ClassName}.g.cs", sb.ToString());
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }
}
