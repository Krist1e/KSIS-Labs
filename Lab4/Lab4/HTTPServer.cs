using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab4;

internal class HTTPServer
{
    private readonly FileManager _fileManager;
    private readonly TcpListener _listener;

    private readonly ConcurrentDictionary<Task, Task> _tasks = new();

    private readonly object _lockObject = new();

    public HTTPServer(IPEndPoint endPoint, string root)
    {
        _listener = new TcpListener(endPoint);
        _fileManager = new FileManager(root);
    }

    public async Task Start()
    {
        _listener.Start();
        try
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var task = HandleClientAsync(client);
                _tasks.TryAdd(task, task);
                var unused = task.ContinueWith(t => _tasks.TryRemove(t, out _));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            await Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var connection = new HTTPConnection(client.GetStream(), _lockObject);

        try
        {
            var request = await connection.ReceiveAsync();
            var response = await HandleRequest(request);
            await connection.SendAsync(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            client.Close();
        }
    }

    private async Task<HTTPResponse> HandleRequest(HTTPRequest request)
    {
        var response = new HTTPResponse();
        response.AddHeader("Date", $"{DateTime.Now:R}");
        response.AddHeader("Server", "MyServer");
        if (request.Method == HTTPRequest.HTTPRequestMethod.GET)
        {
            if (!_fileManager.FileExists(request.Uri == "/" ? "/index2.html" : request.Uri))
            {
                response.StatusCode = 404;
                response.Version = "HTTP/1.0";
                response.StatusMessage = "Not Found";
                return response;
            }
            
            response.Body = await _fileManager.ReadFileAsync(request.Uri == "/" ? "/index2.html" : request.Uri);
            response.StatusCode = 200;
            response.Version = "HTTP/1.0";
            response.StatusMessage = "OK";
        }
        else if (request.Method == HTTPRequest.HTTPRequestMethod.POST)
        {
            response.Body = Encoding.UTF8.GetBytes($"<html><body>{Encoding.UTF8.GetString(request.Body.ToArray())}</body></html>");
            response.StatusCode = 200;
            response.Version = "HTTP/1.0";
            response.StatusMessage = "OK";
        }
        else if (request.Method == HTTPRequest.HTTPRequestMethod.HEAD)
        {
            if (!_fileManager.FileExists(request.Uri == "/" ? "/index2.html" : request.Uri))
            {
                response.StatusCode = 404;
                response.Version = "HTTP/1.0";
                response.StatusMessage = "Not Found";
                return response;
            }
            response.StatusCode = 200;
            response.Version = "HTTP/1.0";
            response.StatusMessage = "OK";
        }
        else 
        {
            response.StatusCode = 405;
            response.Version = "HTTP/1.0";
            response.StatusMessage = "Method Not Allowed";
        }

        return response;
    }

    public async Task Stop()
    {
        _listener.Stop();

        await Task.WhenAll(_tasks.Keys);
    }
}