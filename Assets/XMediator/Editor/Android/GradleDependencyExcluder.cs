using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace XMediator.Editor.Android
{
    /// <summary>
    /// Handles the process of modifying Gradle dependencies to include exclusions.
    /// </summary>
    public class GradleDependencyExcluder
    {
        private readonly string _gradleFilePath;

        /// <summary>
        /// Creates a new instance of GradleDependencyExcluder.
        /// </summary>
        /// <param name="gradleFilePath">Path to the Gradle file to modify.</param>
        public GradleDependencyExcluder(string gradleFilePath)
        {
            _gradleFilePath = gradleFilePath;
        }

        /// <summary>
        /// Applies a list of dependency exclusions to the Gradle file.
        /// </summary>
        /// <param name="excludeDependencies">The list of dependencies with exclusions to apply.</param>
        /// <returns>True if any modifications were made, false otherwise.</returns>
        public bool ApplyExclusions(List<ExcludeDependency> excludeDependencies)
        {
            Debug.Log($"[XMediator] Processing {excludeDependencies.Count} exclude dependencies");

            if (excludeDependencies.Count == 0)
            {
                Debug.Log("[XMediator] No exclusions to process");
                return false;
            }

            var gradleLines = File.ReadAllLines(_gradleFilePath);
            var modified = false;

            for (var i = 0; i < gradleLines.Length; i++)
            {
                var currentLine = gradleLines[i];
                if (!currentLine.TrimStart().StartsWith("implementation "))
                    continue;

                foreach (var excludeDep in excludeDependencies)
                {
                    if (!IsValidExcludeDependency(excludeDep))
                        continue;

                    var dependencyString = $"{excludeDep.group}:{excludeDep.module}:{excludeDep.version}";
                    if (!currentLine.Contains(dependencyString))
                        continue;

                    Debug.Log($"[XMediator] Found implementation for {dependencyString} at line {i}");

                    // Clean up any previously added exclude blocks
                    gradleLines = RemoveExcludeBlocks(gradleLines, i);

                    // Get exclusion clauses for this dependency
                    var excludeClauses = BuildExcludeClauses(excludeDep);
                    if (string.IsNullOrEmpty(excludeClauses))
                        continue;

                    // Apply excludes to the current implementation line
                    if (TryApplyExcludes(ref gradleLines[i], excludeClauses))
                    {
                        Debug.Log($"[XMediator] Added excludes to line {i}: {gradleLines[i]}");
                        modified = true;
                    }
                }
            }

            if (modified)
            {
                File.WriteAllLines(_gradleFilePath, gradleLines);
                Debug.Log("[XMediator] Gradle file updated with dependency exclusions");
            }
            else
            {
                Debug.Log("[XMediator] No changes made to Gradle file");
            }

            return modified;
        }

        /// <summary>
        /// Builds the exclude clauses string for a dependency.
        /// </summary>
        private string BuildExcludeClauses(ExcludeDependency dependency)
        {
            return string.Join(" ", dependency.excludes.ConvertAll(ex => $"exclude group: '{ex.group}', module: '{ex.module}'"));
        }

        /// <summary>
        /// Checks if an ExcludeDependency has valid exclusions.
        /// </summary>
        private bool IsValidExcludeDependency(ExcludeDependency dependency)
        {
            return dependency?.excludes != null && dependency.excludes.Count > 0;
        }

        /// <summary>
        /// Removes any previously added exclude blocks that follow the implementation line.
        /// </summary>
        private string[] RemoveExcludeBlocks(string[] lines, int implementationLineIndex)
        {
            if (implementationLineIndex + 1 >= lines.Length || !lines[implementationLineIndex + 1].TrimStart().StartsWith("{"))
                return lines;

            var j = implementationLineIndex + 1;
            while (j < lines.Length && !lines[j].Contains("}"))
                j++;

            if (j < lines.Length)
                return lines.Take(implementationLineIndex + 1).Concat(lines.Skip(j + 1)).ToArray();

            return lines;
        }

        /// <summary>
        /// Applies exclude clauses to an implementation line, preserving any comments.
        /// </summary>
        /// <returns>True if changes were made, false otherwise.</returns>
        private bool TryApplyExcludes(ref string line, string excludeClauses)
        {
            // Split line into code and comment parts
            var commentIndex = line.IndexOf("//");
            var codePart = commentIndex >= 0 ? line.Substring(0, commentIndex).TrimEnd() : line.TrimEnd();
            var commentPart = commentIndex >= 0 ? " " + line.Substring(commentIndex) : string.Empty;

            // Check if excludes are already present
            if (codePart.Contains("exclude group:"))
                return false;

            // Apply excludes before the comment
            line = codePart + " " + excludeClauses + commentPart;
            return true;
        }
    }
}