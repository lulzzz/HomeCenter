﻿using HomeCenter.Model.Native;
using System.IO;

namespace HomeCenter.Services.Configuration
{
    public class ResourceLocatorService : IResourceLocatorService
    {
        private const string AdapterRepositoryName = "Adapters";
        private const string ConfigurationName = "HomeCenterConfiguration.json";
        private readonly IStorage _nativeStorage;

        public ResourceLocatorService(IStorage nativeStorage)
        {
            _nativeStorage = nativeStorage;
        }

        public string GetRepositoyLocation()
        {
            return Path.Combine(_nativeStorage.LocalFolderPath(), AdapterRepositoryName);
        }

        public string GetConfigurationPath()
        {
            return Path.Combine(_nativeStorage.LocalFolderPath(), ConfigurationName);
        }
    }
}