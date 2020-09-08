using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using Statiq.Web;

namespace StatiqWeb
{
    public static class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Bootstrapper
                .Factory
                .CreateDefault(args)
                .AddThemeFromUri(new Uri("https://github.com/statiqdev/CleanBlog/archive/f732ab7f482b55e8d72bfa8ba4e84bdec9ae6b85.zip"))
                .AddWeb()
                .ConfigureSettings(settings =>
                {
                    settings["GitHubBranch"] = "main";
                })
                .DeployToGitHubPages(
                    "devlead",
                    "devlead.github.io",
                    Config.FromSetting<string>("GITHUB_TOKEN")
                )
        .RunAsync();

        public static Bootstrapper AddThemeFromUri(this Bootstrapper bootstrapper, Uri uri)
        {
            bootstrapper
                .ConfigureEngine(
                    engine =>
                    {

                        var path = engine.FileSystem.RootPath.Combine("theme").Combine(Path.GetFileNameWithoutExtension(uri.LocalPath));

                        var directory = engine.FileSystem.GetDirectory(path);

                        if (!directory.Exists)
                        {
                            using var clientHandler = new HttpClientHandler();
                            using var client = engine.CreateHttpClient(clientHandler);
                            using var responseStream = client
                                .GetStreamAsync(uri)
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();

                            using var zipStream =
                                new System.IO.Compression.ZipArchive(responseStream,
                                    System.IO.Compression.ZipArchiveMode.Read);

                            foreach (var entry in zipStream.Entries.Where(entry =>
                                !string.IsNullOrWhiteSpace(entry.Name)))
                            {
                                var parentPath = Path.GetDirectoryName(entry.FullName);
                                var fileDir = string.IsNullOrWhiteSpace(parentPath)
                                    ? directory
                                    : directory.GetDirectory(parentPath);

                                var file = fileDir.GetFile(entry.Name);
                                using var entryStream = entry.Open();
                                using var fileStream = file.Open(true);
                                entryStream.CopyTo(fileStream);
                            }
                        }

                        var themePaths = directory
                            .GetDirectories()
                            .Where(dir => dir.GetDirectory("input").Exists)
                            .Select(dir => dir.Path)
                            .ToArray();

                        var themeManager = engine.Services.GetRequiredService<ThemeManager>();

                        foreach (var themePath in themePaths)
                        {
                            themeManager.ThemePaths.Add(themePath);
                        }
                    }
                );

            //bootstrapper.AddThemePath(
            //    @"C:\temp\dev\gh\devlead\devlead.se\src\DevLead\theme\CleanBlog-5c6c7f44f19422b99aadd8f1f0038ce87d8e1afc\");
            return bootstrapper;
        }
    }



    //public class HttpZipPipeline : Pipeline
    //{
    //    public HttpZipPipeline()
    //    {
    //        InputModules = new ModuleList {
    //            new ReadWebZip()
    //        };
    //    }
    //}

    //public class ReadWebZip : Module
    //{
    //    protected override async Task BeforeExecutionAsync(IExecutionContext context)
    //    {
    //        using var clientHandler = new HttpClientHandler();
    //        using var client = context.CreateHttpClient(clientHandler);
    //        await using var responseStream = await client.GetStreamAsync("https://github.com/statiqdev/CleanBlog/archive/5c6c7f44f19422b99aadd8f1f0038ce87d8e1afc.zip");
    //        using var zipStream = new System.IO.Compression.ZipArchive(responseStream, System.IO.Compression.ZipArchiveMode.Read);

    //        var path = context.FileSystem.RootPath.Combine("theme");

    //        var directory = context.FileSystem.GetDirectory(path);

    //        var themeManager = context.GetRequiredService<ThemeManager>();

    //        foreach (var entry in zipStream.Entries.Where(entry => !string.IsNullOrWhiteSpace(entry.Name)))
    //        {
    //            var parentPath = Path.GetDirectoryName(entry.FullName);
    //            var fileDir = string.IsNullOrWhiteSpace(parentPath)
    //                ? directory
    //                : directory.GetDirectory(parentPath);

    //            var file = fileDir.GetFile(entry.Name);
    //            await using var entryStream = entry.Open();
    //            await using var fileStream = file.Open(true);
    //            await entryStream.CopyToAsync(fileStream);
    //        }

    //        themeManager.ThemePaths.AddRange(
    //            directory.GetDirectories().Select(dir => dir.Path)
    //        );
    //    }

    //    protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
    //    {
            

    //        return Enumerable.Empty<IDocument>();

    //        //return zipStream
    //        //.Entries
    //        //.Where(entry => !string.IsNullOrEmpty(entry.Name))
    //        //.Select(entry =>
    //        //{
    //        //    var ms = new MemoryStream();
    //        //    using var es = entry.Open();
    //        //    es.CopyTo(ms);

    //        //    return context.CreateDocument(
    //        //        entry.FullName,
    //        //        ms
    //        //    );
    //        //}).ToList();
    //    }
    //}
}
