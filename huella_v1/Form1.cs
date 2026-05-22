using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using MessagePack;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace huella_v1
{
    public partial class Form1 : Form
    {
        private FingerprintCore fingerPrint;
        private FingerprintRawImage rawImage;
        private FingerprintTemplate _template;
        private ClientWebSocket ws;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private string apiServer = "admision.test";
        private string wsServer = "localhost:3000";
        private string _token_conexion = "";
        private bool _authenticated = false;
        private bool _testMode = false;
        private static readonly HttpClient httpClient = new HttpClient();
        private NotifyIcon notifyIcon;
        private bool _minimizado = false;
        private string _codigo = "";
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private readonly object fpLock = new object();
        private readonly string redisKeyPattern = GetConfig("RedisKeyPattern", "comedor_huella:*");
        private readonly int minimumScoreThreshold = GetIntConfig("MinimumScoreThreshold", 100);

        // Cache auxiliar para pruebas locales; Redis es la fuente principal.
        private Dictionary<string, FingerprintTemplate> templatesDB = new Dictionary<string, FingerprintTemplate>();

        public Form1()
        {
            InitializeComponent();

            ConfigurarBandeja();
            ConfigurarEventosBotones();
            ConnectRedis();
            ActualizarEstadosConexion();

            bool soloPrueba = PedirCodigoOPrueba();

            if (soloPrueba)
            {
                ModoSoloPrueba();
                return;
            }

            if (string.IsNullOrEmpty(_codigo))
            {
                MessageBox.Show("Código requerido.");
                Environment.Exit(0);
            }

            fingerPrint = new FingerprintCore();
            fingerPrint.onStatus += fingerPrint_onStatus;
            fingerPrint.onImage += fingerPrint_onImage;

            _ = LoginWithCodeAndConnect();
        }

        #region Métodos de Identificación y Comparación

        public MatchInfo IdentificarEnRedis(FingerprintTemplate templateBuscar, out int mejorScore, out int totalProcesado)
        {
            mejorScore = 0;
            totalProcesado = 0;
            MatchInfo mejorMatch = null;

            if (!EnsureRedis())
            {
                return null;
            }

            try
            {
                IEnumerable<RedisKey> keys = GetRedisKeys(redisKeyPattern);

                lock (fpLock)
                {
                    fingerPrint.IdentifyPrepare(templateBuscar);
                    LogMessage($"🔍 Buscando huella en Redis ({redisKeyPattern})...");

                    foreach (RedisKey key in keys)
                    {
                        totalProcesado++;

                        try
                        {
                            string value = redisDb.StringGet(key);
                            FingerprintTemplate storedTemplate = LoadTemplate(value);
                            int score;
                            int result = fingerPrint.Identify(storedTemplate, out score);

                            if (result == 1 && score >= minimumScoreThreshold && score > mejorScore)
                            {
                                mejorScore = score;
                                mejorMatch = ParseMatchInfo(key.ToString(), score);
                                LogMessage($"   ✓ Match: {key} (score: {score})");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"   ✗ Redis key inválida {key}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error identificando en Redis: {ex.Message}");
            }

            if (mejorMatch != null)
            {
                LogMessage($"✅ IDENTIFICADO: {mejorMatch.UserId} con score {mejorScore}");
            }
            else
            {
                LogMessage($"❌ No se encontró coincidencia. Procesadas: {totalProcesado}");
            }

            return mejorMatch;
        }

        public string IdentificarEnBaseDeDatos(FingerprintTemplate templateBuscar, out int mejorScore)
        {
            int totalProcesado;
            MatchInfo match = IdentificarEnRedis(templateBuscar, out mejorScore, out totalProcesado);
            return match?.UserId;
        }

        public bool CompararTemplates(FingerprintTemplate template1, FingerprintTemplate template2, out int score)
        {
            score = 0;

            if (!IsValidTemplate(template1) || !IsValidTemplate(template2))
            {
                LogMessage("⚠️ No se puede comparar: template inválido.");
                return false;
            }

            try
            {
                int result;

                lock (fpLock)
                {
                    result = fingerPrint.Verify(template1, template2, out score);
                }

                bool coincide = result == 1 && score >= minimumScoreThreshold;
                LogMessage($"📊 Comparación 1:1 - Score: {score}, Umbral: {minimumScoreThreshold}, Coincide: {coincide}");
                return coincide;
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error en comparación 1:1: {ex.Message}");
                return false;
            }
        }

        public bool CompararHuellas(FingerprintTemplate template1, FingerprintTemplate template2, out int score)
        {
            return CompararTemplates(template1, template2, out score);
        }

        public void RegistrarTemplate(string userId, FingerprintTemplate template)
        {
            RegistrarTemplateRedis(userId, "default", 0, template);
        }

        public bool RegistrarTemplateRedis(string userId, string finger, int fingerprintId, FingerprintTemplate template)
        {
            if (!IsValidTemplate(template))
            {
                LogMessage("⚠️ No se puede registrar: template inválido.");
                return false;
            }

            if (!EnsureRedis())
            {
                return false;
            }

            string key = BuildRedisKey(userId, finger, fingerprintId);
            redisDb.StringSet(key, Convert.ToBase64String(template.Buffer));
            templatesDB[userId] = template;
            LogMessage($"✅ Template registrado en Redis: {key}");
            return true;
        }

        public FingerprintTemplate ObtenerTemplateActual()
        {
            return _template;
        }

        public void EliminarTemplate(string userId)
        {
            if (!EnsureRedis())
            {
                return;
            }

            int deleted = 0;
            foreach (RedisKey key in GetRedisKeys($"{redisKeyPattern.Split(':')[0]}:{userId}:*"))
            {
                if (redisDb.KeyDelete(key))
                {
                    deleted++;
                }
            }

            templatesDB.Remove(userId);
            LogMessage($"🗑️ Templates eliminados para {userId}: {deleted}");
        }

        public int CantidadTemplates()
        {
            if (!EnsureRedis())
            {
                return 0;
            }

            return GetRedisKeys(redisKeyPattern).Count();
        }

        #endregion

        #region Configuración y Conexiones

        private static string GetConfig(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int GetIntConfig(string key, int defaultValue)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) ? value : defaultValue;
        }

        private static bool GetBoolConfig(string key, bool defaultValue)
        {
            bool value;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out value) ? value : defaultValue;
        }

        private void ConnectRedis()
        {
            try
            {
                if (redis != null && redis.IsConnected)
                {
                    return;
                }

                string host = GetConfig("RedisHost", "127.0.0.1");
                int port = GetIntConfig("RedisPort", 6380);
                string password = GetConfig("RedisPassword", "comedor_redis");
                int database = GetIntConfig("RedisDatabase", 0);
                bool ssl = GetBoolConfig("RedisSsl", false);

                ConfigurationOptions options = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    Ssl = ssl,
                    DefaultDatabase = database,
                    ConnectRetry = 3,
                    ConnectTimeout = 5000
                };

                options.EndPoints.Add(host, port);

                if (!string.IsNullOrWhiteSpace(password))
                {
                    options.Password = password;
                }

                redis = ConnectionMultiplexer.Connect(options);
                redisDb = redis.GetDatabase(database);
                LogMessage($"✅ Redis conectado: {host}:{port}, DB {database}, patrón {redisKeyPattern}");
                ActualizarEstadosConexion();
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Redis no conectado: {ex.Message}");
                ActualizarEstadosConexion();
            }
        }

        private bool EnsureRedis()
        {
            if (redisDb != null && redis != null && redis.IsConnected)
            {
                return true;
            }

            ConnectRedis();
            return redisDb != null && redis != null && redis.IsConnected;
        }

        private IEnumerable<RedisKey> GetRedisKeys(string pattern)
        {
            if (!EnsureRedis())
            {
                return Enumerable.Empty<RedisKey>();
            }

            EndPoint endpoint = redis.GetEndPoints().FirstOrDefault();
            if (endpoint == null)
            {
                return Enumerable.Empty<RedisKey>();
            }

            IServer server = redis.GetServer(endpoint);
            return server.Keys(database: redisDb.Database, pattern: pattern).ToList();
        }

        public void MostrarRedisDebug(int limite = 20)
        {
            try
            {
                string host = GetConfig("RedisHost", "127.0.0.1");
                int port = GetIntConfig("RedisPort", 6380);
                int database = GetIntConfig("RedisDatabase", 0);

                LogMessage("========== DEBUG REDIS ==========");
                LogMessage($"Config: {host}:{port}, DB {database}, patrón {redisKeyPattern}");

                if (!EnsureRedis())
                {
                    LogMessage("❌ Redis no conectado. Verifica que el túnel SSH siga abierto y que App.config use puerto 6380.");
                    LogMessage("Comando esperado: ssh -L 6380:172.80.80.113:6379 admision@161.132.24.44");
                    LogMessage("=================================");
                    return;
                }

                TimeSpan ping = redisDb.Ping();
                LogMessage($"PING OK: {ping.TotalMilliseconds:0.##} ms");

                List<RedisKey> keys = GetRedisKeys(redisKeyPattern).ToList();
                LogMessage($"Claves encontradas: {keys.Count}");

                int validas = 0;
                int invalidas = 0;

                foreach (RedisKey key in keys.Take(limite))
                {
                    try
                    {
                        RedisValue value = redisDb.StringGet(key);
                        string templateBase64 = value.HasValue ? value.ToString() : "";
                        FingerprintTemplate template = LoadTemplate(templateBase64);
                        MatchInfo info = ParseMatchInfo(key.ToString(), 0);

                        validas++;
                        LogMessage($"OK {key} | user={info.UserId}, dedo={info.Finger}, id={info.FingerprintId}, base64={templateBase64.Length}, bytes={template.Buffer.Length}");
                    }
                    catch (Exception ex)
                    {
                        invalidas++;
                        LogMessage($"ERROR {key}: {ex.Message}");
                    }
                }

                if (keys.Count > limite)
                {
                    LogMessage($"... mostrando {limite} de {keys.Count} claves.");
                }

                LogMessage($"Resumen muestra: válidas={validas}, inválidas={invalidas}");
                LogMessage("=================================");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error en debug Redis: {ex.Message}");
            }
        }

        #endregion

        #region Utilidades Template y Redis

        private bool IsValidTemplate(FingerprintTemplate template)
        {
            return template != null && template.Buffer != null && template.Buffer.Length > 0 && template.Size > 0;
        }

        private FingerprintTemplate LoadTemplate(string templateData)
        {
            if (string.IsNullOrWhiteSpace(templateData))
            {
                throw new ArgumentException("Template vacío");
            }

            string base64 = templateData.Trim();

            if (base64.StartsWith("{"))
            {
                JObject json = JObject.Parse(base64);
                base64 = (string)(json["template"] ?? json["Template"] ?? json["buffer"] ?? json["Buffer"]);
            }

            byte[] bytes = Convert.FromBase64String(base64);
            return new FingerprintTemplate
            {
                Buffer = bytes,
                Size = bytes.Length
            };
        }

        private string BuildRedisKey(string userId, string finger, int fingerprintId)
        {
            string prefix = redisKeyPattern.Contains(":")
                ? redisKeyPattern.Substring(0, redisKeyPattern.IndexOf(':'))
                : "comedor_huella";

            string safeUserId = string.IsNullOrWhiteSpace(userId) ? "sin_usuario" : userId.Trim();
            string safeFinger = string.IsNullOrWhiteSpace(finger) ? "default" : finger.Trim();

            return $"{prefix}:{safeUserId}:{safeFinger}:{fingerprintId}";
        }

        private MatchInfo ParseMatchInfo(string key, int score)
        {
            string[] parts = key.Split(':');

            return new MatchInfo
            {
                FullKey = key,
                UserId = parts.Length >= 2 ? parts[1] : null,
                Finger = parts.Length >= 3 ? parts[2] : null,
                FingerprintId = parts.Length >= 4 && int.TryParse(parts[3], out int id) ? id : 0,
                Score = score
            };
        }

        #endregion

        #region Extracción de Template

        private FingerprintTemplate ExtractTemplateFromImageFile(string filePath)
        {
            using (Bitmap originalBitmap = new Bitmap(filePath))
            {
                Bitmap bitmap = originalBitmap.PixelFormat == PixelFormat.Format8bppIndexed
                    ? new Bitmap(originalBitmap)
                    : ConvertToGrayscale8bpp(originalBitmap);

                try
                {
                    return ExtractTemplateFromBitmap(bitmap);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }

        private FingerprintTemplate ExtractTemplateFromBitmap(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            try
            {
                FingerprintRawImage image = new FingerprintRawImage(data.Scan0, bitmap.Width, bitmap.Height, 500);
                FingerprintTemplate template = null;

                lock (fpLock)
                {
                    fingerPrint.Extract(image, ref template);
                }

                return template;
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        private Bitmap ConvertToGrayscale8bpp(Bitmap source)
        {
            int width = source.Width;
            int height = source.Height;
            Bitmap grayBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette palette = grayBitmap.Palette;

            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            grayBitmap.Palette = palette;
            BitmapData data = grayBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            try
            {
                int stride = data.Stride;
                byte[] bytes = new byte[stride * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixel = source.GetPixel(x, y);
                        bytes[y * stride + x] = (byte)((pixel.R * 0.299) + (pixel.G * 0.587) + (pixel.B * 0.114));
                    }
                }

                Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            }
            finally
            {
                grayBitmap.UnlockBits(data);
            }

            return grayBitmap;
        }

        public class MatchInfo
        {
            public string UserId { get; set; }
            public string Finger { get; set; }
            public int FingerprintId { get; set; }
            public string FullKey { get; set; }
            public int Score { get; set; }
        }

        #endregion

        #region Configuración UI

        private void ConfigurarEventosBotones()
        {
            this.btnTest.Click += BtnTest_Click;
            this.btnConnect.Click += BtnConnect_Click;
            this.btnIdentify.Click += BtnIdentify_Click;
            this.btnLoad.Click += BtnLoad_Click;
            this.btnRedis.Click += BtnRedis_Click;
            this.btnMinimize.Click += BtnMinimize_Click;

            this.btnDashboard.Click += BtnDashboard_Click;
            this.btnCaptura.Click += BtnCaptura_Click;
            this.btnIdentificacion.Click += BtnIdentificacion_Click;
            this.btnConfiguracion.Click += BtnConfiguracion_Click;
            this.btnLogs.Click += BtnLogs_Click;
        }

        private void ActualizarEstadosConexion()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ActualizarEstadosConexion));
                return;
            }

            bool redisConnected = EnsureRedis();
            bool wsConnected = ws != null && ws.State == WebSocketState.Open;

            lblRedisStatus.Text = redisConnected ? "● Redis: Conectado" : "● Redis: Inactivo";
            lblRedisStatus.ForeColor = redisConnected ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);

            lblWebSocketStatus.Text = wsConnected ? "● WebSocket: Activo" : "● WebSocket: Inactivo";
            lblWebSocketStatus.ForeColor = wsConnected ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);

            lblSensorStatus.Text = _authenticated ? "● Sensor: Listo" : "● Sensor: Desconectado";
            lblSensorStatus.ForeColor = _authenticated ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);
        }

        private void ConfigurarBandeja()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "Lector de Huellas";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Mostrar", null, (s, e) => RestaurarVentana());
            contextMenu.Items.Add("Conectar con Código", null, (s, e) => PedirCodigoParaConectar());
            contextMenu.Items.Add("Salir", null, (s, e) => this.Close());
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        #endregion

        #region Event Handlers - Sidebar

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            LogMessage("📊 Dashboard seleccionado");
            btnDashboard.FillColor = Color.FromArgb(59, 130, 246);
            btnCaptura.FillColor = Color.FromArgb(107, 114, 128);
            btnIdentificacion.FillColor = Color.FromArgb(107, 114, 128);
            btnConfiguracion.FillColor = Color.FromArgb(107, 114, 128);
            btnLogs.FillColor = Color.FromArgb(107, 114, 128);
        }

        private void BtnCaptura_Click(object sender, EventArgs e)
        {
            LogMessage("📸 Modo Captura activado");
            btnDashboard.FillColor = Color.FromArgb(107, 114, 128);
            btnCaptura.FillColor = Color.FromArgb(59, 130, 246);
            btnIdentificacion.FillColor = Color.FromArgb(107, 114, 128);
            btnConfiguracion.FillColor = Color.FromArgb(107, 114, 128);
            btnLogs.FillColor = Color.FromArgb(107, 114, 128);
        }

        private void BtnIdentificacion_Click(object sender, EventArgs e)
        {
            LogMessage("🔍 Identificación activada");
            btnDashboard.FillColor = Color.FromArgb(107, 114, 128);
            btnCaptura.FillColor = Color.FromArgb(107, 114, 128);
            btnIdentificacion.FillColor = Color.FromArgb(59, 130, 246);
            btnConfiguracion.FillColor = Color.FromArgb(107, 114, 128);
            btnLogs.FillColor = Color.FromArgb(107, 114, 128);
        }

        private void BtnConfiguracion_Click(object sender, EventArgs e)
        {
            LogMessage("⚙️ Configuración abierta");
            btnDashboard.FillColor = Color.FromArgb(107, 114, 128);
            btnCaptura.FillColor = Color.FromArgb(107, 114, 128);
            btnIdentificacion.FillColor = Color.FromArgb(107, 114, 128);
            btnConfiguracion.FillColor = Color.FromArgb(59, 130, 246);
            btnLogs.FillColor = Color.FromArgb(107, 114, 128);
        }

        private void BtnLogs_Click(object sender, EventArgs e)
        {
            LogMessage("📋 Panel de Logs seleccionado");
            btnDashboard.FillColor = Color.FromArgb(107, 114, 128);
            btnCaptura.FillColor = Color.FromArgb(107, 114, 128);
            btnIdentificacion.FillColor = Color.FromArgb(107, 114, 128);
            btnConfiguracion.FillColor = Color.FromArgb(107, 114, 128);
            btnLogs.FillColor = Color.FromArgb(59, 130, 246);
        }

        #endregion

        #region Event Handlers - Botones de Acción

        private void BtnIdentify_Click(object sender, EventArgs e)
        {
            if (_template == null)
            {
                LogMessage("⚠️ No hay huella capturada para identificar");
                MessageBox.Show("Capture una huella primero", "Identificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EnsureRedis())
            {
                MessageBox.Show("No hay conexión a Redis. Revise App.config o la conexión a la nube.",
                    "Identificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogMessage("🔍 Iniciando identificación...");
            int score;
            int totalProcesado;
            MatchInfo match = IdentificarEnRedis(_template, out score, out totalProcesado);

            if (match != null)
            {
                MessageBox.Show($"✅ IDENTIFICADO: {match.UserId}\nDedo: {match.Finger}\nScore: {score}\nClave: {match.FullKey}",
                    "Identificación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"❌ No se encontró coincidencia\nProcesadas: {totalProcesado}\n\n" +
                    "Posibles causas:\n" +
                    "- La huella no está registrada\n" +
                    "- La calidad de la huella es baja\n" +
                    "- El dedo no está bien colocado",
                    "Identificación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Cargar huella";
                dialog.Filter = "Huella o template|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.txt;*.b64|Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff|Template base64|*.txt;*.b64|Todos los archivos|*.*";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    string extension = Path.GetExtension(dialog.FileName).ToLowerInvariant();

                    if (extension == ".txt" || extension == ".b64")
                    {
                        _template = LoadTemplate(File.ReadAllText(dialog.FileName));
                    }
                    else
                    {
                        _template = ExtractTemplateFromImageFile(dialog.FileName);
                    }

                    if (!IsValidTemplate(_template))
                    {
                        LogMessage("⚠️ No se pudo cargar una plantilla válida.");
                        MessageBox.Show("No se pudo cargar una plantilla válida.", "Cargar huella", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    LogMessage($"✅ Huella cargada: {Path.GetFileName(dialog.FileName)} ({_template.Buffer.Length} bytes)");
                    MessageBox.Show("Huella cargada correctamente. Ahora puede identificar contra Redis.", "Cargar huella", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ Error cargando huella: {ex.Message}");
                    MessageBox.Show($"Error cargando huella: {ex.Message}", "Cargar huella", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRedis_Click(object sender, EventArgs e)
        {
            MostrarRedisDebug(20);
        }

        #endregion

        #region Bandeja de Sistema

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RestaurarVentana();
        }

        private void RestaurarVentana()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            _minimizado = false;
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            _minimizado = true;
            this.Hide();
            notifyIcon.ShowBalloonTip(1000, "Lector de Huellas", "La aplicación sigue funcionando en segundo plano", ToolTipIcon.Info);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            PedirCodigoParaConectar();
        }

        private void PedirCodigoParaConectar()
        {
            using (var form = new Form())
            {
                form.Text = "Conectar con Código";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Size = new Size(350, 150);

                Label lblInfo = new Label()
                {
                    Text = "Ingrese el código de conexión:",
                    Left = 20,
                    Top = 20,
                    Width = 310,
                    Font = new Font("Arial", 9)
                };

                TextBox txtCodigo = new TextBox() { Left = 20, Top = 55, Width = 220, Font = new Font("Arial", 12) };

                Button btnOk = new Button() { Text = "Conectar", Left = 250, Top = 53, Width = 80, DialogResult = DialogResult.OK };

                form.Controls.AddRange(new Control[] { lblInfo, txtCodigo, btnOk });
                form.AcceptButton = btnOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string codigo = txtCodigo.Text.Trim();
                    if (!string.IsNullOrEmpty(codigo))
                    {
                        _codigo = codigo;
                        _testMode = false;
                        _authenticated = false;

                        fingerPrint = new FingerprintCore();
                        fingerPrint.onStatus += fingerPrint_onStatus;
                        fingerPrint.onImage += fingerPrint_onImage;

                        _ = LoginWithCodeAndConnect();
                    }
                }
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (_testMode)
            {
                _testMode = false;
                btnTest.FillColor = Color.FromArgb(59, 130, 246);
                btnTest.Text = "🔍 Prueba";
                LogMessage("--- Modo prueba desactivado ---");

                if (_authenticated)
                {
                    fingerPrint.StartCapture("test");
                    LogMessage("Volviendo a modo normal...");
                }
            }
            else
            {
                _testMode = true;
                btnTest.FillColor = Color.FromArgb(16, 185, 129);
                btnTest.Text = "🔬 Prueba Activa";
                LogMessage("=== MODO PRUEBA ACTIVADO ===");
                LogMessage("Ponga su dedo en el lector para probar");

                try
                {
                    fingerPrint.Initialize();
                    fingerPrint.CaptureInitialize();
                    fingerPrint.StartCapture("test");
                    LogMessage("Lector inicializado en modo prueba.");
                }
                catch (Exception ex)
                {
                    LogMessage($"Error al iniciar lector: {ex.Message}");
                }
            }
        }

        private bool PedirCodigoOPrueba()
        {
            using (var form = new Form())
            {
                form.Text = "Lector de Huellas";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Size = new Size(350, 180);

                Label lblInfo = new Label()
                {
                    Text = "Ingrese el código que recibió en su correo:",
                    Left = 20,
                    Top = 20,
                    Width = 310,
                    Font = new Font("Arial", 9)
                };

                TextBox txtCodigo = new TextBox() { Left = 20, Top = 55, Width = 220, Font = new Font("Arial", 12) };

                Button btnOk = new Button() { Text = "Conectar", Left = 250, Top = 53, Width = 80, DialogResult = DialogResult.OK };

                Button btnTestOnly = new Button()
                {
                    Text = "Solo Probar Lector",
                    Left = 20,
                    Top = 100,
                    Width = 310,
                    Height = 35,
                    BackColor = Color.LightGray
                };

                btnTestOnly.Click += (s, e) =>
                {
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                };

                form.Controls.AddRange(new Control[] { lblInfo, txtCodigo, btnOk, btnTestOnly });
                form.AcceptButton = btnOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    _codigo = txtCodigo.Text.Trim();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void ModoSoloPrueba()
        {
            _testMode = true;
            LogMessage("=== MODO SOLO PRUEBA ===");
            LogMessage("Probando lector sin conexión al servidor");

            fingerPrint = new FingerprintCore();
            fingerPrint.onStatus += fingerPrint_onStatus;
            fingerPrint.onImage += fingerPrint_onImage;

            try
            {
                fingerPrint.Initialize();
                fingerPrint.CaptureInitialize();
                fingerPrint.StartCapture("test");
                LogMessage("Lector activado. Ponga su dedo para probar.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
        }

        #endregion

        #region WebSocket y Conectividad

        private async Task LoginWithCodeAndConnect()
        {
            try
            {
                LogMessage($"Validando código: {_codigo}");

                string url = $"https://{apiServer}/api/get-codigo-conexion/{_codigo}";
                LogMessage($"URL: {url}");

                HttpResponseMessage response = await httpClient.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();

                LogMessage($"Respuesta del servidor: {responseBody}");

                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                if (jsonResponse != null && jsonResponse.ContainsKey("status") && (bool)jsonResponse["status"] == true)
                {
                    _token_conexion = jsonResponse["codigo_conexion"].ToString();
                    LogMessage($"Código válido. Token recibido.");

                    await ConnectWebSocketWithToken();
                    ActualizarEstadosConexion();

                    if (_minimizado == false)
                    {
                        DialogResult result = MessageBox.Show("Conectado exitosamente.\n\n¿Desea minimizar la aplicación a segundo plano?",
                            "Conectado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            BtnMinimize_Click(null, null);
                        }
                    }
                }
                else
                {
                    string error = jsonResponse.ContainsKey("error") ? jsonResponse["error"].ToString() : "Código inválido";
                    LogMessage($"Error: {error}");
                    MessageBox.Show($"Error: {error}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error de conexión: {ex.Message}");
                MessageBox.Show($"Error de conexión: {ex.Message}\n\nVerifique que el servidor esté accesible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ConnectWebSocketWithToken()
        {
            ws = new ClientWebSocket();
            string wsUrl = $"ws://{wsServer}/websocket?token_conexion={HttpUtility.UrlEncode(_token_conexion)}";

            LogMessage($"Conectando a WebSocket: {wsUrl}");

            try
            {
                await ws.ConnectAsync(new Uri(wsUrl), CancellationToken.None);
                LogMessage("✅ Conectado al WebSocket");

                await SendIdentification();
                LogMessage("Esperando frontend...");

                _ = MonitorWebSocketConnection();
                _ = ReceiveMessageAsync();
            }
            catch (Exception ex)
            {
                LogMessage($"Error WebSocket: {ex.Message}");
                await ReconnectWebSocket();
            }
        }

        private async Task SendIdentification()
        {
            try
            {
                var identification = new
                {
                    type = "fingerprint"
                };

                string jsonId = JsonConvert.SerializeObject(identification);
                byte[] idBytes = Encoding.UTF8.GetBytes(jsonId);

                await SafeSendAsync(idBytes, WebSocketMessageType.Text);
                LogMessage("✅ Identificación enviada al servidor (APP)");
            }
            catch (Exception ex)
            {
                LogMessage($"Error al enviar identificación: {ex.Message}");
            }
        }

        private async Task MonitorWebSocketConnection()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (ws != null && ws.State != WebSocketState.Open)
                {
                    LogMessage("Conexión perdida, reconectando...");
                    await ReconnectWebSocket();
                }
                ActualizarEstadosConexion();
                await Task.Delay(10000);
            }
        }

        private async Task ReconnectWebSocket()
        {
            int retryCount = 0;
            int maxRetries = 5;

            while (retryCount < maxRetries && (ws == null || ws.State != WebSocketState.Open))
            {
                try
                {
                    if (ws != null) ws.Dispose();
                    ws = new ClientWebSocket();
                    string wsUrl = $"ws://{wsServer}/websocket?token_conexion={HttpUtility.UrlEncode(_token_conexion)}";
                    await ws.ConnectAsync(new Uri(wsUrl), CancellationToken.None);
                    LogMessage("Reconectado exitosamente.");
                    await SendIdentification();
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogMessage($"Reconexión fallida ({retryCount}/5): {ex.Message}");
                    await Task.Delay(2000);
                }
            }
        }

        private async Task ReceiveMessageAsync()
        {
            var buffer = new byte[4096];

            try
            {
                while (ws != null && ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        ProcessWebSocketMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        LogMessage("Conexión cerrada por el servidor.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error en recepción: {ex.Message}");
            }
        }

        private void ProcessWebSocketMessage(string message)
        {
            try
            {
                JObject jsonData = JObject.Parse(message);

                if (jsonData != null)
                {
                    string type = (string)jsonData["type"];
                    string action = (string)jsonData["action"];

                    if (type == "frontend_conectado")
                    {
                        _authenticated = true;
                        LogMessage("✅ Frontend conectado. Activando lector...");
                        if (!_testMode)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => {
                                    fingerPrint.Initialize();
                                    fingerPrint.CaptureInitialize();
                                    fingerPrint.StartCapture("ws");
                                }));
                            }
                            else
                            {
                                fingerPrint.Initialize();
                                fingerPrint.CaptureInitialize();
                                fingerPrint.StartCapture("ws");
                            }
                        }
                        LogMessage("Lector listo. Puede poner su dedo.");
                        ActualizarEstadosConexion();

                        if (_minimizado)
                        {
                            notifyIcon.ShowBalloonTip(2000, "Frontend Conectado", "El frontend se ha conectado. El lector está listo.", ToolTipIcon.Info);
                        }
                    }

                    if (type == "request_capture")
                    {
                        LogMessage("📸 Frontend solicitó captura; la próxima huella se comparará contra Redis.");
                    }

                    if (action == "identify" || action == "identify_1n")
                    {
                        FingerprintTemplate template = GetTemplateFromMessageOrCurrent(jsonData);
                        int score;
                        int processed;
                        MatchInfo match = IdentificarEnRedis(template, out score, out processed);

                        _ = SendJsonToWebSocketAsync(new
                        {
                            type = "fingerprint_result",
                            action = "identify",
                            found = match != null,
                            match = match,
                            score = score,
                            threshold = minimumScoreThreshold,
                            totalProcessed = processed
                        });
                    }

                    if (action == "compare" || action == "compare_1_1")
                    {
                        FingerprintTemplate template1 = GetTemplateFromMessageOrCurrent(jsonData);
                        FingerprintTemplate template2 = LoadTemplate((string)(jsonData["template2"] ?? jsonData["Template2"]));
                        int score;
                        bool match = CompararTemplates(template1, template2, out score);

                        _ = SendJsonToWebSocketAsync(new
                        {
                            type = "fingerprint_result",
                            action = "compare",
                            match = match,
                            score = score,
                            threshold = minimumScoreThreshold
                        });
                    }

                    if (action == "close")
                    {
                        LogMessage("Cerrando aplicación...");
                        this.Invoke(new Action(() => this.Close()));
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error al procesar mensaje: {ex.Message}");
            }
        }

        private FingerprintTemplate GetTemplateFromMessageOrCurrent(JObject jsonData)
        {
            string template = (string)(jsonData["template"] ?? jsonData["Template"]);

            if (!string.IsNullOrWhiteSpace(template))
            {
                return LoadTemplate(template);
            }

            if (IsValidTemplate(_template))
            {
                return _template;
            }

            throw new InvalidOperationException("No hay template en el mensaje ni huella capturada.");
        }

        #endregion

        #region Eventos Sensor Biométrico

        private async void fingerPrint_onImage(object source, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
        {
            rawImage = ie.RawImage;
            SetImage(rawImage.Image);
            ExtractTemplate();

            if (_template != null)
            {
                try
                {
                    byte[] imageBytes = ConvertImageToBytes(rawImage.Image);

                    if (_testMode)
                    {
                        LogMessage("🔬 [PRUEBA] Huella detectada correctamente!");
                        LogMessage($"   - Calidad: {_template.Quality}");
                        LogMessage($"   - Plantilla: {Convert.ToBase64String(_template.Buffer).Length} bytes");
                        LogMessage($"   - Imagen: {imageBytes.Length} bytes");

                        using (var form = new Form())
                        {
                            form.Text = "Prueba - Huella Detectada";
                            form.Size = new Size(400, 300);
                            form.StartPosition = FormStartPosition.CenterScreen;
                            form.FormBorderStyle = FormBorderStyle.FixedDialog;

                            PictureBox pb = new PictureBox();
                            pb.Image = rawImage.Image;
                            pb.SizeMode = PictureBoxSizeMode.Zoom;
                            pb.Dock = DockStyle.Fill;

                            Button btnOk = new Button() { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 40 };

                            form.Controls.Add(pb);
                            form.Controls.Add(btnOk);
                            form.ShowDialog();
                        }
                    }
                    else if (_authenticated)
                    {
                        int redisScore = 0;
                        int redisProcessed = 0;
                        MatchInfo redisMatch = null;

                        if (EnsureRedis())
                        {
                            redisMatch = IdentificarEnRedis(_template, out redisScore, out redisProcessed);
                        }
                        else
                        {
                            LogMessage("⚠️ Captura recibida, pero Redis no está conectado para identificar.");
                        }

                        var data = new
                        {
                            type = "fingerprint",
                            template = Convert.ToBase64String(_template.Buffer),
                            image = Convert.ToBase64String(imageBytes),
                            quality = _template.Quality,
                            redis_found = redisMatch != null,
                            redis_user_id = redisMatch?.UserId,
                            redis_finger = redisMatch?.Finger,
                            redis_fingerprint_id = redisMatch?.FingerprintId,
                            redis_key = redisMatch?.FullKey,
                            redis_score = redisScore,
                            redis_threshold = minimumScoreThreshold,
                            redis_total_processed = redisProcessed,
                            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        byte[] packedData = MessagePackSerializer.Serialize(data);
                        await SafeSendAsync(packedData);
                        await SendJsonToWebSocketAsync(new
                        {
                            type = "fingerprint_result",
                            action = "identify",
                            found = redisMatch != null,
                            match = redisMatch,
                            score = redisScore,
                            threshold = minimumScoreThreshold,
                            totalProcessed = redisProcessed
                        });

                        LogMessage($"✅ Huella enviada y comparada con Redis (Calidad: {_template.Quality}, Match: {redisMatch?.FullKey ?? "no"})");

                        if (_minimizado)
                        {
                            notifyIcon.ShowBalloonTip(1000, "Huella Enviada", "Se ha capturado y enviado una huella", ToolTipIcon.Info);
                        }
                    }
                    else
                    {
                        LogMessage("Esperando conexión del frontend... Huella guardada localmente.");
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error: {ex.Message}");
                }
            }
        }

        private byte[] ConvertImageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Task SendJsonToWebSocketAsync(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            return SafeSendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text);
        }

        private async Task SafeSendAsync(byte[] data, WebSocketMessageType messageType = WebSocketMessageType.Binary)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (ws != null && ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(data), messageType, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error al enviar: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ExtractTemplate()
        {
            if (rawImage != null)
            {
                try
                {
                    lock (fpLock)
                    {
                        fingerPrint.Extract(rawImage, ref _template);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error al extraer plantilla: {ex.Message}");
                }
            }
        }

        private delegate void DelSetImage(Image img);

        private void SetImage(Image img)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelSetImage(SetImage), img);
            }
            else
            {
                if (pbxHuella.Image != null)
                {
                    pbxHuella.Image.Dispose();
                }
                pbxHuella.Image = img;
            }
        }

        private void fingerPrint_onStatus(object source, GriauleFingerprintLibrary.Events.StatusEventArgs se)
        {
            if (se.StatusEventType == GriauleFingerprintLibrary.Events.StatusEventType.SENSOR_PLUG)
            {
                if (_testMode)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => {
                            fingerPrint.StartCapture(source.ToString());
                            LogMessage("🔬 Sensor conectado en modo prueba.");
                            ActualizarEstadosConexion();
                        }));
                    }
                    else
                    {
                        fingerPrint.StartCapture(source.ToString());
                        LogMessage("🔬 Sensor conectado en modo prueba.");
                        ActualizarEstadosConexion();
                    }
                }
                else if (_authenticated)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => {
                            fingerPrint.StartCapture(source.ToString());
                            LogMessage("Sensor conectado. Listo para capturar.");
                            ActualizarEstadosConexion();
                        }));
                    }
                    else
                    {
                        fingerPrint.StartCapture(source.ToString());
                        LogMessage("Sensor conectado. Listo para capturar.");
                        ActualizarEstadosConexion();
                    }
                }
                else
                {
                    LogMessage("Sensor conectado. Esperando frontend...");
                    ActualizarEstadosConexion();
                }
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        fingerPrint.StopCapture(source);
                        LogMessage("Sensor desconectado.");
                        ActualizarEstadosConexion();
                    }));
                }
                else
                {
                    fingerPrint.StopCapture(source);
                    LogMessage("Sensor desconectado.");
                    ActualizarEstadosConexion();
                }
            }
        }

        #endregion

        #region Cierre y Limpieza

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Form1_FormClosed(this, e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            if (ws != null)
            {
                try
                {
                    if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseSent)
                    {
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando", CancellationToken.None).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error al cerrar: {ex.Message}");
                }
                finally
                {
                    ws.Dispose();
                }
            }
            if (fingerPrint != null)
            {
                try
                {
                    fingerPrint.CaptureFinalize();
                    fingerPrint.Finalizer();
                }
                catch (Exception ex)
                {
                    LogMessage($"Error al finalizar: {ex.Message}");
                }
            }
            redis?.Dispose();
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                richTextBox1.AppendText($"{DateTime.Now:HH:mm:ss}: {message}{Environment.NewLine}");
                richTextBox1.ScrollToCaret();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) { }

        #endregion
    }
}
