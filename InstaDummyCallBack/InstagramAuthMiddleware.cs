using Flurl.Http;
using Microsoft.AspNetCore.Http;

namespace InstaDummyCallBack;

public class InstagramAuthMiddleware
{
    private readonly RequestDelegate _next;

    public InstagramAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/callback/")
        {
            var code = context.Request.Query["code"];
            Console.WriteLine($"code: {code}");
            
            // アクセストークンを取得するためのPOSTデータ
            var postData = new
            {
                code = code.ToString(),
                hash = Environment.GetEnvironmentVariable("INSTA_API_HASH")
            };

            // Instagram APIにPOSTリクエストを送信
            var responseTokenApi = await "http://localhost:8080/v1/instagram/token"
                .SendJsonAsync(HttpMethod.Post, postData)
                .ReceiveString();

            // アクセストークンを表示
            Console.WriteLine($"token: {responseTokenApi}");
            
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("aaa");
        }
        else
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";
            Console.WriteLine($"no code");
            await context.Response.WriteAsync("aaa");
        }
    }
}