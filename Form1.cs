using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// Add these two statements to all SimConnect clients
using System.Runtime.InteropServices;
using System;
using Microsoft.FlightSimulator.SimConnect;

// Usings para a nova funcionalidade de rádio
using NAudio.Wave;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace TCalc_004
{
    public partial class Form1 : Form
    {
        // --- Variáveis do Rádio ---
        private ComboBox _inputDeviceComboBox;
        private ComboBox _outputDeviceComboBox;
        private CheckBox _loopbackCheckBox;
        private WebSocket _ws;
        private WaveInEvent _waveIn;
        private WaveOutEvent _waveOut;
        private BufferedWaveProvider _playbackBuffer;
        private bool _isPttActive = false;
        private const Keys PTT_KEY = Keys.Space; // Tecla PTT (Espaço)
        private const string RADIO_SERVER_URL = "ws://localhost:8080";

        // --- Dados Atuais da Aeronave ---
        private double _currentLatitude;
        private double _currentLongitude;
        private double _currentAltitude;
        private double _currentCom2Frequency;

        // --- Variáveis Originais ---
        private SimConnect my_simconnect;
        private const int WM_USER_SIMCONNECT = 0x402;

        public Form1()
        {
            // O construtor do formulário é chamado antes do InitializeComponent
            InitializeComponent();

            setButtons(true, false);

            InitializeAudioDeviceControls();

            // Configura a detecção de PTT no formulário
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            if (my_simconnect == null)
            {
                try
                {
                    my_simconnect = new SimConnect("Managed Data Request", base.Handle, WM_USER_SIMCONNECT, null, 0);
                    setButtons(false, true);
                    ConnectRadio(); // Conecta ao servidor de rádio
                    initDataRequest();
                    timer1.Enabled = true;
                }
                catch (COMException)
                {
                    label_status.Text = "Unable to connect to sim";
                }
            }
            else
            {
                label_status.Text = "Error - try again";
                closeConnection();
                setButtons(true, false);
                timer1.Enabled = false;
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            setButtons(true, false);
            timer1.Enabled = false;
        }

        private void closeConnection()
        {
            if (my_simconnect != null)
            {
                my_simconnect.Dispose();
                DisconnectRadio(); // Desconecta do servidor de rádio
                my_simconnect = null;
                label_status.Text = "Connection closed";
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (my_simconnect != null)
                {
                    my_simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        // Simplifica o displayText para apenas atualizar o label_status
        private void displayText(string message)
        {
            label_status.Text = message;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeConnection();
            timer1.Enabled = false;
        }

        private void initDataRequest()
        {
            try
            {
                my_simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                my_simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                my_simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Ground Altitude", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM ACTIVE FREQUENCY:2", "Mhz", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);
                my_simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);
            }
            catch (COMException exception1)
            {
                displayText(exception1.Message);
            }
        }

        private void setButtons(bool bConnect, bool bDisconnect)
        {
            button_Connect.Enabled = bConnect;
            button_Disconnect.Enabled = bDisconnect;
        }

        private void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            label_status.Text = "Exception received: " + ((uint)data.dwException);
        }

        private void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            label_status.Text = "Connected to sim";
        }

        private void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            label_status.Text = "sim has exited";
            closeConnection();
            timer1.Enabled = false;
        }

        private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (data.dwRequestID == 0)
            {
                Struct1 struct1 = (Struct1)data.dwData[0];

                // Atualiza os dados da aeronave para o rádio
                // Estes dados são essenciais para a lógica de rádio (distância, frequência)
                bool dataChanged =
                    _currentLatitude != struct1.latitude ||
                    _currentLongitude != struct1.longitude ||
                    _currentAltitude != struct1.groundaltitude ||
                    _currentCom2Frequency != struct1.com2Frequency;

                _currentLatitude = struct1.latitude;
                _currentLongitude = struct1.longitude;
                _currentAltitude = struct1.groundaltitude;
                _currentCom2Frequency = struct1.com2Frequency;

                // Envia atualização para o servidor de rádio se os dados mudaram
                if (dataChanged) SendDataUpdate();
            }
            else
            {
                label_status.Text = "Unknown request ID: " + ((uint)data.dwRequestID);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            my_simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            // label_status.Text = "Request sent..."; // Opcional: pode poluir a UI
        }

        private enum DATA_REQUESTS
        {
            REQUEST_1
        }

        private enum DEFINITIONS
        {
            Struct1
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Struct1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string title;
            public double latitude;
            public double longitude;
            public double trueheading;
            public double groundaltitude;
            public double com2Frequency;
        }

        // A funcionalidade TopMost será mantida, mas sem o checkbox na UI.
        // Se quiser controlar, pode adicionar um botão ou uma opção no menu.
        // Form1.ActiveForm.TopMost = true; // Pode ser definido no construtor se sempre quiser no topo.

        // ####################################################################
        // ###                    NOVA LÓGICA DE RÁDIO                      ###
        // ####################################################################

        #region Radio Logic

        private void InitializeAudioDeviceControls()
        {
            // Label para o seletor de microfone
            var inputLabel = new Label { Text = "Microfone:", Location = new Point(16, 65), Size = new Size(70, 20) };
            this.Controls.Add(inputLabel);
            // ComboBox para os dispositivos de captura (microfone)
            _inputDeviceComboBox = new ComboBox { Location = new Point(90, 62), Size = new Size(320, 21), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(_inputDeviceComboBox);
            // Label para o seletor de alto-falante
            var outputLabel = new Label { Text = "Alto-falante:", Location = new Point(16, 92), Size = new Size(70, 20) };
            this.Controls.Add(outputLabel);
            // ComboBox para os dispositivos de reprodução (alto-falante)
            _outputDeviceComboBox = new ComboBox { Location = new Point(90, 89), Size = new Size(320, 21), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(_outputDeviceComboBox);
            // CheckBox para o teste de loopback
            _loopbackCheckBox = new CheckBox { Text = "Teste de Loopback (áudio local)", Location = new Point(90, 116), Size = new Size(200, 20), AutoSize = true };
            this.Controls.Add(_loopbackCheckBox);
            PopulateAudioDevices();
        }

        private void ConnectRadio()
        {
            if (_ws != null && _ws.IsAlive) return;

            _ws = new WebSocket(RADIO_SERVER_URL);

            _ws.OnOpen += (sender, e) =>
            {
                this.Invoke((MethodInvoker)delegate { displayText("Rádio: Conectado"); });
                InitializeAudio();
                SendInitialData();
            };

            _ws.OnMessage += Ws_OnMessage;

            _ws.OnClose += (sender, e) =>
            {
                this.Invoke((MethodInvoker)delegate { displayText($"Rádio: Desconectado - {e.Reason}"); });
                CleanupAudio();
            };

            _ws.OnError += (sender, e) =>
            {
                this.Invoke((MethodInvoker)delegate { displayText($"Rádio: Erro - {e.Message}"); });
            };

            _ws.ConnectAsync();
        }

        private void DisconnectRadio()
        {
            _ws?.Close(CloseStatusCode.Normal, "Desconexão manual");
            CleanupAudio();
        }

        private void InitializeAudio()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { InitializeAudio(); });
                return;
            }

            // Garante que os recursos de áudio anteriores sejam limpos
            CleanupAudio();

            if (_inputDeviceComboBox.Items.Count == 0 || _outputDeviceComboBox.Items.Count == 0)
            {
                displayText("Rádio: Nenhum dispositivo de áudio encontrado.");
                return;
            }

            // Configura a captura de áudio (microfone)
            _waveIn = new WaveInEvent
            {
                DeviceNumber = _inputDeviceComboBox.SelectedIndex,
                WaveFormat = new WaveFormat(16000, 16, 1) // 16kHz, 16-bit, Mono
            };
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.StartRecording();

            // Configura a reprodução de áudio (alto-falantes)
            _waveOut = new WaveOutEvent
            {
                DeviceNumber = _outputDeviceComboBox.SelectedIndex
            };
            _playbackBuffer = new BufferedWaveProvider(new WaveFormat(16000, 16, 1));
            _waveOut.Init(_playbackBuffer);
            _waveOut.Play();
        }
        private void PopulateAudioDevices()
        {
            // Popula a lista de microfones
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                _inputDeviceComboBox.Items.Add($"{i}: {deviceInfo.ProductName}");
            }
            if (_inputDeviceComboBox.Items.Count > 0)
            {
                _inputDeviceComboBox.SelectedIndex = 0;
            }
            // Popula a lista de alto-falantes
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var deviceInfo = WaveOut.GetCapabilities(i);
                _outputDeviceComboBox.Items.Add($"{i}: {deviceInfo.ProductName}");
            }
            if (_outputDeviceComboBox.Items.Count > 0)
            {
                _outputDeviceComboBox.SelectedIndex = 0;
            }
        }
        private void CleanupAudio()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _waveIn = null;
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _waveOut = null;
            _playbackBuffer = null;
        }
        private void SendJson(object payload)
        {
            if (_ws != null && _ws.IsAlive)
            {
                _ws.Send(JsonConvert.SerializeObject(payload));
            }
        }
        private void SendInitialData()
        {
            var position = new { lat = _currentLatitude, lon = _currentLongitude, alt = _currentAltitude };
            SendJson(new { type = "init", position, frequency = _currentCom2Frequency });
        }
        private void SendDataUpdate()
        {
            var position = new { lat = _currentLatitude, lon = _currentLongitude, alt = _currentAltitude };
            SendJson(new { type = "update", position, frequency = _currentCom2Frequency });
        }
        // Evento chamado quando o microfone captura áudio
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_loopbackCheckBox.Checked)
            {
                // Modo Loopback: envia o áudio capturado diretamente para o buffer de reprodução.
                _playbackBuffer?.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
            else if (_isPttActive && _ws != null && _ws.IsAlive)
            {
                // Modo Normal: envia o áudio capturado para o servidor via WebSocket.
                string audioBase64 = Convert.ToBase64String(e.Buffer, 0, e.BytesRecorded);
                SendJson(new { type = "audio", audioData = audioBase64 });
            }
        }
        // Evento chamado quando uma mensagem chega do servidor
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                dynamic msg = JsonConvert.DeserializeObject(e.Data);
                if (msg.type == "audio")
                {
                    // Decodifica e processa o áudio recebido
                    byte[] audioBytes = Convert.FromBase64String((string)msg.audioData.audioBuffer);
                    double volumeFactor = (double)msg.audioData.volumeFactor;
                    double noiseIntensity = (double)msg.audioData.noiseIntensity;
                    byte[] processedAudio = ApplyClientSideDegradation(audioBytes, volumeFactor, noiseIntensity);
                    // Adiciona o áudio processado ao buffer de reprodução
                    _playbackBuffer?.AddSamples(processedAudio, 0, processedAudio.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao processar mensagem de áudio: " + ex.Message);
            }
        }
        // Aplica degradação (volume e ruído) ao áudio recebido
        private byte[] ApplyClientSideDegradation(byte[] audioBuffer, double volumeFactor, double noiseIntensity)
        {
            byte[] degradedBuffer = new byte[audioBuffer.Length];
            Array.Copy(audioBuffer, degradedBuffer, audioBuffer.Length);
            var random = new Random();
            int maxNoiseAmplitude = 5000; // Para 16-bit PCM
            for (int i = 0; i < degradedBuffer.Length; i += 2)
            {
                short sample = (short)(degradedBuffer[i] | (degradedBuffer[i + 1] << 8));
                // Aplica volume
                sample = (short)(sample * volumeFactor);
                // Adiciona ruído
                double noise = (random.NextDouble() * 2.0 - 1.0) * maxNoiseAmplitude * noiseIntensity;
                int newSample = sample + (int)noise;
                // Limita o valor para evitar clipping
                if (newSample > short.MaxValue) newSample = short.MaxValue;
                if (newSample < short.MinValue) newSample = short.MinValue;
                sample = (short)newSample;
                degradedBuffer[i] = (byte)(sample & 0xFF);
                degradedBuffer[i + 1] = (byte)(sample >> 8);
            }
            return degradedBuffer;
        }
        // --- Lógica PTT ---
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == PTT_KEY && !_isPttActive)
            {
                _isPttActive = true;
                SendJson(new { type = "ptt_start" });
                // Opcional: Mudar cor de um label para indicar transmissão
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == PTT_KEY && _isPttActive)
            {
                _isPttActive = false;
                SendJson(new { type = "ptt_end" });
                // Opcional: Voltar a cor do label ao normal
            }
        }

        #endregion
    }
}
