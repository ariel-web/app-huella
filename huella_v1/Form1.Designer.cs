namespace huella_v1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // TOPBAR - Logo y Estado
            this.pnlTopbar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRedisStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblWebSocketStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblSensorStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();

            // SIDEBAR - Navegación
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.btnDashboard = new Guna.UI2.WinForms.Guna2Button();
            this.btnCaptura = new Guna.UI2.WinForms.Guna2Button();
            this.btnIdentificacion = new Guna.UI2.WinForms.Guna2Button();
            this.btnConfiguracion = new Guna.UI2.WinForms.Guna2Button();
            this.btnLogs = new Guna.UI2.WinForms.Guna2Button();

            // MAIN CONTENT AREA
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();

            // PANEL CENTRAL - Información biométrica
            this.pnlCentral = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.pbxFoto = new Guna.UI2.WinForms.Guna2PictureBox();
            this.pbxHuella = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblBioTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblScore = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCalidad = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblEstadoId = new Guna.UI2.WinForms.Guna2HtmlLabel();

            // PANEL INFERIOR - Información del usuario
            this.pnlInfoUsuario = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.lblDNI = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblNombre = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCarrera = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblAula = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPabellon = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPosicion = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPiso = new Guna.UI2.WinForms.Guna2HtmlLabel();

            // PANEL LOGS - Consola moderna
            this.pnlLogs = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();

            // BOTONES DE ACCIÓN
            this.btnTest = new Guna.UI2.WinForms.Guna2Button();
            this.btnConnect = new Guna.UI2.WinForms.Guna2Button();
            this.btnIdentify = new Guna.UI2.WinForms.Guna2Button();
            this.btnLoad = new Guna.UI2.WinForms.Guna2Button();
            this.btnRedis = new Guna.UI2.WinForms.Guna2Button();
            this.btnMinimize = new Guna.UI2.WinForms.Guna2Button();

            // CONTAINER DE BOTONES
            this.pnlButtonContainer = new Guna.UI2.WinForms.Guna2Panel();

            this.SuspendLayout();

            // ==================== TOPBAR ====================
            this.pnlTopbar.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlTopbar.BorderRadius = 0;
            this.pnlTopbar.Controls.Add(this.lblTitle);
            this.pnlTopbar.Controls.Add(this.lblRedisStatus);
            this.pnlTopbar.Controls.Add(this.lblWebSocketStatus);
            this.pnlTopbar.Controls.Add(this.lblSensorStatus);
            this.pnlTopbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopbar.FillColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlTopbar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopbar.Name = "pnlTopbar";
            this.pnlTopbar.Size = new System.Drawing.Size(1400, 70);
            this.pnlTopbar.TabIndex = 0;

            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔐 Sistema Biométrico Institucional";

            this.lblRedisStatus.AutoSize = false;
            this.lblRedisStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblRedisStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRedisStatus.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblRedisStatus.Location = new System.Drawing.Point(900, 20);
            this.lblRedisStatus.Name = "lblRedisStatus";
            this.lblRedisStatus.Size = new System.Drawing.Size(120, 30);
            this.lblRedisStatus.TabIndex = 1;
            this.lblRedisStatus.Text = "● Redis: Inactivo";

            this.lblWebSocketStatus.AutoSize = false;
            this.lblWebSocketStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblWebSocketStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWebSocketStatus.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblWebSocketStatus.Location = new System.Drawing.Point(1050, 20);
            this.lblWebSocketStatus.Name = "lblWebSocketStatus";
            this.lblWebSocketStatus.Size = new System.Drawing.Size(130, 30);
            this.lblWebSocketStatus.TabIndex = 2;
            this.lblWebSocketStatus.Text = "● WebSocket: Inactivo";

            this.lblSensorStatus.AutoSize = false;
            this.lblSensorStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblSensorStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSensorStatus.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblSensorStatus.Location = new System.Drawing.Point(1210, 20);
            this.lblSensorStatus.Name = "lblSensorStatus";
            this.lblSensorStatus.Size = new System.Drawing.Size(120, 30);
            this.lblSensorStatus.TabIndex = 3;
            this.lblSensorStatus.Text = "● Sensor: Desconectado";

            // ==================== SIDEBAR ====================
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlSidebar.BorderRadius = 0;
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.btnCaptura);
            this.pnlSidebar.Controls.Add(this.btnIdentificacion);
            this.pnlSidebar.Controls.Add(this.btnConfiguracion);
            this.pnlSidebar.Controls.Add(this.btnLogs);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlSidebar.Location = new System.Drawing.Point(0, 70);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(200, 730);
            this.pnlSidebar.TabIndex = 1;

            // Botones del Sidebar
            this.btnDashboard.BorderRadius = 10;
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDashboard.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDashboard.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnDashboard.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnDashboard.FillColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(10, 20);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(180, 45);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.Click += new System.EventHandler(this.BtnDashboard_Click);

            this.btnCaptura.BorderRadius = 10;
            this.btnCaptura.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCaptura.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCaptura.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCaptura.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnCaptura.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnCaptura.FillColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.btnCaptura.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCaptura.ForeColor = System.Drawing.Color.White;
            this.btnCaptura.Location = new System.Drawing.Point(10, 75);
            this.btnCaptura.Name = "btnCaptura";
            this.btnCaptura.Size = new System.Drawing.Size(180, 45);
            this.btnCaptura.TabIndex = 1;
            this.btnCaptura.Text = "📸 Captura";
            this.btnCaptura.Click += new System.EventHandler(this.BtnCaptura_Click);

            this.btnIdentificacion.BorderRadius = 10;
            this.btnIdentificacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIdentificacion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnIdentificacion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnIdentificacion.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnIdentificacion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnIdentificacion.FillColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.btnIdentificacion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnIdentificacion.ForeColor = System.Drawing.Color.White;
            this.btnIdentificacion.Location = new System.Drawing.Point(10, 130);
            this.btnIdentificacion.Name = "btnIdentificacion";
            this.btnIdentificacion.Size = new System.Drawing.Size(180, 45);
            this.btnIdentificacion.TabIndex = 2;
            this.btnIdentificacion.Text = "🔍 Identificación";
            this.btnIdentificacion.Click += new System.EventHandler(this.BtnIdentificacion_Click);

            this.btnConfiguracion.BorderRadius = 10;
            this.btnConfiguracion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfiguracion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnConfiguracion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnConfiguracion.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnConfiguracion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnConfiguracion.FillColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.btnConfiguracion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfiguracion.ForeColor = System.Drawing.Color.White;
            this.btnConfiguracion.Location = new System.Drawing.Point(10, 185);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(180, 45);
            this.btnConfiguracion.TabIndex = 3;
            this.btnConfiguracion.Text = "⚙️ Configuración";
            this.btnConfiguracion.Click += new System.EventHandler(this.BtnConfiguracion_Click);

            this.btnLogs.BorderRadius = 10;
            this.btnLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogs.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLogs.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLogs.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnLogs.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnLogs.FillColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.btnLogs.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogs.ForeColor = System.Drawing.Color.White;
            this.btnLogs.Location = new System.Drawing.Point(10, 240);
            this.btnLogs.Name = "btnLogs";
            this.btnLogs.Size = new System.Drawing.Size(180, 45);
            this.btnLogs.TabIndex = 4;
            this.btnLogs.Text = "📋 Logs";
            this.btnLogs.Click += new System.EventHandler(this.BtnLogs_Click);

            // ==================== MAIN PANEL ====================
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlMain.BorderRadius = 0;
            this.pnlMain.Controls.Add(this.pnlLogs);
            this.pnlMain.Controls.Add(this.pnlInfoUsuario);
            this.pnlMain.Controls.Add(this.pnlCentral);
            this.pnlMain.Controls.Add(this.pnlButtonContainer);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlMain.Location = new System.Drawing.Point(200, 70);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1200, 730);
            this.pnlMain.TabIndex = 2;

            // ==================== PANEL CENTRAL ====================
            this.pnlCentral.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlCentral.BorderRadius = 15;
            this.pnlCentral.Controls.Add(this.pbxFoto);
            this.pnlCentral.Controls.Add(this.pbxHuella);
            this.pnlCentral.Controls.Add(this.lblBioTitle);
            this.pnlCentral.Controls.Add(this.lblScore);
            this.pnlCentral.Controls.Add(this.lblCalidad);
            this.pnlCentral.Controls.Add(this.lblEstadoId);
            this.pnlCentral.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlCentral.Location = new System.Drawing.Point(20, 20);
            this.pnlCentral.Name = "pnlCentral";
            this.pnlCentral.ShadowColor = System.Drawing.Color.Black;
            this.pnlCentral.Size = new System.Drawing.Size(550, 400);
            this.pnlCentral.TabIndex = 0;

            this.lblBioTitle.AutoSize = false;
            this.lblBioTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblBioTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBioTitle.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblBioTitle.Location = new System.Drawing.Point(15, 15);
            this.lblBioTitle.Name = "lblBioTitle";
            this.lblBioTitle.Size = new System.Drawing.Size(520, 30);
            this.lblBioTitle.TabIndex = 0;
            this.lblBioTitle.Text = "📊 Información Biométrica";

            this.pbxFoto.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.pbxFoto.BorderRadius = 10;
            this.pbxFoto.ImageRotate = 0F;
            this.pbxFoto.Location = new System.Drawing.Point(15, 50);
            this.pbxFoto.Name = "pbxFoto";
            this.pbxFoto.Size = new System.Drawing.Size(150, 150);
            this.pbxFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxFoto.TabIndex = 1;
            this.pbxFoto.TabStop = false;

            this.pbxHuella.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.pbxHuella.BorderRadius = 10;
            this.pbxHuella.ImageRotate = 0F;
            this.pbxHuella.Location = new System.Drawing.Point(380, 50);
            this.pbxHuella.Name = "pbxHuella";
            this.pbxHuella.Size = new System.Drawing.Size(150, 150);
            this.pbxHuella.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxHuella.TabIndex = 2;
            this.pbxHuella.TabStop = false;

            this.lblScore.AutoSize = false;
            this.lblScore.BackColor = System.Drawing.Color.Transparent;
            this.lblScore.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblScore.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.lblScore.Location = new System.Drawing.Point(15, 210);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(250, 40);
            this.lblScore.TabIndex = 3;
            this.lblScore.Text = "🎯 Score: --";

            this.lblCalidad.AutoSize = false;
            this.lblCalidad.BackColor = System.Drawing.Color.Transparent;
            this.lblCalidad.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCalidad.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.lblCalidad.Location = new System.Drawing.Point(15, 255);
            this.lblCalidad.Name = "lblCalidad";
            this.lblCalidad.Size = new System.Drawing.Size(250, 40);
            this.lblCalidad.TabIndex = 4;
            this.lblCalidad.Text = "✓ Calidad: --";

            this.lblEstadoId.AutoSize = false;
            this.lblEstadoId.BackColor = System.Drawing.Color.Transparent;
            this.lblEstadoId.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEstadoId.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.lblEstadoId.Location = new System.Drawing.Point(15, 300);
            this.lblEstadoId.Name = "lblEstadoId";
            this.lblEstadoId.Size = new System.Drawing.Size(250, 40);
            this.lblEstadoId.TabIndex = 5;
            this.lblEstadoId.Text = "📌 Estado: Esperando captura";

            // ==================== PANEL INFO USUARIO ====================
            this.pnlInfoUsuario.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlInfoUsuario.BorderRadius = 15;
            this.pnlInfoUsuario.Controls.Add(this.lblDNI);
            this.pnlInfoUsuario.Controls.Add(this.lblNombre);
            this.pnlInfoUsuario.Controls.Add(this.lblCarrera);
            this.pnlInfoUsuario.Controls.Add(this.lblAula);
            this.pnlInfoUsuario.Controls.Add(this.lblPabellon);
            this.pnlInfoUsuario.Controls.Add(this.lblPosicion);
            this.pnlInfoUsuario.Controls.Add(this.lblPiso);
            this.pnlInfoUsuario.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlInfoUsuario.Location = new System.Drawing.Point(590, 20);
            this.pnlInfoUsuario.Name = "pnlInfoUsuario";
            this.pnlInfoUsuario.ShadowColor = System.Drawing.Color.Black;
            this.pnlInfoUsuario.Size = new System.Drawing.Size(590, 400);
            this.pnlInfoUsuario.TabIndex = 1;

            this.lblDNI.AutoSize = false;
            this.lblDNI.BackColor = System.Drawing.Color.Transparent;
            this.lblDNI.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDNI.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblDNI.Location = new System.Drawing.Point(20, 30);
            this.lblDNI.Name = "lblDNI";
            this.lblDNI.Size = new System.Drawing.Size(550, 25);
            this.lblDNI.TabIndex = 0;
            this.lblDNI.Text = "📝 DNI: --";

            this.lblNombre.AutoSize = false;
            this.lblNombre.BackColor = System.Drawing.Color.Transparent;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNombre.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblNombre.Location = new System.Drawing.Point(20, 65);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(550, 25);
            this.lblNombre.TabIndex = 1;
            this.lblNombre.Text = "👤 Nombre: --";

            this.lblCarrera.AutoSize = false;
            this.lblCarrera.BackColor = System.Drawing.Color.Transparent;
            this.lblCarrera.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCarrera.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblCarrera.Location = new System.Drawing.Point(20, 100);
            this.lblCarrera.Name = "lblCarrera";
            this.lblCarrera.Size = new System.Drawing.Size(550, 25);
            this.lblCarrera.TabIndex = 2;
            this.lblCarrera.Text = "🎓 Carrera: --";

            this.lblAula.AutoSize = false;
            this.lblAula.BackColor = System.Drawing.Color.Transparent;
            this.lblAula.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAula.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblAula.Location = new System.Drawing.Point(20, 135);
            this.lblAula.Name = "lblAula";
            this.lblAula.Size = new System.Drawing.Size(250, 25);
            this.lblAula.TabIndex = 3;
            this.lblAula.Text = "🏫 Aula: --";

            this.lblPabellon.AutoSize = false;
            this.lblPabellon.BackColor = System.Drawing.Color.Transparent;
            this.lblPabellon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPabellon.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblPabellon.Location = new System.Drawing.Point(320, 135);
            this.lblPabellon.Name = "lblPabellon";
            this.lblPabellon.Size = new System.Drawing.Size(250, 25);
            this.lblPabellon.TabIndex = 4;
            this.lblPabellon.Text = "🏢 Pabellón: --";

            this.lblPosicion.AutoSize = false;
            this.lblPosicion.BackColor = System.Drawing.Color.Transparent;
            this.lblPosicion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPosicion.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblPosicion.Location = new System.Drawing.Point(20, 170);
            this.lblPosicion.Name = "lblPosicion";
            this.lblPosicion.Size = new System.Drawing.Size(250, 25);
            this.lblPosicion.TabIndex = 5;
            this.lblPosicion.Text = "📍 Posición: --";

            this.lblPiso.AutoSize = false;
            this.lblPiso.BackColor = System.Drawing.Color.Transparent;
            this.lblPiso.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPiso.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            this.lblPiso.Location = new System.Drawing.Point(320, 170);
            this.lblPiso.Name = "lblPiso";
            this.lblPiso.Size = new System.Drawing.Size(250, 25);
            this.lblPiso.TabIndex = 6;
            this.lblPiso.Text = "🏗️ Piso: --";

            // ==================== PANEL LOGS ====================
            this.pnlLogs.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlLogs.BorderRadius = 15;
            this.pnlLogs.Controls.Add(this.richTextBox1);
            this.pnlLogs.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlLogs.Location = new System.Drawing.Point(20, 430);
            this.pnlLogs.Name = "pnlLogs";
            this.pnlLogs.ShadowColor = System.Drawing.Color.Black;
            this.pnlLogs.Size = new System.Drawing.Size(1160, 250);
            this.pnlLogs.TabIndex = 2;

            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = false;
            this.richTextBox1.Size = new System.Drawing.Size(1160, 250);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);

            // ==================== BUTTON CONTAINER ====================
            this.pnlButtonContainer.BackColor = System.Drawing.Color.Transparent;
            this.pnlButtonContainer.BorderRadius = 0;
            this.pnlButtonContainer.Controls.Add(this.btnTest);
            this.pnlButtonContainer.Controls.Add(this.btnConnect);
            this.pnlButtonContainer.Controls.Add(this.btnIdentify);
            this.pnlButtonContainer.Controls.Add(this.btnLoad);
            this.pnlButtonContainer.Controls.Add(this.btnRedis);
            this.pnlButtonContainer.Controls.Add(this.btnMinimize);
            this.pnlButtonContainer.FillColor = System.Drawing.Color.Transparent;
            this.pnlButtonContainer.Location = new System.Drawing.Point(20, 690);
            this.pnlButtonContainer.Name = "pnlButtonContainer";
            this.pnlButtonContainer.Size = new System.Drawing.Size(1160, 20);
            this.pnlButtonContainer.TabIndex = 3;
            this.pnlButtonContainer.Visible = false;

            // Botones de Acción (ocultos por defecto, se controlan por lógica)
            this.btnTest.BorderRadius = 8;
            this.btnTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTest.FillColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnTest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTest.ForeColor = System.Drawing.Color.White;
            this.btnTest.Location = new System.Drawing.Point(0, 0);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 35);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "🔍 Prueba";
            this.btnTest.Visible = false;

            this.btnConnect.BorderRadius = 8;
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.FillColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(110, 0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 35);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "🔌 Conectar";
            this.btnConnect.Visible = false;

            this.btnIdentify.BorderRadius = 8;
            this.btnIdentify.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIdentify.FillColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnIdentify.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnIdentify.ForeColor = System.Drawing.Color.White;
            this.btnIdentify.Location = new System.Drawing.Point(240, 0);
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new System.Drawing.Size(120, 35);
            this.btnIdentify.TabIndex = 2;
            this.btnIdentify.Text = "🔎 Identificar";
            this.btnIdentify.Visible = false;

            this.btnLoad.BorderRadius = 8;
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.FillColor = System.Drawing.Color.FromArgb(139, 92, 246);
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(370, 0);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(110, 35);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "📂 Cargar";
            this.btnLoad.Visible = false;

            this.btnRedis.BorderRadius = 8;
            this.btnRedis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRedis.FillColor = System.Drawing.Color.FromArgb(244, 63, 94);
            this.btnRedis.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRedis.ForeColor = System.Drawing.Color.White;
            this.btnRedis.Location = new System.Drawing.Point(490, 0);
            this.btnRedis.Name = "btnRedis";
            this.btnRedis.Size = new System.Drawing.Size(110, 35);
            this.btnRedis.TabIndex = 4;
            this.btnRedis.Text = "🧪 Redis";
            this.btnRedis.Visible = false;

            this.btnMinimize.BorderRadius = 8;
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.FillColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(610, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(110, 35);
            this.btnMinimize.TabIndex = 5;
            this.btnMinimize.Text = "⬇️ Minimizar";
            this.btnMinimize.Visible = false;

            // ==================== FORM1 ====================
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlTopbar);
            this.ForeColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1400, 850);
            this.MinimumSize = new System.Drawing.Size(1400, 800);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema Biométrico Institucional";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel pnlTopbar;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRedisStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblWebSocketStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSensorStatus;

        private Guna.UI2.WinForms.Guna2Panel pnlSidebar;
        private Guna.UI2.WinForms.Guna2Button btnDashboard;
        private Guna.UI2.WinForms.Guna2Button btnCaptura;
        private Guna.UI2.WinForms.Guna2Button btnIdentificacion;
        private Guna.UI2.WinForms.Guna2Button btnConfiguracion;
        private Guna.UI2.WinForms.Guna2Button btnLogs;

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2ShadowPanel pnlCentral;
        private Guna.UI2.WinForms.Guna2PictureBox pbxFoto;
        private Guna.UI2.WinForms.Guna2PictureBox pbxHuella;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBioTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblScore;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCalidad;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEstadoId;

        private Guna.UI2.WinForms.Guna2ShadowPanel pnlInfoUsuario;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDNI;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNombre;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCarrera;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAula;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPabellon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPosicion;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPiso;

        private Guna.UI2.WinForms.Guna2ShadowPanel pnlLogs;
        private System.Windows.Forms.RichTextBox richTextBox1;

        private Guna.UI2.WinForms.Guna2Panel pnlButtonContainer;
        private Guna.UI2.WinForms.Guna2Button btnTest;
        private Guna.UI2.WinForms.Guna2Button btnConnect;
        private Guna.UI2.WinForms.Guna2Button btnIdentify;
        private Guna.UI2.WinForms.Guna2Button btnLoad;
        private Guna.UI2.WinForms.Guna2Button btnRedis;
        private Guna.UI2.WinForms.Guna2Button btnMinimize;
    }
}
