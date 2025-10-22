using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TCalc_004
{
    /// <summary>
    /// Servidor HTTP local para expor dados do SimConnect para outras aplicações.
    /// </summary>
    public class SimDataHttpBridge : IDisposable
    {
        private readonly HttpListener _listener;
        private bool _isRunning;
        private SimData _currentSimData; // Armazena os dados mais recentes
        private readonly string _url;

        public SimDataHttpBridge(string url)
        {
            _url = url;
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
            _currentSimData = new SimData(); // Inicializa com dados vazios
        }

        /// <summary>
        /// Atualiza os dados da aeronave que serão servidos.
        /// </summary>
        /// <param name="data">Os dados mais recentes da aeronave.</param>
        public void UpdateSimData(SimData data)
        {
            _currentSimData = data;
        }

        /// <summary>
        /// Inicia o servidor HTTP.
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _listener.Start();
            Console.WriteLine($"SimData Bridge escutando em: {_url}");
            Task.Run(() => Listen()); // Executa o loop de escuta em uma thread separada
        }

        /// <summary>
        /// Para o servidor HTTP.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            _listener.Stop();
            Console.WriteLine("SimData Bridge parado.");
        }

        private async Task Listen()
        {
            while (_isRunning && _listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    string jsonResponse = JsonConvert.SerializeObject(_currentSimData);
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.Close();
                }
                catch (HttpListenerException ex) when (ex.ErrorCode == 995) { /* Operação abortada, listener foi parado */ }
                catch (Exception ex) { Console.WriteLine($"Erro no HttpListener: {ex.Message}"); }
            }
        }

        public void Dispose()
        {
            Stop();
            _listener.Close();
        }
    }
}