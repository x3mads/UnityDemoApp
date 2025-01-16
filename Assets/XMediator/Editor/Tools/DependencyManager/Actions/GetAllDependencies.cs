using System;
using System.Collections.Generic;
using XMediator.Editor.Tools.DependencyManager.Entities;
using XMediator.Editor.Tools.DependencyManager.Repository;
using XMediator.Editor.Tools.DependencyManager.Repository.Dto;
using XMediator.Editor.Tools.Settings;

namespace XMediator.Editor.Tools.DependencyManager.Actions
{
    internal class GetAllDependencies
    {
        private readonly DependenciesRepository _dependenciesRepository;
        private readonly InstalledDependenciesRepository _installedDependenciesRepository;
        private readonly XMediatorSettingsRepository _settingsRepository;

        public GetAllDependencies(DependenciesRepository dependenciesRepository, 
            InstalledDependenciesRepository installedDependenciesRepository, 
            XMediatorSettingsRepository settingsRepository)
        {
            _dependenciesRepository = dependenciesRepository;
            _installedDependenciesRepository = installedDependenciesRepository;
            _settingsRepository = settingsRepository;
        }

        // TODO-DM deberia devolver un nuevo modelo, que incluya las disponibles y las instaladas.
        public void Invoke(Action<AvailableDependencies> onSuccess, Action<Exception> onError)
        {
            try
            {
                var publisher = _settingsRepository.GetSettingValue("publisher", "");
                var clientDependencies = ClientDependencies();
                _dependenciesRepository.FindDependencies(publisher, clientDependencies, onSuccess, onError);
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        }

        private List<ClientDependencyDto> ClientDependencies()
        {
            var clientDependencies = new List<ClientDependencyDto>();
            var sdkDependency = _installedDependenciesRepository.GetXMediatorDependency();
            var adaptersDependencies = _installedDependenciesRepository.GetAdaptersDependencies();
            var installedSdkDependency = new ClientDependencyDto("XMediator", sdkDependency.Version.ToString());
            clientDependencies.Add(installedSdkDependency);
            foreach (var adaptersDependency in adaptersDependencies)
            {
                var clientDependencyDto =
                    new ClientDependencyDto(adaptersDependency.Name, adaptersDependency.Version.ToString());
                clientDependencies.Add(clientDependencyDto);
            }

            return clientDependencies;
        }
    }
}