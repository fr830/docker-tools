// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.ImageBuilder.Model;
using Microsoft.DotNet.ImageBuilder.ViewModel;
using Newtonsoft.Json;

namespace Microsoft.DotNet.ImageBuilder.Commands
{
    public abstract class Command<TOptions> : ICommand
        where TOptions : Options, new()
    {
        public ManifestInfo Manifest { get; private set; }
        public TOptions Options { get; private set; }

        Options ICommand.Options
        {
            get { return Options; }
        }

        public Command()
        {
            Options = new TOptions();
        }

        public abstract Task ExecuteAsync();

        public void LoadManifest()
        {
            Logger.WriteHeading("READING MANIFEST");

            string manifestJson = File.ReadAllText(Options.Manifest);
            Manifest manifestModel = JsonConvert.DeserializeObject<Manifest>(manifestJson);
            manifestModel.Validate();

            Manifest = ManifestInfo.Create(
                manifestModel,
                Options.GetManifestFilter(),
                Options);

            if (Options.IsVerbose)
            {
                Logger.WriteMessage(JsonConvert.SerializeObject(Manifest, Formatting.Indented));
            }
        }
    }
}
