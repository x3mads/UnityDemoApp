using System;
using System.Linq;
using XMediator.Editor.Tools.DependencyManager.Entities;

namespace XMediator.Editor.Tools.DependencyManager.Repository.Dto
{
    [Serializable]
    internal class GetDependenciesResponseDto
    {
        public DependencyResponseDto[] dependencies;
        private static string sdkName = "XMediator";

        public AvailableVersions ToModel()
        {
            // TODO-DM Falta agregar el resto de las versiones al modelo. Mostrar todas las versiones que se obtienen del adapter en la UI.
            var availableVersions = dependencies.Select(dto => dto.ToModel()).ToList();
            return new AvailableVersions(
                sdk: availableVersions.FirstOrDefault(version => version.Name == sdkName),
                adapters: availableVersions.Where(version => version.Name != sdkName)
            );
        }

        public AvailableDependencies ToDependenciesModel()
        {
            var availableDependencies = dependencies.Select(dto => dto.ToDependencyModel()).ToList();
            return new AvailableDependencies(
                sdk: availableDependencies.FirstOrDefault(dependency => dependency.Name == sdkName),
                adapters: availableDependencies.Where(dependency => dependency.Name != sdkName));
        }
    }
}