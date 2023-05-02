using System.Net;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InstaDummyCallBack;

static class Program
{
    static async Task Main(string[] args)
    {
        // アプリケーションのクライアントID、クライアントシークレット、リダイレクトURLを設定します
        var clientID = Environment.GetEnvironmentVariable("CLIENT_ID");
        // 待受URL(redirect先として渡す＆このURLで待ち受ける）
        var waitUrl = Environment.GetEnvironmentVariable("WAIT_URL");

        // 認証URLを生成します
        var authURL =
            $"https://api.instagram.com/oauth/authorize?client_id={clientID}&redirect_uri={HttpUtility.UrlEncode(waitUrl)}&scope=user_profile,user_media&response_type=code";

        // 認証URLをコンソールに表示します
        Console.WriteLine("Open the following URL in a web browser and authorize the application:\n" + authURL);

        // ダミーの証明書を生成
        var cert = CertificateUtil.CreateSelfSignedCertificate("CN=localhost");
        
        var host = new WebHostBuilder()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, int.Parse(Environment.GetEnvironmentVariable("PORT")), listenOptions =>
                {
                    listenOptions.UseHttps(cert);
                });
            })
            .UseUrls("https://localhost")
            .Configure(app =>
            {
                app.UseMiddleware<InstagramAuthMiddleware>();
            })
            .Build();

        await host.RunAsync();
    }
}