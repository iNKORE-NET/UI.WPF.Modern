// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using iNKORE.UI.WPF.Modern.Gallery.Models;

namespace iNKORE.UI.WPF.Modern.Gallery.Helpers;

internal class IconsDataSource
{
    public static IconsDataSource Instance { get; } = new();

    public static List<IconData> Icons => Instance.icons;

    private List<IconData> icons = new();

    private IconsDataSource() { }

    public object _lock = new();

    public async Task<List<IconData>> LoadIcons()
    {
        lock (_lock)
        {
            if (icons.Count != 0)
            {
                return icons;
            }
        }

        // Load from embedded resource
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var resourceName = "iNKORE.UI.WPF.Modern.Gallery.DataModel.IconsData.json";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var jsonText = await reader.ReadToEndAsync();
                    lock (_lock)
                    {
                        if (icons.Count == 0)
                        {
                            icons = JsonSerializer.Deserialize<List<IconData>>(jsonText);
                        }
                        return icons;
                    }
                }
            }
        }

        // Fallback: try to load from file
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataModel", "IconsData.json");
        if (File.Exists(filePath))
        {
            var jsonText = await File.ReadAllTextAsync(filePath);
            lock (_lock)
            {
                if (icons.Count == 0)
                {
                    icons = JsonSerializer.Deserialize<List<IconData>>(jsonText);
                }
                return icons;
            }
        }

        return icons;
    }
}
