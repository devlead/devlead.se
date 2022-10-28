using System.Collections.Generic;
using System.Threading.Tasks;
using Statiq.Common;

// ReSharper disable once UnusedType.Global
public sealed class Sponsorship : Shortcode
{
    public override Task<ShortcodeResult> ExecuteAsync(
        KeyValuePair<string, string>[] args,
        string content,
        IDocument document,
        IExecutionContext context
    )
    {
        return Task.FromResult(new ShortcodeResult(
            //lang=html
            """
            <div class="sponsorship">
                <a class="btn btn-lg btn-outline-primary btn-block" href="https://github.com/sponsors/devlead" title="Sponsor Mattias Karlsson on GitHub">
                    <i class="fa fa-heart"></i>
                    Sponsor Mattias
                </a>
            </div>
            """
        ));
    }
}